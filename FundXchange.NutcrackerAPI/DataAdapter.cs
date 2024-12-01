using System;
using System.Collections.Generic;
using FundXchange.Domain.Entities;
using FundXchange.NutcrackerAPI.JSEPublic;
using FundXchange.Domain.ValueObjects;
using Nutcracker.FeederAPI;

namespace FundXchange.NutcrackerAPI
{
    public class DataAdapter
    {
        public static List<Instrument> GetInstrumentsFromWatchlistShareRow(WatchlistShareRow[] rows)
        {
            List<Instrument> instruments = new List<Instrument>();

            foreach (WatchlistShareRow row in rows)
            {
                Instrument instrument = new Instrument();

                instrument.BestBid = (long)row.BidPrice;
                instrument.BestOffer = (long)row.AskPrice;
                instrument.BidVolume = row.BidSize;
                instrument.CentsMoved = (long)row.Change;
                instrument.Exchange = "JSE";//??
                instrument.High = (long)row.High;
                instrument.LastTrade = (long)row.LastTradePrice;
                instrument.Low = (long)row.Low;
                instrument.OfferVolume = row.AskSize;
                instrument.Open = (long)row.YesterdaysClosePrice;
                instrument.PercentageMoved = (long)row.ChangePercentage;
                instrument.Symbol = row.Symbol;
                instrument.TotalTrades = row.NoOfTrades;
                instrument.TotalValue = (long)row.DailyValue;
                instrument.TotalVolume = row.DailyVolume;
                instrument.YesterdayClose = (long)row.YesterdaysClosePrice;

                instruments.Add(instrument);
            }

            return instruments;
        }

        public static List<Index> GetIndexesFromIndexValues(IndexValue[] rows)
        {
            List<Index> indexes = new List<Index>();

            foreach (IndexValue row in rows)
            {
                Index index = new Index();

                index.CentsMoved = (long)row.Differential;
                index.CompanyShortName = row.ShortName;
                index.Exchange = "JSE"; //???
                index.Last12MonthClosePrice = 0; //???
                index.LastMonthClosePrice = 0;
                index.LastWeekClosePrice = 0;
                index.LastYearClosePrice = 0;
                index.PercentageMoved = 0;
                index.ShortName = row.FullName;
                index.Symbol = row.IndexIdentifier;
                index.SymbolCode = row.IndexIdentifier;
                index.Value = (long)row.Value;
                index.YesterdayClose = 0;

                indexes.Add(index);
            }

            return indexes;
        }

        public static List<Sens> GetSensFromNewsMessages(NewsMessage[] newsItems)
        {
            List<Sens> sens = new List<Sens>();

            foreach (NewsMessage news in newsItems)
            {
                Sens sensItem = new Sens();

                sensItem.Description = news.Message;
                sensItem.NewsId = news.MsgId;
                string time = news.TimeStamp.TimeOfDay.ToString();
                string[] timeSplit = time.Split('.');
                sensItem.Time = timeSplit[0];

                sens.Add(sensItem);
            }

            return sens;
        }

        public static List<Trade> GetTradesFromPublicTrades(string symbol, PublicTrade[] result)
        {
            List<Trade> trades = new List<Trade>();

            foreach (PublicTrade pubTrade in result)
            {
                Trade trade = new Trade();

                trade.Symbol = symbol;
                trade.Exchange = "JSE";
                trade.Price = (long)pubTrade.LastTradePrice;

                string time = pubTrade.LastTradeTime;
                trade.TimeStamp = Convert.ToDateTime(time);

                trade.Volume = pubTrade.LastTradeSize;
                trade.SymbolCode = pubTrade.TradeableInstrumentCode;

                trades.Add(trade);
            }

            return trades;
        }

        public static Instrument GetInstrumentFromInstrumentData(InstrumentData dto)
        {
            Instrument instrument = new Instrument();

            instrument.BestBid = (long) dto.BestBid;
            instrument.BestOffer = (long)dto.BestAsk;
            instrument.BidVolume = (long)dto.BestBidVolume;
            instrument.OfferVolume = (long)dto.BestAskVolume;
            instrument.Bids = new List<Bid>();
            instrument.CentsMoved = (long)dto.Move;
            instrument.Exchange = "JSE";
            instrument.High = (long)dto.High;
            instrument.LastTrade = (long)dto.LastTrade;
            instrument.Low = (long)dto.Low;
            instrument.Offers = new List<Offer>();
            instrument.Open = (long)dto.Open;
            instrument.Orders = new List<Order>();
            instrument.PercentageMoved = (decimal)dto.MovePerc;
            instrument.Symbol = dto.Symbol;
            instrument.TotalTrades = dto.Deals;
            instrument.TotalValue = (long)dto.DailyValue;
            instrument.TotalVolume = (long)dto.DailyVolume;
            instrument.YesterdayClose = (long)dto.PrevClose;

            return instrument;
        }
    }
}
