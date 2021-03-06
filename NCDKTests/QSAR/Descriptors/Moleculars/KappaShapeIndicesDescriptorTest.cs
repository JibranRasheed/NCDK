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
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class KappaShapeIndicesDescriptorTest : MolecularDescriptorTest<KappaShapeIndicesDescriptor>
    {
        [TestMethod()]
        public void TestKappaShapeIndicesDescriptor()
        {
            double[] testResult = { 5, 2.25, 4 };
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=C(O)CC");
            AtomContainerManipulator.RemoveHydrogens(mol);
            var retval = CreateDescriptor().Calculate(mol).Values;
            Assert.AreEqual(testResult[2], retval[2], 0.0001);
        }
    }
}
