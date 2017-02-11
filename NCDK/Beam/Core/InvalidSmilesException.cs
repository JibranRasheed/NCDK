/*
 * Copyright (c) 2013, European Bioinformatics Institute (EMBL-EBI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * Any EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * Any DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON Any THEORY OF LIABILITY, WHETHER IN CONTRACT, Strict LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN Any WAY OUT OF THE USE OF THIS
 * SOFTWARE, Even IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies,
 * either expressed or implied, of the FreeBSD Project. 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NCDK.Beam
{
    /// <summary>
    /// An exception thrown when parsing malformed SMILES.
    ///
    /// <author>John May</author>
    /// </summary>
#if TEST
    public
#endif
    sealed class InvalidSmilesException : IOException
    {
        public InvalidSmilesException(string message, CharBuffer buffer)
            : this(message, buffer, 0)
        {
        }

        public InvalidSmilesException(string message, CharBuffer buffer, int offset)
            : base(message + Display(buffer, offset))
        { }

        public InvalidSmilesException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Displays the character buffer and marks on the next line the current
        /// position in the buffer.
        ///
        /// <blockquote><pre>
        /// invalid bracket atom:
        /// C[CCCC
        ///    ^
        /// </pre></blockquote>
        ///
        /// <param name="buffer">a character buffer</param>
        /// <returns>a </returns>3 line string showing the buffer and it's current position
        /// </summary>
        static string Display(CharBuffer buffer, int offset)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('\n');
            sb.Append(buffer);
            sb.Append('\n');
            for (int i = 1; i < (buffer.Position + offset); i++)
                sb.Append(' ');
            sb.Append('^');
            return sb.ToString();
        }

        /// <summary>
        /// Utility for invalid bracket atom error.
        ///
        /// <param name="buffer">the current buffer</param>
        /// <returns>the invalid smiles exception with buffer information</returns>
        /// </summary>
        public static InvalidSmilesException InvalidBracketAtom(CharBuffer buffer)
        {
            return new InvalidSmilesException("Invalid bracket atom, [ <isotope>? <symbol> <chiral>? <hcount>? <charge>? <class>? ]:",
                                              buffer);
        }
    }
}
