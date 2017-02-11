















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;

namespace NCDK.Default
{
    [Serializable]
    public class AtomContainer
        : ChemObject, IAtomContainer, IChemObjectListener, ICloneable
    {
        internal IList<IAtom> atoms;
        internal IList<IBond> bonds;
        internal IList<ILonePair> lonePairs;
        internal IList<ISingleElectron> singleElectrons;
        internal IList<IStereoElement> stereoElements;

		internal bool isAromatic;
        internal bool isSingleOrDouble;

		private void Init(
            IList<IAtom> atoms,
            IList<IBond> bonds,
            IList<ILonePair> lonePairs,
            IList<ISingleElectron> singleElectrons,
            IList<IStereoElement> stereoElements)
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
            IEnumerable<IStereoElement> stereoElements)
		{
			Init(
				CreateObservableChemObjectCollection(atoms, false),
				CreateObservableChemObjectCollection(bonds, true),
				CreateObservableChemObjectCollection(lonePairs, true),
				CreateObservableChemObjectCollection(singleElectrons, true),
				new List<IStereoElement>(stereoElements)
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
                  Array.Empty<IStereoElement>())
        { }

		public AtomContainer()
			: this(
					  Array.Empty<IAtom>(), 
					  Array.Empty<IBond>(), 
					  Array.Empty<ILonePair>(),
					  Array.Empty<ISingleElectron>(),
					  Array.Empty<IStereoElement>())
		{ }

		public AtomContainer(IAtomContainer container)
            : this(
                  container.Atoms,
                  container.Bonds,
                  container.LonePairs,
                  container.SingleElectrons,
                  container.StereoElements)
        { }

        public virtual bool IsAromatic
        {
            get { return isAromatic; }
            set
            {
                isAromatic = value; 
                NotifyChanged();
            }
        }
		
		public virtual bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set
            {
                isSingleOrDouble = value;
                NotifyChanged();
            }
        }

        public virtual IList<IAtom> Atoms => atoms;
        public virtual IList<IBond> Bonds => bonds;
        public virtual IList<ILonePair> LonePairs => lonePairs;
        public virtual IList<ISingleElectron> SingleElectrons => singleElectrons;
        public virtual IList<IStereoElement> StereoElements => stereoElements;
        public virtual void SetStereoElements(IEnumerable<IStereoElement> elements) => stereoElements = new List<IStereoElement>(elements);
        public virtual void AddStereoElement(IStereoElement element) => stereoElements.Add(element);

        /// <summary>
        /// Returns the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connects the two atoms</returns>
        public virtual IBond GetBond(IAtom atom1, IAtom atom2)
        {
            return bonds.Where(bond => bond.Contains(atom1) && bond.GetConnectedAtom(atom1) == atom2).FirstOrDefault();
        }

        public virtual IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
        {
			foreach (var bond in Bonds)
                if (bond.Contains(atom))
                    yield return bond.GetConnectedAtom(atom);
            yield break;
        }

        public virtual IEnumerable<IBond> GetConnectedBonds(IAtom atom)
        {
            return bonds.Where(bond => bond.Contains(atom));
        }

        public virtual IEnumerable<ILonePair> GetConnectedLonePairs(IAtom atom)
        {
            return LonePairs.Where(lonePair => lonePair.Contains(atom));
        }

        public virtual IEnumerable<ISingleElectron> GetConnectedSingleElectrons(IAtom atom)
        {
            return SingleElectrons.Where(singleElectron => singleElectron.Contains(atom));
        }

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

        public virtual double GetBondOrderSum(IAtom atom)
        {
            return GetBondOrders(atom).Select(order => order.Numeric).Sum();
        }

        public virtual BondOrder GetMaximumBondOrder(IAtom atom)
        {
			var max = BondOrder.Single;
			foreach (var order in GetBondOrders(atom))
			{
				if (max.Numeric < order.Numeric)
					max = order;
			}
			return max;
		}

