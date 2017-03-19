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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the BioPolymer class.
    ///
    // @cdk.module test-data
    ///
    // @see org.openscience.cdk.BioPolymer
    /// </summary>
    [TestClass()]
    public class BioPolymerTest : AbstractBioPolymerTest
    {
        public override IChemObject NewChemObject()
        {
            return new BioPolymer();
        }

    [TestMethod()]
    public virtual void TestBioPolymer() {
        IBioPolymer oBioPolymer = new BioPolymer();
        Assert.IsNotNull(oBioPolymer);
        Assert.AreEqual(oBioPolymer.GetMonomerMap().Count(), 0);

        IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
        oStrand1.StrandName = "A";
        IStrand oStrand2 = oBioPolymer.Builder.CreateStrand();
        oStrand2.StrandName = "B";
        IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
        oMono1.MonomerName = "TRP279";
        IMonomer oMono2 = oBioPolymer.Builder.CreateMonomer();
        oMono2.MonomerName = "HOH";
        IMonomer oMono3 = oBioPolymer.Builder.CreateMonomer();
        oMono3.MonomerName = "GLYA16";
        IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
        IAtom oAtom2 = oBioPolymer.Builder.CreateAtom("C2");
        IAtom oAtom3 = oBioPolymer.Builder.CreateAtom("C3");
        IAtom oAtom4 = oBioPolymer.Builder.CreateAtom("C4");
        IAtom oAtom5 = oBioPolymer.Builder.CreateAtom("C5");

        oBioPolymer.Atoms.Add(oAtom1);
        oBioPolymer.AddAtom(oAtom2, oStrand1);
        oBioPolymer.AddAtom(oAtom3, oMono1, oStrand1);
        oBioPolymer.AddAtom(oAtom4, oMono2, oStrand2);
        oBioPolymer.AddAtom(oAtom5, oMono3, oStrand2);
        Assert.IsNotNull(oBioPolymer.Atoms[0]);
        Assert.IsNotNull(oBioPolymer.Atoms[1]);
        Assert.IsNotNull(oBioPolymer.Atoms[2]);
        Assert.IsNotNull(oBioPolymer.Atoms[3]);
        Assert.IsNotNull(oBioPolymer.Atoms[4]);
        Assert.AreEqual(oAtom1, oBioPolymer.Atoms[0]);
        Assert.AreEqual(oAtom2, oBioPolymer.Atoms[1]);
        Assert.AreEqual(oAtom3, oBioPolymer.Atoms[2]);
        Assert.AreEqual(oAtom4, oBioPolymer.Atoms[3]);
        Assert.AreEqual(oAtom5, oBioPolymer.Atoms[4]);

        Assert.IsNull(oBioPolymer.GetMonomer("0815", "A"));
        Assert.IsNull(oBioPolymer.GetMonomer("0815", "B"));
        Assert.IsNull(oBioPolymer.GetMonomer("0815", ""));
        Assert.IsNull(oBioPolymer.GetStrand(""));
        Assert.IsNotNull(oBioPolymer.GetMonomer("TRP279", "A"));
        Assert.AreEqual(oMono1, oBioPolymer.GetMonomer("TRP279", "A"));
        Assert.AreEqual(oBioPolymer.GetMonomer("TRP279", "A").Atoms.Count, 1);
        Assert.IsNotNull(oBioPolymer.GetMonomer("HOH", "B"));
        Assert.AreEqual(oMono2, oBioPolymer.GetMonomer("HOH", "B"));
        Assert.AreEqual(oBioPolymer.GetMonomer("HOH", "B").Atoms.Count, 1);
        Assert.AreEqual(oBioPolymer.GetStrand("B").Atoms.Count, 2);
        Assert.AreEqual(oBioPolymer.GetStrand("B").GetMonomerMap().Count(), 2);
        Assert.IsNull(oBioPolymer.GetStrand("C"));
        Assert.IsNotNull(oBioPolymer.GetStrand("B"));
    }

}
    }
