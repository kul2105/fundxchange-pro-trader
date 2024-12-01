using System;
using System.Collections.Generic;
using Fin24.Markets.State;
using Fin24.Markets.Client.StateRequest;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;

namespace FundXchange.DataProvider
{
    public class DataAdapter
    {
        public static Instrument GetInstrumentsFromDTOs(Fin24.Markets.State.InstrumentDTO dto)
        {
            var instrument = new Instrument();

            instrument.Symbol = dto.Symbol;
            instrument.Exchange = dto.Exchange;
            instrument.CentsMoved = dto.CentsMoved;
            instrument.High = dto.HighTradedPrice;
            instrument.LastTrade = dto.LastTradedPrice;
            instrument.Low = dto.LowTradedPrice;
            instrument.Open = dto.OpeningTradePrice;
            instrument.PercentageMoved = dto.PercentageMoved;
            instrument.Symbol = dto.Symbol;
            instrument.TotalTrades = dto.TotalTrades;
            instrument.TotalValue = dto.TotalValue;
            instrument.TotalVolume = dto.TotalVolume;
            instrument.YesterdayClose = dto.YesterdayClosePrice;

            return instrument;
        }

        public static Index GetIndexFromDTOs(IndexDTO dto)
        {
            var index = new Index();

            index.CentsMoved = dto.CentsMoved;
            index.Exchange = dto.Exchange;
            index.PercentageMoved = dto.PercentageMoved;
            index.Symbol = dto.Symbol;
            index.Value = dto.Value;

            return index;
        }

        public static Trade GetTradeFromDTO(Fin24.Markets.State.TradeDTO dto)
        {
            var trade = new Trade();

            trade.BidVolume = dto.BidVolume;
            trade.OfferVolume = dto.OfferVolume;
            trade.Price = dto.Price;
            trade.TimeStamp = dto.TradeTimeStamp;
            trade.Volume = dto.Volume;
            trade.Symbol = dto.Symbol;
            trade.Exchange = dto.Exchange;

            return trade;
        }

        public static Trade GetTradeFromDTO(Fin24.Markets.Client.StateRequest.TradeDTO dto)
        {
            var trade = new Trade();

            trade.BidVolume = 0;
            trade.OfferVolume = 0;
            trade.Price = dto.Price;
            trade.TimeStamp = dto.TradeTimeStamp;
            trade.Volume = dto.Volume;
            trade.Symbol = dto.Symbol;
            trade.Exchange = "JSE";

            return trade;
        }

        public static List<Quote> GetQuotesFromDTOs(List<Fin24.Markets.State.QuoteDTO> dtos)
        {
            List<Quote> quotes = new List<Quote>();

            foreach (Fin24.Markets.State.QuoteDTO dto in dtos)
            {
                Quote quote = GetQuoteFromQuoteDTO(dto);
                quotes.Add(quote);
            }
            return quotes;
        }

        public static List<Trade> GetTradesFromDTOs(List<Fin24.Markets.State.TradeDTO> dtos)
        {
            List<Trade> trades = new List<Trade>();

            foreach (Fin24.Markets.State.TradeDTO dto in dtos)
            {
                Trade trade = GetTradeFromDTO(dto);
                trades.Add(trade);
            }

            return trades;
        }

        public static List<Bid> GetBidsFromDTOs(IList<BidDTO> bidDTOs)
        {
            var bids = new List<Bid>();

            foreach (BidDTO dto in bidDTOs)
            {
                var bid = new Bid();

                bid.Symbol = dto.Symbol;
                bid.Exchange = dto.Exchange;
                bid.Price = dto.Price;
                bid.Size = dto.Size;

                bids.Add(bid);
            }

            return bids;
        }

        public static List<Offer> GetOffersFromDTOs(IList<OfferDTO> offerDTOs)
        {
            var offers = new List<Offer>();

            foreach (OfferDTO dto in offerDTOs)
            {
                var offer = new Offer();

                offer.Symbol = dto.Symbol;
                offer.Exchange = dto.Exchange;
                offer.Price = dto.Price;
                offer.Size = dto.Size;

                offers.Add(offer);
            }

            return offers;
        }

