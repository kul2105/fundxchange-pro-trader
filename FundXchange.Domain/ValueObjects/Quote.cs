using System;
using FundXchange.Domain.Base;

namespace FundXchange.Domain.ValueObjects
{
    public class Quote : InstrumentBase
    {
        public double BestAskPrice { get; set; }
        public long BestAskSize { get; set; }
        public double BestBidPrice { get; set; }
        public long BestBidSize { get; set; }
        public DateTime QuoteDateAndTime { get; set; }
    }
}
