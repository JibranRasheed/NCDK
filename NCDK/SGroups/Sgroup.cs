/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using System.Collections.Generic;

namespace NCDK.Sgroups
{
    /// <summary>
    /// Generic CTab Sgroup (substructure group) that stores all other types of group. This representation
    /// is allows reading from CTfiles (e.g. Molfile, SDfile).
    /// </summary>
    /// <remarks>
    /// The class uses a key-value store for Sgroup attributes simplifying both input and output.
    /// </remarks>
    public class Sgroup
    {
        /// <summary>
        /// the atoms of this substructure group.
        /// </summary>
        public ISet<IAtom> Atoms { get; private set; }

        /// <summary>
        /// Access the bonds that belong to this substructure group.
        /// For data Sgroups, the bonds are the containment bonds,
        /// for all other <see cref="Sgroup"/> types, they are crossing bonds.
        /// </summary>
        public ISet<IBond> Bonds { get; private set; }

        /// <summary>
        /// the parents of this Sgroup.
        /// </summary>
        public ISet<Sgroup> Parents { get; private set; }

        private readonly IDictionary<SgroupKeys, object> attributes = new SortedDictionary<SgroupKeys, object>();

        /// <summary>
        /// Create a new generic Sgroup.
        /// </summary>
        public Sgroup()
        {
            Atoms = new HashSet<IAtom>();
            Bonds = new HashSet<IBond>();
            Parents = new HashSet<Sgroup>();
            Type = SgroupTypes.CtabGeneric;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="org">original Sgroup instance</param>
        Sgroup(Sgroup org)
        {
            Atoms = new HashSet<IAtom>(org.Atoms);
            Bonds = new HashSet<IBond>(org.Bonds);
            Parents = new HashSet<Sgroup>(org.Parents);
            this.attributes = new Dictionary<SgroupKeys, object>(org.attributes);
        }

        /// <summary>
        /// Access all the attribute keys of this Sgroup.
        /// </summary>
        /// <returns>attribute keys</returns>
        public ICollection<SgroupKeys> AttributeKeys => attributes.Keys;

        /// <summary>
        /// The type of the Sgroup.
        /// </summary>
        public SgroupTypes Type
        {
            set
            {
                PutValue(SgroupKeys.CtabType, value);
            }

            get
            {
                return (SgroupTypes)GetValue(SgroupKeys.CtabType);
            }
        }

        /// <summary>
        /// Add a bond to this Sgroup.
        /// </summary>
        /// <param name="atom">the atom</param>
        public void Add(IAtom atom)
        {
            Atoms.Add(atom);
        }

        /// <summary>
        /// Add a bond to this Sgroup. The bond list
        /// </summary>
        /// <param name="bond">bond to add</param>
        public void Add(IBond bond)
        {
            Bonds.Add(bond);
        }

        /// <summary>
        /// Add a parent Sgroup.
        /// </summary>
        /// <param name="parent">parent sgroup</param>
        public void AddParent(Sgroup parent)
        {
            Parents.Add(parent);
        }

        /// <summary>
        /// Remove the specified parent associations from this Sgroup.
        /// </summary>
        /// <param name="parents">parent associations</param>
        public void RemoveParents(IEnumerable<Sgroup> parents)
        {
            foreach (var p in parents)
                Parents.Remove(p);
        }

        /// <summary>
        /// Store an attribute for the Sgroup.
        /// </summary>
        /// <param name="key">attribute key</param>
        /// <param name="val">attribute value</param>
        public void PutValue(SgroupKeys key, object val)
        {
            attributes[key] = val;
        }

        /// <summary>
        /// Access an attribute for the Sgroup.
        /// </summary>
        /// <param name="key">attribute key</param>
        public object GetValue(SgroupKeys key)
        {
            if (!attributes.TryGetValue(key, out object o))
                return null;
            return o;
        }

        /// <summary>
        /// Access the subscript value.
        /// </summary>
        /// <returns>subscript value (or null if not present)</returns>
        public string Subscript
        {
            get { return (string)GetValue(SgroupKeys.CtabSubScript); }
            set { PutValue(SgroupKeys.CtabSubScript, value); }
        }

        /// <summary>
        /// Add a bracket for this Sgroup.
        /// </summary>
        /// <param name="bracket">sgroup bracket</param>
        public void AddBracket(SgroupBracket bracket)
        {
            IList<SgroupBracket> brackets = (IList<SgroupBracket>)GetValue(SgroupKeys.CtabBracket);
            if (brackets == null)
            {
                brackets = new List<SgroupBracket>(2);
                PutValue(SgroupKeys.CtabBracket, brackets);
            }
            brackets.Add(bracket);
        }

        /// <summary>
        /// Downcast this, maybe generic, Sgroup to a specific concrete implementation. This
        /// method should be called on load by a reader once all data has been added to the sgroup.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <returns>downcast instance</returns>
        public T Downcast<T>() where T : Sgroup
        {
            // ToDo - Implement
            return (T)this;
        }
    }
}
