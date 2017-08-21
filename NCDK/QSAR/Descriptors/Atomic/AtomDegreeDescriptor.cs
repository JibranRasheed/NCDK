/*  Copyright (C)      2005  Matteo Floris <mfe4@users.sf.net>
 *                     2006  Kai Hartmann <kaihartmann@users.sf.net>
 *                     2006  Miguel Rojas-Cherto <miguelrojasch@users.sf.net>
 *                2005-2008  Egon Willighagen <egonw@users.sf.net>
 *                2008-2009  Rajarshi Guha <rajarshi@users.sf.net>
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

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// This class returns the number of not-Hs substituents of an atom, also defined as "atom degree".
    ///
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    ///   <item>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term></term>
    ///     <term>no parameters</term>
    ///   </item>
    /// </list>
    /// </para>
    /// </summary>
    // @author      mfe4
    // @cdk.created 2004-11-13
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:atomDegree
    public class AtomDegreeDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor
    {
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#atomDegree",
                typeof(AtomDegreeDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the AtomDegreeDescriptor object.
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames { get; } = new string[] { "aNeg" };

        /// <summary>
        /// This method calculates the number of not-H substituents of an atom.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
        /// <param name="container">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated for</param>
        /// <returns>The number of bonds on the shortest path between two atoms</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer container)
        {
            int atomDegree = 0;
            var neighboors = container.GetConnectedAtoms(atom);
            foreach (var neighboor in neighboors)
            {
                if (!neighboor.Symbol.Equals("H")) atomDegree += 1;
            }
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<int>(atomDegree), DescriptorNames);
        }

        /// <summary>
        /// Gets the parameterNames attribute of the AtomDegreeDescriptor object.
        /// </summary>
        /// <returns>The parameterNames value</returns>
        public override string[] ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        /// Gets the parameterType attribute of the AtomDegreeDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name) => null;
    }
}
