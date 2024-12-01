using System;
using System.Collections.Generic;
using Fin24.LiveData.Candlestick.Services.API.DTOs;
using Fin24.LiveData.Common.MarketModel.MarketMessages;
using FundXchange.Data.McGregorBFA.ServiceDesk;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;

namespace FundXchange.Data.McGregorBFA
{
    public class DataAdapter
    {
        public static List<Instrument> GetInstrumentsFromDtos(EquityDetailsDto[] dtos)
        {
            List<Instrument> instruments = new List<Instrument>();

            try
            {
                foreach (EquityDetailsDto dto in dtos)
                {
                    Instrument instrument = new Instrument();

                    instrument.BestBid = dto.BestBidPrice;
                    instrument.BestOffer = dto.BestOfferPrice;
                    instrument.BidVolume = dto.VolumeOrdersAtBestBidPrice;
                    instrument.OfferVolume = dto.VolumeOrdersAtBestOfferPrice;
                    instrument.Open = dto.Open;
                   
                    instrument.TotalTrades = 0;
                    instrument.TotalValue = dto.DailyValue;
                    instrument.TotalVolume = dto.DailyVolume;
                    instrument.YesterdayClose = dto.Open;
                    instrument.High = dto.High;
                    instrument.ISIN = dto.ISIN;
                    instrument.LastTrade = dto.LastTrade_TradePrice;
                    instrument.LastTradeSequenceNumber = dto.LastTrade_SequenceNumber;
                    instrument.Low = dto.Low;
                    instrument.CentsMoved = dto.CentsMoved;

                    instrument.Symbol = dto.Symbol;
                    instrument.Exchange = dto.Exchange;

                    if (instrument.YesterdayClose > 0)
                        instrument.PercentageMoved = (instrument.CentsMoved * 100) / instrument.YesterdayClose;

                    instruments.Add(instrument);
                }
            }
            catch (Exception ex)
            {
            }
            return instruments;
        }

        public static List<Instrument> GetInstrumentsFromDtos(Fin24.LiveData.Common.DTOs.EquityDetailsDtoCollection collection)
        {
            List<Instrument> instruments = new List<Instrument>();

            try
            {
                foreach (var dto in collection.Equities)
                {
                    Instrument instrument = new Instrument();

                    instrument.BestBid = dto.BestBidPrice;
                    instrument.BestOffer = dto.BestOfferPrice;
                    instrument.BidVolume = dto.VolumeOrdersAtBestBidPrice;
                    instrument.OfferVolume = dto.VolumeOrdersAtBestOfferPrice;
                    instrument.Open = dto.Open;

                    instrument.TotalTrades = 0;
                    instrument.TotalValue = dto.DailyValue;
                    instrument.TotalVolume = dto.DailyVolume;
                    instrument.YesterdayClose = dto.Open;
                    instrument.High = dto.High;
                    instrument.ISIN = dto.ISIN;
                    instrument.LastTrade = dto.LastTrade_TradePrice;
                    instrument.LastTradeSequenceNumber = dto.LastTrade_SequenceNumber;
                    instrument.Low = dto.Low;
                    instrument.CentsMoved = dto.CentsMoved;

                    instrument.Symbol = dto.Symbol;
                    instrument.Exchange = dto.Exchange;

                    if (instrument.YesterdayClose > 0)
                        instrument.PercentageMoved = (instrument.CentsMoved * 100) / instrument.YesterdayClose;

                    instruments.Add(instrument);
                }
            }
            catch (Exception ex)
            {
            }
            return instruments;
        }

        public static Trade GetTradeFromDto(TradeReport msg)
        {
            Trade trade = new Trade();

            trade.Price = msg.TradePrice / 100000000;
            trade.Symbol = msg.Symbol;
            trade.TimeStamp = ParseTime(msg.TradeTimeAsString);
            trade.Volume = msg.TradeSize;
            trade.SequenceNumber = msg.Header.SequenceNumber;

            return trade;
        }

