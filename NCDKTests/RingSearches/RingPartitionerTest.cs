/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
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
using NCDK.Graphs;
using NCDK.Templates;
using System.Collections.Generic;

namespace NCDK.RingSearches
{
    /**
     * This class tests the RingPartitioner class.
     *
     * @cdk.module test-standard
     *
     * @author         kaihartmann
     * @cdk.created    2005-05-24
     */
    [TestClass()]
    public class RingPartitionerTest : CDKTestCase
    {
        public RingPartitionerTest()
                : base()
        {
        }

        [TestMethod()]
        public void TestConvertToAtomContainer_IRingSet()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeAlphaPinene();

            IRingSet ringSet = Cycles.SSSR(molecule).ToRingSet();
            IAtomContainer ac = RingPartitioner.ConvertToAtomContainer(ringSet);
            Assert.AreEqual(7, ac.Atoms.Count);
            Assert.AreEqual(8, ac.Bonds.Count);
        }

        [TestMethod()]
        public void TestPartitionIntoRings()
        {
            IAtomContainer azulene = TestMoleculeFactory.MakeAzulene();
            IRingSet ringSet = Cycles.SSSR(azulene).ToRingSet();
            IList<IRingSet> list = RingPartitioner.PartitionRings(ringSet);
            Assert.AreEqual(1, list.Count);

            IAtomContainer biphenyl = TestMoleculeFactory.MakeBiphenyl();
            ringSet = Cycles.SSSR(biphenyl).ToRingSet();
            list = RingPartitioner.PartitionRings(ringSet);
            Assert.AreEqual(2, list.Count);

            IAtomContainer spiro = TestMoleculeFactory.MakeSpiroRings();
            ringSet = Cycles.SSSR(spiro).ToRingSet();
            list = RingPartitioner.PartitionRings(ringSet);
            Assert.AreEqual(1, list.Count);

        }
    }
}
