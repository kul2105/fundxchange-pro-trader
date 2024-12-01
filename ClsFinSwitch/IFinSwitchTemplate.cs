using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M4.ClsFinSwitch
{
    public class IFinSwitchTemplate
    {
        public string MsgTypeCode { get; set; }
        public string MsgTypeName { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string MancoCode { get; set; }
        public string MancoName { get; set; }
        public string FundCode { get; set; }
        public string FundName { get; set; }
        public string InvestorCode { get; set; }
        public string InvestorName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string AccountNum { get; set; }
        public string FinSwitchRefNum { get; set; }
        public string Status { get; set; }
    }
}
