/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.Charges;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Bonds
{
    /// <summary>
    ///  The calculation of bond-Polarizability is calculated determining the
    ///  difference the Sigma electronegativity on atoms A and B of a bond.
    ///  </summary>
    /// <remarks>
    ///  <para>
    ///  This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term>bondPosition</term><term>0</term><term>The position of the target bond</term></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <seealso cref="Electronegativity"/>
    // @author      Miguel Rojas
    // @cdk.created 2006-05-08
    // @cdk.module  qsarbond
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:bondSigmaElectronegativity
    public partial class BondSigmaElectronegativityDescriptor : IBondDescriptor
    {
       /// <summary>Number of maximum iterations</summary>
        private int maxIterations = 6;

        private Electronegativity electronegativity;

        private static readonly string[] NAMES = { "elecSigB" };

        /// <summary>
        ///  Constructor for the BondSigmaElectronegativityDescriptor object.
        /// </summary>
        public BondSigmaElectronegativityDescriptor()
        {
            electronegativity = new Electronegativity();
        }

        /// <summary>
        /// The specification attribute of the BondSigmaElectronegativityDescriptor object.
        /// </summary>
        public IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#bondSigmaElectronegativity",
                typeof(BondSigmaElectronegativityDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the BondSigmaElectronegativityDescriptor object.
        /// </summary>
        public IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 1)
                {
                    throw new CDKException("SigmaElectronegativityDescriptor only expects one parameter");
                }
                if (!(value[0] is int))
                {
                    throw new CDKException("The parameter must be of type int");
                }
                if (value.Count == 0)
                    return;

                maxIterations = (int)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { (int)maxIterations };
            }
        }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        private DescriptorValue<Result<double>> GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(double.NaN), NAMES, e);
        }

        /// <summary>
        ///  The method calculates the sigma electronegativity of a given bond
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>return the sigma electronegativity</returns>
        public DescriptorValue<Result<double>> Calculate(IBond aBond, IAtomContainer atomContainer)
        {
            IAtomContainer ac;
            IBond bond;

            try
            {
                ac = (IAtomContainer)atomContainer.Clone();
                bond = ac.Bonds[atomContainer.Bonds.IndexOf(aBond)];
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(e);
            }

            if (maxIterations != -1 && maxIterations != 0) electronegativity.MaxIterations = maxIterations;

            double electroAtom1 = electronegativity.CalculateSigmaElectronegativity(ac, bond.Atoms[0]);
            double electroAtom2 = electronegativity.CalculateSigmaElectronegativity(ac, bond.Atoms[1]);

            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(Math.Abs(electroAtom1 - electroAtom2)), NAMES);
        }

        /// <summary>
        /// The parameterNames attribute of the BondSigmaElectronegativityDescriptor object.
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = new string[] { "maxIterations" };

        /// <summary>
        /// Gets the parameterType attribute of the BondSigmaElectronegativityDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public object GetParameterType(string name) => 0;
    }
}
