/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Common.Mathematics;
using NCDK.Geometries;
using NCDK.Graphs;
using NCDK.Graphs.Matrix;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NCDK.Numerics;

namespace NCDK.Layout
{
    /// <summary>
    ///  Methods for generating coordinates for atoms in various situations. They can
    ///  be used for Automated Structure Diagram Generation or in the interactive
    ///  buildup of molecules by the user.
    /// </summary>
    ///@author      steinbeck
    ///@cdk.created 2003-08-29
    ///@cdk.module  sdg
    // @cdk.githash
    public class AtomPlacer
    {
        public const bool debug = true;
        public const string PRIORITY = "Weight";

        /// <summary>
        ///  Constructor for the AtomPlacer object
        /// </summary>
        public AtomPlacer() { }

        /// <summary>
        /// The molecule the AtomPlacer currently works with
        /// </summary>
        public IAtomContainer Molecule { get; set; } = null;

        /// <summary>
        ///  Distribute the bonded atoms (neighbours) of an atom such that they fill the
        ///  remaining space around an atom in a geometrically nice way.
        ///  IMPORTANT: This method is not supposed to handle the
        ///  case of one or no place neighbor. In the case of
        ///  one placed neigbor, the chain placement methods
        ///  should be used.
        /// </summary>
        /// <param name="atom">The atom whose partners are to be placed</param>
        /// <param name="placedNeighbours">The atoms which are already placed</param>
        /// <param name="unplacedNeighbours">The partners to be placed</param>
        /// <param name="bondLength">The standared bond length for the newly placed Atoms</param>
        /// <param name="sharedAtomsCenter">The 2D centre of the placed Atoms</param>
        public void DistributePartners(IAtom atom, IAtomContainer placedNeighbours, Vector2 sharedAtomsCenter,
                IAtomContainer unplacedNeighbours, double bondLength)
        {
            double occupiedAngle = 0;
            //double smallestDistance = double.MaxValue;
            //IAtom[] nearestAtoms = new IAtom[2];
            IAtom[] sortedAtoms = null;
            double startAngle = 0.0;
            double addAngle = 0.0;
            double radius = 0.0;
            double remainingAngle = 0.0;
            // calculate the direction away from the already placed partners of atom
            //Vector2 sharedAtomsCenter = sharedAtoms.Get2DCenter();
            Vector2 sharedAtomsCenterVector = sharedAtomsCenter;

            Vector2 newDirection = atom.Point2D.Value;
            Vector2 occupiedDirection = sharedAtomsCenter;
            occupiedDirection = Vector2.Subtract(occupiedDirection, newDirection);
            Debug.WriteLine("distributePartners->occupiedDirection.Lenght(): " + occupiedDirection.Length());
            List<IAtom> atomsToDraw = new List<IAtom>();

            Debug.WriteLine("Number of shared atoms: ", placedNeighbours.Atoms.Count);

            // IMPORTANT: This method is not supposed to handle the case of one or
            // no place neighbor. In the case of one placed neigbor, the chain
            // placement methods should be used.
            if (placedNeighbours.Atoms.Count == 1)
            {
                Debug.WriteLine("Only one neighbour...");
                for (int f = 0; f < unplacedNeighbours.Atoms.Count; f++)
                {
                    atomsToDraw.Add(unplacedNeighbours.Atoms[f]);
                }

                addAngle = Math.PI * 2 / (unplacedNeighbours.Atoms.Count + placedNeighbours.Atoms.Count);
                // IMPORTANT: At this point we need a calculation of the start
                // angle. Not done yet.
                IAtom placedAtom = placedNeighbours.Atoms[0];
                //            double xDiff = atom.GetX2d() - placedAtom.GetX2d();
                //            double yDiff = atom.GetY2d() - placedAtom.GetY2d();
                double xDiff = placedAtom.Point2D.Value.X - atom.Point2D.Value.X;
                double yDiff = placedAtom.Point2D.Value.Y - atom.Point2D.Value.Y;

                Debug.WriteLine("distributePartners->xdiff: " + Vectors.RadianToDegree(xDiff));
                Debug.WriteLine("distributePartners->ydiff: " + Vectors.RadianToDegree(yDiff));
                startAngle = GeometryUtil.GetAngle(xDiff, yDiff);
                //- (Math.PI / 2.0);
                Debug.WriteLine("distributePartners->angle: " + Vectors.RadianToDegree(startAngle));

                PopulatePolygonCorners(atomsToDraw, atom.Point2D.Value, startAngle, addAngle, bondLength);
                return;
            }
            else if (placedNeighbours.Atoms.Count == 0)
            {
                Debug.WriteLine("First atom...");
                for (int f = 0; f < unplacedNeighbours.Atoms.Count; f++)
                {
                    atomsToDraw.Add(unplacedNeighbours.Atoms[f]);
                }

                addAngle = Math.PI * 2.0 / unplacedNeighbours.Atoms.Count;
                
                // IMPORTANT: At this point we need a calculation of the start
                // angle. Not done yet.
                startAngle = 0.0;
                PopulatePolygonCorners(atomsToDraw, atom.Point2D.Value, startAngle, addAngle, bondLength);
                return;
            }

            // If the atom is a macrocycle and the substituents are terminal we allow these to point these into the ring
            bool isInMacrocycle = atom.GetProperty<bool>(MacroCycleLayout.MACROCYCLE_ATOM_HINT);
            if (isInMacrocycle &&
                placedNeighbours.Atoms.Count == 2 &&
                Molecule.GetBond(atom, placedNeighbours.Atoms[0]).IsInRing &&
                Molecule.GetBond(atom, placedNeighbours.Atoms[1]).IsInRing)
            {

                int numTerminal = 0;
                foreach (var unplaced in unplacedNeighbours.Atoms)
                    if (Molecule.GetConnectedAtoms(unplaced).Count() == 1)
                        numTerminal++;

                if (numTerminal == unplacedNeighbours.Atoms.Count)
                {
                    Vector2 a = NewVector(placedNeighbours.Atoms[0].Point2D.Value, atom.Point2D.Value);
                    Vector2 b = NewVector(placedNeighbours.Atoms[1].Point2D.Value, atom.Point2D.Value);
                    double d1 = GeometryUtil.GetAngle(a.X, a.Y);
                    double d2 = GeometryUtil.GetAngle(b.X, b.Y);
                    double sweep = Vectors.Angle(a, b);
                    if (sweep < Math.PI)
                    {
                        sweep = 2 * Math.PI - sweep;
                    }
                    startAngle = d2;
                    if (d1 > d2 && d1 - d2 < Math.PI || d2 - d1 >= Math.PI)
                    {
                        startAngle = d1;
                    }
                    sweep /= (1 + unplacedNeighbours.Atoms.Count);
                    PopulatePolygonCorners(unplacedNeighbours.Atoms,
                                           atom.Point2D.Value, startAngle, sweep, bondLength);

                    MarkPlaced(unplacedNeighbours);
                    return;
                }
                else
                {
                    atom.RemoveProperty(MacroCycleLayout.MACROCYCLE_ATOM_HINT);
                }
            }

            // if the least hindered side of the atom is clearly defined (bondLength / 10 is an arbitrary value that seemed reasonable)
            //newDirection.Sub(sharedAtomsCenterVector);
            sharedAtomsCenterVector -= newDirection;
            newDirection = sharedAtomsCenterVector;
            newDirection = Vector2.Normalize(newDirection);
            newDirection = newDirection * bondLength;
            newDirection = -newDirection;
            Debug.WriteLine($"distributePartners->newDirection.Lenght(): {newDirection.Length()}");
            Vector2 distanceMeasure = atom.Point2D.Value;
            distanceMeasure += newDirection;

            /// get the two sharedAtom partners with the smallest distance to the new center
            sortedAtoms = AtomContainerManipulator.GetAtomArray(placedNeighbours);
            GeometryUtil.SortBy2DDistance(sortedAtoms, distanceMeasure);
            Vector2 closestPoint1 = sortedAtoms[0].Point2D.Value;
            Vector2 closestPoint2 = sortedAtoms[1].Point2D.Value;
            closestPoint1 -= atom.Point2D.Value;
            closestPoint2 -= atom.Point2D.Value;
            occupiedAngle = Vectors.Angle(closestPoint1, occupiedDirection);
            occupiedAngle += Vectors.Angle(closestPoint2, occupiedDirection);

            double angle1 = GeometryUtil.GetAngle(sortedAtoms[0].Point2D.Value.X - atom.Point2D.Value.X,
                    sortedAtoms[0].Point2D.Value.Y - atom.Point2D.Value.Y);
            double angle2 = GeometryUtil.GetAngle(sortedAtoms[1].Point2D.Value.X - atom.Point2D.Value.X,
                    sortedAtoms[1].Point2D.Value.Y - atom.Point2D.Value.Y);
            double angle3 = GeometryUtil.GetAngle(distanceMeasure.X - atom.Point2D.Value.X,
                    distanceMeasure.Y - atom.Point2D.Value.Y);
            if (debug)
            {
                try
                {
                    Debug.WriteLine("distributePartners->sortedAtoms[0]: ", (Molecule.Atoms.IndexOf(sortedAtoms[0]) + 1));
                    Debug.WriteLine("distributePartners->sortedAtoms[1]: ", (Molecule.Atoms.IndexOf(sortedAtoms[1]) + 1));
                    Debug.WriteLine("distributePartners->angle1: ", Vectors.RadianToDegree(angle1));
                    Debug.WriteLine("distributePartners->angle2: ", Vectors.RadianToDegree(angle2));
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc);
                }
            }
            IAtom startAtom = null;

