/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@slists.sourceforge.net
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

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading CIF files using a test file with the <see cref="CIFReader"/>.
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public class CIFReaderTest : ChemObjectIOTest
    {
        protected IChemObjectReader ChemObjectReaderToTest => (IChemObjectReader)ChemObjectIOToTest;
        static readonly CIFReader simpleReader = new CIFReader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        /// <summary>
        /// Ensure a CIF file from the crystallography open database can be read.
        /// Example input <see href="http://www.crystallography.net/1100784.cif">1100784</see>.
        /// </summary>
        [TestMethod()]
        public void Cod1100784()
        {
            var ins = ResourceLoader.GetAsStream(GetType(), "1100784.cif");
            CIFReader cifReader = new CIFReader(ins);
            //        try {
            IChemFile chemFile = cifReader.Read(new ChemFile());
            Assert.AreEqual(1, chemFile.Count);
            Assert.AreEqual(1, chemFile[0].Count);
            Assert.IsNotNull(chemFile[0][0].Crystal);
            //        } finally {
            cifReader.Close();
            //        }
        }
    }
}