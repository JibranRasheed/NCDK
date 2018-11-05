/* Copyright (C) 2003-2007  Egon Willighagen <egonw@users.sf.net>
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

using NCDK.Common.Primitives;
using NCDK.Config;
using NCDK.IO.Formats;
using NCDK.Numerics;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// Reads a molecule from an Mol2 file, such as written by Sybyl.
    /// See the specs <see href="http://www.tripos.com/data/support/mol2.pdf">here</see>.
    /// </summary>
    // @author Egon Willighagen
    // @cdk.module io
    // @cdk.githash
    // @cdk.iooptions
    // @cdk.created 2003-08-21
    // @cdk.keyword file format, Mol2
    public class Mol2Reader : DefaultChemObjectReader
    {
        private static readonly AtomTypeFactory atFactory = AtomTypeFactory.GetInstance("NCDK.Config.Data.mol2_atomtypes.xml", Silent.ChemObjectBuilder.Instance);

        bool firstLineisMolecule = false;
        TextReader input = null;

        /// <summary>
        /// Dictionary of known atom type aliases. If the key is seen on input, it
        /// is repleaced with the specified value. Bugs /openbabel/bug/214 and /cdk/bug/1346
        /// </summary>
        private static readonly IDictionary<string, string> ATOM_TYPE_ALIASES =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() {
                // previously produced by Open Babel
                { "S.o2", "S.O2" },
                { "S.o", "S.O" },
                // seen in MMFF94 validation suite
                { "CL", "Cl" }, { "CU", "Cu" },
                { "FE", "Fe" }, { "BR", "Br" },
                { "NA", "Na" }, { "SI", "Si" },
                { "CA", "Ca" }, { "ZN", "Zn" },
                { "LI", "Li" }, { "MG", "Mg" },
            });

        /// <summary>
        /// Constructs a new MDLReader that can read Molecule from a given Reader.
        /// </summary>
        /// <param name="input">The Reader to read from</param>
        public Mol2Reader(TextReader input)
        {
            this.input = input;
        }

        public Mol2Reader(Stream input)
            : this(new StreamReader(input))
        { }

        public override IResourceFormat Format => Mol2Format.Instance;

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            if (typeof(IChemModel).IsAssignableFrom(type)) return true;
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            return false;
        }

        public override T Read<T>(T obj)
        {
            if (obj is IChemFile)
            {
                return (T)ReadChemFile((IChemFile)obj);
            }
            else if (obj is IChemModel)
            {
                return (T)ReadChemModel((IChemModel)obj);
            }
            else if (obj is IAtomContainer)
            {
                return (T)ReadMolecule((IAtomContainer)obj);
            }
            else
            {
                throw new CDKException("Only supported are ChemFile and Molecule.");
            }
        }

        private IChemModel ReadChemModel(IChemModel chemModel)
        {
            var setOfMolecules = chemModel.MoleculeSet;
            if (setOfMolecules == null)
            {
                setOfMolecules = chemModel.Builder.NewAtomContainerSet();
            }
            IAtomContainer m = ReadMolecule(chemModel.Builder.NewAtomContainer());
            if (m != null)
            {
                setOfMolecules.Add(m);
            }
            chemModel.MoleculeSet = setOfMolecules;
            return chemModel;
        }

        private IChemFile ReadChemFile(IChemFile chemFile)
        {
            IChemSequence chemSequence = chemFile.Builder.NewChemSequence();

            IChemModel chemModel = chemFile.Builder.NewChemModel();
            var setOfMolecules = chemFile.Builder.NewAtomContainerSet();
            IAtomContainer m = ReadMolecule(chemFile.Builder.NewAtomContainer());
            if (m != null) setOfMolecules.Add(m);
            chemModel.MoleculeSet = setOfMolecules;
            chemSequence.Add(chemModel);
            setOfMolecules = chemFile.Builder.NewAtomContainerSet();
            chemModel = chemFile.Builder.NewChemModel();
            try
            {
                firstLineisMolecule = true;
                while (m != null)
                {
                    m = ReadMolecule(chemFile.Builder.NewAtomContainer());
                    if (m != null)
                    {
                        setOfMolecules.Add(m);
                        chemModel.MoleculeSet = setOfMolecules;
                        chemSequence.Add(chemModel);
                        setOfMolecules = chemFile.Builder.NewAtomContainerSet();
                        chemModel = chemFile.Builder.NewChemModel();
                    }
                }
            }
            catch (CDKException)
            {
                throw;
            }
            catch (ArgumentException exception)
            {
                string error = "Error while parsing MOL2";
                Trace.TraceError(error);
                Debug.WriteLine(exception);
                throw new CDKException(error, exception);
            }
            try
            {
                input.Close();
            }
            catch (Exception exc)
            {
                string error = "Error while closing file: " + exc.Message;
                Trace.TraceError(error);
                throw new CDKException(error, exc);
            }

            chemFile.Add(chemSequence);

            // reset it to false so that other read methods called later do not get confused
            firstLineisMolecule = false;

            return chemFile;
        }

        public virtual bool Accepts(IChemObject o)
        {
            if (o is IChemFile) return true;
            if (o is IChemModel) return true;
            if (o is IAtomContainer) return true;
            return false;
        }

        /// <summary>
        /// Read a Reaction from a file in MDL RXN format
        /// </summary>
        /// <returns>The Reaction that was read from the MDL file.</returns>
        private IAtomContainer ReadMolecule(IAtomContainer molecule)
        {            
            try
            {
                int atomCount = 0;
                int bondCount = 0;

                string line;
                while (true)
                {
                    line = input.ReadLine();
                    if (line == null)
                        return null;
                    if (line.StartsWith("@<TRIPOS>MOLECULE", StringComparison.Ordinal))
                        break;
                    if (!line.StartsWithChar('#') && line.Trim().Length > 0)
                        break;
                }

                // ok, if we're coming from the chemfile function, we've already read the molecule RTI
                if (firstLineisMolecule)
                    molecule.Title = line;
                else
                {
                    line = input.ReadLine();
                    molecule.Title = line;
                }

                // get atom and bond counts
                string counts = input.ReadLine();
                var tokenizer = Strings.Tokenize(counts);
                try
                {
                    atomCount = int.Parse(tokenizer[0], NumberFormatInfo.InvariantInfo);
                }
                catch (FormatException nfExc)
                {
                    string error = "Error while reading atom count from MOLECULE block";
                    Trace.TraceError(error);
                    Debug.WriteLine(nfExc);
                    throw new CDKException(error, nfExc);
                }
                if (tokenizer.Count > 1)
                {
                    try
                    {
                        bondCount = int.Parse(tokenizer[1], NumberFormatInfo.InvariantInfo);
                    }
                    catch (FormatException nfExc)
                    {
                        string error = "Error while reading atom and bond counts";
                        Trace.TraceError(error);
                        Debug.WriteLine(nfExc);
                        throw new CDKException(error, nfExc);
                    }
                }
                else
                {
                    bondCount = 0;
                }
                Trace.TraceInformation("Reading #atoms: ", atomCount);
                Trace.TraceInformation("Reading #bonds: ", bondCount);

                // we skip mol type, charge type and status bit lines
                Trace.TraceWarning("Not reading molecule qualifiers");

                line = input.ReadLine();
                bool molend = false;
                while (line != null)
                {
                    if (line.StartsWith("@<TRIPOS>MOLECULE", StringComparison.Ordinal))
                    {
                        molend = true;
                        break;
                    }
                    else if (line.StartsWith("@<TRIPOS>ATOM", StringComparison.Ordinal))
                    {
                        Trace.TraceInformation("Reading atom block");
                        for (int i = 0; i < atomCount; i++)
                        {
                            line = input.ReadLine().Trim();
                            if (line.StartsWith("@<TRIPOS>MOLECULE", StringComparison.Ordinal))
                            {
                                molend = true;
                                break;
                            }
                            tokenizer = Strings.Tokenize(line);
                            // disregard the id token
                            string nameStr = tokenizer[1];
                            string xStr = tokenizer[2];
                            string yStr = tokenizer[3];
                            string zStr = tokenizer[4];
                            string atomTypeStr = tokenizer[5];

                            // replace unrecognised atom type
                            if (ATOM_TYPE_ALIASES.ContainsKey(atomTypeStr))
                                atomTypeStr = ATOM_TYPE_ALIASES[atomTypeStr];

                            IAtom atom = molecule.Builder.NewAtom();
                            IAtomType atomType;
                            try
                            {
                                atomType = atFactory.GetAtomType(atomTypeStr);
                            }
                            catch (Exception)
                            {
                                // ok, *not* an mol2 atom type
                                atomType = null;
                            }
                            // Maybe it is just an element
                            if (atomType == null && IsElementSymbol(atomTypeStr))
                            {
                                atom.Symbol = atomTypeStr;
                            }
                            else
                            {
                                if (atomType == null)
                                {
                                    atomType = atFactory.GetAtomType("X");
                                    Trace.TraceError($"Could not find specified atom type: {atomTypeStr}");
                                }
                                AtomTypeManipulator.Configure(atom, atomType);
                            }

                            atom.AtomicNumber = NaturalElement.ToAtomicNumber(atom.Symbol);
                            atom.Id = nameStr;
                            atom.AtomTypeName = atomTypeStr;
                            try
                            {
                                double x = double.Parse(xStr, NumberFormatInfo.InvariantInfo);
                                double y = double.Parse(yStr, NumberFormatInfo.InvariantInfo);
                                double z = double.Parse(zStr, NumberFormatInfo.InvariantInfo);
                                atom.Point3D = new Vector3(x, y, z);
                            }
                            catch (FormatException nfExc)
                            {
                                string error = "Error while reading atom coordinates";
                                Trace.TraceError(error);
                                Debug.WriteLine(nfExc);
                                throw new CDKException(error, nfExc);
                            }
                            molecule.Atoms.Add(atom);
                        }
                    }
                    else if (line.StartsWith("@<TRIPOS>BOND", StringComparison.Ordinal))
                    {
                        Trace.TraceInformation("Reading bond block");
                        for (int i = 0; i < bondCount; i++)
                        {
                            line = input.ReadLine();
                            if (line.StartsWith("@<TRIPOS>MOLECULE", StringComparison.Ordinal))
                            {
                                molend = true;
                                break;
                            }
                            tokenizer = Strings.Tokenize(line);
                            // disregard the id token
                            string atom1Str = tokenizer[1];
                            string atom2Str = tokenizer[2];
                            string orderStr = tokenizer[3];
                            try
                            {
                                int atom1 = int.Parse(atom1Str, NumberFormatInfo.InvariantInfo);
                                int atom2 = int.Parse(atom2Str, NumberFormatInfo.InvariantInfo);
                                if (string.Equals("nc", orderStr, StringComparison.Ordinal))
                                {
                                    // do not connect the atoms
                                }
                                else
                                {
                                    IBond bond = molecule.Builder.NewBond(molecule.Atoms[atom1 - 1], molecule.Atoms[atom2 - 1]);
                                    switch (orderStr)
                                    {
                                        case "1":
                                            bond.Order = BondOrder.Single;
                                            break;
                                        case "2":
                                            bond.Order = BondOrder.Double;
                                            break;
                                        case "3":
                                            bond.Order = BondOrder.Triple;
                                            break;
                                        case "am":
                                        case "ar": 
                                            bond.Order = BondOrder.Single;
                                            bond.IsAromatic = true;
                                            bond.Begin.IsAromatic = true;
                                            bond.End.IsAromatic = true;
                                            break;
                                        case "du":
                                            bond.Order = BondOrder.Single;
                                            break;
                                        case "un":
                                            bond.Order = BondOrder.Single;
                                            break;
                                        default:
                                            break;
                                    }
                                    molecule.Bonds.Add(bond);
                                }
                            }
                            catch (FormatException nfExc)
                            {
                                string error = "Error while reading bond information";
                                Trace.TraceError(error);
                                Debug.WriteLine(nfExc);
                                throw new CDKException(error, nfExc);
                            }
                        }
                    }
                    if (molend) return molecule;
                    line = input.ReadLine();
                }
            }
            catch (IOException exception)
            {
                string error = "Error while reading general structure";
                Trace.TraceError(error);
                Debug.WriteLine(exception);
                throw new CDKException(error, exception);
            }
            return molecule;
        }

        private static bool IsElementSymbol(string atomTypeStr)
        {
            for (int i = 1; i < PeriodicTable.ElementCount; i++)
            {
                if (string.Equals(PeriodicTable.GetSymbol(i), atomTypeStr, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    input.Dispose();
                }

                input = null;

                disposedValue = true;
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}
