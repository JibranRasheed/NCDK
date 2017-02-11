/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.QSAR.Result;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs unit tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    // @see MannholdLogPDescriptor
    [TestClass()]
    public class MannholdLogPDescriptorTest : MolecularDescriptorTest
    {
        public MannholdLogPDescriptorTest()
        {
            SetDescriptor(typeof(MannholdLogPDescriptor));
        }

        [TestMethod()]
        public void TestMethanol()
        {
            IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
            IAtomContainer methanol = builder.CreateAtomContainer();
            methanol.Atoms.Add(builder.CreateAtom("C"));
            methanol.Atoms.Add(builder.CreateAtom("O"));
            methanol.AddBond(methanol.Atoms[0], methanol.Atoms[1], BondOrder.Single);
            IDescriptorResult result = Descriptor.Calculate(methanol).GetValue();
            Assert.IsTrue(result is DoubleResult);
            Assert.AreEqual(1.46, ((DoubleResult)result).Value, 0.01);
        }

        [TestMethod()]
        public void TestMethane()
        {
            IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
            IAtomContainer methane = builder.CreateAtomContainer();
            methane.Atoms.Add(builder.CreateAtom("C"));
            IDescriptorResult result = Descriptor.Calculate(methane).GetValue();
            Assert.IsTrue(result is DoubleResult);
            Assert.AreEqual(1.57, ((DoubleResult)result).Value, 0.01);
        }

        [TestMethod()]
        public void TestChloroform()
        {
            IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
            IAtomContainer chloroform = builder.CreateAtomContainer();
            chloroform.Atoms.Add(builder.CreateAtom("C"));
            for (int i = 0; i < 3; i++)
            {
                chloroform.Atoms.Add(builder.CreateAtom("Cl"));
                chloroform.AddBond(chloroform.Atoms[0], chloroform.Atoms[i + 1], BondOrder.Single);
            }
            IDescriptorResult result = Descriptor.Calculate(chloroform).GetValue();
            Assert.IsTrue(result is DoubleResult);
            Assert.AreEqual(1.24, ((DoubleResult)result).Value, 0.01);
        }
    }
}
