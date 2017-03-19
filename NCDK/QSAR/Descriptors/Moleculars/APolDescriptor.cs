/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Config;
using NCDK.QSAR.Result;
using System;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Sum of the atomic polarizabilities (including implicit hydrogens).
    /// </summary>
    /// <remarks>
    /// Polarizabilities are taken from
    /// <a href="http://www.sunysccc.edu/academic/mst/ptable/p-table2.htm">http://www.sunysccc.edu/academic/mst/ptable/p-table2.htm</a>.
    /// <para>
    /// This class need explicit hydrogens.
    /// </para>
    /// <para>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term></term><term></term><term>no parameters</term></item>
    /// </list>
    /// </para>
    /// <para>
    /// Returns a single value with name <i>apol</i>.
    /// </para>
    /// </remarks>
    // @author      mfe4
    // @cdk.created 2004-11-13
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.set     qsar-descriptors
    // @cdk.dictref qsar-descriptors:apol
    // @cdk.keyword polarizability, atomic
    public class APolDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        /* Atomic polarizabilities ordered by atomic number from 1 to 102. */
        private static double[] polarizabilities;
        private static readonly string[] NAMES = { "apol" };

        /// <summary>
        /// Constructor for the APolDescriptor object.
        /// </summary>
        public APolDescriptor()
        {
            if (polarizabilities == null)
            {
                polarizabilities = new double[]{0, 0.666793, 0.204956, 24.3, 5.6, 3.03, 1.76, 1.1, 0.802, 0.557, 0.3956,
                    23.6, 10.6, 6.8, 5.38, 3.63, 2.9, 2.18, 1.6411, 43.4, 22.8, 17.8, 14.6, 12.4, 11.6, 9.4, 8.4, 7.5,
                    6.8, 6.1, 7.1, 8.12, 6.07, 4.31, 3.77, 3.05, 2.4844, 47.3, 27.6, 22.7, 17.9, 15.7, 12.8, 11.4, 9.6,
                    8.6, 4.8, 7.2, 7.2, 10.2, 7.7, 6.6, 5.5, 5.35, 4.044, 59.6, 39.7, 31.1, 29.6, 28.2, 31.4, 30.1,
                    28.8, 27.7, 23.5, 25.5, 24.5, 23.6, 22.7, 21.8, 21, 21.9, 16.2, 13.1, 11.1, 9.7, 8.5, 7.6, 6.5,
                    5.8, 5.7, 7.6, 6.8, 7.4, 6.8, 6, 5.3, 48.7, 38.3, 32.1, 32.1, 25.4, 27.4, 24.8, 24.5, 23.3, 23,
                    22.7, 20.5, 19.7, 23.8, 18.2, 17.5};
            }
        }

        /// <inheritdoc/>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification("http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#apol",
               typeof(APolDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the APolDescriptor object.
        /// </summary>
        public override object[] Parameters
        {
            get { return null; }
            set
            {
                // no parameters for this descriptor
            }
        }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        /// Calculate the sum of atomic polarizabilities in an <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="container">The <see cref="IAtomContainer"/> for which the descriptor is to be calculated</param>
        /// <returns>The sum of atomic polarizabilities <see cref="IsotopeFactory"/></returns>
        public override DescriptorValue Calculate(IAtomContainer container)
        {
            double apol = 0;
            int atomicNumber;
            try
            {
                IsotopeFactory ifac = Isotopes.Instance;
                IElement element;
                string symbol;
                foreach (var atom in container.Atoms)
                {
                    symbol = atom.Symbol;
                    element = ifac.GetElement(symbol);
                    atomicNumber = element.AtomicNumber.Value;
                    apol += polarizabilities[atomicNumber];
                    if (atom.ImplicitHydrogenCount != null)
                    {
                        apol += polarizabilities[1] * atom.ImplicitHydrogenCount.Value;
                    }
                }
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(apol), DescriptorNames);
            }
            catch (Exception ex1)
            {
                Debug.WriteLine(ex1);
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(double.NaN), DescriptorNames, 
                    new CDKException($"Problems with IsotopeFactory due to {ex1.ToString()}", ex1));
            }
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleResult(0.0);

        /// <summary>
        /// The parameterNames attribute of the APolDescriptor object.
        /// </summary>
        public override string[] ParameterNames => null;  // no param names to return

        /// <summary>
        /// Gets the parameterType attribute of the APolDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name) => null;
    }
}
