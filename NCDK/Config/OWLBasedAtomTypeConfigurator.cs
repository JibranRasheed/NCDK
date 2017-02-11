using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NCDK.Config.AtomType;

namespace NCDK.Config
{
    public class OWLBasedAtomTypeConfigurator
        : IAtomTypeConfigurator
    {
        public Stream Stream { get; set; }

        public OWLBasedAtomTypeConfigurator() { }

        public IEnumerable<IAtomType> ReadAtomTypes(IChemObjectBuilder builder)
        {
            if (Stream == null)
                throw new Exception("There was a problem getting an input stream");

            return new OWLAtomTypeReader(Stream).ReadAtomTypes(builder);
        }
    }
}
