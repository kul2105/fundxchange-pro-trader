using System;
using FundXchange.Domain.Entities;
using System.Collections.Generic;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.Infrastructure;
using FundXchange.Model.ViewModels.Matrix;
using FundXchange.Model.Agents;

namespace FundXchange.Tests.UnitTests
{
    public class InstrumentFactory
    {
        public static Instrument CreateInstrument(string symbol, string exchange)
        {
            Instrument instrument = new Instrument();

            instrument.BestBid = 1;
            instrument.BestOffer = 2;
            instrument.BidVolume = 11;
            instrument.CentsMoved = 3;
            instrument.Exchange = exchange;
            instrument.High = 9;
            instrument.ISIN = "123";
            instrument.LastTrade = 5;
            instrument.Low = 2;
            instrument.OfferVolume = 22;
            instrument.Open = 3;
            instrument.PercentageMoved = 0.5;
            instrument.Symbol = symbol;
            instrument.TotalTrades = 10;
            instrument.TotalValue = 20;
            instrument.TotalVolume = 30;
            instrument.YesterdayClose = 4;

            return instrument;
        }

        public static List<Trade> CreateTrades(string symbol, string exchange, int numTrades)
        {
            List<Trade> trades = new List<Trade>();
            
            for (int i = 0; i < numTrades; i++)
            {
                Trade trade = CreateTrade(symbol, exchange);
                trades.Add(trade);
            }

            return trades;
        }

        public static Trade CreateTrade(string symbol, string exchange)
        {
            Trade trade = new Trade();

            Random rand = new Random();
            int maxValue = 1000;

            trade.BidVolume = rand.Next(maxValue);
            trade.CompanyLongName = symbol;
            trade.CompanyShortName = symbol;
            trade.Exchange = exchange;
            trade.OfferVolume = rand.Next(maxValue);
            trade.Price = rand.Next(maxValue);
            trade.Quote = CreateQuote(symbol, exchange);
            trade.SequenceNumber = rand.Next(maxValue);
            trade.Symbol = symbol;
            trade.SymbolCode = symbol;
            trade.TimeStamp = DateTime.Now;
            trade.Volume = rand.Next(maxValue);

            return trade;
        }

        public static Quote CreateQuote(string symbol, string exchange)
        {
            Quote quote = new Quote();
            int maxValue = 1000;
            Random rand = new Random();

            quote.BestAskPrice = rand.Next(maxValue);
            quote.BestAskSize = rand.Next(maxValue);
            quote.BestBidPrice = rand.Next(maxValue);
            quote.BestBidSize = rand.Next(maxValue);
            quote.CompanyLongName = symbol;
            quote.CompanyShortName = symbol;
            quote.Exchange = exchange;
            quote.QuoteDateAndTime = DateTime.Now;
            quote.Symbol = symbol;
            quote.SymbolCode = symbol;

            return quote;
        }

        public static DepthByPrice CreateDepthByPrice(string symbol, string exchange, int numOrders, bool isBuy)
        {
            DepthByPrice item = new DepthByPrice(symbol, exchange);

            int maxValue = 1000;
            Random rand = new Random();

            item.Price = rand.Next(maxValue);
            item.Volume = rand.Next(maxValue);

            for (int i = 0; i < numOrders; i++)
            {
                OrderbookItem order = CreateOrderbookItem(symbol, exchange, isBuy, item.Price);
                item.Orders.Add(order);
            }

            return item;
        }

        public static OrderbookItem CreateOrderbookItem(string symbol, string exchange, bool isBuy, double price)
        {
            OrderbookItem order = new OrderbookItem();

            int maxValue = 1000;
            Random rand = new Random();

            order.ClosingAsk = rand.Next(maxValue);
            order.ClosingBid = rand.Next(maxValue);
            order.CompanyLongName = symbol;
            order.CompanyShortName = symbol;
            order.CumulativeVolumeAutomaticTrades = rand.Next(maxValue);
            order.CumulativeVolumeNonAutomaticTrades = rand.Next(maxValue);
            order.DepthEntryId = 5;
            order.DOL_High = rand.Next(maxValue);
            order.DOL_Low = rand.Next(maxValue);
            order.Exchange = exchange;
            order.ISIN = symbol;
            order.MarketMakerCode = symbol;
            order.NumberOfQuotes = rand.Next(maxValue);
            order.OrderDate = DateTime.Now;
            order.OriginalOrderCode = rand.Next(maxValue).ToString();
            order.PeriodLengthOfTWAS = rand.Next(maxValue);
            order.Position = rand.Next(maxValue);
            //order.Price = price;//John
            order.Side = OrderSide.Buy;
            if (!isBuy)
                order.Side = OrderSide.Sell;
            order.Size = rand.Next(maxValue);
            order.Symbol = symbol;
            order.SymbolCode = symbol;
            order.TradeDate = DateTime.Now;
            order.TradingPeriodName = symbol;
            order.TWAS = rand.Next(maxValue);

            return order;
        }

        public static MatrixViewItem CreateMatrixViewItem()
        {
            MatrixViewItem item = new MatrixViewItem();

            item.BidOrderCount = 1;
            item.BidSize = 2;
            item.OfferOrderCount = 3;
            item.OfferSize = 4;
            item.Price = 5;
            item.TradeVolume = 6;

            return item;
        }

        public static List<Candlestick> CreateCandlesticks(string symbol, string exchange, int numBars, int barInterval)
        {
            List<Candlestick> candles = new List<Candlestick>();
            int maxValue = 1000;
            Random rand = new Random();
            int numMinutes = numBars * barInterval;

            DateTime endTime = DateTime.Now;
            DateTime startTime = endTime.AddMinutes(-numMinutes);
            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 0);

            int counter = 0;
            while (startTime < endTime)
            {
                counter++;
                Candlestick candle = new Candlestick();

                candle.Close = rand.Next(maxValue);
                candle.CompanyLongName = symbol;
                candle.CompanyShortName = symbol;
                candle.Exchange = exchange;
                candle.High = rand.Next(maxValue);
                candle.Low = rand.Next(maxValue);
                candle.Open = rand.Next(maxValue);
                candle.Symbol = symbol;
                candle.SymbolCode = symbol;
                candle.Volume = rand.Next(maxValue);

                if (counter != numBars)
                    candle.TimeOfClose = startTime;

                candles.Add(candle);
                startTime = startTime.AddMinutes(barInterval);
                
            }

            return candles;
        }
    }
}
