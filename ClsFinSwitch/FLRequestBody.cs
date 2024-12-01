using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M4.ClsFinSwitch
{
    public class FLRequestBody:IFinSwitchTemplate
    {

        public string NewAcctNum { get; set; }
        public string AcctName { get; set; }
        //public string OldAcctNum { get; set; }
        public string StatusComment { get; set; }
        public string DataChange { get; set; }
       
    }
}
