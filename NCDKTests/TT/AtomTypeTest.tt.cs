



/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the <see cref="AtomType"/> 
    /// </summary>
    /// <seealso cref="AtomType"/>
    [TestClass()]
    public class AtomTypeTest : AbstractAtomTypeTest
    {
        public override IChemObject NewChemObject()
        {
            return new AtomType("C");
        }

        [TestMethod()]
        public void TestAtomType_String()
        {
            IAtomType at = new AtomType("C");
            Assert.AreEqual("C", at.Symbol);
        }

        [TestMethod()]
        public void TestAtomType_IElement()
        {
            var element = ChemicalElement.C;
            IAtomType at = new AtomType(element);
            Assert.AreEqual("C", at.Symbol);
        }

        [TestMethod()]
        public void TestAtomType_String_String()
        {
            IAtomType at = new AtomType("C4", "C");
            Assert.AreEqual("C", at.Symbol);
            Assert.AreEqual("C4", at.AtomTypeName);
        }


        [TestMethod()]
        public void TestCompare_AtomTypeName()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.AtomTypeName = "C4";
            at2.AtomTypeName = "C4";
            Assert.IsTrue(at1.Compare(at2));
        }

        [TestMethod()]
        public void TestCompare_DiffAtomTypeName()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.AtomTypeName = "C4";
            at2.AtomTypeName = "C3";
            Assert.IsFalse(at1.Compare(at2));
        }

        [TestMethod()]
        public void TestCompare_BondOrderSum()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.BondOrderSum = 1.5;
            at2.BondOrderSum = 1.5;
            Assert.IsTrue(at1.Compare(at2));
        }

        [TestMethod()]
        public void TestCompare_DiffBondOrderSum()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.BondOrderSum = 1.5;
            at2.BondOrderSum = 2.0;
            Assert.IsFalse(at1.Compare(at2));
        }

    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="AtomType"/> 
    /// </summary>
    /// <seealso cref="AtomType"/>
    [TestClass()]
    public class AtomTypeTest : AbstractAtomTypeTest
    {
        public override IChemObject NewChemObject()
        {
            return new AtomType("C");
        }

        [TestMethod()]
        public void TestAtomType_String()
        {
            IAtomType at = new AtomType("C");
            Assert.AreEqual("C", at.Symbol);
        }

        [TestMethod()]
        public void TestAtomType_IElement()
        {
            var element = ChemicalElement.C;
            IAtomType at = new AtomType(element);
            Assert.AreEqual("C", at.Symbol);
        }

        [TestMethod()]
        public void TestAtomType_String_String()
        {
            IAtomType at = new AtomType("C4", "C");
            Assert.AreEqual("C", at.Symbol);
            Assert.AreEqual("C4", at.AtomTypeName);
        }


        [TestMethod()]
        public void TestCompare_AtomTypeName()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.AtomTypeName = "C4";
            at2.AtomTypeName = "C4";
            Assert.IsTrue(at1.Compare(at2));
        }

        [TestMethod()]
        public void TestCompare_DiffAtomTypeName()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.AtomTypeName = "C4";
            at2.AtomTypeName = "C3";
            Assert.IsFalse(at1.Compare(at2));
        }

        [TestMethod()]
        public void TestCompare_BondOrderSum()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.BondOrderSum = 1.5;
            at2.BondOrderSum = 1.5;
            Assert.IsTrue(at1.Compare(at2));
        }

        [TestMethod()]
        public void TestCompare_DiffBondOrderSum()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.BondOrderSum = 1.5;
            at2.BondOrderSum = 2.0;
            Assert.IsFalse(at1.Compare(at2));
        }

 
                // Overwrite default methods: no notifications are expected!

        [TestMethod()]
        public override void TestNotifyChanged()
        {
            ChemObjectTestHelper.TestNotifyChanged(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_SetFlag()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetFlag(NewChemObject());
        }

        [TestMethod()]
        public void TestNotifyChanged_SetFlags()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetFlags(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_IChemObjectChangeEvent()
        {
            ChemObjectTestHelper.TestNotifyChanged_IChemObjectChangeEvent(NewChemObject());
        }

        [TestMethod()]
        public override void TestStateChanged_IChemObjectChangeEvent()
        {
            ChemObjectTestHelper.TestStateChanged_IChemObjectChangeEvent(NewChemObject());
        }

        [TestMethod()]
        public override void TestClone_ChemObjectListeners()
        {
            ChemObjectTestHelper.TestClone_ChemObjectListeners(NewChemObject());
        }

        [TestMethod()]
        public override void TestAddListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestAddListener_IChemObjectListener(NewChemObject());
        }

        [TestMethod()]
        public override void TestGetListenerCount()
        {
            ChemObjectTestHelper.TestGetListenerCount(NewChemObject());
        }

        [TestMethod()]
        public override void TestRemoveListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestRemoveListener_IChemObjectListener(NewChemObject());
        }

        [TestMethod()]
        public override void TestSetNotification_true()
        {
            ChemObjectTestHelper.TestSetNotification_true(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_SetProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetProperty(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_RemoveProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_RemoveProperty(NewChemObject());
        }

    }
}
