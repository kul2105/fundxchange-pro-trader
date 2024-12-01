using System;
using System.Collections.Generic;
using FundXchange.Domain.ValueObjects;

namespace FundXchange.Domain.Entities
{
    public class Instrument
    {
        public string Symbol { get; set; }
        public string InstrumentID { get; set; }
        public string ISIN { get; set; }
        public string Exchange { get; set; }
        public string CompanyShortName { get; set; }
        public string CompanyLongName { get; set; }
        public double CentsMoved { get; set; }
        public double LastTrade { get; set; }
        public DateTime LastTradeTime { get; set; }
        public long LastTradeSequenceNumber { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
        public double BestBid { get; set; }
        public long BidVolume { get; set; }
        public double BestOffer { get; set; }
        public long OfferVolume { get; set; }
        public double PercentageMoved { get; set; }
        public long TotalTrades { get; set; }
        public long TotalValue { get; set; }
        public long TotalVolume { get; set; }
        public double YesterdayClose { get; set; }
        public bool IsIndex { get; set; }
        public int Multiplier{ get; set; }// John

        public List<Bid> Bids { get; set; }
        public List<Offer> Offers { get; set; }
        public List<Order> Orders { get; set; }
        public List<Trade> Trades { get; private set; }
        public List<Quote> Quotes { get; private set; }

        public Instrument()
        {
            Trades = new List<Trade>();
            Bids = new List<Bid>();
            Offers = new List<Offer>();
            Orders = new List<Order>();
            Quotes = new List<Quote>();
        }

        public void AddTrade(Trade trade)
        {
            //Trades.Add(trade);

            //TotalValue += (trade.Volume * trade.Price);
            //TotalValue += trade.Volume;
            //LastTrade = trade.Price;
            //TotalTrades++;
        }

        public void AddQuote(Quote quote)
        {
            Quotes.Add(quote);

            BestBid = quote.BestBidPrice;
            BidVolume = quote.BestBidSize;
            BestOffer = quote.BestAskPrice;
            OfferVolume = quote.BestAskSize;
        }

        public override bool Equals(object obj)
        {
            if (obj is Instrument)
            {
                return this.Symbol == (obj as Instrument).Symbol && this.Exchange == (obj as Instrument).Exchange;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.ISIN.GetHashCode();
        }
    }
}
