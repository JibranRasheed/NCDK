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
using NCDK.QSAR.Result;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /**
     *   Vertex adjacency information (magnitude):
     *   1 + log2 m where m is the number of heavy-heavy bonds. If m is zero, then zero is returned.
     *   (definition from MOE tutorial on line)
     *
     * <p>This descriptor uses these parameters:
     * <table border="1">
     *   <tr>
     *     <td>Name</td>
     *     <td>Default</td>
     *     <td>Description</td>
     *   </tr>
     *   <tr>
     *     <td></td>
     *     <td></td>
     *     <td>no parameters</td>
     *   </tr>
     * </table>
     *
     * Returns a single value named <i>vAdjMat</i>.
     *
     * @author      mfe4
     * @cdk.created 2004-11-03
     * @cdk.module  qsarmolecular
     * @cdk.githash
     * @cdk.set     qsar-descriptors
     * @cdk.dictref qsar-descriptors:vAdjMa
     */
    public class VAdjMaDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "VAdjMat" };

        /// <summary>
        ///  Constructor for the VAdjMaDescriptor object
        /// </summary>
        public VAdjMaDescriptor() { }

        /// <summary>
        /// The specification attribute of the VAdjMaDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#vAdjMa",
                typeof(VAdjMaDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// Tthe parameters attribute of the VAdjMaDescriptor object
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        /// Calculates the VAdjMa descriptor for an atom container
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>VAdjMa</returns>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            int n = 0; // count all heavy atom - heavy atom bonds
            foreach (var bond in atomContainer.Bonds)
            {
                if (bond.Atoms[0].AtomicNumber != 1 && bond.Atoms[1].AtomicNumber != 1)
                {
                    n++;
                }
            }

            double vadjMa = 0;
            if (n > 0)
            {
                vadjMa += (Math.Log(n) / Math.Log(2)) + 1;
            }
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(vadjMa), DescriptorNames);
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <para>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// </para>
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleResult(0.0);

        /// <summary>
        /// The parameterNames attribute of the VAdjMaDescriptor object
        /// </summary>
        public override string[] ParameterNames => null;

        /// <summary>
        /// Gets the parameterType attribute of the VAdjMaDescriptor object
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetParameterType(string name) => null;
    }
}
