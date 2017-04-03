﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;

namespace NCDK.Formula
{
    [TestClass]
    public class MolecularFormulaGenerator_Example
    {
        [TestMethod]
        [TestCategory("Example")]
        public void Main()
        {
            #region
            IsotopeFactory ifac = Isotopes.Instance;
             IIsotope c = ifac.GetMajorIsotope("C");
             IIsotope h = ifac.GetMajorIsotope("H");
             IIsotope n = ifac.GetMajorIsotope("N");
             IIsotope o = ifac.GetMajorIsotope("O");
             IIsotope p = ifac.GetMajorIsotope("P");
             IIsotope s = ifac.GetMajorIsotope("S");
             
             MolecularFormulaRange mfRange = new MolecularFormulaRange();
             mfRange.Add(c, 0, 50);
             mfRange.Add(h, 0, 100);
             mfRange.Add(o, 0, 50);
             mfRange.Add(n, 0, 50);
             mfRange.Add(p, 0, 10);
             mfRange.Add(s, 0, 10);

            var builder = Silent.ChemObjectBuilder.Instance;
            double minMass = 133.003;
            double maxMass = 133.005;
            MolecularFormulaGenerator mfg = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = mfg.GetAllFormulas();
            #endregion
        }
    }
}