        public virtual BondOrder GetMinimumBondOrder(IAtom atom)
        {
            var min = BondOrder.Quadruple;
			foreach (var order in GetBondOrders(atom))
            {
                if (min.Numeric > order.Numeric)
                    min = order;
            }
			return min;
        }

        public virtual void Add(IAtomContainer atomContainer)
        {
            foreach (var atom in atomContainer.Atoms.Where(atom => !Contains(atom)))
                Add(atom);
            foreach (var bond in atomContainer.Bonds.Where(bond => !Contains(bond)))
                Add(bond);
            foreach (var lonePair in atomContainer.LonePairs.Where(lonePair => !Contains(lonePair)))
                Add(lonePair);
            foreach (var singleElectron in atomContainer.SingleElectrons.Where(singleElectron => !Contains(singleElectron)))
                Add(singleElectron);
            foreach (var se in atomContainer.StereoElements)
                stereoElements.Add(se);

             NotifyChanged();         }

        public virtual void Add(IStereoElement element)
        {
            stereoElements.Add(element);
        }

        public virtual void Add(IAtom atom)
        {
            if (Atoms.Contains(atom))
                return;
            atoms.Add(atom);
        }

        public virtual void Add(IBond bond)
        {
            bonds.Add(bond);
        }

        public virtual void Add(ILonePair lonePair)
        {
            lonePairs.Add(lonePair);
        }

        public virtual void Add(ISingleElectron singleElectron)
        {
            singleElectrons.Add(singleElectron);
        }

        public virtual void Add(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
            {
                Add(bond);
                return;
            }
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
            { 
                Add(lonePair);
                return;
            }
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
            {
                Add(singleElectron);
                return;
            }
        }

        public virtual void Remove(IAtomContainer atomContainer)
        {
            foreach (var atom in atomContainer.Atoms)
                Remove(atom);
            foreach (var bond in atomContainer.Bonds)
                Remove(bond);
            foreach (var lonePair in atomContainer.LonePairs)
                Remove(lonePair);
            foreach (var singleElectron in atomContainer.SingleElectrons)
                Remove(singleElectron);
        }

        public virtual void Remove(IAtom atom)
        {
            atom.Listeners?.Remove(this);
            atoms.Remove(atom);
        }

        public virtual void Remove(IBond bond)
        {
            bond.Listeners?.Remove(this);
            bonds.Remove(bond);
        }

        public virtual void Remove(ILonePair lonePair)
        {
            lonePair.Listeners?.Remove(this);
            lonePairs.Remove(lonePair);
        }

        public virtual void Remove(ISingleElectron singleElectron)
        {
            singleElectron.Listeners?.Remove(this);
            singleElectrons.Remove(singleElectron);
        }

        public virtual void Remove(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
			{
                Remove(bond);
				return;
			}
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
			{
                Remove(lonePair);
				return;
			}
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
			{
                Remove(singleElectron);
				return;
			}
        }

        public virtual void RemoveAtomAndConnectedElectronContainers(IAtom atom)
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

            Remove(atom);

