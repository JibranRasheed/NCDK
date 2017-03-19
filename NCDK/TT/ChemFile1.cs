

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

/* Copyright (C) 1997-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
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
 using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NCDK.Default
{
    /// <summary>
    /// A Object containing a number of ChemSequences. This is supposed to be the
    /// top level container, which can contain all the concepts stored in a chemical
    /// document
    /// </summary>
    // @author        steinbeck
    // @cdk.githash
    // @cdk.module silent
    [Serializable]
    public class ChemFile
        : ChemObject, IChemFile, IChemObjectListener
    {
        protected IList<IChemSequence> chemSequences = new List<IChemSequence>();

        public ChemFile()
            : base()
        { }

        public IChemSequence this[int number]
        {
            get { return chemSequences[number]; }
            set { chemSequences[number] = value; }
        }

        public int Count => chemSequences.Count;

        public bool IsReadOnly => chemSequences.IsReadOnly;
        
        public void Add(IChemSequence chemSequence)
        {
 
            chemSequence.Listeners.Add(this);
            chemSequences.Add(chemSequence);
 
            NotifyChanged(); 
        }

        public void Clear()
        {
            foreach (var item in chemSequences)
                item.Listeners.Remove(this);
            chemSequences.Clear();
             NotifyChanged();         }

        public bool Contains(IChemSequence chemSequence)
        {
            return chemSequences.Contains(chemSequence);
        }

        public void CopyTo(IChemSequence[] array, int arrayIndex)
        {
            chemSequences.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IChemSequence> GetEnumerator()
        {
            return chemSequences.GetEnumerator();
        }

        public int IndexOf(IChemSequence chemSequence)
        {
            return chemSequences.IndexOf(chemSequence);
        }

        public void Insert(int index, IChemSequence chemSequence)
        {
 
            chemSequence.Listeners.Add(this);
            chemSequences.Insert(index, chemSequence);
 
            NotifyChanged(); 
        }

        public bool Remove(IChemSequence chemSequence)
        {
            bool ret = chemSequences.Remove(chemSequence);
 
            chemSequence.Listeners.Remove(this);
            NotifyChanged(); 
            return ret;
        }

        public void RemoveAt(int index)
        {
            chemSequences[index].Listeners.Remove(this);
            chemSequences.RemoveAt(index);
 
            NotifyChanged(); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ChemFile(#S=");
            sb.Append(chemSequences.Count);
            if (chemSequences.Count > 0)
            {
                foreach (var chemSequence in chemSequences)
                {
                    sb.Append(", ");
                    sb.Append(chemSequence.ToString());
                }
            }
            sb.Append(')');
            return sb.ToString(); 
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (ChemFile)base.Clone(map);
            clone.chemSequences = new List<IChemSequence>();
            foreach (var chemSequence in chemSequences)
                clone.chemSequences.Add((IChemSequence)chemSequence.Clone());
            return clone;
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
             NotifyChanged(evt);         }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// A Object containing a number of ChemSequences. This is supposed to be the
    /// top level container, which can contain all the concepts stored in a chemical
    /// document
    /// </summary>
    // @author        steinbeck
    // @cdk.githash
    // @cdk.module silent
    [Serializable]
    public class ChemFile
        : ChemObject, IChemFile, IChemObjectListener
    {
        protected IList<IChemSequence> chemSequences = new List<IChemSequence>();

        public ChemFile()
            : base()
        { }

        public IChemSequence this[int number]
        {
            get { return chemSequences[number]; }
            set { chemSequences[number] = value; }
        }

        public int Count => chemSequences.Count;

        public bool IsReadOnly => chemSequences.IsReadOnly;
        
        public void Add(IChemSequence chemSequence)
        {
            chemSequences.Add(chemSequence);
        }

        public void Clear()
        {
            chemSequences.Clear();
                    }

        public bool Contains(IChemSequence chemSequence)
        {
            return chemSequences.Contains(chemSequence);
        }

        public void CopyTo(IChemSequence[] array, int arrayIndex)
        {
            chemSequences.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IChemSequence> GetEnumerator()
        {
            return chemSequences.GetEnumerator();
        }

        public int IndexOf(IChemSequence chemSequence)
        {
            return chemSequences.IndexOf(chemSequence);
        }

        public void Insert(int index, IChemSequence chemSequence)
        {
            chemSequences.Insert(index, chemSequence);
        }

        public bool Remove(IChemSequence chemSequence)
        {
            bool ret = chemSequences.Remove(chemSequence);
            return ret;
        }

        public void RemoveAt(int index)
        {
            chemSequences.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ChemFile(#S=");
            sb.Append(chemSequences.Count);
            if (chemSequences.Count > 0)
            {
                foreach (var chemSequence in chemSequences)
                {
                    sb.Append(", ");
                    sb.Append(chemSequence.ToString());
                }
            }
            sb.Append(')');
            return sb.ToString(); 
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (ChemFile)base.Clone(map);
            clone.chemSequences = new List<IChemSequence>();
            foreach (var chemSequence in chemSequences)
                clone.chemSequences.Add((IChemSequence)chemSequence.Clone());
            return clone;
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                    }
    }
}
