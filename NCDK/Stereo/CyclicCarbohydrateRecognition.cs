/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *   
 * Contact: cdk-devel@lists.sourceforge.net
 *   
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above 
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using NCDK.Common.Collections;
using NCDK.Graphs;
using NCDK.Numerics;
using NCDK.RingSearches;
using System;
using System.Collections.Generic;
using static NCDK.Graphs.GraphUtil;

namespace NCDK.Stereo
{
    /// <summary>
    /// Recognise stereochemistry of Haworth, Chair, and Boat (not yet implemented)
    /// projections. These projections are a common way of depicting closed-chain
    /// (furanose and pyranose) carbohydrates and require special treatment to
    /// interpret stereo conformation. 
    /// <para>
    /// The methods used are described by <token>cdk-cite-batchelor13</token>. 
    /// </para>
    /// </summary>
    /// <seealso href="http://en.wikipedia.org/wiki/Haworth_projection">Haworth projection (Wikipedia)</seealso>
    /// <seealso href="http://en.wikipedia.org/wiki/Chair_conformation">Chair conformation (Wikipedia)</seealso>
    // @author John May
    // @cdk.githash
    internal sealed class CyclicCarbohydrateRecognition
    {
        /// <summary>
        /// The threshold at which to snap bonds to the cardinal direction. The
        /// threshold allows bonds slightly of absolute directions to be interpreted.
        /// The tested vector is of unit length and so the threshold is simply the
        /// angle (in radians).
        /// </summary>
        public const double CardinalityThreshold = 5.0 / 180 * Math.PI;

        public const double QuartCardinalityThreshold = CardinalityThreshold / 4;

        private readonly IAtomContainer container;
        private readonly int[][] graph;
        private readonly EdgeToBondMap bonds;
        private readonly Stereocenters stereocenters;

        /// <summary>
        /// Required information to recognise stereochemistry.
        /// </summary>
        /// <param name="container">input structure</param>
        /// <param name="graph">adjacency list representation</param>
        /// <param name="bonds">edge to bond index</param>
        /// <param name="stereocenters">location and type of asymmetries</param>
        public CyclicCarbohydrateRecognition(IAtomContainer container,
                                      int[][] graph,
                                      EdgeToBondMap bonds,
                                      Stereocenters stereocenters)
        {
            this.container = container;
            this.graph = graph;
            this.bonds = bonds;
            this.stereocenters = stereocenters;
        }

        /// <summary>
        /// Recognise the cyclic carbohydrate projections.
        /// </summary>
        /// <param name="projections">the types of projections to recognise</param>
        /// <returns>recognised stereocenters</returns>
        public IList<IReadOnlyStereoElement<IChemObject, IChemObject>> Recognise(ICollection<Projection> projections)
        {
            if (!projections.Contains(Projection.Haworth) && !projections.Contains(Projection.Chair))
                return Array.Empty<IReadOnlyStereoElement<IChemObject, IChemObject>>();

            var elements = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>();

            RingSearch ringSearch = new RingSearch(container, graph);
            foreach (var isolated in ringSearch.Isolated())
            {
                if (isolated.Length < 5 || isolated.Length > 7)
                    continue;

                int[] cycle = Arrays.CopyOf(GraphUtil.Cycle(graph, isolated),
                                            isolated.Length);

                Vector2[] points = CoordinatesOfCycle(cycle, container);
                Turns[] turns = GetTurns(points);
                WoundProjection projection = WoundProjection.OfTurns(turns);

                if (!projections.Contains(projection.Projection))
                    continue;

                // ring is not aligned correctly for haworth
                if (projection.Projection == Projection.Haworth && !CheckHaworthAlignment(points))
                    continue;

                Vector2 horizontalXy = HorizontalOffset(points, turns, projection.Projection);

                // near vertical, should also flag as potentially ambiguous 
                if (1 - Math.Abs(horizontalXy.Y) < QuartCardinalityThreshold)
                    continue;

                int[] above = (int[])cycle.Clone();
                int[] below = (int[])cycle.Clone();

                if (!AssignSubstituents(cycle, above, below, projection, horizontalXy))
                    continue;

                elements.AddRange(NewTetrahedralCenters(cycle, above, below, projection));
            }

            return elements;
        }

