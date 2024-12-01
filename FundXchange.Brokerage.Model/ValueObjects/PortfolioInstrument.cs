using System;

namespace FundXchange.Brokerage.Model.ValueObjects
{
    public class PortfolioInstrument
    {
        public string Symbol { get; set; }
        public string ShortName { get; set; }
        public int TotalCost { get; set; }
        public int CurrentPrice { get; set; }
        public int Quantity { get; set; }
    }
}
