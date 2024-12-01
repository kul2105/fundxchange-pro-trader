using System;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using FundXchange.Morningstar.Messages;
using FundXchange.Morningstar.Enumerations;
using System.Collections.Generic;
using System.Diagnostics;
using FundXchange.Domain.Enumerations;
using FundXchange.Model.ExchangeService;

namespace FundXchange.Morningstar
{
    public class DataAdapter
    {
        public static IEnumerable<InstrumentReference> GetInstrumentReferencesFromFileValues(List<string> stocks)
        {
            stocks.RemoveAt(0);

            foreach (string stock in stocks)
            {
                string[] stockData = stock.Split('|');

                var instrumentReference = new InstrumentReference
                {
                    AlternativeSymbol = string.IsNullOrEmpty(stockData[0].Trim()) ? "" : stockData[0],
                    Exchange = GetExchangeNameFromMorningStarId(stockData[4].Trim()),
                    ISIN = string.IsNullOrEmpty(stockData[5].Trim()) ? "" : stockData[5],
                    MarketSegment = string.IsNullOrEmpty(stockData[8].Trim()) ? "" : stockData[8],
                    Security = string.IsNullOrEmpty(stockData[7].Trim()) ? "" : stockData[7],
                    ShortName = string.IsNullOrEmpty(stockData[1].Trim()) ? "" : stockData[1],
                    Symbol = string.IsNullOrEmpty(stockData[9].Trim()) ? stockData[0] : stockData[9]
                };

                yield return instrumentReference;
            }
        }

        public static Instrument GetInstrumentFromRawMessage(MorningstarMessage message, ref Repository repository)
        {
            
            if(message == null) return null;

            string symbol = message.GetFieldValueAs<string>(FieldIdentifiers.TF_FIELD_SYMBOL);
            string exchange = Enum.GetName(typeof (Exchanges),
                                               message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LAST_MARKET));

            Instrument instrument = repository.GetInstrument(symbol);
            if (instrument == null)
            {
                instrument = new Instrument();
                instrument.Symbol = symbol;
                instrument.Exchange = exchange;
            }

