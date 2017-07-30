/*
 * Copyright (C) 2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */




/* Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
using System;

namespace NCDK.SMSD
{
   /// <summary>
    /// This class represents various algorithm type supported by SMSD.
    /// Presently SMSD supports 5 different kinds of algorithms:
    /// <ol>
    /// <item>0: default</item>
    /// <item>1: MCSPlus</item>
    /// <item>2: VFLibMCS</item>
    /// <item>3: CDKMCS</item>
    /// <item>4: SubStructure</item>
    /// <item>5: TurboSubStructure</item>
    /// </ol>
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public partial struct Algorithm : System.IComparable<Algorithm>, System.IComparable
    {
		/// <summary>
		/// The <see cref="Ordinal"/> values of <see cref="Algorithm"/>.
		/// </summary>
		/// <seealso cref="Algorithm"/>
        public static class O
        {
            public const int Default = 0;
            public const int MCSPlus = 1;
            public const int VFLibMCS = 2;
            public const int CDKMCS = 3;
            public const int SubStructure = 4;
            public const int TurboSubStructure = 5;
          
        }

        private readonly int ordinal;
		/// <summary>
		/// The ordinal of this enumeration constant. The list is in <see cref="O"/>.
		/// </summary>
		/// <seealso cref="O"/>
        public int Ordinal => ordinal;

		/// <inheritdoc/>
        public override string ToString()
        {
            return names[Ordinal];
        }

        private static readonly string[] names = new string[] 
        {
            "Default", 
            "MCSPlus", 
            "VFLibMCS", 
            "CDKMCS", 
            "SubStructure", 
            "TurboSubStructure", 
         
        };

        private Algorithm(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator Algorithm(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(Algorithm obj)
        {
            return obj.Ordinal;
        }

        /// <summary>
        /// Default SMSD algorithm.
        /// </summary>
        public static readonly Algorithm Default = new Algorithm(0);
        /// <summary>
        /// MCS Plus algorithm.
        /// </summary>
        public static readonly Algorithm MCSPlus = new Algorithm(1);
        /// <summary>
        /// VF Lib based MCS algorithm.
        /// </summary>
        public static readonly Algorithm VFLibMCS = new Algorithm(2);
        /// <summary>
        /// CDK UIT MCS.
        /// </summary>
        public static readonly Algorithm CDKMCS = new Algorithm(3);
        /// <summary>
        /// Substructure search will return all maps.
        /// </summary>
        public static readonly Algorithm SubStructure = new Algorithm(4);
        /// <summary>
        /// Substructure search will return first map.
        /// </summary>
        public static readonly Algorithm TurboSubStructure = new Algorithm(5);
        private static readonly Algorithm[] values = new Algorithm[]
        {
            Default, 
            MCSPlus, 
            VFLibMCS, 
            CDKMCS, 
            SubStructure, 
            TurboSubStructure, 
    
        };
        public static System.Collections.Generic.IEnumerable<Algorithm> Values => values;

        /* Avoid to cause compiling error */

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public static bool operator==(Algorithm a, object b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator!=(Algorithm a, object b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator==(object a, Algorithm b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator!=(object a, Algorithm b)
        {
            throw new System.Exception();
        }


        public static bool operator==(Algorithm a, Algorithm b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(Algorithm a, Algorithm b)
        {
            return !(a == b);
        }

		/// <inheritdoc/>
        public override bool Equals(object obj)
        {
    
            if (!(obj is Algorithm))
                return false;
            return this.Ordinal == ((Algorithm)obj).Ordinal;
        }

		/// <inheritdoc/>
        public override int GetHashCode()
        {
            return Ordinal;
        }

		/// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var o = (Algorithm)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   

		/// <inheritdoc/>
        public int CompareTo(Algorithm o)
        {
            return (Ordinal).CompareTo(o.Ordinal);
        }   	
	}
	public partial struct Algorithm 
    {
        private static string[] descriptions = new[] 
        {
            "Default SMSD algorithm",
            "MCS Plus algorithm",
            "VF Lib based MCS algorithm",
            "CDK UIT MCS",
            "Substructure search",
            "Turbo Mode- Substructure search",
        };

        /// <summary>
        /// type of algorithm.
        /// </summary>
        public int Type => Ordinal;

        /// <summary>
        /// short description of the algorithm.
        /// </summary>
        public string Description => descriptions[Ordinal];
    }
}
