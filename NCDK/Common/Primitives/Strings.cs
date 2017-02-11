﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCDK.Common.Primitives
{
    public static class Strings
    {
        public sealed class OrdinalComparer : IComparer<string>
        {
            public static readonly OrdinalComparer Instance = new OrdinalComparer();

            public int Compare(string strA, string strB)
            {
                return string.CompareOrdinal(strA, strB);
            }
        }

        public static StringBuilder Append(StringBuilder sb, string str)
        {
            if (str == null)
                return sb.Append("null");
            else
                return sb.Append(str);
        }

        public static string ToFormatedString(double value, string format, int maxChars)
        {
            var s = value.ToString(format);
            var indexOfPeriod = s.IndexOf('.');
            if (indexOfPeriod == -1)
                return s;
            if (indexOfPeriod < maxChars)
                return s.Substring(0, indexOfPeriod);
            return s.Substring(0, maxChars);
        }

        public static string JavaFormat(double value, string format)
        {
            var s = value.ToString(format);
            if (s.StartsWith("0"))
                s = s.Substring(1);
            else if (s.StartsWith("-0"))
                s = "-" + s.Substring(2);
            while (s.EndsWith("0"))
                s = s.Substring(0, s.Length - 1);
            if (s.EndsWith("."))
                s = s.Substring(0, s.Length - 1);
            if (s == "" || s == "-")
                s = "0";
            return s;
        }

        public static string Substring(string str, int start)
        {
            if (str.Length < start)
                return "";
            return str.Substring(start);
        }

        public static string Substring(string str, int start, int length)
        {
            if (str.Length < start)
                return "";
            return str.Substring(start, Math.Min(str.Length - start, length));
        }

        private static char[] Delimitters { get; } = " \t\n\r\f".ToCharArray();

        public static IList<string> Tokenize(string str)
        {
            return Tokenize(str, Delimitters);
        }

        public static IList<string> Tokenize(string str, params char[] delims)
        {
            var ret = new List<string>();
            var tokens = str.Split(delims);
            if (tokens.Length == 0)
                return ret;
            //if (string.IsNullOrEmpty(tokens[0]))
            //    ret.Add("");
            ret.AddRange(tokens.Where(n => !string.IsNullOrEmpty(n)));
            return ret;
        }

        public static string Reverse(string str)
        {
            var aa = str.ToCharArray();
            Array.Reverse(aa);
            return new string(aa);
        }

        public static int GetJavaHashCode(string value)
        {
            int ret = 0;
            foreach (var c in value)
            {
                ret *= 31;
                ret += (int)c;
            }
            return ret;
        }

        public static string ToJavaString(object o)
        {
            if (o is Array)
                return ToJavaString((Array)o);
            if (o is ICollection)
                return ToJavaString((ICollection)o);
            return o.ToString();
        }

        public static string ToJavaString(ICollection list)
        {
            if (list.Count == 0)
                return "[]";

            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            bool isFirst = true;
            foreach (var e in list)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(", ");
                sb.Append(e == list ? "(this Collection)" : ToJavaString((object)e));
            }
            return sb.Append(']').ToString();
        }

        public static string ToJavaString(Array array)
        {
            var sb = new StringBuilder();
            sb.Append("{ ");
            foreach (var e in array)
            {
                sb.Append(ToJavaString((object)e)).Append(", ");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
