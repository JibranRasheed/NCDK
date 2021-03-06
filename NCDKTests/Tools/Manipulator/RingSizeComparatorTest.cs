/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
    [TestClass()]
    public class RingSizeComparatorTest : CDKTestCase
    {
        public RingSizeComparatorTest()
            : base()
        { }

        [TestMethod()]
        public void TestRingSizeComparator_int()
        {
            RingSizeComparator comp = new RingSizeComparator(SortMode.LargeFirst);
            Assert.IsNotNull(comp);
        }

        [TestMethod()]
        public void TestCompare()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IRing cycloPentane = builder.NewRing(5, "C");
            IRing cycloHexane = builder.NewRing(6, "C");
            IRing cycloHexane2 = builder.NewRing(6, "C");

            RingSizeComparator ringSizeComparator = new RingSizeComparator(SortMode.LargeFirst);
            Assert.IsTrue(ringSizeComparator.Compare(cycloHexane, cycloPentane) == -1);
            Assert.IsTrue(ringSizeComparator.Compare(cycloPentane, cycloHexane) == 1);
            Assert.IsTrue(ringSizeComparator.Compare(cycloHexane, cycloHexane2) == 0);

            ringSizeComparator = new RingSizeComparator(SortMode.SmallFirst);
            Assert.IsTrue(ringSizeComparator.Compare(cycloHexane, cycloPentane) == 1);
            Assert.IsTrue(ringSizeComparator.Compare(cycloPentane, cycloHexane) == -1);
            Assert.IsTrue(ringSizeComparator.Compare(cycloHexane, cycloHexane2) == 0);
        }
    }
}
