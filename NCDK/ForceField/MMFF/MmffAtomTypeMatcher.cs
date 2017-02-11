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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Primitives;
using NCDK.Graphs;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.Smiles.SMARTS.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using static NCDK.Graphs.GraphUtil;

namespace NCDK.ForceField.MMFF
{
    /**
    // Determine the MMFF symbolic atom types {@cdk.cite Halgren96a}. The matcher uses SMARTS patterns
    // to assign preliminary symbolic types. The types are then adjusted considering aromaticity {@link
    // MmffAromaticTypeMapping}. The assigned atom types validate completely with the validation suite
    // (http://server.ccl.net/cca/data/MMFF94/).
     *
    // <pre>{@code
    // MmffAtomTypeMatcher mmffAtomTypes = new MmffAtomTypeMatcher();
     *
    // foreach (var container in containers) {
    //     string[] symbs = mmffAtomTypes.SymbolicTypes(container);
    // }
    // }</pre>
     *
    // @author John May
     */
#if TEST
    public
#endif
    sealed class MmffAtomTypeMatcher
    {

        /// <summary>Aromatic types are assigned by this class.</summary>
        private readonly MmffAromaticTypeMapping aromaticTypes = new MmffAromaticTypeMapping();

        /// <summary>Substructure patterns for atom types.</summary>
        private readonly AtomTypePattern[] patterns;

        /// <summary>Mapping of parent to hydrogen symbols.</summary>
        private readonly IDictionary<string, string> hydrogenMap;

        /**
        // Create a new MMFF atom type matcher, definitions are loaded at instantiation.
         */
        public MmffAtomTypeMatcher()
        {
            Stream smaIn = GetType().Assembly.GetManifestResourceStream(GetType(), "MMFFSYMB.sma");
            Stream hdefIn = GetType().Assembly.GetManifestResourceStream(GetType(), "mmff-symb-mapping.tsv");

            try
            {
                this.patterns = LoadPatterns(smaIn);
                this.hydrogenMap = LoadHydrogenDefinitions(hdefIn);
            }
            catch (IOException e)
            {
                throw new ApplicationException("Atom type definitions for MMFF94 Atom Types could not be loaded: "
                        + e.Message);
            }
            finally
            {
                Close(smaIn);
                Close(hdefIn);
            }
        }

        /**
        // Obtain the MMFF symbolic types to the atoms of the provided structure.
         *
        // @param container structure representation
        // @return MMFF symbolic types for each atom index
         */
        public string[] SymbolicTypes(IAtomContainer container)
        {
            EdgeToBondMap bonds = EdgeToBondMap.WithSpaceFor(container);
            int[][] graph = GraphUtil.ToAdjList(container, bonds);
            return SymbolicTypes(container, graph, bonds, new HashSet<IBond>());
        }

        /**
        // Obtain the MMFF symbolic types to the atoms of the provided structure.
         *
        // @param container structure representation
        // @param graph     adj list data structure
        // @param bonds     bond lookup map
        // @param mmffArom  flags which bonds are aromatic by MMFF model
        // @return MMFF symbolic types for each atom index
         */
        public string[] SymbolicTypes(IAtomContainer container, int[][] graph, EdgeToBondMap bonds, ISet<IBond> mmffArom)
        {

            // Array of symbolic types, MMFF refers to these as 'SYMB' and the numeric
            // value a s 'TYPE'.
            string[] symbs = new string[container.Atoms.Count];

            CheckPreconditions(container);

            AssignPreliminaryTypes(container, symbs);

            // aromatic types, set by upgrading preliminary types in specified positions
            // and conditions. This requires a fair bit of code and is delegated to a separate class.
            aromaticTypes.Assign(container, symbs, bonds, graph, mmffArom);

            // special case, 'NCN+' matches entries that the validation suite say should
            // actually be 'NC=N'. We can achieve 100% compliance by checking if NCN+ is still
            // next to CNN+ or CIM+ after aromatic types are assigned
            FixNCNTypes(symbs, graph);

            AssignHydrogenTypes(container, symbs, graph);

            return symbs;
        }

        /**
        // Special case, 'NCN+' matches entries that the validation suite say should actually be 'NC=N'.
        // We can achieve 100% compliance by checking if NCN+ is still next to CNN+ or CIM+ after
        // aromatic types are assigned
         *
        // @param symbs symbolic types
        // @param graph adjacency list graph
         */
        private void FixNCNTypes(string[] symbs, int[][] graph)
        {
            for (int v = 0; v < graph.Length; v++)
            {
                if ("NCN+".Equals(symbs[v]))
                {
                    bool foundCNN = false;
                    foreach (var w in graph[v])
                    {
                        foundCNN = foundCNN || "CNN+".Equals(symbs[w]) || "CIM+".Equals(symbs[w]);
                    }
                    if (!foundCNN)
                    {
                        symbs[v] = "NC=N";
                    }
                }
            }
        }

        /**
        // preconditions, 1. all hydrogens must be present as explicit nodes in the connection table.
        // this requires that each atom explicitly states it has exactly 0 hydrogens 2. the SMARTS treat
        // all atoms as aliphatic and therefore no aromatic flags should be set, we could remove this
        // but ideally we don't want to modify the structure
         *
        // @param container input structure representation
         */
        private void CheckPreconditions(IAtomContainer container)
        {
            foreach (var atom in container.Atoms)
            {
                if (atom.ImplicitHydrogenCount == null || atom.ImplicitHydrogenCount != 0)
                    throw new ArgumentException("Hydrogens should be unsuppressed (explicit)");
                if (atom.IsAromatic)
                    throw new ArgumentException("No aromatic flags should be set");
            }
        }

