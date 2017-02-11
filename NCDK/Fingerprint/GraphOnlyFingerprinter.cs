/* Copyright (C) 2002-2007  Egon Willighagen <egonw@users.sf.net>
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
using System;
using System.Collections;

namespace NCDK.Fingerprint
{
    /**
    // Specialized version of the {@link Fingerprinter} which does not take bond orders
    // into account.
     *
    // @author         egonw
    // @cdk.created    2007-01-11
    // @cdk.keyword    fingerprint
    // @cdk.keyword    similarity
    // @cdk.module     standard
    // @cdk.githash
     *
    // @see            org.openscience.cdk.fingerprint.Fingerprinter
     */
    public class GraphOnlyFingerprinter : Fingerprinter
    {

        /**
        // Creates a fingerprint generator of length <code>defaultSize</code>
        // and with a search depth of <code>defaultSearchDepth</code>.
         */
        public GraphOnlyFingerprinter()
            : base(DEFAULT_SIZE, DEFAULT_SEARCH_DEPTH)
        { }

        public GraphOnlyFingerprinter(int size)
            : base(size, DEFAULT_SEARCH_DEPTH)
        { }

        public GraphOnlyFingerprinter(int size, int searchDepth)
                : base(size, searchDepth)
        { }

        /**
        // Gets the bondSymbol attribute of the Fingerprinter class. Because we do
        // not consider bond orders to be important, we just return "";
         *
        // @param  bond  Description of the Parameter
        // @return       The bondSymbol value
         */

        protected override string GetBondSymbol(IBond bond)
        {
            return "";
        }

        public BitArray GetBitFingerprint(IAtomContainer container, int size)
        {
            int[] hashes = FindPathes(container, base.SearchDepth);
            BitArray bitSet = new BitArray(size);
            foreach (var hash in hashes)
            {
                bitSet.Set(new Random(hash).Next(size), true);
            }
            return bitSet;
        }
    }
}