            if (angle1 > angle3)
            {
                if (angle1 - angle3 < Math.PI)
                {
                    startAtom = sortedAtoms[1];
                }
                else
                {
                    // 12 o'clock is between the two vectors
                    startAtom = sortedAtoms[0];
                }

            }
            else
            {
                if (angle3 - angle1 < Math.PI)
                {
                    startAtom = sortedAtoms[0];
                }
                else
                {
                    // 12 o'clock is between the two vectors
                    startAtom = sortedAtoms[1];
                }
            }
            remainingAngle = (2 * Math.PI) - occupiedAngle;
            addAngle = remainingAngle / (unplacedNeighbours.Atoms.Count + 1);
            if (debug)
            {
                try
                {
                    Debug.WriteLine("distributePartners->startAtom: " + (Molecule.Atoms.IndexOf(startAtom) + 1));
                    Debug.WriteLine("distributePartners->remainingAngle: " + Vectors.RadianToDegree(remainingAngle));
                    Debug.WriteLine("distributePartners->addAngle: " + Vectors.RadianToDegree(addAngle));
                    Debug.WriteLine("distributePartners-> partners.Atoms.Count: " + unplacedNeighbours.Atoms.Count);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc);
                }

            }
            for (int f = 0; f < unplacedNeighbours.Atoms.Count; f++)
            {
                atomsToDraw.Add(unplacedNeighbours.Atoms[f]);
            }
            radius = bondLength;
            startAngle = GeometryUtil.GetAngle(startAtom.Point2D.Value.X - atom.Point2D.Value.X, startAtom.Point2D.Value.Y
                    - atom.Point2D.Value.Y);
            Debug.WriteLine("Before check: distributePartners->startAngle: " + startAngle);
            //        if (startAngle < (Math.PI + 0.001) && startAngle > (Math.PI
            //            -0.001))
            //        {
            //            startAngle = Math.PI/placedNeighbours.Atoms.Count;
            //        }
            Debug.WriteLine("After check: distributePartners->startAngle: " + startAngle);
            PopulatePolygonCorners(atomsToDraw, atom.Point2D.Value, startAngle, addAngle, radius);
        }

        /// <summary>
        /// Places the atoms in a linear chain.
        ///
        /// <para>Expects the first atom to be placed and
        /// places the next atom according to initialBondVector. The rest of the chain
        /// is placed such that it is as linear as possible (in the overall result, the
        /// angles in the chain are set to 120 Deg.)
        /// </para>
        /// </summary>
        /// <param name="atomContainer">The IAtomContainer containing the chain atom to be placed</param>
        /// <param name="initialBondVector">The Vector indicating the direction of the first bond</param>
        /// <param name="bondLength">The factor used to scale the initialBondVector</param>
        public void PlaceLinearChain(IAtomContainer atomContainer, Vector2 initialBondVector, double bondLength)
        {
            IAtomContainer withh = atomContainer.Builder.CreateAtomContainer(atomContainer);

            // BUGFIX - withh does not have cloned cloned atoms, so changes are
            // reflected in our atom container. If we're using implicit hydrogens
            // the correct counts need saving and restoring
            int[] numh = new int[atomContainer.Atoms.Count];
            for (int i = 0, n = atomContainer.Atoms.Count; i < n; i++)
            {
                numh[i] = atomContainer.Atoms[i].ImplicitHydrogenCount ?? 0;
            }

            //        SDG should lay out what it gets and not fiddle with molecules
            //      during layout so this was
            //      removed during debugging. Before you put this in again, contact
            //      er@doktor-steinbeck.de

            //        if(GeometryTools.Has2DCoordinatesNew(atomContainer)==2){
            //            try{
            //                new HydrogenAdder().AddExplicitHydrogensToSatisfyValency(withh);
            //            }Catch(Exception ex){
            //                Trace.TraceWarning("Exception in hydrogen adding. This could mean that cleanup does not respect E/Z: ", ex.Message);
            //                Debug.WriteLine(ex);
            //            }
            //            new HydrogenPlacer().PlaceHydrogens2D(withh, bondLength);
            //        }
            Debug.WriteLine("Placing linear chain of length " + atomContainer.Atoms.Count);
            Vector2 bondVector = initialBondVector;
            IBond prevBond = null;
            for (int f = 0; f < atomContainer.Atoms.Count - 1; f++)
            {
                var atom = atomContainer.Atoms[f];
                var nextAtom = atomContainer.Atoms[f + 1];
                var currBond = atomContainer.GetBond(atom, nextAtom);
                var atomPoint = atom.Point2D.Value;
                bondVector = Vector2.Normalize(bondVector);
                bondVector = bondVector * bondLength;
                atomPoint += bondVector;
                nextAtom.Point2D = atomPoint;
                nextAtom.IsPlaced = true;
                bool trans = false;

                if (prevBond != null && IsColinear(atom, Molecule.GetConnectedBonds(atom)))
                {

                    int atomicNumber = atom.AtomicNumber.Value;
                    int charge = atom.FormalCharge.Value;

                    // double length of the last bond to determing next placement
                    Vector2 p = prevBond.GetConnectedAtom(atom).Point2D.Value;
                    p = Vector2.Lerp(p, atom.Point2D.Value, 2);
                    nextAtom.Point2D = p;
                }

                if (GeometryUtil.Has2DCoordinates(atomContainer))
                {
                    try
                    {
                        if (f > 2
                                && BondTools.IsValidDoubleBondConfiguration(withh,
                                        withh.GetBond(withh.Atoms[f - 2], withh.Atoms[f - 1])))
                        {
                            trans = BondTools.IsCisTrans(withh.Atoms[f - 3], withh.Atoms[f - 2], withh.Atoms[f - 1],
                                    withh.Atoms[f - 0], withh);
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Excpetion in detecting E/Z. This could mean that cleanup does not respect E/Z");
                    }
                    bondVector = GetNextBondVector(nextAtom, atom, GeometryUtil.Get2DCenter(Molecule), trans);
                }
                else
                {
                    bondVector = GetNextBondVector(nextAtom, atom, GeometryUtil.Get2DCenter(Molecule), true);
                }

                prevBond = currBond;
            }

            // BUGFIX part 2 - restore hydrogen counts
            for (int i = 0, n = atomContainer.Atoms.Count; i < n; i++)
            {
                atomContainer.Atoms[i].ImplicitHydrogenCount = numh[i];
            }
        }

        /// <summary>
        ///  Returns the next bond vector needed for drawing an extended linear chain of
        ///  atoms. It assumes an angle of 120 deg for a nice chain layout and
        ///  calculates the two possible placments for the next atom. It returns the
        ///  vector pointing farmost away from a given start atom.
        /// </summary>
        /// <param name="atom">An atom for which the vector to the next atom to draw is calculated</param>
        /// <param name="previousAtom">The preceding atom for angle calculation</param>
        /// <param name="distanceMeasure">A point from which the next atom is to be farmost away</param>
        /// <param name="trans">if true E (trans) configurations are built, false makes Z (cis) configurations</param>
        /// <returns>A vector pointing to the location of the next atom to draw</returns>
        public Vector2 GetNextBondVector(IAtom atom, IAtom previousAtom, Vector2 distanceMeasure, bool trans)
        {
#if DEBUG
            Debug.WriteLine("Entering AtomPlacer.GetNextBondVector()");
            Debug.WriteLine("Arguments are atom: " + atom + ", previousAtom: " + previousAtom + ", distanceMeasure: "
                    + distanceMeasure);
#endif
            double angle = GeometryUtil.GetAngle(previousAtom.Point2D.Value.X - atom.Point2D.Value.X,
                previousAtom.Point2D.Value.Y - atom.Point2D.Value.Y);
            double addAngle = Vectors.DegreeToRadian(120);
            if (!trans) addAngle = Vectors.DegreeToRadian(60);
            if (IsColinear(atom, Molecule.GetConnectedBonds(atom))) addAngle = Vectors.DegreeToRadian(180);

            angle += addAngle;
            Vector2 vec1 = new Vector2(Math.Cos(angle), Math.Sin(angle));
            Vector2 point1 = atom.Point2D.Value;
            point1 += vec1;
            double distance1 = Vector2.Distance(point1, distanceMeasure);
            angle += addAngle;
            Vector2 vec2 = new Vector2(Math.Cos(angle), Math.Sin(angle));
            Vector2 point2 = atom.Point2D.Value;
            point2 += vec2;
            double distance2 = Vector2.Distance(point2, distanceMeasure);
            if (distance2 > distance1)
            {
                Debug.WriteLine("Exiting AtomPlacer.GetNextBondVector()");
                return vec2;
            }
            Debug.WriteLine("Exiting AtomPlacer.GetNextBondVector()");
            return vec1;
        }

        /// <summary>
        ///  Populates the corners of a polygon with atoms. Used to place atoms in a
        ///  geometrically regular way around a ring center or another atom. If this is
        ///  used to place the bonding partner of an atom (and not to draw a ring) we
        ///  want to place the atoms such that those with highest "weight" are placed
        ///  farmost away from the rest of the molecules. The "weight" mentioned here is
        ///  calculated by a modified morgan number algorithm.
        /// </summary>
        /// <param name="atomsToDraw">All the atoms to draw</param>
        /// <param name="startAngle">A start angle, giving the angle of the most clockwise atom which has already been placed</param>
        /// <param name="addAngle">An angle to be added to startAngle for each atom from atomsToDraw</param>
        /// <param name="rotationCenter">The center of a ring, or an atom for which the partners are to be placed</param>
        /// <param name="radius">The radius of the polygon to be populated: bond length or ring radius</param>
        public void PopulatePolygonCorners(IEnumerable<IAtom> atomsToDraw, Vector2 rotationCenter, double startAngle, double addAngle, double radius)
        {
            double angle = startAngle;
            Debug.WriteLine("populatePolygonCorners->startAngle: ", Vectors.RadianToDegree(angle));
            List<Vector2> points = new List<Vector2>();
            //IAtom atom = null;

            Debug.WriteLine("  centerX:", rotationCenter.X);
            Debug.WriteLine("  centerY:", rotationCenter.Y);
            Debug.WriteLine("  radius :", radius);

            foreach (var atom in atomsToDraw)
            {
                angle = angle + addAngle;
                if (angle >= 2.0 * Math.PI)
                {
                    angle -= 2.0 * Math.PI;
                }
                Debug.WriteLine("populatePolygonCorners->angle: ", Vectors.RadianToDegree(angle));
                var x = Math.Cos(angle) * radius;
                var y = Math.Sin(angle) * radius;
                var newX = x + rotationCenter.X;
                var newY = y + rotationCenter.Y;
                Debug.WriteLine("  newX:", newX);
                Debug.WriteLine("  newY:", newY);
                points.Add(new Vector2(newX, newY));
            }

            int i = 0;
            foreach (var connectAtom in atomsToDraw)
            {
                connectAtom.Point2D = (Vector2)points[i++];
                connectAtom.IsPlaced = true;

#if DEBUG
                if (connectAtom != null)
                {
                    try
                    {
                        Debug.WriteLine("populatePolygonCorners->connectAtom: " + (Molecule.Atoms.IndexOf(connectAtom) + 1)
                                + " placed at " + connectAtom.Point2D);
                    }
                    catch (Exception exc)
                    {
                        // nothing to catch here. This is just for logging
                    }
                }
#endif
            }

        }

        /// <summary>
        ///  Partition the bonding partners of a given atom into placed (coordinates assinged) and not placed.
        /// </summary>
        /// <param name="atom">The atom whose bonding partners are to be partitioned</param>
        /// <param name="unplacedPartners">A vector for the unplaced bonding partners to go in</param>
        /// <param name="placedPartners">A vector for the placed bonding partners to go in</param>
        public void PartitionPartners(IAtom atom, IAtomContainer unplacedPartners, IAtomContainer placedPartners)
        {
            var atoms = Molecule.GetConnectedAtoms(atom);
            foreach (var curatom in atoms)
            {
                if (curatom.IsPlaced)
                {
                    placedPartners.Atoms.Add(curatom);
                }
                else
                {
                    unplacedPartners.Atoms.Add(curatom);
                }
            }
        }

        /// <summary>
        ///  Search an aliphatic molecule for the longest chain. This is the method to
        ///  be used if there are no rings in the molecule and you want to layout the
        ///  longest chain in the molecule as a starting point of the structure diagram
        ///  generation.
        /// </summary>
        /// <param name="molecule">The molecule to be search for the longest unplaced chain</param>
        /// <returns>An AtomContainer holding the longest chain.</returns>
        /// <exception cref="NoSuchAtomException">Description of the Exception</exception>
        static public IAtomContainer GetInitialLongestChain(IAtomContainer molecule)
        {
            Debug.WriteLine("Start of GetInitialLongestChain()");
            double[][] conMat = ConnectionMatrix.GetMatrix(molecule);
            Debug.WriteLine("Computing all-pairs-shortest-pathes");
            int[][] apsp = PathTools.ComputeFloydAPSP(conMat);
            int maxPathLength = 0;
            int bestStartAtom = -1;
            int bestEndAtom = -1;
            IAtom atom = null;
            IAtom startAtom = null;
            //IAtom endAtom = null;
            for (int f = 0; f < apsp.Length; f++)
            {
                atom = molecule.Atoms[f];
                if (molecule.GetConnectedBonds(atom).Count() == 1)
                {
                    for (int g = 0; g < apsp.Length; g++)
                    {
                        if (apsp[f][g] > maxPathLength)
                        {
                            maxPathLength = apsp[f][g];
                            bestStartAtom = f;
                            bestEndAtom = g;
                        }
                    }
                }
            }
            Debug.WriteLine("Longest chaing in molecule is of length " + maxPathLength + " between atoms "
                    + (bestStartAtom + 1) + " and " + (bestEndAtom + 1));

            startAtom = molecule.Atoms[bestStartAtom];
            //endAtom = Molecule.GetAtomAt(bestEndAtom);
            IAtomContainer path = molecule.Builder.CreateAtomContainer();
            path.Atoms.Add(startAtom);
            path = GetLongestUnplacedChain(molecule, startAtom);
            //PathTools.DepthFirstTargetSearch(molecule, startAtom, endAtom, path);
            Debug.WriteLine("End of GetInitialLongestChain()");
            return path;
        }

        /// <summary>
        ///  Search a molecule for the longest unplaced, aliphatic chain in it. If an
        ///  aliphatic chain encounters an unplaced ring atom, the ring atom is also
        ///  appended to allow for it to be laid out. This gives us a vector for
        ///  attaching the unplaced ring later.
        /// </summary>
        /// <param name="molecule">The molecule to be search for the longest unplaced chain</param>
        /// <param name="startAtom">A start atom from which the chain search starts</param>
        /// <returns>An AtomContainer holding the longest unplaced chain.</returns>
        /// <exception cref="CDKException">Description of the Exception</exception>
        static public IAtomContainer GetLongestUnplacedChain(IAtomContainer molecule, IAtom startAtom)
        {
            Debug.WriteLine("Start of getLongestUnplacedChain.");
            //ConnectivityChecker cc = new ConnectivityChecker();
            int longest = 0;
            int longestPathLength = 0;
            int maxDegreeSum = 0;
            int degreeSum = 0;
            IAtomContainer[] pathes = new IAtomContainer[molecule.Atoms.Count];
            for (int f = 0; f < molecule.Atoms.Count; f++)
            {
                molecule.Atoms[f].IsVisited = false;
                pathes[f] = molecule.Builder.CreateAtomContainer();
                pathes[f].Atoms.Add(startAtom);
            }
            var startSphere = new List<IAtom>();
            startSphere.Add(startAtom);
            BreadthFirstSearch(molecule, startSphere, pathes);
            for (int f = 0; f < molecule.Atoms.Count; f++)
            {
                if (pathes[f].Atoms.Count >= longestPathLength)
                {
                    degreeSum = GetDegreeSum(pathes[f], molecule);

                    if (degreeSum > maxDegreeSum)
                    {
                        maxDegreeSum = degreeSum;
                        longest = f;
                        longestPathLength = pathes[f].Atoms.Count;
                    }
                }
            }
            Debug.WriteLine("End of getLongestUnplacedChain.");
            return pathes[longest];
        }

        /// <summary>
        ///  Performs a breadthFirstSearch in an AtomContainer starting with a
        ///  particular sphere, which usually consists of one start atom, and searches
        ///  for the longest aliphatic chain which is yet unplaced. If the search
        ///  encounters an unplaced ring atom, it is also appended to the chain so that
        ///  this last bond of the chain can also be laid out. This gives us the
        ///  orientation for the attachment of the ring system.
        /// </summary>
        /// <param name="ac">The AtomContainer to be searched</param>
        /// <param name="sphere">A sphere of atoms to start the search with</param>
        /// <param name="pathes">A vector of N pathes (N = no of heavy atoms).</param>
        /// <exception cref="CDKException"> Description of the Exception</exception>
        static public void BreadthFirstSearch(IAtomContainer ac, IList<IAtom> sphere, IAtomContainer[] pathes)
        {
            IAtom nextAtom = null;
            int atomNr;
            int nextAtomNr;
            //IAtomContainer path = null;
            List<IAtom> newSphere = new List<IAtom>();
            Debug.WriteLine("Start of breadthFirstSearch");

            foreach (var atom in sphere)
            {
                if (!atom.IsInRing)
                {
                    atomNr = ac.Atoms.IndexOf(atom);
                    Debug.WriteLine($"{nameof(BreadthFirstSearch)} around atom {atomNr + 1}");

                    var bonds = ac.GetConnectedBonds(atom);
                    foreach (var curBond in bonds)
                    {
                        nextAtom = curBond.GetConnectedAtom(atom);
                        if (!nextAtom.IsVisited && !nextAtom.IsPlaced)
                        {
                            nextAtomNr = ac.Atoms.IndexOf(nextAtom);
                            Debug.WriteLine("BreadthFirstSearch is meeting new atom " + (nextAtomNr + 1));
                            pathes[nextAtomNr] = ac.Builder.CreateAtomContainer(pathes[atomNr]);
                            Debug.WriteLine("Making copy of path " + (atomNr + 1) + " to form new path " + (nextAtomNr + 1));
                            pathes[nextAtomNr].Atoms.Add(nextAtom);
                            Debug.WriteLine("Adding atom " + (nextAtomNr + 1) + " to path " + (nextAtomNr + 1));
                            pathes[nextAtomNr].Bonds.Add(curBond);
                            if (ac.GetConnectedBonds(nextAtom).Count() > 1)
                            {
                                newSphere.Add(nextAtom);
                            }
                        }
                    }
                }
            }
            if (newSphere.Count > 0)
            {
                for (int f = 0; f < newSphere.Count; f++)
                {
                    newSphere[f].IsVisited = true;
                }
                BreadthFirstSearch(ac, newSphere, pathes);
            }
            Debug.WriteLine("End of breadthFirstSearch");
        }

        /// <summary>
        ///  Returns a string with the numbers of all placed atoms in an AtomContainer
        /// </summary>
        /// <param name="ac">The AtomContainer for which the placed atoms are to be listed</param>
        /// <returns>A string with the numbers of all placed atoms in an AtomContainer</returns>
        public string ListPlaced(IAtomContainer ac)
        {
            string s = "Placed: ";
            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                if (ac.Atoms[f].IsPlaced)
                {
                    s += (f + 1) + "+ ";
                }
                else
                {
                    s += (f + 1) + "- ";
                }
            }
            return s;
        }

        /// <summary>
        ///  Returns a string with the numbers of all atoms in an AtomContainer relative
        ///  to a given Molecule. I.e. the number the is listesd is the position of each
        ///  atom in the Molecule.
        /// </summary>
        /// <param name="ac">The AtomContainer for which the placed atoms are to be listed</param>
        /// <param name="mol">Description of  Parameter</param>
        /// <returns>A string with the numbers of all placed atoms in an AtomContainer</returns>
        /// <exception cref="CDKException"></exception>
        static public string ListNumbers(IAtomContainer mol, IAtomContainer ac)
        {
            string s = "Numbers: ";
            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                s += (mol.Atoms.IndexOf(ac.Atoms[f]) + 1) + " ";
            }
            return s;
        }

        /// <summary>
        ///  Returns a string with the numbers of all atoms in a Vector relative to a
        ///  given Molecule. I.e. the number the is listesd is the position of each atom
        ///  in the Molecule.
        /// </summary>
        /// <param name="ac">The Vector for which the placed atoms are to be listed</param>
        /// <param name="mol">Description of the Parameter</param>
        /// <returns>A string with the numbers of all placed atoms in an AtomContainer</returns>
        static public string ListNumbers(IAtomContainer mol, List<IAtom> ac)
        {
            string s = "Numbers: ";
            for (int f = 0; f < ac.Count; f++)
            {
                s += (mol.Atoms.IndexOf((IAtom)ac[f]) + 1) + " ";
            }
            return s;
        }

        /// <summary>
        ///  True is all the atoms in the given AtomContainer have been placed
        ///
        /// <param name="ac">The AtomContainer to be searched</param>
        /// <returns>True is all the atoms in the given AtomContainer have been placed</returns>
        /// </summary>
        static public bool AllPlaced(IAtomContainer ac)
        {
            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                if (!ac.Atoms[f].IsPlaced)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///  Marks all the atoms in the given AtomContainer as not placed
        /// </summary>
        /// <param name="ac">The AtomContainer whose atoms are to be marked</param>
        static public void MarkNotPlaced(IAtomContainer ac)
        {
            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                ac.Atoms[f].IsPlaced = false;
            }

        }

        /// <summary>
        ///  Marks all the atoms in the given AtomContainer as placed
        ///
        /// <param name="ac">The AtomContainer whose atoms are to be marked</param>
        /// </summary>
        static public void MarkPlaced(IAtomContainer ac)
        {
            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                ac.Atoms[f].IsPlaced = true;
            }
        }

        /// <summary>
        ///  Get all the placed atoms in an AtomContainer
        /// </summary>
        /// <param name="ac">The AtomContainer to be searched for placed atoms</param>
        /// <returns>An AtomContainer containing all the placed atoms</returns>
        static public IAtomContainer GetPlacedAtoms(IAtomContainer ac)
        {
            IAtomContainer ret = ac.Builder.CreateAtomContainer();
            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                if (ac.Atoms[f].IsPlaced)
                {
                    ret.Atoms.Add(ac.Atoms[f]);
                }
            }
            return ret;
        }

        /// <summary>
        ///  Sums up the degrees of atoms in an atomcontainer
        /// </summary>
        /// <param name="ac">The atomcontainer to be processed</param>
        /// <param name="superAC">The superAtomContainer from which the former has been derived</param>
        /// <returns>sum of degrees</returns>
        static int GetDegreeSum(IAtomContainer ac, IAtomContainer superAC)
        {
            int degreeSum = 0;
            //string path = "DegreeSum for Path: ";
            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                //path += ac.Atoms[f].Symbol;
                degreeSum += superAC.GetConnectedBonds(ac.Atoms[f]).Count();

                degreeSum += ac.Atoms[f].ImplicitHydrogenCount ?? 0;
            }
            //Console.Out.WriteLine(path + ": " + degreeSum);
            return degreeSum;
        }

        /// <summary>
        /// Calculates priority for atoms in a Molecule.
        /// </summary>
        /// <param name="mol">connected molecule</param>
        /// <seealso cref="PRIORITY"/>
        internal static void Prioritise(IAtomContainer mol)
        {
            Prioritise(mol, GraphUtil.ToAdjList(mol));
        }

        /// <summary>
        /// Calculates priority for atoms in a Molecule.
        /// </summary>
        /// <param name="mol">connected molecule</param>
        /// <param name="adjList">fast adjacency lookup</param>
        /// <seealso cref="PRIORITY"/>
        static void Prioritise(IAtomContainer mol, int[][] adjList)
        {
            int[] weights = GetPriority(mol, adjList);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                mol.Atoms[i].SetProperty(PRIORITY, weights[i]);
            }
        }

        /// <summary>
        /// Prioritise atoms of a molecule base on how 'buried' they are. The priority
        /// is cacheted with a morgan-like relaxation O(n^2 lg n). Priorities are assign
        /// from 1..|V| (usually less than |V| due to symmetry) where the lowest numbers
        /// have priority.
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <param name="adjList">fast adjacency lookup</param>
        /// <returns>the priority</returns>
        static int[] GetPriority(IAtomContainer mol, int[][] adjList)
        {
            int n = mol.Atoms.Count;
            int[] order = new int[n];
            int[] rank = new int[n];
            int[] prev = new int[n];

            // init priorities, Helson 99 favours cyclic (init=2)
            for (int f = 0; f < n; f++)
            {
                rank[f] = 1;
                prev[f] = 1;
                order[f] = f;
            }

            int nDistinct = 1;

            for (int rep = 0; rep < n; rep++)
            {
                for (int i = 0; i < n; i++)
                {
                    rank[i] = 3 * prev[i];
                    foreach (var w in adjList[i])
                        rank[i] += prev[w];
                }

                // assign new ranks
                Array.Sort(order, (a, b) => rank[a].CompareTo(rank[b]));
                int clsNum = 1;
                prev[order[0]] = clsNum;
                for (int i = 1; i < n; i++)
                {
                    if (rank[order[i]] != rank[order[i - 1]])
                        clsNum++;
                    prev[order[i]] = clsNum;
                }

                // no refinement over previous
                if (clsNum == nDistinct)
                    break;
                nDistinct = clsNum;
            }

            // we want values 1 ≤ x < |V|
            for (int i = 0; i < n; i++)
                prev[i] = 1 + nDistinct - prev[i];

            return prev;
        }

        /// <summary>
        /// -C#N
        /// -[N+]#[C-]
        /// -C=[N+]=N
        /// -N=[N+]=N
        /// </summary>
        bool IsColinear(IAtom atom, IEnumerable<IBond> bonds)
        {
            if (bonds.Count() != 2)
                return false;

            int numSgl = atom.ImplicitHydrogenCount ?? 0;
            int numDbl = 0;
            int numTpl = 0;

            foreach (var bond in bonds)
            {
                switch (bond.Order.Numeric)
                {
                    case 1:
                        numSgl++;
                        break;
                    case 2:
                        numDbl++;
                        break;
                    case 3:
                        numTpl++;
                        break;
                    case 4:
                        return true;
                    default:
                        return false;
                }
            }

            switch (atom.AtomicNumber)
            {
                case 6:
                case 7:
                case 14:
                case 32:
                    if (numTpl == 1 && numSgl == 1)
                        return true;
                    if (numDbl == 2 && numSgl == 0)
                        return true;
                    break;
            }

            return false;
        }

        [Obsolete]
        static public bool ShouldBeLinear(IAtom atom, IAtomContainer molecule)
        {
            int sum = 0;
            var bonds = molecule.GetConnectedBonds(atom);
            foreach (var bond in bonds)
            {
                if (bond.Order == BondOrder.Triple)
                    sum += 10;
                else if (bond.Order == BondOrder.Single) sum += 1;
                //            else if (bond.Order == BondOrder.Double) sum += 5;
            }
            if (sum >= 10) return true;
            return false;
        }

        static Vector2 NewVector(Vector2 to, Vector2 from)
        {
            return new Vector2(to.X - from.X, to.Y - from.Y);
        }
    }
}
