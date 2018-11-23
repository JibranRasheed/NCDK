/* Copyright (C) 2004-2018  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.Silent;
using NCDK.Templates;

namespace NCDK.Smiles.SMARTS
{
    // @cdk.module  test-smarts
    [TestClass()]
    public class SMARTSTest : CDKTestCase
    {
        private UniversalIsomorphismTester uiTester = new UniversalIsomorphismTester();

        [TestMethod()]
        public void TestStrictSMARTS()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;

            var sp = new SmilesParser(builder);
            var atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C"); // acetic acid anhydride
            var query = new QueryAtomContainer(builder);
            var atom1 = new SymbolQueryAtom(builder) { Symbol = "N" };
            var atom2 = new SymbolQueryAtom(builder) { Symbol = "C" };
            query.Atoms.Add(atom1);
            query.Atoms.Add(atom2);
            query.Bonds.Add(new NCDK.Isomorphisms.Matchers.OrderQueryBond(atom1, atom2, BondOrder.Double, builder));

            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestSMARTS()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            var sp = new SmilesParser(builder);
            var atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C"); // acetic acid anhydride
            var query = new QueryAtomContainer(builder);
            var atom1 = new NCDK.Isomorphisms.Matchers.SMARTS.AnyAtom(builder);
            var atom2 = new SymbolQueryAtom(builder) { Symbol = "C" };
            query.Atoms.Add(atom1);
            query.Atoms.Add(atom2);
            query.Bonds.Add(new NCDK.Isomorphisms.Matchers.OrderQueryBond(atom1, atom2, BondOrder.Double, builder));

            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));
        }

        private IAtomContainer CreateEthane()
        {
            IAtomContainer container = new AtomContainer(); // SMILES "CC"
            IAtom carbon = new Atom("C");
            IAtom carbon2 = carbon.Builder.NewAtom("C");
            carbon.ImplicitHydrogenCount = 3;
            carbon2.ImplicitHydrogenCount = 3;
            container.Atoms.Add(carbon);
            container.Atoms.Add(carbon2);
            container.Bonds.Add(carbon.Builder.NewBond(carbon, carbon2, BondOrder.Single));
            return container;
        }

        [TestMethod()]
        public void TestImplicitHCountAtom()
        {
            IAtomContainer container = CreateEthane();

            IChemObjectBuilder builder = ChemObjectBuilder.Instance;

            QueryAtomContainer query1 = new QueryAtomContainer(builder); // SMARTS [h3][h3]
            var atom1 = new ImplicitHCountAtom(3, builder);
            var atom2 = new ImplicitHCountAtom(3, builder);
            query1.Atoms.Add(atom1);
            query1.Atoms.Add(atom2);
            query1.Bonds.Add(new NCDK.Isomorphisms.Matchers.OrderQueryBond(atom1, atom2, BondOrder.Single, builder));
            Assert.IsTrue(uiTester.IsSubgraph(container, query1));
        }

        [TestMethod()]
        public void TestImplicitHCountAtom2()
        {
            IAtomContainer container = CreateEthane();

            IChemObjectBuilder builder = ChemObjectBuilder.Instance;

            QueryAtomContainer query1 = new QueryAtomContainer(builder); // SMARTS [h3][h2]
            var atom1 = new ImplicitHCountAtom(3, builder);
            var atom2 = new ImplicitHCountAtom(2, builder);
            query1.Atoms.Add(atom1);
            query1.Atoms.Add(atom2);
            query1.Bonds.Add(new NCDK.Isomorphisms.Matchers.OrderQueryBond(atom1, atom2, BondOrder.Single, builder));
            Assert.IsFalse(uiTester.IsSubgraph(container, query1));
        }

        [TestMethod()]
        public void TestMatchInherited()
        {
            try
            {
                IChemObjectBuilder builder = ChemObjectBuilder.Instance;

                SymbolQueryAtom c1 = new SymbolQueryAtom(new Atom("C"));
                SymbolAndChargeQueryAtom c2 = new SymbolAndChargeQueryAtom(new Atom("C"));

                IAtomContainer c = TestMoleculeFactory.MakeAlkane(2);

                QueryAtomContainer query1 = new QueryAtomContainer(builder);
                query1.Atoms.Add(c1);
                query1.Atoms.Add(c2);
                query1.Bonds.Add(new NCDK.Isomorphisms.Matchers.OrderQueryBond(c1, c2, BondOrder.Single, builder));
                Assert.IsTrue(uiTester.IsSubgraph(c, query1));

                var query = new QueryAtomContainer(builder);
                query.Atoms.Add(c1);
                query.Atoms.Add(c2);
                query.Bonds.Add(new AnyOrderQueryBond(c1, c2, BondOrder.Single, builder));
                Assert.IsTrue(uiTester.IsSubgraph(c, query));
            }
            catch (CDKException exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [TestMethod()]
        public void TestUnspecifiedIsotope()
        {
            IAtom aexpr = Smiles.SMARTS.Parser.SMARTSParser.Parse("[!0]", CDK.Builder).Atoms[0];
            Assert.IsInstanceOfType(aexpr, typeof(LogicalOperatorAtom));
            Assert.AreEqual("not", ((LogicalOperatorAtom)aexpr).Operator);
            IQueryAtom subexpr = ((LogicalOperatorAtom)aexpr).Left;
            Assert.IsInstanceOfType(subexpr, typeof(MassAtom));
            Assert.AreEqual(0, subexpr.MassNumber);
        }
    }
}
