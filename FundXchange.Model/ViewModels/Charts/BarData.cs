using System;

namespace FundXchange.Model.ViewModels.Charts
{
    [Serializable]
    public class BarData
    {
        public DateTime StartDateTime;
        public DateTime TradeDateTime;
        public DateTime CloseDateTime;
        public double OpenPrice;
        public double HighPrice;
        public double LowPrice;
        public double ClosePrice;
        public double Volume;
        public long CloseSequenceNumber { get; set; }

        public string Symbol;
        public int interval { get; set; }
        public string Periodicity { get; set; }
    }
}
