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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class VdWRadiusDescriptorTest : AtomicDescriptorTest<VdWRadiusDescriptor>
    {
        protected override VdWRadiusDescriptor Descriptor { get; } = new VdWRadiusDescriptor();
            
        [TestMethod()]
        public void TestVdWRadiusDescriptor()
        {
            double[] testResult = { 1.7 };
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("NCCN(C)(C)");
            var descriptor = CreateDescriptor();
            var retval = descriptor.Calculate(mol.Atoms[1]).Value;

            Assert.AreEqual(testResult[0], retval, 0.01);
        }
    }
}
