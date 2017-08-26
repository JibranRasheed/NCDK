/*
 *  Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.Common.Collections;
using NCDK.Graphs;
using NCDK.Graphs.Matrix;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Calculates the Molecular Distance Edge descriptor described in <token>cdk-cite-LIU98</token>.
    /// This class evaluates the 10 MDE descriptors described by Liu et al. and
    /// in addition it calculates variants where O and N are considered (as found in the DEDGE routine
    /// from ADAPT).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The variants are described below:
    /// <list type="table">
    /// <item>
    /// <term>MDEC-11</term><term> molecular distance edge between all primary carbons</term></item><item>
    /// <term>MDEC-12</term><term> molecular distance edge between all primary and secondary carbons</term></item><item>
    /// <term>MDEC-13</term><term> molecular distance edge between all primary and tertiary carbons</term></item><item>
    /// <term>MDEC-14</term><term> molecular distance edge between all primary and quaternary carbons </term></item><item>
    /// <term>MDEC-22</term><term> molecular distance edge between all secondary carbons </term></item><item>
    /// <term>MDEC-23</term><term> molecular distance edge between all secondary and tertiary carbons</term></item><item>
    /// <term>MDEC-24</term><term> molecular distance edge between all secondary and quaternary carbons </term></item><item>
    /// <term>MDEC-33</term><term> molecular distance edge between all tertiary carbons</term></item><item>
    /// <term>MDEC-34</term><term> molecular distance edge between all tertiary and quaternary carbons </term></item><item>
    /// <term>MDEC-44</term><term> molecular distance edge between all quaternary carbons </term></item><item>
    /// <term>MDEO-11</term><term> molecular distance edge between all primary oxygens </term></item><item>
    /// <term>MDEO-12</term><term> molecular distance edge between all primary and secondary oxygens </term></item><item>
    /// <term>MDEO-22</term><term> molecular distance edge between all secondary oxygens </term></item><item>
    /// <term>MDEN-11</term><term> molecular distance edge between all primary nitrogens</term></item><item>
    /// <term>MDEN-12</term><term> molecular distance edge between all primary and secondary nitrogens </term></item><item>
    /// <term>MDEN-13</term><term> molecular distance edge between all primary and tertiary niroqens </term></item><item>
    /// <term>MDEN-22</term><term> molecular distance edge between all secondary nitroqens </term></item><item>
    /// <term>MDEN-23</term><term> molecular distance edge between all secondary and tertiary nitrogens </term></item><item>
    /// <term>MDEN-33</term><term> molecular distance edge between all tertiary nitrogens</term></item>
    /// </list>
    /// </para>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2006-09-18
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:mde
    public partial class MDEDescriptor : IMolecularDescriptor
    {
        private static readonly string[] NAMES = {"MDEC-11", "MDEC-12", "MDEC-13", "MDEC-14", "MDEC-22", "MDEC-23",
            "MDEC-24", "MDEC-33", "MDEC-34", "MDEC-44", "MDEO-11", "MDEO-12", "MDEO-22", "MDEN-11", "MDEN-12",
            "MDEN-13", "MDEN-22", "MDEN-23", "MDEN-33"};

        public const int MDEC11 = 0;
        public const int MDEC12 = 1;
        public const int MDEC13 = 2;
        public const int MDEC14 = 3;
        public const int MDEC22 = 4;
        public const int MDEC23 = 5;
        public const int MDEC24 = 6;
        public const int MDEC33 = 7;
        public const int MDEC34 = 8;
        public const int MDEC44 = 9;

        public const int MDEO11 = 10;
        public const int MDEO12 = 11;
        public const int MDEO22 = 12;

        public const int MDEN11 = 13;
        public const int MDEN12 = 14;
        public const int MDEN13 = 15;
        public const int MDEN22 = 16;
        public const int MDEN23 = 17;
        public const int MDEN33 = 18;

        private const int C_1 = 1;
        private const int C_2 = 2;
        private const int C_3 = 3;
        private const int C_4 = 4;

        private const int O_1 = 1;
        private const int O_2 = 2;

        private const int N_1 = 1;
        private const int N_2 = 2;
        private const int N_3 = 3;

        public MDEDescriptor()
        {
        }

        /// <inheritdoc/>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#mde",
                typeof(MDEDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the WeightDescriptor object.
        /// </summary>
        /// <exception cref="CDKException">if more than 1 parameter is specified or if the parameter is not of type string</exception>
        public object[] Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        /// Calculate the weight of specified element type in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="container">The AtomContainer for which this descriptor is to be calculated. If 'H'
        ///                  is specified as the element symbol make sure that the AtomContainer has hydrogens.</param>
        /// <returns>The total weight of atoms of the specified element type</returns>
        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer container)
        {
            IAtomContainer local = AtomContainerManipulator.RemoveHydrogens(container);

            ArrayResult<double> retval = new ArrayResult<double>(19);
            for (int i = 0; i < 19; i++)
            {
                retval.Add(Dedge(local, i));
            }

            return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, retval,
                    DescriptorNames);
        }

        /// <inheritdoc/>
        public IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(19);

        private double Dedge(IAtomContainer atomContainer, int which)
        {
            int[][] adjMatrix = AdjacencyMatrix.GetMatrix(atomContainer);
            int[][] tdist = PathTools.ComputeFloydAPSP(adjMatrix);

            int[][] atypes = null;

            switch (which)
            {
                case MDEC11:
                case MDEC12:
                case MDEC13:
                case MDEC14:
                case MDEC22:
                case MDEC23:
                case MDEC24:
                case MDEC33:
                case MDEC34:
                case MDEC44:
                    atypes = EvalATable(atomContainer, 6);
                    break;
                case MDEO11:
                case MDEO12:
                case MDEO22:
                    atypes = EvalATable(atomContainer, 8);
                    break;
                case MDEN11:
                case MDEN12:
                case MDEN13:
                case MDEN22:
                case MDEN23:
                case MDEN33:
                    atypes = EvalATable(atomContainer, 7);
                    break;
            }
            double retval = 0;
            switch (which)
            {
                case MDEC11:
                    retval = EvalCValue(tdist, atypes, C_1, C_1);
                    break;
                case MDEC12:
                    retval = EvalCValue(tdist, atypes, C_1, C_2);
                    break;
                case MDEC13:
                    retval = EvalCValue(tdist, atypes, C_1, C_3);
                    break;
                case MDEC14:
                    retval = EvalCValue(tdist, atypes, C_1, C_4);
                    break;
                case MDEC22:
                    retval = EvalCValue(tdist, atypes, C_2, C_2);
                    break;
                case MDEC23:
                    retval = EvalCValue(tdist, atypes, C_2, C_3);
                    break;
                case MDEC24:
                    retval = EvalCValue(tdist, atypes, C_2, C_4);
                    break;
                case MDEC33:
                    retval = EvalCValue(tdist, atypes, C_3, C_3);
                    break;
                case MDEC34:
                    retval = EvalCValue(tdist, atypes, C_3, C_4);
                    break;
                case MDEC44:
                    retval = EvalCValue(tdist, atypes, C_4, C_4);
                    break;

                case MDEO11:
                    retval = EvalCValue(tdist, atypes, O_1, O_1);
                    break;
                case MDEO12:
                    retval = EvalCValue(tdist, atypes, O_1, O_2);
                    break;
                case MDEO22:
                    retval = EvalCValue(tdist, atypes, O_2, O_2);
                    break;

                case MDEN11:
                    retval = EvalCValue(tdist, atypes, N_1, N_1);
                    break;
                case MDEN12:
                    retval = EvalCValue(tdist, atypes, N_1, N_2);
                    break;
                case MDEN13:
                    retval = EvalCValue(tdist, atypes, N_1, N_3);
                    break;
                case MDEN22:
                    retval = EvalCValue(tdist, atypes, N_2, N_2);
                    break;
                case MDEN23:
                    retval = EvalCValue(tdist, atypes, N_2, N_3);
                    break;
                case MDEN33:
                    retval = EvalCValue(tdist, atypes, N_3, N_3);
                    break;
            }

            return retval;
        }

        private int[][] EvalATable(IAtomContainer atomContainer, int atomicNum)
        {
            //IAtom[] atoms = atomContainer.GetAtoms();
            int natom = atomContainer.Atoms.Count;
            int[][] atypes = Arrays.CreateJagged<int>(natom, 2);
            for (int i = 0; i < natom; i++)
            {
                IAtom atom = atomContainer.Atoms[i];
                int numConnectedBonds = atomContainer.GetConnectedBonds(atom).Count();
                atypes[i][1] = i;
                if (atomicNum == (atom.AtomicNumber == null ? 0 : atom.AtomicNumber))
                    atypes[i][0] = numConnectedBonds;
                else
                    atypes[i][0] = -1;
            }
            return atypes;
        }

        private double EvalCValue(int[][] distmat, int[][] codemat, int type1, int type2)
        {
            double lambda = 1;
            double n = 0;

            List<int> v1 = new List<int>();
            List<int> v2 = new List<int>();
            for (int i = 0; i < codemat.Length; i++)
            {
                if (codemat[i][0] == type1) v1.Add(codemat[i][1]);
                if (codemat[i][0] == type2) v2.Add(codemat[i][1]);
            }

            for (int i = 0; i < v1.Count; i++)
            {
                for (int j = 0; j < v2.Count; j++)
                {
                    int a = v1[i];
                    int b = v2[j];
                    if (a == b) continue;
                    double distance = distmat[a][b];
                    lambda = lambda * distance;
                    n++;
                }
            }

            if (type1 == type2)
            {
                lambda = Math.Sqrt(lambda);
                n = n / 2;
            }
            if (n == 0)
                return 0.0;
            else
                return n / Math.Pow(Math.Pow(lambda, 1.0 / (2.0 * n)), 2);
        }

        /// <summary>
        /// The parameterNames attribute of the WeightDescriptor object.
        /// </summary>
        public IReadOnlyList<string> ParameterNames => null;

        /// <summary>
        /// Gets the parameterType attribute of the WeightDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object whose class is that of the parameter requested</returns>
        public object GetParameterType(string name) => null;
    }
}
