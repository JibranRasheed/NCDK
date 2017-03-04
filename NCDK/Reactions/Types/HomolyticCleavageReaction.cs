/* Copyright (C) 2008 Miguel Rojas <miguelrojasch@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Reactions.Types.Parameters;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// <para>IReactionProcess which breaks the bond homogeneously leading to radical ions.
    /// It is also called homogeneous bond-breaking.Depending of the bond order,
    /// the bond will be removed or simply the order decreased.</para>
    /// <code>A-B => [A*] + [B*]</code>
    /// <para>It is processed by the HomolyticCleavageMechanism class</para>
    /// <code>
    ///  IAtomContainerSet setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
    ///  setOfReactants.Add(new AtomContainer());
    ///  IReactionProcess type = new HomolyticCleavageReaction();
    ///  object[] parameters = {bool.FALSE};
    ///  type.Parameters = parameters;
    ///  IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].SetFlag(CDKConstants.REACTIVE_CENTER,true);</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    ///
    ///
    // @author         Miguel Rojas
    ///
    // @cdk.created    2006-10-27
    // @cdk.module     reaction
    // @cdk.githash
    // @cdk.set        reaction-types
    ///
    /// <seealso cref="HomolyticCleavageMechanism"/>
    ///*/
    public class HomolyticCleavageReaction : ReactionEngine, IReactionProcess
    {

        /// <summary>
        /// Constructor of the HomolyticCleavageReaction object.
        ///
        /// </summary>
        public HomolyticCleavageReaction() { }

        /// <summary>
        ///  Gets the specification attribute of the HomolyticCleavageReaction object.
        ///
        /// <returns>The specification value</returns>
        /// </summary>

        public ReactionSpecification Specification =>
            new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#HomolyticCleavage", this
                            .GetType().Name, "$Id$", "The Chemistry Development Kit");

        /// <summary>
        ///  Initiate process.
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency
        ///  from the class tools.HydrogenAdder.
        ///
        ///
        /// <exception cref="CDKException"> Description of the Exception</exception>
        /// <param name="reactants">reactants of the reaction.</param>
       /// <param name="agents">agents of the reaction (Must be in this case null).</param>
        /// </summary>
        public IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {

            Debug.WriteLine("initiate reaction: HomolyticCleavageReaction");

            if (reactants.Count != 1)
            {
                throw new CDKException("HomolyticCleavageReaction only expects one reactant");
            }
            if (agents != null)
            {
                throw new CDKException("HomolyticCleavageReaction don't expects agents");
            }

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            /// if the parameter hasActiveCenter is not fixed yet, set the active centers
            IParameterReact ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);

            foreach (var bondi in reactant.Bonds)
            {
                IAtom atom1 = bondi.Atoms[0];
                IAtom atom2 = bondi.Atoms[1];
                if (bondi.IsReactiveCenter && atom1.IsReactiveCenter
                        && atom2.IsReactiveCenter
                        && (atom1.FormalCharge ?? 0) == 0
                        && (atom2.FormalCharge ?? 0) == 0
                        && !reactant.GetConnectedSingleElectrons(atom1).Any()
                        && !reactant.GetConnectedSingleElectrons(atom2).Any())
                {

                    var atomList = new List<IAtom>();
                    atomList.Add(atom1);
                    atomList.Add(atom2);
                    var bondList = new List<IBond>();
                    bondList.Add(bondi);
                    IAtomContainerSet<IAtomContainer> moleculeSet = reactant.Builder.CreateAtomContainerSet();
                    moleculeSet.Add(reactant);
                    IReaction reaction = Mechanism.Initiate(moleculeSet, atomList, bondList);
                    if (reaction == null)
                        continue;
                    else
                        setOfReactions.Add(reaction);

                }

            }

            return setOfReactions;
        }

        /// <summary>
        /// set the active center for this molecule.
        /// The active center will be those which correspond with A-B. If
        /// the bond is simple, it will be broken forming two fragments.
        /// <code>
        /// A: Atom
        /// #/=/-: bond
        /// B: Atom
        ///  </code>
        ///
        /// <param name="reactant">The molecule to set the activity</param>
        // @
        /// </summary>
        private void SetActiveCenters(IAtomContainer reactant)
        {
            foreach (var bond in reactant.Bonds)
            {
                IAtom atom1 = bond.Atoms[0];
                IAtom atom2 = bond.Atoms[1];
                if ((atom1.FormalCharge ?? 0) == 0
                        && (atom2.FormalCharge ?? 0) == 0
                        && !reactant.GetConnectedSingleElectrons(atom1).Any()
                        && !reactant.GetConnectedSingleElectrons(atom2).Any())
                {
                    bond.IsReactiveCenter = true;
                    atom1.IsReactiveCenter = true;
                    atom2.IsReactiveCenter = true;
                }
            }
        }
    }
}
