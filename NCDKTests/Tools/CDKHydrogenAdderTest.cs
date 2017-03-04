/* Copyright (C) 2007  Egon Willighagen <egonw@sci.kun.nl>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.AtomTypes;
using NCDK.Config;
using NCDK.Default;
using NCDK.IO;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;


namespace NCDK.Tools
{
    /// <summary>
    /// Tests CDK's hydrogen adding capabilities in terms of
    /// example molecules.
    ///
    // @cdk.module  test-valencycheck
    ///
    // @author      Egon Willighagen <egonw@users.sf.net>
    // @cdk.created 2007-07-28
    /// </summary>
    [TestClass()]
    public class CDKHydrogenAdderTest : CDKTestCase
    {
        private readonly static CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(Silent.ChemObjectBuilder
                                                                .Instance);
        private readonly static CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(Silent.ChemObjectBuilder
                                                                .Instance);

        [TestMethod()]
        public void TestInstance()
        {
            Assert.IsNotNull(adder);
        }

        [TestMethod()]
        public void TestMethane()
        {
            IAtomContainer molecule = new AtomContainer();
            IAtom newAtom = new Atom(Elements.Carbon.ToIElement());
            molecule.Add(newAtom);
            IAtomType type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.AreEqual(4, newAtom.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestFormaldehyde()
        {
            IAtomContainer molecule = new AtomContainer();
            IAtom newAtom = new Atom(Elements.Carbon.ToIElement());
            IAtom newAtom2 = new Atom(Elements.Oxygen.ToIElement());
            molecule.Add(newAtom);
            molecule.Add(newAtom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            IAtomType type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);
            type = matcher.FindMatchingAtomType(molecule, newAtom2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom2, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.IsNotNull(newAtom2.ImplicitHydrogenCount);
            Assert.AreEqual(2, newAtom.ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, newAtom2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestMethanol()
        {
            IAtomContainer molecule = new AtomContainer();
            IAtom newAtom = new Atom(Elements.Carbon.ToIElement());
            IAtom newAtom2 = new Atom(Elements.Oxygen.ToIElement());
            molecule.Add(newAtom);
            molecule.Add(newAtom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            IAtomType type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);
            type = matcher.FindMatchingAtomType(molecule, newAtom2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom2, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.IsNotNull(newAtom2.ImplicitHydrogenCount);
            Assert.AreEqual(3, newAtom.ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, newAtom2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestHCN()
        {
            IAtomContainer molecule = new AtomContainer();
            IAtom newAtom = new Atom(Elements.Carbon.ToIElement());
            IAtom newAtom2 = new Atom(Elements.Nitrogen.ToIElement());
            molecule.Add(newAtom);
            molecule.Add(newAtom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Triple);
            IAtomType type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);
            type = matcher.FindMatchingAtomType(molecule, newAtom2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom2, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.IsNotNull(newAtom2.ImplicitHydrogenCount);
            Assert.AreEqual(1, newAtom.ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, newAtom2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestMethylAmine()
        {
            IAtomContainer molecule = new AtomContainer();
            IAtom newAtom = new Atom(Elements.Carbon.ToIElement());
            IAtom newAtom2 = new Atom(Elements.Nitrogen.ToIElement());
            molecule.Add(newAtom);
            molecule.Add(newAtom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            IAtomType type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);
            type = matcher.FindMatchingAtomType(molecule, newAtom2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom2, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.IsNotNull(newAtom2.ImplicitHydrogenCount);
            Assert.AreEqual(3, newAtom.ImplicitHydrogenCount.Value);
            Assert.AreEqual(2, newAtom2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestMethyleneImine()
        {
            IAtomContainer molecule = new AtomContainer();
            IAtom newAtom = new Atom(Elements.Carbon.ToIElement());
            IAtom newAtom2 = new Atom(Elements.Nitrogen.ToIElement());
            molecule.Add(newAtom);
            molecule.Add(newAtom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            IAtomType type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);
            type = matcher.FindMatchingAtomType(molecule, newAtom2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom2, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.IsNotNull(newAtom2.ImplicitHydrogenCount);
            Assert.AreEqual(2, newAtom.ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, newAtom2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestSulphur()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("S");
            mol.Add(atom);
            IAtomType type = matcher.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(atom, type);

            Assert.IsNull(atom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(1, mol.Atoms.Count);
            Assert.IsNotNull(atom.ImplicitHydrogenCount);
            Assert.AreEqual(2, atom.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestProton()
        {
            IAtomContainer mol = new AtomContainer();
            Atom proton = new Atom("H");
            proton.FormalCharge = +1;
            mol.Add(proton);
            IAtomType type = matcher.FindMatchingAtomType(mol, proton);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(proton, type);

            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(1, mol.Atoms.Count);
            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula(mol);
            Assert.AreEqual(1,
                    MolecularFormulaManipulator.GetElementCount(formula, mol.Builder.CreateElement("H")));
            Assert.AreEqual(0, mol.GetConnectedBonds(proton).Count());
            Assert.IsNotNull(proton.ImplicitHydrogenCount);
            Assert.AreEqual(0, proton.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestHydrogen()
        {
            IAtomContainer mol = new AtomContainer();
            Atom proton = new Atom("H");
            mol.Add(proton);
            IAtomType type = matcher.FindMatchingAtomType(mol, proton);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(proton, type);

            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(1, mol.Atoms.Count);
            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula(mol);
            Assert.AreEqual(2,
                    MolecularFormulaManipulator.GetElementCount(formula, mol.Builder.CreateElement("H")));
            Assert.AreEqual(0, mol.GetConnectedBonds(proton).Count());
            Assert.IsNotNull(proton.ImplicitHydrogenCount);
            Assert.AreEqual(1, proton.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestAmmonia()
        {
            IAtomContainer mol = new AtomContainer();
            Atom nitrogen = new Atom("N");
            mol.Add(nitrogen);
            IAtomType type = matcher.FindMatchingAtomType(mol, nitrogen);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(nitrogen, type);

            adder.AddImplicitHydrogens(mol);
            Assert.IsNotNull(nitrogen.ImplicitHydrogenCount);
            Assert.AreEqual(3, nitrogen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestAmmonium()
        {
            IAtomContainer mol = new AtomContainer();
            Atom nitrogen = new Atom("N");
            nitrogen.FormalCharge = +1;
            mol.Add(nitrogen);
            IAtomType type = matcher.FindMatchingAtomType(mol, nitrogen);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(nitrogen, type);

            adder.AddImplicitHydrogens(mol);
            Assert.IsNotNull(nitrogen.ImplicitHydrogenCount);
            Assert.AreEqual(4, nitrogen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestWater()
        {
            IAtomContainer mol = new AtomContainer();
            Atom oxygen = new Atom("O");
            mol.Add(oxygen);
            IAtomType type = matcher.FindMatchingAtomType(mol, oxygen);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(oxygen, type);

            adder.AddImplicitHydrogens(mol);
            Assert.IsNotNull(oxygen.ImplicitHydrogenCount);
            Assert.AreEqual(2, oxygen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestHydroxonium()
        {
            IAtomContainer mol = new AtomContainer();
            Atom oxygen = new Atom("O");
            oxygen.FormalCharge = +1;
            mol.Add(oxygen);
            IAtomType type = matcher.FindMatchingAtomType(mol, oxygen);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(oxygen, type);

            adder.AddImplicitHydrogens(mol);
            Assert.IsNotNull(oxygen.ImplicitHydrogenCount);
            Assert.AreEqual(3, oxygen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestHydroxyl()
        {
            IAtomContainer mol = new AtomContainer();
            Atom oxygen = new Atom("O");
            oxygen.FormalCharge = -1;
            mol.Add(oxygen);
            IAtomType type = matcher.FindMatchingAtomType(mol, oxygen);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(oxygen, type);

            adder.AddImplicitHydrogens(mol);
            Assert.IsNotNull(oxygen.ImplicitHydrogenCount);
            Assert.AreEqual(1, oxygen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestHalogens()
        {
            HalogenTest("I");
            HalogenTest("F");
            HalogenTest("Cl");
            HalogenTest("Br");
        }

        [TestMethod()]
        public void TestHalogenAnions()
        {
            NegativeHalogenTest("I");
            NegativeHalogenTest("F");
            NegativeHalogenTest("Cl");
            NegativeHalogenTest("Br");
        }

        private void HalogenTest(string halogen)
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom(halogen);
            mol.Add(atom);
            IAtomType type = matcher.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(atom, type);

            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(1, mol.Atoms.Count);
            Assert.IsNotNull(atom.ImplicitHydrogenCount);
            Assert.AreEqual(1, atom.ImplicitHydrogenCount.Value);
        }

        private void NegativeHalogenTest(string halogen)
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom(halogen);
            atom.FormalCharge = -1;
            mol.Add(atom);
            IAtomType type = matcher.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(atom, type);

            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(1, mol.Atoms.Count);
            Assert.IsNotNull(atom.ImplicitHydrogenCount);
            Assert.AreEqual(0, atom.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestSulfite()
        {
            IAtomContainer mol = new AtomContainer();
            Atom s = new Atom("S");
            Atom o1 = new Atom("O");
            Atom o2 = new Atom("O");
            Atom o3 = new Atom("O");
            mol.Add(s);
            mol.Add(o1);
            mol.Add(o2);
            mol.Add(o3);
            Bond b1 = new Bond(s, o1, BondOrder.Single);
            Bond b2 = new Bond(s, o2, BondOrder.Single);
            Bond b3 = new Bond(s, o3, BondOrder.Double);
            mol.Add(b1);
            mol.Add(b2);
            mol.Add(b3);
            IAtomType type = matcher.FindMatchingAtomType(mol, s);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(s, type);
            type = matcher.FindMatchingAtomType(mol, o1);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(o1, type);
            type = matcher.FindMatchingAtomType(mol, o2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(o2, type);
            type = matcher.FindMatchingAtomType(mol, o3);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(o3, type);

            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(4, mol.Atoms.Count);
            Assert.AreEqual(3, mol.Bonds.Count);
            Assert.IsNotNull(s.ImplicitHydrogenCount);
            Assert.AreEqual(0, s.ImplicitHydrogenCount.Value);
            Assert.IsNotNull(o1.ImplicitHydrogenCount);
            Assert.AreEqual(1, o1.ImplicitHydrogenCount.Value);
            Assert.IsNotNull(o2.ImplicitHydrogenCount);
            Assert.AreEqual(1, o2.ImplicitHydrogenCount.Value);
            Assert.IsNotNull(o3.ImplicitHydrogenCount);
            Assert.AreEqual(0, o3.ImplicitHydrogenCount.Value);

        }

        [TestMethod()]
        public void TestAceticAcid()
        {
            IAtomContainer mol = new AtomContainer();
            Atom carbonylOxygen = new Atom("O");
            Atom hydroxylOxygen = new Atom("O");
            Atom methylCarbon = new Atom("C");
            Atom carbonylCarbon = new Atom("C");
            mol.Add(carbonylOxygen);
            mol.Add(hydroxylOxygen);
            mol.Add(methylCarbon);
            mol.Add(carbonylCarbon);
            Bond b1 = new Bond(methylCarbon, carbonylCarbon, BondOrder.Single);
            Bond b2 = new Bond(carbonylOxygen, carbonylCarbon, BondOrder.Double);
            Bond b3 = new Bond(hydroxylOxygen, carbonylCarbon, BondOrder.Single);
            mol.Add(b1);
            mol.Add(b2);
            mol.Add(b3);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(4, mol.Atoms.Count);
            Assert.AreEqual(3, mol.Bonds.Count);
            Assert.AreEqual(0, carbonylOxygen.ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, hydroxylOxygen.ImplicitHydrogenCount.Value);
            Assert.AreEqual(3, methylCarbon.ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, carbonylCarbon.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestEthane()
        {
            IAtomContainer mol = new AtomContainer();
            Atom carbon1 = new Atom("C");
            Atom carbon2 = new Atom("C");
            Bond b = new Bond(carbon1, carbon2, BondOrder.Single);
            mol.Add(carbon1);
            mol.Add(carbon2);
            mol.Add(b);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            Assert.AreEqual(3, carbon1.ImplicitHydrogenCount.Value);
            Assert.AreEqual(3, carbon2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestEthaneWithPReSetImplicitHCount()
        {
            IAtomContainer mol = new AtomContainer();
            Atom carbon1 = new Atom("C");
            Atom carbon2 = new Atom("C");
            Bond b = new Bond(carbon1, carbon2, BondOrder.Single);
            mol.Add(carbon1);
            mol.Add(carbon2);
            mol.Add(b);
            carbon1.ImplicitHydrogenCount = 3;
            carbon2.ImplicitHydrogenCount = 3;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            Assert.AreEqual(3, carbon1.ImplicitHydrogenCount.Value);
            Assert.AreEqual(3, carbon2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestEthene()
        {
            IAtomContainer mol = new AtomContainer();
            Atom carbon1 = new Atom("C");
            Atom carbon2 = new Atom("C");
            Bond b = new Bond(carbon1, carbon2, BondOrder.Double);
            mol.Add(carbon1);
            mol.Add(carbon2);
            mol.Add(b);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            Assert.AreEqual(2, carbon1.ImplicitHydrogenCount.Value);
            Assert.AreEqual(2, carbon2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestEthyne()
        {
            IAtomContainer mol = new AtomContainer();
            Atom carbon1 = new Atom("C");
            Atom carbon2 = new Atom("C");
            Bond b = new Bond(carbon1, carbon2, BondOrder.Triple);
            mol.Add(carbon1);
            mol.Add(carbon2);
            mol.Add(b);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            Assert.AreEqual(1, carbon1.ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, carbon2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestAromaticSaturation()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Single); // 6
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single); // 7
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Triple); // 8

            for (int f = 0; f < 6; f++)
            {
                mol.Atoms[f].IsAromatic = true;
                mol.Atoms[f].Hybridization = Hybridization.SP2;
                mol.Bonds[f].IsAromatic = true;
            }
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(6, AtomContainerManipulator.GetTotalHydrogenCount(mol));
        }

        [TestMethod()]
        public void TestaddImplicitHydrogensToSatisfyValency_OldValue()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C"));
            Atom oxygen = new Atom("O");
            mol.Add(oxygen);
            mol.Add(new Atom("C"));

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.IsNotNull(oxygen.ImplicitHydrogenCount);
            Assert.AreEqual(0, oxygen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestAdenine()
        {
            IAtomContainer mol = new AtomContainer(); // Adenine
            IAtom a1 = mol.Builder.CreateAtom("C");
            a1.Point2D = new Vector2(21.0223, -17.2946);
            mol.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("C");
            a2.Point2D = new Vector2(21.0223, -18.8093);
            mol.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            a3.Point2D = new Vector2(22.1861, -16.6103);
            mol.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("N");
            a4.Point2D = new Vector2(19.8294, -16.8677);
            mol.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("N");
            a5.Point2D = new Vector2(22.2212, -19.5285);
            mol.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("N");
            a6.Point2D = new Vector2(19.8177, -19.2187);
            mol.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("N");
            a7.Point2D = new Vector2(23.4669, -17.3531);
            mol.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("N");
            a8.Point2D = new Vector2(22.1861, -15.2769);
            mol.Add(a8);
            IAtom a9 = mol.Builder.CreateAtom("C");
            a9.Point2D = new Vector2(18.9871, -18.0139);
            mol.Add(a9);
            IAtom a10 = mol.Builder.CreateAtom("C");
            a10.Point2D = new Vector2(23.4609, -18.8267);
            mol.Add(a10);
            IBond b1 = mol.Builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a2, a5, BondOrder.Single);
            mol.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a2, a6, BondOrder.Single);
            mol.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a3, a7, BondOrder.Double);
            mol.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a3, a8, BondOrder.Single);
            mol.Add(b7);
            IBond b8 = mol.Builder.CreateBond(a4, a9, BondOrder.Double);
            mol.Add(b8);
            IBond b9 = mol.Builder.CreateBond(a5, a10, BondOrder.Double);
            mol.Add(b9);
            IBond b10 = mol.Builder.CreateBond(a6, a9, BondOrder.Single);
            mol.Add(b10);
            IBond b11 = mol.Builder.CreateBond(a7, a10, BondOrder.Single);
            mol.Add(b11);

            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(5, AtomContainerManipulator.GetTotalHydrogenCount(mol));
        }

        /// <summary>
        // @cdk.bug 1727373
        ///
        /// </summary>
        [TestMethod()]
        public void TestBug1727373()
        {
            IAtomContainer molecule = null;
            string filename = "NCDK.Data.MDL.carbocations.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            molecule = reader.Read(new AtomContainer());
            FindAndConfigureAtomTypesForAllAtoms(molecule);
            adder.AddImplicitHydrogens(molecule);
            Assert.AreEqual(2, molecule.Atoms[0].ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, molecule.Atoms[1].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[2].ImplicitHydrogenCount.Value);
            Assert.AreEqual(2, molecule.Atoms[3].ImplicitHydrogenCount.Value);
        }

        /// <summary>
        // @cdk.bug 1575269
        /// </summary>
        [TestMethod()]
        public void TestBug1575269()
        {
            string filename = "NCDK.Data.MDL.furan.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = reader.Read(new AtomContainer());
            FindAndConfigureAtomTypesForAllAtoms(molecule);
            adder.AddImplicitHydrogens(molecule);
            Assert.AreEqual(1, molecule.Atoms[0].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[1].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[2].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[3].ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestImpHByAtom()
        {
            string filename = "NCDK.Data.MDL.furan.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = reader.Read(new AtomContainer());
            FindAndConfigureAtomTypesForAllAtoms(molecule);
            foreach (var atom in molecule.Atoms)
            {
                adder.AddImplicitHydrogens(molecule, atom);
            }
            Assert.AreEqual(1, molecule.Atoms[0].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[1].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[2].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[3].ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestPseudoAtom()
        {
            IAtomContainer molecule = new AtomContainer();
            molecule.Add(new PseudoAtom("Waterium"));
            FindAndConfigureAtomTypesForAllAtoms(molecule);
            Assert.IsNull(molecule.Atoms[0].ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void TestNaCl()
        {
            IAtomContainer mol = new AtomContainer();
            Atom cl = new Atom("Cl");
            cl.FormalCharge = -1;
            mol.Add(cl);
            Atom na = new Atom("Na");
            na.FormalCharge = +1;
            mol.Add(na);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(0, AtomContainerManipulator.GetTotalHydrogenCount(mol));
            Assert.AreEqual(0, mol.GetConnectedBonds(cl).Count());
            Assert.AreEqual(0, cl.ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, mol.GetConnectedBonds(na).Count());
            Assert.AreEqual(0, na.ImplicitHydrogenCount.Value);
        }

        /// <summary>
        // @cdk.bug 1244612
        /// </summary>
        [TestMethod()]
        public void TestSulfurCompound_ImplicitHydrogens()
        {
            string filename = "NCDK.Data.MDL.sulfurCompound.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IList<IAtomContainer> containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);

            IAtomContainer atomContainer_0 = (IAtomContainer)containersList[0];
            Assert.AreEqual(10, atomContainer_0.Atoms.Count);
            IAtom sulfur = atomContainer_0.Atoms[1];
            FindAndConfigureAtomTypesForAllAtoms(atomContainer_0);
            adder.AddImplicitHydrogens(atomContainer_0);
            Assert.AreEqual("S", sulfur.Symbol);
            Assert.IsNotNull(sulfur.ImplicitHydrogenCount);
            Assert.AreEqual(0, sulfur.ImplicitHydrogenCount.Value);
            Assert.AreEqual(3, atomContainer_0.GetConnectedAtoms(sulfur).Count());

            Assert.AreEqual(10, atomContainer_0.Atoms.Count);

            Assert.IsNotNull(sulfur.ImplicitHydrogenCount);
            Assert.AreEqual(0, sulfur.ImplicitHydrogenCount.Value);
            Assert.AreEqual(3, atomContainer_0.GetConnectedAtoms(sulfur).Count());
        }

        /// <summary>
        // @cdk.bug 1627763
        /// </summary>
        [TestMethod()]
        public void TestBug1627763()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(mol.Builder.CreateAtom("C"));
            mol.Add(mol.Builder.CreateAtom("O"));
            mol.Add(mol.Builder.CreateBond(mol.Atoms[0], mol.Atoms[1],
                    BondOrder.Single));
            AddExplicitHydrogens(mol);
            int hCount = 0;
            IEnumerator<IAtom> neighbors = mol.GetConnectedAtoms(mol.Atoms[0]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(3, hCount);
            hCount = 0;
            neighbors = mol.GetConnectedAtoms(mol.Atoms[1]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(1, hCount);
        }

        [TestMethod()]
        public void TestMercaptan()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(mol.Builder.CreateAtom("C"));
            mol.Add(mol.Builder.CreateAtom("C"));
            mol.Add(mol.Builder.CreateAtom("C"));
            mol.Add(mol.Builder.CreateAtom("S"));
            mol.Add(mol.Builder.CreateBond(mol.Atoms[0], mol.Atoms[1],
                    BondOrder.Double));
            mol.Add(mol.Builder.CreateBond(mol.Atoms[1], mol.Atoms[2],
                    BondOrder.Single));
            mol.Add(mol.Builder.CreateBond(mol.Atoms[2], mol.Atoms[3],
                    BondOrder.Single));
            AddExplicitHydrogens(mol);
            int hCount = 0;
            IEnumerator<IAtom> neighbors = mol.GetConnectedAtoms(mol.Atoms[0]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(2, hCount);
            hCount = 0;
            neighbors = mol.GetConnectedAtoms(mol.Atoms[1]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(1, hCount);
            hCount = 0;
            neighbors = mol.GetConnectedAtoms(mol.Atoms[2]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(2, hCount);
            hCount = 0;
            neighbors = mol.GetConnectedAtoms(mol.Atoms[3]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(1, hCount);
        }

        [TestMethod()]
        public void UnknownAtomTypeLeavesHydrogenCountAlone()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            CDKHydrogenAdder hydrogenAdder = CDKHydrogenAdder.GetInstance(bldr);
            IAtomContainer container = bldr.CreateAtomContainer();
            IAtom atom = bldr.CreateAtom("C");
            atom.ImplicitHydrogenCount = 3;
            atom.AtomTypeName = "X";
            container.Add(atom);
            hydrogenAdder.AddImplicitHydrogens(container);
            Assert.AreEqual(3, atom.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void UnknownAtomTypeLeavesHydrogenCountAloneUnlessNull()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            CDKHydrogenAdder hydrogenAdder = CDKHydrogenAdder.GetInstance(bldr);
            IAtomContainer container = bldr.CreateAtomContainer();
            IAtom atom = bldr.CreateAtom("C");
            atom.ImplicitHydrogenCount = null;
            atom.AtomTypeName = "X";
            container.Add(atom);
            hydrogenAdder.AddImplicitHydrogens(container);
            Assert.AreEqual(0, atom.ImplicitHydrogenCount);
        }

        private void FindAndConfigureAtomTypesForAllAtoms(IAtomContainer container)
        {
            IEnumerator<IAtom> atoms = container.Atoms.GetEnumerator();
            while (atoms.MoveNext())
            {
                IAtom atom = atoms.Current;
                IAtomType type = matcher.FindMatchingAtomType(container, atom);
                Assert.IsNotNull(type);
                AtomTypeManipulator.Configure(atom, type);
            }
        }
    }
}
