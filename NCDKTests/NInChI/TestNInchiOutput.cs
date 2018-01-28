/*
 * Copyright 2006-2011 Sam Adams <sea36 at users.sourceforge.net>
 *
 * This file is part of JNI-InChI.
 *
 * JNI-InChI is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * JNI-InChI is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with JNI-InChI.  If not, see <http://www.gnu.org/licenses/>.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Graphs.InChI
{
    [TestClass()]
    public class TestNInchiOutput
    {
        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiOutput.ReturnStatus'
        /// </summary>
        [TestMethod()]
        public void TestGetReturnStatus()
        {
            NInchiOutput output = new NInchiOutput(InChIReturnCode.Ok, null, null, null, null);
            Assert.AreEqual(InChIReturnCode.Ok, output.ReturnStatus);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiOutput.Inchi'
        /// </summary>
        [TestMethod()]
        public void TestGetInchi()
        {
            NInchiOutput output = new NInchiOutput(InChIReturnCode.Unknown, "Inchi=1/C6H6/c1-2-4-6-5-3-1/h1-6H", null, null, null);
            Assert.AreEqual("Inchi=1/C6H6/c1-2-4-6-5-3-1/h1-6H", output.InChI);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiOutput.AuxInfo'
        /// </summary>
        [TestMethod()]
        public void TestGetAuxInfo()
        {
            NInchiOutput output = new NInchiOutput(InChIReturnCode.Unknown, null, "AuxInfo=1/0/N:1,2,6,3,5,4/E:(1,2,3,4,5,6)/rA:6nCCCCCC/rB:d1;s2;d3;s4;s1d5;/rC:-.7145,.4125,0;-.7145,-.4125,0;0,-.825,0;.7145,-.4125,0;.7145,.4125,0;0,.825,0;", null, null);
            Assert.AreEqual("AuxInfo=1/0/N:1,2,6,3,5,4/E:(1,2,3,4,5,6)/rA:6nCCCCCC/rB:d1;s2;d3;s4;s1d5;/rC:-.7145,.4125,0;-.7145,-.4125,0;0,-.825,0;.7145,-.4125,0;.7145,.4125,0;0,.825,0;", output.AuxInfo);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiOutput.Message'
        /// </summary>
        [TestMethod()]
        public void TestGetMessage()
        {
            NInchiOutput output = new NInchiOutput(InChIReturnCode.Unknown, null, null, "Test message", null);
            Assert.AreEqual("Test message", output.Message);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiOutput.Log'
        /// </summary>
        [TestMethod()]
        public void TestGetLog()
        {
            NInchiOutput output = new NInchiOutput(InChIReturnCode.Unknown, null, null, null, "Test log");
            Assert.AreEqual("Test log", output.Log);
        }
    }
}