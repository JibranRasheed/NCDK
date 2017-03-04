/* 
 * Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
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
 *
 */
namespace NCDK.Validate
{
    /// <summary>
    /// Tool to mark IChemObject's as having a problem. There are two levels:
    /// a problem, and a warning, to allow for different coloring by renderer's.
    /// </summary>
    // @cdk.module standard
    // @cdk.githash
    // @author   Egon Willighagen
    // @cdk.created  2003-08-11
    public class ProblemMarker
    {
        public const string ErrorMarker = "NCDK.Validate.error";
        public const string WarningMarker = "NCDK.Validate.warning";

        public static void MarkWithError(IChemObject obj)
        {
            obj.SetProperty(ErrorMarker, true);
        }

        public static void MarkWithWarning(IChemObject obj)
        {
            obj.SetProperty(WarningMarker, true);
        }

        public static void UnMarkWithError(IChemObject obj)
        {
            obj.RemoveProperty(ErrorMarker);
        }

        public static void UnMarkWithWarning(IChemObject obj)
        {
            obj.RemoveProperty(WarningMarker);
        }

        public static void Unmark(IChemObject obj)
        {
            UnMarkWithWarning(obj);
            UnMarkWithError(obj);
        }
    }
}
