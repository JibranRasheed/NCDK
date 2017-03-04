/* Copyright (C) 2002-2007  Stefan Kuhn <shk3@users.sf.net>
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
using NCDK.Graphs;
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.Fingerprint
{
    /// <summary>
    /// Generates an extended fingerprint for a given <see cref="IAtomContainer"/>, that
    /// the <see cref="Fingerprinter"/> with additional bits describing ring
    /// features.
    /// </summary>
    /// <see cref="Fingerprinter"/>
    // @author         shk3
    // @cdk.created    2006-01-13
    // @cdk.keyword    fingerprint
    // @cdk.keyword    similarity
    // @cdk.module     fingerprint
    // @cdk.githash
    internal class ExtendedFingerprinter : IFingerprinter
    {

        private const int RESERVED_BITS = 25;

        private Fingerprinter fingerprinter = null;

        /// <summary>
        /// Creates a fingerprint generator of length <code>DEFAULT_SIZE</code>
        /// and with a search depth of <code>DEFAULT_SEARCH_DEPTH</code>.
        /// </summary>
        public ExtendedFingerprinter()
            : this(Fingerprinter.DEFAULT_SIZE, Fingerprinter.DEFAULT_SEARCH_DEPTH)
        { }

        public ExtendedFingerprinter(int size)
           : this(size, Fingerprinter.DEFAULT_SEARCH_DEPTH)
        { }

        /// <summary>
        /// Constructs a fingerprint generator that creates fingerprints of
        /// the given size, using a generation algorithm with the given search
        /// depth.
        /// </summary>
        /// <param name="size">The desired size of the fingerprint</param>
        /// <param name="searchDepth">The desired depth of search</param>
        public ExtendedFingerprinter(int size, int searchDepth)
        {
            this.fingerprinter = new Fingerprinter(size - RESERVED_BITS, searchDepth);
        }

        /// <summary>
        /// Generates a fingerprint of the default size for the given
        /// AtomContainer, using path and ring metrics. It contains the
        /// informations from GetBitFingerprint() and bits which tell if the structure
        /// has 0 rings, 1 or less rings, 2 or less rings ... 10 or less rings
        /// (referring to smallest set of smallest rings) and bits which tell if
        /// there is a fused ring system with 1,2...8 or more rings in it
        /// </summary>
        /// <param name="container">The AtomContainer for which a Fingerprint is generated</param>
        /// <returns>a bit fingerprint for the given <see cref="IAtomContainer"/>.</returns>
        public IBitFingerprint GetBitFingerprint(IAtomContainer container)
        {
            return this.GetBitFingerprint(container, null, null);
        }

        /// <inheritdoc/>
        public IDictionary<string, int> GetRawFingerprint(IAtomContainer iAtomContainer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Generates a fingerprint of the default size for the given
        /// AtomContainer, using path and ring metrics. It contains the
        /// informations from GetBitFingerprint() and bits which tell if the structure
        /// has 0 rings, 1 or less rings, 2 or less rings ... 10 or less rings and
        /// bits which tell if there is a fused ring system with 1,2...8 or more
        /// rings in it. The RingSet used is passed via rs parameter. This must be
        /// a smallesSetOfSmallestRings. The List must be a list of all ring
        /// systems in the molecule.
        /// </summary>
        /// <param name="atomContainer">The AtomContainer for which a Fingerprint is generated</param>
        /// <param name="ringSet">An SSSR RingSet of ac (if not available, use GetExtendedFingerprint(AtomContainer ac), which does the calculation)</param>
        /// <param name="rslist">A list of all ring systems in ac</param>
        /// <exception cref="CDKException">for example if input can not be cloned.</exception>
        /// <returns>a BitArray representing the fingerprint</returns>
        public IBitFingerprint GetBitFingerprint(IAtomContainer atomContainer, IRingSet ringSet, IList<IRingSet> rslist)
        {
            IAtomContainer container;
            container = (IAtomContainer)atomContainer.Clone();

            IBitFingerprint fingerprint = fingerprinter.GetBitFingerprint(container);
            int size = this.Count;
            double weight = MolecularFormulaManipulator.GetTotalNaturalAbundance(MolecularFormulaManipulator
                    .GetMolecularFormula(container));
            for (int i = 1; i < 11; i++)
            {
                if (weight > (100 * i)) fingerprint.Set(size - 26 + i); // 26 := RESERVED_BITS+1
            }
            if (ringSet == null)
            {
                ringSet = Cycles.SSSR(container).ToRingSet();
                rslist = RingPartitioner.PartitionRings(ringSet);
            }
            for (int i = 0; i < 7; i++)
            {
                if (ringSet.Count > i) fingerprint.Set(size - 15 + i); // 15 := RESERVED_BITS+1+10 mass bits
            }
            int maximumringsystemsize = 0;
            for (int i = 0; i < rslist.Count; i++)
            {
                if (((IRingSet)rslist[i]).Count > maximumringsystemsize)

                    maximumringsystemsize = ((IRingSet)rslist[i]).Count;
            }
            for (int i = 0; i < maximumringsystemsize && i < 9; i++)
            {
                fingerprint.Set(size - 8 + i - 3);
            }
            return fingerprint;
        }

        /// <inheritdoc/>

        public int Count => fingerprinter.Count + RESERVED_BITS;

        /// <inheritdoc/>

        public ICountFingerprint GetCountFingerprint(IAtomContainer container)
        {
            throw new NotSupportedException();
        }
    }
}
