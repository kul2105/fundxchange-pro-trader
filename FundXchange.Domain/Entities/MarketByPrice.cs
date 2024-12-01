using System;
using FundXchange.Domain.Base;

namespace FundXchange.Domain.Entities
{
    public class MarketByPrice : InstrumentBase
    {
        public int Id { get; set; }
        //public long Price { get; set; }
        public double Price { get; set; }
        public long Size { get; set; }
        public OrderSide Side { get; set; }
        public int OrderCount { get; set; }
    }
}
