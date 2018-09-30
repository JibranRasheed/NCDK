/* Copyright (C) 2004-2007  Matteo Floris <mfe4@users.sf.net>
 *                     2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// This class returns the valence of an atom.
    /// This descriptor does not have any parameters.
    /// </summary>
    /// <seealso cref="AtomValenceTool"/>
    // @author      mfe4
    // @cdk.created 2004-11-13
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:atomValence
    public partial class AtomValenceDescriptor : IAtomicDescriptor
    {
        /// <summary>
        /// Constructor for the AtomValenceDescriptor object
        /// </summary>
        public AtomValenceDescriptor() { }

        /// <summary>
        /// The specification attribute of the AtomValenceDescriptor object
        /// </summary>
        public IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#atomValence",
                typeof(AtomValenceDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        ///  The parameters attribute of the VdWRadiusDescriptor object.
        /// </summary>
        public IReadOnlyList<object> Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames { get; } = new string[] { "val" };

        /// <summary>
        /// This method calculates the valence of an atom.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="IDescriptorValue"/> is requested</param>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>The valence of an atom</returns>
        public DescriptorValue<Result<int>> Calculate(IAtom atom, IAtomContainer container)
        {
            int atomValence = AtomValenceTool.GetValence(atom);
            return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(atomValence), DescriptorNames);
        }

        /// <summary>
        /// The parameterNames attribute of the VdWRadiusDescriptor object.
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        /// Gets the parameterType attribute of the VdWRadiusDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public object GetParameterType(string name) => null;
    }
}
