using System;

namespace FundXchange.Brokerage.Model.ValueObjects
{
    public class Transaction
    {
        public string Exchange { get; set; }
        public string Symbol { get; set; }
        public int Price { get; set; }
        public int Volume { get; set; }
        public DateTime TradeDate { get; set; }
    }
}
