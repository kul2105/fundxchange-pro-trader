using System;

namespace M4.DataManagerItems
{
    public class PriceUpdates
    {
        public DateTime TradeDateTime;
        public string Excange;
        public string Symbol;
        public double Price;
        public long Volume;
        public double Bid;
        public long BidSize;
        public double Ask;
        public long AskSize;
        public bool SystemWatch;

        public Double High;
        public Double Low;
        public Double YesterdayClose;
        public Double Move;
        public Decimal MovePercent;
        public long Deals;
        public Double DailyValue;
        public Double DailyVolume;

        public Double LastWeekClosePrice;
        public Double LastMonthClosePrice;
        public Double LastYearClosePrice;
        public Double Last12MonthClosePrice;
        public Decimal MovePercentLastWeek;
        public Decimal MovePercentLastMonth;
        public Decimal MovePercentLastYear;
        public Decimal MovePercentLast12MonthAgo;
        public Double OpenPrice;
    }
}
