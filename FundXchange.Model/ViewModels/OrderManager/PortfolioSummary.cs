using System;

namespace FundXchange.Model.ViewModels.OrderManager
{
    public class PortfolioSummary
    {
        public decimal PortfolioValue { get; set; }
        public decimal TotalProfitLoss { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal Trades { get; set; }
    }
}
