﻿// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara




namespace NCDK
{
    /// <summary>
    /// Hybridization states.
    /// </summary>
    public struct Hybridization : System.IComparable
    {
        public static class O
        {
            public const int Unset = 0;
            public const int S = 1;
            public const int SP1 = 2;
            public const int SP2 = 3;
            public const int SP3 = 4;
            public const int Planar3 = 5;
            public const int SP3D1 = 6;
            public const int SP3D2 = 7;
            public const int SP3D3 = 8;
            public const int SP3D4 = 9;
            public const int SP3D5 = 10;
          
        }

        private readonly int ordinal;
        public int Ordinal => ordinal;

        public override string ToString()
        {
            return names[Ordinal];
        }

        private static readonly string[] names = new string[] 
        {
            "Unset", 
            "S", 
            "SP1", 
            "SP2", 
            "SP3", 
            "Planar3", 
            "SP3D1", 
            "SP3D2", 
            "SP3D3", 
            "SP3D4", 
            "SP3D5", 
         
        };

        private Hybridization(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator Hybridization(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(Hybridization obj)
        {
            return obj.Ordinal;
        }

        /// <summary>
        /// A undefined hybridization.
        /// </summary>
        public static readonly Hybridization Unset = new Hybridization(0);
        public static readonly Hybridization S = new Hybridization(1);
        /// <summary>
        /// A geometry of neighboring atoms when an s orbital is hybridized with one p orbital.
        /// </summary>
        public static readonly Hybridization SP1 = new Hybridization(2);
        /// <summary>
        /// A geometry of neighboring atoms when an s orbital is hybridized with two p orbitals.
        /// </summary>
        public static readonly Hybridization SP2 = new Hybridization(3);
        /// <summary>
        /// A geometry of neighboring atoms when an s orbital is hybridized with three p orbitals.
        /// </summary>
        public static readonly Hybridization SP3 = new Hybridization(4);
        /// <summary>
        /// trigonal planar (lone pair in pz)
        /// </summary>
        public static readonly Hybridization Planar3 = new Hybridization(5);
        /// <summary>
        /// A geometry of neighboring atoms when an s orbital is hybridized with three p orbitals with one d orbital.
        /// </summary>
        public static readonly Hybridization SP3D1 = new Hybridization(6);
        /// <summary>
        /// A geometry of neighboring atoms when an s orbital is hybridized with three p orbitals with two d orbitals.
        /// </summary>
        public static readonly Hybridization SP3D2 = new Hybridization(7);
        /// <summary>
        /// A geometry of neighboring atoms when an s orbital is hybridized with three p orbitals with three d orbitals.
        /// </summary>
        public static readonly Hybridization SP3D3 = new Hybridization(8);
        /// <summary>
        /// A geometry of neighboring atoms when an s orbital is hybridized with three p orbitals with four d orbitals.
        /// </summary>
        public static readonly Hybridization SP3D4 = new Hybridization(9);
        /// <summary>
        /// A geometry of neighboring atoms when an s orbital is hybridized with three p orbitals with five d orbitals.
        /// </summary>
        public static readonly Hybridization SP3D5 = new Hybridization(10);
        private static readonly Hybridization[] values = new Hybridization[]
        {
            Unset, 
            S, 
            SP1, 
            SP2, 
            SP3, 
            Planar3, 
            SP3D1, 
            SP3D2, 
            SP3D3, 
            SP3D4, 
            SP3D5, 
    
        };
        public static System.Collections.Generic.IEnumerable<Hybridization> Values => values;

        /* In order to cause compiling error */

        public static bool operator==(Hybridization a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(Hybridization a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator==(object a, Hybridization b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(object a, Hybridization b)
        {
            throw new System.Exception();
        }

        public static bool operator==(Hybridization a, Hybridization b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(Hybridization a, Hybridization b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
    
            if (!(obj is Hybridization))
                return false;
            return this.Ordinal == ((Hybridization)obj).Ordinal;
        }

        public override int GetHashCode()
        {
            return Ordinal;
        }

        public int CompareTo(object obj)
        {
            var o = (Hybridization)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   
        public string Name => ToString();

        public bool IsUnset => this.Ordinal == 0;

        internal static Hybridization GetInstance(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Unset;

            switch (value.ToLowerInvariant())
            {
                case "s":
                    return S;
                case "sp":
                    return SP1;
                case "sp1":
                    return SP1;
                case "sp2":
                    return SP2;
                case "sp3":
                case "tetrahedral":
                    return SP3;
                case "planar":
                    return Planar3;
                case "sp3d1":
                    return SP3D1;
                case "sp3d2":
                case "octahedral":
                    return SP3D2;
                case "sp3d3":
                    return SP3D3;
                case "sp3d4":
                    return SP3D4;
                case "sp3d5":
                    return SP3D5;
                default:
                    throw new System.ArgumentException("Unrecognized hybridization", nameof(value));
            }
        }
    }
}
