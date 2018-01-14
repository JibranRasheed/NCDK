/* Copyright (C) 2010  Gilleain Torrance <gilleain.torrance@gmail.com>
 *               2012  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;
using NCDK.Renderers.Colors;
using NCDK.Renderers.Elements;
using NCDK.Validate;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using static NCDK.Renderers.Generators.BasicAtomGenerator;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators
{
    // @cdk.module test-renderextra
    [TestClass()]
    public class ExtendedAtomGeneratorTest : BasicAtomGeneratorTest
    {
        private ExtendedAtomGenerator generator;

        public override Rect? GetCustomCanvas()
        {
            return null;
        }

        public ExtendedAtomGeneratorTest()
            : base()
        {
            this.generator = new ExtendedAtomGenerator();
            base.SetTestedGenerator(generator);
        }

        [TestMethod()]
        public override void GenerateElementTest()
        {
            IAtom atom = base.builder.NewAtom("C");
            atom.Point2D = new Vector2(2, 3);
            atom.ImplicitHydrogenCount = 0;
            int alignment = 1;
            var element = generator.GenerateElement(atom, alignment, model);
            Assert.AreEqual(atom.Point2D.Value.X, element.coord.X, 0.1);
            Assert.AreEqual(atom.Point2D.Value.Y, element.coord.Y, 0.1);
            Assert.AreEqual(atom.Symbol, element.text);
            Assert.AreEqual((int)atom.FormalCharge, element.formalCharge);
            Assert.AreEqual((int)atom.ImplicitHydrogenCount, element.hydrogenCount);
            Assert.AreEqual(alignment, element.alignment);
            Assert.AreEqual(generator.GetAtomColor(atom, model), element.color);
        }

        [TestMethod()]
        public override void HasCoordinatesTest()
        {
            IAtom atomWithCoordinates = base.builder.NewAtom();
            atomWithCoordinates.Point2D = new Vector2(0, 0);
            Assert.IsTrue(generator.HasCoordinates(atomWithCoordinates));

            IAtom atomWithoutCoordinates = base.builder.NewAtom();
            atomWithoutCoordinates.Point2D = null;
            Assert.IsFalse(generator.HasCoordinates(atomWithoutCoordinates));

            IAtom nullAtom = null;
            Assert.IsFalse(generator.HasCoordinates(nullAtom));
        }

        [TestMethod()]
        public override void CanDrawTest()
        {
            IAtom drawableCAtom = base.builder.NewAtom("C");
            drawableCAtom.Point2D = new Vector2(0, 0);

            IAtom drawableHAtom = base.builder.NewAtom("H");
            drawableHAtom.Point2D = new Vector2(0, 0);

            IAtomContainer dummyContainer = base.builder.NewAtomContainer();

            model.SetKekuleStructure(true);
            model.SetShowExplicitHydrogens(true);

            Assert.IsTrue(generator.CanDraw(drawableCAtom, dummyContainer, model));
            Assert.IsTrue(generator.CanDraw(drawableHAtom, dummyContainer, model));
        }

        [TestMethod()]
        public override void InvisibleHydrogenTest()
        {
            IAtom hydrogen = base.builder.NewAtom("H");
            model.SetShowExplicitHydrogens(false);
            Assert.IsTrue(generator.InvisibleHydrogen(hydrogen, model));

            model.SetShowExplicitHydrogens(true);
            Assert.IsFalse(generator.InvisibleHydrogen(hydrogen, model));

            IAtom nonHydrogen = base.builder.NewAtom("C");
            model.SetShowExplicitHydrogens(false);
            Assert.IsFalse(generator.InvisibleHydrogen(nonHydrogen, model));

            model.SetShowExplicitHydrogens(true);
            Assert.IsFalse(generator.InvisibleHydrogen(nonHydrogen, model));
        }

        [TestMethod()]
        public override void InvisibleCarbonTest()
        {
            // NOTE : just testing the element symbol here, see showCarbonTest
            // for the full range of possibilities...
            IAtom carbon = base.builder.NewAtom("C");
            IAtomContainer dummyContainer = base.builder.NewAtomContainer();

            // we force the issue by making isKekule=true
            model.SetKekuleStructure(true);

            Assert.IsFalse(generator.InvisibleCarbon(carbon, dummyContainer, model));
        }

        [TestMethod()]
        public override void ShowCarbon_KekuleTest()
        {
            IAtomContainer atomContainer = base.MakeCCC();
            IAtom carbon = atomContainer.Atoms[1];

            model.SetKekuleStructure(true);
            Assert.IsTrue(generator.ShowCarbon(carbon, atomContainer, model));
        }

        [TestMethod()]
        public override void ShowCarbon_FormalChargeTest()
        {
            IAtomContainer atomContainer = base.MakeCCC();
            IAtom carbon = atomContainer.Atoms[1];

            carbon.FormalCharge = 1;
            Assert.IsTrue(generator.ShowCarbon(carbon, atomContainer, model));
        }

        [TestMethod()]
        public override void ShowCarbon_SingleCarbonTest()
        {
            IAtomContainer atomContainer = base.MakeSingleAtom("C");
            IAtom carbon = atomContainer.Atoms[0];

            Assert.IsTrue(generator.ShowCarbon(carbon, atomContainer, model));
        }

        [TestMethod()]
        public override void ShowCarbon_ShowEndCarbonsTest()
        {
            IAtomContainer atomContainer = base.MakeCCC();
            IAtom carbon = atomContainer.Atoms[0];
            model.SetShowEndCarbons(true);
            Assert.IsTrue(generator.ShowCarbon(carbon, atomContainer, model));
        }

        [TestMethod()]
        public override void ShowCarbon_ErrorMarker()
        {
            IAtomContainer atomContainer = base.MakeCCC();
            IAtom carbon = atomContainer.Atoms[1];
            ProblemMarker.MarkWithError(carbon);
            Assert.IsTrue(generator.ShowCarbon(carbon, atomContainer, model));
        }

        [TestMethod()]
        public override void ShowCarbon_ConnectedSingleElectrons()
        {
            IAtomContainer atomContainer = base.MakeCCC();
            IAtom carbon = atomContainer.Atoms[1];
            atomContainer.AddSingleElectronTo(atomContainer.Atoms[1]);
            Assert.IsTrue(generator.ShowCarbon(carbon, atomContainer, model));
        }

        [TestMethod()]
        public override void OvalShapeTest()
        {
            IAtomContainer singleAtom = MakeSingleAtom();
            model.SetCompactShape(AtomShapeTypes.Oval);
            model.SetCompactAtom(true);
            var elements = GetAllSimpleElements(generator, singleAtom);
            Assert.AreEqual(1, elements.Count);
            Assert.AreEqual(typeof(OvalElement), elements[0].GetType());
        }

        [TestMethod()]
        public override void SquareShapeTest()
        {
            IAtomContainer singleAtom = MakeSingleAtom();
            model.SetCompactShape(AtomShapeTypes.Square);
            model.SetCompactAtom(true);
            var elements = GetAllSimpleElements(generator, singleAtom);
            Assert.AreEqual(1, elements.Count);
            Assert.AreEqual(typeof(RectangleElement), elements[0].GetType());
        }

        [TestMethod()]
        public override void GetAtomColorTest()
        {
            var testColor = WPF::Media.Colors.Red;
            IAtomContainer singleAtom = MakeSingleAtom("O");
            model.SetAtomColor(testColor);
            model.SetAtomColorByType(false);
            generator.GetAtomColor(singleAtom.Atoms[0], model);

            var elements = GetAllSimpleElements(generator, singleAtom);
            Assert.AreEqual(1, elements.Count);
            TextGroupElement element = ((TextGroupElement)elements[0]);
            Assert.AreEqual(testColor, element.color);
        }


        [TestMethod()]
        public override void AtomColorerTest()
        {
            IAtomContainer cnop = MakeSNOPSquare();
            var colorMap = new Dictionary<string, Color>
            {
                ["S"] = WPF::Media.Colors.Yellow,
                ["N"] = WPF::Media.Colors.Blue,
                ["O"] = WPF::Media.Colors.Red,
                ["P"] = WPF::Media.Colors.Magenta
            };
            IAtomColorer atomColorer = new AtomColorerTestIAtomColorer(colorMap);
            model.SetAtomColorer(atomColorer);
            model.SetAtomColorByType(true);
            var elements = GetAllSimpleElements(generator, cnop);
            Assert.AreEqual(4, elements.Count);
            foreach (var element in elements)
            {
                TextGroupElement symbolElement = (TextGroupElement)element;
                string symbol = symbolElement.text;
                Assert.IsTrue(colorMap.ContainsKey(symbol));
                Assert.AreEqual(colorMap[symbol], symbolElement.color);
            }
        }

        [TestMethod()]
        public override void ColorByTypeTest()
        {
            IAtomContainer snop = MakeSNOPSquare();
            model.SetAtomColorByType(false);
            var elements = GetAllSimpleElements(generator, snop);
            Color defaultColor = RenderModelTools.DefaultAtomColor;
            foreach (var element in elements)
            {
                TextGroupElement symbolElement = (TextGroupElement)element;
                Assert.AreEqual(defaultColor, symbolElement.color);
            }
        }

        [TestMethod()]
        public override void ShowExplicitHydrogensTest()
        {
            IAtomContainer methane = MakeMethane();
            // don't generate elements for hydrogens
            model.SetShowExplicitHydrogens(false);
            var carbonOnly = GetAllSimpleElements(generator, methane);
            Assert.AreEqual(1, carbonOnly.Count);

            // do generate elements for hydrogens
            model.SetShowExplicitHydrogens(true);
            var carbonPlusHydrogen = GetAllSimpleElements(generator, methane);
            Assert.AreEqual(5, carbonPlusHydrogen.Count);
        }

        [TestMethod()]
        public override void KekuleTest()
        {
            IAtomContainer singleBond = MakeSingleBond();
            model.SetKekuleStructure(true);
            Assert.AreEqual(2, GetAllSimpleElements(generator, singleBond).Count);
            model.SetKekuleStructure(false);
            Assert.AreEqual(0, GetAllSimpleElements(generator, singleBond).Count);
        }

        [TestMethod()]
        public override void ShowEndCarbonsTest()
        {
            IAtomContainer singleBond = MakeCCC();
            model.SetShowEndCarbons(true);
            Assert.AreEqual(2, GetAllSimpleElements(generator, singleBond).Count);
            model.SetShowEndCarbons(false);
            Assert.AreEqual(0, GetAllSimpleElements(generator, singleBond).Count);
        }

        [TestMethod()]
        public override void TestSingleAtom()
        {
            IAtomContainer singleAtom = MakeSingleAtom();

            // nothing should be made
            IRenderingElement root = generator.Generate(singleAtom, singleAtom.Atoms[0], model);
            var elements = elementUtil.GetAllSimpleElements(root);
            Assert.AreEqual(1, elements.Count);
        }

        [TestMethod()]
        public override void TestSingleBond()
        {
            IAtomContainer container = MakeSingleBond();
            model.SetCompactAtom(true);
            model.SetCompactShape(AtomShapeTypes.Oval);
            model.SetShowEndCarbons(true);

            // generate the single line element
            IRenderingElement root = generator.Generate(container, model);
            var elements = elementUtil.GetAllSimpleElements(root);
            Assert.AreEqual(2, elements.Count);

            // test that the endpoints are distinct
            OvalElement ovalA = (OvalElement)elements[0];
            OvalElement ovalB = (OvalElement)elements[1];
            Assert.AreNotSame(0, Distance(ovalA.coord, ovalB.coord));
        }

        [TestMethod()]
        public override void TestSquare()
        {
            IAtomContainer square = MakeSquare();
            model.SetKekuleStructure(true);

            // generate all four atoms
            IRenderingElement root = generator.Generate(square, model);
            var elements = elementUtil.GetAllSimpleElements(root);
            Assert.AreEqual(4, elements.Count);

            // test that the center is at the origin
            Assert.AreEqual(new Point(0, 0), Center(elements));
        }
    }
}
