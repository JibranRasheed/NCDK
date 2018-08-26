



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
using System.Linq;

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the <see cref="IStrand"/>. 
    /// </summary>
    [TestClass()]
    public class StrandTest : AbstractStrandTest
    {
        public override IChemObject NewChemObject()
        {
            return new Strand();
        }

        [TestMethod()]
        public void TestStrand()
        {
            IStrand oStrand = new Strand();
            Assert.IsNotNull(oStrand);
            Assert.AreEqual(oStrand.GetMonomerMap().Count(), 0);

            IMonomer oMono1 = oStrand.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oStrand.Builder.NewMonomer();
            oMono2.MonomerName = "HOH";
            IMonomer oMono3 = oStrand.Builder.NewMonomer();
            oMono3.MonomerName = "GLYA16";
            IAtom oAtom1 = oStrand.Builder.NewAtom("C");
            IAtom oAtom2 = oStrand.Builder.NewAtom("C");
            IAtom oAtom3 = oStrand.Builder.NewAtom("C");
            IAtom oAtom4 = oStrand.Builder.NewAtom("C");
            IAtom oAtom5 = oStrand.Builder.NewAtom("C");

            oStrand.AddAtom(oAtom1);
            oStrand.AddAtom(oAtom2);
            oStrand.AddAtom(oAtom3, oMono1);
            oStrand.AddAtom(oAtom4, oMono2);
            oStrand.AddAtom(oAtom5, oMono3);
            Assert.IsNotNull(oStrand.Atoms[0]);
            Assert.IsNotNull(oStrand.Atoms[1]);
            Assert.IsNotNull(oStrand.Atoms[2]);
            Assert.IsNotNull(oStrand.Atoms[3]);
            Assert.IsNotNull(oStrand.Atoms[4]);
            Assert.AreEqual(oAtom1, oStrand.Atoms[0]);
            Assert.AreEqual(oAtom2, oStrand.Atoms[1]);
            Assert.AreEqual(oAtom3, oStrand.Atoms[2]);
            Assert.AreEqual(oAtom4, oStrand.Atoms[3]);
            Assert.AreEqual(oAtom5, oStrand.Atoms[4]);

            Assert.IsNull(oStrand.GetMonomer("0815"));
            Assert.IsNotNull(oStrand.GetMonomer(""));
            Assert.IsNotNull(oStrand.GetMonomer("TRP279"));
            Assert.AreEqual(oMono1, oStrand.GetMonomer("TRP279"));
            Assert.AreEqual(oStrand.GetMonomer("TRP279").Atoms.Count, 1);
            Assert.IsNotNull(oStrand.GetMonomer("HOH"));
            Assert.AreEqual(oMono2, oStrand.GetMonomer("HOH"));
            Assert.AreEqual(oStrand.GetMonomer("HOH").Atoms.Count, 1);
            Assert.AreEqual(oStrand.GetMonomer("").Atoms.Count, 2);
            Assert.AreEqual(oStrand.Atoms.Count, 5);
            Assert.AreEqual(oStrand.GetMonomerMap().Count(), 3);
        }
        
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="IStrand"/>. 
    /// </summary>
    [TestClass()]
    public class StrandTest : AbstractStrandTest
    {
        public override IChemObject NewChemObject()
        {
            return new Strand();
        }

        [TestMethod()]
        public void TestStrand()
        {
            IStrand oStrand = new Strand();
            Assert.IsNotNull(oStrand);
            Assert.AreEqual(oStrand.GetMonomerMap().Count(), 0);

            IMonomer oMono1 = oStrand.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oStrand.Builder.NewMonomer();
            oMono2.MonomerName = "HOH";
            IMonomer oMono3 = oStrand.Builder.NewMonomer();
            oMono3.MonomerName = "GLYA16";
            IAtom oAtom1 = oStrand.Builder.NewAtom("C");
            IAtom oAtom2 = oStrand.Builder.NewAtom("C");
            IAtom oAtom3 = oStrand.Builder.NewAtom("C");
            IAtom oAtom4 = oStrand.Builder.NewAtom("C");
            IAtom oAtom5 = oStrand.Builder.NewAtom("C");

            oStrand.AddAtom(oAtom1);
            oStrand.AddAtom(oAtom2);
            oStrand.AddAtom(oAtom3, oMono1);
            oStrand.AddAtom(oAtom4, oMono2);
            oStrand.AddAtom(oAtom5, oMono3);
            Assert.IsNotNull(oStrand.Atoms[0]);
            Assert.IsNotNull(oStrand.Atoms[1]);
            Assert.IsNotNull(oStrand.Atoms[2]);
            Assert.IsNotNull(oStrand.Atoms[3]);
            Assert.IsNotNull(oStrand.Atoms[4]);
            Assert.AreEqual(oAtom1, oStrand.Atoms[0]);
            Assert.AreEqual(oAtom2, oStrand.Atoms[1]);
            Assert.AreEqual(oAtom3, oStrand.Atoms[2]);
            Assert.AreEqual(oAtom4, oStrand.Atoms[3]);
            Assert.AreEqual(oAtom5, oStrand.Atoms[4]);

            Assert.IsNull(oStrand.GetMonomer("0815"));
            Assert.IsNotNull(oStrand.GetMonomer(""));
            Assert.IsNotNull(oStrand.GetMonomer("TRP279"));
            Assert.AreEqual(oMono1, oStrand.GetMonomer("TRP279"));
            Assert.AreEqual(oStrand.GetMonomer("TRP279").Atoms.Count, 1);
            Assert.IsNotNull(oStrand.GetMonomer("HOH"));
            Assert.AreEqual(oMono2, oStrand.GetMonomer("HOH"));
            Assert.AreEqual(oStrand.GetMonomer("HOH").Atoms.Count, 1);
            Assert.AreEqual(oStrand.GetMonomer("").Atoms.Count, 2);
            Assert.AreEqual(oStrand.Atoms.Count, 5);
            Assert.AreEqual(oStrand.GetMonomerMap().Count(), 3);
        }
        
 
                [TestMethod()]
        public override void TestSetAtoms_RemoveListener()
        {
            ChemObjectTestHelper.TestSetAtoms_RemoveListener(NewChemObject());
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

