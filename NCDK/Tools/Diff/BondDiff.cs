/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools.Diff.Tree
{
    /// <summary>
    /// Compares two <see cref="IBond"/> classes.
    /// </summary>
    // @author     egonw
    // @cdk.module diff
    // @cdk.githash
    public class BondDiff
    {
        /// <summary>
        /// Overwrite the default public constructor because this class is not
        /// supposed to be instantiated.
        /// </summary>
        private BondDiff() { }

        /// <summary>
        /// Compare two <see cref="IChemObject"/> classes and return the difference as a <see cref="string"/>.
        /// </summary>
        /// <param name="first">the first of the two classes to compare</param>
        /// <param name="second">the second of the two classes to compare</param>
        /// <returns>a <see cref="string"/> representation of the difference between the first and second <see cref="IChemObject"/>.</returns>
        public static string Diff(IChemObject first, IChemObject second)
        {
            IDifference diff = Difference(first, second);
            if (diff == null)
            {
                return "";
            }
            else
            {
                return diff.ToString();
            }
        }

        /// <summary>
        /// Compare two <see cref="IChemObject"/> classes and return the difference as an <see cref="IDifference"/>.
        /// </summary>
        /// <param name="first">the first of the two classes to compare</param>
        /// <param name="second">the second of the two classes to compare</param>
        /// <returns>an <see cref="IDifference"/> representation of the difference between the first and second <see cref="IChemObject"/>.</returns>
        public static IDifference Difference(IChemObject first, IChemObject second)
        {
            if (!(first is IBond && second is IBond))
            {
                return null;
            }
            IBond firstB = (IBond)first;
            IBond secondB = (IBond)second;
            IDifferenceList totalDiff = new ChemObjectDifference("BondDiff");
            totalDiff.AddChild(BondOrderDifference.Construct("order", firstB.Order, secondB.Order));
            totalDiff.AddChild(IntegerDifference.Construct("atomCount", firstB.Atoms.Count, secondB.Atoms.Count));
            if (firstB.Atoms.Count == secondB.Atoms.Count)
            {
                for (int i = 0; i < firstB.Atoms.Count; i++)
                {
                    totalDiff.AddChild(AtomDiff.Difference(firstB.Atoms[i], secondB.Atoms[i]));
                }
            }
            totalDiff.AddChild(ElectronContainerDiff.Difference(first, second));
            if (totalDiff.ChildCount() > 0)
            {
                return totalDiff;
            }
            else
            {
                return null;
            }
        }
    }
}
