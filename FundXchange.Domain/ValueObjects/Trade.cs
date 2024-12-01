using System;
using FundXchange.Domain.Base;
using FundXchange.Domain.Enumerations;

namespace FundXchange.Domain.ValueObjects
{
    public class Trade : InstrumentBase
    {
        public long SequenceNumber { get; set; }
        //public long Price { get; set; }
        public double Price { get; set; }
        public long BidVolume { get; set; }
        public long OfferVolume { get; set; }
        public long Volume { get; set; }

        public int Multiplier { get; set; }

        public DateTime TimeStamp { get; set; }
        public TradeStatus TradeStatus
        {
            get
            {
                if (Quote == null)
                    return TradeStatus.Unknown;
                if (Price == Quote.BestAskPrice)
                    return TradeStatus.AtOffer;
                if (Price == Quote.BestBidPrice)
                    return TradeStatus.AtBid;
                return TradeStatus.BetweenBidAndOffer;
            }
        }
        public Quote Quote { get; set; }
    }
}
