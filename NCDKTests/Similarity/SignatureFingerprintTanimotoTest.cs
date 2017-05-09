/* Copyright (C) 1997-2007  The Chemistry Development Kit (CKD) project
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
 *
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Fingerprints;
using NCDK.Similarity;
using NCDK.Smiles;
using NCDK.Templates;
using System.Collections.Generic;

namespace NCDK.Similarity
{
    // @cdk.module test-signature
    [TestClass()]
    public class SignatureFingerprintTanimotoTest : CDKTestCase
    {
        // @cdk.bug 3310138
        [TestMethod()]
        public void TestRawTanimotoBetween0and1()
        {
            SmilesParser smilesParser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = smilesParser.ParseSmiles("Cc1nc(C(=O)NC23CC4CC(CC(C4)C2)C3)c(C)n1C5CCCCC5");
            IAtomContainer mol2 = smilesParser
                    .ParseSmiles("CS(=O)(=O)Nc1ccc(Cc2onc(n2)c3ccc(cc3)S(=O)(=O)Nc4ccc(CCNC[C@H](O)c5cccnc5)cc4)cc1");
            SignatureFingerprinter fingerprinter = new SignatureFingerprinter(0);
            IDictionary<string, int> fp1 = fingerprinter.GetRawFingerprint(mol1);
            IDictionary<string, int> fp2 = fingerprinter.GetRawFingerprint(mol2);
            var tanimoto = Tanimoto.Calculate(fp1, fp2);
            Assert.IsTrue(tanimoto > 0 && tanimoto < 1, $"Tanimoto expected to be between 0 and 1, was:{tanimoto}");
        }

        [TestMethod()]
        public void TestICountFingerprintComparison()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeIndole();
            IAtomContainer mol2 = TestMoleculeFactory.MakeIndole();
            SignatureFingerprinter fingerprinter = new SignatureFingerprinter();
            ICountFingerprint fp1 = fingerprinter.GetCountFingerprint(mol1);
            ICountFingerprint fp2 = fingerprinter.GetCountFingerprint(mol2);
            double tanimoto = Tanimoto.Calculate(fp1, fp2);
            Assert.AreEqual(1.0, tanimoto, 0.001);

        }

        [TestMethod()]
        public void CompareCountFingerprintAndRawFingerprintTanimoto()
        {
            IAtomContainer mol1 = TestMoleculeFactory.Make123Triazole();
            IAtomContainer mol2 = TestMoleculeFactory.MakeImidazole();
            SignatureFingerprinter fingerprinter = new SignatureFingerprinter(1);
            ICountFingerprint countFp1 = fingerprinter.GetCountFingerprint(mol1);
            ICountFingerprint countFp2 = fingerprinter.GetCountFingerprint(mol2);
            IDictionary<string, int> feat1 = fingerprinter.GetRawFingerprint(mol1);
            IDictionary<string, int> feat2 = fingerprinter.GetRawFingerprint(mol2);
            var rawTanimoto = Tanimoto.Calculate(feat1, feat2);
            double countTanimoto = Tanimoto.Method1(countFp1, countFp2);
            Assert.AreEqual(rawTanimoto, countTanimoto, 0.001);
        }

        [TestMethod()]
        public void TestCountMethod1and2()
        {
            ICountFingerprint fp1 = new IntArrayCountFingerprint(new Dictionary<string, int>()
                { { "A", 3 } });
            ICountFingerprint fp2 = new IntArrayCountFingerprint(new Dictionary<string, int>()
                { { "A", 4 } });
            Assert.AreEqual(0.923, Tanimoto.Method1(fp1, fp2), 0.001);
            Assert.AreEqual(0.75, Tanimoto.Method2(fp1, fp2), 0.001);

            IAtomContainer mol1 = TestMoleculeFactory.MakeIndole();
            IAtomContainer mol2 = TestMoleculeFactory.MakeIndole();
            SignatureFingerprinter fingerprinter = new SignatureFingerprinter();
            fp1 = fingerprinter.GetCountFingerprint(mol1);
            fp2 = fingerprinter.GetCountFingerprint(mol2);
            Assert.AreEqual(1.0, Tanimoto.Method1(fp1, fp2), 0.001);
            Assert.AreEqual(1.0, Tanimoto.Method2(fp1, fp2), 0.001);
        }

        [TestMethod()]
        public void TestCompaRingBitFingerprintAndCountBehavingAsBit()
        {
            IAtomContainer mol1 = TestMoleculeFactory.Make123Triazole();
            IAtomContainer mol2 = TestMoleculeFactory.MakeImidazole();

            SignatureFingerprinter fingerprinter = new SignatureFingerprinter(1);
            ICountFingerprint countFp1 = fingerprinter.GetCountFingerprint(mol1);
            ICountFingerprint countFp2 = fingerprinter.GetCountFingerprint(mol2);
            countFp1.SetBehaveAsBitFingerprint(true);
            countFp2.SetBehaveAsBitFingerprint(true);
            IBitFingerprint bitFp1 = fingerprinter.GetBitFingerprint(mol1);
            IBitFingerprint bitFp2 = fingerprinter.GetBitFingerprint(mol2);
            double bitTanimoto = Tanimoto.Calculate(bitFp1, bitFp2);
            double countTanimoto1 = Tanimoto.Method1(countFp1, countFp2);
            double countTanimoto2 = Tanimoto.Method2(countFp1, countFp2);

            Assert.AreEqual(countTanimoto1, countTanimoto2, 0.001);
            Assert.AreEqual(bitTanimoto, countTanimoto1, 0.001);
        }
    }
}
