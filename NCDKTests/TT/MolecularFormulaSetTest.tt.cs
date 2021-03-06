



/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Formula;

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the <see cref="MolecularFormulaSet"/>.
    /// </summary>
    [TestClass()]
    public class MolecularFormulaSetTest : AbstractMolecularFormulaSetTest
    {
        protected override IChemObjectBuilder Builder { get; } = CDK.Builder;

        [TestMethod()]
        public void TestMolecularFormulaSet()
        {
            IMolecularFormulaSet mfS = new MolecularFormulaSet();
            Assert.IsNotNull(mfS);
        }

        [TestMethod()]
        public void TestMolecularFormulaSet_IMolecularFormula()
        {
            IMolecularFormulaSet mfS = new MolecularFormulaSet(Builder.NewMolecularFormula());
            Assert.AreEqual(1, mfS.Count);
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="MolecularFormulaSet"/>.
    /// </summary>
    [TestClass()]
    public class MolecularFormulaSetTest : AbstractMolecularFormulaSetTest
    {
        protected override IChemObjectBuilder Builder { get; } = CDK.Builder;

        [TestMethod()]
        public void TestMolecularFormulaSet()
        {
            IMolecularFormulaSet mfS = new MolecularFormulaSet();
            Assert.IsNotNull(mfS);
        }

        [TestMethod()]
        public void TestMolecularFormulaSet_IMolecularFormula()
        {
            IMolecularFormulaSet mfS = new MolecularFormulaSet(Builder.NewMolecularFormula());
            Assert.AreEqual(1, mfS.Count);
        }
    }
}
