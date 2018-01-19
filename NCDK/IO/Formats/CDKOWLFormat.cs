/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Tools;

namespace NCDK.IO.Formats
{
    /// <summary>
    /// Serializes a CDK model into the Web Ontology Language using the N3 format.
    /// </summary>
    // @cdk.module ioformats
    // @cdk.githash
    public class CDKOWLFormat : SimpleChemFormatMatcher, IChemFormatMatcher
    {
        private static IResourceFormat myself = null;

        public CDKOWLFormat() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new CDKOWLFormat();
                return myself;
            }
        }

        /// <inheritdoc/>
        public override string FormatName => "CDK OWL (N3)";
        
        /// <inheritdoc/>
        public override string MIMEType => "text/n3";
        
        /// <inheritdoc/>
        public override string PreferredNameExtension => NameExtensions[0];
        
        /// <inheritdoc/>
        public override string[] NameExtensions => new string[] { "n3" };

        /// <inheritdoc/>
        public override string ReaderClassName { get; } = typeof(RDF.CDKOWLReader).FullName;
        
        /// <inheritdoc/>
        public override string WriterClassName { get; } = typeof(RDF.CDKOWLWriter).FullName;
        
        /// <inheritdoc/>
        public override bool Matches(int lineNumber, string line)
        {
            if (line.StartsWith("PREFIX") && line.Contains("http://cdk.sourceforge.net/model.owl#"))
            {
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public override bool IsXmlBased => false;
        
        /// <inheritdoc/>
        public override DataFeatures SupportedDataFeatures =>
                DataFeatures.HAS_2D_COORDINATES | DataFeatures.HAS_3D_COORDINATES
                    | DataFeatures.HAS_ATOM_PARTIAL_CHARGES | DataFeatures.HAS_ATOM_FORMAL_CHARGES
                    | DataFeatures.HAS_ATOM_MASS_NUMBERS | DataFeatures.HAS_ATOM_ISOTOPE_NUMBERS
                    | DataFeatures.HAS_GRAPH_REPRESENTATION | DataFeatures.HAS_ATOM_ELEMENT_SYMBOL;

        /// <inheritdoc/>
        public override DataFeatures RequiredDataFeatures => DataFeatures.None;
    }
}
