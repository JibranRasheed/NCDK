/* Copyright (C) 2012  Gilleain Torrance <gilleain.torrance@gmail.com>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Groups
{
    /**
     * A tool for determining the automorphism group of the atoms in a molecule, or
     * for checking for a canonical form of a molecule.
     *
     * If two atoms are equivalent under an automorphism in the group, then
     * roughly speaking they are in symmetric positions in the molecule. For
     * example, the C atoms in two methyl groups attached to a benzene ring
     * are 'equivalent' in this sense.
     *
     * <p>There are a couple of ways to use it - firstly, get the automorphisms.</p>
     *
     * <code>
     *     IAtomContainer ac = ... // get an atom container somehow
     *     AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
     *     PermutationGroup autG = refiner.GetAutomorphismGroup(ac);
     *     foreach (var automorphism in autG.All()) {
     *         ... // do something with the permutation
     *     }
     * </code>
     *
     * <p>Another is to check an atom container to see if it is canonical:</p>
     *
     * <code>
     *     IAtomContainer ac = ... // get an atom container somehow
     *     AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
     *     if (refiner.IsCanonical(ac)) {
     *         ... // do something with the atom container
     *     }
     * </code>
     *
     * Note that it is not necessary to call {@link #Refine(IAtomContainer)} before
     * either of these methods. However if both the group and the canonical check
     * are required, then the code should be:
     *
     * <code>
     *     AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
     *     refiner.Refine(ac);
     *     bool isCanon = refiner.IsCanonical();
     *     PermutationGroup autG = refiner.GetAutomorphismGroup();
     * </code>
     *
     * This way, the refinement is not carried out multiple times. Finally, remember
     * to call {@link #reset} if the refiner is re-used on multiple structures.
     *
     * @author maclean
     * @cdk.module group
     */
    public class AtomDiscretePartitionRefiner : AbstractDiscretePartitionRefiner
    {

        /**
         * A convenience lookup table for atom-atom connections.
         */
        private int[][] connectionTable;

        /**
         * A convenience lookup table for bond orders.
         */
        private int[][] bondOrders;

        /**
         * Specialised option to allow generating automorphisms
         * that ignore the element symbols.
         */
        private bool ignoreElements;

        /**
         * Specialised option to allow generating automorphisms
         * that ignore the bond order.
         */
        private bool ignoreBondOrders;

        /**
         * Default constructor - does not ignore elements or bond orders
         * or bond orders.
         */
        public AtomDiscretePartitionRefiner()
            : this(false, false)
        { }

        /**
         * Make a refiner with various advanced options.
         *
         * @param ignoreElements ignore element symbols when making automorphisms
         * @param ignoreBondOrders ignore bond order when making automorphisms
         */
        public AtomDiscretePartitionRefiner(bool ignoreElements, bool ignoreBondOrders)
        {
            this.ignoreElements = ignoreElements;
            this.ignoreBondOrders = ignoreBondOrders;
        }

        public override int GetVertexCount()
        {
            return connectionTable.Length;
        }

        public override int GetConnectivity(int i, int j)
        {
            int indexInRow;
            int maxRowIndex = connectionTable[i].Length;
            for (indexInRow = 0; indexInRow < maxRowIndex; indexInRow++)
            {
                if (connectionTable[i][indexInRow] == j)
                {
                    break;
                }
            }
            if (ignoreBondOrders)
            {
                if (indexInRow < maxRowIndex)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (indexInRow < maxRowIndex)
                {
                    return bondOrders[i][indexInRow];
                }
                else
                {
                    return 0;
                }
            }
        }

        /**
         * Used by the equitable refiner to get the indices of atoms connected to
         * the atom at <code>atomIndex</code>.
         *
         * @param atomIndex the index of the incident atom
         * @return an array of atom indices
         */
        public virtual int[] GetConnectedIndices(int atomIndex)
        {
            return connectionTable[atomIndex];
        }

        /**
         * Get the element partition from an atom container, which is simply a list
         * of sets of atom indices where all atoms in one set have the same element
         * symbol.
         *
         * So for atoms [C0, N1, C2, P3, C4, N5] the partition would be
         * [{0, 2, 4}, {1, 5}, {3}] with cells for elements C, N, and P.
         *
         * @param atomContainer the atom container to get element symbols from
         * @return a partition of the atom indices based on the element symbols
         */
        public Partition GetElementPartition(IAtomContainer atomContainer)
        {
            if (ignoreElements)
            {
                int n = atomContainer.Atoms.Count;
                return Partition.Unit(n);
            }

            if (connectionTable == null)
            {
                SetupConnectionTable(atomContainer);
            }

            var cellMap = new Dictionary<string, SortedSet<int>>();
            int numberOfAtoms = atomContainer.Atoms.Count;
            for (int atomIndex = 0; atomIndex < numberOfAtoms; atomIndex++)
            {
                string symbol = atomContainer.Atoms[atomIndex].Symbol;
                SortedSet<int> cell;
                if (cellMap.ContainsKey(symbol))
                {
                    cell = cellMap[symbol];
                }
                else
                {
                    cell = new SortedSet<int>();
                    cellMap[symbol] = cell;
                }
                cell.Add(atomIndex);
            }

            List<string> atomSymbols = new List<string>(cellMap.Keys);
            atomSymbols.Sort();

            Partition elementPartition = new Partition();
            foreach (var key in atomSymbols)
            {
                SortedSet<int> cell = cellMap[key];
                elementPartition.AddCell(cell);
            }

            return elementPartition;
        }

        /**
         * Reset the connection table.
         */
        public void Reset()
        {
            connectionTable = null;
        }

        /**
         * Refine an atom container, which has the side effect of calculating
         * the automorphism group.
         *
         * If the group is needed afterwards, call {@link #GetAutomorphismGroup()}
         * instead of {@link #GetAutomorphismGroup(IAtomContainer)} otherwise the
         * refine method will be called twice.
         *
         * @param atomContainer the atomContainer to refine
         */
        public void Refine(IAtomContainer atomContainer)
        {
            Refine(atomContainer, GetElementPartition(atomContainer));
        }

        /**
         * Refine an atom partition based on the connectivity in the atom container.
         *
         * @param atomContainer the atom container to use
         * @param partition the initial partition of the atoms
         */
        public void Refine(IAtomContainer atomContainer, Partition partition)
        {
            Setup(atomContainer);
            base.Refine(partition);
        }

        /**
         * Checks if the atom container is canonical. Note that this calls
         * {@link #refine} first.
         *
         * @param atomContainer the atom container to check
         * @return true if the atom container is canonical
         */
        public bool IsCanonical(IAtomContainer atomContainer)
        {
            Setup(atomContainer);
            base.Refine(GetElementPartition(atomContainer));
            return IsCanonical();
        }

        /**
         * Gets the automorphism group of the atom container. By default it uses an
         * initial partition based on the element symbols (so all the carbons are in
         * one cell, all the nitrogens in another, etc). If this behaviour is not
         * desired, then use the {@link #ignoreElements} flag in the constructor.
         *
         * @param atomContainer the atom container to use
         * @return the automorphism group of the atom container
         */
        public PermutationGroup GetAutomorphismGroup(IAtomContainer atomContainer)
        {
            Setup(atomContainer);
            base.Refine(GetElementPartition(atomContainer));
            return base.GetAutomorphismGroup();
        }

        /**
         * Speed up the search for the automorphism group using the automorphisms in
         * the supplied group. Note that the behaviour of this method is unknown if
         * the group does not contain automorphisms...
         *
         * @param atomContainer the atom container to use
         * @param group the group of known automorphisms
         * @return the full automorphism group
         */
        public PermutationGroup GetAutomorphismGroup(IAtomContainer atomContainer, PermutationGroup group)
        {
            Setup(atomContainer, group);
            base.Refine(GetElementPartition(atomContainer));
            return base.GetAutomorphismGroup();
        }

        /**
         * Get the automorphism group of the molecule given an initial partition.
         *
         * @param atomContainer the atom container to use
         * @param initialPartition an initial partition of the atoms
         * @return the automorphism group starting with this partition
         */
        public PermutationGroup GetAutomorphismGroup(IAtomContainer atomContainer, Partition initialPartition)
        {
            Setup(atomContainer);
            base.Refine(initialPartition);
            return base.GetAutomorphismGroup();
        }

        /**
         * Get the automorphism partition (equivalence classes) of the atoms.
         *
         * @param atomContainer the molecule to calculate equivalence classes for
         * @return a partition of the atoms into equivalence classes
         */
        public Partition GetAutomorphismPartition(IAtomContainer atomContainer)
        {
            Setup(atomContainer);
            base.Refine(GetElementPartition(atomContainer));
            return base.GetAutomorphismPartition();
        }

        /**
         * Makes a lookup table for the connection between atoms, to avoid looking
         * through the bonds each time.
         *
         * @param atomContainer the atom
         */
        private void SetupConnectionTable(IAtomContainer atomContainer)
        {
            int atomCount = atomContainer.Atoms.Count;
            connectionTable = new int[atomCount][];
            if (!ignoreBondOrders)
            {
                bondOrders = new int[atomCount][];
            }
            for (int atomIndex = 0; atomIndex < atomCount; atomIndex++)
            {
                IAtom atom = atomContainer.Atoms[atomIndex];
                var connectedAtoms = atomContainer.GetConnectedAtoms(atom);
                int numConnAtoms = connectedAtoms.Count();
                connectionTable[atomIndex] = new int[numConnAtoms];
                if (!ignoreBondOrders)
                {
                    bondOrders[atomIndex] = new int[numConnAtoms];
                }
                int i = 0;
                foreach (var connected in connectedAtoms)
                {
                    int index = atomContainer.Atoms.IndexOf(connected);
                    connectionTable[atomIndex][i] = index;
                    if (!ignoreBondOrders)
                    {
                        IBond bond = atomContainer.GetBond(atom, connected);
                        bool isArom = bond.IsAromatic;
                        int orderNumber = (isArom) ? 5 : bond.Order.Numeric;
                        bondOrders[atomIndex][i] = orderNumber;
                    }
                    i++;
                }
            }
        }

        private void Setup(IAtomContainer atomContainer)
        {
            // have to setup the connection table before making the group
            // otherwise the size may be wrong, but only setup if it doesn't exist
            if (connectionTable == null)
            {
                SetupConnectionTable(atomContainer);
            }
            int size = GetVertexCount();
            PermutationGroup group = new PermutationGroup(new Permutation(size));
            base.Setup(group, new AtomEquitablePartitionRefiner(this));
        }

        private void Setup(IAtomContainer atomContainer, PermutationGroup group)
        {
            SetupConnectionTable(atomContainer);
            base.Setup(group, new AtomEquitablePartitionRefiner(this));
        }
    }
}
