/* Copyright (C) 2001-2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.IO.CML;
using NCDK.IO.Formats;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using NCDK.Util.Xml;

namespace NCDK.IO
{
    /// <summary>
    /// Reads a molecule in CML 1.X and 2.0 format.
    /// CML is an XML based application {@cdk.cite PMR99}, and this Reader applies the method described in {@cdk.cite WIL01}.
    /// </summary>
    // @author      Egon L. Willighagen
    // @cdk.created 2001-02-01
    // @cdk.module  io
    // @cdk.githash
    // @cdk.keyword file format, CML
    // @cdk.bug     1544406
    // @cdk.iooptions
    public class CMLReader : DefaultChemObjectReader, IDisposable
    {
        private Stream input;
        private string url;

        private IDictionary<string, ICMLModule> userConventions = new Dictionary<string, ICMLModule>();

        /// <summary>
        ///  Reads CML from stream.
        /// </summary>
        /// <param name="input">Stream to read.</param>
        public CMLReader(Stream input)
        {
            this.input = input;
        }

        public CMLReader()
            : this(new MemoryStream(new byte[0]))
        {
        }

        public void RegisterConvention(string convention, ICMLModule conv)
        {
            userConventions[convention] = conv;
        }

        /// <summary>
        /// Define this <see cref="CMLReader"/> to take the input.
        /// </summary>
        /// <param name="url">Points to the file to be read</param>
        public CMLReader(string url)
        {
            this.url = url;
        }

        public override IResourceFormat Format => CMLFormat.Instance;

        /// <summary>
        /// This method must not be used; XML reading requires the use of an Stream.
        /// Use SetReader(Stream) instead.
        /// </summary>
        /// <param name="reader"></param>
        public override void SetReader(TextReader reader)
        {
            throw new CDKException("Invalid method call; use SetReader(Stream) instead.");
        }

        public override void SetReader(Stream input) {
            this.input = input;
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Read a IChemObject from input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>The content in a ChemFile object</returns>
        public override T Read<T>(T obj)
        {
            if (obj is IChemFile)
            {
                return (T)ReadChemFile((IChemFile)obj);
            }
            else
            {
                throw new CDKException("Only supported is reading of ChemFile objects.");
            }
        }

        // private functions

        private IChemFile ReadChemFile(IChemFile file)
        {
            Debug.WriteLine("Started parsing from input...");

            XmlReaderSettings setting = new XmlReaderSettings();
            setting.DtdProcessing = DtdProcessing.Parse;
            setting.ValidationFlags = XmlSchemaValidationFlags.None;
            setting.XmlResolver = new CMLResolver();

            XmlReader parser;
            if (input == null)
            {
                Debug.WriteLine("Parsing from URL: ", url);
                parser = XmlReader.Create(url, setting);
            }
            else
            {
                Debug.WriteLine("Parsing from Reader");
                parser = XmlReader.Create(input, setting);
            }

            CMLHandler handler = new CMLHandler(file);
            // copy the manually added conventions
            foreach (var conv in userConventions.Keys)
            {
                handler.RegisterConvention(conv, userConventions[conv]);
            }

            var reader = new XReader();
            reader.Handler = handler;
            try
            {
                XDocument doc = XDocument.Load(parser);
                reader.Read(doc);
            }
            catch (IOException e) {
                string error = "Error while reading file: " + e.Message;
                Trace.TraceError(error);
                Debug.WriteLine(e);
                throw new CDKException(error, e);
            }
            catch (XmlException saxe)
            {
                string error = "Error while parsing XML: " + saxe.Message;
                Trace.TraceError(error);
                Debug.WriteLine(saxe);
                throw new CDKException(error, saxe);
            }

            return file;
        }

        public override void Close()
        {
            if (input != null) input.Close();
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
