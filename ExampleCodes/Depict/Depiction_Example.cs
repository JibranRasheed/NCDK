﻿namespace NCDK.Depict
{
    class Depiction_Example
    {
        public void Main()
        {
            Depiction depiction = null;
            #region EnsureSuffix
            depiction.WriteTo(Depiction.SvgFormatKey, "~/chemical"); // create a file "~/chemical.svg" 
            #endregion
        }
    }
}
