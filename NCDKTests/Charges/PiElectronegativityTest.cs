/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@yahoo.es>
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
using NCDK.Default;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.Charges
{
    // @cdk.module test-charges
    [TestClass()]
    public class PiElectronegativityTest : CDKTestCase
    {
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        private LonePairElectronChecker lpcheck = new LonePairElectronChecker();

        /// <summary>
        /// Constructor of the PiElectronegativityTest.
        /// </summary>
        public PiElectronegativityTest()
            : base()
        { }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativity()
        {

            Assert.IsNotNull(new PiElectronegativity());
        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativity_Int_Int()
        {

            Assert.IsNotNull(new PiElectronegativity(6, 50));
        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        ///  @cdk.inchi InChI=1/C4H8/c1-3-4-2/h3H,1,4H2,2H3
        ///
        /// <returns>The test suite</returns>
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestCalculatePiElectronegativity_IAtomContainer_IAtom()
        {

            PiElectronegativity pe = new PiElectronegativity();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("F"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                if (i == 0)
                    Assert.AreNotSame(0.0, pe.CalculatePiElectronegativity(molecule, molecule.Atoms[i]));
                else
                    Assert.AreEqual(0.0, pe.CalculatePiElectronegativity(molecule, molecule.Atoms[i]), 0.001);

            }
        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        ///  @cdk.inchi InChI=1/C4H8/c1-3-4-2/h3H,1,4H2,2H3
        ///
        /// <returns>The test suite</returns>
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestCalculatePiElectronegativity_IAtomContainer_IAtom_Int_Int()
        {

            PiElectronegativity pe = new PiElectronegativity();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("F"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                if (i == 0)
                    Assert.AreNotSame(0.0, pe.CalculatePiElectronegativity(molecule, molecule.Atoms[i], 6, 50));
                else
                    Assert.AreEqual(0.0, pe.CalculatePiElectronegativity(molecule, molecule.Atoms[i], 6, 50), 0.001);

            }
        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestGetMaxIterations()
        {
            PiElectronegativity pe = new PiElectronegativity();
            Assert.AreEqual(6, pe.MaxIterations);
        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestGetMaxResonStruc()
        {
            PiElectronegativity pe = new PiElectronegativity();
            Assert.AreEqual(50, pe.MaxResonanceStructures);
        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestSetMaxIterations_Int()
        {
            PiElectronegativity pe = new PiElectronegativity();
            int maxIter = 10;
            pe.MaxIterations = maxIter;
            Assert.AreEqual(maxIter, pe.MaxIterations);
        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestSetMaxResonStruc_Int()
        {
            PiElectronegativity pe = new PiElectronegativity();
            int maxRes = 10;
            pe.MaxResonanceStructures = maxRes;
            Assert.AreEqual(maxRes, pe.MaxResonanceStructures);
        }
    }
}
