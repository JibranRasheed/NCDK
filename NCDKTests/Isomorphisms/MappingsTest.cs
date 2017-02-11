/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Smiles;
using NCDK.Util;
using System.Collections.Generic;

namespace NCDK.Isomorphisms
{
    /**
     * @author John May
     * @cdk.module test-smarts
     */
    [TestClass()]
    public class MappingsTest
    {
        [TestMethod()]
        public void Filter()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);

            int[] p1 = { 0, 1, 2 };
            int[] p2 = { 0, 2, 1 };
            int[] p3 = { 0, 3, 4 };
            int[] p4 = { 0, 4, 3 };

            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);
            m_iterator.SetupSequence(n => n.Current).Returns(p1).Returns(p2).Returns(p3).Returns(p4);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);

            var m_f = new Mock<Predicate<int[]>>(); var f = m_f.Object;
            m_f.Setup(n => n.Apply(p1)).Returns(false);
            m_f.Setup(n => n.Apply(p2)).Returns(true);
            m_f.Setup(n => n.Apply(p3)).Returns(false);
            m_f.Setup(n => n.Apply(p4)).Returns(true);

            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { p2, p4 }, ms.Filter(f).ToArray()));
        }

        [TestMethod()]
        public void Map()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);

            int[] p1 = { 0, 1, 2 };
            int[] p2 = { 0, 2, 1 };
            int[] p3 = { 0, 3, 4 };
            int[] p4 = { 0, 4, 3 };

            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);
            m_iterator.SetupSequence(n => n.Current).Returns(p1).Returns(p2).Returns(p3).Returns(p4);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);

            var m_f = new Mock<Function<int[], string>>(); var f = m_f.Object;
            m_f.Setup(n => n.Apply(p1)).Returns("p1");
            m_f.Setup(n => n.Apply(p2)).Returns("p2");
            m_f.Setup(n => n.Apply(p3)).Returns("p3");
            m_f.Setup(n => n.Apply(p4)).Returns("p4");

            var strings = ms.GetMapping(f);
            var stringIt = strings.GetEnumerator();

            m_f.Verify(n => n.Apply(It.IsAny<int[]>()), Times.AtMost(0));

            Assert.IsTrue(stringIt.MoveNext());
            Assert.AreEqual(stringIt.Current, "p1");
            Assert.IsTrue(stringIt.MoveNext());
            Assert.AreEqual(stringIt.Current, "p2");
            Assert.IsTrue(stringIt.MoveNext());
            Assert.AreEqual(stringIt.Current, "p3");
            Assert.IsTrue(stringIt.MoveNext());
            Assert.AreEqual(stringIt.Current, "p4");
            Assert.IsFalse(stringIt.MoveNext());

            m_f.Verify(n => n.Apply(It.IsAny<int[]>()), Times.AtMost(4));
        }

        [TestMethod()]
        public void Limit()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);
            m_iterator.SetupGet(n => n.Current).Returns(new int[0]);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.AreEqual(2, ms.Limit(2).Count());
            m_iterator.Verify(n => n.Current, Times.AtMost(2)); // was only called twice
        }

        [TestMethod()]
        public void Stereochemistry()
        {
            // tested by Filter() + StereoMatchTest
        }

        [TestMethod()]
        public void UniqueAtoms()
        {
            // tested by Filter() + MappingPredicatesTest
        }

        [TestMethod()]
        public void UniqueBonds()
        {
            // tested by Filter() + MappingPredicatesTest
        }

        [TestMethod()]
        public void ToArray()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);

            int[] p1 = { 0, 1, 2 };
            int[] p2 = { 0, 2, 1 };
            int[] p3 = { 0, 3, 4 };
            int[] p4 = { 0, 4, 3 };

            m_iterator.SetupSequence(n => n.Current).Returns(p1).Returns(p2).Returns(p3).Returns(p4);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { p1, p2, p3, p4 }, ms.ToArray()));
        }

        [TestMethod()]
        public void ToAtomMap()
        {
            IAtomContainer query = Smi("CC");
            IAtomContainer target = Smi("CC");

            var iterable = Pattern.FindIdentical(query).MatchAll(target).ToAtomMap();
            var iterator = iterable.GetEnumerator();

            Assert.IsTrue(iterator.MoveNext());
            IDictionary<IAtom, IAtom> m1 = iterator.Current;
            Assert.AreEqual(m1[query.Atoms[0]], target.Atoms[0]);
            Assert.AreEqual(m1[query.Atoms[1]], target.Atoms[1]);
            Assert.IsTrue(iterator.MoveNext());
            IDictionary<IAtom, IAtom> m2 = iterator.Current;
            Assert.AreEqual(m2[query.Atoms[0]], target.Atoms[1]);
            Assert.AreEqual(m2[query.Atoms[1]], target.Atoms[0]);
            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod()]
        public void ToBondMap()
        {
            IAtomContainer query = Smi("CCC");
            IAtomContainer target = Smi("CCC");

            var iterable = Pattern.FindIdentical(query).MatchAll(target).ToBondMap();
            var iterator = iterable.GetEnumerator();

            Assert.IsTrue(iterator.MoveNext());
            IDictionary<IBond, IBond> m1 = iterator.Current;
            Assert.AreEqual(m1[query.Bonds[0]], target.Bonds[0]);
            Assert.AreEqual(m1[query.Bonds[1]], target.Bonds[1]);
            Assert.IsTrue(iterator.MoveNext());
            IDictionary<IBond, IBond> m2 = iterator.Current;
            Assert.AreEqual(m2[query.Bonds[0]], target.Bonds[1]);
            Assert.AreEqual(m2[query.Bonds[1]], target.Bonds[0]);
            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod()]
        public void ToAtomBondMap()
        {
            IAtomContainer query = Smi("CCC");
            IAtomContainer target = Smi("CCC");

            var iterable = Pattern.FindIdentical(query).MatchAll(target).ToAtomBondMap();
            var iterator = iterable.GetEnumerator();

            Assert.IsTrue(iterator.MoveNext());
            IDictionary<IChemObject, IChemObject> m1 = iterator.Current;
            Assert.AreEqual(m1[query.Atoms[0]], (IChemObject)target.Atoms[0]);
            Assert.AreEqual(m1[query.Atoms[1]], (IChemObject)target.Atoms[1]);
            Assert.AreEqual(m1[query.Atoms[2]], (IChemObject)target.Atoms[2]);
            Assert.AreEqual(m1[query.Bonds[0]], (IChemObject)target.Bonds[0]);
            Assert.AreEqual(m1[query.Bonds[1]], (IChemObject)target.Bonds[1]);
            Assert.IsTrue(iterator.MoveNext());
            IDictionary<IChemObject, IChemObject> m2 = iterator.Current;
            Assert.AreEqual(m2[query.Atoms[0]], (IChemObject)target.Atoms[2]);
            Assert.AreEqual(m2[query.Atoms[1]], (IChemObject)target.Atoms[1]);
            Assert.AreEqual(m2[query.Atoms[2]], (IChemObject)target.Atoms[0]);
            Assert.AreEqual(m2[query.Bonds[0]], (IChemObject)target.Bonds[1]);
            Assert.AreEqual(m2[query.Bonds[1]], (IChemObject)target.Bonds[0]);
            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod()]
        public void ToSubstructures()
        {
            IAtomContainer query = Smi("O1CC1");
            IAtomContainer target = Smi("C1OC1CCC");

            var iterable = Pattern.FindSubstructure(query)
                                                       .MatchAll(target)
                                                       .GetUniqueAtoms()
                                                       .ToSubstructures();
            var iterator = iterable.GetEnumerator();

            Assert.IsTrue(iterator.MoveNext());
            IAtomContainer submol = iterator.Current;
            Assert.AreNotEqual(query, submol);
            // note that indices are mapped from query to target
            Assert.AreEqual(submol.Atoms[0], target.Atoms[1]); // oxygen
            Assert.AreEqual(submol.Atoms[1], target.Atoms[0]); // C
            Assert.AreEqual(submol.Atoms[2], target.Atoms[2]); // C
            Assert.AreEqual(submol.Bonds[0], target.Bonds[0]); // C-O bond
            Assert.AreEqual(submol.Bonds[1], target.Bonds[2]); // O-C bond
            Assert.AreEqual(submol.Bonds[2], target.Bonds[1]); // C-C bond
            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod()]
        public void AtLeast()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);
            m_iterator.SetupGet(n => n.Current).Returns(new int[0]);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);

            Assert.IsTrue(ms.AtLeast(2));
            m_iterator.Verify(n => n.Current, Times.AtMost(2)); // was only called twice
        }

        [TestMethod()]
        public void First()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(false);

            int[] p1 = new int[0];
            int[] p2 = new int[0];

            m_iterator.SetupSequence(n => n.Current).Returns(p1).Returns(p2);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.AreSame(p1, ms.First());
        }

        [TestMethod()]
        public void Count()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);
            m_iterator.SetupSequence(n => n.Current).Returns(new int[0]);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.AreEqual(5, ms.Count());
        }

        [TestMethod()]
        public void CountUnique()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);

            int[] p1 = { 0, 1, 2 };
            int[] p2 = { 0, 2, 1 };
            int[] p3 = { 0, 3, 4 };
            int[] p4 = { 0, 4, 3 };

            m_iterator.SetupSequence(n => n.Current).Returns(p1).Returns(p2).Returns(p3).Returns(p4);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.AreEqual(2, ms.CountUnique());
        }

        [TestMethod()]
        public void iterator()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.AreSame(iterator, ms.GetEnumerator());
        }

        SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);

        IAtomContainer Smi(string smi)
        {
            return smipar.ParseSmiles(smi);
        }
    }
}
