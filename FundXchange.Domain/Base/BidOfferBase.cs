using System;

namespace FundXchange.Domain.Base
{
    public class BidOfferBase : InstrumentBase
    {
        public long Price { get; set; }
        public long Size { get; set; }
        public int OrderCount { get; set; }
    }
}
