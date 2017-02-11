/*  Copyright (C) 2003-2005  Christoph Steinbeck
 *                2003-2008  Egon Willighagen
 *                           Stefan Kuhn
 *                           Rajarshi Guha
 *                2015       John May
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
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
using NCDK.Isomorphisms;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using NCDK.IO;
using NCDK.Tools.Manipulator;
using NCDK.Geometries;

namespace NCDK.Layout
{
    /**
     * Helper class for Structure Diagram Generation. Handles templates. This is
     * our layout solution for ring systems which are notoriously difficult to
     * layout, like cubane, adamantane, porphyrin, etc.
     *
     * @author steinbeck
     * @cdk.created 2003-09-04
     * @cdk.keyword layout
     * @cdk.keyword 2D-coordinates
     * @cdk.keyword structure diagram generation
     * @cdk.require java1.4+
     * @cdk.module sdg
     * @cdk.githash
     */
    public sealed class TemplateHandler
    {
        private readonly List<IAtomContainer> templates = new List<IAtomContainer>();
        private readonly List<Pattern> anonPatterns = new List<Pattern>();
        private readonly List<Pattern> elemPatterns = new List<Pattern>();


        private readonly AtomMatcher elemAtomMatcher = new ElemAtomMatcher();
        class ElemAtomMatcher : AtomMatcher
        {
            public override bool Matches(IAtom a, IAtom b)
            {
                return a.AtomicNumber.Equals(b.AtomicNumber);
            }
        }

        private readonly AtomMatcher anonAtomMatcher = new AnonAtomMatcher();
        class AnonAtomMatcher : AtomMatcher
        {
            public override bool Matches(IAtom a, IAtom b)
            {
                return true;
            }
        }

        private readonly BondMatcher anonBondMatcher = new AnonBondMatcher();
        class AnonBondMatcher : BondMatcher
        {
            public override bool Matches(IBond a, IBond b)
            {
                return true;
            }
        }

        /**
         * Creates a new TemplateHandler with default templates loaded.
         */
        public TemplateHandler(IChemObjectBuilder builder)
        {
            LoadTemplates(builder);
        }

        /**
         * Creates a new TemplateHandler without any default templates.
         */
        public TemplateHandler()
        {
        }

        /**
         * Loads all existing templates into memory. To add templates to be used in
         * SDG, place a drawing with the new template in org/openscience/cdk/layout/templates and add the
         * template filename to org/openscience/cdk/layout/templates/template.list
         */
        public void LoadTemplates(IChemObjectBuilder builder)
        {
            try
            {
                using (Stream ins = this.GetType().Assembly.GetManifestResourceStream("NCDK.Layout.Templates.templates.list"))
                using (var reader = new StreamReader(ins))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = "NCDK.Layout.Templates." + line;
                        Debug.WriteLine($"Attempting to read template {line}");
                        try
                        {
                            CMLReader structureReader = new CMLReader(this.GetType().Assembly.GetManifestResourceStream(line));
                            IChemFile file = structureReader.Read(builder.CreateChemFile());
                            var files = ChemFileManipulator.GetAllAtomContainers(file);
                            foreach (var f in files)
                                AddMolecule(f);
                            Debug.WriteLine($"Successfully read template {line}");
                        }
                        catch (CDKException e)
                        {
                            Trace.TraceWarning($"Could not read template {line}, reason: {e.Message}");
                            Debug.WriteLine(e.Message);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Trace.TraceWarning($"Could not read (all of the) templates, reason: {e.Message}");
                Debug.WriteLine(e.Message);
            }
        }

        /**
         * Adds a Molecule to the list of templates use by this TemplateHandler.
         *
         * @param molecule The molecule to be added to the TemplateHandler
         */
        public void AddMolecule(IAtomContainer molecule)
        {
            if (!GeometryUtil.Has2DCoordinates(molecule))
                throw new ArgumentException("Template did not have 2D coordinates");

            // we want a consistent scale!
            GeometryUtil.ScaleMolecule(molecule, GeometryUtil.GetScaleFactor(molecule,
                                                                             StructureDiagramGenerator.DEFAULT_BOND_LENGTH));

            templates.Add(molecule);
            anonPatterns.Add(VentoFoggia.FindSubstructure(molecule,
                                                          anonAtomMatcher,
                                                          anonBondMatcher));
            elemPatterns.Add(VentoFoggia.FindSubstructure(molecule,
                                                          elemAtomMatcher,
                                                          anonBondMatcher));
        }

        public IAtomContainer RemoveMolecule(IAtomContainer molecule)
        {
            for (int i = 0; i < templates.Count; i++)
            {
                if (VentoFoggia.FindIdentical(templates[i], anonAtomMatcher, anonBondMatcher)
                               .Matches(molecule))
                {
                    elemPatterns.RemoveAt(i);
                    anonPatterns.RemoveAt(i);
                    var ret = templates[i];
                    templates.RemoveAt(i);
                    return ret;
                }
            }
            return null;
        }

        /**
         * Checks if one of the loaded templates is isomorph to the given
         * Molecule. If so, it assigns the coordinates from the template to the
         * respective atoms in the Molecule, and marks the atoms as ISPLACED.
         *
         * @param molecule The molecule to be check for potential templates
         * @return True if there was a possible mapping
         */
        public bool MapTemplateExact(IAtomContainer molecule)
        {
            foreach (var template in templates)
            {
                Mappings mappings = VentoFoggia.FindIdentical(template, anonAtomMatcher, anonBondMatcher)
                                               .MatchAll(molecule);
                foreach (var atoms in mappings.ToAtomMap())
                {
                    foreach (var e in atoms)
                    {
                        e.Value.Point2D = e.Key.Point2D;
                        e.Value.IsPlaced = true;
                    }
                    if (atoms.Count != 0)
                        return true;
                }
            }
            return false;
        }

        /**
         * Checks if one of the loaded templates is a substructure in the given
         * Molecule. If so, it assigns the coordinates from the template to the
         * respective atoms in the Molecule, and marks the atoms as ISPLACED.
         *
         * @param molecule The molecule to be check for potential templates
         * @return True if there was a possible mapping
         */
        public bool MapTemplates(IAtomContainer molecule)
        {
            // match element patterns first so hetero atoms are oriented correctly
            foreach (var anonPattern in elemPatterns)
            {
                foreach (var atoms in anonPattern.MatchAll(molecule).ToAtomMap())
                {
                    foreach (var e in atoms)
                    {
                        e.Value.Point2D = e.Key.Point2D;
                        e.Value.IsPlaced = true;
                    }
                    if (atoms.Count != 0)
                        return true;
                }
            }
            // no element pattern matched, try anonymous patterns
            foreach (var anonPattern in anonPatterns)
            {
                foreach (var atoms in anonPattern.MatchAll(molecule).ToAtomMap())
                {
                    foreach (var e in atoms)
                    {
                        e.Value.Point2D = e.Key.Point2D;
                        e.Value.IsPlaced = true;
                    }
                    if (atoms.Count != 0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// The templateCount attribute of the <see cref="TemplateHandler"/> object.
        /// </summary>
        public int TemplateCount => templates.Count;

        /**
         * Gets the templateAt attribute of the TemplateHandler object
         *
         * @param position Description of the Parameter
         * @return The templateAt value
         */
        public IAtomContainer GetTemplateAt(int position)
        {
            return templates[position];
        }

        /**
         * Checks if one of the loaded templates is a substructure in the given
         * Molecule and returns all matched substructures in a IAtomContainerSet.
         * This method does not assign any coordinates.
         *
         * @param molecule The molecule to be check for potential templates
         * @return an IAtomContainerSet of all matched substructures of
         * the molecule
         * @throws CDKException if an error occurs
         */
        public IAtomContainerSet<IAtomContainer> GetMappedSubstructures(IAtomContainer molecule)
        {

            var matchedSubstructures = molecule.Builder.CreateAtomContainerSet();
            var matchedChemObjs = new HashSet<IChemObject>();

            foreach (var anonPattern in anonPatterns)
            {
                foreach (var map in anonPattern.MatchAll(molecule).GetUniqueAtoms().ToAtomBondMap())
                {
                    bool overlaps = false;
                    IAtomContainer matched = molecule.Builder
                                                     .CreateAtomContainer();
                    foreach (var e in map)
                    {
                        if (matchedChemObjs.Contains(e.Value))
                            overlaps = true;
                        if (e.Value is IAtom)
                            matched.Atoms.Add((IAtom)e.Value);
                        else if (e.Value is IBond)
                            matched.Bonds.Add((IBond)e.Value);
                    }

                    // only add if the atoms/bonds of this match don't overlap existing
                    if (!overlaps)
                    {
                        foreach (var atom in matched.Atoms)
                            matchedChemObjs.Add(atom);
                        foreach (var bond in matched.Bonds)
                            matchedChemObjs.Add(bond);
                        matchedSubstructures.Add(matched);
                    }
                }
            }
            return matchedSubstructures;
        }

        /**
         * Singleton template instance, mainly useful for aligning molecules. If the template
         * does not have coordinates an error is thrown.
         *
         * For safety we clone the molecule.
         *
         * @param template the molecule
         * @return new template handler
         */
        public static TemplateHandler CreateSingleton(IAtomContainer template)
        {
            TemplateHandler handler = new TemplateHandler();
            IAtomContainer copy = (IAtomContainer)template.Clone();
            handler.AddMolecule(copy);
            return handler;
        }

        /**
         * Create a template from a substructure pattern. Using this template handler in the diagram
         * generator then allows us to align to common reference.
         *
         * @param template the molecule
         * @return new template handler
         */
        public static TemplateHandler CreateFromSubstructure(Pattern ptrn, IEnumerable<IAtomContainer> mols)
        {
            foreach (var mol in mols)
            {
                foreach (var template in ptrn.MatchAll(mol).ToSubstructures())
                    return CreateSingleton(template);
            }
            throw new ArgumentException("Pattern does not match any provided molecules");
        }

        /**
         * Create a template from a substructure pattern. Using this template handler in the diagram
         * generator then allows us to align to common reference.
         *
         * @param template the molecule
         * @return new template handler
         */
        public static TemplateHandler CreateFromSubstructure(Pattern ptrn, IAtomContainer mol)
        {
            foreach (var template in ptrn.MatchAll(mol).ToSubstructures())
                return CreateSingleton(template);
            throw new ArgumentException("Pattern does not match any provided molecules");
        }

        /**
         * Convert to an identity template library.
         *
         * @return identity template library
         */
#if TEST
        public
#else
        internal
#endif
            IdentityTemplateLibrary ToIdentityTemplateLibrary()
        {
            IdentityTemplateLibrary lib = IdentityTemplateLibrary.Empty();
            foreach (var mol in templates)
            {
                lib.Add(mol);
                lib.Add(AtomContainerManipulator.Anonymise(mol));
            }
            return lib;
        }
    }
}