        public static Quote GetQuoteFromDto(EnhancedBestPrice msg)
        {
            if (msg == null)
                return null;

            Quote quote = new Quote();

            quote.BestAskPrice = msg.BestOfferPrice / 100000000;
            quote.BestAskSize = msg.VolumeOfOrdersAtBestOfferPrice; ;
            quote.BestBidPrice = msg.BestBidPrice / 100000000;
            quote.BestBidSize = msg.VolumeOfOrdersAtBestBidPrice;
            quote.QuoteDateAndTime = DateTime.Now;

            return quote;
        }

        public static Quote GetQuoteFromDto(Fin24.LiveData.Common.DTOs.QuoteDto msg)
        {
            if (msg == null)
                return null;

            Quote quote = new Quote();

            quote.BestAskPrice = msg.BestOfferPrice / 100000000;
            quote.BestAskSize = msg.VolumeOfOrdersAtBestOfferPrice; ;
            quote.BestBidPrice = msg.BestBidPrice / 100000000;
            quote.BestBidSize = msg.VolumeOfOrdersAtBestBidPrice;
            quote.QuoteDateAndTime = DateTime.Now;

            return quote;
        }

        public static OrderbookItem GetOrderFromDto(OrderDetails msg, string symbol)
        {
            OrderbookItem order = new OrderbookItem();

            order.Exchange = msg.Header.ExchangeCode;
            order.Price = msg.OrderPrice / 100000000;
            order.Side = OrderSide.Buy;
            if (msg.BuySellIndicator == "S")
            {
                order.Side = OrderSide.Sell;
            }
            order.Symbol = symbol;//msg.Header.Symbol;
            order.SymbolCode = symbol;//msg.Header.Symbol;
            order.OrderDate = new DateTime(msg.Header.TimeMessageReceivedAsTicks);
            order.Size = msg.AggregateSize;
            order.OriginalOrderCode = msg.PrivateOrderCode;
            order.ISIN = msg.TradableInstrumentCode;

            return order;
        }

        public static OrderbookItem GetOrderFromDto(Fin24.LiveData.Common.DTOs.OrderDto orderDto, string symbol)
        {
            OrderbookItem order = new OrderbookItem();

            order.Exchange = orderDto.ExchangeCode;
            order.Price = orderDto.OrderPrice / 100000000;
            order.Side = OrderSide.Buy;
            if (orderDto.BuySellIndicator == "S")
            {
                order.Side = OrderSide.Sell;
            }
            order.Symbol = symbol;//msg.Header.Symbol;
            order.SymbolCode = symbol;//msg.Header.Symbol;
            order.OrderDate = new DateTime(orderDto.TimeMessageReceivedAsTicks);
            order.Size = orderDto.AggregateSize;
            order.OriginalOrderCode = orderDto.PrivateOrderCode;
            order.ISIN = orderDto.TradableInstrumentCode;

            return order;
        }

        public static List<Candlestick> GetEquityCandlesticksFromDtos(EquityCandlestick[] candlestickDtos, InstrumentReference instrumentReference)
        {
            List<Candlestick> result = new List<Candlestick>();

            foreach (EquityCandlestick dto in candlestickDtos)
            {
                Candlestick snapshot = new Candlestick
                                          {
                                              Close = dto.ClosePrice,
                                              CompanyLongName = instrumentReference.CompanyLongName,
                                              CompanyShortName = instrumentReference.CompanyShortName,
                                              Exchange = instrumentReference.Exchange,
                                              High = dto.HighPrice,
                                              Low = dto.LowPrice,
                                              Open = dto.OpenPrice,
                                              Symbol = instrumentReference.Symbol,
                                              SymbolCode = instrumentReference.SymbolCode,
                                              TimeOfClose = new DateTime(dto.TimeOfCloseAsTicks),
                                              TimeOfStart = new DateTime(dto.TimeOfStartAsTicks),
                                              Volume = dto.TotalVolume,
                                              CloseSequenceNumber = dto.CloseSequenceNumber
                                          };
                result.Add(snapshot);
            }

            return result;
        }

