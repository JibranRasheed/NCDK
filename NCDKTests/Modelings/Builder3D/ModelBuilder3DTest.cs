/* Copyright (C) 1997-2007  Christian Hoppe <chhoppe@users.sf.net>
 *                     2006  Mario Baseda
 *
 *  Contact: cdk-devel@list.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Geometries;
using NCDK.IO;
using NCDK.Layout;
using NCDK.Numerics;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NCDK.Modelings.Builder3D
{
    /// <summary>
    /// Description of the Class
    /// </summary>
    // @cdk.module test-builder3d
    // @author chhoppe
    // @cdk.created    2004-11-04
    [TestClass()]
    public class ModelBuilder3DTest : CDKTestCase
    {
        bool standAlone = false;

        /// <summary>
        /// Sets the standAlone attribute
        /// </summary>
        /// <param name="standAlone">The new standAlone value</param>
        public void SetStandAlone(bool standAlone)
        {
            this.standAlone = standAlone;
        }

        /// <summary>
        /// A unit test with methylenfluoride
        /// </summary>
        [TestMethod()]
        public void TestModelBuilder3D_CF()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            Vector3 c_coord = new Vector3(1.392, 0.0, 0.0);
            Vector3 f_coord = new Vector3(0.0, 0.0, 0.0);
            Vector3 h1_coord = new Vector3(1.7439615035767404, 1.0558845107302222, 0.0);
            Vector3 h2_coord = new Vector3(1.7439615035767404, -0.5279422553651107, 0.914422809754875);
            Vector3 h3_coord = new Vector3(1.7439615035767402, -0.5279422553651113, -0.9144228097548747);

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CF");
            AddExplicitHydrogens(mol);
            //mb3d.SetTemplateHandler();
            mol = mb3d.Generate3DCoordinates(mol, false);
            AssertAreEqual(c_coord, mol.Atoms[0].Point3D, 0.0001);
            AssertAreEqual(f_coord, mol.Atoms[1].Point3D, 0.0001);
            AssertAreEqual(h1_coord, mol.Atoms[2].Point3D, 0.0001);
            AssertAreEqual(h2_coord, mol.Atoms[3].Point3D, 0.0001);
            AssertAreEqual(h3_coord, mol.Atoms[4].Point3D, 0.0001);
            CheckAverageBondLength(mol);
        }

        [TestMethod()]
        public void TestModelBuilder3D_CccccC()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            string smile = "CccccC";
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles(smile);
            AddExplicitHydrogens(mol);
            mol = mb3d.Generate3DCoordinates(mol, false);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                Assert.IsNotNull(mol.Atoms[i].Point3D);
            }
            CheckAverageBondLength(mol);
            //Debug.WriteLine("Layout molecule with SMILE: "+smile);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestModelBuilder3D_c1ccccc1C0()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            string smile = "c1ccccc1C=O";
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles(smile);
            AddExplicitHydrogens(mol);
            mb3d.Generate3DCoordinates(mol, false);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                Assert.IsNotNull(mol.Atoms[i].Point3D);
            }
            CheckAverageBondLength(mol);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestModelBuilder3D_Konstanz()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(ChemObjectBuilder.Instance);
            string smile = "C12(-[H])-C3(-C(-[H])(-[H])-C(-C4(-C5(-C(-Cl)(-Cl)-C(-C-3-4-[H])(-Cl)-C(-Cl)(-[H])-C-5(-Cl)-[H])-Cl)-[H])(-[H])-C-2(-O-1)-[H])-[H]";
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles(smile);
            AddExplicitHydrogens(mol);
            mol = mb3d.Generate3DCoordinates(mol, false);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                Assert.IsNotNull(mol.Atoms[i].Point3D);
            }
            CheckAverageBondLength(mol);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void XTestModelBuilder3D_Konstanz2()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            string smile = "c1(:c(:c(:c(-[H]):c(-Cl):c:1-[H])-[H])-[H])-[H]";
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles(smile);
            AddExplicitHydrogens(mol);
            mol = mb3d.Generate3DCoordinates(mol, false);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                Assert.IsNotNull(mol.Atoms[i].Point3D);
            }
            CheckAverageBondLength(mol);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestModelBuilder3D_C1CCCCCCC1CC()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            string smile = "C1CCCCCCC1CC";
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles(smile);
            AddExplicitHydrogens(mol);
            mol = mb3d.Generate3DCoordinates(mol, false);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                Assert.IsNotNull(mol.Atoms[i].Point3D);
            }
            CheckAverageBondLength(mol);
        }

        /// <summary>
        /// Bug #1610997 says the modelbuilder does not work if 2d coordinates exist before - we test this here
        /// </summary>
        // @cdk.bug 1610997
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestModelBuilder3D_CCCCCCCCCC_with2d()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            string smile = "CCCCCCCCCC";
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles(smile);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                mol.Atoms[i].Point2D = new Vector2(1, 1);
            }
            AddExplicitHydrogens(mol);
            mol = mb3d.Generate3DCoordinates(mol, false);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                Assert.IsNotNull(mol.Atoms[i].Point3D);
            }
            CheckAverageBondLength(mol);
        }

        // @cdk.bug 1315823
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestModelBuilder3D_232()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            string filename = "NCDK.Data.MDL.allmol232.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            var chemFile = reader.Read(new ChemFile());
            reader.Close();
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToReadOnlyList();
            IAtomContainer ac = new Silent.AtomContainer(containersList[0]);
            AddExplicitHydrogens(ac);
            ac = mb3d.Generate3DCoordinates(ac, false);
            Assert.IsNotNull(ac.Atoms[0].Point3D);
            CheckAverageBondLength(ac);
        }

        public static void CheckAverageBondLength(IAtomContainer ac)
        {
            double avlength = GeometryUtil.GetBondLengthAverage3D(ac);
            for (int i = 0; i < ac.Bonds.Count; i++)
            {
                double distance = Vector3.Distance(ac.Bonds[i].Begin.Point3D.Value, ac.Bonds[i].End.Point3D.Value);
                Assert.IsTrue(distance >= avlength / 2 && distance <= avlength * 2, "Unreasonable bond length (" + distance + ") for bond " + i);
            }
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestModelBuilder3D_231()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            string filename = "NCDK.Data.MDL.allmol231.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            var chemFile = reader.Read(new ChemFile());
            reader.Close();
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToArray();
            IAtomContainer ac = new Silent.AtomContainer(containersList[0]);
            AddExplicitHydrogens(ac);
            ac = mb3d.Generate3DCoordinates(ac, false);
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                Assert.IsNotNull(ac.Atoms[i].Point3D);
            }
            CheckAverageBondLength(ac);
        }

        /// <summary>
        /// Test for SF bug #1309731.
        /// </summary>
        // @cdk.bug 1309731
        [TestMethod()]
        public void TestModelBuilder3D_keepChemObjectIDs()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);

            IAtomContainer methanol = new Silent.AtomContainer();
            IChemObjectBuilder builder = methanol.Builder;

            IAtom carbon1 = builder.NewAtom("C");
            carbon1.Id = "carbon1";
            methanol.Atoms.Add(carbon1);
            for (int i = 0; i < 3; i++)
            {
                IAtom hydrogen = builder.NewAtom("H");
                methanol.Atoms.Add(hydrogen);
                methanol.Bonds.Add(builder.NewBond(carbon1, hydrogen, BondOrder.Single));
            }
            IAtom oxygen1 = builder.NewAtom("O");
            oxygen1.Id = "oxygen1";
            methanol.Atoms.Add(oxygen1);
            methanol.Bonds.Add(builder.NewBond(carbon1, oxygen1, BondOrder.Single));
            {
                IAtom hydrogen = builder.NewAtom("H");
                methanol.Atoms.Add(hydrogen);
                methanol.Bonds.Add(builder.NewBond(hydrogen, oxygen1, BondOrder.Single));
            }

            Assert.AreEqual(6, methanol.Atoms.Count);
            Assert.AreEqual(5, methanol.Bonds.Count);

            mb3d.Generate3DCoordinates(methanol, false);

            CheckAverageBondLength(methanol);
            Assert.AreEqual("carbon1", carbon1.Id);
            Assert.AreEqual("oxygen1", oxygen1.Id);
        }

        /// <summary>
        /// this is a test contributed by mario baseda / see bug #1610997
        /// </summary>
        // @cdk.bug 1610997
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestModel3D_bug_1610997()
        {
            //bool notCalculatedResults = false;
            var inputList = new List<IAtomContainer>();

            ////////////////////////////////////////////////////////////////////////////////////////////
            //generate the input molecules. This are molecules without x, y, z coordinates

            string[] smiles = new string[]{"CC", "OCC", "O(C)CCC", "c1ccccc1", "C(=C)=C", "OCC=CCc1ccccc1(C=C)",
                "O(CC=C)CCN", "CCCCCCCCCCCCCCC", "OCC=CCO", "NCCCCN"};
            var sp = CDK.SmilesParser;
            IAtomContainer[] atomContainer = new IAtomContainer[smiles.Length];
            for (int i = 0; i < smiles.Length; i++)
            {
                atomContainer[i] = sp.ParseSmiles(smiles[i]);

                inputList.Add(atomContainer[i]);
            }
            ///////////////////////////////////////////////////////////////////////////////////////////
            // Generate 2D coordinates for the input molecules with the Structure Diagram Generator

            StructureDiagramGenerator str;
            List<IAtomContainer> resultList = new List<IAtomContainer>();
            foreach (var molecule in inputList)
            {
                str = new StructureDiagramGenerator() { Molecule = molecule };
                str.GenerateCoordinates();
                resultList.Add(str.Molecule);
            }
            inputList = resultList;

            /////////////////////////////////////////////////////////////////////////////////////////////
            // Delete x and y coordinates

            foreach (var molecule in inputList)
                foreach (var atom in molecule.Atoms)
                    atom.Point2D = null;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // Test for the method Model3DBuildersWithMM2ForceField
            var builder = Silent.ChemObjectBuilder.Instance;

            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            for (var i = 0; i < inputList.Count; i++)
            {
                var input = inputList[i];
                {
                    // shallow copy
                    IAtomContainer mol = builder.NewAtomContainer(input);
                    try
                    {
                        mol = mb3d.Generate3DCoordinates(mol, false);
                        foreach (var a in mol.Atoms)
                            Assert.IsNotNull(a.Point3D, $"{smiles[0]} has unplaced atom");
                        CheckAverageBondLength(mol);
                    }
                    catch (Exception e)
                    {
                        if (e is CDKException || e is IOException)
                        {
                            Assert.Fail($"3D coordinated could not be generator for {smiles[i]}: {e.StackTrace}");
                        }
                        else
                            throw;
                    }
                }
            }
        }

        // @cdk.bug 1241421
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestModelBuilder3D_bug_1241421()
        {
            var mb3d = ModelBuilder3D.GetInstance(ChemObjectBuilder.Instance);
            string filename = "NCDK.Data.MDL.bug1241421.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            var chemFile = reader.Read(new ChemFile());
            reader.Close();
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToReadOnlyList();
            IAtomContainer ac = new Silent.AtomContainer(containersList[0]);
            ac = mb3d.Generate3DCoordinates(ac, false);
            CheckAverageBondLength(ac);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestModelBuilder3D_reserpine()
        {
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(ChemObjectBuilder.Instance);
            string filename = "NCDK.Data.MDL.reserpine.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            var chemFile = reader.Read(new ChemFile());
            reader.Close();
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToReadOnlyList();
            IAtomContainer ac = new Silent.AtomContainer(containersList[0]);
            ac = mb3d.Generate3DCoordinates(ac, false);
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                Assert.IsNotNull(ac.Atoms[i].Point3D);
            }
            CheckAverageBondLength(ac);
        }

        [TestMethod()]
        public void TestAlkanes()
        {
            string smiles1 = "CCCCCCCCCCCCCCCCCC";
            string smiles2 = "CCCCCC(CCCC)CCCC";
            SmilesParser parser = CDK.SmilesParser;
            var nonBranchedAlkane = parser.ParseSmiles(smiles1);
            var branchedAlkane = parser.ParseSmiles(smiles2);
            ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance).Generate3DCoordinates(nonBranchedAlkane,
                    false);
            ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance).Generate3DCoordinates(branchedAlkane, false);
        }

        [TestMethod()]
        public void HydrogenAsFirstAtomInMethane()
        {
            SmilesParser smipar = CDK.SmilesParser;
            var methane = smipar.ParseSmiles("[H]C([H])([H])[H]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            mb3d.Generate3DCoordinates(methane, false);
            foreach (var atom in methane.Atoms)
                Assert.IsNotNull(atom.Point3D);
        }

        [TestMethod()]
        public void HydrogenAsFirstAtomInEthane()
        {
            SmilesParser smipar = CDK.SmilesParser;
            var ethane = smipar.ParseSmiles("[H]C([H])([H])C([H])([H])[H]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ethane);
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Silent.ChemObjectBuilder.Instance);
            mb3d.Generate3DCoordinates(ethane, false);
            foreach (var atom in ethane.Atoms)
                Assert.IsNotNull(atom.Point3D);
        }
    }
}