        /// <summary>
        /// Determine the turns in the polygon formed of the provided coordinates.
        /// </summary>
        /// <param name="points">polygon points</param>
        /// <returns>array of turns (left, right) or null if a parallel line was found</returns>
        public static Turns[] GetTurns(Vector2[] points)
        {
            Turns[] turns = new Turns[points.Length];

            // cycle of size 6 is [1,2,3,4,5,6] not closed
            for (int i = 1; i <= points.Length; i++)
            {
                Vector2 prevXy = points[i - 1];
                Vector2 currXy = points[i % points.Length];
                Vector2 nextXy = points[(i + 1) % points.Length];
                int parity = (int)Math.Sign(Det(prevXy.X, prevXy.Y,
                                                   currXy.X, currXy.Y,
                                                   nextXy.X, nextXy.Y));
                if (parity == 0) return null;
                turns[i % points.Length] = parity < 0 ? Turns.Right : Turns.Left;
            }

            return turns;
        }

        /// <summary>
        /// Given a projected cycle, assign the exocyclic substituents to being above
        /// of below the projection. For Haworth projections, the substituents must
        /// be directly up or down (within some threshold).
        /// </summary>
        /// <param name="cycle">vertices that form a cycle</param>
        /// <param name="above">vertices that will be above the cycle (filled by method)</param>
        /// <param name="below">vertices that will be below the cycle (filled by method)</param>
        /// <param name="projection">the type of projection</param>
        /// <param name="horizontalXy">offset from the horizontal axis</param>
        /// <returns>assignment okay (true), not okay (false)</returns>
        private bool AssignSubstituents(int[] cycle,
                                           int[] above,
                                           int[] below,
                                           WoundProjection projection,
                                           Vector2 horizontalXy)
        {
            bool haworth = projection.Projection == Projection.Haworth;

            int found = 0;

            for (int i = 1; i <= cycle.Length; i++)
            {

                int j = i % cycle.Length;

                int prev = cycle[i - 1];
                int curr = cycle[j];
                int next = cycle[(i + 1) % cycle.Length];

                // get the substituents not in the ring (i.e. excl. prev and next)
                int[] ws = Filter(graph[curr], prev, next);

                if (ws.Length > 2 || ws.Length < 1)
                    continue;

                Vector2 centerXy = container.Atoms[curr].Point2D.Value;

                // determine the direction of each substituent 
                foreach (var w in ws)
                {
                    Vector2 otherXy = container.Atoms[w].Point2D.Value;
                    Direction direction = ObtainDirection(centerXy, otherXy, horizontalXy, haworth);

                    switch (direction)
                    {
                        case Direction.Up:
                            if (above[j] != curr) return false;
                            above[j] = w;
                            break;
                        case Direction.Down:
                            if (below[j] != curr) return false;
                            below[j] = w;
                            break;
                        case Direction.Other:
                            return false;
                    }
                }

                if (above[j] != curr || below[j] != curr)
                    found++;
            }

            // must have at least 2 that look projected for Haworth
            return found > 1 || projection.Projection != Projection.Haworth;
        }

        /// <summary>
        /// Create the tetrahedral stereocenters for the provided cycle.
        /// </summary>
        /// <param name="cycle">vertices in projected cycle</param>
        /// <param name="above">vertices above the cycle</param>
        /// <param name="below">vertices below the cycle</param>
        /// <param name="type">type of projection</param>
        /// <returns>zero of more stereocenters</returns>
        private IList<ITetrahedralChirality> NewTetrahedralCenters(int[] cycle, int[] above, int[] below, WoundProjection type)
        {
            List<ITetrahedralChirality> centers = new List<ITetrahedralChirality>(cycle.Length);

            for (int i = 1; i <= cycle.Length; i++)
            {
                int prev = cycle[i - 1];
                int curr = cycle[i % cycle.Length];
                int next = cycle[(i + 1) % cycle.Length];

                int up = above[i % cycle.Length];
                int down = below[i % cycle.Length];

                if (!stereocenters.IsStereocenter(curr))
                    continue;

                // Any wedge or hatch bond causes us to exit, this may still be
                // a valid projection. Currently it can cause a collision with
                // one atom have two tetrahedral stereo elements. 
                if (!IsPlanarSigmaBond(bonds[curr, prev])
                        || !IsPlanarSigmaBond(bonds[curr, next])
                        || (up != curr && !IsPlanarSigmaBond(bonds[curr, up]))
                        || (down != curr && !IsPlanarSigmaBond(bonds[curr, down])))
                    return Array.Empty<ITetrahedralChirality>();

                centers.Add(new TetrahedralChirality(container.Atoms[curr],
                                                     new IAtom[]{container.Atoms[up],
                                                                 container.Atoms[prev],
                                                                 container.Atoms[down],
                                                                 container.Atoms[next]},
                                                     type.Winding
                ));
            }

            return centers;
        }

