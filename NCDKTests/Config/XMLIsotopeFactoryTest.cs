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
using NCDK.Silent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NCDK.Config
{
    /// <summary>
    /// Checks the functionality of the <see cref="XMLIsotopeFactory"/>
    /// </summary>
    // @cdk.module test-extra
    [TestClass()]
    public class XMLIsotopeFactoryTest 
        : CDKTestCase
    {
        readonly bool standAlone = false;
        readonly static AtomTypeFactory atf = AtomTypeFactory.GetInstance(new ChemObject().Builder);
        private const string JAXP_SCHEMA_LANGUAGE = "http://java.sun.com/xml/jaxp/properties/schemaLanguage";
        private const string W3C_XML_SCHEMA = "http://www.w3.org/2001/XMLSchema";

        private static IList<FileInfo> tmpFiles = new List<FileInfo>();
        private static string tmpCMLSchema;

        [ClassInitialize()]
        public static void Initialize(TestContext context)
        {
            using (var ins = ResourceLoader.GetAsStream(typeof(AtomTypeFactory).Assembly, "NCDK.IO.CML.Data.cml25b1.xsd"))
            {
                tmpCMLSchema = CopyFileToTmp("cml2.5.b1", ".xsd", ins, null, null);
            }
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            foreach (var fi in tmpFiles)
            {
                try
                {
                    fi.Delete();
                }
                catch (Exception)
                { }
            }
            tmpFiles.Clear();
        }

        [TestMethod()]
        public void TestGetInstanceIChemObjectBuilder()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Assert.IsNotNull(isofac);
        }

        [TestMethod()]
        public void TestGetSize()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Assert.IsTrue(isofac.Count > 0);
        }

        [TestMethod()]
        public void TestConfigureIAtom()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Atom atom = new Atom("H");
            isofac.Configure(atom);
            Assert.AreEqual(1, atom.AtomicNumber);
        }

        [TestMethod()]
        public void TestConfigureIAtomIIsotope()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Atom atom = new Atom("H");
            IIsotope isotope = new Default.Isotope("H", 2);
            isofac.Configure(atom, isotope);
            Assert.AreEqual(2, atom.MassNumber.Value);
        }

        [TestMethod()]
        public void TestGetMajorIsotopeString()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope isotope = isofac.GetMajorIsotope("Te");
            if (standAlone) Console.Out.WriteLine($"Isotope: {isotope}");
            Assert.AreEqual(129.9062244, isotope.ExactMass.Value, 0.0001);
        }

        [TestMethod()]
        public void TestGetMajorIsotopeNonelement()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope isotope = isofac.GetMajorIsotope("E");
            Assert.IsNull(isotope);
        }

        [TestMethod()]
        public void TestGetMajorIsotopeInt()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope isotope = isofac.GetMajorIsotope(17);
            Assert.AreEqual("Cl", isotope.Symbol);
        }

        [TestMethod()]
        public void TestGetElementString()
        {
            XMLIsotopeFactory elfac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IElement element = elfac.GetElement("Br");
            Assert.AreEqual(35, element.AtomicNumber);
        }

        [TestMethod()]
        public void TestGetElementNonelement()
        {
            XMLIsotopeFactory elfac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IElement element = elfac.GetElement("E");
            Assert.IsNull(element);
        }

        [TestMethod()]
        public void TestGetElemenInt()
        {
            XMLIsotopeFactory elfac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IElement element = elfac.GetElement(6);
            Assert.AreEqual("C", element.Symbol);
        }

        [TestMethod()]
        public void TestGetElementSymbolInt()
        {
            XMLIsotopeFactory elfac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            String symbol = elfac.GetElementSymbol(8);
            Assert.AreEqual("O", symbol);
        }

        [TestMethod()]
        public void TestGetIsotopesString()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope[] list = isofac.GetIsotopes("He").ToArray();
            Assert.AreEqual(8, list.Length);
        }

        [TestMethod()]
        public void TestGetIsotopesNonelement()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope[] list = isofac.GetIsotopes("E").ToArray();
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Length);
        }

        [TestMethod()]
        public void TestGetIsotopes()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope[] list = isofac.GetIsotopes().ToArray();
            Assert.IsTrue(list.Length > 200);
        }

        [TestMethod()]
        public void TestGetIsotopesDoubleDouble()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope[] list = isofac.GetIsotopes(87.90, 0.01).ToArray();
            //        should return:
            //        Isotope match: 88Sr has mass 87.9056121
            //        Isotope match: 88Y has mass 87.9095011
            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(88, list[0].MassNumber.Value);
            Assert.AreEqual(88, list[1].MassNumber.Value);
        }

        [TestMethod()]
        public void TestIsElementString()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Assert.IsTrue(isofac.IsElement("C"));
        }

        [TestMethod()]
        public void TestConfigureAtomsIAtomContainer()
        {
            AtomContainer container = new AtomContainer();
            container.Atoms.Add(new Atom("C"));
            container.Atoms.Add(new Atom("H"));
            container.Atoms.Add(new Atom("N"));
            container.Atoms.Add(new Atom("O"));
            container.Atoms.Add(new Atom("F"));
            container.Atoms.Add(new Atom("Cl"));
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            isofac.ConfigureAtoms(container);
            foreach (var atom in container.Atoms)
            {
                Assert.IsTrue(0 < atom.AtomicNumber);
            }
        }

        [TestCategory("XMLValidity"), Ignore()]
        [TestMethod()]
        public void TestXMLValidityHybrid()
        {
            AssertValidCML("NCDK.Config.Data.isotopes.xml", "Isotopes");
        }

        private static void AssertValidCML(string atomTypeList, string shortcut)
        {
            using (var ins = ResourceLoader.GetAsStream(atomTypeList))
            {
                var tmpInput = CopyFileToTmp(shortcut, ".cmlinput", ins, "../../io/cml/data/cml25b1.xsd", new Uri(tmpCMLSchema).AbsolutePath);
                Assert.IsNotNull(ins, "Could not find the atom type list CML source");

                var doc = new XmlDocument();
                doc.Load(tmpInput);
                doc.Validate((sender, e) => Assert.Fail($"{shortcut} is not valid on line {e.Exception.LinePosition}: {e.Message}"));
            }
        }

        [TestMethod()]
        public void TestCanReadCMLSchema()
        {
            using (var cmlSchema = new FileStream(tmpCMLSchema, FileMode.Open))
            {
                Assert.IsNotNull(cmlSchema, "Could not find the CML schema");

                // make sure the schema is read
                var schemaDoc = XDocument.Load(cmlSchema);
                Assert.IsNotNull(schemaDoc.Root);
                Assert.AreEqual(((XNamespace)"http://www.w3.org/2001/XMLSchema") + "schema", schemaDoc.Root.Name);
            }
        }

        [TestMethod()]
        public void TestGetNaturalMassIElement()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Assert.AreEqual(1.0079760, isofac.GetNaturalMass(new Default.Element("H")), 0.1);
        }

        [TestMethod()]
        public void TestGetIsotope()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Assert.AreEqual(13.00335484, isofac.GetIsotope("C", 13).ExactMass.Value, 0.0000001);
        }

        [TestMethod()]
        public void TestGetIsotopeFromExactMass()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope carbon13 = isofac.GetIsotope("C", 13);
            IIsotope match = isofac.GetIsotope(carbon13.Symbol, carbon13.ExactMass.Value, 0.0001);
            Assert.IsNotNull(match);
            Assert.AreEqual(13, match.MassNumber.Value);
        }

        [TestMethod()]
        public void TestYeahSure()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope match = isofac.GetIsotope("H", (int)13.00001, 0.0001);
            Assert.IsNull(match);
        }

        [TestMethod()]
        public void TestGetIsotopeFromExactMassLargeTolerance()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope carbon13 = isofac.GetIsotope("C", 13);
            IIsotope match = isofac.GetIsotope(carbon13.Symbol, carbon13.ExactMass.Value, 2.0);
            Assert.IsNotNull(match);
            Assert.AreEqual(13, match.MassNumber.Value);
        }

        // @cdk.bug 3534288
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNonexistingElement()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IAtom xxAtom = new Atom("Xx");
            isofac.Configure(xxAtom);
        }
    }
}
