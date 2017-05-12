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

namespace NCDK.QSAR.Result
{
    // @cdk.module test-standard
    public class DoubleResultTest : CDKTestCase
    {
        public DoubleResultTest()
            : base()
        { }

        [TestMethod()]
        public void TestDoubleResult_Double()
        {
            DoubleResult result = new DoubleResult(5.0);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestToString()
        {
            DoubleResult result = new DoubleResult(5.0);
            Assert.AreEqual("5.0", result.ToString());
        }

        [TestMethod()]
        public void TestDoubleValue()
        {
            DoubleResult result = new DoubleResult(5);
            Assert.AreEqual(5.0, result.Value, 0.000001);
        }

        [TestMethod()]
        public void TestLength()
        {
            DoubleResult result = new DoubleResult(5);
            Assert.AreEqual(1, result.Length);
        }
    }
}
