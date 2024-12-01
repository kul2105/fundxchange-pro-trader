using System;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using System.Collections.Generic;
using FundXchange.Domain.Enumerations;
//using MQTTAPI;
//using JsonLibCommon;
//using FundXchange.Data.McGregorBFA;

namespace FundXchange.DataProviderContracts
{
    public delegate void InstrumentUpdatedDelegate(Instrument instrument);
    public delegate void IndexUpdatedDelegate(Index index);
    public delegate void TradeOccurredDelegate(Trade trade);
    public delegate void OrderAddedDelegate(OrderbookItem order);
    public delegate void OrderbookInitializedDelegate(List<OrderbookItem> orders);
    public delegate void OrderDeletedDelegate(string orderCode);
    public delegate void QuoteOccurredDelegate(Quote quote);
    public delegate void ErrorOccurredDelegate(string error);
    public delegate void DataProviderConnectionChangedDelegate(bool isConnected);
    public delegate void Level2SymbolChangedDelegate(string symbol);
    public delegate void HeartbeatMessageReceivedDelegate(DateTime servertime);
    public delegate void InstrumentsChangedDelegate(List<Instrument> instruments);
    public delegate void ChangedDelegate();
    public delegate void SensNewsDelegate(MQTTAPI.FullSensNews sensNews);
    public interface IRealTimeDataProvider
    {
        Instrument SubscribeLevelOneWatch(string symbol, string exchange);
        Instrument SubscribeIndex(string indexIndentifier, string exchange);
        List<Instrument> SubscribeLevelOneWatches(List<string> symbols, string exchange);
        void UnsubscribeLevelOneWatch(string symbol, string exchange);
        void UnsubscribeLevelOneWatches(List<string> symbols, string exchange);
        
        void SubscribeIndexWatch(string symbol, string exchange);
        void UnsubscribeIndexWatch(string symbol, string exchange);

        bool SubscribeLevelTwoWatch(string symbol, string exchange);
        bool UnsubscribeLevelTwoWatch(string symbol, string exchange);

        string VendorId { get; }

        List<Candlestick> GetEquityCandlesticks(string symbol, string exchange, int interval, int barsCount, PeriodEnum period);
        List<Candlestick> GetIndexCandlesticks(string index, string exchange, int interval, int barsCount, PeriodEnum period);

        event InstrumentUpdatedDelegate InstrumentUpdatedEvent;
        event IndexUpdatedDelegate IndexUpdatedEvent;
        event TradeOccurredDelegate TradeOccurredEvent;
        event OrderAddedDelegate OrderAddedEvent;
        event OrderbookInitializedDelegate OrderbookInitializedEvent;
        event OrderDeletedDelegate OrderDeletedEvent;
        event QuoteOccurredDelegate QuoteOccurredEvent;
        event ErrorOccurredDelegate ErrorOccurredEvent;
        event DataProviderConnectionChangedDelegate DataProviderConnectionChangedEvent;
        event HeartbeatMessageReceivedDelegate HeartbeatReceivedEvent;
        event SensNewsDelegate OnSensNewsEvent;

        List<Instrument> GetTop20InstrumentsByValue(string exchange);
        List<Instrument> GetTop20InstrumentsByVolume(string exchange);
        List<Instrument> GetWinners(string exchange);
        List<Instrument> GetLosers(string exchange);
        List<Index> GetIndices(string exchange);
        List<Index> GetIndicesStatistics(string exchange);
        List<Instrument> GetTop40Instruments(string exchange);
        List<InstrumentStatistics> GetMarketsHomeData(string exchange);
        List<Sens> GetSensData();
        string GetSensBody(string id);
        List<InstrumentReference> GetInstrumentReferencesForType(InstrumentType type);
        List<InstrumentReference> GetInstrumentReferenceForAllCurrentInstruments();
        List<Index> GetIndexReferenceForAllCurrentIndeces();
        List<Trade> GetTrades(string exchange, string symbol, int pageSize);
        List<Quote> GetQuotes(string exchange, string symbol, DateTime fromDate);

        void SetSymbolDictionary(Dictionary<string, InstrumentReference> AllInstrumentReferences);
        List<JsonLibCommon.FinSwitchHD> GetEODWithDates(string symbol, DateTime startdt, DateTime endDT, string PriceType);
        List<JsonLibCommon.FinSwitchFundsDetails> GetFinSwitchMFList();
        List<string> GetFundTypes();
        List<JsonLibCommon.Sector> GetSectors(string FType);
        List<JsonLibCommon.CISFunds> GetCISFunds(string Ftype, string SectorCode);
    }
}
