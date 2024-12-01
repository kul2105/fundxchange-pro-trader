using System;
using FundXchange.Domain.Base;

namespace FundXchange.Domain.ValueObjects
{
    public class Index : InstrumentBase
    {
        public string ShortName { get; set; }
        public string Alpha { get; set; }
        public long Value { get; set; }
        public long CentsMoved { get; set; }
        public double PercentageMoved { get; set; }
        public long SequenceNumber { get; set; }
        public DateTime TimeStamp { get; set; }

        public long YesterdayClose { get; set; }
        public long LastWeekClosePrice { get; set; }
        public long LastMonthClosePrice { get; set; }
        public long LastYearClosePrice { get; set; }
        public long Last3MonthClosePrice { get; set; }
        
    }
}
