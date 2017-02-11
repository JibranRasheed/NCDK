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
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.Reactions.Types
{
    /**
     * TestSuite that runs a test for the RearrangementRadicalReactionTest.
     * Generalized Reaction: [A*+]-B-C => [A+]=B + [c*].
     *
     * @cdk.module test-reaction
     */
    [TestClass()]
    public class RadicalChargeSiteInitiationReactionTest : ReactionProcessTest
    {

        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /**
         *  The JUnit setup method
         */
        public RadicalChargeSiteInitiationReactionTest()
        {
            SetReaction(typeof(RadicalChargeSiteInitiationReaction));
        }

        /**
         *  The JUnit setup method
         */
        [TestMethod()]
        public void TestRadicalChargeSiteInitiationReaction()
        {
            IReactionProcess type = new RadicalChargeSiteInitiationReaction();
            Assert.IsNotNull(type);
        }

        /**
         * A unit test suite for JUnit. Reaction: [O+*]C([H])([H])C([H])([H])([H]) => [O+]=C([H])([H]) +[C*]([H])([H])([H])
         * Automatic search of the center active.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            IReactionProcess type = new RadicalChargeSiteInitiationReaction();

            var setOfReactants = GetExampleReactants();

            /* initiate */

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            IAtomContainer product1 = setOfReactions[0].Products[0];

            IAtomContainer molecule1 = GetExpectedProducts()[0];

            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product1, queryAtom));

            IAtomContainer product2 = setOfReactions[0].Products[1];

            IAtomContainer molecule2 = GetExpectedProducts()[1];

            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product2);
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
            IReactionProcess type = new RadicalChargeSiteInitiationReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually put the reactive center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[0].IsReactiveCenter);
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
            IReactionProcess type = new RadicalChargeSiteInitiationReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;
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

            Assert.AreEqual(9, setOfReactions[0].Mappings.Count);
            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA1, product1.Atoms[1]);
            IAtom mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[2]);
            Assert.AreEqual(mappedProductA2, product2.Atoms[0]);
            IAtom mappedProductA3 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA3, product1.Atoms[0]);

        }

        /**
         * Get the IAtomContainer
         *
         * @return The IAtomContainerSet
         */
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("O"));
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[5], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[7], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[8], BondOrder.Single);

            IAtom atom = molecule.Atoms[0];
            atom.FormalCharge = 1;
            molecule.Add(new SingleElectron(atom));

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
            IAtomContainer molecule1 = builder.CreateAtomContainer();
            molecule1.Add(builder.CreateAtom("O"));
            molecule1.Add(builder.CreateAtom("C"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[1], BondOrder.Double);
            molecule1.Add(builder.CreateAtom("H"));
            molecule1.AddBond(molecule1.Atoms[1], molecule1.Atoms[2], BondOrder.Single);
            molecule1.Add(builder.CreateAtom("H"));
            molecule1.AddBond(molecule1.Atoms[1], molecule1.Atoms[3], BondOrder.Single);
            molecule1.Atoms[0].FormalCharge = 1;
            molecule1.Add(builder.CreateAtom("H"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[4], BondOrder.Single);
            setOfProducts.Add(molecule1);

            IAtomContainer molecule2 = builder.CreateAtomContainer();
            molecule2.Add(builder.CreateAtom("C"));
            molecule2.Add(builder.CreateAtom("H"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Single);
            molecule2.Add(builder.CreateAtom("H"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[2], BondOrder.Single);
            molecule2.Add(builder.CreateAtom("H"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[3], BondOrder.Single);
            molecule2.Add(new SingleElectron(molecule2.Atoms[0]));
            setOfProducts.Add(molecule2);

            return setOfProducts;
        }
    }
}
