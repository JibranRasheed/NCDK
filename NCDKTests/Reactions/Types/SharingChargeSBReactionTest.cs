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
    /// <summary>
    /// TestSuite that runs a test for the SharingChargeDBReactionTest.
    /// Generalized Reaction: [A+]=B => A| + [B+].
    ///
    // @cdk.module test-reaction
    /// </summary>
    [TestClass()]
    public class SharingChargeSBReactionTest : ReactionProcessTest
    {

        private readonly LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        public SharingChargeSBReactionTest()
        {
            SetReaction(typeof(SharingChargeSBReaction));
        }

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        [TestMethod()]
        public void TestSharingChargeSBReaction()
        {
            IReactionProcess type = new SharingChargeSBReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction:  methoxymethane.
        /// C[O+]!-!C =>  CO + [C+]
        ///
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]

        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            IReactionProcess type = new SharingChargeSBReaction();

            var setOfReactants = GetExampleReactants();

            /* initiate */

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(3, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            IAtomContainer product1 = setOfReactions[1].Products[0];

            IAtomContainer molecule1 = GetExpectedProducts()[0];
            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule1, queryAtom));

            IAtomContainer product2 = setOfReactions[0].Products[1];
            IAtomContainer expected2 = GetExpectedProducts()[0];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product2, queryAtom));

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction:  methoxymethane.
        /// C[O+]!-!C =>  CO + [C+]
        /// Manually put of the center active.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestManuallyCentreActive()
        {
            IReactionProcess type = new SharingChargeSBReaction();

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
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            IAtomContainer product1 = setOfReactions[0].Products[0];
            IAtomContainer molecule1 = GetExpectedProducts()[0];
            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule1, queryAtom));

            IAtomContainer product2 = setOfReactions[0].Products[1];
            IAtomContainer expected2 = GetExpectedProducts()[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product2, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new SharingChargeSBReaction();

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

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new SharingChargeSBReaction();

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

            IAtomContainer product1 = setOfReactions[0].Products[0];
            IAtomContainer product2 = setOfReactions[0].Products[1];

            Assert.AreEqual(10, setOfReactions[0].Mappings.Count);

            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA1, product1.Atoms[1]);
            mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[2]);
            Assert.AreEqual(mappedProductA1, product2.Atoms[0]);

        }

        /// <summary>
        /// Test to recognize if this IAtomContainer_1 matches correctly into the CDKAtomTypes.
        /// </summary>
        [TestMethod()]
        public void TestAtomTypesAtomContainer1()
        {
            IAtomContainer moleculeTest = GetExampleReactants()[0];
            MakeSureAtomTypesAreRecognized(moleculeTest);

        }

        /// <summary>
        /// Test to recognize if this IAtomContainer_2 matches correctly into the CDKAtomTypes.
        /// </summary>
        [TestMethod()]
        public void TestAtomTypesAtomContainer2()
        {
            IAtomContainer moleculeTest = GetExpectedProducts()[0];
            MakeSureAtomTypesAreRecognized(moleculeTest);

        }

        /// <summary>
        /// get the molecule 1: C[O+]!-!C
        ///
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();

            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("C"));
            molecule.Add(builder.CreateAtom("O"));
            molecule.Atoms[1].FormalCharge = +1;
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[7], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[8], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[9], BondOrder.Single);
            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                lpcheck.Saturate(molecule);
            }
            catch (CDKException e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            setOfReactants.Add(molecule);
            return setOfReactants;
        }

        /// <summary>
        /// Get the expected set of molecules.
        ///
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.CreateAtomContainerSet();

            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Add(builder.CreateAtom("C"));
            expected1.Add(builder.CreateAtom("O"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[5], BondOrder.Single);
            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
                lpcheck.Saturate(expected1);
            }
            catch (CDKException e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            IAtomContainer expected2 = builder.CreateAtomContainer();
            expected2.Add(builder.CreateAtom("C"));
            expected2.Atoms[0].FormalCharge = +1;
            expected2.Add(builder.CreateAtom("H"));
            expected2.Add(builder.CreateAtom("H"));
            expected2.Add(builder.CreateAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);
            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            }
            catch (CDKException e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            setOfProducts.Add(expected1);
            setOfProducts.Add(expected2);
            return setOfProducts;
        }

        /// <summary>
        /// Test to recognize if a IAtomContainer matcher correctly identifies the CDKAtomTypes.
        ///
        /// <param name="molecule">The IAtomContainer to analyze</param>
        // @throws CDKException
        /// </summary>
        private void MakeSureAtomTypesAreRecognized(IAtomContainer molecule)
        {
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(molecule.Builder);
            foreach (var nextAtom in molecule.Atoms)
            {
                Assert.IsNotNull(matcher.FindMatchingAtomType(molecule, nextAtom), "Missing atom type for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction:.
        /// C[N+]!-!C => CN + [C+]
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestNsp3ChargeSingleB()
        {
            //CreateFromSmiles("C[N+]C")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("C"));
            molecule.Add(builder.CreateAtom("N"));
            molecule.Atoms[1].FormalCharge = +1;
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[7], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[8], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[9], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[10], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new SharingChargeSBReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("CN")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Add(builder.CreateAtom("C"));
            expected1.Add(builder.CreateAtom("N"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[6], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C+]")
            IAtomContainer expected2 = builder.CreateAtomContainer();
            expected2.Add(builder.CreateAtom("C"));
            expected2.Atoms[0].FormalCharge = +1;
            expected2.Add(builder.CreateAtom("H"));
            expected2.Add(builder.CreateAtom("H"));
            expected2.Add(builder.CreateAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            IAtomContainer product2 = setOfReactions[0].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product2, queryAtom));

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction:.
        /// C=[N+]!-!C => C=N + [C+]
        ///
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestNsp2ChargeSingleB()
        {
            //CreateFromSmiles("C=[N+]C")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("C"));
            molecule.Add(builder.CreateAtom("N"));
            molecule.Atoms[1].FormalCharge = 1;
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[7], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[8], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new SharingChargeSBReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C=N")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Add(builder.CreateAtom("C"));
            expected1.Add(builder.CreateAtom("N"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[4], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C+]")
            IAtomContainer expected2 = builder.CreateAtomContainer();
            expected2.Add(builder.CreateAtom("C"));
            expected2.Atoms[0].FormalCharge = +1;
            expected2.Add(builder.CreateAtom("H"));
            expected2.Add(builder.CreateAtom("H"));
            expected2.Add(builder.CreateAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            IAtomContainer product2 = setOfReactions[0].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product2, queryAtom));

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction:.
        /// [F+]!-!C => F + [C+]
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestFspChargeSingleB()
        {
            //CreateFromSmiles("[F+]C")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("F"));
            molecule.Atoms[0].FormalCharge = +1;
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[5], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new SharingChargeSBReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            //CreateFromSmiles("FH")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Add(builder.CreateAtom("F"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C+]")
            IAtomContainer expected2 = builder.CreateAtomContainer();
            expected2.Add(builder.CreateAtom("C"));
            expected2.Atoms[0].FormalCharge = +1;
            expected2.Add(builder.CreateAtom("H"));
            expected2.Add(builder.CreateAtom("H"));
            expected2.Add(builder.CreateAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            IAtomContainer product2 = setOfReactions[0].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product2, queryAtom));
        }
    }
}
