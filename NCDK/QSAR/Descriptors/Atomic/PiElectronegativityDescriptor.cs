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
using NCDK.QSAR.Result;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Atomic {
    /// <summary>
    ///  Pi electronegativity is given by X = a + bq + c(q*q)
    ///
    ///
    /// <p>This descriptor uses these parameters:
    /// <table border="1">
    ///   <tr>
    ///     <td>Name</td>
    ///     <td>Default</td>
    ///     <td>Description</td>
    ///   </tr>
    ///   <tr>
    ///     <td>maxIterations</td>
    ///     <td>0</td>
    ///     <td>Number of maximum iterations</td>
    ///   </tr>
    /// </table>
    ///
    // @author      Miguel Rojas
    // @cdk.created 2006-05-17
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.set     qsar-descriptors
    // @cdk.dictref qsar-descriptors:piElectronegativity
    ///
    /// <seealso cref="Electronegativity"/>
    /// </summary>
    public class PiElectronegativityDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor {
        /// <summary>Number of maximum iterations*/
        private int maxIterations = -1;
        /// <summary>Number of maximum resonance structures*/
        private int maxResonStruc = -1;
        /// <summary> make a lone pair electron checker. Default true*/
        private bool lpeChecker = true;

        private static readonly string[] NAMES = { "elecPiA" };
        private PiElectronegativity electronegativity;

        /// <summary>
        ///  Constructor for the PiElectronegativityDescriptor object
        /// </summary>
        public PiElectronegativityDescriptor() {
            electronegativity = new PiElectronegativity();
        }

        /// <summary>
        ///  Gets the specification attribute of the PiElectronegativityDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#piElectronegativity",
                typeof(PiElectronegativityDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        ///  Sets the parameters attribute of the PiElectronegativityDescriptor object
        /// </summary>
        /// <remarks>
        /// The number of maximum iterations.
        /// <list type="bullet">
        /// <item>
        /// <term><value>1</value></term>
        /// <description>maxIterations</description>
        /// </item>
        /// <item>
        /// <term><value>2</value></term>
        /// <description>maxResonStruc</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <exception cref="exception">CDKException</exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 3) throw new CDKException("PartialPiChargeDescriptor only expects three parameter");

                if (!(value[0] is int)) throw new CDKException("The parameter must be of type int");
                maxIterations = (int)value[0];

                if (value.Length > 1 && value[1] != null)
                {
                    if (!(value[1] is bool)) throw new CDKException("The parameter must be of type bool");
                    lpeChecker = (bool)value[1];
                }

                if (value.Length > 2 && value[2] != null)
                {
                    if (!(value[2] is int)) throw new CDKException("The parameter must be of type int");
                    maxResonStruc = (int)value[2];
                }
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { maxIterations, lpeChecker, maxResonStruc };
            }
        }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        ///  The method calculates the pi electronegativity of a given atom
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="atom">The IAtom for which the DescriptorValue is requested</param>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>return the pi electronegativity</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer atomContainer) {
            IAtomContainer clone;
            IAtom localAtom;
            try {
                clone = (IAtomContainer)atomContainer.Clone();
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(clone);
                if (lpeChecker) {
                    LonePairElectronChecker lpcheck = new LonePairElectronChecker();
                    lpcheck.Saturate(atomContainer);
                }
                localAtom = clone.Atoms[atomContainer.Atoms.IndexOf(atom)];
            }
            catch (CDKException) {
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(double.NaN), NAMES, null);
            }

            if (maxIterations != -1 && maxIterations != 0) electronegativity.MaxIterations = maxIterations;
            if (maxResonStruc != -1 && maxResonStruc != 0) electronegativity.MaxResonanceStructures = maxResonStruc;

            double result = electronegativity.CalculatePiElectronegativity(clone, localAtom);

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(result),
                                       NAMES);
        }

        /// <summary>
        /// The parameterNames attribute of the SigmaElectronegativityDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "maxIterations", "lpeChecker", "maxResonStruc" };

        /// <summary>
        ///  Gets the parameterType attribute of the SigmaElectronegativityDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) {
            if ("maxIterations".Equals(name)) return int.MaxValue;
            if ("lpeChecker".Equals(name)) return true;
            if ("maxResonStruc".Equals(name)) return int.MaxValue;
            return null;
        }
    }
}
