/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Exceptions
{
    /// <summary>
    /// Checks the functionality of the NoSuchAtomTypeException class.
    ///
    // @cdk.module test-core
    ///
    /// <seealso cref="NoSuchAtomTypeException"/>
    /// </summary>
    [TestClass()]
    public class NoSuchAtomTypeExceptionTest : CDKTestCase
    {

        [TestMethod()]
        public void TestNoSuchAtomTypeException_String()
        {
            string EXPLANATION = "Buckybull is not an atom type!";
            NoSuchAtomTypeException exception = new NoSuchAtomTypeException(EXPLANATION);
            Assert.IsNotNull(exception);
            Assert.AreEqual(EXPLANATION, exception.Message);
        }
    }
}
