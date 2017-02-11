/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.IO.PubChemXml;
using System;
using System.IO;
using System.Xml.Linq;

namespace NCDK.IO
{
    /// <summary>
    /// Reads an object from ASN.1 XML formated input for PubChem Compound entries.
    /// The following bits are supported: atoms.aid, atoms.element, atoms.2d,
    /// atoms.3d, bonds.aid1, bonds.aid2.
    /// </summary>
    // @cdk.module  io
    // @cdk.githash
    // @cdk.iooptions
    /// 
    // @cdk.keyword file format, PubChem Compound XML
    public class PCCompoundXMLReader : DefaultChemObjectReader
    {
        private TextReader input;
        private XElement parser;
        private XElement Parser
        {
			get
            {
                if (parser == null)
                {
                    try
                    {
                        parser = XDocument.Load(input).Root;
                    }
                    catch (Exception exception)
                    {
                        throw new CDKException($"Error while creating reader: {exception.Message}", exception);
                    }
                }
                return parser;
            }
        }
        private PubChemXMLHelper parserHelper;
        private IChemObjectBuilder builder;

        /// <summary>
        /// Construct a new reader from a Reader type object.
        /// </summary>
        /// <param name="input">reader from which input is read</param>
        public PCCompoundXMLReader(TextReader input)
        {
            SetReader(input);
        }

        public PCCompoundXMLReader(Stream input)
        {
            SetReader(input);
        }

        public PCCompoundXMLReader()
                : this(new StringReader(""))
        { }

        public override IResourceFormat Format => PubChemSubstanceXMLFormat.Instance;

        public override void SetReader(TextReader input)
        {
            this.input = input;
        }

        public override void SetReader(Stream input)
        {
            SetReader(new StreamReader(input));
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            return false;
        }

        public override T Read<T>(T obj)
        {
            if (obj is IAtomContainer)
            {
                try
                {
                    parserHelper = new PubChemXMLHelper(obj.Builder);
                    builder = obj.Builder;
                    return (T)ReadMolecule();
                }
                catch (IOException e)
                {
                    throw new CDKException("An IO Exception occured while reading the file.", e);
                }
                catch (CDKException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new CDKException("An error occured: " + e.Message, e);
                }
            }
            else
            {
                throw new CDKException("Only supported is reading of IAtomContainer objects.");
            }
        }

        public override void Close()
        {
            input.Close();
        }

        public override void Dispose()
        {
            Close();
        }

        // private procedures
        private IAtomContainer ReadMolecule()
        {
            foreach (var elm in Parser.DescendantsAndSelf())
            {
                if (elm.Name.Equals(PubChemXMLHelper.Name_EL_PCCOMPOUND))
                {
                    return parserHelper.ParseMolecule(elm, builder);
                }
            }
            return null;
        }
    }
}