             NotifyChanged();         }

        public virtual void RemoveAllElements()
        {
            RemoveAllElectronContainers();
            foreach (var atom in atoms)
                atom.Listeners?.Remove(this);
            atoms.Clear();
            stereoElements.Clear();

             NotifyChanged();         }

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

        public virtual void RemoveAllBonds()
        {
            foreach (var e in bonds)
                e.Listeners?.Remove(this);
            bonds.Clear();
             NotifyChanged();         }

        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            var bond = Builder.CreateBond(atom1, atom2, order, stereo);
            Add(bond);
            // no OnStateChanged
        }

        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            IBond bond = Builder.CreateBond(atom1, atom2, order);
            Add(bond);
            // no OnStateChanged
        }

        public virtual void AddLonePair(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.CreateLonePair(atom);
            e.Listeners?.Add(this);
            Add(e);
            // no OnStateChanged
        }

        public virtual void AddSingleElectron(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.CreateSingleElectron(atom);
            e.Listeners?.Add(this);
            Add(e);
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
                Remove(bond);
            return bond;
        }

        public virtual bool Contains(IAtom atom) => atoms.Any(n => n == atom);
        public virtual bool Contains(IBond bond) => bonds.Any(n => n == bond);
        public virtual bool Contains(ILonePair lonePair) => lonePairs.Any(n => n == lonePair);
        public virtual bool Contains(ISingleElectron singleElectron) => singleElectrons.Any(n => n == singleElectron);

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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AtomContainer(");
            sb.Append(ToStringInner());
            sb.Append(')');
            return sb.ToString();
        }

        protected virtual string ToStringInner()
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

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (AtomContainer)base.Clone(map);            
            clone.atoms = CreateObservableChemObjectCollection(atoms.Where(n => n != null).Select(n => (IAtom)n.Clone(map)), false);
            clone.bonds = CreateObservableChemObjectCollection(bonds.Where(n => n != null).Select(n => (IBond)n.Clone(map)), true);
            clone.lonePairs = CreateObservableChemObjectCollection(lonePairs.Where(n => n != null).Select(n => (ILonePair)n.Clone(map)), true);
            clone.singleElectrons = CreateObservableChemObjectCollection(singleElectrons.Where(n => n != null).Select(n => (ISingleElectron)n.Clone(map)), true);
            clone.stereoElements = new List<IStereoElement>(stereoElements.Select(n => (IStereoElement)n.Clone(map)));

            return clone;
        }

        public virtual IEnumerable<IElectronContainer> GetElectronContainers()
        {
            return bonds.Cast<IElectronContainer>().Concat(LonePairs).Concat(SingleElectrons);
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
             NotifyChanged(evt);         }

        public void SetAtoms(IEnumerable<IAtom> atoms)
        {
            this.atoms.Clear();
            foreach (var atom in atoms)
                this.atoms.Add(atom);
        }

        public void SetBonds(IEnumerable<IBond> bonds)
        {
            this.bonds.Clear();
            foreach (var bond in bonds)
                this.bonds.Add(bond);
        }

        public virtual bool IsEmpty => atoms.Count == 0;
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class AtomContainer
        : ChemObject, IAtomContainer, IChemObjectListener, ICloneable
    {
        internal IList<IAtom> atoms;
        internal IList<IBond> bonds;
        internal IList<ILonePair> lonePairs;
        internal IList<ISingleElectron> singleElectrons;
        internal IList<IStereoElement> stereoElements;

		internal bool isAromatic;
        internal bool isSingleOrDouble;

		private void Init(
            IList<IAtom> atoms,
            IList<IBond> bonds,
            IList<ILonePair> lonePairs,
            IList<ISingleElectron> singleElectrons,
            IList<IStereoElement> stereoElements)
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
            IEnumerable<IStereoElement> stereoElements)
		{
			Init(
				CreateObservableChemObjectCollection(atoms, false),
				CreateObservableChemObjectCollection(bonds, true),
				CreateObservableChemObjectCollection(lonePairs, true),
				CreateObservableChemObjectCollection(singleElectrons, true),
				new List<IStereoElement>(stereoElements)
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
                  Array.Empty<IStereoElement>())
        { }

		public AtomContainer()
			: this(
					  Array.Empty<IAtom>(), 
					  Array.Empty<IBond>(), 
					  Array.Empty<ILonePair>(),
					  Array.Empty<ISingleElectron>(),
					  Array.Empty<IStereoElement>())
		{ }

		public AtomContainer(IAtomContainer container)
            : this(
                  container.Atoms,
                  container.Bonds,
                  container.LonePairs,
                  container.SingleElectrons,
                  container.StereoElements)
        { }

        public virtual bool IsAromatic
        {
            get { return isAromatic; }
            set
            {
                isAromatic = value; 
            }
        }
		
		public virtual bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set
            {
                isSingleOrDouble = value;
            }
        }

        public virtual IList<IAtom> Atoms => atoms;
        public virtual IList<IBond> Bonds => bonds;
        public virtual IList<ILonePair> LonePairs => lonePairs;
        public virtual IList<ISingleElectron> SingleElectrons => singleElectrons;
        public virtual IList<IStereoElement> StereoElements => stereoElements;
        public virtual void SetStereoElements(IEnumerable<IStereoElement> elements) => stereoElements = new List<IStereoElement>(elements);
        public virtual void AddStereoElement(IStereoElement element) => stereoElements.Add(element);

        /// <summary>
        /// Returns the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connects the two atoms</returns>
        public virtual IBond GetBond(IAtom atom1, IAtom atom2)
        {
            return bonds.Where(bond => bond.Contains(atom1) && bond.GetConnectedAtom(atom1) == atom2).FirstOrDefault();
        }

        public virtual IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
        {
			foreach (var bond in Bonds)
                if (bond.Contains(atom))
                    yield return bond.GetConnectedAtom(atom);
            yield break;
        }

        public virtual IEnumerable<IBond> GetConnectedBonds(IAtom atom)
        {
            return bonds.Where(bond => bond.Contains(atom));
        }

        public virtual IEnumerable<ILonePair> GetConnectedLonePairs(IAtom atom)
        {
            return LonePairs.Where(lonePair => lonePair.Contains(atom));
        }

        public virtual IEnumerable<ISingleElectron> GetConnectedSingleElectrons(IAtom atom)
        {
            return SingleElectrons.Where(singleElectron => singleElectron.Contains(atom));
        }

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

        public virtual double GetBondOrderSum(IAtom atom)
        {
            return GetBondOrders(atom).Select(order => order.Numeric).Sum();
        }

        public virtual BondOrder GetMaximumBondOrder(IAtom atom)
        {
			var max = BondOrder.Single;
			foreach (var order in GetBondOrders(atom))
			{
				if (max.Numeric < order.Numeric)
					max = order;
			}
			return max;
		}

        public virtual BondOrder GetMinimumBondOrder(IAtom atom)
        {
            var min = BondOrder.Quadruple;
			foreach (var order in GetBondOrders(atom))
            {
                if (min.Numeric > order.Numeric)
                    min = order;
            }
			return min;
        }

        public virtual void Add(IAtomContainer atomContainer)
        {
            foreach (var atom in atomContainer.Atoms.Where(atom => !Contains(atom)))
                Add(atom);
            foreach (var bond in atomContainer.Bonds.Where(bond => !Contains(bond)))
                Add(bond);
            foreach (var lonePair in atomContainer.LonePairs.Where(lonePair => !Contains(lonePair)))
                Add(lonePair);
            foreach (var singleElectron in atomContainer.SingleElectrons.Where(singleElectron => !Contains(singleElectron)))
                Add(singleElectron);
            foreach (var se in atomContainer.StereoElements)
                stereoElements.Add(se);

                    }

        public virtual void Add(IStereoElement element)
        {
            stereoElements.Add(element);
        }

        public virtual void Add(IAtom atom)
        {
            if (Atoms.Contains(atom))
                return;
            atoms.Add(atom);
        }

        public virtual void Add(IBond bond)
        {
            bonds.Add(bond);
        }

        public virtual void Add(ILonePair lonePair)
        {
            lonePairs.Add(lonePair);
        }

        public virtual void Add(ISingleElectron singleElectron)
        {
            singleElectrons.Add(singleElectron);
        }

        public virtual void Add(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
            {
                Add(bond);
                return;
            }
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
            { 
                Add(lonePair);
                return;
            }
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
            {
                Add(singleElectron);
                return;
            }
        }

        public virtual void Remove(IAtomContainer atomContainer)
        {
            foreach (var atom in atomContainer.Atoms)
                Remove(atom);
            foreach (var bond in atomContainer.Bonds)
                Remove(bond);
            foreach (var lonePair in atomContainer.LonePairs)
                Remove(lonePair);
            foreach (var singleElectron in atomContainer.SingleElectrons)
                Remove(singleElectron);
        }

        public virtual void Remove(IAtom atom)
        {
            atom.Listeners?.Remove(this);
            atoms.Remove(atom);
        }

        public virtual void Remove(IBond bond)
        {
            bond.Listeners?.Remove(this);
            bonds.Remove(bond);
        }

        public virtual void Remove(ILonePair lonePair)
        {
            lonePair.Listeners?.Remove(this);
            lonePairs.Remove(lonePair);
        }

        public virtual void Remove(ISingleElectron singleElectron)
        {
            singleElectron.Listeners?.Remove(this);
            singleElectrons.Remove(singleElectron);
        }

        public virtual void Remove(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
			{
                Remove(bond);
				return;
			}
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
			{
                Remove(lonePair);
				return;
			}
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
			{
                Remove(singleElectron);
				return;
			}
        }

        public virtual void RemoveAtomAndConnectedElectronContainers(IAtom atom)
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

            Remove(atom);

                    }

        public virtual void RemoveAllElements()
        {
            RemoveAllElectronContainers();
            foreach (var atom in atoms)
                atom.Listeners?.Remove(this);
            atoms.Clear();
            stereoElements.Clear();

                    }

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

        public virtual void RemoveAllBonds()
        {
            foreach (var e in bonds)
                e.Listeners?.Remove(this);
            bonds.Clear();
                    }

        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            var bond = Builder.CreateBond(atom1, atom2, order, stereo);
            Add(bond);
            // no OnStateChanged
        }

        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            IBond bond = Builder.CreateBond(atom1, atom2, order);
            Add(bond);
            // no OnStateChanged
        }

        public virtual void AddLonePair(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.CreateLonePair(atom);
            e.Listeners?.Add(this);
            Add(e);
            // no OnStateChanged
        }

        public virtual void AddSingleElectron(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.CreateSingleElectron(atom);
            e.Listeners?.Add(this);
            Add(e);
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
                Remove(bond);
            return bond;
        }

        public virtual bool Contains(IAtom atom) => atoms.Any(n => n == atom);
        public virtual bool Contains(IBond bond) => bonds.Any(n => n == bond);
        public virtual bool Contains(ILonePair lonePair) => lonePairs.Any(n => n == lonePair);
        public virtual bool Contains(ISingleElectron singleElectron) => singleElectrons.Any(n => n == singleElectron);

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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AtomContainer(");
            sb.Append(ToStringInner());
            sb.Append(')');
            return sb.ToString();
        }

        protected virtual string ToStringInner()
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

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (AtomContainer)base.Clone(map);            
            clone.atoms = CreateObservableChemObjectCollection(atoms.Where(n => n != null).Select(n => (IAtom)n.Clone(map)), false);
            clone.bonds = CreateObservableChemObjectCollection(bonds.Where(n => n != null).Select(n => (IBond)n.Clone(map)), true);
            clone.lonePairs = CreateObservableChemObjectCollection(lonePairs.Where(n => n != null).Select(n => (ILonePair)n.Clone(map)), true);
            clone.singleElectrons = CreateObservableChemObjectCollection(singleElectrons.Where(n => n != null).Select(n => (ISingleElectron)n.Clone(map)), true);
            clone.stereoElements = new List<IStereoElement>(stereoElements.Select(n => (IStereoElement)n.Clone(map)));

            return clone;
        }

        public virtual IEnumerable<IElectronContainer> GetElectronContainers()
        {
            return bonds.Cast<IElectronContainer>().Concat(LonePairs).Concat(SingleElectrons);
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                    }

        public void SetAtoms(IEnumerable<IAtom> atoms)
        {
            this.atoms.Clear();
            foreach (var atom in atoms)
                this.atoms.Add(atom);
        }

        public void SetBonds(IEnumerable<IBond> bonds)
        {
            this.bonds.Clear();
            foreach (var bond in bonds)
                this.bonds.Add(bond);
        }

        public virtual bool IsEmpty => atoms.Count == 0;
    }
}