        /// <summary>
        /// Obtain the coordinates of atoms in a cycle.
        /// </summary>
        /// <param name="cycle">vertices that form a cycles</param>
        /// <param name="container">structure representation</param>
        /// <returns>coordinates of the cycle</returns>
        private static Vector2[] CoordinatesOfCycle(int[] cycle, IAtomContainer container)
        {
            Vector2[] points = new Vector2[cycle.Length];
            for (int i = 0; i < cycle.Length; i++)
            {
                points[i] = container.Atoms[cycle[i]].Point2D.Value;
            }
            return points;
        }

        /// <summary>
        /// Filter an array, excluding two provided values. These values must be
        /// present in the input.
        /// </summary>
        /// <param name="org">input array</param>
        /// <param name="skip1">skip this item</param>
        /// <param name="skip2">skip this item also</param>
        /// <returns>array without skip1 and skip2</returns>
        private static int[] Filter(int[] org, int skip1, int skip2)
        {
            int n = 0;
            int[] dest = new int[org.Length - 2];
            foreach (var w in org)
            {
                if (w != skip1 && w != skip2) dest[n++] = w;
            }
            return dest;
        }

        /// <summary>
        /// Obtain the direction of a substituent relative to the center location. In
        /// a Haworth projection the substituent must be directly above or below
        /// (with threshold) the center.
        /// </summary>
        /// <param name="centerXy">location of center</param>
        /// <param name="substituentXy">location fo substituent</param>
        /// <param name="horizontalXy">horizontal offset, x > 0</param>
        /// <param name="haworth">is Haworth project (substituent must be directly up or down)</param>
        /// <returns>the direction (up, down, other)</returns>
        private static Direction ObtainDirection(Vector2 centerXy, Vector2 substituentXy, Vector2 horizontalXy, bool haworth)
        {
            double deltaX = substituentXy.X - centerXy.X;
            double deltaY = substituentXy.Y - centerXy.Y;

            // normalise vector length so threshold is independent of length 
            double mag = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            deltaX /= mag;
            deltaY /= mag;

            // account for an offset horizontal reference and re-normalise,
            // we presume no vertical chairs and use the deltaX +ve or -ve to
            // determine direction, the horizontal offset should be deltaX > 0.
            if (deltaX > 0)
            {
                deltaX -= horizontalXy.X;
                deltaY -= horizontalXy.Y;
            }
            else
            {
                deltaX += horizontalXy.X;
                deltaY += horizontalXy.Y;
            }
            mag = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            deltaX /= mag;
            deltaY /= mag;

            if (haworth && Math.Abs(deltaX) > CardinalityThreshold)
                return Direction.Other;

            return deltaY > 0 ? Direction.Up : Direction.Down;
        }

