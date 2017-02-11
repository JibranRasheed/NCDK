/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.AtomTypes;
using NCDK.Default;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Reactions.Types.Parameters;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /**
     * TestSuite that runs a test for the SharingChargeDBReactionTest.
     * Generalized Reaction: [A+]=B => A|-[B+].
     *
     * @cdk.module test-reaction
     */
    [TestClass()]
    public class SharingChargeDBReactionTest : ReactionProcessTest
    {

        private readonly LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /**
         *  The JUnit setup method
         */
        public SharingChargeDBReactionTest()
        {
            SetReaction(typeof(SharingChargeDBReaction));
        }

        /**
         *  The JUnit setup method
         */
        [TestMethod()]
        public void TestSharingChargeDBReaction()
        {
            IReactionProcess type = new SharingChargeDBReaction();
            Assert.IsNotNull(type);
        }

        /**
         * A unit test suite for JUnit. Reaction: C-C=[O+] => C-[C+]O|
         * Automatic search of the center active.
         *
         * @return    The test suite
         */
        [TestMethod()]

        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            IReactionProcess type = new SharingChargeDBReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* initiate */

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[0].Products[0];
            Assert.AreEqual(1, product.Atoms[1].FormalCharge.Value);
            Assert.AreEqual(0, product.GetConnectedLonePairs(molecule.Atoms[1]).Count());

            /* C[C+]O| */
            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        /**
         * A unit test suite for JUnit. Reaction: C-C=[O+] => C-[C+]O|
         * Manually put of the center active.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestManuallyCentreActive()
        {
            IReactionProcess type = new SharingChargeDBReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually put the center active */
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */

            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[0].Products[0];

            /* C-[C+]O| */
            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new SharingChargeDBReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually put the reactive center */
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[1].IsReactiveCenter);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new SharingChargeDBReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* automatic search of the center active */
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            /* initiate */

            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer product = setOfReactions[0].Products[0];

            Assert.AreEqual(8, setOfReactions[0].Mappings.Count);

            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA1, product.Atoms[1]);
            mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[2]);
            Assert.AreEqual(mappedProductA1, product.Atoms[2]);

        }

        /**
         * Test to recognize if this IAtomContainer_1 matches correctly into the CDKAtomTypes.
         */
        [TestMethod()]
        public void TestAtomTypesAtomContainer1()
        {
            IAtomContainer moleculeTest = GetExampleReactants()[0];
            MakeSureAtomTypesAreRecognized(moleculeTest);

        }

        /**
         * Test to recognize if this IAtomContainer_2 matches correctly into the CDKAtomTypes.
         */
        [TestMethod()]
        public void TestAtomTypesAtomContainer2()
        {
            IAtomContainer moleculeTest = GetExpectedProducts()[0];
            MakeSureAtomTypesAreRecognized(moleculeTest);

        }

        /**
         * get the molecule 1: C-C=[O+]
         *
         * @return The IAtomContainerSet
         */
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();

            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("C"));
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Add(builder.CreateAtom("O"));
            molecule.Atoms[2].FormalCharge = 1;
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            try
            {
                AddExplicitHydrogens(molecule);

                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                lpcheck.Saturate(molecule);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }
            setOfReactants.Add(molecule);
            return setOfReactants;
        }

        /**
         * Get the expected set of molecules.
         *
         * @return The IAtomContainerSet
         */
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.CreateAtomContainerSet();
            //C[C+]O|
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("C"));
            molecule.Add(builder.CreateAtom("C"));
            molecule.Atoms[1].FormalCharge = 1;
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Add(builder.CreateAtom("O"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);

            try
            {
                AddExplicitHydrogens(molecule);

                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                lpcheck.Saturate(molecule);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }
            setOfProducts.Add(molecule);
            return setOfProducts;
        }

        /**
         * Test to recognize if a IAtomContainer matcher correctly identifies the CDKAtomTypes.
         *
         * @param molecule          The IAtomContainer to analyze
         * @throws CDKException
         */
        private void MakeSureAtomTypesAreRecognized(IAtomContainer molecule)
        {
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(molecule.Builder);

            foreach (var nextAtom in molecule.Atoms)
            {
                Assert.IsNotNull(matcher.FindMatchingAtomType(molecule, nextAtom), "Missing atom type for: " + nextAtom);
            }
        }

    }
}
