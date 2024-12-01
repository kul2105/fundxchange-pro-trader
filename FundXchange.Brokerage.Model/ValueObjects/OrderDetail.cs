using System;

namespace FundXchange.Brokerage.Model.ValueObjects
{
    public class OrderDetail
    {
        public int MatchedToday { get; set; }
        public int VWAP { get; set; }
    }
}
