/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@yahoo.es>
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
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  The stabilization of the positive charge
    ///  (e.g.) obtained in the polar breaking of a bond is calculated from the sigma- and
    ///  lone pair-electronegativity values of the atoms that are in conjugation to the atoms
    ///  obtaining the charges. The method is based following <token>cdk-cite-Saller85</token>.
    ///  The value is calculated looking for resonance structures which can stabilize the charge.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term>maxIterations</term><term>0</term><term>Number of maximum iterations</term></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <seealso cref="StabilizationCharges"/>
    // @author         Miguel Rojas Cherto
    // @cdk.created    2008-104-31
    // @cdk.module     qsaratomic
    // @cdk.githash
    public partial class StabilizationPlusChargeDescriptor : IAtomicDescriptor
    {
        private static readonly string[] NAMES = { "stabilPlusC" };

        /// <summary>
        /// The specification attribute of the StabilizationPlusChargeDescriptor object
        /// </summary>
        public IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#stabilizationPlusCharge",
                typeof(StabilizationPlusChargeDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the StabilizationPlusChargeDescriptor object
        /// </summary>
        /// <value>
        /// <list type="bullet">
        /// <item>
        /// <term>1</term>
        /// <description>max iterations (optional, defaults to 20)</description>
        /// </item>
        /// </list>       
        /// </value>
        public IReadOnlyList<object> Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        ///  The method calculates the stabilization of charge of a given atom
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="IDescriptorValue"/> is requested</param>
        /// <param name="container">AtomContainer</param>
        /// <returns>return the stabilization value</returns>
        public DescriptorValue<Result<double>> Calculate(IAtom atom, IAtomContainer container)
        {
            IAtomContainer clone;
            IAtom localAtom;
            try
            {
                clone = (IAtomContainer)container.Clone();
                localAtom = clone.Atoms[container.Atoms.IndexOf(atom)];
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(clone);
            }
            catch (CDKException e)
            {
                return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(double.NaN), NAMES, e);
            }

            double result = StabilizationCharges.CalculatePositive(clone, localAtom);

            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(result), NAMES);
        }

        /// <summary>
        /// The parameterNames attribute of the StabilizationPlusChargeDescriptor object
        /// </summary>
        public IReadOnlyList<string> ParameterNames => null;

        /// <summary>
        ///  Gets the parameterType attribute of the StabilizationPlusChargeDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public object GetParameterType(string name) => null;
    }
}
