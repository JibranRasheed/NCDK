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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;

namespace NCDK.Geometries.CIP
{
    // @cdk.module test-cip
    [TestClass()]
    public class LigandTest : CDKTestCase
    {
        [TestMethod()]
        public void TestConstructorAndGetMethods()
        {
            SmilesParser smiles = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer molecule = smiles.ParseSmiles("ClC(Br)(I)[H]");

            ILigand ligand = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            Assert.IsNotNull(ligand);
            Assert.AreEqual(molecule, ligand.GetAtomContainer());
            Assert.AreEqual(molecule.Atoms[1], ligand.GetCentralAtom());
            Assert.AreEqual(molecule.Atoms[0], ligand.GetLigandAtom());
        }

        [TestMethod()]
        public void TestVisitedTracking()
        {
            SmilesParser smiles = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer molecule = smiles.ParseSmiles("ClC(Br)(I)[H]");

            ILigand ligand = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            Assert.IsTrue(ligand.GetVisitedAtoms().IsVisited(molecule.Atoms[1]));
            Assert.IsTrue(ligand.IsVisited(molecule.Atoms[1]));
            Assert.IsFalse(ligand.GetVisitedAtoms().IsVisited(molecule.Atoms[0]));
            Assert.IsFalse(ligand.IsVisited(molecule.Atoms[0]));
        }
    }
}
