/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using NCDK.Silent;
using System;

namespace NCDK.Stereo
{
    // @cdk.module test-core
    [TestClass()]
    public class TetrahedralChiralityTest : CDKTestCase
    {
        private static IAtomContainer molecule;
        private static IAtom[] ligands;

        static TetrahedralChiralityTest()
        {
            molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("Cl"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("Br"));
            molecule.Atoms.Add(new Atom("I"));
            molecule.Atoms.Add(new Atom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            ligands = new IAtom[] { molecule.Atoms[4], molecule.Atoms[3], molecule.Atoms[2], molecule.Atoms[0] };
        }

        [TestMethod()]
        public void TestTetrahedralChirality_IAtom_arrayIAtom_ITetrahedralChirality_Stereo()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            Assert.IsNotNull(chirality);
        }

        [TestMethod()]
        public void TestBuilder()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            chirality.Builder = ChemObjectBuilder.Instance;
            Assert.AreEqual(ChemObjectBuilder.Instance, chirality.Builder);
        }

        [TestMethod()]
        public void TestGetChiralAtom()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            Assert.IsNotNull(chirality);
            Assert.AreEqual(molecule.Atoms[1], chirality.ChiralAtom);
        }

        [TestMethod()]
        public void TestGetStereo()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            Assert.IsNotNull(chirality);
            Assert.AreEqual(molecule.Atoms[1], chirality.ChiralAtom);
            for (int i = 0; i < ligands.Length; i++)
            {
                Assert.AreEqual(ligands[i], chirality.Ligands[i]);
            }
            Assert.AreEqual(TetrahedralStereo.Clockwise, chirality.Stereo);
        }

        [TestMethod()]
        public void TestGetLigands()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            Assert.IsNotNull(chirality);
            for (int i = 0; i < ligands.Length; i++)
            {
                Assert.AreEqual(ligands[i], chirality.Ligands[i]);
            }
        }

        [TestMethod()]
        public void TestMap_Map_Map()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;

            IAtom c1 = builder.NewAtom("C");
            IAtom o2 = builder.NewAtom("O");
            IAtom n3 = builder.NewAtom("N");
            IAtom c4 = builder.NewAtom("C");
            IAtom h5 = builder.NewAtom("H");

            // new stereo element
            ITetrahedralChirality original = new TetrahedralChirality(c1, new IAtom[] { o2, n3, c4, h5 }, TetrahedralStereo.Clockwise);

            // clone the atoms and place in a map
            var mapping = new CDKObjectMap();
            IAtom c1clone = (IAtom)c1.Clone();
            mapping.Set(c1, c1clone);
            IAtom o2clone = (IAtom)o2.Clone();
            mapping.Set(o2, o2clone);
            IAtom n3clone = (IAtom)n3.Clone();
            mapping.Set(n3, n3clone);
            IAtom c4clone = (IAtom)c4.Clone();
            mapping.Set(c4, c4clone);
            IAtom h5clone = (IAtom)h5.Clone();
            mapping.Set(h5, h5clone);

            // map the existing element a new element
            ITetrahedralChirality mapped = (ITetrahedralChirality)original.Clone(mapping);

            Assert.AreNotSame(original.ChiralAtom, mapped.ChiralAtom, "mapped chiral atom was the same as the original");
            Assert.AreSame(c1clone, mapped.ChiralAtom, "mapped chiral atom was not the clone");

            var originalLigands = original.Ligands;
            var mappedLigands = mapped.Ligands;

            Assert.AreNotSame(originalLigands[0], mappedLigands[0], "first ligand was the same as the original");
            Assert.AreSame(o2clone, mappedLigands[0], "first mapped ligand was not the clone");
            Assert.AreNotSame(originalLigands[1], mappedLigands[1], "second ligand was the same as the original");
            Assert.AreSame(n3clone, mappedLigands[1], "second mapped ligand was not the clone");
            Assert.AreNotSame(originalLigands[2], mappedLigands[2], "third ligand was the same as the original");
            Assert.AreSame(c4clone, mappedLigands[2], "third mapped ligand was not the clone");
            Assert.AreNotSame(originalLigands[3], mappedLigands[3], "forth ligand was te same as the original");
            Assert.AreSame(h5clone, mappedLigands[3], "forth mapped ligand was not the clone");

            Assert.AreEqual(original.Stereo, mapped.Stereo, "stereo was not mapped");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMap_Null_Map()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;

            IAtom c1 = builder.NewAtom("C");
            IAtom o2 = builder.NewAtom("O");
            IAtom n3 = builder.NewAtom("N");
            IAtom c4 = builder.NewAtom("C");
            IAtom h5 = builder.NewAtom("H");

            // new stereo element
            ITetrahedralChirality original = new TetrahedralChirality(c1, new IAtom[] { o2, n3, c4, h5 }, TetrahedralStereo.Clockwise);

            // map the existing element a new element 
            ITetrahedralChirality mapped = (ITetrahedralChirality)original.Clone(null);
        }

        [TestMethod()] 
        public void TestMap_Map_Map_EmptyMapping()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;

            IAtom c1 = builder.NewAtom("C");
            IAtom o2 = builder.NewAtom("O");
            IAtom n3 = builder.NewAtom("N");
            IAtom c4 = builder.NewAtom("C");
            IAtom h5 = builder.NewAtom("H");

            // new stereo element
            ITetrahedralChirality original = new TetrahedralChirality(c1, new IAtom[] { o2, n3, c4, h5 }, TetrahedralStereo.Clockwise);

            // map the existing element a new element - should through an IllegalArgumentException
            var map = new CDKObjectMap();
            ITetrahedralChirality mapped = (ITetrahedralChirality)original.Clone(map);

            Assert.AreSame(original.ChiralAtom, mapped.ChiralAtom);
        }

        [TestMethod()]
        public void Contains()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;

            IAtom c1 = builder.NewAtom("C");
            IAtom o2 = builder.NewAtom("O");
            IAtom n3 = builder.NewAtom("N");
            IAtom c4 = builder.NewAtom("C");
            IAtom h5 = builder.NewAtom("H");

            // new stereo element
            ITetrahedralChirality element = new TetrahedralChirality(c1, new IAtom[] { o2, n3, c4, h5 }, TetrahedralStereo.Clockwise);

            Assert.IsTrue(element.Contains(c1));
            Assert.IsTrue(element.Contains(o2));
            Assert.IsTrue(element.Contains(n3));
            Assert.IsTrue(element.Contains(c4));
            Assert.IsTrue(element.Contains(h5));

            Assert.IsFalse(element.Contains(builder.NewAtom()));
            Assert.IsFalse(element.Contains(null));
        }

        [TestMethod()]
        public void TestToString()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            string stringRepr = chirality.ToString();
            Assert.AreNotSame(0, stringRepr.Length);
            Assert.IsFalse(stringRepr.Contains("\n"));
        }
    }
}
