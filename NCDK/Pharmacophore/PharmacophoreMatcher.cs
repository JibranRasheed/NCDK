/* Copyright (C) 2004-2008  Rajarshi Guha <rajarshi.guha@gmail.com>
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
using NCDK.Common.Collections;
using NCDK.Numerics;
using NCDK.Aromaticities;
using NCDK.Geometries;
using NCDK.Graphs;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Isomorphisms.Matchers.SMARTS;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Pharmacophore
{
    /// <summary>
    /// Identifies atoms whose 3D arrangement matches a specified pharmacophore query.
    /// <para>
    /// A pharmacophore is defined by a set of atoms and distances between them. More generically
    /// we can restate this as a set of pharmacophore groups and the distances between them. Note
    /// that a pharmacophore group may consist of one or more atoms and the distances can be
    /// specified as a distance range rather than an exact distance.
    /// </para>
    /// <para>
    /// The goal of a pharmacophore query is to identify atom in a molecule whose 3D arrangement
    /// match a specified query.
    /// </para>
    /// <para>
    /// To perform a query one must first create a set of pharmacophore groups and specify the
    /// distances between them. Each pharmacophore group is represented by a {@link org.openscience.cdk.pharmacophore.PharmacophoreAtom}
    /// and the distances between them are represented by a {@link org.openscience.cdk.pharmacophore.PharmacophoreBond}.
    /// These are collected in a <see cref="QueryAtomContainer"/>.
    /// </para>
    /// <para>
    /// Given the query pharmacophore one can use this class to check with it occurs in a specified molecule.
    /// Note that for full generality pharmacophore searches are performed using conformations of molecules.
    /// This can easily be accomplished using this class together with the {@link org.openscience.cdk.ConformerContainer}
    /// class.  See the example below.
    /// </para>
    /// <para>
    /// Currently this class will allow you to perform pharmacophore searches using triads, quads or any number
    /// of pharmacophore groups. However, only distances and angles between pharmacophore groups are considered, so
    /// alternative constraints such as torsions and so on cannot be considered at this point.
    /// </para>
    /// <para>
    /// After a query has been performed one can retrieve the matching groups (as opposed to the matching atoms
    /// of the target molecule). However since a pharmacophore group (which is an object of class <see cref="PharmacophoreAtom"/>)
    /// allows you to access the indices of the corresponding atoms in the target molecule, this is not very
    /// difficult.
    /// </para>
    /// </summary> 
    /// <example>
    /// Example usage:
    /// <code>
    /// QueryAtomContainer query = new QueryAtomContainer();
    /// <p/>
    /// PharmacophoreQueryAtom o = new PharmacophoreQueryAtom("D", "[OX1]");
    /// PharmacophoreQueryAtom n1 = new PharmacophoreQueryAtom("A", "[N]");
    /// PharmacophoreQueryAtom n2 = new PharmacophoreQueryAtom("A", "[N]");
    /// <p/>
    /// query.Atoms.Add(o);
    /// query.Atoms.Add(n1);
    /// query.Atoms.Add(n2);
    /// <p/>
    /// PharmacophoreQueryBond b1 = new PharmacophoreQueryBond(o, n1, 4.0, 4.5);
    /// PharmacophoreQueryBond b2 = new PharmacophoreQueryBond(o, n2, 4.0, 5.0);
    /// PharmacophoreQueryBond b3 = new PharmacophoreQueryBond(n1, n2, 5.4, 5.8);
    /// <p/>
    /// query.Bonds.Add(b1);
    /// query.Bonds.Add(b2);
    /// query.Bonds.Add(b3);
    /// <p/>
    /// string filename = "/Users/rguha/pcore1.sdf";
    /// IteratingMDLConformerReader reader = new IteratingMDLConformerReader(
    ///      new FileReader(new File(filename)), Default.ChemObjectBuilder.Instance);
    /// <p/>
    /// ConformerContainer conformers;
    /// if (reader.HasNext()) conformers = (ConformerContainer) reader.Next();
    /// <p/>
    /// bool firstTime = true;
    /// foreach (var conf in conformers) {
    ///   bool status;
    ///   if (firstTime) {
    ///     status = matcher.Matches(conf, true);
    ///     firstTime = false;
    ///   } else status = matcher.Matches(conf, false);
    ///   if (status) {
    ///     // OK, matched. Do something
    ///   }
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>Extensions to SMARTS</para>
    /// <para>
    /// The pharmacophore supports some extentions to the SMARTS language that lead
    /// to flexible pharmacophore definitions  Note that these extensions are specific to
    /// pharmacophore usage and are not generally provided by the SMARTS parser itself.
    /// </para>
    /// <list type="bullet">
    /// <item> | - this allows one to perform a logical OR between two or more SMARTS patterns. An example might
    /// be a pharmacophore group that is meant to match a 5 membered ring or a 6 membered ring. This cannot be
    /// written in a single ordinary SMARTS pattern. However using this one extension one can write
    /// <code>A1AAAA1|A1AAAAA1</code>
    /// <item>
    /// </item>
    /// </item></list>l
    /// </remarks>
    /// <seealso cref="PharmacophoreAtom"/>
    /// <seealso cref="PharmacophoreBond"/>
    /// <seealso cref="PharmacophoreQueryAtom"/>
    /// <seealso cref="PharmacophoreQueryBond"/>
    // @author Rajarshi Guha
    // @cdk.module pcore
    // @cdk.githash
    // @cdk.keyword pharmacophore
    // @cdk.keyword 3D isomorphism
    public class PharmacophoreMatcher
    {
        private PharmacophoreQuery pharmacophoreQuery = null;
        private IAtomContainer pharmacophoreMolecule = null;

        private Mappings mappings = null;

        private readonly Aromaticity arom = new Aromaticity(ElectronDonation.DaylightModel, Cycles.Or(Cycles.AllFinder, Cycles.RelevantFinder));

        /// <summary>
        /// An empty constructor.
        /// <para>
        /// You should set the query before performing a match
        /// </para>
        /// </summary>
        public PharmacophoreMatcher() { }

        /// <summary>
        /// Initialize the matcher with a query.
        /// </summary>
        /// <param name="pharmacophoreQuery">The query pharmacophore</param>
        /// <seealso cref="PharmacophoreQueryAtom"/>
        /// <seealso cref="PharmacophoreQueryBond"/>
        public PharmacophoreMatcher(PharmacophoreQuery pharmacophoreQuery)
        {
            this.pharmacophoreQuery = pharmacophoreQuery;
        }

        /// <summary>
        /// Performs the pharmacophore matching.
        /// <para>
        /// This method will analyze the specified target molecule to identify pharmacophore
        /// groups. If dealing with conformer data it is probably more efficient to use
        /// the other form of this method which allows one to skip the pharmacophore group
        /// identification step after the first conformer.
        /// </para>
        /// </summary>
        /// <param name="atomContainer">The target molecule. Must have 3D coordinates</param>
        /// <returns>true is the target molecule contains the query pharmacophore</returns>
        /// <exception cref="CDKException">if the query pharmacophore was not set or the query is invalid or if the molecule does not have 3D coordinates</exception>
        /// <seealso cref="Matches(IAtomContainer, bool)"/>
        public bool Matches(IAtomContainer atomContainer)
        {
            return Matches(atomContainer, true);
        }

        /// <summary>
        /// Performs the pharmacophore matching.
        /// </summary>
        /// <param name="atomContainer">The target molecule. Must have 3D coordinates</param>
        /// <param name="initializeTarget">If <see langword="true"/>, the target molecule specified in the
        ///                         first argument will be analyzed to identify matching pharmacophore groups. If <see langword="false"/>
        ///                         this is not performed. The latter case is only useful when dealing with conformers
        ///                         since for a given molecule, all conformers will have the same pharmacophore groups
        ///                         and only the constraints will change from one conformer to another.</param>
        /// <returns>true is the target molecule contains the query pharmacophore</returns>
        /// <exception cref="CDKException">
        ///          if the query pharmacophore was not set or the query is invalid or if the molecule
        ///          does not have 3D coordinates</exception>
        public bool Matches(IAtomContainer atomContainer, bool initializeTarget)
        {
            if (!GeometryUtil.Has3DCoordinates(atomContainer)) throw new CDKException("Molecule must have 3D coordinates");
            if (pharmacophoreQuery == null) throw new CDKException("Must set the query pharmacophore before matching");
            if (!CheckQuery(pharmacophoreQuery))
                throw new CDKException(
                        "A problem in the query. Make sure all pharmacophore groups of the same symbol have the same same SMARTS");
            string title = atomContainer.GetProperty<string>(CDKPropertyName.Title);

            if (initializeTarget)
                pharmacophoreMolecule = GetPharmacophoreMolecule(atomContainer);
            else
            {
                // even though the atoms comprising the pcore groups are
                // constant, their coords will differ, so we need to make
                // sure we get the latest set of effective coordinates
                foreach (var iAtom in pharmacophoreMolecule.Atoms)
                {
                    PharmacophoreAtom patom = (PharmacophoreAtom)iAtom;
                    var tmpList = new List<int>();
                    foreach (var idx in patom.GetMatchingAtoms())
                        tmpList.Add(idx);
                    Vector3 coords = GetEffectiveCoordinates(atomContainer, tmpList);
                    patom.Point3D = coords;
                }
            }

            if (pharmacophoreMolecule.Atoms.Count < pharmacophoreQuery.Atoms.Count)
            {
                Debug.WriteLine("Target [" + title + "] did not match the query SMARTS. Skipping constraints");
                return false;
            }

            mappings = Pattern.FindSubstructure(pharmacophoreQuery)
                              .MatchAll(pharmacophoreMolecule);

            // XXX: doing one search then discarding
            return mappings.AtLeast(1);
        }

        /// <summary>
        /// Get the matching pharmacophore constraints.
        /// <para>
        /// The method should be called after performing the match, otherwise the return value is null.
        /// The method returns a List of List's. Each List represents the pharmacophore constraints in the
        /// target molecule that matched the query. Since constraints are conceptually modeled on bonds
        /// the result is a list of list of IBond. You should coerce these to the appropriate pharmacophore
        /// bond to get at the underlying grops.
        /// </para>
        /// </summary>
        /// <returns>a List of a List of pharmacophore constraints in the target molecule that match the query</returns>
        /// <seealso cref="PharmacophoreBond"/>
        /// <seealso cref="PharmacophoreAngleBond"/>
        public IList<IList<IBond>> GetMatchingPharmacophoreBonds()
        {
            if (mappings == null) return null;

            // XXX: re-subsearching the query
            var bonds = new List<IList<IBond>>();
            foreach (var map in mappings.ToBondMap())
            {
                bonds.Add(new List<IBond>(map.Values));
            }

            return bonds;
        }

        /// <summary>
        /// Return a list of HashMap's that allows one to get the query constraint for a given pharmacophore bond.
        /// <para>
        /// If the matching is successful, the return value is a List of HashMaps, each
        /// HashMap corresponding to a separate match. Each HashMap is keyed on the <see cref="PharmacophoreBond"/>
        /// in the target molecule that matched a constraint (<see cref="PharmacophoreQueryBond"/> or
        /// <see cref="PharmacophoreQueryAngleBond"/>. The value is the corresponding query bond.
        /// </para>
        /// </summary>
        /// <returns>A List of HashMaps, identifying the query constraint corresponding to a matched constraint in the target molecule.</returns>
        public List<IDictionary<IBond, IBond>> GetTargetQueryBondMappings()
        {
            if (mappings == null) return null;

            var bondMap = new List<IDictionary<IBond, IBond>>();

            // query -> target so need to inverse the mapping
            // XXX: re-subsearching the query
            foreach (var map in mappings.ToBondMap())
            {
                var inv = new Dictionary<IBond, IBond>();
                foreach (var e in map)
                    inv.Add(e.Value, e.Key);
                bondMap.Add(inv);
            }

            return bondMap;
        }

        /// <summary>
        /// Get the matching pharmacophore groups.
        /// <para>
        /// The method should be called after performing the match, otherwise the return value is null.
        /// The method returns a List of List's. Each List represents the pharmacophore groups in the
        /// target molecule that matched the query. Each pharmacophore group contains the indices of the
        /// atoms (in the target molecule) that correspond to the group.
        /// </para>
        /// </summary>
        /// <returns>a List of a List of pharmacophore groups in the target molecule that match the query</returns>
        /// <seealso cref="PharmacophoreAtom"/>
        public IList<IList<PharmacophoreAtom>> GetMatchingPharmacophoreAtoms()
        {
            if (pharmacophoreMolecule == null || mappings == null) return null;
            return GetPCoreAtoms(mappings);
        }

        /// <summary>
        /// Get the uniue matching pharmacophore groups.
        /// <para>
        /// The method should be called after performing the match, otherwise the return value is null.
        /// The method returns a List of List's. Each List represents the pharmacophore groups in the
        /// target molecule that matched the query. Each pharmacophore group contains the indices of the
        /// atoms (in the target molecule) that correspond to the group.
        /// </para>
        /// <para>
        /// This is analogous to the USA form of return value from a SMARTS match.
        /// </para>
        /// </summary>
        /// <returns>a List of a List of pharmacophore groups in the target molecule that match the query</returns>
        /// <seealso cref="PharmacophoreAtom"/>
        public IList<IList<PharmacophoreAtom>> GetUniqueMatchingPharmacophoreAtoms()
        {
            if (pharmacophoreMolecule == null || mappings == null) return null;
            return GetPCoreAtoms(mappings.GetUniqueAtoms());
        }

        private IList<IList<PharmacophoreAtom>> GetPCoreAtoms(Mappings mappings)
        {
            var atoms = new List<IList<PharmacophoreAtom>>();
            // XXX: re-subsearching the query
            foreach (var map in mappings.ToAtomMap())
            {
                var pcoreatoms = new List<PharmacophoreAtom>();
                foreach (var atom in map.Values)
                    pcoreatoms.Add((PharmacophoreAtom)atom);
                atoms.Add(pcoreatoms);
            }
            return atoms;
        }

        /// <summary>
        /// Get the query pharmacophore.
        /// </summary>
        /// <returns>The query</returns>
        public PharmacophoreQuery GetPharmacophoreQuery()
        {
            return pharmacophoreQuery;
        }

        /// <summary>
        /// Set a pharmacophore query.
        /// </summary>
        /// <param name="query">The query</param>
        public void SetPharmacophoreQuery(PharmacophoreQuery query)
        {
            pharmacophoreQuery = query;
        }

        /// <summary>
        /// Convert the input into a pcore molecule.
        /// </summary>
        /// <param name="input">the compound being converted from</param>
        /// <returns>pcore molecule </returns>
        /// <exception cref="CDKException">match failed</exception>
        private IAtomContainer GetPharmacophoreMolecule(IAtomContainer input)
        {
            // XXX: prepare query, to be moved
            PrepareInput(input);

            IAtomContainer pharmacophoreMolecule = input.Builder.CreateAtomContainer();

            var matched = new HashSet<string>();
            var uniqueAtoms = new LinkedHashSet<PharmacophoreAtom>();

            Debug.WriteLine($"Converting [{input.GetProperty<string>(CDKPropertyName.Title)}] to a pcore molecule");

            // lets loop over each pcore query atom
            foreach (var atom in pharmacophoreQuery.Atoms)
            {
                PharmacophoreQueryAtom qatom = (PharmacophoreQueryAtom)atom;
                string smarts = qatom.Smarts;

                // a pcore query might have multiple instances of a given pcore atom (say
                // 2 hydrophobic groups separated by X unit). In such a case we want to find
                // the atoms matching the pgroup SMARTS just once, rather than redoing the
                // matching for each instance of the pcore query atom.
                if (!matched.Add(qatom.Symbol))
                    continue;

                // see if the smarts for this pcore query atom gets any matches
                // in our query molecule. If so, then collect each set of
                // matching atoms and for each set make a new pcore atom and
                // add it to the pcore atom container object
                int count = 0;
                foreach (var query in qatom.CompiledSmarts)
                {
                    // create the lazy mappings iterator
                    Mappings mappings = Pattern.FindSubstructure(query)
                                                    .MatchAll(input)
                                                    .GetUniqueAtoms();

                    foreach (var mapping in mappings)
                    {
                        uniqueAtoms.Add(NewPCoreAtom(input, qatom, smarts, mapping));
                        count++;
                    }
                }
                Debug.WriteLine("\tFound " + count + " unique matches for " + smarts);
            }

            pharmacophoreMolecule.SetAtoms(uniqueAtoms.ToArray());

            // now that we have added all the pcore atoms to the container
            // we need to join all atoms with pcore bonds   (i.e. distance constraints)
            if (HasDistanceConstraints(pharmacophoreQuery))
            {
                int npatom = pharmacophoreMolecule.Atoms.Count;
                for (int i = 0; i < npatom - 1; i++)
                {
                    for (int j = i + 1; j < npatom; j++)
                    {
                        PharmacophoreAtom atom1 = (PharmacophoreAtom)pharmacophoreMolecule.Atoms[i];
                        PharmacophoreAtom atom2 = (PharmacophoreAtom)pharmacophoreMolecule.Atoms[j];
                        PharmacophoreBond bond = new PharmacophoreBond(atom1, atom2);
                        pharmacophoreMolecule.Bonds.Add(bond);
                    }
                }
            }

            // if we have angle constraints, generate only the valid
            // possible angle relationships, rather than all possible
            if (HasAngleConstraints(pharmacophoreQuery))
            {
                int nangleDefs = 0;

                foreach (var bond in pharmacophoreQuery.Bonds)
                {
                    if (!(bond is PharmacophoreQueryAngleBond)) continue;

                    IAtom startQAtom = bond.Atoms[0];
                    IAtom middleQAtom = bond.Atoms[1];
                    IAtom endQAtom = bond.Atoms[2];

                    // make a list of the patoms in the target that match
                    // each type of angle atom
                    List<IAtom> startl = new List<IAtom>();
                    List<IAtom> middlel = new List<IAtom>();
                    List<IAtom> endl = new List<IAtom>();

                    foreach (var tatom in pharmacophoreMolecule.Atoms)
                    {
                        if (tatom.Symbol.Equals(startQAtom.Symbol)) startl.Add(tatom);
                        if (tatom.Symbol.Equals(middleQAtom.Symbol)) middlel.Add(tatom);
                        if (tatom.Symbol.Equals(endQAtom.Symbol)) endl.Add(tatom);
                    }

                    // now we form the relevant angles, but we will
                    // have reversed repeats
                    List<IAtom[]> tmpl = new List<IAtom[]>();
                    foreach (var middle in middlel)
                    {
                        foreach (var start in startl)
                        {
                            if (middle.Equals(start)) continue;
                            foreach (var end in endl)
                            {
                                if (start.Equals(end) || middle.Equals(end)) continue;
                                tmpl.Add(new IAtom[] { start, middle, end });
                            }
                        }
                    }

                    // now clean up reversed repeats
                    List<IAtom[]> unique = new List<IAtom[]>();
                    for (int i = 0; i < tmpl.Count; i++)
                    {
                        IAtom[] seq1 = tmpl[i];
                        bool isRepeat = false;
                        for (int j = 0; j < unique.Count; j++)
                        {
                            if (i == j) continue;
                            IAtom[] seq2 = unique[j];
                            if (seq1[1] == seq2[1] && seq1[0] == seq2[2] && seq1[2] == seq2[0])
                            {
                                isRepeat = true;
                            }
                        }
                        if (!isRepeat) unique.Add(seq1);
                    }

                    // finally we can add the unique angle to the target
                    foreach (var seq in unique)
                    {
                        PharmacophoreAngleBond pbond = new PharmacophoreAngleBond((PharmacophoreAtom)seq[0],
                                (PharmacophoreAtom)seq[1], (PharmacophoreAtom)seq[2]);
                        pharmacophoreMolecule.Bonds.Add(pbond);
                        nangleDefs++;
                    }

                }
                Debug.WriteLine("Added " + nangleDefs + " defs to the target pcore molecule");
            }

            return pharmacophoreMolecule;
        }

        private PharmacophoreAtom NewPCoreAtom(IAtomContainer input, PharmacophoreQueryAtom qatom, string smarts, int[] mapping)
        {
            Vector3 coords = GetEffectiveCoordinates(input, mapping);
            PharmacophoreAtom patom = new PharmacophoreAtom(smarts, qatom.Symbol, coords);
            // n.b. mapping[] copy is mad by pcore atom 
            patom.SetMatchingAtoms(mapping);
            return patom;
        }

        private void PrepareInput(IAtomContainer input)
        {
            SmartsMatchers.Prepare(input, true);
            arom.Apply(input);
        }

        private bool HasDistanceConstraints(IQueryAtomContainer query)
        {
            foreach (var bond in query.Bonds)
            {
                if (bond is PharmacophoreQueryBond) return true;
            }
            return false;
        }

        private bool HasAngleConstraints(IQueryAtomContainer query)
        {
            foreach (var bond in query.Bonds)
            {
                if (bond is PharmacophoreQueryAngleBond) return true;
            }
            return false;
        }

        private int[] IntIndices(List<int> atomIndices)
        {
            int[] ret = new int[atomIndices.Count];
            for (int i = 0; i < atomIndices.Count; i++)
                ret[i] = atomIndices[i];
            return ret;
        }

        private Vector3 GetEffectiveCoordinates(IAtomContainer atomContainer, List<int> atomIndices)
        {
            Vector3 ret = Vector3.Zero;
            foreach (var atomIndice in atomIndices)
            {
                int atomIndex = (int)atomIndice;
                Vector3 coord = atomContainer.Atoms[atomIndex].Point3D.Value;
                ret.X += coord.X;
                ret.Y += coord.Y;
                ret.Z += coord.Z;
            }
            ret.X /= atomIndices.Count;
            ret.Y /= atomIndices.Count;
            ret.Z /= atomIndices.Count;
            return ret;
        }

        private Vector3 GetEffectiveCoordinates(IAtomContainer atomContainer, int[] atomIndices)
        {
            Vector3 ret = Vector3.Zero;
            foreach (var i in atomIndices)
            {
                Vector3 coord = atomContainer.Atoms[i].Point3D.Value;
                ret.X += coord.X;
                ret.Y += coord.Y;
                ret.Z += coord.Z;
            }
            ret.X /= atomIndices.Length;
            ret.Y /= atomIndices.Length;
            ret.Z /= atomIndices.Length;
            return ret;
        }

        private bool CheckQuery(IQueryAtomContainer query)
        {
            if (!(query is PharmacophoreQuery)) return false;
            Dictionary<string, string> map = new Dictionary<string, string>();
            for (int i = 0; i < query.Atoms.Count; i++)
            {
                IQueryAtom atom = (IQueryAtom)query.Atoms[i];
                if (!(atom is PharmacophoreQueryAtom)) return false;

                PharmacophoreQueryAtom pqatom = (PharmacophoreQueryAtom)atom;
                string label = pqatom.Symbol;
                string smarts = pqatom.Smarts;

                if (!map.ContainsKey(label))
                    map[label] = smarts;
                else
                {
                    if (!map[label].Equals(smarts)) return false;
                }
            }
            return true;
        }
    }
}
