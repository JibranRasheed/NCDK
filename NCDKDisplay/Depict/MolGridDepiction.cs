/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
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

using NCDK.Renderers;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Generators;
using NCDK.Renderers.Visitors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NCDK.Depict
{
    /// <summary>
    /// Internal - depicts a set of molecules aligned in a grid. This class
    /// also handles the degenerate case of a single molecule as a 1x1 grid.
    /// </summary>
    sealed class MolGridDepiction 
        : Depiction
    {
        private readonly RendererModel model;
        private readonly Dimensions dimensions;
        private readonly int nCol, nRow;
        private readonly List<Bounds> elements;

        public MolGridDepiction(RendererModel model,
                                List<Bounds> molecules,
                                List<Bounds> titles,
                                Dimensions dimensions,
                                int nRow, int nCol)
                : base(model)
        {
            this.model = model;
            this.dimensions = dimensions;

            this.elements = new List<Bounds>();

            // degenerate case is when no title are provided
            if (!titles.Any())
            {
                elements.AddRange(molecules);
            }
            else
            {
                Trace.Assert(molecules.Count == titles.Count);
                // interweave molecules and titles
                for (int r = 0; r < nRow; r++)
                {
                    int fromIndex = r * nCol;
                    int toIndex = Math.Min(molecules.Count, (r + 1) * nCol);
                    if (fromIndex >= toIndex)
                        break;

                    List<Bounds> molsublist = molecules.GetRange(fromIndex, toIndex - fromIndex);
                    // need to pad list
                    while (molsublist.Count < nCol)
                        molsublist.Add(new Bounds());

                    elements.AddRange(molsublist);
                    elements.AddRange(titles.GetRange(fromIndex, toIndex - fromIndex));
                }
                nRow *= 2;
            }

            this.nCol = nCol;
            this.nRow = nRow;
        }

        public override RenderTargetBitmap ToBitmap()
        {
            // format margins and padding for raster images
            double margin = GetMarginValue(DepictionGenerator.DEFAULT_PX_MARGIN);
            double padding = GetPaddingValue(DefaultPaddingFactor * margin);
            double scale = model.GetV<double>(typeof(BasicSceneGenerator.Scale));
            double zoom = model.GetV<double>(typeof(BasicSceneGenerator.ZoomFactor));

            // row and col offsets for alignment
            double[] yOffset = new double[nRow + 1];
            double[] xOffset = new double[nCol + 1];

            Dimensions required = Dimensions.OfGrid(elements, yOffset, xOffset).Scale(scale * zoom);

            Dimensions total = CalcTotalDimensions(margin, padding, required, null);
            double fitting = CalcFitting(margin, padding, required, null);

            // we use the AWT for vector graphics if though we're raster because
            // fractional strokes can be figured out by interpolation, without
            // when we shrink diagrams bonds can look too bold/chubby
            var dv = new DrawingVisual();
            using (var g2 = dv.RenderOpen())
            {
                IDrawVisitor visitor = WPFDrawVisitor.ForVectorGraphics(g2);

                visitor.SetTransform(new ScaleTransform(1, 1));
                visitor.Visit(new RectangleElement(new Point(0, 0), total.w, total.h, true, model.GetV<Color>(typeof(BasicSceneGenerator.BackgroundColor))));

                // compound the zoom, fitting and scaling into a single value
                double rescale = zoom * fitting * scale;

                // x,y base coordinates include the margin and centering (only if fitting to a size)
                double xBase = margin + (total.w - 2 * margin - (nCol - 1) * padding - (rescale * xOffset[nCol])) / 2;
                double yBase = margin + (total.h - 2 * margin - (nRow - 1) * padding - (rescale * yOffset[nRow])) / 2;

                for (int i = 0; i < elements.Count; i++)
                {
                    int row = i / nCol;
                    int col = i % nCol;

                    // skip empty elements
                    var bounds = this.elements[i];
                    if (bounds.IsEmpty())
                        continue;

                    // calculate the 'view' bounds:
                    //  amount of padding depends on which row or column we are in.
                    //  the width/height of this col/row can be determined by the next offset
                    double x = xBase + col * padding + rescale * xOffset[col];
                    double y = yBase + row * padding + rescale * yOffset[row];
                    double w = rescale * (xOffset[col + 1] - xOffset[col]);
                    double h = rescale * (yOffset[row + 1] - yOffset[row]);

                    Draw(visitor, zoom, bounds, new Rect(x, y, w, h));
                }
            }

            // create the image for rendering
            var img = new RenderTargetBitmap((int)Math.Ceiling(total.w), (int)Math.Ceiling(total.h), 96, 96, PixelFormats.Pbgra32);
            img.Render(dv);
            return img;
        }

        private double CalcFitting(double margin, double padding, Dimensions required, string fmt)
        {
            if (dimensions == Dimensions.AUTOMATIC)
                return 1; // no fitting
            Dimensions targetDim = dimensions;

            // PDF and PS are in point to we need to account for that
            if (PDF_FMT.Equals(fmt) || PS_FMT.Equals(fmt))
                targetDim = targetDim.Scale(M_MMToPoint);

            targetDim = targetDim.Add(-2 * margin, -2 * margin)
                                             .Add(-((nCol - 1) * padding), -((nRow - 1) * padding));
            double resize = Math.Min(targetDim.w / required.w,
                                     targetDim.h / required.h);
            if (resize > 1 && !model.GetV<bool>(typeof(BasicSceneGenerator.FitToScreen)))
                resize = 1;
            return resize;
        }

        private Dimensions CalcTotalDimensions(double margin, double padding, Dimensions required, string fmt)
        {
            if (dimensions == Dimensions.AUTOMATIC)
            {
                return required.Add(2 * margin, 2 * margin)
                               .Add((nCol - 1) * padding, (nRow - 1) * padding);
            }
            else
            {
                // we want all vector graphics dims in MM
                if (PDF_FMT.Equals(fmt) || PS_FMT.Equals(fmt))
                    return dimensions.Scale(M_MMToPoint);
                else
                    return dimensions;
            }
        }

        internal override string ToVecStr(string fmt)
        {
            // format margins and padding for raster images
            double margin = GetMarginValue(DepictionGenerator.DEFAULT_MM_MARGIN);
            double padding = GetPaddingValue(DefaultPaddingFactor * margin);
            double scale = model.GetV<double>(typeof(BasicSceneGenerator.Scale));

            // All vector graphics will be written in mm not px to we need to
            // adjust the size of the molecules accordingly. For now the rescaling
            // is fixed to the bond length proposed by ACS 1996 guidelines (~5mm)
            double zoom = model.GetV<double>(typeof(BasicSceneGenerator.ZoomFactor)) * RescaleForBondLength(Depiction.ACS_1996_BOND_LENGTH_MM);

            // PDF and PS units are in Points (1/72 inch) in FreeHEP so need to adjust for that
            if (fmt.Equals(PDF_FMT) || fmt.Equals(PS_FMT))
            {
                zoom *= M_MMToPoint;
                margin *= M_MMToPoint;
                padding *= M_MMToPoint;
            }

            // row and col offsets for alignment
            double[] yOffset = new double[nRow + 1];
            double[] xOffset = new double[nCol + 1];

            Dimensions required = Dimensions.OfGrid(elements, yOffset, xOffset).Scale(zoom * scale);

            Dimensions total = CalcTotalDimensions(margin, padding, required, fmt);
            double fitting = CalcFitting(margin, padding, required, fmt);

            // create the image for rendering
            FreeHepWrapper wrapper = null;
            if (!fmt.Equals(SVG_FMT))
                wrapper = new FreeHepWrapper(fmt, total.w, total.h);
            IDrawVisitor visitor = fmt.Equals(SVG_FMT) ? (IDrawVisitor)new SvgDrawVisitor(total.w, total.h)
                                                            : (IDrawVisitor)WPFDrawVisitor.ForVectorGraphics(wrapper.g2);

            if (fmt.Equals(SVG_FMT))
            {
                SvgPrevisit(fmt, scale * zoom * fitting, (SvgDrawVisitor)visitor, elements);
            }
            else
            {
            }

            visitor.SetTransform(new ScaleTransform(1, -1));
            visitor.Visit(new RectangleElement(new Point(0, -total.h), total.w, total.h, true, model.GetV<Color>(typeof(BasicSceneGenerator.BackgroundColor))));

            // compound the fitting and scaling into a single value
            double rescale = zoom * fitting * scale;

            // x,y base coordinates include the margin and centering (only if fitting to a size)
            double xBase = margin + (total.w - 2 * margin - (nCol - 1) * padding - (rescale * xOffset[nCol])) / 2;
            double yBase = margin + (total.h - 2 * margin - (nRow - 1) * padding - (rescale * yOffset[nRow])) / 2;

            for (int i = 0; i < elements.Count; i++)
            {
                int row = i / nCol;
                int col = i % nCol;

                // calculate the 'view' bounds:
                //  amount of padding depends on which row or column we are in.
                //  the width/height of this col/row can be determined by the next offset
                double x = xBase + col * padding + rescale * xOffset[col];
                double y = yBase + row * padding + rescale * yOffset[row];
                double w = rescale * (xOffset[col + 1] - xOffset[col]);
                double h = rescale * (yOffset[row + 1] - yOffset[row]);

                Draw(visitor, zoom, elements[i], new Rect(x, y, w, h));
            }

            if (wrapper != null)
            {
                wrapper.Dispose();
                return wrapper.ToString();
            }
            else
            {
                return visitor.ToString();
            }
        }
    }
}
