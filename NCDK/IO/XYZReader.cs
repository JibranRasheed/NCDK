/* Copyright (C) 2001-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Common.Mathematics;
using NCDK.Common.Primitives;
using NCDK.IO.Formats;
using System;
using System.Diagnostics;
using System.IO;
using NCDK.Numerics;

namespace NCDK.IO
{
    /// <summary>
    /// Reads an object from XYZ formated input.
    ///
    /// <p>This class is based on Dan Gezelter's XYZReader from Jmol
    ///
    // @cdk.module io
    // @cdk.githash
    // @cdk.iooptions
    ///
    // @cdk.keyword file format, XYZ
    /// </summary>
    public class XYZReader : DefaultChemObjectReader
    {
        private TextReader input;

        /// <summary>
        /// Construct a new reader from a Reader type object.
        ///
        /// <param name="input">reader from which input is read</param>
        /// </summary>
        public XYZReader(TextReader input)
        {
            this.input = input;
        }

        public XYZReader(Stream input)
            : this(new StreamReader(input))
        { }

        public XYZReader()
            : this(new StringReader(""))
        { }

        public override IResourceFormat Format => XYZFormat.Instance;

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
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// reads the content from a XYZ input. It can only return a
        /// IChemObject of type ChemFile
        ///
        /// <param name="object">class must be of type ChemFile</param>
        ///
        /// <seealso cref="IChemFile"/>
        /// </summary>
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

        // private procedures

        /// <summary>
        ///  Private method that actually parses the input to read a ChemFile
        ///  object.
        ///
        /// <returns>A ChemFile containing the data parsed from input.</returns>
        /// </summary>
        private IChemFile ReadChemFile(IChemFile file)
        {
            IChemSequence chemSequence = file.Builder.CreateChemSequence();

            int number_of_atoms = 0;

            try
            {
                string line;
                while ((line = input.ReadLine()) != null)
                {
                    // parse frame by frame
                    string token = line.Split('\t', ' ', ',', ';')[0];
                    number_of_atoms = int.Parse(token);
                    string info = input.ReadLine();

                    IChemModel chemModel = file.Builder.CreateChemModel();
                    var setOfMolecules = file.Builder.CreateAtomContainerSet();

                    IAtomContainer m = file.Builder.CreateAtomContainer();
                    m.SetProperty(CDKPropertyName.TITLE, info);

                    for (int i = 0; i < number_of_atoms; i++)
                    {
                        line = input.ReadLine();
                        if (line == null) break;
                        if (line.StartsWith("#") && line.Length > 1)
                        {
                            var comment = m.GetProperty(CDKPropertyName.COMMENT, "");
                            comment = comment + line.Substring(1).Trim();
                            m.SetProperty(CDKPropertyName.COMMENT, comment);
                            Debug.WriteLine("Found and set comment: ", comment);
                            i--; // a comment line does not count as an atom
                        }
                        else
                        {
                            double x = 0.0f, y = 0.0f, z = 0.0f;
                            double charge = 0.0f;
                            var tokenizer = Strings.Tokenize(line, '\t', ' ', ',', ';');
                            int fields = tokenizer.Count;

                            if (fields < 4)
                            {
                                // this is an error but cannot throw exception
                            }
                            else
                            {
                                string atomtype = tokenizer[0];
                                x = double.Parse(tokenizer[1]);
                                y = double.Parse(tokenizer[2]);
                                z = double.Parse(tokenizer[3]);
                                if (fields == 8) charge = double.Parse(tokenizer[4]);

                                IAtom atom = file.Builder.CreateAtom(atomtype, new Vector3(x, y, z));
                                atom.Charge = charge;
                                m.Atoms.Add(atom);
                            }
                        }
                    }

                    setOfMolecules.Add(m);
                    chemModel.MoleculeSet = setOfMolecules;
                    chemSequence.Add(chemModel);
                    line = input.ReadLine();
                }
                file.Add(chemSequence);
            }
            catch (IOException e)
            {
                // should make some noise now
                file = null;
                Trace.TraceError("Error while reading file: ", e.Message);
                Debug.WriteLine(e);
            }
            return file;
        }

        public override void Close()
        {
            input.Close();
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
