/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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

using System.Collections.Generic;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    /// Class with convenience methods that provide methods from
    /// methods from ChemObjects within the ChemSequence.
    /// </summary>
    /// <seealso cref="IAtomContainer.RemoveAtomAndConnectedElectronContainers(IAtom)"/>
    // @cdk.module standard
    // @cdk.githash
    public static class ChemSequenceManipulator
    {
        /// <summary>
        /// Get the total number of atoms inside an IChemSequence.
        /// </summary>
        /// <param name="sequence">The IChemSequence object.</param>
        /// <returns>The number of Atom objects inside.</returns>
        public static int GetAtomCount(IChemSequence sequence)
        {
            int count = 0;
            for (int i = 0; i < sequence.Count; i++)
            {
                count += ChemModelManipulator.GetAtomCount(sequence[i]);
            }
            return count;
        }

        /// <summary>
        /// Get the total number of bonds inside an IChemSequence.
        ///
        /// <param name="sequence">The IChemSequence object.</param>
        /// <returns>The number of Bond objects inside.</returns>
        /// </summary>
        public static int GetBondCount(IChemSequence sequence)
        {
            int count = 0;
            for (int i = 0; i < sequence.Count; i++)
            {
                count += ChemModelManipulator.GetBondCount(sequence[i]);
            }
            return count;
        }

        /// <summary>
        /// Returns all the AtomContainer's of a ChemSequence.
        /// </summary>
        public static List<IAtomContainer> GetAllAtomContainers(IChemSequence sequence)
        {
            List<IAtomContainer> acList = new List<IAtomContainer>();
            foreach (var model in sequence)
            {
                acList.AddRange(ChemModelManipulator.GetAllAtomContainers(model));
            }
            return acList;
        }

        /// <summary>
        /// Returns a List of all IChemObject inside a ChemSequence.
        /// </summary>
        /// <returns>A List of all ChemObjects.</returns>
        public static List<IChemObject> GetAllChemObjects(IChemSequence sequence)
        {
            List<IChemObject> list = new List<IChemObject>();
            // list.Add(sequence);
            for (int i = 0; i < sequence.Count; i++)
            {
                list.Add(sequence[i]);
                List<IChemObject> current = ChemModelManipulator.GetAllChemObjects(sequence[i]);
                foreach (var chemObject in current)
                {
                    if (!list.Contains(chemObject)) list.Add(chemObject);
                }

            }
            return list;
        }

        public static IList<string> GetAllIDs(IChemSequence sequence)
        {
            List<string> list = new List<string>();
            if (sequence.Id != null) list.Add(sequence.Id);
            for (int i = 0; i < sequence.Count; i++)
            {
                list.AddRange(ChemModelManipulator.GetAllIDs(sequence[i]));
            }
            return list;
        }
    }
}

