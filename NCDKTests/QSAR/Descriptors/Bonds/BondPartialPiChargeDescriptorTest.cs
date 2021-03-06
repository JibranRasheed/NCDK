/* Copyright (C) 2004-2008  Miguel Rojas <miguel.rojas@uni-koeln.de>
 *                          Egon Willighagen <egonw@users.sf.net>
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

namespace NCDK.QSAR.Descriptors.Bonds
{
    // @cdk.module test-qsarbond
    [TestClass()]
    public class BondPartialPiChargeDescriptorTest : BondDescriptorTest<BondPartialPiChargeDescriptor>
    {
        [TestMethod()]
        public void TestBondPiElectronegativityDescriptor()
        {
            double[] testResult = { 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CF");
            AddExplicitHydrogens(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 2; i++)
            {
                var result = descriptor.Calculate(mol.Bonds[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// A unit test with Allyl bromide
        /// </summary>
        [TestMethod()]
        public void TestBondPiElectronegativityDescriptor_Allyl_bromide()
        {
            double[] testResult = { 0.0022, 0.0011, 0.0011, 0.0011, 0.0011, 0.0, 0.0, 0.0 }; 
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CCBr");
            AddExplicitHydrogens(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 8; i++)
            {
                var result = descriptor.Calculate(mol.Bonds[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.03);
            }
        }

        /// <summary>
        /// A unit test with Isopentyl iodide
        /// </summary>
        [TestMethod()]
        public void TestBondPiElectronegativityDescriptor_Isopentyl_iodide()
        {
            double testResult = 0.0; 
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C(C)(C)CCI");
            AddExplicitHydrogens(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 6; i++)
            {
                var result = descriptor.Calculate(mol.Bonds[i]).Value;
                Assert.AreEqual(testResult, result, 0.001);
            }
        }

        /// <summary>
        /// A unit test with Allyl mercaptan
        /// </summary>
        [TestMethod()]
        public void TestBondPiElectronegativityDescriptor_Allyl_mercaptan()
        {
            double[] testResult = { 0.0006, 0.0003, 0.0003, 0.0003, 0.0003, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CCS");
            AddExplicitHydrogens(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 9; i++)
            {
                var result = descriptor.Calculate(mol.Bonds[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.03);
            }
        }
    }
}
