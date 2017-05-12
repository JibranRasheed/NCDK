/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Config;
using NCDK.Default;
using NCDK.Graphs;
using NCDK.IO;
using NCDK.Isomorphisms;
using NCDK.Stereo;
using NCDK.Templates;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NCDK.Numerics;

namespace NCDK.Smiles
{
    // @author         steinbeck
    // @cdk.created    2004-02-09
    // @cdk.module     test-smiles
    [TestClass()]
    public class SmilesGeneratorTest : CDKTestCase
    {
        [TestMethod()]
        public void TestSmilesGenerator()
        {
            IAtomContainer mol2 = TestMoleculeFactory.MakeAlphaPinene();
            SmilesGenerator sg = new SmilesGenerator();
            AddImplicitHydrogens(mol2);
            string smiles2 = sg.Create(mol2);
            Assert.IsNotNull(smiles2);
            Assert.AreEqual("C1(=CCC2CC1C2(C)C)C", smiles2);
        }

        [TestMethod()]
        public void TestEthylPropylPhenantren()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeEthylPropylPhenantren();
            SmilesGenerator sg = new SmilesGenerator();
            FixCarbonHCount(mol1);
            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("C=1C=CC(=C2C=CC3=C(C12)C=CC(=C3)CCC)CC", smiles1);
        }

