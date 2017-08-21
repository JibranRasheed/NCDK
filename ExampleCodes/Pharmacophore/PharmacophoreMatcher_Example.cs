﻿using NCDK.IO.Iterator;
using NCDK.Isomorphisms.Matchers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Pharmacophore
{
    class PharmacophoreMatcher_Example
    {
        void Main()
        {
            var matcher = new PharmacophoreMatcher();

            #region
            QueryAtomContainer query = new QueryAtomContainer(Default.ChemObjectBuilder.Instance);

            PharmacophoreQueryAtom o = new PharmacophoreQueryAtom("D", "[OX1]");
            PharmacophoreQueryAtom n1 = new PharmacophoreQueryAtom("A", "[N]");
            PharmacophoreQueryAtom n2 = new PharmacophoreQueryAtom("A", "[N]");

            query.Atoms.Add(o);
            query.Atoms.Add(n1);
            query.Atoms.Add(n2);

            PharmacophoreQueryBond b1 = new PharmacophoreQueryBond(o, n1, 4.0, 4.5);
            PharmacophoreQueryBond b2 = new PharmacophoreQueryBond(o, n2, 4.0, 5.0);
            PharmacophoreQueryBond b3 = new PharmacophoreQueryBond(n1, n2, 5.4, 5.8);

            query.Bonds.Add(b1);
            query.Bonds.Add(b2);
            query.Bonds.Add(b3);

            string filename = "/Users/rguha/pcore1.sdf";
            using (var srm = new FileStream(filename, FileMode.Open))
            {
                foreach (var conformers in new IEnumerableMDLConformerReader(srm, Default.ChemObjectBuilder.Instance))
                {
                    bool firstTime = true;
                    foreach (var conf in conformers)
                    {
                        bool status;
                        if (firstTime)
                        {
                            status = matcher.Matches(conf, true);
                            firstTime = false;
                        }
                        else status = matcher.Matches(conf, false);
                        if (status)
                        {
                            // OK, matched. Do something
                        }
                    }
                    #endregion
                }
            }
        }
    }
}

