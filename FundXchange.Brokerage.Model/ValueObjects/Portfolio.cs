using System;
using System.Collections.Generic;

namespace FundXchange.Brokerage.Model.ValueObjects
{
    public class Portfolio
    {
        public int AtHomeQuantity { get; set; }
        public int AtHomeCost { get; set; }
        public int CashOnHand { get; set; }
        public List<PortfolioInstrument> Instruments { get; set; }
    }
}
