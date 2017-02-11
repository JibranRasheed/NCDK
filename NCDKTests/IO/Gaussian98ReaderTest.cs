using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Tools.Manipulator;
using System.IO;
using System.Linq;

namespace NCDK.IO
{
    /// <summary>
    /// A Test case for the gaussian 98 (G98Reader) class.
    /// </summary>
    // @cdk.module test-io
    // @author Christoph Steinbeck
	[TestClass()]
    public class Gaussian98ReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.Gaussian.g98ReaderNMRTest.log";
        static readonly Gaussian98Reader simpleReader = new Gaussian98Reader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        [TestMethod()]
        public void TestAccepts()
        {
            Assert.IsTrue(ChemObjectIOToTest.Accepts(typeof(ChemFile)));
        }

        [TestMethod()]
        public void TestNMRReading()
        {
            IAtomContainer atomContainer = null;
            //bool foundOneShieldingEntry = false;
            //Double shielding = null;
            object obj = null;
            int shieldingCounter = 0;
            string filename = "NCDK.Data.Gaussian.g98ReaderNMRTest.log";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            TextReader inputReader = new StreamReader(ins);
            Gaussian98Reader g98Reader = new Gaussian98Reader(inputReader);
            ChemFile chemFile = (ChemFile)g98Reader.Read(new ChemFile());
            g98Reader.Close();
            var atomContainersList = ChemFileManipulator.GetAllAtomContainers(chemFile);
            Assert.IsNotNull(atomContainersList);
            Assert.IsTrue(atomContainersList.Count() == 54);
            //Debug.WriteLine("Found " + atomContainers.Length + " atomContainers");
            int counter = 0;
            foreach (var ac in atomContainersList)
            {
                shieldingCounter = 0;
                atomContainer = ac;
                for (int g = 0; g < atomContainer.Atoms.Count; g++)
                {
                    obj = atomContainer.Atoms[g].GetProperty<double?>(CDKPropertyName.ISOTROPIC_SHIELDING);
                    if (obj != null)
                    {
                        //shielding = (double)object;
                        shieldingCounter++;
                    }
                }
                if (counter < 53)
                    Assert.IsTrue(shieldingCounter == 0);
                else
                    Assert.IsTrue(shieldingCounter == ac.Atoms.Count);
                //Debug.WriteLine("AtomContainer " + (f + 1) + " has " + atomContainers[f].Atoms.Count + " atoms and " + shieldingCounter + " shielding entries");
                counter++;
            }
        }
    }
}
