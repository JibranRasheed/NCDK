/* Copyright (C) 2008  Arvid Berg <goglepox@users.sf.net>
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
using System.Windows;
using System.Windows.Media;

namespace NCDK.Renderers.Elements
{
    /// <summary>
    /// An oval element (should) have both a width and a height.
    /// </summary>
    // @cdk.module renderbasic
    // @cdk.githash
    public class OvalElement : IRenderingElement
    {
        /// <summary>The center of the oval. </summary>
        public readonly Point coord;

        /// <summary>The radius of the oval. </summary>
        public readonly double radius; // TODO : width AND height

        /// <summary>If true, draw the oval as filled. </summary>
        public readonly bool fill;

        /// <summary>The color to draw the oval. </summary>
        public readonly Color color;

        /// <summary>
        /// Make an oval with a default radius of 10.
        /// </summary>
        /// <param name="xCoord">the x-coordinate of the center of the oval</param>
        /// <param name="yCoord">the y-coordinate of the center of the oval</param>
        /// <param name="color">the color of the oval</param>
        public OvalElement(Point coord, Color color)
            : this(coord, 10, color)
        { }

        /// <summary>
        /// Make an oval with the supplied radius.
        /// </summary>
        /// <param name="coord">the coordinate of the center of the oval</param>
        /// <param name="radius">the radius of the oval</param>
        /// <param name="color">the color of the oval</param>
        public OvalElement(Point coord, double radius, Color color)
            : this(coord, radius, true, color)
        { }

        /// <summary>
        /// Make an oval with a particular fill and color.
        /// </summary>
        /// <param name="coord">the coordinate of the center of the oval</param>
        /// <param name="radius">the radius of the oval</param>
        /// <param name="fill">if true, fill the oval when drawing</param>
        /// <param name="color">the color of the oval</param>
        public OvalElement(Point coord, double radius, bool fill, Color color)
        {
            this.coord = coord;
            this.radius = radius;
            this.fill = fill;
            this.color = color;
        }

        /// <inheritdoc/>
        public virtual void Accept(IRenderingVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
