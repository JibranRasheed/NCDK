/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;

namespace NCDK
{
    /// <summary>
    /// TestCase for the AtomContainer class.
    /// </summary>
    // @cdk.module test-extra
    // @author  Christoph Steinbeck
    // @cdk.created 2001-08-09
    [TestClass()]
    public class CloneAtomContainerTest : CDKTestCase
    {
        [TestMethod()]
        public void TestClone()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeAlphaPinene();
            IAtomContainer clonedMol = (IAtomContainer)molecule.Clone();
            Assert.IsTrue(molecule.Atoms.Count == clonedMol.Atoms.Count);
            for (int f = 0; f < molecule.Atoms.Count; f++)
            {
                for (int g = 0; g < clonedMol.Atoms.Count; g++)
                {
                    Assert.IsNotNull(molecule.Atoms[f]);
                    Assert.IsNotNull(clonedMol.Atoms[g]);
                    Assert.IsTrue(molecule.Atoms[f] != clonedMol.Atoms[g]);
                }
            }
        }
    }
}
