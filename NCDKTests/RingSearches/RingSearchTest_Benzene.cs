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
using NCDK.Templates;
using System.Linq;

namespace NCDK.RingSearches
{
    /// <summary>
    /// ring search unit tests for benzene
    /// </summary>
    // @author John May
    // @cdk.module test-standard
    [TestClass()]
    public sealed class RingSearchTest_Benzene
    {
        private readonly IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

        [TestMethod()]
        public void TestCyclic()
        {
            Assert.AreEqual(benzene.Atoms.Count, new RingSearch(benzene).Cyclic().Length);
        }

        [TestMethod()]
        public void TestCyclic_Int()
        {
            var n = benzene.Atoms.Count;
            var ringSearch = new RingSearch(benzene);
            for (int i = 0; i < n; i++)
            {
                Assert.IsTrue(ringSearch.Cyclic(i));
            }
        }

        [TestMethod()]
        public void TestIsolated()
        {
            var search = new RingSearch(benzene);
            var isolated = search.Isolated();
            Assert.AreEqual(1, isolated.Length);
            Assert.AreEqual(6, isolated[0].Length);
        }

        [TestMethod()]
        public void TestFused()
        {
            Assert.AreEqual(0, new RingSearch(benzene).Fused().Length);
        }

        [TestMethod()]
        public void TestRingFragments()
        {
            var fragment = new RingSearch(benzene).RingFragments();
            Assert.AreEqual(benzene.Atoms.Count, fragment.Atoms.Count);
            Assert.AreEqual(benzene.Bonds.Count, fragment.Bonds.Count);
        }

        [TestMethod()]
        public void TestIsolatedRingFragments()
        {
            var search = new RingSearch(benzene);
            var isolated = search.IsolatedRingFragments().ToReadOnlyList();
            Assert.AreEqual(1, isolated.Count);
            Assert.AreEqual(6, isolated[0].Atoms.Count);
            Assert.AreEqual(6, isolated[0].Bonds.Count);
        }

        [TestMethod()]
        public void TestFusedRingFragments()
        {
            var search = new RingSearch(benzene);
            var fused = search.FusedRingFragments();
            Assert.AreEqual(0, fused.Count());
        }
    }
}
