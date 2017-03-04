/* Copyright (C) 2009-2010 maclean {gilleain.torrance@gmail.com}
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using FaulonSignatures;
using System.Collections.Generic;

namespace NCDK.Signature
{
    /// <summary>
    /// <p>
    /// A molecule signature is a way to produce <see cref="AtomSignature"/>s and to get
    /// the canonical {@cdk.cite FAU04} signature string for a molecule. There are
    /// several possible uses for a molecule signature.
    /// </p>
    ///
    /// <p>
    /// Firstly, a signature with a height greater than the diameter of a molecule
    /// can be used to reconstruct the molecule. In this sense, the signature string
    /// is like a SMILES {@cdk.cite WEI88, WEI89} string. It is more verbose, but it
    /// will work for all molecules.
    /// </p>
    ///
    /// <p>
    /// Secondly, the set of signatures for a molecule partition the atoms into
    /// equivalence classes (or 'orbits' - see the <see cref="Orbit"/> class). This is
    /// similar to partitioning atoms by Morgan number {@cdk.cite MOR65} except that
    /// it works for 3-regular graphs like fullerenes.
    /// </p>
    ///
    /// <p>
    /// Thirdly, signatures can be calculated at different heights to give
    /// descriptions of the connectivity around atoms. 'Height' is the same as the
    /// idea of a 'sphere' in HOSE codes, and signatures are also path descriptors in
    /// this sense.
    /// </p>
    ///
    /// So, for example, to get the canonical signature for a molecule:
    ///
    /// <code>
    /// IAtomContainer diamantane = MoleculeFactory.MakeBenzene();
    /// MoleculeSignature moleculeSignature = new MoleculeSignature(diamantane);
    /// string canonicalSignature = moleculeSignature.ToCanonicalString();
    /// </code>
    ///
    /// to get the orbits of this molecule:
    ///
    /// <code>
    /// List&lt;Orbit&gt; orbits = moleculeSignature.CalculateOrbits();
    /// </code>
    ///
    /// and to get the height-2 signature string of just atom 5:
    ///
    /// <code>
    /// string hSignatureForAtom5 = moleculeSignature.SignatureStringForVertex(5, 2);
    /// </code>
    ///
    /// it is also possible to get AtomSignatures using the signatureForVertex method
    /// - which is just a convenience method equivalent to calling the constructor of
    /// an AtomSignature class.
    ///
    // @cdk.module signature
    // @author maclean
    // @cdk.githash
    /// </summary>
    public class MoleculeSignature : AbstractGraphSignature
    {
        /// <summary>
        /// The molecule to use when making atom signatures
        /// </summary>
        private IAtomContainer molecule;

        /// <summary>
        /// Creates a signature that represents this molecule.
        ///
        /// <param name="molecule">the molecule to convert to a signature</param>
        /// </summary>
        public MoleculeSignature(IAtomContainer molecule)
            : base()
        {
            this.molecule = molecule;
        }

        /// <summary>
        /// Creates a signature with a maximum height of <code>height</code>
        /// for molecule <code>molecule</code>.
        ///
        /// <param name="molecule">the molecule to convert to a signature</param>
        /// <param name="height">the maximum height of the signature</param>
        /// </summary>
        public MoleculeSignature(IAtomContainer molecule, int height)
            : base(height)
        {
            this.molecule = molecule;
        }

        /// <inheritdoc/>
        public override int GetVertexCount()
        {
            return this.molecule.Atoms.Count;
        }

        /// <inheritdoc/>
        public override string SignatureStringForVertex(int vertexIndex)
        {
            AtomSignature atomSignature;
            int height = base.Height;
            if (height == -1)
            {
                atomSignature = new AtomSignature(vertexIndex, this.molecule);
            }
            else
            {
                atomSignature = new AtomSignature(vertexIndex, height, this.molecule);
            }
            return atomSignature.ToCanonicalString();
        }

        /// <inheritdoc/>
        public override string SignatureStringForVertex(int vertexIndex, int height)
        {
            AtomSignature atomSignature = new AtomSignature(vertexIndex, height, this.molecule);
            return atomSignature.ToCanonicalString();
        }

        /// <inheritdoc/>
        public override AbstractVertexSignature SignatureForVertex(int vertexIndex)
        {
            return new AtomSignature(vertexIndex, this.molecule);
        }

        /// <summary>
        /// Calculates the orbits of the atoms of the molecule.
        ///
        /// <returns>a list of orbits</returns>
        /// </summary>
        public List<Orbit> CalculateOrbits()
        {
            List<Orbit> orbits = new List<Orbit>();
            List<SymmetryClass> symmetryClasses = base.GetSymmetryClasses();
            foreach (var symmetryClass in symmetryClasses)
            {
                Orbit orbit = new Orbit(symmetryClass.GetSignatureString(), -1);
                foreach (var atomIndex in symmetryClass)
                {
                    orbit.AddAtomAt(atomIndex);
                }
                orbits.Add(orbit);
            }
            return orbits;
        }

        /// <summary>
        /// Builder for molecules (rather, for atom containers) from signature
        /// strings.
        ///
        /// <param name="signatureString">the signature string to use</param>
        /// <param name="coBuilder"><see cref="IChemObjectBuilder"/> to build the returned atom container from</param>
        /// <returns>an atom container</returns>
        /// </summary>
        public static IAtomContainer FromSignatureString(string signatureString, IChemObjectBuilder coBuilder)
        {
            ColoredTree tree = AtomSignature.Parse(signatureString);
            MoleculeFromSignatureBuilder builder = new MoleculeFromSignatureBuilder(coBuilder);
            builder.MakeFromColoredTree(tree);
            return builder.GetAtomContainer();
        }

        /// <summary>
        /// Make a canonical signature string of a given height.
        ///
        /// <param name="height">the maximum height to make signatures</param>
        /// <returns>the canonical signature string</returns>
        /// </summary>
        public string ToCanonicalSignatureString(int height)
        {
            string canonicalSignature = null;
            for (int i = 0; i < GetVertexCount(); i++)
            {
                string signatureForI = SignatureStringForVertex(i, height);
                if (canonicalSignature == null || canonicalSignature.CompareTo(signatureForI) < 0)
                {
                    canonicalSignature = signatureForI;
                }
            }
            return canonicalSignature;
        }
    }
}
