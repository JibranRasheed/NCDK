/*  Copyright (C) 2009  Gilleain Torrance <gilleain.torrance@gmail.com>
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
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using System;
using System.Linq;
using System.Collections.Generic;
using NCDK.Common.Collections;
using System.Collections.ObjectModel;

namespace NCDK.Graphs
{
    /**
     * An atom container atom permutor that uses ranking and unranking to calculate
     * the next permutation in the series.</p>
     *
     * <p>Typical use:<pre>
     * AtomContainerAtomPermutor permutor = new AtomContainerAtomPermutor(container);
     * while (permutor.MoveNext()) {
     *   IAtomContainer permutedContainer = permutor.Next();
     *   ...
     * }</pre>
     *
     * @author maclean
     * @cdk.created 2009-09-09
     * @cdk.keyword permutation
     * @cdk.module standard
     * @cdk.githash
     */
    public class AtomContainerAtomPermutor : AtomContainerPermutor
    {
        /**
         * A permutor wraps the original atom container, and produces cloned
         * (and permuted!) copies on demand.
         *
         * @param atomContainer the atom container to permute
         */
        public AtomContainerAtomPermutor(IAtomContainer atomContainer)
            : base(atomContainer.Atoms.Count, atomContainer)
        {
        }

        /**
         * Generate the atom container with this permutation of the atoms.
         *
         * @param permutation the permutation to use
         * @return the
         */
        public override IAtomContainer ContainerFromPermutation(int[] permutation)
        {
            IAtomContainer permutedContainer = (IAtomContainer)atomContainer.Clone();
            IAtom[] atoms = new IAtom[atomContainer.Atoms.Count];
            for (int i = 0; i < atomContainer.Atoms.Count; i++)
            {
                atoms[permutation[i]] = permutedContainer.Atoms[i];
            }
            permutedContainer.SetAtoms(atoms);
            return permutedContainer;
        }
    }
}
