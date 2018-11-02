/*  Copyright (C) 2004-2007  Christian Hoppe
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
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Class that returns the complexity of a system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The complexity is defined as <token>cdk-cite-Nilakantan06</token>:
    /// <pre>
    /// C=Abs(B^2-A^2+A)+H/100
    /// </pre>
    /// where C=complexity, A=number of non-hydrogen atoms, B=number of bonds and H=number of heteroatoms
    /// </para>
    /// <para>This descriptor uses no parameters.</para>
    /// </remarks>
    // @author      chhoppe from EUROSCREEN
    // @cdk.created 2006-8-22
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:NilaComplexity
    public class FragmentComplexityDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "fragC" };

        public FragmentComplexityDescriptor() { }

        /// <inheritdoc/>
        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#NilaComplexity",
                typeof(FragmentComplexityDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the FragmentComplexityDescriptor object.
        /// </summary>
        /// <remarks>
        /// This descriptor takes no parameter.
        /// </remarks>
        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 0)
                {
                    throw new CDKException("FragmentComplexityDescriptor expects no parameter");
                }
            }
            get
            {
                return Array.Empty<object>();
            }
        }

        public override IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        /// Calculate the complexity in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="container">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated</param>
        /// <returns>the complexity</returns>
        public DescriptorValue<Result<double>> Calculate(IAtomContainer container)
        {
            int a = 0;
            double h = 0;
            foreach (var atom in container.Atoms)
            {
                switch (atom.AtomicNumber)
                {
                    default:
                        h++;
                        goto case ChemicalElement.AtomicNumbers.C;
                    case ChemicalElement.AtomicNumbers.C:
                        a++;
                        goto case ChemicalElement.AtomicNumbers.H;
                    case ChemicalElement.AtomicNumbers.H:
                        break;
                }
            }
            var b = container.Bonds.Count + AtomContainerManipulator.GetImplicitHydrogenCount(container);
            var c = Math.Abs(b * b - a * a + a) + (h / 100);
            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(c), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<double>(0.0);

        /// <summary>
        /// The parameterNames attribute of the FragmentComplexityDescriptor object.
        /// </summary>
        public override IReadOnlyList<string> ParameterNames => null;

        public override object GetParameterType(string name) => null;

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
