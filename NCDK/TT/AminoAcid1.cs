

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

/* Copyright (C) 2005-2007  Egon Willighagen <e.willighagen@science.ru.nl>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class AminoAcid
        : Monomer, IAminoAcid
    {
        internal IAtom nTerminus;
        internal IAtom cTerminus;

        public AminoAcid()
        {
        }

        /// <summary>N-terminus atom.</summary>
        public IAtom NTerminus => nTerminus;

        /// <summary>C-terminus atom.</summary>
        public IAtom CTerminus => cTerminus;
        
        public void AddNTerminus(IAtom atom)
        {
            base.Atoms.Add(atom);    //  OnStateChanged is called here
            nTerminus = atom;
        }

        public void AddCTerminus(IAtom atom)
        {
            base.Atoms.Add(atom);    //  OnStateChanged is called here
            cTerminus = atom;
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (AminoAcid)base.Clone(map);
            if (nTerminus != null)
                clone.nTerminus = clone.atoms[this.atoms.IndexOf(nTerminus)];
            if (cTerminus != null)
                clone.cTerminus = clone.atoms[this.atoms.IndexOf(cTerminus)];
            return clone;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AminoAcid(");
            sb.Append(GetHashCode());
            if (nTerminus != null)
                sb.Append(", N:").Append(nTerminus.ToString());
            if (cTerminus != null)
                sb.Append(", C:").Append(cTerminus.ToString());
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class AminoAcid
        : Monomer, IAminoAcid
    {
        internal IAtom nTerminus;
        internal IAtom cTerminus;

        public AminoAcid()
        {
        }

        /// <summary>N-terminus atom.</summary>
        public IAtom NTerminus => nTerminus;

        /// <summary>C-terminus atom.</summary>
        public IAtom CTerminus => cTerminus;
        
        public void AddNTerminus(IAtom atom)
        {
            base.Atoms.Add(atom);    //  OnStateChanged is called here
            nTerminus = atom;
        }

        public void AddCTerminus(IAtom atom)
        {
            base.Atoms.Add(atom);    //  OnStateChanged is called here
            cTerminus = atom;
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (AminoAcid)base.Clone(map);
            if (nTerminus != null)
                clone.nTerminus = clone.atoms[this.atoms.IndexOf(nTerminus)];
            if (cTerminus != null)
                clone.cTerminus = clone.atoms[this.atoms.IndexOf(cTerminus)];
            return clone;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AminoAcid(");
            sb.Append(GetHashCode());
            if (nTerminus != null)
                sb.Append(", N:").Append(nTerminus.ToString());
            if (cTerminus != null)
                sb.Append(", C:").Append(cTerminus.ToString());
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }
    }
}
