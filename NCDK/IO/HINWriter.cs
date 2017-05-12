/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.IO.Formats;
using System.IO;
using System;
using System.Collections.Generic;
using NCDK.Numerics;

namespace NCDK.IO
{
    /// <summary>
    /// Writer that outputs in the HIN format.
    /// </summary>
    // @author Rajarshi Guha <rajarshi@presidency.com>
    // @cdk.module io
    // @cdk.githash
    // @cdk.created 2004-01-27
    // @cdk.iooptions
    public class HINWriter : DefaultChemObjectWriter
    {
        private TextWriter writer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">the stream to write the HIN file to.</param>
        public HINWriter(TextWriter output)
        {
            this.writer = output;
        }

        public HINWriter(Stream output)
                : this(new StreamWriter(output))
        { }

        public HINWriter()
                : this(new StringWriter())
        { }

        public override IResourceFormat Format => HINFormat.Instance;

        public override void SetWriter(TextWriter output)
        {
            this.writer = output;
        }

        public override void SetWriter(Stream output)
        {
            SetWriter(new StreamWriter(output));
        }


        /// <summary>
        /// Flushes the output and closes this object.
        /// </summary>
        public override void Close()
        {
            writer.Close();
        }

        public override void Dispose()
        {
            Close();
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            if (typeof(IAtomContainerSet).IsAssignableFrom(type)) return true;
            return false;
        }

        public override void Write(IChemObject obj)
        {
            if (obj is IAtomContainer)
            {
                try
                {
                    IAtomContainerSet<IAtomContainer> som = obj.Builder.CreateAtomContainerSet();
                    som.Add((IAtomContainer)obj);
                    WriteAtomContainer(som);
                }
                catch (Exception ex)
                {
                    if (!(ex is ArgumentException | ex is IOException))
                        throw;
                    throw new CDKException("Error while writing HIN file: " + ex.Message, ex);
                }
            }
            else if (obj is IAtomContainerSet<IAtomContainer>)
            {
                try
                {
                    WriteAtomContainer((IAtomContainerSet<IAtomContainer>)obj);
                }
                catch (IOException)
                {
                    //
                }
            }
            else
            {
                throw new CDKException("HINWriter only supports output of Molecule or SetOfMolecule classes.");
            }
        }

        /// <summary>
        /// writes all the molecules supplied in a MoleculeSet class to
        /// a single HIN file. You can also supply a single Molecule object
        /// as well
        /// </summary>
        /// <param name="som">the set of molecules to write</param>
        /// <exception cref="IOException">if there is a problem writing the molecule</exception>
        private void WriteAtomContainer(IAtomContainerSet<IAtomContainer> som)
        {
            //int na = 0;
            //string info = "";
            string sym;
            double chrg;
            //bool writecharge = true;

            for (int molnum = 0; molnum < som.Count; molnum++)
            {
                IAtomContainer mol = som[molnum];

                try
                {
                    string molname = "mol " + (molnum + 1) + " " + mol.GetProperty<string>(CDKPropertyName.Title);

                    writer.Write(molname, 0, molname.Length);
                    writer.WriteLine();

                    // Loop through the atoms and write them out:

                    int i = 0;
                    foreach (var atom in mol.Atoms)
                    {
                        string line = "atom ";

                        sym = atom.Symbol;
                        chrg = atom.Charge.Value;
                        Vector3 point = atom.Point3D.Value;

                        line = line + (i + 1).ToString() + " - " + sym + " ** - " + chrg.ToString() + " "
                                + point.X.ToString() + " " + point.Y.ToString() + " "
                                + point.Z.ToString() + " ";

                        string abuf = "";
                        int ncon = 0;
                        foreach (var bond in mol.Bonds)
                        {
                            if (bond.Contains(atom))
                            {
                                // current atom is in the bond so lets get the connected atom
                                IAtom connectedAtom = bond.GetConnectedAtom(atom);
                                BondOrder bondOrder = bond.Order;
                                int serial;
                                string bondType = "";

                                // get the serial no for this atom
                                serial = mol.Atoms.IndexOf(connectedAtom);

                                if (bondOrder == BondOrder.Single)
                                    bondType = "s";
                                else if (bondOrder == BondOrder.Double)
                                    bondType = "d";
                                else if (bondOrder == BondOrder.Triple)
                                    bondType = "t";
                                else if (bond.IsAromatic) bondType = "a";
                                abuf = abuf + (serial + 1).ToString() + " " + bondType + " ";
                                ncon++;
                            }
                        }
                        line = line + " " + ncon.ToString() + " " + abuf;
                        writer.Write(line, 0, line.Length);
                        writer.WriteLine();
                        i++;
                    }
                    string buf = "endmol " + (molnum + 1);
                    writer.Write(buf, 0, buf.Length);
                    writer.WriteLine();
                }
                catch (IOException e)
                {
                    throw e;
                }
            }
        }
    }
}