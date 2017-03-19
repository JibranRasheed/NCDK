/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */

namespace NCDK
{
    /// <summary>
    /// Represents the concept of a stereo element in the molecule. Stereo elements can be
    /// that of quadrivalent atoms, cis/trans isomerism around double bonds, but also include
    /// axial and helical stereochemistry.
    /// </summary>
    // @cdk.module interfaces
    // @cdk.githash
    // @author      egonw
    // @cdk.keyword stereochemistry
    public interface IStereoElement
        : ICDKObject
    {
        /// <summary>
        /// Does the stereo element contain the provided atom.
        /// </summary>
        /// <param name="atom">an atom to test membership</param>
        /// <returns>whether the atom is present</returns>
        bool Contains(IAtom atom);
    }
}
