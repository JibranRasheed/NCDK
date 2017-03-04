/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Config;
using NCDK.Default;
using System;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class AtomTypeManipulatorTest : CDKTestCase
    {

        public AtomTypeManipulatorTest()
            : base()
        { }

        [TestMethod()]
        public void TestConfigure_IAtom_IAtomType()
        {
            IAtom atom = new Atom(Elements.Carbon.ToIElement());
            IAtomType atomType = new AtomType(Elements.Carbon.ToIElement());
            atomType.IsHydrogenBondAcceptor = true;
            AtomTypeManipulator.Configure(atom, atomType);
            Assert.AreEqual(atomType.IsHydrogenBondAcceptor, atom.IsHydrogenBondAcceptor);
        }

        [TestMethod()]
        public void TestConfigureUnSetProperties_DontOverwriterSetProperties()
        {
            IAtom atom = new Atom(Elements.Carbon.ToIElement());
            atom.ExactMass = 13.0;
            IAtomType atomType = new AtomType(Elements.Carbon.ToIElement());
            atomType.ExactMass = 12.0;
            AtomTypeManipulator.ConfigureUnsetProperties(atom, atomType);
            Assert.AreEqual(13.0, atom.ExactMass.Value, 0.1);
        }

        [TestMethod()]
        public void TestConfigureUnSetProperties()
        {
            IAtom atom = new Atom(Elements.Carbon.ToIElement());
            IAtomType atomType = new AtomType(Elements.Carbon.ToIElement());
            atomType.ExactMass = 12.0;
            AtomTypeManipulator.ConfigureUnsetProperties(atom, atomType);
            Assert.AreEqual(12.0, atom.ExactMass.Value, 0.1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConfigure_IAtom_Null()
        {
            IAtom atom = new Atom(Elements.Carbon.ToIElement());
            IAtomType atomType = null;
            AtomTypeManipulator.Configure(atom, atomType);
        }

        [TestMethod()]
        public void UnknownAtomTypeDoesNotModifyProperties()
        {
            IAtom atom = new Atom(Elements.Carbon.ToIElement());
            IAtomType atomType = new AtomType(Elements.Unknown.ToIElement());
            atomType.AtomTypeName = "X";
            AtomTypeManipulator.Configure(atom, atomType);
            Assert.AreEqual("C", atom.Symbol);
            Assert.AreEqual(6, atom.AtomicNumber);
        }
    }
}
