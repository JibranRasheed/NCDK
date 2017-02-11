/*
 * Copyright (c) 2013, European Bioinformatics Institute (EMBL-EBI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * Any EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * Any DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON Any THEORY OF LIABILITY, WHETHER IN CONTRACT, Strict LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN Any WAY OUT OF THE USE OF THIS
 * SOFTWARE, Even IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies,
 * either expressed or implied, of the FreeBSD Project.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Beam
{
    /// <summary>
    /// Given a molecule with bond-based double bond configurations - add directional
    /// labels to edges which do not have it assigned. For example the molecule
    /// {@code NC(/C)=C\C} has no directional label between the nitrogen and the
    /// carbon. Applying this procedure will 'fill-in' missing directional
    /// information on the edge - {@code N/C(/C)=C\C}. <br/>
    ///
    /// If required the directional labels in conjugated systems may be adjusted to
    /// allow for full-specification. Attempting to assign a directional label to the
    /// central carbon of {@code F/C=C(/F)C(/F)=C/F} creates a conflict. This conflict
    /// will be resolved by flipping the labels on the second double-bond - {@code
    /// F/C=C(/F)\C(\F)=C\F}.
    ///
    /// <author>John May</author>
    /// </summary>
#if TEST
	public
#endif
    sealed class AddDirectionalLabels
        : AbstractFunction<Graph, Graph>
    {

        /// <summary>
        /// Transform all implicit up/down to their explicit type. The original graph
        /// is unmodified
        /// </summary>
        /// <param name="g">a chemical graph</param>
        /// <returns>new chemical graph but with all explicit bonds</returns>
        public override Graph Apply(Graph g)
        {

            Graph h = new Graph(g.Order);

            // copy atom/topology information this is unchanged
            for (int u = 0; u < g.Order; u++)
            {
                h.AddAtom(g.GetAtom_(u));
                h.AddTopology(g.TopologyOf(u));
            }

            IDictionary<Edge, Edge> replacements = new Dictionary<Edge, Edge>();
            ICollection<Edge> remaining = new HashSet<Edge>();

            // change edges (only changed added to replacement)
            for (int u = 0; u < g.Order; u++)
            {
                foreach (var e in g.GetEdges(u))
                {
                    if (e.Other(u) > u && e.Bond == Bond.Double)
                    {
                        remaining.Add(e);
                    }
                }
            }

            bool altered = false;
            do
            {
                foreach (var e in new List<Edge>(remaining))
                {
                    if (!RePlaceImplWithExpl(g, e, replacements))
                    {
                        remaining.Remove(e);
                        altered = true;
                    }
                }
            } while (altered && remaining.Count > 0);

            // append the edges, replacing any which need to be changed
            for (int u = 0; u < g.Order; u++)
            {
                foreach (var e in g.GetEdges(u))
                {
                    if (e.Other(u) > u)
                    {
                        Edge ee = e;
                        Edge replacement;
                        if (replacements.TryGetValue(e, out replacement))
                            ee = replacement;
                        h.AddEdge(ee);
                    }
                }
            }

            return h;
        }

        /// <summary>
        /// Given a double bond edge traverse the neighbors of both endpoints and
        /// accumulate any explicit replacements in the 'acc' accumulator.
        ///
        /// <param name="g">  the chemical graph</param>
        /// <param name="e">  a edge in the graph </param>('double bond type')
        /// <param name="acc">accumulator for new edges</param>
        // @ thrown if the edge could not be converted
        /// </summary>
        private bool RePlaceImplWithExpl(Graph g,
                                            Edge e,
                                            IDictionary<Edge, Edge> acc)
        {

            int u = e.Either(), v = e.Other(u);

            bool uDone = RePlaceImplWithExpl(g, e, u, acc);
            bool vDone = RePlaceImplWithExpl(g, e, v, acc);

            return uDone || vDone;
        }

        /// <summary>
        /// Given a double bond edge traverse the neighbors of one of the endpoints
        /// and accumulate any explicit replacements in the 'acc' accumulator.
        ///
        /// <param name="g">  the chemical graph</param>
        /// <param name="e">  a edge in the graph </param>('double bond type')
        /// <param name="u">  a endpoint of the edge 'e'</param>
        /// <param name="acc">accumulator for new edges</param>
        /// <returns>does the edge 'e' need to be reconsidered later</returns>
        // @ thrown if the edge could not be converted
        /// </summary>
        private bool RePlaceImplWithExpl(Graph g,
                                            Edge e,
                                            int u,
                                            IDictionary<Edge, Edge> acc)
        {
            Edge implicit_ = null;
            Edge explicit_ = null;

            foreach (var f in g.GetEdges(u))
            {
                Edge f2 = acc.ContainsKey(f) ? acc[f] : f;
                var aa = f2.GetBond(u);
                if (aa == Bond.Single || aa == Bond.Implicit)
                {
                    if (implicit_ != null)
                        return true;
                    implicit_ = f;
                }
                else if (aa == Bond.Double)
                {
                    if (!f.Equals(e))
                        return false;
                }
                else if (aa == Bond.Up || aa == Bond.Down)
                {
                    if (explicit_ != null)
                    {
                        if (acc.ContainsKey(explicit_))
                            explicit_ = acc[explicit_];

                        // original bonds are invalid
                        if ((f.Bond == Bond.Up || f.Bond == Bond.Down) &&
                                explicit_.GetBond(u).Inverse() != f.GetBond(u))
                        {
                            throw new InvalidSmilesException("invalid double bond configuration");
                        }

                        if (explicit_.GetBond(u).Inverse() != f2.GetBond(u))
                        {
                            acc[f] = f2.Inverse();
                            BitArray visited = new BitArray(g.Order);
                            visited.Set(u, true);
                            InvertExistingDirectionalLabels(g, visited, acc, f2
                                    .Other(u));
                        }
                        return false;
                    }
                    explicit_ = f;
                }
            }

            // no implicit or explicit bond? don't do anything
            if (implicit_ == null || explicit_ == null)
                return false;

            if (acc.ContainsKey(explicit_))
                explicit_ = acc[explicit_];

            int v = implicit_.Other(u);

            Edge existing;
            if (!acc.TryGetValue(implicit_, out existing))
                existing = null;
            acc[implicit_] = new Edge(u,
                                      v,
                                      explicit_.GetBond(u)
                                               .Inverse());

            if (existing != null && existing.GetBond(u) != explicit_.GetBond(u).Inverse())
                throw new InvalidSmilesException("unable to assign explict type for " + implicit_);

            return false;
        }

        private void InvertExistingDirectionalLabels(Graph g,
                                                     BitArray visited,
                                                     IDictionary<Edge, Edge> replacement,
                                                     int u)
        {
            visited.Set(u, true);
            if (g.TopologyOf(u) == null)
                return;
            foreach (var e in g.GetEdges(u))
            {
                int v = e.Other(u);
                if (!visited[v])
                {
                    Edge f;
                    if (replacement.TryGetValue(e, out f))
                    {
                        replacement[e] = f.Inverse();
                    }
                    else
                    {
                        replacement[e] = e.Inverse();
                    }
                    InvertExistingDirectionalLabels(g, visited, replacement, v);
                }
            }
        }

    }
}