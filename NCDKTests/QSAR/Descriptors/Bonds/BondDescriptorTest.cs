/* Copyright (C) 2007-2008  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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

using NCDK.Common.Base;
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Diff.Tree;
using System;

namespace NCDK.QSAR.Descriptors.Bonds
{
    // @cdk.module test-qsarbond
    [TestClass()]
    public abstract class BondDescriptorTest<T> : DescriptorTest<T> where T : IBondDescriptor
    {
        [TestMethod()]
        public void TestCalculate_IBond_IAtomContainer()
        {
            var mol = SomeoneBringMeSomeWater();
            var v = CreateDescriptor(mol).Calculate(mol.Bonds[0]);
            Assert.IsNotNull(v);
            Assert.AreNotEqual(0, v.Count, "The descriptor did not calculate any value.");
        }

        /// <summary>
        /// Checks if the given labels are consistent.
        /// </summary>
        /// <exception cref="Exception">Passed on from calculate.</exception>
        [TestMethod()]
        public void TestLabels()
        {
            var mol = SomeoneBringMeSomeWater();
            var v = CreateDescriptor(mol).Calculate(mol.Bonds[0]);
            Assert.IsNotNull(v);
            var names = v.Keys.ToReadOnlyList();
            Assert.IsNotNull(names, "The descriptor must return labels using the Names method.");
            Assert.AreNotEqual(0, names.Count, "At least one label must be given.");
            for (int i = 0; i < names.Count; i++)
            {
                Assert.IsNotNull(names[i], "A descriptor label may not be null.");
                Assert.AreNotEqual(0, names[i].Length, "The label string must not be empty.");
            }
            Assert.IsNotNull(v);
            int valueCount = v.Count;
            Assert.AreEqual(names.Count, valueCount, "The number of labels must equals the number of values.");
        }

        [TestMethod()]
        public void TestCalculate_NoModifications()
        {
            var mol = SomeoneBringMeSomeWater();
            var bond = mol.Bonds[0];
            var clone = (IBond)mol.Bonds[0].Clone();
            CreateDescriptor(mol).Calculate(bond);
            var diff = BondDiff.Diff(clone, bond);
            Assert.AreEqual(0, diff.Length, $"({typeof(T).FullName}) The descriptor must not change the passed bond in any respect, but found this diff: {diff}");
        }

        private IAtomContainer SomeoneBringMeSomeWater()
        {
            var mol = CDK.Builder.NewAtomContainer();
            var c1 = CDK.Builder.NewAtom("O");
            c1.Point3D = new Vector3(0.0, 0.0, 0.0);
            var h1 = CDK.Builder.NewAtom("H");
            h1.Point3D = new Vector3(1.0, 0.0, 0.0);
            var h2 = CDK.Builder.NewAtom("H");
            h2.Point3D = new Vector3(-1.0, 0.0, 0.0);
            mol.Atoms.Add(c1);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            return mol;
        }
    }
}
