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
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;
using System.Diagnostics;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// IReactionProcess which tries to reproduce the delocalization of electrons
    ///  which are unsaturated bonds from conjugated rings. Only is allowed those
    ///  movements which produces from neutral to neutral structures and not take account the possible
    ///  movements influenced from lone pairs, or empty orbitals. This movements are
    ///  typically from rings without any access or deficiency of charge and have a
    ///  even number of atoms. 
    ///  </summary>
    ///  <example>
    ///  <para>The reaction don't care if the product are the same in symmetry.</para>
    ///  <code>
    ///  IAtomContainerSet setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
    ///  setOfReactants.Add(new AtomContainer());
    ///  IReactionProcess type = new PiBondingMovementReaction();
    ///  object[] parameters = {bool.FALSE};
    ///  type.Parameters = parameters;
    ///  IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    // @author         Miguel Rojas
    // @cdk.created    2007-02-02
    // @cdk.module     reaction
    // @cdk.set        reaction-types
    // @cdk.githash
    public class PiBondingMovementReaction : ReactionEngine, IReactionProcess
    {
        /// <summary>
        /// Constructor of the PiBondingMovementReaction object
        /// </summary>
        public PiBondingMovementReaction() { }

        /// <summary>
        ///  The specification attribute of the PiBondingMovementReaction object
        /// </summary>
        public ReactionSpecification Specification =>
            new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#PiBondingMovement", this
                            .GetType().Name, "$Id$", "The Chemistry Development Kit");

        /// <summary>
        ///  Initiate process.
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency
        ///  from the class tools.HydrogenAdder.
        /// </summary>
        /// <exception cref="CDKException"> Description of the Exception</exception>
        /// <param name="reactants">reactants of the reaction.</param>
        /// <param name="agents">agents of the reaction (Must be in this case null).</param>
        public IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {
            Debug.WriteLine("initiate reaction: PiBondingMovementReaction");

            if (reactants.Count != 1)
            {
                throw new CDKException("PiBondingMovementReaction only expects one reactant");
            }
            if (agents != null)
            {
                throw new CDKException("PiBondingMovementReaction don't expects agents");
            }

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactant);

            // if the parameter hasActiveCenter is not fixed yet, set the active centers
            IParameterReact ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);

            //        if((bool)paramsMap["lookingSymmetry"]){
            //            Aromaticity.CDKLegacy.Apply(reactant);
            //        }

            AllRingsFinder arf = new AllRingsFinder();
            IRingSet ringSet = arf.FindAllRings((IAtomContainer)reactant);
            for (int ir = 0; ir < ringSet.Count; ir++)
            {
                IRing ring = (IRing)ringSet[ir];

                //only rings with even number of atoms
                int nrAtoms = ring.Atoms.Count;
                if (nrAtoms % 2 == 0)
                {
                    int nrSingleBonds = 0;
                    foreach (var bond in ring.Bonds)
                        if (bond.Order == BondOrder.Single) nrSingleBonds++;
                    //if exactly half (nrAtoms/2==nrSingleBonds)
                    if (nrSingleBonds != 0 && nrAtoms / 2 == nrSingleBonds)
                    {
                        bool ringCompletActive = false;
                        foreach (var bond in ring.Bonds)
                            if (bond.IsReactiveCenter)
                                ringCompletActive = true;
                            else
                            {
                                ringCompletActive = false;
                                break;
                            }
                        if (!ringCompletActive) continue;

                        IReaction reaction = reactants.Builder.CreateReaction();
                        reaction.Reactants.Add(reactant);

                        IAtomContainer reactantCloned;
                        reactantCloned = (IAtomContainer)reactant.Clone();

                        foreach (var bondi in ring.Bonds)
                        {
                            int bondiP = reactant.Bonds.IndexOf(bondi);
                            if (bondi.Order == BondOrder.Single)
                                BondManipulator.IncreaseBondOrder(reactantCloned.Bonds[bondiP]);
                            else
                                BondManipulator.DecreaseBondOrder(reactantCloned.Bonds[bondiP]);

                        }

                        reaction.Products.Add((IAtomContainer)reactantCloned);
                        setOfReactions.Add(reaction);
                    }

                }
            }

            return setOfReactions;
        }

        /// <summary>
        /// Set the active center for this molecule.
        /// The active center will be those which correspond to a ring
        /// with pi electrons with resonance.
        ///
        /// FIXME REACT: It could be possible that a ring is a super ring of others small rings
        /// </summary>
        /// <param name="reactant">The molecule to set the activity</param>
        private void SetActiveCenters(IAtomContainer reactant)
        {
            AllRingsFinder arf = new AllRingsFinder();
            IRingSet ringSet = arf.FindAllRings(reactant);
            for (int ir = 0; ir < ringSet.Count; ir++)
            {
                IRing ring = (IRing)ringSet[ir];
                //only rings with even number of atoms
                int nrAtoms = ring.Atoms.Count;
                if (nrAtoms % 2 == 0)
                {
                    int nrSingleBonds = 0;
                    foreach (var bond in ring.Bonds)
                    {
                        if (bond.Order == BondOrder.Single) nrSingleBonds++;
                    }
                    //if exactly half (nrAtoms/2==nrSingleBonds)
                    if (nrSingleBonds != 0 && nrAtoms / 2 == nrSingleBonds)
                    {
                        foreach (var bond in ring.Bonds)
                        {
                            bond.IsReactiveCenter = true;
                        }
                    }
                }
            }
        }
    }
}
