/*  Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
 *                     2013  Egon Willighagen <egonw@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Common.Primitives;
using NCDK.Config;
using NCDK.Tools;

using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK.Modeling.Builder3D
{
    /// <summary>
    /// AtomType list configurator that uses the ParameterSet originally
    /// defined in mm2.prm from tinker. This class was added to be able to port
    /// mm2 to CDK.
    ///
    // @author         chhoppe
    // @cdk.created    2004-09-07
    // @cdk.module     forcefield
    // @cdk.githash
    // @cdk.keyword    atom type, MM2
    /// </summary>
    public class MM2BasedParameterSetReader
    {
        private string configFile = "NCDK.Modeling.ForceField.Data.mm2.prm";
        private Stream ins = null;
        private IDictionary<string, object> parameterSet;
        public IList<IAtomType> AtomTypes { get; private set; }
        private IEnumerator<string> st;
        private string key = "";

        /// <summary>
        /// Constructor for the MM2BasedParameterSetReader object.
        /// </summary>
        public MM2BasedParameterSetReader()
        {
            parameterSet = new Dictionary<string, object>();
            AtomTypes = new List<IAtomType>();
        }

        public IDictionary<string, object> GetParamterSet()
        {
            return parameterSet;
        }

        /// <summary>
        /// Sets the file containing the config data.
        ///
        /// <param name="ins">The new inputStream type Stream</param>
        /// </summary>
        public void SetInputStream(Stream ins)
        {
            this.ins = ins;
        }

        /// <summary>
        /// Read a text based configuration file out of the force field mm2 file
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetForceFieldDefinitions()
        {
            string sid = st.Current; st.MoveNext();
            string svalue = st.Current; st.MoveNext();
            if (sid.Equals(">bontunit"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">bond-cubic"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">bond-quartic"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">angleunit"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">angle-sextic"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">strbndunit"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">opbendunit"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">torsionunit"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">vdwtype"))
            {
                key = sid.Substring(1);
                //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                parameterSet[key] = svalue;
            }
            else if (sid.Equals(">radiusrule"))
            {
                key = sid.Substring(1);
                //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                parameterSet[key] = svalue;
            }
            else if (sid.Equals(">radiustype"))
            {
                key = sid.Substring(1);
                //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                parameterSet[key] = svalue;
            }
            else if (sid.Equals(">radiussize"))
            {
                key = sid.Substring(1);
                //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                parameterSet[key] = svalue;
            }
            else if (sid.Equals(">epsilonrule"))
            {
                key = sid.Substring(1);
                //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                parameterSet[key] = svalue;
            }
            else if (sid.Equals(">a-expterm"))
            {
                try
                {
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = svalue;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals("b-expterm"))
            {
                try
                {
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = svalue;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">c-expterm"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">vdw-14-scale"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">chg-14-scale"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else if (sid.Equals(">dielectric"))
            {
                try
                {
                    double value1 = double.Parse(svalue);
                    key = sid.Substring(1);
                    //if (parameterSet.ContainsKey(key)){Debug.WriteLine("KeyError: hasKey "+key);}
                    parameterSet[key] = value1;
                }
                catch (FormatException)
                {
                    throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
                }
            }
            else
            {
            }
        }

        /// <summary>
        ///  Read and stores the atom types in a vector
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetAtomTypes(IChemObjectBuilder builder)
        {
            string name = "";
            string rootType = "";
            int an = 0;
            int rl = 255;
            int gl = 20;
            int bl = 147;
            int maxbond = 0;

            double mass = 0.0;
            st.MoveNext();
            string sid = st.Current; st.MoveNext();
            rootType = st.Current; st.MoveNext();
            name = st.Current; st.MoveNext();
            string san = st.Current; st.MoveNext();
            string sam = st.Current; st.MoveNext();
            string smaxbond = st.Current; st.MoveNext();

            try
            {
                mass = double.Parse(sam);
                an = int.Parse(san);
                maxbond = int.Parse(smaxbond);

            }
            catch (FormatException)
            {
                throw new IOException("AtomTypeTable.ReadAtypes: " + "Malformed Number");
            }

            IAtomType atomType = builder.CreateAtomType(name, rootType);
            atomType.AtomicNumber = an;
            atomType.ExactMass = mass;
            atomType.MassNumber = MassNumber(an, mass);
            atomType.FormalNeighbourCount = maxbond;
            atomType.Symbol = rootType;
            var co = CDKPropertyName.RGB2Int(rl, gl, bl);
            atomType.SetProperty(CDKPropertyName.COLOR, co);
            atomType.AtomTypeName = sid;
            AtomTypes.Add(atomType);
        }

        /// <summary>
        ///  Read vdw radius, stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetvdWaals()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid = st.Current; st.MoveNext();
            string sradius = st.Current; st.MoveNext();
            string sepsi = st.Current; st.MoveNext();
            try
            {
                double epsi = double.Parse(sepsi);
                double radius = double.Parse(sradius);
                data.Add(radius);
                data.Add(epsi);

            }
            catch (FormatException)
            {
                throw new IOException("VdWaalsTable.ReadvdWaals: " + "Malformed Number");
            }
            key = "vdw" + sid;
            //if (parameterSet.ContainsKey(key)){Console.Out.WriteLine("KeyError: hasKey "+key);}
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Read vdW pair radius,stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetvdWaalpr()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();
            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                data.Add(va1);
                data.Add(va2);

            }
            catch (FormatException nfe)
            {
                throw new IOException($"VdWaalsTable.ReadvdWaalsPR:Malformed Number due to {nfe.ToString()}");
            }
            key = "vdwpr" + sid1 + ";" + sid2;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the bond attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetBond()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                data.Add(va1);
                data.Add(va2);

            }
            catch (FormatException)
            {
                throw new IOException("setBond: Malformed Number");
            }
            key = "bond" + sid1 + ";" + sid2;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the bond3 attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetBond3()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                data.Add(va1);
                data.Add(va2);

            }
            catch (FormatException)
            {
                throw new IOException("setBond3: Malformed Number");
            }
            key = "bond3_" + sid1 + ";" + sid2;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the bond4 attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetBond4()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                data.Add(va1);
                data.Add(va2);

            }
            catch (FormatException)
            {
                throw new IOException("setBond4: Malformed Number");
            }
            key = "bond4_" + sid1 + ";" + sid2;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the angle attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetAngle()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string sid3 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();
            string value3 = st.Current; st.MoveNext();
            string value4 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                double va3 = double.Parse(value3);
                double va4 = double.Parse(value4);
                data.Add(va1);
                data.Add(va2);
                data.Add(va3);
                data.Add(va4);

                key = "angle" + sid1 + ";" + sid2 + ";" + sid3;
                if (parameterSet.ContainsKey(key))
                {
                    data = (IList<double>)parameterSet[key];
                    data.Add(va1);
                    data.Add(va2);
                    data.Add(va3);
                    data.Add(va4);
                }
                parameterSet[key] = data;

            }
            catch (FormatException)
            {
                throw new IOException("setAngle: Malformed Number");
            }

        }

        /// <summary>
        ///  Sets the angle3 attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetAngle3()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string sid3 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();
            string value3 = st.Current; st.MoveNext();
            string value4 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                double va3 = double.Parse(value3);
                double va4 = double.Parse(value4);
                data.Add(va1);
                data.Add(va2);
                data.Add(va3);
                data.Add(va4);

            }
            catch (FormatException)
            {
                throw new IOException("setAngle3: Malformed Number");
            }
            key = "angle3_" + sid1 + ";" + sid2 + ";" + sid3;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the angle4 attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetAngle4()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string sid3 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();
            string value3 = st.Current; st.MoveNext();
            string value4 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                double va3 = double.Parse(value3);
                double va4 = double.Parse(value4);
                data.Add(va1);
                data.Add(va2);
                data.Add(va3);
                data.Add(va4);

            }
            catch (FormatException)
            {
                throw new IOException("setAngle4: Malformed Number");
            }
            key = "angle4_" + sid1 + ";" + sid2 + ";" + sid3;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the strBnd attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetStrBnd()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                data.Add(va1);
            }
            catch (FormatException)
            {
                throw new IOException("setStrBnd: Malformed Number");
            }
            key = "strbnd" + sid1;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the opBend attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetOpBend()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                data.Add(va1);
                key = "opbend" + sid1 + ";" + sid2;
                if (parameterSet.ContainsKey(key))
                {
                    //Debug.WriteLine("KeyError: hasKey "+key);
                    data = (IList<double>)parameterSet[key];
                    data.Add(va1);
                }
                parameterSet[key] = data;

            }
            catch (FormatException)
            {
                throw new IOException("setOpBend: Malformed Number");
            }

        }

        /// <summary>
        ///  Sets the torsion attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetTorsion()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string sid3 = st.Current; st.MoveNext();
            string sid4 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            st.MoveNext();
            st.MoveNext();
            string value2 = st.Current; st.MoveNext();
            st.MoveNext();
            st.MoveNext();
            string value3 = st.Current; st.MoveNext();
            st.MoveNext();
            st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                double va3 = double.Parse(value3);
                data.Add(va1);
                data.Add(va2);
                data.Add(va3);

                key = "torsion" + sid1 + ";" + sid2 + ";" + sid3 + ";" + sid4;
                if (parameterSet.ContainsKey(key))
                {
                    data = (IList<double>)parameterSet[key];
                    data.Add(va1);
                    data.Add(va2);
                    data.Add(va3);
                }
                parameterSet[key] = data;
            }
            catch (FormatException)
            {
                throw new IOException("setTorsion: Malformed Number");
            }

        }

        /// <summary>
        ///  Sets the torsion4 attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetTorsion4()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string sid3 = st.Current; st.MoveNext();
            string sid4 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            st.MoveNext();
            st.MoveNext();
            string value2 = st.Current; st.MoveNext();
            st.MoveNext();
            st.MoveNext();
            string value3 = st.Current; st.MoveNext();
            st.MoveNext();
            st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                double va3 = double.Parse(value3);
                data.Add(va1);
                data.Add(va2);
                data.Add(va3);

            }
            catch (FormatException)
            {
                throw new IOException("setTorsion4: Malformed Number");
            }
            key = "torsion4_" + sid1 + ";" + sid2 + ";" + sid3 + ";" + sid4;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the charge attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetCharge()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                data.Add(va1);
            }
            catch (FormatException nfe)
            {
                throw new IOException($"setCharge: Malformed Number due to {nfe.ToString()}");
            }
            key = "charge" + sid1;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the dipole attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetDipole()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                data.Add(va1);
                data.Add(va2);

            }
            catch (FormatException)
            {
                throw new IOException("setDipole: " + "Malformed Number");
            }
            key = "dipole" + sid1 + ";" + sid2;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the dipole3 attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetDipole3()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                data.Add(va1);
                data.Add(va2);

            }
            catch (FormatException)
            {
                throw new IOException("setDipole3: " + "Malformed Number");
            }
            key = "dipole3_" + sid1 + ";" + sid2;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the piAtom attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetPiAtom()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();
            string value3 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                double va3 = double.Parse(value3);
                data.Add(va1);
                data.Add(va2);
                data.Add(va3);

            }
            catch (FormatException)
            {
                throw new IOException("setPiAtom: " + "Malformed Number");
            }
            key = "piatom" + sid1;
            parameterSet[key] = data;
        }

        /// <summary>
        ///  Sets the piBond attribute stored into the parameter set
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        private void SetPiBond()
        {
            IList<double> data = new List<double>();
            st.MoveNext();
            string sid1 = st.Current; st.MoveNext();
            string sid2 = st.Current; st.MoveNext();
            string value1 = st.Current; st.MoveNext();
            string value2 = st.Current; st.MoveNext();

            try
            {
                double va1 = double.Parse(value1);
                double va2 = double.Parse(value2);
                data.Add(va1);
                data.Add(va2);

            }
            catch (FormatException)
            {
                throw new IOException("setPiBond: " + "Malformed Number");
            }
            key = "pibond" + sid1 + ";" + sid2;
            parameterSet[key] = data;
        }

        /// <summary>
        /// The main method which parses through the force field configuration file
        ///
        /// <exception cref="Exception"> Description of the Exception</exception>
        /// </summary>
        public void ReadParameterSets(IChemObjectBuilder builder)
        {
            //vdW,vdWp,bond,bond4,bond3,angle,angle4,angle3,
            //strbond,opbend,torsion,torsion4,charge,dipole,
            //dipole3,piatom,pibond,dipole3
            //Debug.WriteLine("------ ReadParameterSets ------");

            if (ins == null)
            {
                ins = ResourceLoader.GetAsStream(configFile);
            }
            if (ins == null)
            {
                throw new IOException("There was a problem getting the default stream: " + configFile);
            }

            // read the contents from file
            TextReader r = new StreamReader(ins);
            string s;
            int[] a = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            try
            {
                while (true)
                {
                    s = r.ReadLine();
                    if (s == null)
                    {
                        break;
                    }
                    var e_st = Strings.Tokenize(s, '\t', ' ', ';'); st = e_st.GetEnumerator(); st.MoveNext();
                    int nt = e_st.Count;
                    if (s.StartsWith(">") & nt > 1)
                    {
                        SetForceFieldDefinitions();
                        a[0]++;
                    }
                    else if (s.StartsWith("atom") & nt <= 8)
                    {
                        a[0]++;
                        SetAtomTypes(builder);
                    }
                    else if (s.StartsWith("vdw ") & nt <= 5)
                    {
                        SetvdWaals();
                        a[1]++;
                    }
                    else if (s.StartsWith("vdwpr ") & nt <= 6)
                    {
                        SetvdWaalpr();
                        a[2]++;
                    }
                    else if (s.StartsWith("bond ") & nt <= 7)
                    {
                        SetBond();
                        a[3]++;
                    }
                    else if (s.StartsWith("bond4 ") & nt == 5)
                    {
                        SetBond4();
                        a[4]++;
                    }
                    else if (s.StartsWith("bond3 ") & nt == 5)
                    {
                        SetBond3();
                        a[5]++;
                    }
                    else if (s.StartsWith("angle ") & nt == 8)
                    {
                        SetAngle();
                        a[6]++;
                    }
                    else if (s.StartsWith("angle4 ") & nt == 8)
                    {
                        SetAngle4();
                        a[17]++;
                    }
                    else if (s.StartsWith("angle3 ") & nt == 8)
                    {
                        SetAngle3();
                        a[7]++;
                    }
                    else if (s.StartsWith("strbnd ") & nt == 5)
                    {
                        SetStrBnd();
                        a[8]++;
                    }
                    else if (s.StartsWith("opbend ") & nt == 4)
                    {
                        SetOpBend();
                        a[9]++;
                    }
                    else if (s.StartsWith("torsion ") & nt == 14)
                    {
                        SetTorsion();
                        a[10]++;
                    }
                    else if (s.StartsWith("torsion4 ") & nt == 14)
                    {
                        SetTorsion4();
                        a[11]++;
                    }
                    else if (s.StartsWith("charge ") & nt == 3)
                    {
                        SetCharge();
                        a[12]++;
                    }
                    else if (s.StartsWith("dipole ") & nt == 5)
                    {
                        SetDipole();
                        a[13]++;
                    }
                    else if (s.StartsWith("dipole3 ") & nt == 5)
                    {
                        SetDipole3();
                        a[14]++;
                    }
                    else if (s.StartsWith("piatom ") & nt == 5)
                    {
                        SetPiAtom();
                        a[15]++;
                    }
                    else if (s.StartsWith("pibond ") & nt == 5)
                    {
                        SetPiBond();
                        a[16]++;
                    }
                }// end while
                ins.Close();
            }
            catch (IOException e)
            {
                throw new IOException("There was a problem parsing the mm2 forcefield due to:" + e.ToString());
            }
        }

        /// <summary>
        /// Mass number for a atom with a given atomic number and exact mass.
        /// <param name="atomicNumber">atomic number</param>
        /// <param name="exactMass">exact mass</param>
        /// <returns>the mass number (or null) if no mass number was found</returns>
        /// <exception cref="IOException">isotope configuration could not be loaded</exception>
        /// </summary>
        private int? MassNumber(int atomicNumber, double exactMass)
        {
            string symbol = PeriodicTable.GetSymbol(atomicNumber);
            IIsotope isotope = Isotopes.Instance.GetIsotope(symbol, exactMass, 0.001);
            return isotope != null ? isotope.MassNumber : null;
        }
    }
}
