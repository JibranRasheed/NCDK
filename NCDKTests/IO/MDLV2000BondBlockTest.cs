/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Isomorphisms.Matchers;
using System.IO;

namespace NCDK.IO
{
    // @author John May
    // @cdk.module test-io
    [TestClass()]
    public class MDLV2000BondBlockTest
    {
        private readonly MDLV2000Reader reader = new MDLV2000Reader(new StringReader(""));
        private static readonly IChemObjectBuilder builder = CDK.Builder;
        private readonly IAtom[] atoms = new IAtom[]
            {
                new Mock<IAtom>().Object, new Mock<IAtom>().Object,
                new Mock<IAtom>().Object, new Mock<IAtom>().Object,
                new Mock<IAtom>().Object
            };

        [TestMethod()]
        public void AtomNumbers()
        {
            var input = "  1  3  1  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(atoms[0], bond.Begin);
            Assert.AreEqual(atoms[2], bond.End);
        }

        [TestMethod()]
        public void SingleBond()
        {
            var input = "  1  3  1  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
        }

        [TestMethod()]
        public void DoubleBond()
        {
            var input = "  1  3  2  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Double, bond.Order);
            Assert.AreEqual(BondStereo.EZByCoordinates, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
        }

        [TestMethod()]
        public void TripleBond()
        {
            var input = "  1  3  3  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Triple, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
        }

        [TestMethod()]
        public void AromaticBond()
        {
            var input = "  1  3  4  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsTrue(bond.IsAromatic);
            Assert.IsTrue(bond.IsSingleOrDouble);
        }

        [TestMethod()]
        public void SingleOrDoubleBond()
        {
            var input = "  1  3  5  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
            Assert.IsInstanceOfType(bond, typeof(QueryBond));
            Assert.AreEqual(ExprType.SingleOrDouble, ((QueryBond)bond).Expression.GetExprType());
        }

        [TestMethod()]
        public void SingleOrAromaticBond()
        {
            var input = "  1  3  6  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Unset, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
            Assert.IsInstanceOfType(bond, typeof(QueryBond));
            Assert.AreEqual(ExprType.SingleOrAromatic, ((QueryBond)bond).Expression.GetExprType());
        }

        [TestMethod()]
        public void DoubleOrAromaticBond()
        {
            var input = "  1  3  7  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Unset, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
            Assert.IsInstanceOfType(bond, typeof(QueryBond));
            Assert.AreEqual(ExprType.DoubleOrAromatic, ((QueryBond)bond).Expression.GetExprType());
        }

        [TestMethod()]
        public void AnyBond()
        {
            var input = "  1  3  8  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Unset, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
            Assert.IsInstanceOfType(bond, typeof(QueryBond));
            Assert.AreEqual(ExprType.True, ((QueryBond)bond).Expression.GetExprType());
        }

        [TestMethod()]
        public void UpBond()
        {
            var input = "  1  3  1  1  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.Up, bond.Stereo);
        }

        [TestMethod()]
        public void DownBond()
        {
            var input = "  1  3  1  6  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.Down, bond.Stereo);
        }

        [TestMethod()]
        public void UpOrDownBond()
        {
            var input = "  1  3  1  4  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.UpOrDown, bond.Stereo);
        }

        [TestMethod()]
        public void CisOrTrans()
        {
            var input = "  1  3  2  3  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Double, bond.Order);
            Assert.AreEqual(BondStereo.EOrZ, bond.Stereo);
        }

        [TestMethod()]
        public void CisOrTransByCoordinates()
        {
            var input = "  1  3  2  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(BondOrder.Double, bond.Order);
            Assert.AreEqual(BondStereo.EZByCoordinates, bond.Stereo);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void UpDoubleBond()
        {
            var input = "  1  3  2  1  0  0  0";
            reader.ReaderMode = ChemObjectReaderMode.Strict;
            reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void DownDoubleBond()
        {
            var input = "  1  3  2  1  0  0  0";
            reader.ReaderMode = ChemObjectReaderMode.Strict;
            reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void UpOrDownDoubleBond()
        {
            var input = "  1  3  2  4  0  0  0";
            reader.ReaderMode = ChemObjectReaderMode.Strict;
            reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void CisOrTransSingleBond()
        {
            var input = "  1  3  1  3  0  0  0";
            reader.ReaderMode = ChemObjectReaderMode.Strict;
            reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
        }

        [TestMethod()]
        public void LongLine()
        {
            var input = "  1  3  1  0  0  0  0  0  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(atoms[0], bond.Begin);
            Assert.AreEqual(atoms[2], bond.End);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
        }

        [TestMethod()]
        public void LongLineWithPadding()
        {
            var input = "  1  3  1  0  0  0  0    ";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(atoms[0], bond.Begin);
            Assert.AreEqual(atoms[2], bond.End);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
        }

        [TestMethod()]
        public void ShortLine()
        {
            var input = "  1  3  1  0";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(atoms[0], bond.Begin);
            Assert.AreEqual(atoms[2], bond.End);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
        }

        [TestMethod()]
        public void ShortLineWithPadding()
        {
            var input = "  1  3  1  0       ";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(atoms[0], bond.Begin);
            Assert.AreEqual(atoms[2], bond.End);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
        }

        [TestMethod()]
        public void ShortLineNoStereo()
        {
            var input = "  1  3  1";
            var bond = reader.ReadBondFast(input, builder, atoms, new int[atoms.Length], 1);
            Assert.AreEqual(atoms[0], bond.Begin);
            Assert.AreEqual(atoms[2], bond.End);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
            Assert.IsFalse(bond.IsAromatic);
            Assert.IsFalse(bond.IsSingleOrDouble);
        }
    }
}
