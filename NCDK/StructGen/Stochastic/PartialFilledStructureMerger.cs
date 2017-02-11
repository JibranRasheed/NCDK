/* Copyright (C) 1997-2009  Christoph Steinbeck, Stefan Kuhn <shk3@users.sf.net>
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
 */
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;
using System.Linq;

namespace NCDK.StructGen.Stochastic
{
    /**
     * Randomly generates a single, connected, correctly bonded structure from
     * a number of fragments.
     * <p>Assign hydrogen counts to each heavy atom. The hydrogens should not be
     * in the atom pool but should be assigned implicitly to the heavy atoms in
     * order to reduce computational cost.
     *
     * @author     steinbeck
     * @cdk.created    2001-09-04
     * @cdk.module     structgen
     * @cdk.githash
     */
    public class PartialFilledStructureMerger
    {
        SaturationChecker satCheck;

        /**
         * Constructor for the PartialFilledStructureMerger object.
         */
        public PartialFilledStructureMerger()
        {
            satCheck = new SaturationChecker();
        }

        /// <summary>
        /// Randomly generates a single, connected, correctly bonded structure from
        /// a number of fragments.  IMPORTANT: The AtomContainers in the set must be
        /// connected. If an AtomContainer is disconnected, no valid result will
        /// be formed
        /// </summary>
        /// <param name="atomContainers">The fragments to generate for.</param>
        /// <returns>The newly formed structure.</returns>
        /// <exception cref="CDKException">No valid result could be formed.</exception>"
        public IAtomContainer Generate(IAtomContainerSet<IAtomContainer> atomContainers)
        {
            var container = Generate2(atomContainers);
            if (container == null)
                throw new CDKException("Could not combine the fragments to combine a valid, satured structure");
            return container;
        }

        public IAtomContainer Generate2(IAtomContainerSet<IAtomContainer> atomContainers)
        {
            int iteration = 0;
            bool structureFound = false;
            do
            {
                iteration++;
                bool bondFormed;
                do
                {
                    bondFormed = false;
                    var atomContainersArray = atomContainers.ToList();
                    for (var atomContainersArrayIndex = 0; atomContainersArrayIndex < atomContainersArray.Count; atomContainersArrayIndex++)
                    {
                        var ac = atomContainersArray[atomContainersArrayIndex];
                        if (ac == null)
                            continue;

                        foreach (var atom in AtomContainerManipulator.GetAtomArray(ac))
                        {
                            if (!satCheck.IsSaturated(atom, ac))
                            {
                                IAtom partner = GetAnotherUnsaturatedNode(atom, atomContainers);
                                if (partner != null)
                                {
                                    IAtomContainer toadd = AtomContainerSetManipulator.GetRelevantAtomContainer(
                                            atomContainers, partner);
                                    double cmax1 = satCheck.GetCurrentMaxBondOrder(atom, ac);
                                    double cmax2 = satCheck.GetCurrentMaxBondOrder(partner, toadd);
                                    double max = Math.Min(cmax1, cmax2);
                                    double order = Math.Min(Math.Max(1.0, max), 3.0);//(double)Math.Round(Math.Random() * max)
                                    Debug.WriteLine($"cmax1, cmax2, max, order: {cmax1}, {cmax2}, {max}, {order}");
                                    if (toadd != ac)
                                    {
                                        var indexToRemove = atomContainersArray.IndexOf(toadd);
                                        if (indexToRemove != -1)
                                            atomContainersArray[indexToRemove] = null;
                                        atomContainers.Remove(toadd);
                                        ac.Add(toadd);
                                    }
                                    ac.Bonds.Add(ac.Builder.CreateBond(atom, partner,
                                            BondManipulator.CreateBondOrder(order)));
                                    bondFormed = true;
                                }
                            }
                        }
                    }
                } while (bondFormed);
                if (atomContainers.Count == 1
                        && satCheck.AllSaturated(atomContainers[0]))
                {
                    structureFound = true;
                }
            } while (!structureFound && iteration < 5);
            if (atomContainers.Count == 1 && satCheck.AllSaturated(atomContainers[0]))
            {
                structureFound = true;
            }
            if (!structureFound)
                return null;
            return atomContainers[0];
        }

        /**
         *  Gets a randomly selected unsaturated atom from the set. If there are any, it will be from another
         *  container than exclusionAtom.
         *
         * @return  The unsaturated atom.
         */
        private IAtom GetAnotherUnsaturatedNode(IAtom exclusionAtom, IAtomContainerSet<IAtomContainer> atomContainers)
        {
            IAtom atom;

            foreach (var ac in atomContainers)
            {
                if (!ac.Contains(exclusionAtom))
                {
                    int next = 0;//(int) (Math.Random() * ac.Atoms.Count);
                    for (int f = next; f < ac.Atoms.Count; f++)
                    {
                        atom = ac.Atoms[f];
                        if (!satCheck.IsSaturated(atom, ac) && exclusionAtom != atom
                                && !ac.GetConnectedAtoms(exclusionAtom).Contains(atom))
                        {
                            return atom;
                        }
                    }
                }
            }
            foreach (var ac in atomContainers)
            {
                int next = ac.Atoms.Count;//(int) (Math.Random() * ac.Atoms.Count);
                for (int f = 0; f < next; f++)
                {
                    atom = ac.Atoms[f];
                    if (!satCheck.IsSaturated(atom, ac) && exclusionAtom != atom
                            && !ac.GetConnectedAtoms(exclusionAtom).Contains(atom))
                    {
                        return atom;
                    }
                }
            }
            return null;
        }
    }
}
