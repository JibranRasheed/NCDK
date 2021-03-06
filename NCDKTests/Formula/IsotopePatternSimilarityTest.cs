using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Manipulator;

namespace NCDK.Formula
{
    /// <summary>
    /// Class testing the IsotopePatternSimilarity class.
    /// </summary>
    // @cdk.module test-formula
    [TestClass()]
    public class IsotopePatternSimilarityTest : CDKTestCase
    {
        private readonly static IChemObjectBuilder builder = CDK.Builder;

        public IsotopePatternSimilarityTest()
            : base()
        { }

        [TestMethod()]
        public void TestIsotopePatternSimilarity()
        {
            IsotopePatternSimilarity is_ = new IsotopePatternSimilarity();
            Assert.IsNotNull(is_);
        }

        [TestMethod()]
        public void TestSeToleranceDouble()
        {
            IsotopePatternSimilarity is_ = new IsotopePatternSimilarity { Tolerance = 0.001 };
            Assert.IsNotNull(is_);
        }

        [TestMethod()]
        public void TestGetTolerance()
        {
            IsotopePatternSimilarity is_ = new IsotopePatternSimilarity { Tolerance = 0.001 };
            Assert.AreEqual(0.001, is_.Tolerance, 0.000001);
        }

        /// <summary>
        /// Histidine example
        /// </summary>
        [TestMethod()]
        public void TestCompareIsotopePatternIsotopePattern()
        {
            var is_ = new IsotopePatternSimilarity();

            IsotopePattern spExp = new IsotopePattern(new[]
                {
                    new IsotopeContainer(156.07770, 1),
                    new IsotopeContainer(157.07503, 0.0004),
                    new IsotopeContainer(157.08059, 0.0003),
                    new IsotopeContainer(158.08135, 0.002),
                });
            spExp.MonoIsotope = spExp.Isotopes[0];
            
            var formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C6H10N3O2", builder);
            var isotopeGe = new IsotopePatternGenerator(0.1);
            var patternIsoPredicted = isotopeGe.GetIsotopes(formula);
            var patternIsoNormalize = IsotopePatternManipulator.Normalize(patternIsoPredicted);
            var score = is_.Compare(spExp, patternIsoNormalize);
            Assert.AreNotSame(0.0, score);
        }

        /// <summary>
        /// Histidine example
        /// </summary>
        [TestMethod()]
        public void TestSelectingMF()
        {
            var is_ = new IsotopePatternSimilarity();

            IsotopePattern spExp = new IsotopePattern(new[]
                {
                    new IsotopeContainer(156.07770, 1),
                    new IsotopeContainer(157.07503, 0.0101),
                    new IsotopeContainer(157.08059, 0.074),
                    new IsotopeContainer(158.08135, 0.0024),
                });
            spExp.MonoIsotope = spExp.Isotopes[0];
            spExp.Charge = 1;

            double score = 0;
            string mfString = "";
            string[] listMF = { "C4H8N6O", "C2H12N4O4", "C3H12N2O5", "C6H10N3O2", "CH10N5O4", "C4H14NO5" };

            for (int i = 0; i < listMF.Length; i++)
            {
                IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula(listMF[i], builder);
                IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(0.01);
                IsotopePattern patternIsoPredicted = isotopeGe.GetIsotopes(formula);

                IsotopePattern patternIsoNormalize = IsotopePatternManipulator.Normalize(patternIsoPredicted);
                double tempScore = is_.Compare(spExp, patternIsoNormalize);
                if (score < tempScore)
                {
                    mfString = MolecularFormulaManipulator.GetString(formula);
                    score = tempScore;
                }
            }
            Assert.AreEqual("C6H10N3O2", mfString);
        }

        /// <summary>
        /// Real example. Lipid PC
        /// </summary>
        [TestMethod()]
        public void TestExperiment()
        {
            var spExp = new IsotopePattern(new[]
                {
                    new IsotopeContainer(762.6006, 124118304),
                    new IsotopeContainer(763.6033, 57558840),
                    new IsotopeContainer(764.6064, 15432262),
                });
            spExp.MonoIsotope = spExp.Isotopes[0];
            spExp.Charge = 1.0;

            var formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C42H85NO8P", CDK.Builder);
            var isotopeGe = new IsotopePatternGenerator(0.01);
            var patternIsoPredicted = isotopeGe.GetIsotopes(formula);
            var is_ = new IsotopePatternSimilarity();
            var score = is_.Compare(spExp, patternIsoPredicted);

            Assert.AreEqual(0.97, score, .01);
        }
    }
}
