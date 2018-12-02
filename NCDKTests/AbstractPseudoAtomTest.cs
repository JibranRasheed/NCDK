/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Numerics;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IPseudoAtom"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    public abstract class AbstractPseudoAtomTest : AbstractAtomTest
    {
        [TestMethod()]
        public virtual void TestGetLabel()
        {
            var label = "Arg255";
            var a = (IPseudoAtom)NewChemObject();
            a.Label = label;
            Assert.AreEqual(label, a.Label);
        }

        [TestMethod()]
        public virtual void TestSetLabel_String()
        {
            var label = "Arg255";
            var atom = (IPseudoAtom)NewChemObject();
            atom.Label = label;
            var label2 = "His66";
            atom.Label = label2;
            Assert.AreEqual(label2, atom.Label);
        }

        [TestMethod()]
        public override void TestGetFormalCharge()
        {
            var atom = (IPseudoAtom)NewChemObject();
            Assert.AreEqual(0, atom.FormalCharge.Value);
        }

        [TestMethod()]
        public override void TestSetFormalCharge_Integer()
        {
            var atom = (IPseudoAtom)NewChemObject();
            atom.FormalCharge = +5;
            Assert.AreEqual(+5, atom.FormalCharge.Value);
        }

        [TestMethod()]
        public virtual void TestSetHydrogenCount_Integer()
        {
            var atom = (IPseudoAtom)NewChemObject();
            atom.ImplicitHydrogenCount = +5;
            Assert.AreEqual(5, atom.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public override void TestSetChargeDouble()
        {
            var atom = (IPseudoAtom)NewChemObject();
            atom.Charge = 0.78;
            Assert.AreEqual(0.78, atom.Charge.Value, 0.001);
        }

        [TestMethod()]
        public override void TestSetExactMass_Double()
        {
            var atom = (IPseudoAtom)NewChemObject();
            atom.ExactMass = 12.001;
            Assert.AreEqual(12.001, atom.ExactMass.Value, 0.001);
        }

        [TestMethod()]
        public override void TestSetStereoParityInteger()
        {
            var atom = (IPseudoAtom)NewChemObject();
            atom.StereoParity = -1;
            Assert.AreEqual(0, atom.StereoParity);
        }

        [TestMethod()]
        public virtual void TestPseudoAtom_IAtom()
        {
            var obj = NewChemObject();
            var atom = obj.Builder.NewAtom("C");
            var fract = new Vector3(0.5, 0.5, 0.5);
            var threeD = new Vector3(0.5, 0.5, 0.5);
            var twoD = new Vector2(0.5, 0.5);
            atom.FractionalPoint3D = fract;
            atom.Point3D = threeD;
            atom.Point2D = twoD;

            var a = obj.Builder.NewPseudoAtom(atom);
            AssertAreEqual(fract, a.FractionalPoint3D.Value, 0.0001);
            AssertAreEqual(threeD, a.Point3D.Value, 0.0001);
            AssertAreEqual(twoD, a.Point2D.Value, 0.0001);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public override void TestClone()
        {
            var atom = (IPseudoAtom)NewChemObject();
            object clone = atom.Clone();
            Assert.IsTrue(clone is IPseudoAtom);
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]
        public override void TestToString()
        {
            var atom = (IPseudoAtom)NewChemObject();
            var description = atom.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        /// <summary>
        /// Test for bug #1778479 "MDLWriter writes empty PseudoAtom label string".
        /// We decided to let the pseudo atoms have a default label of '*'.
        /// </summary>
        // @author Andreas Schueller <a.schueller@chemie.uni-frankfurt.de>
        // @cdk.bug 1778479
        [TestMethod()]
        public virtual void TestBug1778479DefaultLabel()
        {
            var atom = (IPseudoAtom)NewChemObject();
            Assert.IsNotNull(atom.Label, "Test for PseudoAtom's default label");
            Assert.AreEqual("*", atom.Label, "Test for PseudoAtom's default label");
        }

        /// <summary>
        /// Overwrite the method in <see cref="AbstractAtomTest"/> to always
        /// expect zero hydrogen counts.
        /// </summary>
        [TestMethod()]
        public override void TestCloneHydrogenCount()
        {
            var atom = (IAtom)NewChemObject();
            atom.ImplicitHydrogenCount = 3;
            var clone = (IAtom)atom.Clone();

            // test cloning
            atom.ImplicitHydrogenCount = 4;
            Assert.AreEqual(3, clone.ImplicitHydrogenCount.Value);
        }

        /// <summary>
        /// Overwrite the method in <see cref="AbstractAtomTest"/> to always
        /// expect zero hydrogen counts.
        /// </summary>
        [TestMethod()]
        public virtual void TestGetHydrogenCount()
        {
            // expect zero by definition
            var a = (IAtom)NewChemObject();
            Assert.IsNull(a.ImplicitHydrogenCount);
            a.ImplicitHydrogenCount = 5;
            Assert.AreEqual(5, a.ImplicitHydrogenCount.Value);
            a.ImplicitHydrogenCount = null;
            Assert.IsNull(a.ImplicitHydrogenCount);
        }

        /// <summary>
        /// Overwrite the method in <see cref="AbstractAtomTypeTest"/> to always
        /// expect zero stereo parity.
        /// </summary>
        [TestMethod()]
        public override void TestCloneStereoParity()
        {
            var atom = (IAtom)NewChemObject();
            atom.StereoParity = 3;
            var clone = (IAtom)atom.Clone();

            // test cloning
            atom.StereoParity = 4;
            Assert.AreEqual(0, clone.StereoParity);
        }

        [TestMethod()]
        public virtual void TestPseudoAtomCharges()
        {
            var label = "charged patom";
            var a = (IPseudoAtom)NewChemObject();
            a.Label = label;
            a.FormalCharge = -1;
            Assert.IsNotNull(a);
            Assert.IsNotNull(a.FormalCharge);
            Assert.AreEqual(-1, a.FormalCharge.Value);
        }
    }
}