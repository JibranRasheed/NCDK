﻿/*
 * Copyright (c) 2016 John May <jwmay@users.sf.net>
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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using NCDK.Common.Primitives;
using NCDK.NInChI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCDK.Graphs.InChI
{
    public class JniInChIInputAdapter : NInchiInput
    {
        /// <summary>
        /// Flag indicating windows or linux.
        /// </summary>
        private static readonly bool IS_WINDOWS = Environment.OSVersion.Platform < PlatformID.Unix;

        /// <summary>
        /// Switch character for passing options. / in windows, - on other systems.
        /// </summary>
        private static readonly string FLAG_CHAR = IS_WINDOWS ? "/" : "-";

        public static readonly string FIVE_SECOND_TIMEOUT = FLAG_CHAR + "W5";

        public JniInChIInputAdapter(string options)
        {
            this.Options = options == null ? "" : CheckOptions(options);
        }

        public JniInChIInputAdapter(IList<INCHI_OPTION> options)
        {
            this.Options = options == null ? "" : CheckOptions(options);
        }

        private static bool IsTimeoutOptions(string op)
        {
            if (op == null || op.Length < 2) return false;
            int pos = 0;
            int len = op.Length;
            if (op[pos] == 'W')
                pos++;
            while (pos < len && char.IsDigit(op[pos]))
                pos++;
            if (pos < len && op[pos] == '.')
                pos++;
            while (pos < len && char.IsDigit(op[pos]))
                pos++;
            return pos == len;
        }

        private static string CheckOptions(string ops)
        {
            if (ops == null)
            {
                throw new ArgumentNullException(nameof(ops));
            }
            StringBuilder sbOptions = new StringBuilder();

            bool hasUserSpecifiedTimeout = false;

            var tok = Strings.Tokenize(ops);
            string options = string.Join(" ", tok.Select(n =>
                {
                    string op = n;
                    if (op.StartsWith("-") || op.StartsWith("/"))
                    {
                        op = op.Substring(1);
                    }

                    INCHI_OPTION option = INCHI_OPTION.ValueOfIgnoreCase(op);
                    if (option != null)
                    {
                        return FLAG_CHAR + option.Name;
                    }
                    else if (IsTimeoutOptions(op))
                    {
                        hasUserSpecifiedTimeout = true;
                        return FLAG_CHAR + op;
                    }
                    // 1,5 tautomer option
                    else if ("15T".Equals(op))
                    {
                        return FLAG_CHAR + "15T";
                    }
                    // keto-enol tautomer option
                    else if ("KET".Equals(op))
                    {
                        return FLAG_CHAR + "KET";
                    }
                    else
                    {
                        throw new NInchiException("Unrecognised InChI option");
                    }
                }));

            if (!hasUserSpecifiedTimeout)
            {
                if (options.Length > 0)
                    options += " ";
                options += FIVE_SECOND_TIMEOUT;
            }
            return options;
        }

        private static string CheckOptions(IList<INCHI_OPTION> ops)
        {
            if (ops == null)
            {
                throw new ArgumentNullException(nameof(ops), "Null options");
            }
            string options = string.Join(" ", ops.Select(op => FLAG_CHAR + op.Name));
            if (options.Length > 0)
                options += " ";
            options += FIVE_SECOND_TIMEOUT;

            return options;
        }
    }
}
