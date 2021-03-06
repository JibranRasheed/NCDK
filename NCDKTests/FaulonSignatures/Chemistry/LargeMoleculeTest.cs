using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NCDK.FaulonSignatures.Chemistry
{
    [TestClass()]
    public class LargeMoleculeTest
    {
        public void AddRing(int atomToAttachTo, int ringSize, Molecule molecule)
        {
            var numberOfAtoms = molecule.GetAtomCount();
            var previous = atomToAttachTo;
            for (int i = 0; i < ringSize; i++)
            {
                molecule.AddAtom("C");
                int current = numberOfAtoms + i;
                molecule.AddSingleBond(previous, current);
                previous = current;
            }
            molecule.AddSingleBond(numberOfAtoms, numberOfAtoms + (ringSize - 1));
        }

        public Molecule MakeMinimalMultiRing(int ringCount, int ringSize)
        {
            var mol = new Molecule();
            mol.AddAtom("C");
            for (int i = 0; i < ringCount; i++)
            {
                AddRing(0, ringSize, mol);
            }
            return mol;
        }

        public Molecule MakeTetrakisTriphenylPhosphoranylRhodium()
        {
            var ttpr = new Molecule();
            ttpr.AddAtom("Rh");
            int phosphateCount = 3;
            for (int i = 1; i <= phosphateCount; i++)
            {
                ttpr.AddAtom("P");
                ttpr.AddSingleBond(0, i);
            }

            int phenylCount = 3;
            for (int j = 1; j <= phosphateCount; j++)
            {
                for (int k = 0; k < phenylCount; k++)
                {
                    AddRing(j, 6, ttpr);
                }
            }
            return ttpr;
        }

        [TestMethod()]
        public void DodecahedraneTest()
        {
            var mol = new Molecule();
            for (int i = 0; i < 20; i++) { mol.AddAtom("C"); }
            mol.AddSingleBond(0, 1);
            mol.AddSingleBond(0, 4);
            mol.AddSingleBond(1, 2);
            mol.AddSingleBond(2, 7);
            mol.AddSingleBond(3, 4);
            mol.AddSingleBond(3, 8);
            mol.AddSingleBond(5, 10);
            mol.AddSingleBond(5, 11);
            mol.AddSingleBond(6, 11);
            mol.AddSingleBond(6, 12);
            mol.AddSingleBond(7, 13);
            mol.AddSingleBond(8, 14);
            mol.AddSingleBond(9, 10);
            mol.AddSingleBond(9, 14);
            mol.AddSingleBond(12, 17);
            mol.AddSingleBond(13, 18);
            mol.AddSingleBond(15, 16);
            mol.AddSingleBond(15, 19);
            mol.AddSingleBond(16, 17);
            mol.AddSingleBond(18, 19);

            mol.AddBond(0, 5, Molecule.BondOrder.Double);
            mol.AddBond(1, 6, Molecule.BondOrder.Double);
            mol.AddBond(2, 3, Molecule.BondOrder.Double);
            mol.AddBond(4, 9, Molecule.BondOrder.Double);
            mol.AddBond(7, 12, Molecule.BondOrder.Double);
            mol.AddBond(8, 13, Molecule.BondOrder.Double);
            mol.AddBond(10, 15, Molecule.BondOrder.Double);
            mol.AddBond(11, 16, Molecule.BondOrder.Double);
            mol.AddBond(17, 18, Molecule.BondOrder.Double);
            mol.AddBond(14, 19, Molecule.BondOrder.Double);

            for (int i = 0; i < 20; i++)
            {
                Assert.AreEqual(4, mol.GetTotalOrder(i), "Atom " + i + " has wrong order");
            }

            var mqg = new MoleculeQuotientGraph(mol);
            Console.Out.WriteLine(mqg);
            Assert.AreEqual(5, mqg.GetVertexCount());
            Assert.AreEqual(9, mqg.GetEdgeCount());
            Assert.AreEqual(3, mqg.NumberOfLoopEdges());
        }

        [TestMethod()]
        public void TtprTest()
        {
            var ttpr = MakeTetrakisTriphenylPhosphoranylRhodium();
            var molSig = new MoleculeSignature(ttpr);
            var sigString = molSig.SignatureStringForVertex(0);
            Console.Out.WriteLine(sigString);
        }

        [TestMethod()]
        public void TestMinimalMol()
        {
            var mol = MakeMinimalMultiRing(6, 3);
            var molSig = new MoleculeSignature(mol);
            string sigString = molSig.SignatureStringForVertex(0);
            Console.Out.WriteLine(sigString);
            Console.Out.WriteLine(mol);
            Console.Out.WriteLine("result " + sigString);
        }

        public Molecule MakeChain(int length)
        {
            var chain = new Molecule();
            int previous = -1;
            for (int i = 0; i < length; i++)
            {
                chain.AddAtom("C");
                if (previous != -1)
                {
                    chain.AddSingleBond(previous, i);
                }
                previous = i;
            }
            return chain;
        }

        [TestMethod()]
        public void TestLongChains()
        {
            int length = 10;
            var chain = MakeChain(length);
            var molSig = new MoleculeSignature(chain);
            string sigString = molSig.ToCanonicalString();
            Console.Out.WriteLine(sigString);
        }

        public void DrawTrees(MoleculeQuotientGraph mqg, string directoryPath)
        {
            var signatureStrings = mqg.GetVertexSignatureStrings();
            Trace.TraceWarning("TreeDrawer is not implemented yet.");
        }

        [TestMethod()]
        public void BuckyballTest()
        {
            var mol = MoleculeReader.ReadMolfile("NCDK.FaulonSignatures.Data.buckyball.mol");
            var mqg = new MoleculeQuotientGraph(mol);
            Console.Out.WriteLine(mqg);
            Assert.AreEqual(32, mqg.GetVertexCount());
            Assert.AreEqual(49, mqg.GetEdgeCount());
            Assert.AreEqual(6, mqg.NumberOfLoopEdges());
        }

        [TestMethod()]
        public void BuckyballWithoutMultipleBonds()
        {
            var mol = MoleculeReader.ReadMolfile("NCDK.FaulonSignatures.Data.buckyball.mol");
            foreach (Molecule.Bond bond in mol.Bonds())
            {
                bond.order = Molecule.BondOrder.Single;
            }
            var mqg = new MoleculeQuotientGraph(mol);
            Console.Out.WriteLine(mqg);
            Assert.AreEqual(1, mqg.GetVertexCount());
            Assert.AreEqual(1, mqg.GetEdgeCount());
            Assert.AreEqual(1, mqg.NumberOfLoopEdges());
        }

        [TestMethod()]
        public void FaulonsBuckySignatures()
        {
            var mol = MoleculeReader.ReadMolfile("data/buckyball.mol");
            try
            {
                var filename = "data/buckysigs3.txt";
                var sigs = ReadSigs2(filename);
                var mqg = new MoleculeQuotientGraph(mol, sigs);
                Console.Out.WriteLine(mqg);
                Assert.AreEqual(32, mqg.GetVertexCount());
                Assert.AreEqual(49, mqg.GetEdgeCount());
                Assert.AreEqual(6, mqg.NumberOfLoopEdges());
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e);
                return;
            }
        }

        public IList<string> ReadSigs(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                string line;
                var sigs = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    var index = line.IndexOf(" ") + 1;
                    var count = int.Parse(line.Substring(0, index - 1));
                    var sig = line.Substring(index);
                    Console.Out.WriteLine(count);
                    sigs.Add(sig);
                }
                return sigs;
            }
        }

        public List<string> ReadSigs2(string filename)
        {
            using (var reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read)))
            {
                string line;
                var sigs = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    var bits = Strings.Tokenize(line).ToArray();
                    var sig = bits[3];
                    sigs.Add(sig);
                }
                reader.Close();
                sigs.Reverse();
                return sigs;
            }
        }
    }
}