        public static List<Order> GetOrdersFromDTOs(IList<Fin24.Markets.State.OrderDTO> orderDTOs)
        {
            var orders = new List<Order>();

            foreach (Fin24.Markets.State.OrderDTO dto in orderDTOs)
            {
                var order = new Order();

                order.Symbol = dto.Symbol;
                order.Exchange = dto.Exchange;
                order.Id = dto.Id;
                order.Time = dto.LastUpdated;
                order.Price = dto.Price;
                order.Volume = dto.Volume;

                if (dto.Side == Fin24.Markets.State.OrderSide.Buy)
                    order.Side = FundXchange.Domain.Entities.OrderSide.Buy;
                else
                    order.Side = FundXchange.Domain.Entities.OrderSide.Sell;
                
                orders.Add(order);
            }
            return orders;
        }

        public static Quote GetQuoteFromQuoteDTO(Fin24.Markets.State.QuoteDTO dto)
        {
            Quote quote = new Quote();

            quote.BestAskPrice = dto.BestAskPrice;
            quote.BestAskSize = dto.BestAskSize;
            quote.BestBidPrice = dto.BestBidPrice;
            quote.BestBidSize = dto.BestBidSize;
            quote.Exchange = dto.Exchange;
            quote.Symbol = dto.Symbol;
            quote.QuoteDateAndTime = dto.QuoteDateAndTime;
            
            return quote;
        }

        public static List<Instrument> GetInstrumentsFromInstrumentSummaryDTOs(Fin24.Markets.Client.StateRequest.InstrumentSummaryDTO[] dtos)
        {
            List<Instrument> instruments = new List<Instrument>();
            if (dtos != null)
            {
                foreach (InstrumentSummaryDTO summary in dtos)
                {
                    Instrument instrument = new Instrument();

                    instrument.CentsMoved = summary.CentsMoved;
                    instrument.LastTrade = summary.LastTradedPrice;
                    instrument.PercentageMoved = summary.PercentageMoved;
                    instrument.Symbol = summary.Symbol;
                    instrument.TotalValue = summary.TotalValue;
                    instrument.TotalVolume = summary.TotalVolume;
                    instrument.YesterdayClose = summary.YesterdayClose;
                    instrument.Exchange = "JSE";

                    instruments.Add(instrument);
                }
            }
            return instruments;
        }

        public static List<Instrument> GetInstrumentsFromMoversDTOs(MoversDTO[] dtos)
        {
            List<Instrument> instruments = new List<Instrument>();
            if (dtos != null)
            {
                foreach (MoversDTO mover in dtos)
                {
                    Instrument instrument = new Instrument();

                    instrument.Symbol = mover.Symbol;
                    instrument.LastTrade = mover.LastTradedPrice;
                    instrument.TotalVolume = mover.TotalVolume;
                    instrument.CentsMoved = mover.CentsMoved;
                    instrument.PercentageMoved = mover.PercentageMoved;

                    instruments.Add(instrument);
                }
            }
            return instruments;
        }
        public static List<InstrumentSnapshot> GetSnapshotsFromDTOs(IList<Fin24.Markets.State.SnapshotDTO> retList)
        {
            List<InstrumentSnapshot> snapshots = new List<InstrumentSnapshot>();
            if (retList != null)
            {
                foreach (Fin24.Markets.State.SnapshotDTO dto in retList)
                {
                    InstrumentSnapshot snapshot = new InstrumentSnapshot();

                    snapshot.Close = dto.ClosePrice;
                    snapshot.Exchange = dto.Exchange;
                    snapshot.High = dto.HighPrice;
                    snapshot.Low = dto.LowPrice;
                    snapshot.Open = dto.OpenPrice;
                    snapshot.SnapshotTime = dto.SnapshotTime;
                    snapshot.Symbol = dto.Symbol;
                    snapshot.Volume = dto.Volume;

                    snapshots.Add(snapshot);
                }
            }
            return snapshots;
        }

