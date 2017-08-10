/* Copyright (C) 2008 Arvid Berg <goglepox@users.sf.net>
 * Contact: cdk-devel@list.sourceforge.net
 * This program
 * is free software; you can redistribute it and/or modify it under the terms of
 * the GNU Lesser General Public License as published by the Free Software
 * Foundation; either version 2.1 of the License, or (at your option) any later
 * version. This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details. You should have received a copy of the GNU Lesser General
 * Public License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System.Windows;
using System.Windows.Media;

namespace NCDK.Renderers.Elements
{
    /// <summary>
    /// A line between two points.
    /// </summary>
    // @cdk.module renderbasic
    // @cdk.githash
    public class LineElement : IRenderingElement
    {
        /// <summary>The first point. </summary>
        public readonly Point firstPoint;

        /// <summary>The second point. </summary>
        public readonly Point secondPoint;

        /// <summary>The width of the line. </summary>
        public readonly double width;

        /// <summary>The color of the line. </summary>
        public readonly Color color;

        /// <summary>
        /// Make a line element.
        /// </summary>
        /// <param name="firstPoint">coordinate of the first point</param>
        /// <param name="secondPoint">coordinate of the second point</param>
        /// <param name="width">the width of the line</param>
        /// <param name="color">the color of the line</param>
        public LineElement(Point firstPoint, Point secondPoint, double width, Color color)
        {
            this.firstPoint = firstPoint;
            this.secondPoint = secondPoint;
            this.width = width;
            this.color = color;
        }

        /// <inheritdoc/>
        public virtual void Accept(IRenderingVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// The type of the line.
        /// </summary>
        public struct LineType
        {
            public static readonly LineType Single = new LineType(1);
            public static readonly LineType Double = new LineType(2);
            public static readonly LineType Triple = new LineType(3);
            public static readonly LineType Quadruple = new LineType(4);

            int n;

            private LineType(int n)
            {
                this.n = n;
            }

            /// <summary>
            /// Returns the count for this line type.
            /// </summary>
            /// <returns>the count for this line type.</returns>
            public int Count => n;
        }
    }
}
