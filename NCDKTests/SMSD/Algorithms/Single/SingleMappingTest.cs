/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.Default;

namespace NCDK.SMSD.Algorithms.Single
{
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public class SingleMappingTest
    {
        public SingleMappingTest() { }

        /// <summary>
        /// Test of getOverLaps method, of class SingleMapping.
        /// </summary>
        [TestMethod()]
        public void TestGetOverLaps()
        {
            IAtom atomSource = new Atom("R");
            IAtom atomTarget = new Atom("R");
            IAtomContainer source = new AtomContainer();
            source.Atoms.Add(atomSource);
            IAtomContainer target = new AtomContainer();
            target.Atoms.Add(atomTarget);
            bool removeHydrogen = false;
            SingleMapping instance = new SingleMapping();
            Assert.IsNotNull(instance.GetOverLaps(source, target, removeHydrogen));
        }
    }
}