        public static Index GetIndexFromInstrumentDTO(InstrumentReferenceDTO reference, InstrumentAndTradesDTO instrumentsWithTrades)
        {
            Index index = new Index();

            index.Symbol = reference.Symbol;
            index.ShortName = reference.ShortName;

            if (instrumentsWithTrades != null && instrumentsWithTrades.Instrument != null)
            {
                index.Value = instrumentsWithTrades.Instrument.LastTradedPrice;
                index.CentsMoved = instrumentsWithTrades.Instrument.CentsMoved;
                index.Last12MonthClosePrice = instrumentsWithTrades.Instrument.Last12MonthsClosePrice;
                index.LastMonthClosePrice = instrumentsWithTrades.Instrument.LastMonthClosePrice;
                index.LastWeekClosePrice = instrumentsWithTrades.Instrument.LastWeekClosePrice;
                index.LastYearClosePrice = instrumentsWithTrades.Instrument.LastYearClosePrice;
                index.PercentageMoved = instrumentsWithTrades.Instrument.PercentageMoved;
                index.YesterdayClose = instrumentsWithTrades.Instrument.YesterdayClosePrice;
            }
            return index;
        }

        public static List<FundXchange.Domain.ValueObjects.InstrumentStatistics> GetInstrumentStatistics(List<Fin24.Markets.Client.StateRequest.InstrumentStatistics> instrumentList)
        {
            var statistics = new List<FundXchange.Domain.ValueObjects.InstrumentStatistics>();

            if (instrumentList != null)
            {
                foreach (Fin24.Markets.Client.StateRequest.InstrumentStatistics instrument in instrumentList)
                {
                    var stat = new FundXchange.Domain.ValueObjects.InstrumentStatistics();

                    stat.Currency = instrument.Currency;
                    stat.DateStamp = instrument.DateStamp;
                    stat.Exchange = instrument.Exchange;
                    stat.Movement = instrument.Movement;
                    stat.MovementIndicator = instrument.MovementIndicator;
                    stat.PercentageMoved = instrument.PercentageMoved;
                    stat.RP = instrument.RP;
                    stat.ShortName = instrument.ShortName;
                    stat.Symbol = instrument.Symbol;
                    stat.Time = instrument.Time;

                    statistics.Add(stat);
                }
            }
            return statistics;
        }

        public static List<FundXchange.Domain.ValueObjects.Sens> GetSens(Fin24.Markets.Client.StateRequest.Sens[] sensList)
        {
            var sens = new List<FundXchange.Domain.ValueObjects.Sens>();
            if (sensList != null)
            {
                foreach (Fin24.Markets.Client.StateRequest.Sens s in sensList)
                {
                    var newSens = new FundXchange.Domain.ValueObjects.Sens();

                    newSens.Description = s.Description;
                    newSens.Story = s.Story;
                    newSens.Ticker = s.Ticker;
                    newSens.Time = s.Time;

                    sens.Add(newSens);
                }
            }
            return sens;
        }

        public static List<InstrumentReference> GetInstrumentReferencesFromDTOs(InstrumentReferenceDTO[] dtos)
        {
            var instruments = new List<InstrumentReference>();

            if (dtos != null)
            {
                foreach (InstrumentReferenceDTO dto in dtos)
                {
                    InstrumentReference instrument = new InstrumentReference();

                    instrument.Exchange = dto.Exchange;
                    instrument.Symbol = dto.Symbol;
                    instrument.ShortName = dto.ShortName;
                    
                    instruments.Add(instrument);
                }
            }
            return instruments;
        }

        public static Index GetIndexFromInstrumentDTO(Fin24.Markets.Client.StateRequest.InstrumentWithDescriptionDTO instrument)
        {
            Index index = new Index();

            //index.Exchange = reference.ex;
            index.ShortName = instrument.ShareShortName;
            index.Symbol = instrument.Symbol;
            index.CentsMoved = instrument.CentsMoved;
            index.Last12MonthClosePrice = instrument.Last12MonthsClosePrice;
            index.LastMonthClosePrice = instrument.LastMonthClosePrice;
            index.LastWeekClosePrice = instrument.LastWeekClosePrice;
            index.LastYearClosePrice = instrument.LastYearClosePrice;
            index.PercentageMoved = instrument.PercentageMoved;
            index.Value = instrument.LastTradedPrice;
            index.YesterdayClose = instrument.YesterdayClosePrice;

            return index;
        }
    }
}