        [TestMethod()]
        public void TestPropylCycloPropane()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakePropylCycloPropane();
            SmilesGenerator sg = new SmilesGenerator();
            FixCarbonHCount(mol1);
            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("C1CC1CCC", smiles1);
        }

        [TestMethod()]
        public void TestAlanin()
        {
            IAtomContainer mol1 = new AtomContainer();
            SmilesGenerator sg = SmilesGenerator.Isomeric();
            mol1.Atoms.Add(new Atom("N", new Vector2(1, 0)));
            // 1
            mol1.Atoms.Add(new Atom("C", new Vector2(1, 2)));
            // 2
            mol1.Atoms.Add(new Atom("F", new Vector2(1, 2)));
            // 3
            mol1.Atoms.Add(new Atom("C", Vector2.Zero));
            // 4
            mol1.Atoms.Add(new Atom("C", new Vector2(1, 4)));
            // 5
            mol1.Atoms.Add(new Atom("O", new Vector2(1, 5)));
            // 6
            mol1.Atoms.Add(new Atom("O", new Vector2(1, 6)));
            // 7
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[1], BondOrder.Single);
            // 1
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[2], BondOrder.Single, BondStereo.Up);
            // 2
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[3], BondOrder.Single, BondStereo.Down);
            // 3
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[4], BondOrder.Single);
            // 4
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[5], BondOrder.Single);
            // 5
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[6], BondOrder.Double);
            // 6
            // hydrogens in-lined from hydrogen adder/placer
            mol1.Atoms.Add(new Atom("H", new Vector2(0.13, -0.50)));
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[7], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(1.87, -0.50)));
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[8], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(-0.89, 0.45)));
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[9], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(-0.45, -0.89)));
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[10], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(0.89, -0.45)));
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[11], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(1.00, 6.00)));
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[12], BondOrder.Single);
            AddImplicitHydrogens(mol1);

            IsotopeFactory ifac = Isotopes.Instance;
            ifac.ConfigureAtoms(mol1);

            Define(mol1, Anticlockwise(mol1, 1, 0, 2, 3, 4));

            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);

            Assert.AreEqual("N([C@](F)(C([H])([H])[H])C(O[H])=O)([H])[H]", smiles1);

            Define(mol1, Clockwise(mol1, 1, 0, 2, 3, 4));

            smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("N([C@@](F)(C([H])([H])[H])C(O[H])=O)([H])[H]", smiles1);
        }

        [TestMethod()]
        public void TestCIsResorcinol()
        {
            IAtomContainer mol1 = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            SmilesGenerator sg = SmilesGenerator.Isomeric();
            mol1.Atoms.Add(new Atom("O", new Vector2(3, 1)));
            // 1
            mol1.Atoms.Add(new Atom("H", new Vector2(2, 0)));
            // 2
            mol1.Atoms.Add(new Atom("C", new Vector2(2, 1)));
            // 3
            mol1.Atoms.Add(new Atom("C", new Vector2(1, 1)));
            // 4
            mol1.Atoms.Add(new Atom("C", new Vector2(1, 4)));
            // 5
            mol1.Atoms.Add(new Atom("C", new Vector2(1, 5)));
            // 6
            mol1.Atoms.Add(new Atom("C", new Vector2(1, 2)));
            // 7
            mol1.Atoms.Add(new Atom("C", new Vector2(2, 2)));
            // 1
            mol1.Atoms.Add(new Atom("O", new Vector2(3, 2)));
            // 2
            mol1.Atoms.Add(new Atom("H", new Vector2(2, 3)));
            // 3
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[2], BondOrder.Single, BondStereo.Down);
            // 1
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[2], BondOrder.Single, BondStereo.Up);
            // 2
            mol1.AddBond(mol1.Atoms[2], mol1.Atoms[3], BondOrder.Single);
            // 3
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[4], BondOrder.Single);
            // 4
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[5], BondOrder.Single);
            // 5
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[6], BondOrder.Single);
            // 6
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[7], BondOrder.Single);
            // 3
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[8], BondOrder.Single, BondStereo.Up);
            // 4
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[9], BondOrder.Single, BondStereo.Down);
            // 5
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[2], BondOrder.Single);
            // 6

            // hydrogens in-lined from hydrogen adder/placer
            mol1.Atoms.Add(new Atom("H", new Vector2(4.00, 1.00)));
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[10], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(0.00, 1.00)));
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[11], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(1.00, 0.00)));
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[12], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(0.13, 4.50)));
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[13], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(0.13, 3.50)));
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[14], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(1.87, 5.50)));
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[15], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(0.13, 5.50)));
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[16], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(0.00, 2.00)));
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[17], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(1.00, 1.00)));
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[18], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(4.00, 2.00)));
            mol1.AddBond(mol1.Atoms[8], mol1.Atoms[19], BondOrder.Single);

            AddImplicitHydrogens(mol1);
            IsotopeFactory ifac = Isotopes.Instance;

            ifac.ConfigureAtoms(mol1);
            Define(mol1, Clockwise(mol1, 2, 0, 1, 3, 7), Clockwise(mol1, 7, 2, 6, 8, 9));
            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("O([C@@]1([H])C(C(C(C([C@]1(O[H])[H])([H])[H])([H])[H])([H])[H])([H])[H])[H]", smiles1);
            mol1 = AtomContainerManipulator.RemoveHydrogens(mol1);
            smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("O[C@H]1CCCC[C@H]1O", smiles1);
        }

        [TestMethod()]
        public void TestCIsTransDecalin()
        {
            IAtomContainer mol1 = new AtomContainer();
            SmilesGenerator sg = SmilesGenerator.Isomeric();

            mol1.Atoms.Add(new Atom("H", new Vector2(0, 3))); // 0
            mol1.Atoms.Add(new Atom("C", new Vector2(0, 1))); // 1
            mol1.Atoms.Add(new Atom("C", new Vector2(0, -1))); // 2
            mol1.Atoms.Add(new Atom("H", new Vector2(0, -3))); // 3

            mol1.Atoms.Add(new Atom("C", new Vector2(1.5, 2))); // 4
            mol1.Atoms.Add(new Atom("C", new Vector2(3, 1))); // 5
            mol1.Atoms.Add(new Atom("C", new Vector2(3, -1))); // 6
            mol1.Atoms.Add(new Atom("C", new Vector2(1.5, -2))); // 7

            mol1.Atoms.Add(new Atom("C", new Vector2(-1.5, 2))); // 8
            mol1.Atoms.Add(new Atom("C", new Vector2(-3, 1))); // 9
            mol1.Atoms.Add(new Atom("C", new Vector2(-3, -1))); // 10
            mol1.Atoms.Add(new Atom("C", new Vector2(-1.5, -2))); // 11

            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[0], BondOrder.Single, BondStereo.Down);
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[2], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[2], mol1.Atoms[3], BondOrder.Single, BondStereo.Down);

            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[4], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[5], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[6], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[7], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[2], BondOrder.Single);

            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[8], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[8], mol1.Atoms[9], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[9], mol1.Atoms[10], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[10], mol1.Atoms[11], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[11], mol1.Atoms[2], BondOrder.Single);

            // hydrogens in-lined from hydrogen adder/placer
            mol1.Atoms.Add(new Atom("H", new Vector2(2.16, 2.75)));
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[12], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(0.84, 2.75)));
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[13], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(3.98, 0.81)));
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[14], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(3.38, 1.92)));
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[15], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(3.38, -1.92)));
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[16], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(3.98, -0.81)));
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[17], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(0.84, -2.75)));
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[18], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(2.16, -2.75)));
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[19], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(-0.84, 2.75)));
            mol1.AddBond(mol1.Atoms[8], mol1.Atoms[20], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(-2.16, 2.75)));
            mol1.AddBond(mol1.Atoms[8], mol1.Atoms[21], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(-3.38, 1.92)));
            mol1.AddBond(mol1.Atoms[9], mol1.Atoms[22], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(-3.98, 0.81)));
            mol1.AddBond(mol1.Atoms[9], mol1.Atoms[23], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(-3.98, -0.81)));
            mol1.AddBond(mol1.Atoms[10], mol1.Atoms[24], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(-3.38, -1.92)));
            mol1.AddBond(mol1.Atoms[10], mol1.Atoms[25], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(-2.16, -2.75)));
            mol1.AddBond(mol1.Atoms[11], mol1.Atoms[26], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(-0.84, -2.75)));
            mol1.AddBond(mol1.Atoms[11], mol1.Atoms[27], BondOrder.Single);
            AddImplicitHydrogens(mol1);
            IsotopeFactory ifac = Isotopes.Instance;
            ifac.ConfigureAtoms(mol1);
            Define(mol1, Clockwise(mol1, 1, 0, 2, 4, 8), Clockwise(mol1, 2, 1, 3, 7, 1));
            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual(
                    "[H][C@@]12[C@@]([H])(C(C(C(C1([H])[H])([H])[H])([H])[H])([H])[H])C(C(C(C2([H])[H])([H])[H])([H])[H])([H])[H]",
                    smiles1);
            Define(mol1, Clockwise(mol1, 1, 0, 2, 4, 8), Anticlockwise(mol1, 2, 1, 3, 7, 1));
            string smiles3 = sg.Create(mol1);
            Assert.AreNotEqual(smiles3, smiles1);
        }

        [TestMethod()]
        public void TestDoubleBondConfiguration()
        {
            IAtomContainer mol1 = new AtomContainer();
            SmilesGenerator sg = SmilesGenerator.Isomeric();
            mol1.Atoms.Add(new Atom("S", Vector2.Zero));
            // 1
            mol1.Atoms.Add(new Atom("C", new Vector2(1, 1)));
            // 2
            mol1.Atoms.Add(new Atom("F", new Vector2(2, 0)));
            // 3
            mol1.Atoms.Add(new Atom("C", new Vector2(1, 2)));
            // 4
            mol1.Atoms.Add(new Atom("F", new Vector2(2, 3)));
            // 5
            mol1.Atoms.Add(new Atom("S", new Vector2(0, 3)));
            // 1

            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[1], BondOrder.Single);
            // 1
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[2], BondOrder.Single);
            // 2
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[3], BondOrder.Double);
            // 3
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[4], BondOrder.Single);
            // 4
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[5], BondOrder.Single);
            // 4
            AddImplicitHydrogens(mol1);
            IsotopeFactory ifac = Isotopes.Instance;
            ifac.ConfigureAtoms(mol1);

            mol1.SetStereoElements(new List<IStereoElement>()); // clear existing
            mol1.StereoElements.Add(new DoubleBondStereochemistry(mol1.Bonds[2], new IBond[]{mol1.Bonds[1],
                mol1.Bonds[3]}, DoubleBondConformation.Opposite));
            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("S\\C(\\F)=C(/F)\\S", smiles1);

            mol1.SetStereoElements(new List<IStereoElement>()); // clear existing
            mol1.StereoElements.Add(new DoubleBondStereochemistry(mol1.Bonds[2], new IBond[]{mol1.Bonds[1],
                mol1.Bonds[3]}, DoubleBondConformation.Together));

            smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("S\\C(\\F)=C(\\F)/S", smiles1);

            // hydrogens in-lined from hydrogen adder/placer
            mol1.Atoms.Add(new Atom("H", new Vector2(-0.71, -0.71)));
            mol1.Atoms[0].ImplicitHydrogenCount = 0;
            mol1.Atoms[mol1.Atoms.Count - 1].ImplicitHydrogenCount = 0;
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[6], BondOrder.Single);
            mol1.Atoms.Add(new Atom("H", new Vector2(2.71, 3.71)));
            mol1.Atoms[5].ImplicitHydrogenCount = 0;
            mol1.Atoms[mol1.Atoms.Count - 1].ImplicitHydrogenCount = 0;
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[7], BondOrder.Single);

            mol1.SetStereoElements(new List<IStereoElement>()); // clear existing
            mol1.StereoElements.Add(new DoubleBondStereochemistry(mol1.Bonds[2], new IBond[]{mol1.Bonds[0],
                mol1.Bonds[3]}, DoubleBondConformation.Opposite));

            smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("S(/C(/F)=C(/F)\\S[H])[H]", smiles1);

            mol1.SetStereoElements(new List<IStereoElement>()); // clear existing
            mol1.StereoElements.Add(new DoubleBondStereochemistry(mol1.Bonds[2], new IBond[]{mol1.Bonds[0],
                mol1.Bonds[3]}, DoubleBondConformation.Together));

            smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("S(/C(/F)=C(\\F)/S[H])[H]", smiles1);
        }

        [TestMethod()]
        public void TestPartitioning()
        {
            string smiles = "";
            IAtomContainer molecule = new AtomContainer();
            SmilesGenerator sg = new SmilesGenerator();
            Atom sodium = new Atom("Na");
            sodium.FormalCharge = +1;
            Atom hydroxyl = new Atom("O");
            hydroxyl.ImplicitHydrogenCount = 1;
            hydroxyl.FormalCharge = -1;
            molecule.Atoms.Add(sodium);
            molecule.Atoms.Add(hydroxyl);
            AddImplicitHydrogens(molecule);
            smiles = sg.Create(molecule);
            Assert.IsTrue(smiles.IndexOf(".") != -1);
        }

        // @cdk.bug 791091
        [TestMethod()]
        public void TestBug791091()
        {
            string smiles = "";
            IAtomContainer molecule = new AtomContainer();
            SmilesGenerator sg = new SmilesGenerator();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("N"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[0], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[3], BondOrder.Single);
            FixCarbonHCount(molecule);
            smiles = sg.Create(molecule);
            Assert.AreEqual("C1CCN1C", smiles);
        }

        // @cdk.bug 590236
        [TestMethod()]
        public void TestBug590236()
        {
            string smiles = "";
            IAtomContainer molecule = new AtomContainer();
            SmilesGenerator sg = SmilesGenerator.Isomeric();
            molecule.Atoms.Add(new Atom("C"));
            Atom carbon2 = new Atom("C");
            carbon2.MassNumber = 13;
            molecule.Atoms.Add(carbon2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            FixCarbonHCount(molecule);
            smiles = sg.Create(molecule);
            Assert.AreEqual("C[13CH3]", smiles);
        }

        /// <summary>
        /// A bug reported for JChemPaint.
        /// </summary>
        // @cdk.bug 956923
        [TestMethod()]
        public void TestSFBug956923_aromatic()
        {
            string smiles = "";
            IAtomContainer molecule = new AtomContainer();
            SmilesGenerator sg = new SmilesGenerator().Aromatic();
            Atom sp2CarbonWithOneHydrogen = new Atom("C");
            sp2CarbonWithOneHydrogen.Hybridization = Hybridization.SP2;
            sp2CarbonWithOneHydrogen.ImplicitHydrogenCount = 1;
            molecule.Atoms.Add(sp2CarbonWithOneHydrogen);
            molecule.Atoms.Add((Atom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((Atom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((Atom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((Atom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((Atom)sp2CarbonWithOneHydrogen.Clone());
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[0], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            smiles = sg.Create(molecule);
            Assert.AreEqual("c1ccccc1", smiles);
        }

        [TestMethod()]
        public void TestSFBug956923_nonAromatic()
        {
            string smiles = "";
            IAtomContainer molecule = new AtomContainer();
            SmilesGenerator sg = new SmilesGenerator();
            Atom sp2CarbonWithOneHydrogen = new Atom("C");
            sp2CarbonWithOneHydrogen.Hybridization = Hybridization.SP2;
            sp2CarbonWithOneHydrogen.ImplicitHydrogenCount = 1;
            molecule.Atoms.Add(sp2CarbonWithOneHydrogen);
            molecule.Atoms.Add((Atom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((Atom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((Atom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((Atom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((Atom)sp2CarbonWithOneHydrogen.Clone());
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[0], BondOrder.Single);
            smiles = sg.Create(molecule);
            Assert.AreEqual("[CH]1[CH][CH][CH][CH][CH]1", smiles);
        }

        [TestMethod()]
        public void TestAtomPermutation()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("S"));
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            mol.Atoms[3].ImplicitHydrogenCount = 1;
            mol.Atoms[4].ImplicitHydrogenCount = 1;
            AddImplicitHydrogens(mol);
            AtomContainerAtomPermutor acap = new AtomContainerAtomPermutor(mol);
            SmilesGenerator sg = SmilesGenerator.Unique();
            string smiles = "";
            string oldSmiles = sg.Create(mol);
            while (acap.MoveNext())
            {
                smiles = sg.Create(new AtomContainer((AtomContainer)acap.Current));
                //Debug.WriteLine(smiles);
                Assert.AreEqual(oldSmiles, smiles);
            }
        }

        [TestMethod()]
        public void TestBondPermutation()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("S"));
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            mol.Atoms[3].ImplicitHydrogenCount = 1;
            mol.Atoms[4].ImplicitHydrogenCount = 1;
            AddImplicitHydrogens(mol);
            AtomContainerBondPermutor acbp = new AtomContainerBondPermutor(mol);
            SmilesGenerator sg = SmilesGenerator.Unique();
            string smiles = "";
            string oldSmiles = sg.Create(mol);
            while (acbp.MoveNext())
            {
                smiles = sg.Create(new AtomContainer((AtomContainer)acbp.Current));
                //Debug.WriteLine(smiles);
                Assert.AreEqual(oldSmiles, smiles);
            }
        }

        private void FixCarbonHCount(IAtomContainer mol)
        {
            // the following line are just a quick fix for this particluar
            // carbon-only molecule until we have a proper hydrogen count
            // configurator
            double bondCount = 0;
            IAtom atom;
            for (int f = 0; f < mol.Atoms.Count; f++)
            {
                atom = mol.Atoms[f];
                bondCount = mol.GetBondOrderSum(atom);
                int correction = (int)(bondCount - (atom.Charge ?? 0));
                if (atom.Symbol.Equals("C"))
                {
                    atom.ImplicitHydrogenCount = 4 - correction;
                }
                else if (atom.Symbol.Equals("N"))
                {
                    atom.ImplicitHydrogenCount = 3 - correction;
                }
            }
        }

        [TestMethod()]
        public void TestPseudoAtom()
        {
            IAtom atom = new PseudoAtom("Star");
            SmilesGenerator sg = new SmilesGenerator(SmiFlavor.Generic);
            string smiles = "";
            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(atom);
            AddImplicitHydrogens(molecule);
            smiles = sg.Create(molecule);
            Assert.AreEqual("*", smiles);
        }

        /// <summary>
        ///  Test generation of a reaction SMILES. I know, it's a stupid alchemic
        ///  reaction, but it serves its purpose.
        /// </summary>
        [TestMethod()]
        public void TestReactionSMILES()
        {
            Reaction reaction = new Reaction();
            AtomContainer methane = new AtomContainer();
            methane.Atoms.Add(new Atom("C"));
            reaction.Reactants.Add(methane);
            IAtomContainer magic = new AtomContainer();
            magic.Atoms.Add(new PseudoAtom("magic"));
            reaction.Agents.Add(magic);
            IAtomContainer gold = new AtomContainer();
            gold.Atoms.Add(new Atom("Au"));
            reaction.Products.Add(gold);

            methane.Atoms[0].ImplicitHydrogenCount = 4;
            gold.Atoms[0].ImplicitHydrogenCount = 0;

            SmilesGenerator sg = new SmilesGenerator(SmiFlavor.Generic);
            string smiles = sg.Create(reaction);
            //Debug.WriteLine("Generated SMILES: " + smiles);
            Assert.AreEqual("C>*>[Au]", smiles);
        }

        /// <summary>
        ///  Test generation of a D and L alanin.
        /// </summary>
        [TestMethod()]
        public void TestAlaSMILES()
        {
            string filename = "NCDK.Data.MDL.l-ala.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader.Read(Default.ChemObjectBuilder.Instance.CreateAtomContainer());
            filename = "NCDK.Data.MDL.d-ala.mol";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol2 = reader.Read(Default.ChemObjectBuilder.Instance.CreateAtomContainer());
            SmilesGenerator sg = SmilesGenerator.Isomeric();

            Define(mol1, Anticlockwise(mol1, 1, 0, 2, 3, 6));
            Define(mol2, Clockwise(mol2, 1, 0, 2, 3, 6));

            string smiles1 = sg.Create(mol1);
            string smiles2 = sg.Create(mol2);
            Assert.AreNotEqual(smiles2, smiles1);
    }

    /// <summary>
    ///  Test some sugars
    /// </summary>
    [TestMethod()]
        public void TestSugarSMILES()
        {
            string filename = "NCDK.Data.MDL.D-mannose.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader.Read(new AtomContainer());
            filename = "NCDK.Data.MDL.D+-glucose.mol";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol2 = reader.Read(new AtomContainer());
            SmilesGenerator sg = SmilesGenerator.Isomeric();

            Define(mol1, Anticlockwise(mol1, 0, 0, 1, 5, 9), Anticlockwise(mol1, 1, 1, 0, 2, 8),
                    Clockwise(mol1, 2, 2, 1, 3, 6), Anticlockwise(mol1, 5, 5, 0, 4, 10));
            Define(mol2, Anticlockwise(mol2, 0, 0, 1, 5, 9), Anticlockwise(mol2, 1, 1, 0, 2, 8),
                    Clockwise(mol2, 2, 2, 1, 3, 6), Clockwise(mol2, 5, 5, 0, 4, 10), Anticlockwise(mol2, 4, 4, 3, 5, 11));

            string smiles1 = sg.Create(mol1);
            string smiles2 = sg.Create(mol2);
            Assert.AreNotEqual(smiles2, smiles1);
        }

        /// <summary>
        ///  Test for some rings where the double bond is broken
        /// </summary>
        [TestMethod()]
        public void TestCycloOctan()
        {
            string filename = "NCDK.Data.MDL.cyclooctan.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader.Read(new AtomContainer());
            SmilesGenerator sg = new SmilesGenerator();
            string moleculeSmile = sg.Create(mol1);
            Assert.AreEqual("C1=CCCCCCC1", moleculeSmile);
        }

        [TestMethod()]
        public void TestCycloOcten()
        {
            string filename = "NCDK.Data.MDL.cycloocten.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader.Read(new AtomContainer());
            SmilesGenerator sg = new SmilesGenerator();
            string moleculeSmile = sg.Create(mol1);
            Assert.AreEqual("C1C=CCCCCC1", moleculeSmile);
        }

        /// <summary>
        ///  A unit test for JUnit
        /// </summary>
        [TestMethod()]
        public void TestCycloOctadien()
        {
            string filename = "NCDK.Data.MDL.cyclooctadien.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader.Read(Default.ChemObjectBuilder.Instance.CreateAtomContainer());
            SmilesGenerator sg = new SmilesGenerator();
            string moleculeSmile = sg.Create(mol1);
            Assert.AreEqual("C=1CCC=CCCC1", moleculeSmile);
        }

        // @cdk.bug 1089770
        [TestMethod()]
        public void TestSFBug1089770_1()
        {
            string filename = "NCDK.Data.MDL.bug1089770-1.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader.Read(new AtomContainer());
            SmilesGenerator sg = new SmilesGenerator();
            string moleculeSmile = sg.Create(mol1);
            //Debug.WriteLine(filename + " -> " + moleculeSmile);
            Assert.AreEqual("C1CCC2=C(C1)CCC2", moleculeSmile);
        }

        // @cdk.bug 1089770
        [TestMethod()]
        public void TestSFBug1089770_2()
        {
            string filename = "NCDK.Data.MDL.bug1089770-2.mol";
        var ins = ResourceLoader.GetAsStream(filename);
        MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
        IAtomContainer mol1 = reader.Read(new AtomContainer());
        SmilesGenerator sg = new SmilesGenerator();
        string moleculeSmile = sg.Create(mol1);
        //Debug.WriteLine(filename + " -> " + moleculeSmile);
        Assert.AreEqual("C=1CCC=CCCC1", moleculeSmile);
        }

        // @cdk.bug 1014344
        // MDL -> CML (slow) -> SMILES round tripping
        [TestMethod()]
        public void TestSFBug1014344()
        {
            string filename = "NCDK.Data.MDL.bug1014344-1.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLReader reader = new MDLReader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader.Read(new AtomContainer());
            AddImplicitHydrogens(mol1);
            SmilesGenerator sg = new SmilesGenerator();
            string molSmiles = sg.Create(mol1);
            StringWriter output = new StringWriter();
            CMLWriter cmlWriter = new CMLWriter(output);
            cmlWriter.Write(mol1);

            var aa = output.ToString();
            var bb = Encoding.UTF8.GetBytes(aa);

            CMLReader cmlreader = new CMLReader(new MemoryStream(bb));
            IAtomContainer mol2 = ((IChemFile)cmlreader.Read(new ChemFile()))[0][0].MoleculeSet[0];
            AddImplicitHydrogens(mol2);
            string cmlSmiles = sg.Create(new AtomContainer(mol2));
            Assert.AreEqual(molSmiles, cmlSmiles);
        }

        // @cdk.bug 1014344
        [TestMethod()]
        public void TestTest()
        {
            string filename_cml = "NCDK.Data.MDL.9554-with-exp-hyd.mol";
            string filename_mol = "NCDK.Data.MDL.9553-with-exp-hyd.mol";
            var ins1 = ResourceLoader.GetAsStream(filename_cml);
            var ins2 = ResourceLoader.GetAsStream(filename_mol);
            MDLV2000Reader reader1 = new MDLV2000Reader(ins1, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader1.Read(new AtomContainer());

            MDLV2000Reader reader2 = new MDLV2000Reader(ins2, ChemObjectReaderModes.Strict);
            IAtomContainer mol2 = reader2.Read(new AtomContainer());

            SmilesGenerator sg = SmilesGenerator.Isomeric();

            Define(mol1, Clockwise(mol1, 0, 1, 5, 12, 13), Clockwise(mol1, 1, 0, 2, 6, 12),
                    Clockwise(mol1, 2, 1, 3, 9, 10), Clockwise(mol1, 5, 0, 4, 11, 18));
            Define(mol2, Clockwise(mol2, 0, 1, 5, 12, 13), Clockwise(mol2, 1, 0, 2, 6, 12),
                    Anticlockwise(mol2, 2, 1, 3, 9, 10), Clockwise(mol2, 5, 0, 4, 11, 18));

            string moleculeSmile1 = sg.Create(mol1);
            string moleculeSmile2 = sg.Create(mol2);
            Assert.AreNotEqual(moleculeSmile2, moleculeSmile1);
        }

        // @cdk.bug 1535055
        [TestMethod()]
        public void TestSFBug1535055()
        {
            string filename_cml = "NCDK.Data.CML.test1.cml";
            var ins1 = ResourceLoader.GetAsStream(filename_cml);
            CMLReader reader1 = new CMLReader(ins1);
            IChemFile chemFile = (IChemFile)reader1.Read(new ChemFile());
            Assert.IsNotNull(chemFile);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            IAtomContainer mol1 = model.MoleculeSet[0];
            Assert.IsNotNull(mol1);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AddImplicitHydrogens(mol1);
            Assert.IsTrue(Aromaticity.CDKLegacy.Apply(mol1));

            SmilesGenerator sg = new SmilesGenerator().Aromatic();

            string mol1SMILES = sg.Create(mol1);
            Assert.IsTrue(mol1SMILES.Contains("nH"));
        }

        // @cdk.bug 1014344
        [TestMethod()]
        public void TestSFBug1014344_1()
        {
            string filename_cml = "NCDK.Data.CML.bug1014344-1.cml";
            string filename_mol = "NCDK.Data.MDL.bug1014344-1.mol";
            var ins1 = ResourceLoader.GetAsStream(filename_cml);
            var ins2 = ResourceLoader.GetAsStream(filename_mol);
            CMLReader reader1 = new CMLReader(ins1);
            IChemFile chemFile = (IChemFile)reader1.Read(new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer mol1 = model.MoleculeSet[0];

            MDLReader reader2 = new MDLReader(ins2);
            IAtomContainer mol2 = reader2.Read(new AtomContainer());

            AddImplicitHydrogens(mol1);
            AddImplicitHydrogens(mol2);

            SmilesGenerator sg = new SmilesGenerator();

            string moleculeSmile1 = sg.Create(mol1);
            //        Debug.WriteLine(filename_cml + " -> " + moleculeSmile1);
            string moleculeSmile2 = sg.Create(mol2);
            //        Debug.WriteLine(filename_mol + " -> " + moleculeSmile2);
            Assert.AreEqual(moleculeSmile1, moleculeSmile2);
        }

        // @cdk.bug 1875946
        [TestMethod()]
        public void TestPreservingFormalCharge()
        {
            AtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom(Elements.Oxygen.ToIElement()));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(new Atom(Elements.Carbon.ToIElement()));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            AddImplicitHydrogens(mol);
            SmilesGenerator generator = new SmilesGenerator();
            generator.Create(new AtomContainer(mol));
            Assert.AreEqual(-1, mol.Atoms[0].FormalCharge.Value);
            // mmm, that does not reproduce the bug findings yet :(
        }

        [TestMethod()]
        public void TestIndole()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            SmilesGenerator smilesGenerator = new SmilesGenerator().Aromatic();
            string smiles = smilesGenerator.Create(mol);
            Assert.IsTrue(smiles.IndexOf("[nH]") >= 0);
        }

        [TestMethod()]
        public void TestPyrrole()
        {
            IAtomContainer mol = TestMoleculeFactory.MakePyrrole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            SmilesGenerator smilesGenerator = new SmilesGenerator().Aromatic();
            string smiles = smilesGenerator.Create(mol);
            Assert.IsTrue(smiles.IndexOf("[nH]") >= 0);
        }

        // @cdk.bug 1300
        [TestMethod()]
        public void TestDoubleBracketProblem()
        {
            IAtomContainer mol = TestMoleculeFactory.MakePyrrole();
            mol.Atoms[1].FormalCharge = -1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            AddImplicitHydrogens(mol);
            SmilesGenerator smilesGenerator = new SmilesGenerator().Aromatic();
            string smiles = smilesGenerator.Create(mol);
            Assert.IsFalse(smiles.Contains("[[nH]-]"));
        }

        // @cdk.bug 1300
        [TestMethod()]
        public void TestHydrogenOnChargedNitrogen()
        {
            IAtomContainer mol = TestMoleculeFactory.MakePyrrole();
            mol.Atoms[1].FormalCharge = -1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            SmilesGenerator smilesGenerator = new SmilesGenerator().Aromatic();
            string smiles = smilesGenerator.Create(mol);
            Assert.IsTrue(smiles.Contains("[n-]"));
        }

        // @cdk.bug 545
        [TestMethod()]
        public void TestTimeOut()
        {
            string filename = "NCDK.Data.MDL.24763.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            IAtomContainer container = containersList[0];
            SmilesGenerator smilesGenerator = new SmilesGenerator();
            string genSmiles = smilesGenerator.Create(container);
            Console.Out.WriteLine(genSmiles);
        }

        // @cdk.bug 2051597
        [TestMethod()]
        public void TestSFBug2051597()
        {
            string smiles = "c1(c2ccc(c8ccccc8)cc2)" + "c(c3ccc(c9ccccc9)cc3)" + "c(c4ccc(c%10ccccc%10)cc4)"
                + "c(c5ccc(c%11ccccc%11)cc5)" + "c(c6ccc(c%12ccccc%12)cc6)" + "c1(c7ccc(c%13ccccc%13)cc7)";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer cdkMol = smilesParser.ParseSmiles(smiles);
            SmilesGenerator smilesGenerator = new SmilesGenerator();
            string genSmiles = smilesGenerator.Create(cdkMol);

            // check that we have the appropriate ring closure symbols
            Assert.IsTrue(genSmiles.IndexOf("%") >= 0, "There were'nt any % ring closures in the output");
            Assert.IsTrue(genSmiles.IndexOf("%10") >= 0);
            Assert.IsTrue(genSmiles.IndexOf("%11") >= 0);
            Assert.IsTrue(genSmiles.IndexOf("%12") >= 0);
            Assert.IsTrue(genSmiles.IndexOf("%13") >= 0);

            // check that we can read in the SMILES we got
            IAtomContainer cdkRoundTripMol = smilesParser.ParseSmiles(genSmiles);
            Assert.IsNotNull(cdkRoundTripMol);
        }

        // @cdk.bug 2596061
        [TestMethod()]
        public void TestRoundTripPseudoAtom()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string smiles = "[12*H2-]";
            IAtomContainer mol = sp.ParseSmiles(smiles);
            SmilesGenerator smilesGenerator = SmilesGenerator.Isomeric();
            string genSmiles = smilesGenerator.Create(mol);
            Assert.AreEqual(smiles, genSmiles);
        }

        // @cdk.bug 2781199
        [TestMethod()]
        public void TestBug2781199()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string smiles = "n1ncn(c1)CC";
            IAtomContainer mol = sp.ParseSmiles(smiles);
            SmilesGenerator smilesGenerator = new SmilesGenerator().Aromatic();
            string genSmiles = smilesGenerator.Create(mol);
            Assert.IsTrue(genSmiles.IndexOf("H") == -1, "Generated SMILES should not have explicit H: " + genSmiles);
        }

        // @cdk.bug 2898032
        [TestMethod()]
        public void TestCanSmiWithoutConfiguredAtoms()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string s1 = "OC(=O)C(Br)(Cl)N";
            string s2 = "ClC(Br)(N)C(=O)O";

            IAtomContainer m1 = sp.ParseSmiles(s1);
            IAtomContainer m2 = sp.ParseSmiles(s2);

            SmilesGenerator sg = SmilesGenerator.Unique();
            string o1 = sg.Create(m1);
            string o2 = sg.Create(m2);

            Assert.IsTrue(o1.Equals(o2), "The two canonical SMILES should match");
        }

        // @cdk.bug 2898032
        [TestMethod()]
        public void TestCanSmiWithConfiguredAtoms()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string s1 = "OC(=O)C(Br)(Cl)N";
            string s2 = "ClC(Br)(N)C(=O)O";

            IAtomContainer m1 = sp.ParseSmiles(s1);
            IAtomContainer m2 = sp.ParseSmiles(s2);

            IsotopeFactory fact = Isotopes.Instance;
            fact.ConfigureAtoms(m1);
            fact.ConfigureAtoms(m2);

            SmilesGenerator sg = SmilesGenerator.Unique();
            string o1 = sg.Create(m1);
            string o2 = sg.Create(m2);

            Assert.IsTrue(o1.Equals(o2), "The two canonical SMILES should match");
        }

        // @cdk.bug 3040273
        [TestMethod()]
        public void TestBug3040273()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string testSmiles = "C1(C(C(C(C(C1Br)Br)Br)Br)Br)Br";
            IAtomContainer mol = sp.ParseSmiles(testSmiles);
            IsotopeFactory fact = Isotopes.Instance;
            fact.ConfigureAtoms(mol);
            SmilesGenerator sg = new SmilesGenerator();
            string smiles = sg.Create((IAtomContainer)mol);
            IAtomContainer mol2 = sp.ParseSmiles(smiles);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(mol, mol2));
        }

        [TestMethod()]
        public void TestCreateSMILESWithoutCheckForMultipleMolecules_withDetectAromaticity()
        {
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            AddImplicitHydrogens(benzene);
            SmilesGenerator sg = new SmilesGenerator();
            string smileswithoutaromaticity = sg.Create(benzene);
            Assert.AreEqual("C=1C=CC=CC1", smileswithoutaromaticity);
        }

        [TestMethod()]
        public void TestCreateSMILESWithoutCheckForMultipleMolecules_withoutDetectAromaticity()
        {
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            AddImplicitHydrogens(benzene);
            SmilesGenerator sg = new SmilesGenerator().Aromatic();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(benzene);
            Aromaticity.CDKLegacy.Apply(benzene);
            string smileswitharomaticity = sg.Create(benzene);
            Assert.AreEqual("c1ccccc1", smileswitharomaticity);
        }

        [TestMethod()]
        public void OutputOrder()
        {
            IAtomContainer adenine = TestMoleculeFactory.MakeAdenine();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(adenine);
            CDKHydrogenAdder.GetInstance(Silent.ChemObjectBuilder.Instance).AddImplicitHydrogens(adenine);

            SmilesGenerator sg = SmilesGenerator.Generic();
            int[] order = new int[adenine.Atoms.Count];

            string smi = sg.Create(adenine, order);
            string[] at = new string[adenine.Atoms.Count];

            for (int i = 0; i < at.Length; i++)
            {
                at[order[i]] = adenine.Atoms[i].AtomTypeName;
            }

            // read in the SMILES
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer adenine2 = sp.ParseSmiles(smi);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(adenine2);
            CDKHydrogenAdder.GetInstance(Silent.ChemObjectBuilder.Instance).AddImplicitHydrogens(adenine2);

            // check atom types
            for (int i = 0; i < adenine2.Atoms.Count; i++)
            {
                Assert.AreEqual(adenine2.Atoms[i].AtomTypeName, at[i]);
            }
        }

        [TestMethod()]
        public void OutputCanOrder()
        {
            IAtomContainer adenine = TestMoleculeFactory.MakeAdenine();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(adenine);
            CDKHydrogenAdder.GetInstance(Silent.ChemObjectBuilder.Instance).AddImplicitHydrogens(adenine);

            SmilesGenerator sg = SmilesGenerator.Unique();
            int[] order = new int[adenine.Atoms.Count];

            string smi = sg.Create(adenine, order);
            string[] at = new string[adenine.Atoms.Count];

            for (int i = 0; i < at.Length; i++)
            {
                at[order[i]] = adenine.Atoms[i].AtomTypeName;
            }

            // read in the SMILES
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer adenine2 = sp.ParseSmiles(smi);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(adenine2);
            CDKHydrogenAdder.GetInstance(Silent.ChemObjectBuilder.Instance).AddImplicitHydrogens(adenine2);

            // check atom types
            for (int i = 0; i < adenine2.Atoms.Count; i++)
            {
                Assert.AreEqual(adenine2.Atoms[i].AtomTypeName, at[i]);
            }
        }

        [TestMethod()]
        public void AtomClasses()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            IAtomContainer ethanol = new SmilesParser(bldr).ParseSmiles("C[CH2:6]O");
            Assert.AreEqual("CCO", SmilesGenerator.Generic().Create(ethanol));
            Assert.AreEqual("C[CH2:6]O", SmilesGenerator.Generic().WithAtomClasses().Create(ethanol));
        }

        // @cdk.bug 328
        [TestMethod()]
        public void Bug328()
        {
            Assert.AreEqual(
                Canon("Clc1ccc(Cl)c2[nH]c([nH0]c21)C(F)(F)F"),
                Canon("[H]c2c([H])c(c1c(nc(n1([H]))C(F)(F)F)c2Cl)Cl"));
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void WarnOnBadInput()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            smipar.Kekulise(false);
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1");
            System.Console.Error.WriteLine(SmilesGenerator.Isomeric().Create(mol));
        }

        /// <summary>
        /// <see href="https://tech.knime.org/forum/cdk/buggy-behavior-of-molecule-to-cdk-node"/>
        /// </summary>
        [TestMethod()]
        public void AssignDbStereo()
        {
            string ins = "C(/N)=C\\C=C\\1/N=C1";
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = smipar.ParseSmiles(ins);
            Assert.AreEqual("C(\\N)=C/C=C/1N=C1", SmilesGenerator.Isomeric().Create(mol));
        }

        [TestMethod()]
        public void CanonicalReactions()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IReaction r1 = smipar.ParseReactionSmiles("CC(C)C1=CC=CC=C1.C(CC(=O)Cl)CCl>[Al+3].[Cl-].[Cl-].[Cl-].C(Cl)Cl>CC(C)C1=CC=C(C=C1)C(=O)CCCCl");
            IReaction r2 = smipar.ParseReactionSmiles("C(CC(=O)Cl)CCl.CC(C)C1=CC=CC=C1>[Al+3].[Cl-].[Cl-].[Cl-].C(Cl)Cl>CC(C)C1=CC=C(C=C1)C(=O)CCCCl");
            IReaction r3 = smipar.ParseReactionSmiles("CC(C)C1=CC=CC=C1.C(CC(=O)Cl)CCl>C(Cl)Cl.[Al+3].[Cl-].[Cl-].[Cl-]>CC(C)C1=CC=C(C=C1)C(=O)CCCCl");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavor.Canonical);
            Assert.AreEqual(smigen.Create(r2), smigen.Create(r1));
            Assert.AreEqual(smigen.Create(r3), smigen.Create(r2));
        }

        static ITetrahedralChirality Anticlockwise(IAtomContainer container, int central, int a1, int a2, int a3, int a4)
        {
            return new TetrahedralChirality(container.Atoms[central], new IAtom[]{container.Atoms[a1],
                                                                                container.Atoms[a2], container.Atoms[a3], container.Atoms[a4]},
                                            TetrahedralStereo.AntiClockwise);
        }

        static ITetrahedralChirality Clockwise(IAtomContainer container, int central, int a1, int a2, int a3, int a4)
        {
            return new TetrahedralChirality(container.Atoms[central], new IAtom[]{container.Atoms[a1],
                                                                                container.Atoms[a2], container.Atoms[a3], container.Atoms[a4]},
                                            TetrahedralStereo.Clockwise);
        }

        static void Define(IAtomContainer container, params IStereoElement[] elements)
        {
            container.SetStereoElements(new List<IStereoElement>(elements));
        }

        static string Canon(string smi)
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer container = smipar.ParseSmiles(smi);
            AtomContainerManipulator.SuppressHydrogens(container);
            Aromaticity arom = new Aromaticity(ElectronDonation.DaylightModel, Cycles.AllFinder);
            arom.Apply(container);
            return SmilesGenerator.Unique().Create(container);
        }
    }
}