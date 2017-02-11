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
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;

namespace NCDK
{
    /**
     * Checks the functionality of the ChemObjectChangeEvent class.
     *
     * @cdk.module test-data
     *
     * @see org.openscience.cdk.event.ChemObjectChangeEvent
     */
    [TestClass()]
    public class ChemObjectChangeEventTest : CDKTestCase
    {
        [TestMethod()]
        public void TestChemObjectChangeEvent_Object()
        {
            var evt = new ChemObjectChangeEventArgs(new Atom());
            Assert.IsNotNull(evt);
        }
    }
}
