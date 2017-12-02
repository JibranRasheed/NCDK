


// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>

/* Copyright (C) 1997-2007  Christoph Steinbeck
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NCDK.Default
{
    /// <summary>
    /// Base class for all chemical objects that maintain a list of Atoms and
    /// ElectronContainers. 
    /// </summary>
    /// <example>
    /// Looping over all Bonds in the AtomContainer is typically done like: 
    /// <code>
    /// foreach (IBond aBond in atomContainer.Bonds)
    /// {
    ///         // do something
    /// }
    /// </code>
    /// </example>
    // @cdk.githash 
    // @author steinbeck
    // @cdk.created 2000-10-02 
    [Serializable]
    public class AtomContainer
        : ChemObject, IAtomContainer, IChemObjectListener
    {
        /// <summary>
        /// Atoms contained by this object.
        /// </summary>
        internal ObservableChemObjectCollection<IAtom> atoms;

        /// <summary>
        /// Bonds contained by this object.
        /// </summary>
        internal ObservableChemObjectCollection<IBond> bonds;

        /// <summary>
        /// Lone pairs contained by this object.
        /// </summary>
        internal ObservableChemObjectCollection<ILonePair> lonePairs;

        /// <summary>
        /// Single electrons contained by this object.
        /// </summary>
        internal ObservableChemObjectCollection<ISingleElectron> singleElectrons;

        /// <summary>
        /// Stereo elements contained by this object.
        /// </summary>
        internal IList<IReadOnlyStereoElement<IChemObject, IChemObject>> stereoElements;

        internal bool isAromatic;
        internal bool isSingleOrDouble;

        private void Init(
            ObservableChemObjectCollection<IAtom> atoms,
            ObservableChemObjectCollection<IBond> bonds,
            ObservableChemObjectCollection<ILonePair> lonePairs,
            ObservableChemObjectCollection<ISingleElectron> singleElectrons,
            IList<IReadOnlyStereoElement<IChemObject, IChemObject>> stereoElements)
        {
            this.atoms = atoms;
            this.bonds = bonds;
            this.lonePairs = lonePairs;
            this.singleElectrons = singleElectrons;
            this.stereoElements = stereoElements;
        }

        public AtomContainer(
            IEnumerable<IAtom> atoms,
            IEnumerable<IBond> bonds,
            IEnumerable<ILonePair> lonePairs,
            IEnumerable<ISingleElectron> singleElectrons,
            IEnumerable<IReadOnlyStereoElement<IChemObject, IChemObject>> stereoElements)
        {
            Init(
                CreateObservableChemObjectCollection(atoms, false),
                CreateObservableChemObjectCollection(bonds, true),
                CreateObservableChemObjectCollection(lonePairs, true),
                CreateObservableChemObjectCollection(singleElectrons, true),
                new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(stereoElements)
            );
        }

        private ObservableChemObjectCollection<T> CreateObservableChemObjectCollection<T>(IEnumerable<T> objs, bool allowDup) where T : IChemObject
        {
 
            var list = new ObservableChemObjectCollection<T>(this, objs);
            list.AllowDuplicate = allowDup;
            return list;
        }

        public AtomContainer(
           IEnumerable<IAtom> atoms,
           IEnumerable<IBond> bonds)
             : this(
                  atoms,
                  bonds,
                  Array.Empty<ILonePair>(),
                  Array.Empty<ISingleElectron>(),
                  Array.Empty<IReadOnlyStereoElement<IChemObject, IChemObject>>())
        { }

        /// <summary>
        ///  Constructs an empty AtomContainer.
        /// </summary>
        public AtomContainer()
            : this(
                      Array.Empty<IAtom>(), 
                      Array.Empty<IBond>(), 
                      Array.Empty<ILonePair>(),
                      Array.Empty<ISingleElectron>(),
                      Array.Empty<IReadOnlyStereoElement<IChemObject, IChemObject>>())
        { }

        /// <summary>
        /// Constructs an AtomContainer with a copy of the atoms and electronContainers
        /// of another AtomContainer (A shallow copy, i.e., with the same objects as in
        /// the original AtomContainer).
        /// </summary>
        /// <param name="container">An AtomContainer to copy the atoms and electronContainers from</param>
        public AtomContainer(IAtomContainer container)
            : this(
                  container.Atoms,
                  container.Bonds,
                  container.LonePairs,
                  container.SingleElectrons,
                  container.StereoElements)
        { }

        /// <inheritdoc/>
        public virtual bool IsAromatic
        {
            get { return isAromatic; }
            set
            {
                isAromatic = value; 
                NotifyChanged();
            }
        }
        
        /// <inheritdoc/>
        public virtual bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set
            {
                isSingleOrDouble = value;
                NotifyChanged();
            }
        }

        /// <inheritdoc/>
        public virtual IList<IAtom> Atoms => atoms;

        /// <inheritdoc/>
        public virtual IList<IBond> Bonds => bonds;

        /// <inheritdoc/>
        public virtual IList<ILonePair> LonePairs => lonePairs;

        /// <inheritdoc/>
        public virtual IList<ISingleElectron> SingleElectrons => singleElectrons;

        /// <inheritdoc/>
        public virtual IList<IReadOnlyStereoElement<IChemObject, IChemObject>> StereoElements => stereoElements;

        /// <inheritdoc/>
        public virtual void SetStereoElements(IEnumerable<IReadOnlyStereoElement<IChemObject, IChemObject>> elements)
		{
			stereoElements = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(elements);
		}

        /// <summary>
        /// Returns the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connects the two atoms</returns>
        public virtual IBond GetBond(IAtom atom1, IAtom atom2)
        {
            return bonds.Where(bond => bond.Contains(atom1) && bond.GetOther(atom1).Equals(atom2)).FirstOrDefault();
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
        {
            if (!atoms.Contains(atom))
                throw new NoSuchAtomException("Atom does not belong to the container!");
            return Bonds.Where(n => n.Contains(atom)).Select(n => n.GetOther(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IBond> GetConnectedBonds(IAtom atom)
        {
            if (!atoms.Contains(atom))
                throw new NoSuchAtomException("Atom does not belong to the container!");
            return bonds.Where(bond => bond.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ILonePair> GetConnectedLonePairs(IAtom atom)
        {
            if (!atoms.Contains(atom))
                throw new NoSuchAtomException("Atom does not belong to the container!");
            return LonePairs.Where(lonePair => lonePair.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ISingleElectron> GetConnectedSingleElectrons(IAtom atom)
        {
            if (!atoms.Contains(atom))
                throw new NoSuchAtomException("Atom does not belong to the container!");
            return SingleElectrons.Where(singleElectron => singleElectron.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IElectronContainer> GetConnectedElectronContainers(IAtom atom)
        {
            foreach (var e in GetConnectedBonds(atom))
                yield return e;
            foreach (var e in GetConnectedLonePairs(atom))
                yield return e;
            foreach (var e in GetConnectedSingleElectrons(atom))
                yield return e;
            yield break;
        }

        private IEnumerable<BondOrder> GetBondOrders(IAtom atom)
        {
            return bonds.Where(bond => bond.Contains(atom))
                .Select(bond => bond.Order)
                .Where(order => !order.IsUnset);
        }

        /// <inheritdoc/>
        public virtual double GetBondOrderSum(IAtom atom)
        {
            return GetBondOrders(atom).Select(order => order.Numeric).Sum();
        }

        /// <inheritdoc/>
        public virtual BondOrder GetMaximumBondOrder(IAtom atom)
        {
			BondOrder max = BondOrder.Unset;
			foreach (IBond bond in Bonds)
			{
				if (!bond.Contains(atom))
					continue;
				if (max == BondOrder.Unset || bond.Order.Numeric > max.Numeric) 
				{
					max = bond.Order;
				}
			}
			if (max == BondOrder.Unset)
			{
				if (!Contains(atom))
					throw new NoSuchAtomException("Atom does not belong to this container!");
				if (atom.ImplicitHydrogenCount != null &&
					atom.ImplicitHydrogenCount > 0)
					max = BondOrder.Single;
				else
					max = BondOrder.Unset;
			}
			return max;
        }

        /// <inheritdoc/>
        public virtual BondOrder GetMinimumBondOrder(IAtom atom)
        {
			BondOrder min = BondOrder.Unset;
			foreach (IBond bond in Bonds) 
			{
				if (!bond.Contains(atom))
					continue;
				if (min == BondOrder.Unset || bond.Order.Numeric < min.Numeric) 
				{
					min = bond.Order;
				}
			}
			if (min == BondOrder.Unset) 
			{
				if (!Contains(atom))
					throw new NoSuchAtomException("Atom does not belong to this container!");
				if (atom.ImplicitHydrogenCount != null &&
					atom.ImplicitHydrogenCount > 0)
					min = BondOrder.Single;
				else
					min = BondOrder.Unset;
			}
			return min;
        }

        /// <inheritdoc/>
        public virtual void Add(IAtomContainer that)
        {
			foreach (IAtom atom in that.Atoms)
                atom.IsVisited = false;
            foreach (IBond bond in that.Bonds)
                bond.IsVisited = false;
            foreach (IAtom atom in this.Atoms)
                atom.IsVisited = true;
            foreach (IBond bond in this.Bonds)
                bond.IsVisited = true;

            foreach (var atom in that.Atoms.Where(atom => !atom.IsVisited))
                Atoms.Add(atom);
            foreach (var bond in that.Bonds.Where(bond => !bond.IsVisited))
                Bonds.Add(bond);
            foreach (var lonePair in that.LonePairs.Where(lonePair => !lonePair.IsVisited))
                LonePairs.Add(lonePair);
            foreach (var singleElectron in that.SingleElectrons.Where(singleElectron => !Contains(singleElectron)))
                SingleElectrons.Add(singleElectron);
            foreach (var se in that.StereoElements)
                stereoElements.Add(se);

             NotifyChanged();         }

        /// <inheritdoc/>
        public virtual void AddElectronContainer(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
            {
                Bonds.Add(bond);
                return;
            }
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
            { 
                LonePairs.Add(lonePair);
                return;
            }
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
            {
                SingleElectrons.Add(singleElectron);
                return;
            }
        }

        /// <inheritdoc/>
        public virtual void Remove(IAtomContainer atomContainer)
        {
            foreach (var atom in atomContainer.Atoms)
                Atoms.Remove(atom);
            foreach (var bond in atomContainer.Bonds)
                Bonds.Remove(bond);
            foreach (var lonePair in atomContainer.LonePairs)
                LonePairs.Remove(lonePair);
            foreach (var singleElectron in atomContainer.SingleElectrons)
                SingleElectrons.Remove(singleElectron);
        }

        /// <inheritdoc/>
        public virtual void RemoveElectronContainer(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
            {
                Bonds.Remove(bond);
                return;
            }
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
            {
                LonePairs.Remove(lonePair);
                return;
            }
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
            {
                SingleElectrons.Remove(singleElectron);
                return;
            }
        }

        /// <inheritdoc/>
		[Obsolete]
        public virtual void RemoveAtomAndConnectedElectronContainers(IAtom atom)
        {
			RemoveAtom(atom);
		}

		/// <inheritdoc/>
        public virtual void RemoveAtom(IAtom atom)
		{
            {
                var toRemove = bonds.Where(bond => bond.Contains(atom)).ToList();
                foreach (var bond in toRemove)
                    bonds.Remove(bond);
            }
            {
                var toRemove = lonePairs.Where(lonePair => lonePair.Contains(atom)).ToList();
                foreach (var lonePair in toRemove)
                    lonePairs.Remove(lonePair);
            }
            {
                var toRemove = singleElectrons.Where(singleElectron => singleElectron.Contains(atom)).ToList();
                foreach (var singleElectron in toRemove)
                    singleElectrons.Remove(singleElectron);
            }
            {
                var toRemove = stereoElements.Where(stereoElement => stereoElement.Contains(atom)).ToList();
                foreach (var stereoElement in toRemove)
                    stereoElements.Remove(stereoElement);
            }

            Atoms.Remove(atom);

             NotifyChanged();         }

		/// <inheritdoc/>
		public virtual void RemoveAtom(int pos) 
		{
			RemoveAtom(Atoms[pos]);
		}

        /// <inheritdoc/>
        public virtual void RemoveAllElements()
        {
            RemoveAllElectronContainers();
            foreach (var atom in atoms)
                atom.Listeners?.Remove(this);
            atoms.Clear();
            stereoElements.Clear();

             NotifyChanged();         }

        /// <inheritdoc/>
        public virtual void RemoveAllElectronContainers()
        {
            RemoveAllBonds();
            foreach (var e in lonePairs)
                e.Listeners?.Remove(this);
            foreach (var e in singleElectrons)
                e.Listeners?.Remove(this);
            lonePairs.Clear();
            singleElectrons.Clear();

             NotifyChanged();         }

        /// <inheritdoc/>
        public virtual void RemoveAllBonds()
        {
            foreach (var e in bonds)
                e.Listeners?.Remove(this);
            bonds.Clear();
             NotifyChanged();         }

        /// <inheritdoc/>
        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            var bond = Builder.NewBond(atom1, atom2, order, stereo);
            Bonds.Add(bond);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            IBond bond = Builder.NewBond(atom1, atom2, order);
            Bonds.Add(bond);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddLonePairTo(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.NewLonePair(atom);
            e.Listeners?.Add(this);
            LonePairs.Add(e);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddSingleElectronTo(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.NewSingleElectron(atom);
            e.Listeners?.Add(this);
            SingleElectrons.Add(e);
            // no OnStateChanged
        }

        /// <summary>
        /// Removes the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connects the two atoms</returns>
        public virtual IBond RemoveBond(IAtom atom1, IAtom atom2)
        {
            var bond = GetBond(atom1, atom2);
            if (bond != null)
                Bonds.Remove(bond);
            return bond;
        }

        /// <inheritdoc/>
        public virtual bool Contains(IAtom atom) => atoms.Any(n => n.Equals(atom));

        /// <inheritdoc/>
        public virtual bool Contains(IBond bond) => bonds.Any(n => n.Equals(bond));

        /// <inheritdoc/>
        public virtual bool Contains(ILonePair lonePair) => lonePairs.Any(n => n == lonePair);

        /// <inheritdoc/>
        public virtual bool Contains(ISingleElectron singleElectron) => singleElectrons.Any(n => n == singleElectron);

        /// <inheritdoc/>
        public virtual bool Contains(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
                return Contains(bond);
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
                return Contains(lonePair);
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
                return Contains(singleElectron);
            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AtomContainer(");
            sb.Append(ToStringInner());
            sb.Append(')');
            return sb.ToString();
        }

        internal virtual string ToStringInner()
        {
            var sb = new StringBuilder();
            sb.Append(GetHashCode());
            Append(sb, atoms, "A");
            Append(sb, bonds, "B");
            Append(sb, lonePairs, "LP");
            Append(sb, singleElectrons, "SE");
            if (stereoElements.Count > 0)
            {
                sb.Append(", ST:[#").Append(stereoElements.Count);
                foreach (var elements in stereoElements)
                    sb.Append(", ").Append(elements.ToString());
                sb.Append(']');
            }
            return sb.ToString();
        }

        private void Append<T>(StringBuilder sb, ICollection<T> os, string tag)
        {
            if (os.Count > 0)
            {
                sb.Append(", #").Append(tag).Append(":").Append(os.Count);
                foreach (var e in os)
                    sb.Append(", ").Append(e.ToString());
            }
        }

        /// <inheritdoc/>
        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (AtomContainer)base.Clone(map);            
            clone.atoms = CreateObservableChemObjectCollection(atoms.Where(n => n != null).Select(n => (IAtom)n.Clone(map)), false);
            clone.bonds = CreateObservableChemObjectCollection(bonds.Where(n => n != null).Select(n => (IBond)n.Clone(map)), true);
            clone.lonePairs = CreateObservableChemObjectCollection(lonePairs.Where(n => n != null).Select(n => (ILonePair)n.Clone(map)), true);
            clone.singleElectrons = CreateObservableChemObjectCollection(singleElectrons.Where(n => n != null).Select(n => (ISingleElectron)n.Clone(map)), true);
            clone.stereoElements = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(stereoElements.Select(n => (IReadOnlyStereoElement<IChemObject, IChemObject>)n.Clone(map)));

            return clone;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IElectronContainer> GetElectronContainers()
        {
            return bonds.Cast<IElectronContainer>().Concat(LonePairs).Concat(SingleElectrons);
        }

        /// <inheritdoc/>
        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
             NotifyChanged(evt);         }

        /// <inheritdoc/>
        public void SetAtoms(IEnumerable<IAtom> atoms)
        {
            this.atoms.Clear();
            foreach (var atom in atoms)
                this.atoms.Add(atom);
        }

        /// <inheritdoc/>
        public void SetBonds(IEnumerable<IBond> bonds)
        {
            this.bonds.Clear();
            foreach (var bond in bonds)
                this.bonds.Add(bond);
        }

		public void SetAtom(int idx, IAtom atom)
        {
            if (idx >= atoms.Count)
                throw new IndexOutOfRangeException("No atom at index: " + idx);
            int aidx = atoms.IndexOf(atom);
            if (aidx >= 0)
                throw new ArgumentException("Atom already in container at index: " + idx, nameof(atom));
            IAtom oldAtom = atoms[idx];
            atoms[idx] = atom;
		 
            atom.Listeners.Add(this);
            oldAtom.Listeners.Remove(this);
		
            // replace in electron containers
            foreach (var bond in bonds)
            {
                for (int i = 0; i < bond.Atoms.Count; i++)
                {
                    if (oldAtom.Equals(bond.Atoms[i]))
                    {
                        bond.Atoms[i] = atom;
                    }
                }
            }
            foreach (var ec in singleElectrons)
            {
                if (oldAtom.Equals(ec.Atom))
                    ec.Atom = atom; 
            }
            foreach (var lp in lonePairs)
            {
                if (oldAtom.Equals(lp.Atom))
                    lp.Atom = atom;
            }

            // update stereo
            CDKObjectMap map = null;
            List<IReadOnlyStereoElement<IChemObject, IChemObject>> oldStereo = null;
            List<IReadOnlyStereoElement<IChemObject, IChemObject>> newStereo = null;

            foreach (var se in stereoElements)
            {
                if (se.Contains(oldAtom))
                {
                    if (oldStereo == null)
                    {
                        oldStereo = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>();
                        newStereo = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>();
                        map = new CDKObjectMap();
                        foreach (var a in atoms)
                            map.Add(a, a);
                        map.Set(oldAtom, atom);
                    }
                    oldStereo.Add(se);
                    newStereo.Add((IReadOnlyStereoElement<IChemObject, IChemObject>)se.Clone(map));
                }
            }
            if (oldStereo != null)
            {
                foreach (var stereo in oldStereo)
                    stereoElements.Remove(stereo);
                foreach (var stereo in newStereo)
                    stereoElements.Add(stereo);
            }

             NotifyChanged();         }

        /// <inheritdoc/>
        public virtual bool IsEmpty() => atoms.Count == 0;

        /// <inheritdoc/>
        public virtual string Title 
		{ 
			get { return  GetProperty<string>(CDKPropertyName.Title); }
			set { SetProperty(CDKPropertyName.Title, value); }
		}
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Base class for all chemical objects that maintain a list of Atoms and
    /// ElectronContainers. 
    /// </summary>
    /// <example>
    /// Looping over all Bonds in the AtomContainer is typically done like: 
    /// <code>
    /// foreach (IBond aBond in atomContainer.Bonds)
    /// {
    ///         // do something
    /// }
    /// </code>
    /// </example>
    // @cdk.githash 
    // @author steinbeck
    // @cdk.created 2000-10-02 
    [Serializable]
    public class AtomContainer
        : ChemObject, IAtomContainer, IChemObjectListener
    {
        /// <summary>
        /// Atoms contained by this object.
        /// </summary>
        internal ObservableChemObjectCollection<IAtom> atoms;

        /// <summary>
        /// Bonds contained by this object.
        /// </summary>
        internal ObservableChemObjectCollection<IBond> bonds;

        /// <summary>
        /// Lone pairs contained by this object.
        /// </summary>
        internal ObservableChemObjectCollection<ILonePair> lonePairs;

        /// <summary>
        /// Single electrons contained by this object.
        /// </summary>
        internal ObservableChemObjectCollection<ISingleElectron> singleElectrons;

        /// <summary>
        /// Stereo elements contained by this object.
        /// </summary>
        internal IList<IReadOnlyStereoElement<IChemObject, IChemObject>> stereoElements;

        internal bool isAromatic;
        internal bool isSingleOrDouble;

        private void Init(
            ObservableChemObjectCollection<IAtom> atoms,
            ObservableChemObjectCollection<IBond> bonds,
            ObservableChemObjectCollection<ILonePair> lonePairs,
            ObservableChemObjectCollection<ISingleElectron> singleElectrons,
            IList<IReadOnlyStereoElement<IChemObject, IChemObject>> stereoElements)
        {
            this.atoms = atoms;
            this.bonds = bonds;
            this.lonePairs = lonePairs;
            this.singleElectrons = singleElectrons;
            this.stereoElements = stereoElements;
        }

        public AtomContainer(
            IEnumerable<IAtom> atoms,
            IEnumerable<IBond> bonds,
            IEnumerable<ILonePair> lonePairs,
            IEnumerable<ISingleElectron> singleElectrons,
            IEnumerable<IReadOnlyStereoElement<IChemObject, IChemObject>> stereoElements)
        {
            Init(
                CreateObservableChemObjectCollection(atoms, false),
                CreateObservableChemObjectCollection(bonds, true),
                CreateObservableChemObjectCollection(lonePairs, true),
                CreateObservableChemObjectCollection(singleElectrons, true),
                new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(stereoElements)
            );
        }

        private ObservableChemObjectCollection<T> CreateObservableChemObjectCollection<T>(IEnumerable<T> objs, bool allowDup) where T : IChemObject
        {
 
            var list = new ObservableChemObjectCollection<T>(null, objs);
            list.AllowDuplicate = allowDup;
            return list;
        }

        public AtomContainer(
           IEnumerable<IAtom> atoms,
           IEnumerable<IBond> bonds)
             : this(
                  atoms,
                  bonds,
                  Array.Empty<ILonePair>(),
                  Array.Empty<ISingleElectron>(),
                  Array.Empty<IReadOnlyStereoElement<IChemObject, IChemObject>>())
        { }

        /// <summary>
        ///  Constructs an empty AtomContainer.
        /// </summary>
        public AtomContainer()
            : this(
                      Array.Empty<IAtom>(), 
                      Array.Empty<IBond>(), 
                      Array.Empty<ILonePair>(),
                      Array.Empty<ISingleElectron>(),
                      Array.Empty<IReadOnlyStereoElement<IChemObject, IChemObject>>())
        { }

        /// <summary>
        /// Constructs an AtomContainer with a copy of the atoms and electronContainers
        /// of another AtomContainer (A shallow copy, i.e., with the same objects as in
        /// the original AtomContainer).
        /// </summary>
        /// <param name="container">An AtomContainer to copy the atoms and electronContainers from</param>
        public AtomContainer(IAtomContainer container)
            : this(
                  container.Atoms,
                  container.Bonds,
                  container.LonePairs,
                  container.SingleElectrons,
                  container.StereoElements)
        { }

        /// <inheritdoc/>
        public virtual bool IsAromatic
        {
            get { return isAromatic; }
            set
            {
                isAromatic = value; 
            }
        }
        
        /// <inheritdoc/>
        public virtual bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set
            {
                isSingleOrDouble = value;
            }
        }

        /// <inheritdoc/>
        public virtual IList<IAtom> Atoms => atoms;

        /// <inheritdoc/>
        public virtual IList<IBond> Bonds => bonds;

        /// <inheritdoc/>
        public virtual IList<ILonePair> LonePairs => lonePairs;

        /// <inheritdoc/>
        public virtual IList<ISingleElectron> SingleElectrons => singleElectrons;

        /// <inheritdoc/>
        public virtual IList<IReadOnlyStereoElement<IChemObject, IChemObject>> StereoElements => stereoElements;

        /// <inheritdoc/>
        public virtual void SetStereoElements(IEnumerable<IReadOnlyStereoElement<IChemObject, IChemObject>> elements)
		{
			stereoElements = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(elements);
		}

        /// <summary>
        /// Returns the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connects the two atoms</returns>
        public virtual IBond GetBond(IAtom atom1, IAtom atom2)
        {
            return bonds.Where(bond => bond.Contains(atom1) && bond.GetOther(atom1).Equals(atom2)).FirstOrDefault();
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
        {
            if (!atoms.Contains(atom))
                throw new NoSuchAtomException("Atom does not belong to the container!");
            return Bonds.Where(n => n.Contains(atom)).Select(n => n.GetOther(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IBond> GetConnectedBonds(IAtom atom)
        {
            if (!atoms.Contains(atom))
                throw new NoSuchAtomException("Atom does not belong to the container!");
            return bonds.Where(bond => bond.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ILonePair> GetConnectedLonePairs(IAtom atom)
        {
            if (!atoms.Contains(atom))
                throw new NoSuchAtomException("Atom does not belong to the container!");
            return LonePairs.Where(lonePair => lonePair.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ISingleElectron> GetConnectedSingleElectrons(IAtom atom)
        {
            if (!atoms.Contains(atom))
                throw new NoSuchAtomException("Atom does not belong to the container!");
            return SingleElectrons.Where(singleElectron => singleElectron.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IElectronContainer> GetConnectedElectronContainers(IAtom atom)
        {
            foreach (var e in GetConnectedBonds(atom))
                yield return e;
            foreach (var e in GetConnectedLonePairs(atom))
                yield return e;
            foreach (var e in GetConnectedSingleElectrons(atom))
                yield return e;
            yield break;
        }

        private IEnumerable<BondOrder> GetBondOrders(IAtom atom)
        {
            return bonds.Where(bond => bond.Contains(atom))
                .Select(bond => bond.Order)
                .Where(order => !order.IsUnset);
        }

        /// <inheritdoc/>
        public virtual double GetBondOrderSum(IAtom atom)
        {
            return GetBondOrders(atom).Select(order => order.Numeric).Sum();
        }

        /// <inheritdoc/>
        public virtual BondOrder GetMaximumBondOrder(IAtom atom)
        {
			BondOrder max = BondOrder.Unset;
			foreach (IBond bond in Bonds)
			{
				if (!bond.Contains(atom))
					continue;
				if (max == BondOrder.Unset || bond.Order.Numeric > max.Numeric) 
				{
					max = bond.Order;
				}
			}
			if (max == BondOrder.Unset)
			{
				if (!Contains(atom))
					throw new NoSuchAtomException("Atom does not belong to this container!");
				if (atom.ImplicitHydrogenCount != null &&
					atom.ImplicitHydrogenCount > 0)
					max = BondOrder.Single;
				else
					max = BondOrder.Unset;
			}
			return max;
        }

        /// <inheritdoc/>
        public virtual BondOrder GetMinimumBondOrder(IAtom atom)
        {
			BondOrder min = BondOrder.Unset;
			foreach (IBond bond in Bonds) 
			{
				if (!bond.Contains(atom))
					continue;
				if (min == BondOrder.Unset || bond.Order.Numeric < min.Numeric) 
				{
					min = bond.Order;
				}
			}
			if (min == BondOrder.Unset) 
			{
				if (!Contains(atom))
					throw new NoSuchAtomException("Atom does not belong to this container!");
				if (atom.ImplicitHydrogenCount != null &&
					atom.ImplicitHydrogenCount > 0)
					min = BondOrder.Single;
				else
					min = BondOrder.Unset;
			}
			return min;
        }

        /// <inheritdoc/>
        public virtual void Add(IAtomContainer that)
        {
			foreach (IAtom atom in that.Atoms)
                atom.IsVisited = false;
            foreach (IBond bond in that.Bonds)
                bond.IsVisited = false;
            foreach (IAtom atom in this.Atoms)
                atom.IsVisited = true;
            foreach (IBond bond in this.Bonds)
                bond.IsVisited = true;

            foreach (var atom in that.Atoms.Where(atom => !atom.IsVisited))
                Atoms.Add(atom);
            foreach (var bond in that.Bonds.Where(bond => !bond.IsVisited))
                Bonds.Add(bond);
            foreach (var lonePair in that.LonePairs.Where(lonePair => !lonePair.IsVisited))
                LonePairs.Add(lonePair);
            foreach (var singleElectron in that.SingleElectrons.Where(singleElectron => !Contains(singleElectron)))
                SingleElectrons.Add(singleElectron);
            foreach (var se in that.StereoElements)
                stereoElements.Add(se);

                    }

        /// <inheritdoc/>
        public virtual void AddElectronContainer(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
            {
                Bonds.Add(bond);
                return;
            }
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
            { 
                LonePairs.Add(lonePair);
                return;
            }
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
            {
                SingleElectrons.Add(singleElectron);
                return;
            }
        }

        /// <inheritdoc/>
        public virtual void Remove(IAtomContainer atomContainer)
        {
            foreach (var atom in atomContainer.Atoms)
                Atoms.Remove(atom);
            foreach (var bond in atomContainer.Bonds)
                Bonds.Remove(bond);
            foreach (var lonePair in atomContainer.LonePairs)
                LonePairs.Remove(lonePair);
            foreach (var singleElectron in atomContainer.SingleElectrons)
                SingleElectrons.Remove(singleElectron);
        }

        /// <inheritdoc/>
        public virtual void RemoveElectronContainer(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
            {
                Bonds.Remove(bond);
                return;
            }
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
            {
                LonePairs.Remove(lonePair);
                return;
            }
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
            {
                SingleElectrons.Remove(singleElectron);
                return;
            }
        }

        /// <inheritdoc/>
		[Obsolete]
        public virtual void RemoveAtomAndConnectedElectronContainers(IAtom atom)
        {
			RemoveAtom(atom);
		}

		/// <inheritdoc/>
        public virtual void RemoveAtom(IAtom atom)
		{
            {
                var toRemove = bonds.Where(bond => bond.Contains(atom)).ToList();
                foreach (var bond in toRemove)
                    bonds.Remove(bond);
            }
            {
                var toRemove = lonePairs.Where(lonePair => lonePair.Contains(atom)).ToList();
                foreach (var lonePair in toRemove)
                    lonePairs.Remove(lonePair);
            }
            {
                var toRemove = singleElectrons.Where(singleElectron => singleElectron.Contains(atom)).ToList();
                foreach (var singleElectron in toRemove)
                    singleElectrons.Remove(singleElectron);
            }
            {
                var toRemove = stereoElements.Where(stereoElement => stereoElement.Contains(atom)).ToList();
                foreach (var stereoElement in toRemove)
                    stereoElements.Remove(stereoElement);
            }

            Atoms.Remove(atom);

                    }

		/// <inheritdoc/>
		public virtual void RemoveAtom(int pos) 
		{
			RemoveAtom(Atoms[pos]);
		}

        /// <inheritdoc/>
        public virtual void RemoveAllElements()
        {
            RemoveAllElectronContainers();
            foreach (var atom in atoms)
                atom.Listeners?.Remove(this);
            atoms.Clear();
            stereoElements.Clear();

                    }

        /// <inheritdoc/>
        public virtual void RemoveAllElectronContainers()
        {
            RemoveAllBonds();
            foreach (var e in lonePairs)
                e.Listeners?.Remove(this);
            foreach (var e in singleElectrons)
                e.Listeners?.Remove(this);
            lonePairs.Clear();
            singleElectrons.Clear();

                    }

        /// <inheritdoc/>
        public virtual void RemoveAllBonds()
        {
            foreach (var e in bonds)
                e.Listeners?.Remove(this);
            bonds.Clear();
                    }

        /// <inheritdoc/>
        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            var bond = Builder.NewBond(atom1, atom2, order, stereo);
            Bonds.Add(bond);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            IBond bond = Builder.NewBond(atom1, atom2, order);
            Bonds.Add(bond);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddLonePairTo(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.NewLonePair(atom);
            e.Listeners?.Add(this);
            LonePairs.Add(e);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddSingleElectronTo(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.NewSingleElectron(atom);
            e.Listeners?.Add(this);
            SingleElectrons.Add(e);
            // no OnStateChanged
        }

        /// <summary>
        /// Removes the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connects the two atoms</returns>
        public virtual IBond RemoveBond(IAtom atom1, IAtom atom2)
        {
            var bond = GetBond(atom1, atom2);
            if (bond != null)
                Bonds.Remove(bond);
            return bond;
        }

        /// <inheritdoc/>
        public virtual bool Contains(IAtom atom) => atoms.Any(n => n.Equals(atom));

        /// <inheritdoc/>
        public virtual bool Contains(IBond bond) => bonds.Any(n => n.Equals(bond));

        /// <inheritdoc/>
        public virtual bool Contains(ILonePair lonePair) => lonePairs.Any(n => n == lonePair);

        /// <inheritdoc/>
        public virtual bool Contains(ISingleElectron singleElectron) => singleElectrons.Any(n => n == singleElectron);

        /// <inheritdoc/>
        public virtual bool Contains(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
                return Contains(bond);
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
                return Contains(lonePair);
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
                return Contains(singleElectron);
            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AtomContainer(");
            sb.Append(ToStringInner());
            sb.Append(')');
            return sb.ToString();
        }

        internal virtual string ToStringInner()
        {
            var sb = new StringBuilder();
            sb.Append(GetHashCode());
            Append(sb, atoms, "A");
            Append(sb, bonds, "B");
            Append(sb, lonePairs, "LP");
            Append(sb, singleElectrons, "SE");
            if (stereoElements.Count > 0)
            {
                sb.Append(", ST:[#").Append(stereoElements.Count);
                foreach (var elements in stereoElements)
                    sb.Append(", ").Append(elements.ToString());
                sb.Append(']');
            }
            return sb.ToString();
        }

        private void Append<T>(StringBuilder sb, ICollection<T> os, string tag)
        {
            if (os.Count > 0)
            {
                sb.Append(", #").Append(tag).Append(":").Append(os.Count);
                foreach (var e in os)
                    sb.Append(", ").Append(e.ToString());
            }
        }

        /// <inheritdoc/>
        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (AtomContainer)base.Clone(map);            
            clone.atoms = CreateObservableChemObjectCollection(atoms.Where(n => n != null).Select(n => (IAtom)n.Clone(map)), false);
            clone.bonds = CreateObservableChemObjectCollection(bonds.Where(n => n != null).Select(n => (IBond)n.Clone(map)), true);
            clone.lonePairs = CreateObservableChemObjectCollection(lonePairs.Where(n => n != null).Select(n => (ILonePair)n.Clone(map)), true);
            clone.singleElectrons = CreateObservableChemObjectCollection(singleElectrons.Where(n => n != null).Select(n => (ISingleElectron)n.Clone(map)), true);
            clone.stereoElements = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(stereoElements.Select(n => (IReadOnlyStereoElement<IChemObject, IChemObject>)n.Clone(map)));

            return clone;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IElectronContainer> GetElectronContainers()
        {
            return bonds.Cast<IElectronContainer>().Concat(LonePairs).Concat(SingleElectrons);
        }

        /// <inheritdoc/>
        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                    }

        /// <inheritdoc/>
        public void SetAtoms(IEnumerable<IAtom> atoms)
        {
            this.atoms.Clear();
            foreach (var atom in atoms)
                this.atoms.Add(atom);
        }

        /// <inheritdoc/>
        public void SetBonds(IEnumerable<IBond> bonds)
        {
            this.bonds.Clear();
            foreach (var bond in bonds)
                this.bonds.Add(bond);
        }

		public void SetAtom(int idx, IAtom atom)
        {
            if (idx >= atoms.Count)
                throw new IndexOutOfRangeException("No atom at index: " + idx);
            int aidx = atoms.IndexOf(atom);
            if (aidx >= 0)
                throw new ArgumentException("Atom already in container at index: " + idx, nameof(atom));
            IAtom oldAtom = atoms[idx];
            atoms[idx] = atom;
		
            // replace in electron containers
            foreach (var bond in bonds)
            {
                for (int i = 0; i < bond.Atoms.Count; i++)
                {
                    if (oldAtom.Equals(bond.Atoms[i]))
                    {
                        bond.Atoms[i] = atom;
                    }
                }
            }
            foreach (var ec in singleElectrons)
            {
                if (oldAtom.Equals(ec.Atom))
                    ec.Atom = atom; 
            }
            foreach (var lp in lonePairs)
            {
                if (oldAtom.Equals(lp.Atom))
                    lp.Atom = atom;
            }

            // update stereo
            CDKObjectMap map = null;
            List<IReadOnlyStereoElement<IChemObject, IChemObject>> oldStereo = null;
            List<IReadOnlyStereoElement<IChemObject, IChemObject>> newStereo = null;

            foreach (var se in stereoElements)
            {
                if (se.Contains(oldAtom))
                {
                    if (oldStereo == null)
                    {
                        oldStereo = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>();
                        newStereo = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>();
                        map = new CDKObjectMap();
                        foreach (var a in atoms)
                            map.Add(a, a);
                        map.Set(oldAtom, atom);
                    }
                    oldStereo.Add(se);
                    newStereo.Add((IReadOnlyStereoElement<IChemObject, IChemObject>)se.Clone(map));
                }
            }
            if (oldStereo != null)
            {
                foreach (var stereo in oldStereo)
                    stereoElements.Remove(stereo);
                foreach (var stereo in newStereo)
                    stereoElements.Add(stereo);
            }

                    }

        /// <inheritdoc/>
        public virtual bool IsEmpty() => atoms.Count == 0;

        /// <inheritdoc/>
        public virtual string Title 
		{ 
			get { return  GetProperty<string>(CDKPropertyName.Title); }
			set { SetProperty(CDKPropertyName.Title, value); }
		}
    }
}
