/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by the
 * Free Software Foundation; either version 2.1 of the License, or (at your
 * option) any later version. All we ask is that proper credit is given for our
 * work, which includes - but is not limited to - adding the above copyright
 * notice to the beginning of your source code files, and to any copyright
 * notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using System.Linq;

namespace NCDK.RingSearches
{
    /// <summary>
    /// benzylbenzene ring search unit tests
    /// </summary>
    // @author John May
    // @cdk.module test-standard
    [TestClass()]
    public sealed class RingSearchTest_BenzylBenzene
    {
        private readonly IAtomContainer benzylbenzene = Benzylbenzene();

        [TestMethod()]
        public void TestCyclic()
        {
            Assert.AreEqual(benzylbenzene.Atoms.Count - 1, new RingSearch(benzylbenzene).Cyclic().Length);
        }

        [TestMethod()]
        public void TestCyclic_Int()
        {
            var n = benzylbenzene.Atoms.Count;
            var ringSearch = new RingSearch(benzylbenzene);

            int cyclic = 0, acyclic = 0;
            for (int i = 0; i < n; i++)
            {
                if (ringSearch.Cyclic(i))
                    cyclic++;
                else
                    acyclic++;
            }

            // single atom not in a ring
            Assert.AreEqual(1, acyclic);
            Assert.AreEqual(n - 1, cyclic);
        }

        [TestMethod()]
        public void TestIsolated()
        {
            var search = new RingSearch(benzylbenzene);
            var isolated = search.Isolated();
            Assert.AreEqual(2, isolated.Length);
            Assert.AreEqual(6, isolated[0].Length);
            Assert.AreEqual(6, isolated[1].Length);
        }

        [TestMethod()]
        public void TestFused()
        {
            Assert.AreEqual(0, new RingSearch(benzylbenzene).Fused().Length);
        }

        [TestMethod()]
        public void TestRingFragments()
        {
            var fragment = new RingSearch(benzylbenzene).RingFragments();
            Assert.AreEqual(benzylbenzene.Atoms.Count - 1, fragment.Atoms.Count);
            Assert.AreEqual(benzylbenzene.Bonds.Count - 2, fragment.Bonds.Count);
        }

        [TestMethod()]
        public void TestIsolatedRingFragments()
        {
            var search = new RingSearch(benzylbenzene);
            var isolated = search.IsolatedRingFragments().ToList();
            Assert.AreEqual(2, isolated.Count);
            Assert.AreEqual(6, isolated[0].Atoms.Count);
            Assert.AreEqual(6, isolated[0].Bonds.Count);
            Assert.AreEqual(6, isolated[1].Atoms.Count);
            Assert.AreEqual(6, isolated[1].Bonds.Count);
        }

        [TestMethod()]
        public void TestFusedRingFragments()
        {
            var search = new RingSearch(benzylbenzene);
            var fused = search.FusedRingFragments();
            Assert.AreEqual(0, fused.Count());
        }

        // @cdk.inchi InChI=1S/C13H12/c1-3-7-12(8-4-1)11-13-9-5-2-6-10-13/h1-10H,11H2
        public static IAtomContainer Benzylbenzene()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = new Atom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = new Atom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = new Atom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = new Atom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = new Atom("C");
            mol.Atoms.Add(a5);
            IAtom a6 = new Atom("C");
            mol.Atoms.Add(a6);
            IAtom a7 = new Atom("C");
            mol.Atoms.Add(a7);
            IAtom a8 = new Atom("C");
            mol.Atoms.Add(a8);
            IAtom a9 = new Atom("C");
            mol.Atoms.Add(a9);
            IAtom a10 = new Atom("C");
            mol.Atoms.Add(a10);
            IAtom a11 = new Atom("C");
            mol.Atoms.Add(a11);
            IAtom a12 = new Atom("C");
            mol.Atoms.Add(a12);
            IAtom a13 = new Atom("C");
            mol.Atoms.Add(a13);
            IBond b1 = new Bond(a6, a7, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = new Bond(a7, a8, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = new Bond(a6, a5, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = new Bond(a5, a4, BondOrder.Double);
            mol.Bonds.Add(b4);
            IBond b5 = new Bond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = new Bond(a3, a2, BondOrder.Double);
            mol.Bonds.Add(b6);
            IBond b7 = new Bond(a6, a1, BondOrder.Double);
            mol.Bonds.Add(b7);
            IBond b8 = new Bond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = new Bond(a10, a11, BondOrder.Double);
            mol.Bonds.Add(b9);
            IBond b10 = new Bond(a10, a9, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = new Bond(a9, a8, BondOrder.Double);
            mol.Bonds.Add(b11);
            IBond b12 = new Bond(a8, a13, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = new Bond(a13, a12, BondOrder.Double);
            mol.Bonds.Add(b13);
            IBond b14 = new Bond(a12, a11, BondOrder.Single);
            mol.Bonds.Add(b14);
            return mol;
        }
    }
}
