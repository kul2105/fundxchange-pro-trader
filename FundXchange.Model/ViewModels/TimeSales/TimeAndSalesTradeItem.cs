using System;
using System.Drawing;
using FundXchange.Domain.Enumerations;

namespace FundXchange.Model.ViewModels.TimeSales
{
    public class TimeAndSalesTradeItem
    {
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public DateTime TradeTime { get; set; }
        public string Type
        {
            get
            {
                return "Trade";
            }
        }
        public double Price { get; set; }
        public long Size { get; set; }

        public TradeStatus Condition { get; set; }
        public string ConditionDisplay
        {
            get
            {
                switch (Condition)
                {
                    case TradeStatus.AtBid:
                        return "At Bid";
                    case TradeStatus.AtOffer:
                        return "At Offer";
                    case TradeStatus.BetweenBidAndOffer:
                        return "Btwn B&O";
                    case TradeStatus.Unknown:
                        return "Unknown";
                }
                return "Unknown";
            }
        }
        public Color Color
        {
            get
            {
                switch (Condition)
                {
                    case TradeStatus.AtBid:
                        return Color.Red;
                    case TradeStatus.AtOffer:
                        return Color.LimeGreen;
                    case TradeStatus.BetweenBidAndOffer:
                        return Color.White;
                    case TradeStatus.Unknown:
                        return Color.White;
                }
                return Color.White;
            }
        }
    }
}
