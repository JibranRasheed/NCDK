using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public partial class ChiPathClusterDescriptorTest : MolecularDescriptorTest<ChiPathClusterDescriptor>
    {
  
        [TestMethod()]
        public void TestDan64()
        {
            var mol = CDK.Builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("C");
            a1.Point2D = new Vector2(0.7500000000000004, 2.799038105676658);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("C");
            a2.Point2D = new Vector2(0.0, 1.5);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            a3.Point2D = new Vector2(0.0, 0.0);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("O");
            a4.Point2D = new Vector2(-1.2990381056766582, 0.7500000000000001);
            mol.Atoms.Add(a4);
            IBond b1 = mol.Builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a4, a2, BondOrder.Single);
            mol.Bonds.Add(b4);

            var ret = CreateDescriptor().Calculate(mol).Values;

            Assert.AreEqual(0.0000, ret[0], 0.0001);
            Assert.AreEqual(0.0000, ret[1], 0.0001);
            Assert.AreEqual(0.0000, ret[2], 0.0001);
            Assert.AreEqual(0.0000, ret[3], 0.0001);
            Assert.AreEqual(0.0000, ret[4], 0.0001);
            Assert.AreEqual(0.0000, ret[5], 0.0001);
        }

        [TestMethod()]
        public void TestDan154()
        {
            var mol = CDK.Builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("C");
            a1.Point2D = new Vector2(0.0, 1.5);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("C");
            a2.Point2D = new Vector2(0.0, 0.0);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            a3.Point2D = new Vector2(-1.2990381056766584, -0.7500000000000001);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("C");
            a4.Point2D = new Vector2(-2.598076211353316, -2.220446049250313E-16);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("C");
            a5.Point2D = new Vector2(-2.5980762113533165, 1.5);
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.NewAtom("C");
            a6.Point2D = new Vector2(-1.2990381056766582, 2.2500000000000004);
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.NewAtom("Cl");
            a7.Point2D = new Vector2(-1.2990381056766582, 3.7500000000000004);
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.NewAtom("Cl");
            a8.Point2D = new Vector2(1.2990381056766576, -0.7500000000000007);
            mol.Atoms.Add(a8);
            IBond b1 = mol.Builder.NewBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a4, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a5, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a6, a5, BondOrder.Double);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.NewBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.NewBond(a7, a6, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.NewBond(a8, a2, BondOrder.Single);
            mol.Bonds.Add(b8);

            var ret = CreateDescriptor().Calculate(mol).Values;

            Assert.AreEqual(0.7416, ret[0], 0.0001);
            Assert.AreEqual(1.0934, ret[1], 0.0001);
            Assert.AreEqual(1.0202, ret[2], 0.0001);
            Assert.AreEqual(0.4072, ret[3], 0.0001);
            Assert.AreEqual(0.5585, ret[4], 0.0001);
            Assert.AreEqual(0.4376, ret[5], 0.0001);
        }

        [TestMethod()]
        public void TestDan248()
        {
            var mol = CDK.Builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("C");
            a1.Point2D = new Vector2(0.0, 1.5);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("C");
            a2.Point2D = new Vector2(0.0, 0.0);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            a3.Point2D = new Vector2(-1.2990381056766584, -0.7500000000000001);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("C");
            a4.Point2D = new Vector2(-2.598076211353316, -2.220446049250313E-16);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("C");
            a5.Point2D = new Vector2(-2.5980762113533165, 1.5);
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.NewAtom("C");
            a6.Point2D = new Vector2(-1.2990381056766582, 2.2500000000000004);
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.NewAtom("C");
            a7.Point2D = new Vector2(-3.897114317029975, 2.249999999999999);
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.NewAtom("O");
            a8.Point2D = new Vector2(-1.2990381056766587, -2.25);
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.NewAtom("C");
            a9.Point2D = new Vector2(1.477211629518312, 1.2395277334996044);
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.NewAtom("C");
            a10.Point2D = new Vector2(0.5130302149885025, 2.909538931178863);
            mol.Atoms.Add(a10);
            IBond b1 = mol.Builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a5, a4, BondOrder.Double);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a6, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.NewBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.NewBond(a7, a5, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.NewBond(a8, a3, BondOrder.Double);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.NewBond(a9, a1, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.NewBond(a10, a1, BondOrder.Single);
            mol.Bonds.Add(b10);

            var ret = CreateDescriptor().Calculate(mol).Values;

            Assert.AreEqual(1.6076, ret[0], 0.0001);
            Assert.AreEqual(3.6550, ret[1], 0.0001);

            Assert.AreEqual(3.3337, ret[2], 0.0001);
            Assert.AreEqual(1.1410, ret[3], 0.0001);
            Assert.AreEqual(2.1147, ret[4], 0.0001);
            Assert.AreEqual(1.7148, ret[5], 0.0001);
        }
    }
}