        public static List<Candlestick> GetEquityCandlesticksFromDtos(Fin24.LiveData.Common.DTOs.EquityCandlestickCollection collection, InstrumentReference instrumentReference)
        {
            List<Candlestick> result = new List<Candlestick>();

            foreach (var dto in collection.Candlesticks)
            {
                Candlestick snapshot = new Candlestick
                {
                    Close = dto.ClosePrice,
                    CompanyLongName = instrumentReference.CompanyLongName,
                    CompanyShortName = instrumentReference.CompanyShortName,
                    Exchange = instrumentReference.Exchange,
                    High = dto.HighPrice,
                    Low = dto.LowPrice,
                    Open = dto.OpenPrice,
                    Symbol = instrumentReference.Symbol,
                    SymbolCode = instrumentReference.SymbolCode,
                    TimeOfClose = new DateTime(dto.TimeOfCloseAsTicks),
                    TimeOfStart = new DateTime(dto.TimeOfStartAsTicks),
                    Volume = dto.TotalVolume,
                    CloseSequenceNumber = dto.CloseSequenceNumber
                };
                result.Add(snapshot);
            }

            return result;
        }

        public static List<Candlestick> GetIndexCandlesticksFromDto(IndexCandlestick[] candlestickDtos, Index indexRef)
        {
            List<Candlestick> result = new List<Candlestick>();

            foreach (IndexCandlestick dto in candlestickDtos)
            {
                Candlestick snapshot = new Candlestick
                                              {
                                                  Close = dto.ClosePrice,
                                                  CompanyLongName = indexRef.CompanyLongName,
                                                  CompanyShortName = indexRef.CompanyShortName,
                                                  Exchange = indexRef.Exchange,
                                                  High = dto.HighPrice,
                                                  Low = dto.LowPrice,
                                                  Open = dto.OpenPrice,
                                                  Symbol = indexRef.Symbol,
                                                  SymbolCode = indexRef.SymbolCode,
                                                  TimeOfClose = new DateTime(dto.TimeOfCloseAsTicks),
                                                  TimeOfStart = new DateTime(dto.TimeOfStartAsTicks),
                                                  CloseSequenceNumber = dto.CloseSequenceNumber
                                              };
                result.Add(snapshot);
            }

            return result;
        }

        public static List<Candlestick> GetIndexCandlesticksFromDto(Fin24.LiveData.Common.DTOs.IndexCandlestickCollection collection, Index indexRef)
        {
            List<Candlestick> result = new List<Candlestick>();

            foreach (var dto in collection.Candlesticks)
            {
                Candlestick snapshot = new Candlestick
                {
                    Close = dto.ClosePrice,
                    CompanyLongName = indexRef.CompanyLongName,
                    CompanyShortName = indexRef.CompanyShortName,
                    Exchange = indexRef.Exchange,
                    High = dto.HighPrice,
                    Low = dto.LowPrice,
                    Open = dto.OpenPrice,
                    Symbol = indexRef.Symbol,
                    SymbolCode = indexRef.SymbolCode,
                    TimeOfClose = new DateTime(dto.TimeOfCloseAsTicks),
                    TimeOfStart = new DateTime(dto.TimeOfStartAsTicks),
                    CloseSequenceNumber = dto.CloseSequenceNumber
                };
                result.Add(snapshot);
            }

            return result;
        }

        public static List<Instrument> GetInstrumentsFromDtos(EquityDto[] equityDto)
        {
            List<Instrument> instruments = new List<Instrument>();

            foreach (EquityDto dto in equityDto)
            {
                Instrument instrument = new Instrument
                                            {
                                                Exchange = dto.ExchangeCode,
                                                ISIN = dto.ISIN,
                                                Symbol = dto.Symbol,
                                            };
                instruments.Add(instrument);
            }

            return instruments;
        }

        public static List<Instrument> GetInstrumentsFromDtos(Fin24.LiveData.Common.DTOs.EquityDto[] equityDto)
        {
            List<Instrument> instruments = new List<Instrument>();

            foreach (var dto in equityDto)
            {
                Instrument instrument = new Instrument
                {
                    Exchange = dto.ExchangeCode,
                    ISIN = dto.ISIN,
                    Symbol = dto.Symbol,
                };
                instruments.Add(instrument);
            }

            return instruments;
        }