        /// <summary>
        /// Ensures at least one cyclic bond is horizontal.
        /// </summary>
        /// <param name="points">the points of atoms in the ring</param>
        /// <returns>whether the Haworth alignment is correct</returns>
        private bool CheckHaworthAlignment(Vector2[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 curr = points[i];
                Vector2 next = points[(i + 1) % points.Length];

                double deltaY = curr.Y - next.Y;

                if (Math.Abs(deltaY) < CardinalityThreshold)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determine the horizontal offset of the projection. This allows
        /// projections that are drawn at angle to be correctly interpreted. 
        /// Currently only projections of chair conformations are considered.
        /// </summary>
        /// <param name="points">points of the cycle</param>
        /// <param name="turns">the turns in the cycle (left/right)</param>
        /// <param name="projection">the type of projection</param>
        /// <returns>the horizontal offset</returns>
        private Vector2 HorizontalOffset(Vector2[] points, Turns[] turns, Projection projection)
        {
            // Haworth must currently be drawn vertically, I have seen them drawn
            // slanted but it's difficult to determine which way the projection
            // is relative
            if (projection != Projection.Chair)
                return Vector2.Zero;

            // the atoms either side of a central atom are our reference
            int offset = ChairCenterOffset(turns);
            int prev = (offset + 5) % 6;
            int next = (offset + 7) % 6;

            // and the axis formed by these atoms is our horizontal reference which
            // we normalise
            double deltaX = points[prev].X - points[next].X;
            double deltaY = points[prev].Y - points[next].Y;
            double mag = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            deltaX /= mag;
            deltaY /= mag;

            // we now ensure the reference always points left to right (presumes no
            // vertical chairs) 
            if (deltaX < 0)
            {
                deltaX = -deltaX;
                deltaY = -deltaY;
            }

            // horizontal = <1,0> so the offset if the difference from this 
            return new Vector2((1 - deltaX), deltaY);
        }

        /// <summary>
        /// Determines the center index offset for the chair projection. The center
        /// index is that of the two atoms with opposite turns (fewest). For, LLRLLR
        /// the two centers are R and the index is 2 (first is in position 2). 
        /// </summary>
        /// <param name="turns">calculated turns in the chair projection</param>
        /// <returns>the offset</returns>
        private static int ChairCenterOffset(Turns[] turns)
        {
            if (turns[1] == turns[2])
            {
                return 0;
            }
            else if (turns[0] == turns[2])
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        // 3x3 determinant helper for a constant third column
        private static double Det(double xa, double ya, double xb, double yb, double xc, double yc)
        {
            return (xa - xc) * (yb - yc) - (ya - yc) * (xb - xc);
        }

        /// <summary>
        /// Helper method determines if a bond is defined (not null) and whether it
        /// is a sigma (single) bond with no stereo attribute (wedge/hatch).
        /// </summary>
        /// <param name="bond">the bond to test</param>
        /// <returns>the bond is a planar sigma bond</returns>
        private static bool IsPlanarSigmaBond(IBond bond)
        {
            return bond != null &&
                    BondOrder.Single.Equals(bond.Order) &&
                    BondStereo.None.Equals(bond.Stereo);
        }

        /// <summary>
        /// Direction of substituent relative to ring atom.
        /// </summary>
        enum Direction
        {
            Up,
            Down,
            Other
        }

        /// <summary>
        /// Turns, recorded when walking around the cycle.
        /// </summary>
        public enum Turns
        {
            Left,
            Right
        }

        /// <summary>
        /// Pairing of Projection + Winding. The wound projection is determined
        /// from an array of turns.
        /// </summary>
        private class WoundProjection
        {
            public static readonly WoundProjection HaworthClockwise = new WoundProjection(Projection.Haworth, TetrahedralStereo.Clockwise);
            public static readonly WoundProjection HaworthAnticlockwise = new WoundProjection(Projection.Haworth, TetrahedralStereo.AntiClockwise);
            public static readonly WoundProjection ChairClockwise = new WoundProjection(Projection.Chair, TetrahedralStereo.Clockwise);
            public static readonly WoundProjection ChairAnticlockwise = new WoundProjection(Projection.Chair, TetrahedralStereo.AntiClockwise);
            public static readonly WoundProjection BoatClockwise = new WoundProjection(Projection.Unset, TetrahedralStereo.Clockwise);
            public static readonly WoundProjection BoatAnticlockwise = new WoundProjection(Projection.Unset, TetrahedralStereo.AntiClockwise);
            public static readonly WoundProjection Other = new WoundProjection(Projection.Unset, TetrahedralStereo.Unset);

            public Projection Projection { get; private set; }
            public TetrahedralStereo Winding { get; private set; }
            private readonly static IDictionary<Key, WoundProjection> map;

            static WoundProjection()
            {
                map = new Dictionary<Key, WoundProjection>();

                // Haworth |V| = 5
                map.Add(new Key(Turns.Left, Turns.Left, Turns.Left, Turns.Left, Turns.Left), HaworthAnticlockwise);
                map.Add(new Key(Turns.Right, Turns.Right, Turns.Right, Turns.Right, Turns.Right), HaworthClockwise);

                // Haworth |V| = 6
                map.Add(new Key(Turns.Left, Turns.Left, Turns.Left, Turns.Left, Turns.Left, Turns.Left), HaworthAnticlockwise);
                map.Add(new Key(Turns.Right, Turns.Right, Turns.Right, Turns.Right, Turns.Right, Turns.Right), HaworthClockwise);

                // Haworth |V| = 7
                map.Add(new Key(Turns.Left, Turns.Left, Turns.Left, Turns.Left, Turns.Left, Turns.Left, Turns.Left), HaworthAnticlockwise);
                map.Add(new Key(Turns.Right, Turns.Right, Turns.Right, Turns.Right, Turns.Right, Turns.Right, Turns.Right), HaworthClockwise);

                // Chair
                map.Add(new Key(Turns.Left, Turns.Right, Turns.Right, Turns.Left, Turns.Right, Turns.Right), ChairClockwise);
                map.Add(new Key(Turns.Right, Turns.Left, Turns.Right, Turns.Right, Turns.Left, Turns.Right), ChairClockwise);
                map.Add(new Key(Turns.Right, Turns.Right, Turns.Left, Turns.Right, Turns.Right, Turns.Left), ChairClockwise);
                map.Add(new Key(Turns.Right, Turns.Left, Turns.Left, Turns.Right, Turns.Left, Turns.Left), ChairAnticlockwise);
                map.Add(new Key(Turns.Left, Turns.Right, Turns.Left, Turns.Left, Turns.Right, Turns.Left), ChairAnticlockwise);
                map.Add(new Key(Turns.Left, Turns.Left, Turns.Right, Turns.Left, Turns.Left, Turns.Right), ChairAnticlockwise);

                // Boat
                map.Add(new Key(Turns.Right, Turns.Right, Turns.Left, Turns.Left, Turns.Left, Turns.Left), BoatAnticlockwise);
                map.Add(new Key(Turns.Right, Turns.Left, Turns.Left, Turns.Left, Turns.Left, Turns.Right), BoatAnticlockwise);
                map.Add(new Key(Turns.Left, Turns.Left, Turns.Left, Turns.Left, Turns.Right, Turns.Right), BoatAnticlockwise);
                map.Add(new Key(Turns.Left, Turns.Left, Turns.Left, Turns.Right, Turns.Right, Turns.Left), BoatAnticlockwise);
                map.Add(new Key(Turns.Left, Turns.Left, Turns.Right, Turns.Right, Turns.Left, Turns.Left), BoatAnticlockwise);
                map.Add(new Key(Turns.Left, Turns.Right, Turns.Right, Turns.Left, Turns.Left, Turns.Left), BoatAnticlockwise);
                map.Add(new Key(Turns.Left, Turns.Left, Turns.Right, Turns.Right, Turns.Right, Turns.Right), BoatClockwise);
                map.Add(new Key(Turns.Left, Turns.Right, Turns.Right, Turns.Right, Turns.Right, Turns.Left), BoatClockwise);
                map.Add(new Key(Turns.Right, Turns.Right, Turns.Right, Turns.Right, Turns.Left, Turns.Left), BoatClockwise);
                map.Add(new Key(Turns.Right, Turns.Right, Turns.Right, Turns.Left, Turns.Left, Turns.Right), BoatClockwise);
                map.Add(new Key(Turns.Right, Turns.Right, Turns.Left, Turns.Left, Turns.Right, Turns.Right), BoatClockwise);
                map.Add(new Key(Turns.Right, Turns.Left, Turns.Left, Turns.Right, Turns.Right, Turns.Right), BoatClockwise);
            }

            public WoundProjection(Projection projection, TetrahedralStereo winding)
            {
                this.Projection = projection;
                this.Winding = winding;
            }

            public static WoundProjection OfTurns(Turns[] turns)
            {
                if (turns == null) return WoundProjection.Other;
                WoundProjection type;
                if (map.TryGetValue(new Key(turns), out type))
                    return type;
                return Other;
            }

            private sealed class Key
            {
                private readonly Turns[] turns;

                internal Key(params Turns[] turns)
                {
                    this.turns = turns;
                }


                public override bool Equals(object o)
                {
                    Key key = o as Key;
                    if (key == null) return false;
                    return Arrays.AreEqual(turns, key.turns);
                }

                public override int GetHashCode()
                {
                    return turns != null ? Arrays.GetHashCode(turns) : 0;
                }
            }
        }
    }
}
