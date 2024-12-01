using System;

namespace FundXchange.Model.ViewModels.DataManager
{
    public class DataManagerInstrument
    {
        public string InstrumentID { get; set; }
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public string Exchange { get; set; }
        public string Time { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Bid { get; set; }
        public long BidVolume { get; set; }
        public double Offer { get; set; }
        public long OfferVolume { get; set; }
        public double Close { get; set; }
        public double YesterdayClose { get; set; }
        public double Move { get; set; }
        public double PercentageMove { get; set; }
        public long Deals { get; set; }
        public double DailyValue { get; set; }
        public double DailyVolume { get; set; }
        public double Open { get; set; }
        public double Price { get; set; }
    }
}
