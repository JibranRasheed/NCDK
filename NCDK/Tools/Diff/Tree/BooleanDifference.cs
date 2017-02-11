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
    /**
     * {@link IDifference} between two {@link bool?}s.
     *
     * @author     egonw
     * @cdk.module diff
     * @cdk.githash
     */
    public class BooleanDifference : IDifference
    {
        private string name;
        private bool? first;
        private bool? second;

        private BooleanDifference(string name, bool? first, bool? second)
        {
            this.name = name;
            this.first = first;
            this.second = second;
        }

        /**
         * Constructs a new {@link IDifference} object.
         *
         * @param name   a name reflecting the nature of the created {@link IDifference}
         * @param first  the first object to compare
         * @param second the second object to compare
         * @return       an {@link IDifference} reflecting the differences between the first and second object
         */
        public static IDifference Construct(string name, bool? first, bool? second)
        {
            if (first == second)
            {
                return null;
            }
            return new BooleanDifference(name, first, second);
        }

        /**
         * Returns a {@link string} representation for this {@link IDifference}.
         *
         * @return a {@link string}
         */
        public override string ToString()
        {
            return name + ":" + (first == null ? "NA" : (first == true ? "T" : "F")) + "/"
                    + (second == null ? "NA" : (second == true ? "T" : "F"));
        }
    }
}
