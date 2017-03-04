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
using NCDK.QSAR.Result;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Bonds
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarbond
    [TestClass()]
    public class BondPartialSigmaChargeDescriptorTest : BondDescriptorTest
    {
        public BondPartialSigmaChargeDescriptorTest()
        {
            descriptor = new BondPartialSigmaChargeDescriptor();
            SetDescriptor(typeof(BondPartialSigmaChargeDescriptor));
        }

        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor()
        {
            double[] testResult = { 0.3323, 0.0218 }; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CF");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 2; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Bonds[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Methyl chloride
        /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Methyl_chloride()
        {
            double[] testResult = { 0.2137, 0.0075 }; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCl");
            AddExplicitHydrogens(mol);
            for (int i = 0; i < 2; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Bonds[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.05);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Allyl bromide
        /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Allyl_bromide()
        {
            double[] testResult = { 0.0265, 0.1268, 0.1872, 0.1564, 0.1564, 0.1347, 0.0013, 0.0013 }; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CCBr");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 8; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Bonds[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.03);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Isopentyl iodide
        /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Isopentyl_iodide()
        {
            double testResult = 0.0165;  // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C(C)(C)CCI");
            AddExplicitHydrogens(mol);

            double result = ((DoubleResult)descriptor.Calculate(mol.Bonds[0], mol).GetValue()).Value;
            Assert.AreEqual(testResult, result, 0.001);
        }

         /// <summary>
         /// A unit test for JUnit with Ethoxy ethane
         /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Ethoxy_ethane()
        {
            double[] testResult = { 0.0864, 0.4262, 0.4262, 0.0864, 0.0662, 0.0662, 0.0662, 0.0104, 0.0104 }; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCOCC");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 8; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Bonds[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.002);
            }
        }

         /// <summary>
         /// A unit test for JUnit with Ethanolamine
         /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Ethanolamine()
        {
            double[] testResult = { 0.3463, 0.0274, 0.448, 0.448, 0.448 };  // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("NCCO");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 5; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Bonds[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.06);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Allyl mercaptan
        /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Allyl_mercaptan()
        {
            double[] testResult = { 0.0203, 0.0921, 0.1835, 0.1569, 0.3593, 8.5917 }; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CCS");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 4; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Bonds[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.005);
            }
        }
    }
}
