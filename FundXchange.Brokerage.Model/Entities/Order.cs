using System;

namespace FundXchange.Brokerage.Model.Entities
{
    public class Order : EntityBase
    {
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public TradingAccount TradingAccount { get; set; }
        public long Price { get; set; }
        public long Volume { get; set; }
    }
}
