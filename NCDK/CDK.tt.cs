
/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
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


namespace NCDK
{
    /// <summary>
    /// Helper class to provide general information about this CDK library.
    /// </summary>
    // @cdk.module core
    // @cdk.githash
    public static class CDK
    {
        /// <summary>
        /// Returns the version of this CDK library.
        /// </summary>
        /// <returns>The library version</returns>
        public static string Version => typeof(CDK).Assembly.GetName().Version.ToString();


        private static Config.AtomTypeFactory localJmolAtomTypeFactory = null;
        private static readonly object lockJmolAtomTypeFactory = new object();
        internal static Config.AtomTypeFactory JmolAtomTypeFactory
        {
            get
            {
                if (localJmolAtomTypeFactory == null)
                    lock (lockJmolAtomTypeFactory)
                    {
                        if (localJmolAtomTypeFactory == null)
                            localJmolAtomTypeFactory = Config.AtomTypeFactory.GetInstance("NCDK.Config.Data.jmol_atomtypes.txt", Silent.ChemObjectBuilder.Instance);
                    }
                return localJmolAtomTypeFactory;
            }
        }

        private static Config.AtomTypeFactory localCdkAtomTypeFactory = null;
        private static readonly object lockCdkAtomTypeFactory = new object();
        internal static Config.AtomTypeFactory CdkAtomTypeFactory
        {
            get
            {
                if (localCdkAtomTypeFactory == null)
                    lock (lockCdkAtomTypeFactory)
                    {
                        if (localCdkAtomTypeFactory == null)
                            localCdkAtomTypeFactory = Config.AtomTypeFactory.GetInstance("NCDK.Dict.Data.cdk-atom-types.owl", Silent.ChemObjectBuilder.Instance);
                    }
                return localCdkAtomTypeFactory;
            }
        }

        private static Config.AtomTypeFactory localStructgenAtomTypeFactory = null;
        private static readonly object lockStructgenAtomTypeFactory = new object();
        internal static Config.AtomTypeFactory StructgenAtomTypeFactory
        {
            get
            {
                if (localStructgenAtomTypeFactory == null)
                    lock (lockStructgenAtomTypeFactory)
                    {
                        if (localStructgenAtomTypeFactory == null)
                            localStructgenAtomTypeFactory = Config.AtomTypeFactory.GetInstance("NCDK.Config.Data.structgen_atomtypes.xml", Silent.ChemObjectBuilder.Instance);
                    }
                return localStructgenAtomTypeFactory;
            }
        }

        private static Smiles.SmilesParser localSilentSmilesParser = null;
        private static readonly object lockSilentSmilesParser = new object();
        internal static Smiles.SmilesParser SilentSmilesParser
        {
            get
            {
                if (localSilentSmilesParser == null)
                    lock (lockSilentSmilesParser)
                    {
                        if (localSilentSmilesParser == null)
                            localSilentSmilesParser = new Smiles.SmilesParser(Silent.ChemObjectBuilder.Instance);
                    }
                return localSilentSmilesParser;
            }
        }

        private static Tools.ISaturationChecker localSaturationChecker = null;
        private static readonly object lockSaturationChecker = new object();
        internal static Tools.ISaturationChecker SaturationChecker
        {
            get
            {
                if (localSaturationChecker == null)
                    lock (lockSaturationChecker)
                    {
                        if (localSaturationChecker == null)
                            localSaturationChecker = new Tools.SaturationChecker();
                    }
                return localSaturationChecker;
            }
        }

        private static Tools.ILonePairElectronChecker localLonePairElectronChecker = null;
        private static readonly object lockLonePairElectronChecker = new object();
        internal static Tools.ILonePairElectronChecker LonePairElectronChecker
        {
            get
            {
                if (localLonePairElectronChecker == null)
                    lock (lockLonePairElectronChecker)
                    {
                        if (localLonePairElectronChecker == null)
                            localLonePairElectronChecker = new Tools.LonePairElectronChecker();
                    }
                return localLonePairElectronChecker;
            }
        }

        private static AtomTypes.IAtomTypeMatcher localAtomTypeMatcher = null;
        private static readonly object lockAtomTypeMatcher = new object();
        internal static AtomTypes.IAtomTypeMatcher AtomTypeMatcher
        {
            get
            {
                if (localAtomTypeMatcher == null)
                    lock (lockAtomTypeMatcher)
                    {
                        if (localAtomTypeMatcher == null)
                            localAtomTypeMatcher = AtomTypes.CDKAtomTypeMatcher.GetInstance();
                    }
                return localAtomTypeMatcher;
            }
        }

        private static Tools.IHydrogenAdder localHydrogenAdder = null;
        private static readonly object lockHydrogenAdder = new object();
        internal static Tools.IHydrogenAdder HydrogenAdder
        {
            get
            {
                if (localHydrogenAdder == null)
                    lock (lockHydrogenAdder)
                    {
                        if (localHydrogenAdder == null)
                            localHydrogenAdder = Tools.CDKHydrogenAdder.Instance;
                    }
                return localHydrogenAdder;
            }
        }

        private static StructGen.Stochastic.PartialFilledStructureMerger localPartialFilledStructureMerger = null;
        private static readonly object lockPartialFilledStructureMerger = new object();
        internal static StructGen.Stochastic.PartialFilledStructureMerger PartialFilledStructureMerger
        {
            get
            {
                if (localPartialFilledStructureMerger == null)
                    lock (lockPartialFilledStructureMerger)
                    {
                        if (localPartialFilledStructureMerger == null)
                            localPartialFilledStructureMerger = new StructGen.Stochastic.PartialFilledStructureMerger();
                    }
                return localPartialFilledStructureMerger;
            }
        }
    }
}

