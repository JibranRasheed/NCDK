﻿/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using System.Linq;

namespace NCDK.Config
{
    /// <summary>
    /// Checks the functionality of the <see cref="CDKBasedAtomTypeConfigurator"/>.
    /// </summary>
    // @cdk.module test-core
    [TestClass()]
    public class CDKBasedAtomTypeConfiguratorTest
    {
        [TestMethod()]
        public virtual void TestCDKBasedAtomTypeConfigurator()
        {
            var configurator = new CDKBasedAtomTypeConfigurator();
            Assert.IsNotNull(configurator);
        }

        [TestMethod()]
        public virtual void TestReadAtomTypes_IChemObjectBuilder()
        {
            var configFile = "NCDK.Config.Data.structgen_atomtypes.xml";
            var ins = ResourceLoader.GetAsStream(typeof(CDKBasedAtomTypeConfigurator), configFile);
            var configurator = new CDKBasedAtomTypeConfigurator();
            configurator.SetStream(ins);
            var atomTypes = configurator.ReadAtomTypes();
            Assert.AreNotSame(0, atomTypes.Count());
        }

        [TestMethod()]
        public void TestSetInputStream_InputStream()
        {
            TestReadAtomTypes_IChemObjectBuilder();
        }
    }
}