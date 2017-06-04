/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Tools.Diff.Tree;

namespace NCDK.Tools.Diff
{
    // @cdk.module test-diff 
    public class BondDiffTest : CDKTestCase
    {
        [TestMethod()]
        public void TestMatchAgainstItself()
        {
            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            string result = BondDiff.Diff(bond1, bond1);
            AssertZeroLength(result);
        }

        [TestMethod()]
        public void TestDiff()
        {

            var m_carbon = new Mock<IAtom>(); IAtom carbon = m_carbon.Object;
            var m_oxygen = new Mock<IAtom>(); IAtom oxygen = m_oxygen.Object;

            m_carbon.SetupGet(n => n.Symbol).Returns("C");
            m_oxygen.SetupGet(n => n.Symbol).Returns("O");

            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;

            m_bond1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_bond2.SetupGet(n => n.Order).Returns(BondOrder.Double);

            m_bond1.SetupGet(n => n.Atoms.Count).Returns(2);
            m_bond2.SetupGet(n => n.Atoms.Count).Returns(2);

            m_bond1.SetupGet(n => n.Begin).Returns(carbon);
            m_bond1.SetupGet(n => n.End).Returns(carbon);
            m_bond2.SetupGet(n => n.Begin).Returns(carbon);
            m_bond2.SetupGet(n => n.End).Returns(oxygen);

            bond1.Order = BondOrder.Single;
            bond2.Order = BondOrder.Double;

            string result = BondDiff.Diff(bond1, bond2);
            Assert.IsNotNull(result);
            Assert.AreNotSame(0, result.Length);
            AssertContains(result, "BondDiff");
            AssertContains(result, "Single/DOUBLE");
            AssertContains(result, "AtomDiff");
            AssertContains(result, "C/O");
        }

        [TestMethod()]
        public void TestDifference()
        {
            var m_carbon = new Mock<IAtom>(); IAtom carbon = m_carbon.Object;
            var m_oxygen = new Mock<IAtom>(); IAtom oxygen = m_oxygen.Object;

            m_carbon.SetupGet(n => n.Symbol).Returns("C");
            m_oxygen.SetupGet(n => n.Symbol).Returns("O");

            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;

            m_bond1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_bond2.SetupGet(n => n.Order).Returns(BondOrder.Double);

            m_bond1.SetupGet(n => n.Atoms.Count).Returns(2);
            m_bond2.SetupGet(n => n.Atoms.Count).Returns(2);

            m_bond1.SetupGet(n => n.Begin).Returns(carbon);
            m_bond1.SetupGet(n => n.End).Returns(carbon);
            m_bond2.SetupGet(n => n.Begin).Returns(carbon);
            m_bond2.SetupGet(n => n.End).Returns(oxygen);

            bond1.Order = BondOrder.Single;
            bond2.Order = BondOrder.Double;

            IDifference difference = BondDiff.Difference(bond1, bond2);
            Assert.IsNotNull(difference);
        }
    }
}
