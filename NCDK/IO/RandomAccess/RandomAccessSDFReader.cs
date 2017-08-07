/* Copyright (C) 2005-2008   Nina Jeliazkova <nina@acad.bg>
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
 */
using NCDK.IO.Formats;
using NCDK.IO.Listener;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace NCDK.IO.RandomAccess
{
    /// <summary>
    /// Random access of SDF file. Doesn't load molecules in memory, uses prebuilt
    /// index and seeks to find the correct record offset.
    /// </summary>
    // @author     Nina Jeliazkova <nina@acad.bg>
    // @cdk.module io
    // @cdk.githash
    public class RandomAccessSDFReader : RandomAccessReader
    {
        public RandomAccessSDFReader(string file, IChemObjectBuilder builder)
                : this(file, builder, null)
        { }

        public RandomAccessSDFReader(string file, IChemObjectBuilder builder, IReaderListener listener)
                : base(file, builder, listener)
        { }

        public override ISimpleChemObjectReader CreateChemObjectReader(TextReader reader)
        {
            return new MDLV2000Reader(reader);
        }

        protected override bool IsRecordEnd(string line)
        {
            return line.Equals("$$$$");
        }

        /// <seealso cref="IChemObjectIO.Format"/>
        public IResourceFormat Format => MDLFormat.Instance;

        public override bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IChemObject this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        protected override IChemObject ProcessContent()
        {
            // return chemObjectReader.Read(builder.CreateAtomContainer());
            //Read(IAtomContainer) doesn't read properties ...
            IChemObject co = chemObjectReader.Read(builder.CreateChemFile());
            if (co is IChemFile)
            {
                var cm = ((IChemFile)co).FirstOrDefault();
                if (cm != null)
                {
                    var sm = cm.FirstOrDefault()?.MoleculeSet;
                    if (sm != null)
                    {
                        var ssm = sm.FirstOrDefault();
                        if (ssm != null)
                            co = ssm;
                    }
                }
            }
            return co;
        }

        public void SetReader(TextReader reader)
        {
            throw new NotSupportedException();
        }

        public void SetReader(Stream reader)
        {
            throw new NotSupportedException();

        }

        public bool Accepts(Type classObject)
        {
            return chemObjectReader.Accepts(classObject);
        }

        public void Remove()
        {
            throw new NotSupportedException("Cannot remove entries with " + "the " + nameof(RandomAccessSDFReader));
        }

        public override int IndexOf(IChemObject item)
        {
            throw new NotImplementedException();
        }

        public override void Insert(int index, IChemObject item)
        {
            throw new NotImplementedException();
        }

        public override void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override bool Contains(IChemObject item)
        {
            throw new NotImplementedException();
        }

        public override void CopyTo(IChemObject[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(IChemObject item)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<IChemObject> GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