            if (message.HasField(FieldIdentifiers.TF_FIELD_LAST))
            {
                instrument.LastTrade = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_LAST);
                long open = instrument.Open;
                if (message.HasField(FieldIdentifiers.TF_FIELD_OPEN))
                {
                    open = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_OPEN);
                }
                instrument.CentsMoved = instrument.LastTrade - open;
                instrument.PercentageMoved = GetPercentageMoved(open, instrument.LastTrade);
            }
            if (message.HasField(FieldIdentifiers.TF_FIELD_LAST))
                instrument.High = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_HIGH);

            if (message.HasField(FieldIdentifiers.TF_FIELD_LOW))
                instrument.Low = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_LOW);
            if (message.HasField(FieldIdentifiers.TF_FIELD_OPEN))
                instrument.Open = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_OPEN);
            if (message.HasField(FieldIdentifiers.TF_FIELD_TRADE_COUNT))
                instrument.TotalTrades = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_TRADE_COUNT);
            if (message.HasField(FieldIdentifiers.TF_FIELD_CUM_VOLUME))
                instrument.TotalVolume = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_CUM_VOLUME);
            if (message.HasField(FieldIdentifiers.TF_FIELD_OPEN))
                instrument.YesterdayClose = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_OPEN);
            if (message.HasField(FieldIdentifiers.TF_FIELD_VWAP_PRICE))
                instrument.TotalValue = (long)(message.GetFieldValueAs<decimal>(FieldIdentifiers.TF_FIELD_VWAP_PRICE) * (instrument.TotalTrades));
            if (message.HasField(FieldIdentifiers.TF_FIELD_BID))
                instrument.BestBid = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_BID);
            if (message.HasField(FieldIdentifiers.TF_FIELD_BID_SIZE))
                instrument.BidVolume = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_BID_SIZE);
            if (message.HasField(FieldIdentifiers.TF_FIELD_ASK))
                instrument.BestOffer = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_ASK);
            if (message.HasField(FieldIdentifiers.TF_FIELD_ASK_SIZE))
                instrument.OfferVolume = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_ASK_SIZE);

            repository.UpdateInstrument(instrument);

            return instrument;
        }

        public static Trade GetTradeFromRawMessage(MorningstarMessage message)
        {
            if(message == null) return null;

            var trade = new Trade();

            trade.Symbol = message.GetFieldValueAs<string>(FieldIdentifiers.TF_FIELD_SYMBOL);
            trade.Exchange = Enum.GetName(typeof (Exchanges),
                                          message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LAST_MARKET));
            trade.Volume = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_VOLUME);
            trade.TimeStamp = DateTime.FromBinary(message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_ORG_TRADE_TIME)); // DateTime.Parse(.ToString()),
            trade.BidVolume = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_VOLUME);
            trade.OfferVolume = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_VOLUME);
            trade.Price = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_LAST);
            trade.TradeStatus = TradeStatus.AtBid; // Need to find this value 

            return trade;
        }

        public static Quote GetQuoteFromRawMessage(MorningstarMessage message, ref Repository repository)
        {
            if(message == null) return null;

            string symbol = message.GetFieldValueAs<string>(FieldIdentifiers.TF_FIELD_SYMBOL);
            string exchange = Enum.GetName(typeof(Exchanges),
                                               message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LAST_MARKET));

            Quote quote = repository.GetQuote(symbol);
            if (quote == null)
            {
                quote = new Quote();
                quote.Symbol = symbol;
                quote.Exchange = exchange;
            }

            if (message.HasField(FieldIdentifiers.TF_FIELD_ASK))
                quote.BestAskPrice = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_ASK);
            if (message.HasField(FieldIdentifiers.TF_FIELD_ASK_SIZE))
                quote.BestAskSize = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_ASK_SIZE);
            if (message.HasField(FieldIdentifiers.TF_FIELD_BID))
                quote.BestBidPrice = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_BID);
            if (message.HasField(FieldIdentifiers.TF_FIELD_BID_SIZE))
                quote.BestBidSize = message.GetFieldValueAs<long>(FieldIdentifiers.TF_FIELD_BID_SIZE);

            return quote;
        }

        public static OrderEvent GetOrderEventFromRawMessage(MorningstarMessage message, Dictionary<string, InstrumentDTO> instruments)
        {
            if (message == null) return null;

            var orderEvent = new OrderEvent();
            var order = new OrderbookItem();

            order.SymbolCode = message.GetFieldValueAs<string>(FieldIdentifiers.TF_FIELD_SYMBOL);
            //order.Symbol = instruments[order.SymbolCode].Symbol;
            //order.CompanyShortName = instruments[order.SymbolCode].ShortName;

            order.Exchange = Enum.GetName(typeof(Exchanges),
                                          message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LAST_MARKET));

            int operation = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_AMD);
            if (operation == 1)
                orderEvent.Operation = OrderEventType.Add;
            else if (operation == 2)
                orderEvent.Operation = OrderEventType.Update;
            else if (operation == 3)
                orderEvent.Operation = OrderEventType.Delete;

            int orderSide = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_SIDE);
            if (orderSide == 0)
                order.Side = OrderSide.Buy;
            else
                order.Side = OrderSide.Sell;

            order.Position = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_POS);
            order.Price = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_PRICE);
            order.Size = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_SIZE);
            order.NumberOfQuotes = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_NO);
            order.MarketMakerCode = message.GetFieldValueAs<string>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_MM);
            order.TradingPeriodName = message.GetFieldValueAs<string>(FieldIdentifiers.TF_FIELD_LSE_PERIOD);
            order.DepthEntryId = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_ID);
            order.CumulativeVolumeAutomaticTrades = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_AVOL);
            order.CumulativeVolumeNonAutomaticTrades = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_NVOL);
            order.TWAS = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_TWAS);
            order.PeriodLengthOfTWAS = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_PERIOD_LEN);

            int time = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_TIME);
            int date = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_DATE);
            order.OrderDate = GetDateAndTime(date, time);

            order.OriginalOrderCode = message.GetFieldValueAs<string>(FieldIdentifiers.TF_FIELD_LSE_DEPTH_CODE);
            order.ClosingBid = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_CLOSE_BID);
            order.ClosingAsk = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_LSE_CLOSE_ASK);
            order.DOL_High = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_DOL_HIGH);
            order.DOL_Low = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_DOL_LOW);
            int tradeDate = message.GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_TRADE_DATE);
            order.TradeDate = GetDate(tradeDate);

            orderEvent.Order = order;

            return orderEvent;
        }

        #region Private Methods

        private static DateTime GetDate(int julianDate)
        {
            if (julianDate == 0)
                return DateTime.MinValue;
            return Convert.ToDateTime(julianDate);
        }

        private static DateTime GetDateAndTime(int date, int time)
        {
            if (date == 0)
                return DateTime.MinValue;

            DateTime dateObj = Convert.ToDateTime(date);
            int hour = time / 60;
            int minute = time - (hour * 60);

            return new DateTime(dateObj.Year, dateObj.Month, dateObj.Day, hour, minute, 0);

        }

        private static string GetExchangeNameFromMorningStarId(string morningStarExchangeId)
        {
            if (string.IsNullOrEmpty(morningStarExchangeId))
                return "";
            else
                return Enum.GetName(typeof(Exchanges), Convert.ToInt32(morningStarExchangeId));
        }

        private static decimal GetPercentageMoved(long valueBefore, long valueNow)
        {
            long movement = valueNow - valueBefore;

            if (movement != 0)
            {
                try
                {
                    return (decimal) ((decimal) movement/(decimal) valueBefore)*100M;
                }
                catch (DivideByZeroException ex)
                {
                    Debug.WriteLine(ex.ToString());
                    return 0;
                }
            }

            return 0;
        }
        #endregion
    }
}
