/* Copyright (C) 2008 Rajarshi Guha <rajarshi@users.sourceforge.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Primitives;
using NCDK.Graphs;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.RingSearches;
using NCDK.Smiles.SMARTS.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.Fingerprints
{
    /// <summary>
    /// This fingerprinter generates 166 bit MACCS keys.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The SMARTS patterns for each of the features was taken from
    /// <see href="http://www.rdkit.org"> RDKit</see>. However given that there is no
    /// official and explicit listing of the original key definitions, the results
    /// of this implementation may differ from others.
    /// </para>
    /// <para>
    /// This class assumes that aromaticity perception, atom typing and adding of
    /// implicit hydrogens have been performed prior to generating the fingerprint.
    /// </para>
    /// <note type="note">
    /// Currently bits 1 and 44 are completely ignored since the RDKit
    /// defs do not provide a definition and I can't find an official description
    /// of them.
    /// </note>
    /// <note type="warning">
    /// MACCS substructure keys cannot be used for substructure
    /// filtering. It is possible for some keys to match substructures and not match
    /// the superstructures. Some keys check for hydrogen counts which may not be
    /// preserved in a superstructure.
    /// </note>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2008-07-23
    // @cdk.keyword fingerprint
    // @cdk.keyword similarity
    // @cdk.module  fingerprint
    // @cdk.githash
    internal class MACCSFingerprinter : AbstractFingerprinter, IFingerprinter
    {
        private const string KeyDefinitions = "Data.maccs.txt";

        private volatile IList<MaccsKey> keys = null;

        public MACCSFingerprinter() { }

        public MACCSFingerprinter(IChemObjectBuilder builder)
        {
            try
            {
                keys = ReadKeyDef(builder);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
            }
            catch (CDKException e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <inheritdoc/>
        public override IBitFingerprint GetBitFingerprint(IAtomContainer container)
        {
            var keys = GetKeys(container.Builder);
            BitArray fp = new BitArray(keys.Count);

            // init SMARTS invariants (connectivity, degree, etc)
            SmartsMatchers.Prepare(container, false);

            int numAtoms = container.Atoms.Count;

            GraphUtil.EdgeToBondMap bmap = GraphUtil.EdgeToBondMap.WithSpaceFor(container);
            int[][] adjlist = GraphUtil.ToAdjList(container, bmap);

            for (int i = 0; i < keys.Count; i++)
            {
                MaccsKey key = keys[i];
                Pattern pattern = key.Pattern;

                switch (key.Smarts)
                {
                    case "[!*]":
                        break;
                    case "[!0]":
                        foreach (IAtom atom in container.Atoms)
                        {
                            if (atom.MassNumber != null)
                            {
                                fp.Set(i, true);
                                break;
                            }
                        }
                        break;
                    // ring bits
                    case "[R]1@*@*@1": // 3M RING bit22
                    case "[R]1@*@*@*@1": // 4M RING bit11
                    case "[R]1@*@*@*@*@1": // 5M RING bit96
                    case "[R]1@*@*@*@*@*@1": // 6M RING bit163, x2=bit145
                    case "[R]1@*@*@*@*@*@*@1": // 7M RING, bit19
                    case "[R]1@*@*@*@*@*@*@*@1": // 8M RING, bit101
                                                 // handled separately
                        break;
                    case "(*).(*)":
                        // bit 166 (*).(*) we can match this in SMARTS but it's faster to just
                        // count the number of components or in this case try to traverse the
                        // component, iff there are some atoms not visited we have more than
                        // one component
                        bool[] visit = new bool[numAtoms];
                        if (numAtoms > 1 && VisitPart(visit, adjlist, 0, -1) < numAtoms)
                            fp.Set(165, true);
                        break;
                    default:
                        if (key.Count == 0)
                        {
                            if (pattern.Matches(container))
                                fp.Set(i, true);
                        }
                        else
                        {
                            // check if there are at least 'count' unique hits, key.count = 0
                            // means find at least one match hence we add 1 to out limit
                            if (pattern.MatchAll(container).GetUniqueAtoms().AtLeast(key.Count + 1))
                                fp.Set(i, true);
                        }
                        break;
                }
            }

            // Ring Bits

            // threshold=126, see AllRingsFinder.Threshold.PubChem_97
            if (numAtoms > 2)
            {
                AllCycles allcycles = new AllCycles(adjlist,
                                                    Math.Min(8, numAtoms),
                                                    126);
                int numArom = 0;
                foreach (int[] path in allcycles.GetPaths())
                {
                    // length is +1 as we repeat the closure vertex
                    switch (path.Length)
                    {
                        case 4: // 3M bit22
                            fp.Set(21, true);
                            break;
                        case 5: // 4M bit11
                            fp.Set(10, true);
                            break;
                        case 6: // 5M bit96
                            fp.Set(95, true);
                            break;
                        case 7: // 6M bit163->bit145, bit124 numArom > 1

                            if (numArom < 2)
                            {
                                if (IsAromPath(path, bmap))
                                {
                                    numArom++;
                                    if (numArom == 2)
                                        fp.Set(124, true);
                                }
                            }

                            if (fp[162])
                            {
                                fp.Set(144, true); // >0
                            }
                            else
                            {
                                fp.Set(162, true); // >1
                            }
                            break;
                        case 8: // 7M bit19
                            fp.Set(18, true);
                            break;
                        case 9: // 8M bit101
                            fp.Set(100, true);
                            break;
                    }
                }
            }

            return new BitSetFingerprint(fp);
        }

        private static int VisitPart(bool[] visit, int[][] g, int beg, int prev)
        {
            visit[beg] = true;
            int visited = 1;
            foreach (int end in g[beg])
            {
                if (end != prev && !visit[end])
                    visited += VisitPart(visit, g, end, beg);
            }
            return visited;
        }

        private static bool IsAromPath(int[] path, GraphUtil.EdgeToBondMap bmap)
        {
            int end = path.Length - 1;
            for (int i = 0; i < end; i++)
            {
                if (!bmap[path[i], path[i + 1]].IsAromatic)
                    return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public override IDictionary<string, int> GetRawFingerprint(IAtomContainer iAtomContainer)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override int Count => 166;

        private IList<MaccsKey> ReadKeyDef(IChemObjectBuilder builder)
        {
            List<MaccsKey> keys = new List<MaccsKey>(166);
            var reader = new StreamReader(ResourceLoader.GetAsStream(GetType(), KeyDefinitions));

            // now process the keys
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line[0] == '#') continue;
                string data = line.Substring(0, line.IndexOf('|')).Trim();
                var toks = Strings.Tokenize(data);

                keys.Add(new MaccsKey(toks[1], CreatePattern(toks[1], builder), int.Parse(toks[2])));
            }
            if (keys.Count != 166) throw new CDKException("Found " + keys.Count + " keys during setup. Should be 166");
            return keys;
        }

        private class MaccsKey
        {
            private string smarts;
            private int count;
            private Pattern pattern;

            public MaccsKey(string smarts, Pattern pattern, int count)
            {
                this.smarts = smarts;
                this.pattern = pattern;
                this.count = count;
            }

            public string Smarts => smarts;

            public int Count => count;

            public Pattern Pattern => pattern;
        }

        /// <inheritdoc/>
        public override ICountFingerprint GetCountFingerprint(IAtomContainer container)
        {
            throw new NotSupportedException();
        }

        private readonly object syncLock = new object();

        /// <summary>
        /// Access MACCS keys definitions.
        /// </summary>
        /// <returns>array of MACCS keys.</returns>
        /// <exception cref="CDKException">maccs keys could not be loaded</exception>
        private IList<MaccsKey> GetKeys(IChemObjectBuilder builder)
        {
            var result = keys;
            if (result == null)
            {
                lock (syncLock)
                {
                    result = keys;
                    if (result == null)
                    {
                        try
                        {
                            keys = result = ReadKeyDef(builder);
                        }
                        catch (IOException e)
                        {
                            throw new CDKException("could not read MACCS definitions", e);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Create a pattern for the provided SMARTS - if the SMARTS is '?' a pattern
        /// is not created.
        /// </summary>
        /// <param name="smarts">a smarts pattern</param>
        /// <param name="builder">chem object builder</param>
        /// <returns>the pattern to match</returns>
        private Pattern CreatePattern(string smarts, IChemObjectBuilder builder)
        {
            if (smarts.Equals("[!0]")) return null; // FIXME can't be parsed by our SMARTS Grammar ATM
            return VentoFoggia.FindSubstructure(SMARTSParser.Parse(smarts, builder));
        }
    }
}
