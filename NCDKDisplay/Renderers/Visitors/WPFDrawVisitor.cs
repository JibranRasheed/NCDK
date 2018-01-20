/* Copyright (C) 2008 Gilleain Torrance <gilleain.torrance@gmail.com>
 *
 *  Contact: cdk-devel@list.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Numerics;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Fonts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using static NCDK.Renderers.Generators.Standards.VecmathUtil;
using WPF = System.Windows;

namespace NCDK.Renderers.Visitors
{
    /// <summary>
    /// Implementation of the <see cref="IDrawVisitor"/> interface for the WPF,
    /// allowing molecules to be rendered with toolkits based on WPF.
    /// </summary>
    // @cdk.module renderawt
    // @cdk.githash
    public class WPFDrawVisitor : AbstractWPFDrawVisitor
    {
        /// <summary>
        /// The font manager cannot be set by the constructor as it needs to
        /// be managed by the Renderer.
        /// </summary>
        private WPFFontManager fontManager;

        /// <summary>
        /// The renderer model cannot be set by the constructor as it needs to
        /// be managed by the Renderer.
        /// </summary>
        private RendererModel rendererModel;

        /// <summary>
        /// The current stroke map.
        /// </summary>
        public IDictionary<int, Pen> StrokeMap { get; } = new Dictionary<int, Pen>();

        /// <inheritdoc/>
        public override RendererModel RendererModel
        {
            get => rendererModel;
            set => rendererModel = value;
        }

        private readonly double minStroke;
        private readonly bool strokeCache;

        /// <summary>
        /// The <see cref="DrawingContext"/> for this visitor.
        /// </summary>
        private readonly DrawingContext graphics;

        /// <summary>
        /// The <see cref="DrawingContext"/> for this visitor. 
        /// </summary>
        public DrawingContext Graphics => graphics;

        /// <summary>
        /// Constructs a new <see cref="IDrawVisitor"/>
        /// taking a <see cref="DrawingContext"/> object to which the chemical content
        /// is drawn.
        /// </summary>
        /// <param name="graphics"><see cref="DrawingContext"/> to which will be drawn</param>
        public WPFDrawVisitor(DrawingContext graphics)
            : this(graphics, true, 1.5)
        { }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="graphics">the graphics instance</param>
        /// <param name="strokeCache">cache strokes internally, only sizes at 0.25 increments are stored</param>
        /// <param name="minStroke">the minimum stroke, strokes smaller than this are automatically resized</param>
        private WPFDrawVisitor(DrawingContext graphics, bool strokeCache, double minStroke)
        {
            this.graphics = graphics;
            this.fontManager = null;
            this.rendererModel = null;
            this.strokeCache = strokeCache;
            this.minStroke = minStroke;
        }

        /// <summary>
        /// Create a draw visitor that will be rendering to a vector graphics output. This disables
        /// the minimum stroke size and stroke caching when drawing lines.
        /// </summary>
        /// <param name="g2">graphics environment</param>
        /// <returns>draw visitor</returns>
        public static WPFDrawVisitor ForVectorGraphics(DrawingContext g2)
        {
            return new WPFDrawVisitor(g2, false, double.NegativeInfinity);
        }

        private void Visit(ElementGroup elementGroup)
        {
            elementGroup.Visit(this);
        }

        private void Visit(LineElement line)
        {
            // scale the stroke by zoom + scale (both included in the AffineTransform)
            var width = line.Width;
            if (width < minStroke) width = minStroke;

            int key = (int)(width * 4); // store 2.25, 2.5, 2.75 etc to separate keys

            if (strokeCache && StrokeMap.TryGetValue(key, out Pen pen))
            {
                // do nothing
            }
            else
            {
                Pen stroke = new Pen(new SolidColorBrush(line.Color), width);
                pen = stroke;
                StrokeMap[key] = stroke;
            }

            var linePoints = new WPF::Point[] { line.FirstPoint, line.SecondPoint };

            linePoints[0] = linePoints[0];
            linePoints[1] = linePoints[1];
            graphics.DrawLine(pen, linePoints[0], linePoints[1]);
        }

        private void Visit(OvalElement oval)
        {
            var radius = oval.Radius;
            var diameter = oval.Radius * 2;
            var center = oval.Coord;
            var brush = new SolidColorBrush(oval.Color);

            if (oval.Fill)
            {
                this.graphics.DrawEllipse(
                    brush,
                    null,
                    new WPF.Point(center.X - radius, center.Y - radius), diameter, diameter);
            }
            else
            {
                this.graphics.DrawEllipse(
                    null,
                    new Pen(brush, 1),
                    new WPF.Point(center.X - radius, center.Y - radius), diameter, diameter);
            }
        }

        private Color BackgroundColor
        {
            get
            {
                return rendererModel == null ?
                    RendererModelTools.DefaultBackgroundColor :
                    rendererModel.GetBackgroundColor();
            }
        }

        private void Visit(TextElement textElement)
        {
            var point = this.GetTextBasePoint(textElement.Text, textElement.Coord, this.fontManager.Typeface, this.fontManager.Size);
            var textBounds = this.GetTextBounds(textElement.Text, textElement.Coord, this.fontManager.Typeface, this.fontManager.Size);
            var backColor = this.BackgroundColor;
            this.graphics.DrawRectangle(new SolidColorBrush(backColor), null, textBounds);
            this.graphics.DrawText(new FormattedText(
                textElement.Text,
                CultureInfo.InvariantCulture,
                WPF.FlowDirection.LeftToRight,
                this.fontManager.CureentTypeface,
                this.fontManager.Size,
                new SolidColorBrush(textElement.Color)), point);
        }

        private void Visit(WedgeLineElement wedge)
        {
            // make the vector normal to the wedge axis
            var normal = new Vector2(wedge.FirstPoint.Y - wedge.SecondPoint.Y, wedge.SecondPoint.X - wedge.FirstPoint.X);
            normal = Vector2.Normalize(normal);
            normal *= (rendererModel.GetWedgeWidth() / rendererModel.GetScale());

            // make the triangle corners
            var vertexA = new Vector2(wedge.FirstPoint.X, wedge.FirstPoint.Y);
            var vertexB = new Vector2(wedge.SecondPoint.X, wedge.SecondPoint.Y) + normal;
            var vertexC = vertexB - normal;

            var brush = new SolidColorBrush(wedge.Color);

            if (wedge.BondType == WedgeLineElement.WedgeType.Dashed)
            {
                var pen = new Pen(brush, 1);

                // calculate the distances between lines
                double distance = Vector2.Distance(vertexB, vertexA);
                double gapFactor = 0.1;
                double gap = distance * gapFactor;
                double numberOfDashes = distance / gap;
                double displacement = 0;

                // draw by interpolating along the edges of the triangle
                for (int i = 0; i < numberOfDashes; i++)
                {
                    Vector2 point1 = Vector2.Lerp(vertexA, vertexB, displacement);
                    Vector2 point2 = Vector2.Lerp(vertexA, vertexC, displacement);
                    var p1T = ToPoint(point1);
                    var p2T = ToPoint(point2);
                    this.graphics.DrawLine(pen, p1T, p2T);
                    if (distance * (displacement + gapFactor) >= distance)
                    {
                        break;
                    }
                    else
                    {
                        displacement += gapFactor;
                    }
                }
            }
            else if (wedge.BondType == WedgeLineElement.WedgeType.Wedged)
            {
                var pointB = ToPoint(vertexB);
                var pointC = ToPoint(vertexC);
                var pointA = ToPoint(vertexA);

                var figure = new PathFigure
                {
                    StartPoint = pointB
                };
                figure.Segments.Add(new LineSegment(pointC, false));
                figure.Segments.Add(new LineSegment(pointA, false));
                var g = new PathGeometry(new[] { figure });
                this.graphics.DrawGeometry(brush, null, g);
            }
            else if (wedge.BondType == WedgeLineElement.WedgeType.Indiff)
            {
                var pen = new Pen(brush, 1)
                {
                    StartLineCap = PenLineCap.Round,
                    LineJoin = PenLineJoin.Round,
                    EndLineCap = PenLineCap.Round
                };

                // calculate the distances between lines
                double distance = Vector2.Distance(vertexB, vertexA);
                double gapFactor = 0.05;
                double gap = distance * gapFactor;
                double numberOfDashes = distance / gap;
                double displacement = 0;

                // draw by interpolating along the edges of the triangle
                Vector2 point1 = Vector2.Lerp(vertexA, vertexB, displacement);
                bool flip = false;
                var p1T = ToPoint(point1);
                displacement += gapFactor;
                for (int i = 0; i < numberOfDashes; i++)
                {
                    Vector2 point2;
                    if (flip)
                    {
                        point2 = Vector2.Lerp(vertexA, vertexC, displacement);
                    }
                    else
                    {
                        point2 = Vector2.Lerp(vertexA, vertexB, displacement);
                    }
                    flip = !flip;
                    var p2T = ToPoint(point2);
                    this.graphics.DrawLine(pen, p1T, p2T);
                    if (distance * (displacement + gapFactor) >= distance)
                    {
                        break;
                    }
                    else
                    {
                        p1T = p2T;
                        displacement += gapFactor;
                    }
                }
            }
        }

        private void Visit(AtomSymbolElement atomSymbol)
        {
            var xy = atomSymbol.Coord;

            var bounds = GetTextBounds(atomSymbol.Text, this.fontManager.CureentTypeface, this.fontManager.Size);

            double w = bounds.Width;
            double h = bounds.Height;

            double xOffset = bounds.X;
            double yOffset = bounds.Y + bounds.Height;

            bounds = new WPF.Rect(xy.X - (w / 2), xy.Y - (h / 2), w, h);

            var backgroundBrush = new SolidColorBrush(this.BackgroundColor);
            var atomSymbolBrush = new SolidColorBrush(atomSymbol.Color);

            double padding = h / 4;
            this.graphics.DrawRoundedRectangle(
                backgroundBrush, null,
                new WPF::Rect(
                    bounds.X - (padding / 2), bounds.Y - (padding / 2),
                    bounds.Width + padding, bounds.Height + padding), padding, padding);
            this.graphics.DrawText(new FormattedText(
                    atomSymbol.Text,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    atomSymbolBrush),
                    new WPF::Point(bounds.X - xOffset, bounds.Y + h - yOffset));

            int offset = 10; // XXX
            string chargeString;
            if (atomSymbol.FormalCharge == 0)
            {
                return;
            }
            else if (atomSymbol.FormalCharge == 1)
            {
                chargeString = "+";
            }
            else if (atomSymbol.FormalCharge > 1)
            {
                chargeString = atomSymbol.FormalCharge + "+";
            }
            else if (atomSymbol.FormalCharge == -1)
            {
                chargeString = "-";
            }
            else if (atomSymbol.FormalCharge < -1)
            {
                int absCharge = Math.Abs(atomSymbol.FormalCharge);
                chargeString = absCharge + "-";
            }
            else
            {
                return;
            }

            var xCoord = bounds.CenterX();
            var yCoord = bounds.CenterY();
            if (atomSymbol.Alignment == 1)
            { // RIGHT
                this.graphics.DrawText(new FormattedText(
                    chargeString,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    atomSymbolBrush),
                    new WPF::Point(xCoord + offset, bounds.Top));
            }
            else if (atomSymbol.Alignment == -1)
            { // LEFT
                this.graphics.DrawText(new FormattedText(
                    chargeString,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    atomSymbolBrush),
                    new WPF::Point(xCoord - offset, bounds.Top));
            }
            else if (atomSymbol.Alignment == 2)
            { // TOP
                this.graphics.DrawText(new FormattedText(
                    chargeString,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    atomSymbolBrush),
                    new WPF::Point(xCoord, yCoord - offset));
            }
            else if (atomSymbol.Alignment == -2)
            { // BOT
                this.graphics.DrawText(new FormattedText(
                    chargeString,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    atomSymbolBrush),
                    new WPF::Point(xCoord, yCoord + offset));
            }
        }

        private void Visit(RectangleElement rectangle)
        {
            var width = rectangle.Width;
            var height = rectangle.Height;
            var p = rectangle.Coord;
            var rect = new WPF.Rect(p.X, p.Y, width, height);
            if (rectangle.Filled)
            {
                this.graphics.DrawRectangle(new SolidColorBrush(rectangle.Color), null, rect);
            }
            else
            {
                this.graphics.DrawRectangle(null, new Pen(new SolidColorBrush(rectangle.Color), 1), rect);
            }
        }

        private void Visit(GeneralPath path)
        {
            if (path.Fill)
            {
                this.graphics.DrawGeometry(
                    new SolidColorBrush(path.Color),
                    null,
                    path.Elements);
            }
            else
            {
                var pen = new Pen(new SolidColorBrush(path.Color), path.StrokeWith);
                this.graphics.DrawGeometry(null, pen, path.Elements);
            }
        }

        private void Visit(ArrowElement line)
        {
            double scale = rendererModel.GetScale();

            Pen pen = null;
            {
                int w = (int)(line.Width * scale);
                if (StrokeMap.ContainsKey(w))
                {
                    pen = StrokeMap[w];
                }
                else
                {
                    pen = new Pen(new SolidColorBrush(line.Color), w);
                    StrokeMap[w] = pen;
                }
            }

            var a = line.Start;
            var b = line.End;
            graphics.DrawLine(pen, a, b);
            double aW = rendererModel.GetArrowHeadWidth() / scale;
            if (line.Direction)
            {
                var c = new WPF.Point(line.Start.X - aW, line.Start.Y - aW);
                var d = new WPF.Point(line.Start.X - aW, line.Start.Y + aW);
                graphics.DrawLine(pen, a, c);
                graphics.DrawLine(pen, a, d);
            }
            else
            {
                var c = new WPF.Point(line.End.X + aW, line.End.Y - aW);
                var d = new WPF.Point(line.End.X + aW, line.End.Y + aW);
                graphics.DrawLine(pen, b, c);
                graphics.DrawLine(pen, b, d);
            }
        }

        private void Visit(TextGroupElement textGroup)
        {
            var point = GetTextBasePoint(textGroup.Text, textGroup.Coord, fontManager.CureentTypeface, fontManager.Size);
            var textBounds = GetTextBounds(textGroup.Text, textGroup.Coord, fontManager.CureentTypeface, fontManager.Size);
            this.graphics.DrawRectangle(new SolidColorBrush(this.BackgroundColor), null, textBounds);
            this.graphics.DrawText(new FormattedText(
                textGroup.Text,
                CultureInfo.CurrentCulture,
                WPF.FlowDirection.LeftToRight,
                this.fontManager.CureentTypeface,
                this.fontManager.Size,
                new SolidColorBrush(textGroup.Color)),
                new WPF::Point(point.X, point.Y));

            var coord = new WPF::Point(textBounds.CenterX(), textBounds.CenterY());
            var coord1 = textBounds.TopLeft;
            var coord2 = new WPF::Point(point.X + textBounds.Width, textBounds.Bottom);

            var vo = coord2 - coord1;
            var o = new WPF::Size(vo.X, vo.Y);
            foreach (var child in textGroup.children)
            {
                WPF::Point p;

                switch (child.position)
                {
                    case TextGroupElement.Position.NE:
                        p = new WPF::Point(coord2.X, coord1.Y);
                        break;
                    case TextGroupElement.Position.N:
                        p = new WPF::Point(coord1.X, coord1.Y);
                        break;
                    case TextGroupElement.Position.NW:
                        p = new WPF::Point(coord1.X - o.Width, coord1.Y);
                        break;
                    case TextGroupElement.Position.W:
                        p = new WPF::Point(coord1.X - o.Width, coord.Y);
                        break;
                    case TextGroupElement.Position.SW:
                        p = new WPF::Point(coord1.X - o.Width, coord1.Y + o.Height);
                        break;
                    case TextGroupElement.Position.S:
                        p = new WPF::Point(coord1.X, coord2.Y + o.Height);
                        break;
                    case TextGroupElement.Position.SE:
                        p = new WPF::Point(coord2.X, coord2.Y + o.Height);
                        break;
                    case TextGroupElement.Position.E:
                        p = new WPF::Point(coord2.X, coord.Y);
                        break;
                    default:
                        p = new WPF::Point(coord.X, coord.Y);
                        break;
                }

                this.graphics.DrawText(new FormattedText(
                    child.text,
                    CultureInfo.InvariantCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    new SolidColorBrush(textGroup.Color)),
                    p);

                if (child.subscript != null)
                {
                    var childBounds = GetTextBounds(child.text, p, fontManager.CureentTypeface, fontManager.Size);
                    var scx = p.X + (childBounds.Width * 0.75);
                    var scy = p.Y + (childBounds.Height / 3);
                    this.graphics.DrawText(new FormattedText(
                        child.subscript,
                        CultureInfo.InvariantCulture,
                        WPF.FlowDirection.LeftToRight,
                        this.fontManager.CureentTypeface,
                        this.fontManager.Size - 2,
                        new SolidColorBrush(textGroup.Color)),
                        p);
                }
            }
        }

        /// <inheritdoc/>
        public override void Visit(IRenderingElement element, Transform transform)
        {
            this.graphics.PushTransform(transform);
            Visit(element);
            this.graphics.Pop();
        }

        /// <inheritdoc/>
        public override void Visit(IRenderingElement element)
        {
            switch (element)
            {
                case ElementGroup e:
                    Visit(e);
                    break;
                case WedgeLineElement e:
                    Visit(e);
                    break;
                case LineElement e:
                    Visit(e);
                    break;
                case OvalElement e:
                    Visit(e);
                    break;
                case TextGroupElement e:
                    Visit(e);
                    break;
                case AtomSymbolElement e:
                    Visit(e);
                    break;
                case TextElement e:
                    Visit(e);
                    break;
                case RectangleElement e:
                    Visit(e);
                    break;
                case GeneralPath e:
                    Visit(e);
                    break;
                case ArrowElement e:
                    Visit(e);
                    break;
                case Bounds e:
                    Visit(e.Root);
                    break;
                case MarkedElement e:
                    Visit(e.Element());
                    break;
                default:
                    Console.Error.WriteLine($"Visitor method for {element.GetType()} is not implemented.");
                    break;
            }
        }

        public override IFontManager FontManager
        {
            get => this.fontManager;
            set => this.fontManager = (WPFFontManager)value;
        }
    }
}
