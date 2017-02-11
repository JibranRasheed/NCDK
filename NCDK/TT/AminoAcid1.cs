















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara


using System;
using System.Collections.Generic;
using NCDK.Numerics;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class AminoAcid
        : Monomer, IAminoAcid, ICloneable
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
            base.Add(atom);	//  OnStateChanged is called here
            nTerminus = atom;
        }

        public void AddCTerminus(IAtom atom)
        {
            base.Add(atom);	//  OnStateChanged is called here
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
        : Monomer, IAminoAcid, ICloneable
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
            base.Add(atom);	//  OnStateChanged is called here
            nTerminus = atom;
        }

        public void AddCTerminus(IAtom atom)
        {
            base.Add(atom);	//  OnStateChanged is called here
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
