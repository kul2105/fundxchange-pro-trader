using System;

namespace FundXchange.Domain.Base
{
    public abstract class InstrumentBase
    {
        public string Symbol { get; set; }
        public string SymbolCode { get; set; }
        public string CompanyShortName { get; set; }
        public string CompanyLongName { get; set; }
        public string Exchange { get; set; }
        
    }
}
