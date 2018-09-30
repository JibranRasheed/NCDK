/* Copyright (C) 2011  Egon Willighagen <egon.willighagen@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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

using NCDK.Geometries.Volume;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Volume descriptor using the method implemented in the <see cref="VABCVolume"/> class.
    /// </summary>
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:vabc
    // @cdk.keyword volume
    // @cdk.keyword descriptor
    public class VABCDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        /// <inheritdoc/>
        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#vabc",
                typeof(VABCDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <inheritdoc/>
        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count != 0)
                {
                    throw new CDKException("The VABCDescriptor expects zero parameters");
                }
            }
            get
            {
                return Array.Empty<object>();
            }
        }

        public override IReadOnlyList<string> DescriptorNames { get; } = new string[] { "VABC" };

        private DescriptorValue<Result<double>> GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(double.NaN), DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the descriptor value using the <see cref="VABCVolume"/> class.
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> whose volume is to be calculated</param>
        /// <returns>A double containing the volume</returns>
        public DescriptorValue<Result<double>> Calculate(IAtomContainer atomContainer)
        {
            double volume;
            try
            {
                // clone: don't mod original
                volume = VABCVolume.Calculate(Clone(atomContainer));
            }
            catch (CDKException exception)
            {
                return GetDummyDescriptorValue(exception);
            }

            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(volume), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<double>();

        /// <inheritdoc/>
        public override IReadOnlyList<string> ParameterNames { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public override object GetParameterType(string name) => null;

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}

