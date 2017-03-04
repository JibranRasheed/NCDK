/*  Copyright (C) 2009  Stefan Kuhn <shk3@users.sf.net>
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
using NCDK.Renderers.Elements;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using static NCDK.Renderers.Generators.BasicSceneGenerator;
using static NCDK.Renderers.Generators.ReactionSceneGenerator;

namespace NCDK.Renderers.Generators
{
    /// <summary>
    /// Generate the symbols for radicals.
    /// </summary>
    // @author maclean
    // @cdk.module renderextra
    // @cdk.githash
    public class ReactionBoxGenerator : IGenerator<IReaction>
    {
        /// <inheritdoc/>
        public IRenderingElement Generate(IReaction reaction, RendererModel model)
        {
            if (!model.GetV<bool>(typeof(ShowReactionBoxes))) return null;
            double separation = model.GetV<double>(typeof(BondLength)) / model.GetV<double>(typeof(Scale));
            var totalBounds = BoundsCalculator.CalculateBounds(reaction);
            if (totalBounds == null) return null;

            ElementGroup diagram = new ElementGroup();
            var foregroundColor = model.GetV<Color>(typeof(BasicSceneGenerator.ForegroundColor));
            diagram.Add(new RectangleElement(new Rect(totalBounds.Value.Left - separation, totalBounds.Value.Top - separation,
                    totalBounds.Value.Right + separation, totalBounds.Value.Bottom + separation), foregroundColor));
            if (reaction.Id != null)
            {
                diagram.Add(new TextElement(
                    new Point((totalBounds.Value.Left + totalBounds.Value.Right) / 2, totalBounds.Value.Top - separation),
                    reaction.Id, foregroundColor));
            }
            return diagram;
        }

        /// <inheritdoc/>
        public IList<IGeneratorParameter> Parameters => Array.Empty<IGeneratorParameter>();
    }
}