        public static List<Instrument> GetInstrumentsFromDtos(Fin24.LiveData.Common.DTOs.EquityDetailsDto[] equityDetailsDtos)
        {
            List<Instrument> instruments = new List<Instrument>();

            foreach (var dto in equityDetailsDtos)
            {
                Instrument instrument = new Instrument();

                instrument.BestBid = dto.BestBidPrice;
                instrument.BestOffer = dto.BestOfferPrice;
                instrument.BidVolume = dto.VolumeOrdersAtBestBidPrice;
                instrument.OfferVolume = dto.VolumeOrdersAtBestOfferPrice;
                instrument.Open = dto.Open;

                instrument.TotalTrades = 0;
                instrument.TotalValue = dto.DailyValue;
                instrument.TotalVolume = dto.DailyVolume;
                instrument.YesterdayClose = dto.Open;
                instrument.High = dto.High;
                instrument.ISIN = dto.ISIN;
                instrument.LastTrade = dto.LastTrade_TradePrice;
                instrument.LastTradeSequenceNumber = dto.LastTrade_SequenceNumber;
                instrument.Low = dto.Low;
                instrument.CentsMoved = dto.CentsMoved;

                instrument.Symbol = dto.Symbol;
                instrument.Exchange = dto.Exchange;
                instrument.ISIN = dto.ISIN;

                if (instrument.YesterdayClose > 0)
                    instrument.PercentageMoved = (instrument.CentsMoved * 100) / instrument.YesterdayClose;

                instruments.Add(instrument);
            }

            return instruments;
        }

        public static Quote GetQuoteFromDto(QuoteDto dto)
        {
            Quote quote = new Quote();

            quote.BestAskPrice = dto.BestOfferPrice / 100000000;
            quote.BestAskSize = dto.VolumeOfOrdersAtBestOfferPrice;
            quote.BestBidPrice = dto.BestBidPrice / 100000000;
            quote.BestBidSize = dto.VolumeOfOrdersAtBestBidPrice;
            quote.Exchange = "JSE";
            quote.Symbol = dto.Symbol;
            quote.SymbolCode = dto.Symbol;

            return quote;
        }

        public static DateTime ParseTime(string time)
        {
            int hour = Convert.ToInt32(time.Substring(0, 2));
            int minute = Convert.ToInt32(time.Substring(2, 2));
            int second = Convert.ToInt32(time.Substring(4, 2));

            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, second);
        }

        internal static List<Candlestick> GetCandlesticksFromDtos(CandlestickCollectionDto collection, InstrumentReference instrumentReference)
        {
            List<Candlestick> result = new List<Candlestick>();

            foreach (var dto in collection.Candlesticks)
            {
                Candlestick snapshot = new Candlestick
                {
                    Close = dto.ClosePrice,
                    CompanyLongName = instrumentReference.CompanyLongName,
                    CompanyShortName = instrumentReference.CompanyShortName,
                    Exchange = instrumentReference.Exchange,
                    High = dto.HighPrice,
                    Low = dto.LowPrice,
                    Open = dto.OpenPrice,
                    Symbol = instrumentReference.Symbol,
                    SymbolCode = instrumentReference.SymbolCode,
                    TimeOfClose = DateTime.Parse(dto.TimeOfClose),
                    TimeOfStart = DateTime.Parse(dto.TimeOfStart),
                    Volume = dto.TotalVolume,
                    CloseSequenceNumber = dto.CloseSequenceNumber
                };
                result.Add(snapshot);
            }

            return result;
        }

        public static List<Candlestick> GetCandlesticksFromDtos(CandlestickCollectionDto collection, Index allIndicesReference)
        {
            List<Candlestick> result = new List<Candlestick>();

            foreach (var dto in collection.Candlesticks)
            {
                Candlestick snapshot = new Candlestick
                {
                    Close = dto.ClosePrice,
                    CompanyLongName = allIndicesReference.CompanyLongName,
                    CompanyShortName = allIndicesReference.CompanyShortName,
                    Exchange = allIndicesReference.Exchange,
                    High = dto.HighPrice,
                    Low = dto.LowPrice,
                    Open = dto.OpenPrice,
                    Symbol = allIndicesReference.Symbol,
                    SymbolCode = allIndicesReference.SymbolCode,
                    TimeOfClose = DateTime.Parse(dto.TimeOfClose),
                    TimeOfStart = DateTime.Parse(dto.TimeOfStart),
                    Volume = dto.TotalVolume,
                    CloseSequenceNumber = dto.CloseSequenceNumber
                };
                result.Add(snapshot);
            }

            return result;
        }
    }
}
