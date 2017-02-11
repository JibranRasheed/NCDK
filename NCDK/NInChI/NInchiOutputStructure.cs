/*
 * Copyright 2006-2011 Sam Adams <sea36 at users.sourceforge.net>
 *
 * This file is part of JNI-InChI.
 *
 * JNI-InChI is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * JNI-InChI is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with JNI-InChI.  If not, see <http://www.gnu.org/licenses/>.
 */
namespace NCDK.NInChI
{
    /**
     * Encapsulates output from InChI to structure conversion.
     * @author Sam Adams
     */
    public class NInchiOutputStructure : NInchiStructure
    {
        /**
         * Return status from conversion.
         */
        public INCHI_RET ReturnStatus { get; protected set; }

        /**
         * Error/warning messages generated.
         */
        public string Message {
            get;
#if !TEST
            protected 
#endif
            set; }

        /**
         * Log generated.
         */
        public string Log {
            get;
#if !TEST
            protected 
#endif
            set;
        }

        /**
         * <p>Warning flags, see INCHIDIFF in inchicmp.h.
         *
         * <p>[x][y]:
         * <br>x=0 => Reconnected if present in InChI otherwise Disconnected/Normal
         * <br>x=1 => Disconnected layer if Reconnected layer is present
         * <br>y=1 => Main layer or Mobile-H
         * <br>y=0 => Fixed-H layer
         */
        public ulong[,] WarningFlags {
            get;
#if !TEST
            protected 
#endif
            set;
        } = new ulong[2, 2];


        public NInchiOutputStructure(int ret, string message, string log, ulong w00, ulong w01, ulong w10, ulong w11)
            : this((INCHI_RET)ret)
        {
            Message = message;
            Log = log;
            WarningFlags = new[,] { { w00, w01, }, { w10, w11 } };
        }

        public NInchiOutputStructure(INCHI_RET value)
        {
            this.ReturnStatus = value;
        }

        protected void SetWarningFlags(ulong f00, ulong f01, ulong f10, ulong f11)
        {
            this.WarningFlags = new [,] { { f00, f01 }, { f10, f11 } };
        }
    }
}
