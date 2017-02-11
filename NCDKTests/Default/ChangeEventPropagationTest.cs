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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Default
{
    /**
     * Checks the propagation of ChangeEvents through a
     * nested set of objects.
     *
     * @cdk.module test-data
     *
     * @see org.openscience.cdk.ChemFile
     */
	[TestClass()]
    public class ChangeEventPropagationTest : CDKTestCase
    {
        [TestMethod()]
        public virtual void TestPropagation()
        {
            ChemFile cf = new ChemFile();
            ChemSequence cs = new ChemSequence();
            ChemModel cm = new ChemModel();
            IAtomContainerSet<IAtomContainer> som = new AtomContainerSet<IAtomContainer>();
            IAtomContainer mol = new AtomContainer();
            Atom a1 = new Atom("C");
            Atom a2 = new Atom("C");
            Bond b1 = new Bond(a1, a2);
            mol.Add(a1);
            mol.Add(a2);
            mol.Add(b1);
            som.Add(mol);
            cm.MoleculeSet = som;
            cs.Add(cm);
            cf.Add(cs);
            TestListener ts = new TestListener();
            cf.Listeners.Add(ts);
            a2.Symbol = "N";
            Assert.IsInstanceOfType(ts.changedObject, typeof(Atom));
            Assert.AreEqual("N", ((Atom)ts.changedObject).Symbol);
        }

        class TestListener : IChemObjectListener
        {
            public ChemObject changedObject { get; set; }

            public virtual void OnStateChanged(ChemObjectChangeEventArgs evt)
            {
                changedObject = (ChemObject)evt.Source;
            }
        }
    }
}

