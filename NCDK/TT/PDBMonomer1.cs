
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

/* Copyright (C) 2005-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Default;
using System.Text;

namespace NCDK.Default
{
    /// <summary>
    /// Represents the idea of an monomer as used in PDB files. It contains extra fields
    /// normally associated with atoms in such files.
    ///
    // @cdk.module data
    // @cdk.githash
    ///
    // @see  PDBAtom
    /// </summary>
    public class PDBMonomer : Monomer, ICloneable, IPDBMonomer
    {
        public string ICode { get; set; }

        /// <summary>
        /// Denotes which chain in the PDB file this monomer is in.
        /// </summary>
        public string ChainID { get; set; }

        /// <summary>
        /// Denotes which residue sequence in the current chain that this monomer is in.
        /// </summary>
        public string ResSeq { get; set; }

        public PDBMonomer()
            : base()
        {
            InitValues();
        }

        private void InitValues()
        {
            ICode = null;
            ChainID = null;
        }

        /// <summary>
        /// Returns a one line string representation of this Atom.
        /// Methods is conform RFC #9.
        ///
        /// <returns>The string representation of this Atom</returns>
        /// </summary>

        public override string ToString()
        {
            StringBuilder description = new StringBuilder(50);
            description.Append("PDBMonomer(");
            description.Append(this.GetHashCode());
            description.Append(", iCode=").Append(ICode);
            description.Append(", chainID=").Append(ChainID).Append(", ");
            description.Append(base.ToString());
            description.Append(')');
            return description.ToString();
        }

        /// <summary>
        // @inheritDoc
        /// </summary>
        public override object Clone()
        {
            return (IPDBMonomer)base.Clone();
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Represents the idea of an monomer as used in PDB files. It contains extra fields
    /// normally associated with atoms in such files.
    ///
    // @cdk.module data
    // @cdk.githash
    ///
    // @see  PDBAtom
    /// </summary>
    public class PDBMonomer : Monomer, ICloneable, IPDBMonomer
    {
        public string ICode { get; set; }

        /// <summary>
        /// Denotes which chain in the PDB file this monomer is in.
        /// </summary>
        public string ChainID { get; set; }

        /// <summary>
        /// Denotes which residue sequence in the current chain that this monomer is in.
        /// </summary>
        public string ResSeq { get; set; }

        public PDBMonomer()
            : base()
        {
            InitValues();
        }

        private void InitValues()
        {
            ICode = null;
            ChainID = null;
        }

        /// <summary>
        /// Returns a one line string representation of this Atom.
        /// Methods is conform RFC #9.
        ///
        /// <returns>The string representation of this Atom</returns>
        /// </summary>

        public override string ToString()
        {
            StringBuilder description = new StringBuilder(50);
            description.Append("PDBMonomer(");
            description.Append(this.GetHashCode());
            description.Append(", iCode=").Append(ICode);
            description.Append(", chainID=").Append(ChainID).Append(", ");
            description.Append(base.ToString());
            description.Append(')');
            return description.ToString();
        }

        /// <summary>
        // @inheritDoc
        /// </summary>
        public override object Clone()
        {
            return (IPDBMonomer)base.Clone();
        }
    }
}
