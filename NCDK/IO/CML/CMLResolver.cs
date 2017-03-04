/* Copyright (C) 1997-2007  Dan Gezelter
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace NCDK.IO.CML
{
    /// <summary>
    /// This class resolves DOCTYPE declaration for Chemical Markup Language (CML)
    /// files and uses a local version for validation. More information about
    /// CML can be found at <a href="http://www.xml-cml.org/">http://www.xml-cml.org/</a>.
    ///
    // @cdk.module io
    // @cdk.githash
    ///
    // @author Egon Willighagen <egonw@sci.kun.nl>
    ///*/
    public class CMLResolver : XmlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            Debug.WriteLine(nameof(CMLResolver) + ": resolving " + absoluteUri);
            var systemId = absoluteUri.AbsolutePath.ToLowerInvariant();
            if ((systemId.IndexOf("cml-1999-05-15.dtd") != -1) || (systemId.IndexOf("cml.dtd") != -1)
                    || (systemId.IndexOf("cml1_0.dtd") != -1))
            {
                Trace.TraceInformation("File has CML 1.0 DTD");
                return GetCMLType("cml1_0.dtd");
            }
            else if ((systemId.IndexOf("cml-2001-04-06.dtd") != -1) || (systemId.IndexOf("cml1_0_1.dtd") != -1)
                  || (systemId.IndexOf("cml_1_0_1.dtd") != -1))
            {
                Trace.TraceInformation("File has CML 1.0.1 DTD");
                return GetCMLType("cml1_0_1.dtd");
            }
            else
            {
                Trace.TraceWarning("Could not resolve systemID: ", systemId);
                return null;
            }
        }

        /// <summary>
        /// Returns an InputSource of the appropriate CML DTD. It accepts
        /// two CML DTD names: cml1_0.dtd and cml1_0_1.dtd. Returns null
        /// for any other name.
        ///
        /// <param name="type">the name of the CML DTD version</param>
        /// <returns>the InputSource to the CML DTD</returns>
        /// </summary>
        private Stream GetCMLType(string type)
        {
            try
            {
                var ins = ResourceLoader.GetAsStream("NCDK.IO.CML.Data." + type);
                return ins;
            }
            catch (Exception e)
            {
                Trace.TraceError("Error while trying to read CML DTD (" + type + "): ", e.Message);
                Debug.WriteLine(e);
                return null;
            }
        }
    }
}
