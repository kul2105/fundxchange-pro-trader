using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    public class Strategy
    {
        public string Name = string.Empty;
        public string Notes = string.Empty;
        
        public int BarInterval = 0;
        public string BarPeriodicity = string.Empty;
        public int BarHistory = 0;
        
        public string script_EnterLong = string.Empty;
        public string script_ExitLong = string.Empty;
        public string script_EnterShort = string.Empty;
        public string script_ExitShort = string.Empty;
    }
}
