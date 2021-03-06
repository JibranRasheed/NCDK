/*
 *  Copyright (C) 2009  Mark Rijnbeek <markrynbeek@gmail.com>
 *
 *  Contact: cdk-devel@list.sourceforge.net
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;
using System.Collections.Generic;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    /// Test class for <see cref="AtomContainerComparatorBy2DCenter{T}"/>.
    /// </summary>
    // @cdk.module test-standard
    [TestClass()]
    public class AtomContainerComparatorBy2DCenterTest : CDKTestCase
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;

        public AtomContainerComparatorBy2DCenterTest()
            : base()
        { }

        [TestMethod()]
        public void TestCompare_Null_Null()
        {
            var comparator = new AtomContainerComparatorBy2DCenter<IAtomContainer>();
            Assert.AreEqual(0, comparator.Compare(null, null), "null <-> null");
        }

        [TestMethod()]
        public void TestCompare_Null_2DCoordinates()
        {
            var atomContainer = builder.NewAtomContainer();
            atomContainer.Atoms.Add(builder.NewAtom("N"));
            var comparator = new AtomContainerComparatorBy2DCenter<IAtomContainer>();
            Assert.AreEqual(0, comparator.Compare(atomContainer, atomContainer), "null 2d Coords<-> null 2d coords");
        }

        [TestMethod()]
        public void TestCompare_self_valid_2DCoordinates()
        {
            var atomContainer = builder.NewAtomContainer();
            var atom = builder.NewAtom("N");
            atom.Point2D = new Vector2(10, 10);
            atomContainer.Atoms.Add(atom);

            var comparator = new AtomContainerComparatorBy2DCenter<IAtomContainer>();
            Assert.AreEqual(0, comparator.Compare(atomContainer, atomContainer), "self 2d Coords<-> self 2d coords");
        }

        [TestMethod()]
        public void TestCompare_minusOne()
        {
            var atomContainer = builder.NewAtomContainer();
            var atom = builder.NewAtom("N");
            atom.Point2D = new Vector2(10, 10);
            atomContainer.Atoms.Add(atom);

            var atomContainer2 = builder.NewAtomContainer();
            var atom2 = builder.NewAtom("P");
            atom2.Point2D = new Vector2(20, 5);
            atomContainer2.Atoms.Add(atom2);

            var comparator = new AtomContainerComparatorBy2DCenter<IAtomContainer>();
            Assert.AreEqual(-1, comparator.Compare(atomContainer, atomContainer2), "(10,10)<-> (20,10)");
        }

        [TestMethod()]
        public void TestCompare_plusOne()
        {
            var atomContainer = builder.NewAtomContainer();
            var atom = builder.NewAtom("N");
            atom.Point2D = new Vector2(20, 10);
            atomContainer.Atoms.Add(atom);

            var atomContainer2 = builder.NewAtomContainer();
            var atom2 = builder.NewAtom("P");
            atom2.Point2D = new Vector2(20, 5);
            atomContainer2.Atoms.Add(atom2);

            var comparator = new AtomContainerComparatorBy2DCenter<IAtomContainer>();
            Assert.AreEqual(1, comparator.Compare(atomContainer, atomContainer2), "(20,10)<-> (20,5)");
        }
    }
}