        /**
        // Hydrogen types, assigned based on the MMFFHDEF.PAR parent associations.
         *
        // @param container input structure representation
        // @param symbs     symbolic atom types
        // @param graph     adjacency list graph
         */
        private void AssignHydrogenTypes(IAtomContainer container, string[] symbs, int[][] graph)
        {
            for (int v = 0; v < graph.Length; v++)
            {
                if (container.Atoms[v].Symbol.Equals("H") && graph[v].Length == 1)
                {
                    int w = graph[v][0];
                    var symb = symbs[w];
                    symbs[v] = symb == null ? null : this.hydrogenMap[symb];
                }
            }
        }

        /**
        // Preliminary atom types are assigned using SMARTS definitions.
         *
        // @param container input structure representation
        // @param symbs     symbolic atom types
         */
        private void AssignPreliminaryTypes(IAtomContainer container, string[] symbs)
        {
            SmartsMatchers.Prepare(container, true);
            foreach (var matcher in patterns)
            {
                foreach (var idx in matcher.Matches(container))
                {
                    if (symbs[idx] == null)
                    {
                        symbs[idx] = matcher.symb;
                    }
                }
            }
        }

        /**
        // Internal - load the SMARTS patterns for each atom type from MMFFSYMB.sma.
         *
        // @param smaIn input stream of MMFFSYMB.sma
        // @return array of patterns
        // @throws IOException
         */
#if TEST
            public
#endif
        static AtomTypePattern[] LoadPatterns(Stream smaIn)
        {

            List<AtomTypePattern> matchers = new List<AtomTypePattern>();

            using (var br = new StreamReader(smaIn))
            {
                string line = null;
                while ((line = br.ReadLine()) != null)
                {
                    if (SkipLine(line)) continue;
                    var cols = Strings.Tokenize(line, ' ');
                    string sma = cols[0];
                    string symb = cols[1];

                    try
                    {
                        IQueryAtomContainer container = SMARTSParser.Parse(sma, null);
                        matchers.Add(new AtomTypePattern(VentoFoggia.FindSubstructure(container), symb));
                    }
                    catch (ArgumentException e)
                    {
                        throw new IOException(line + " could not be loaded: " + e.Message);
                    }
                    catch (TokenMgrError e)
                    {
                        throw new IOException(line + " could not be loaded: " + e.Message);
                    }
                }

                return matchers.ToArray();
            }
        }

        /**
        // Hydrogen atom types are assigned based on their parent types. The mmff-symb-mapping file
        // provides this mapping.
         *
        // @param hdefIn input stream of mmff-symb-mapping.tsv
        // @return mapping of parent to hydrogen definitions
        // @throws IOException
         */
        private IDictionary<string, string> LoadHydrogenDefinitions(Stream hdefIn)
        {

            // maps of symbolic atom types to hydrogen atom types and internal types
            IDictionary<string, string> hdefs = new Dictionary<string, string>(200);

            using (var br = new StreamReader(hdefIn))
            {
                br.ReadLine(); // header
                string line = null;
                while ((line = br.ReadLine()) != null)
                {
                    var cols = Strings.Tokenize(line, '\t');
                    hdefs[cols[0].Trim()] = cols[3].Trim();
                }
            }

            // these associations list hydrogens that are not listed in MMFFSYMB.PAR but present in MMFFHDEF.PAR
            // N=O HNO, NO2 HNO2, F HX, I HX, ONO2 HON, BR HX, ON=O HON, CL HX, SNO HSNO, and OC=S HOCS

            return hdefs;
        }

        /**
        // A line is skipped if it is empty or is a comment. MMFF files use '*' to mark comments and '$'
        // for end of file.
         *
        // @param line an input line
        // @return whether to skip this line
         */
        private static bool SkipLine(string line)
        {
            return line.Length == 0 || line[0] == '*' || line[0] == '$';
        }

        /**
        // Safely close an input stream.
         *
        // @param in stream to close
         */
        private static void Close(Stream in_)
        {
            try
            {
                if (in_ != null) in_.Close();
            }
            catch (IOException)
            {
                // ignored
            }
        }

        /**
        // A class that associates a pattern instance with the MMFF symbolic type. Using SMARTS the
        // implied type is at index 0. The matching could be improved in future to skip subgraph
        // matching of all typed atoms.
         */
#if TEST
        public
#else
        private
#endif
        sealed class AtomTypePattern
        {

            private readonly Pattern pattern;
            public readonly string symb;

            /**
            // Create the atom type pattern.
             *
            // @param pattern substructure pattern
            // @param symb    MMFF symbolic type
             */
            public AtomTypePattern(Pattern pattern, string symb)
            {
                this.pattern = pattern;
                this.symb = symb;
            }

            /**
            // Find the atoms that match this atom type.
             *
            // @param container structure representation
            // @return indices of atoms that matched this type
             */
            public ISet<int> Matches(IAtomContainer container)
            {
                var matchedIdx = new HashSet<int>();
                foreach (var mapping in pattern.MatchAll(container))
                {
                    matchedIdx.Add(mapping[0]);
                }
                return matchedIdx;
            }
        }
    }
}

