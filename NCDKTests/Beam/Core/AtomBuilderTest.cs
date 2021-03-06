using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static NCDK.Beam.Element;

namespace NCDK.Beam
{
   /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class AtomBuilderTest
    {
        [TestMethod()]
        public void Aliphatic_element_c()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), false);
        }

        [TestMethod()]
        public void Aliphatic_element_n()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Nitrogen)
                                .Build();
            Assert.AreEqual(a.Element, Element.Nitrogen);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), false);
        }

        [TestMethod()]
        public void Aliphatic_element_null()
        {
            try
            {
                IAtom a = AtomBuilder.Aliphatic((Element)null).Build();
                Assert.Fail();
            }
            catch (ArgumentNullException)
            { }
        }

        [TestMethod()]
        public void IsAromatic_element_c()
        {
            IAtom a = AtomBuilder.Aromatic(Carbon).Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), true);
        }

        [TestMethod()]
        public void IsAromatic_element_n()
        {
            IAtom a = AtomBuilder.Aromatic(Nitrogen)
                                .Build();
            Assert.AreEqual(a.Element, Element.Nitrogen);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), true);
        }

        [TestMethod()]
        public void IsAromatic_element_cl()
        {
            try
            {
                IAtom a = AtomBuilder.Aromatic(Chlorine).Build();
                Assert.Fail();
            }
            catch (ArgumentException)
            { }
        }

        [TestMethod()]
        public void IsAromatic_element_null()
        {
            try
            {
                IAtom a = AtomBuilder.Aromatic((Element)null).Build();
                Assert.Fail();
            }
            catch (ArgumentNullException)
            { }
        }

        [TestMethod()]
        public void Aliphatic_symbol_c()
        {
            IAtom a = AtomBuilder.Aliphatic("C").Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), false);
        }

        [TestMethod()]
        public void Aliphatic_symbol_n()
        {
            IAtom a = AtomBuilder.Aliphatic("N").Build();
            Assert.AreEqual(a.Element, Element.Nitrogen);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), false);
        }

        [TestMethod()]
        public void Aliphatic_symbol_null()
        {
            try
            {
                IAtom a = AtomBuilder.Aliphatic((string)null).Build();
                Assert.Fail();
            }
            catch (ArgumentNullException)
            { }
        }

        [TestMethod()]
        public void IsAromatic_symbol_c()
        {
            IAtom a = AtomBuilder.Aromatic("C").Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), true);
        }

        [TestMethod()]
        public void IsAromatic_symbol_n()
        {
            IAtom a = AtomBuilder.Aromatic("N")
                                .Build();
            Assert.AreEqual(a.Element, Element.Nitrogen);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), true);
        }

        [TestMethod()]
        public void IsAromatic_symbol_cl()
        {
            try
            {
                IAtom a = AtomBuilder.Aromatic("Cl").Build();
                Assert.Fail();
            }
            catch (ArgumentException)
            { }
        }

        [TestMethod()]
        public void IsAromatic_symbol_null()
        {
            try
            {
                IAtom a = AtomBuilder.Aromatic((string)null).Build();
                Assert.Fail();
            }
            catch (ArgumentNullException)
            { }
        }

        [TestMethod()]
        public void Create_symbol_aliphatic_c()
        {
            IAtom a = AtomBuilder.Create("C").Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), false);
        }

        [TestMethod()]
        public void Create_symbol_IsAromatic_c()
        {
            IAtom a = AtomBuilder.Create("c")
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), true);
        }

        [TestMethod()]
        public void Create_symbol_non_IsAromatic()
        {
            try
            {
                IAtom a = AtomBuilder.Create("cl").Build();
                Assert.Fail();
            }
            catch (ArgumentException)
            { }
        }

        [TestMethod()]
        public void Create_symbol_defaultToUnknown()
        {
            IAtom a = AtomBuilder.Create("N/A")
                                .Build();
            Assert.AreEqual(a.Element, Element.Unknown);
        }

        [TestMethod()]
        public void Create_symbol_null()
        {
            IAtom a = AtomBuilder.Create((string)null)
                                .Build();
            Assert.AreEqual(a.Element, Element.Unknown);
        }

        [TestMethod()]
        public void Aliphatic_charged_carbon_minus2()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Charge(-2)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Charge, -2);
        }

        [TestMethod()]
        public void Aliphatic_charged_carbon_plus2()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Charge(+2)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Charge, +2);
        }

        [TestMethod()]
        public void Aliphatic_charged_carbon_anion()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Anion
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Charge, -1);
        }

        [TestMethod()]
        public void Aliphatic_charged_carbon_plus1()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Cation
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Charge, +1);
        }

        [TestMethod()]
        public void Aliphatic_carbon_13()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Isotope(13)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, 13);
        }

        [TestMethod()]
        public void Aliphatic_carbon_14()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Isotope(13)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, 13);
        }

        [TestMethod()]
        public void Aliphatic_carbon_class1()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .AtomClass(1)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.AtomClass, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Aliphatic_carbon_class_negative()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .AtomClass(-10)
                                .Build();
        }

        [TestMethod()]
        public void Aliphatic_carbon_3_hydrogens()
        {
            IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .NumOfHydrogens(3)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.NumOfHydrogens, 3);
        }

        [TestMethod()]
        public void Aliphatic_carbon_negative_hydrogens()
        {
            try
            {
                IAtom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .NumOfHydrogens(-3)
                                .Build();
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException)
            { }
        }

        [TestMethod()]
        public void IsAromatic_Unknown_from_element()
        {
            Assert.IsNotNull(AtomBuilder.Aromatic(Unknown).Build());
        }

        [TestMethod()]
        public void IsAromatic_Unknown_from_symbol()
        {
            Assert.IsNotNull(AtomBuilder.Aromatic("*").Build());
            Assert.IsNotNull(AtomBuilder.Aromatic("R").Build());
        }
    }
}
