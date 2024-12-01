using System;
using FundXchange.Domain.Base;

namespace FundXchange.Domain.Entities
{
    public class OrderbookItem : InstrumentBase, ICloneable
    {
        public OrderSide Side { get; set; }
        public int Position { get; set; }
        //public long Price { get; set; }
        public double Price { get; set; }
        public long Size { get; set; }
        public int NumberOfQuotes { get; set; }
        public string MarketMakerCode { get; set; }
        public string TradingPeriodName { get; set; }
        public int DepthEntryId { get; set; }
        public int CumulativeVolumeAutomaticTrades { get; set; }
        public int CumulativeVolumeNonAutomaticTrades { get; set; }
        public int TWAS { get; set; }
        public int PeriodLengthOfTWAS { get; set; }
        public DateTime OrderDate { get; set; }
        public string OriginalOrderCode { get; set; }
        public int ClosingBid { get; set; }
        public int ClosingAsk { get; set; }
        public int DOL_High { get; set; }
        public int DOL_Low { get; set; }
        public DateTime TradeDate { get; set; }
        public string ISIN { get; set; }
        public long SequenceNumber { get; set; }


        public object Clone()
        {
            OrderbookItem clone= new OrderbookItem();
            clone.Side = Side;
            clone.Position = Position;
            clone.Price = Price;
            clone.Size = Size;
            clone.NumberOfQuotes = NumberOfQuotes;
            clone.MarketMakerCode = MarketMakerCode;
            clone.TradingPeriodName = TradingPeriodName;
            clone.DepthEntryId = DepthEntryId;
            clone.CumulativeVolumeAutomaticTrades = CumulativeVolumeAutomaticTrades;
            clone.CumulativeVolumeNonAutomaticTrades = CumulativeVolumeNonAutomaticTrades;
            clone.TWAS = TWAS;
            clone.PeriodLengthOfTWAS = PeriodLengthOfTWAS;
            clone.OrderDate = OrderDate;
            clone.OriginalOrderCode = OriginalOrderCode;
            clone.ClosingBid = ClosingBid;
            clone.ClosingAsk = ClosingAsk;
            clone.DOL_High = DOL_High;
            clone.DOL_Low = DOL_Low;
            clone.TradeDate = TradeDate;
            clone.ISIN = ISIN;
            clone.SequenceNumber = SequenceNumber;

            return clone;


        }
    }
}
