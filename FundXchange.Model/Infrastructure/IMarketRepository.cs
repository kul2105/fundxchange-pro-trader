using System.Collections.Generic;
using FundXchange.Domain.Entities;
using FundXchange.DataProviderContracts;
using FundXchange.Domain.Enumerations;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.Agents;
using System;
//using JsonLibCommon;
//using JSONdataLib;

namespace FundXchange.Model.Infrastructure
{
    public interface IMarketRepository
    {
        event InstrumentUpdatedDelegate InstrumentUpdatedEvent;
        event TradeOccurredDelegate TradeOccurredEvent;
        event IndexUpdatedDelegate IndexUpdatedEvent;
        event ErrorOccurredDelegate ErrorOccurred;
        event Level2SymbolChangedDelegate Level2SymbolChanged;
        event HeartbeatMessageReceivedDelegate HeartbeatReceivedEvent;        
        event SensNewsDelegate OnSensNewsEvent;

        List<InstrumentReference> AllInstrumentReferences { get; }
        List<Index> AllIndexReferences { get; }
        Dictionary<string, Instrument> PortfolioWatchList { get; }
        Dictionary<string, Index> IndexWatchList { get; }
        List<Trade> GetTrades(string exchange, string symbol, int pageSize);
        List<InstrumentReference> GetInstrumentReferencesForType(InstrumentType type);
        
        void GetIndices();
        Instrument AddInstrumentToPortfolioWatch(string symbol, string exchange);
        
        string GetIndexSymbol(string indexName);
        IOrderbook Orderbook { get; }
        List<string> LevelTwoSymbol { get; }
        List<Instrument> LevelTwoInstrument { get; }
        Instrument GetInstrument(string symbol, string exchange);

        Instrument SubscribeLevelOneWatch(string symbol, string exchange);
        Instrument SubscribeIndex(string symbol, string exchange);
        Instrument SubscribeLevelOneWatch(string symbol, string exchange, bool forceGet);
        void SubscribeLevelOneWatches(List<string> symbols, string exchange);
        bool SubscribeLevelTwoWatch(string symbol, string exchange);        

        void UnsubscribeLevelTwoWatch(string symbol, string exchange);
        void UnsubscribeLevelOneWatch(string symbol, string exchange);
        void UnsubscribeLevelOneWatches(List<string> symbolsEquities, string exchangeCode);
        void UnsubscribeIndexWatch(string symbol, string exchange);        
        void UnsubscribeIndexWatches(string exchange);
        
        List<Candlestick> GetEquityCandlesticks(string symbol, string exchange, int interval, int barsCount, PeriodEnum period);        
        IEnumerable<Candlestick> GetIndexCandlesticks(string symbol, string exchange, int interval, int numberOfCandles, PeriodEnum period);

        List<Sens> GetSensData();
        List<Instrument> GetWinners(string exchange);
        List<Instrument> GetLosers(string exchange);
        List<Instrument> GetLeadersByValue(string exchange);
        List<Instrument> GetLeadersByVolume(string exchange);
        List<Instrument> GetTop40Instruments(string exchange);
        List<InstrumentStatistics> GetMarketsHomeData();
        
        void SetSymbolDictionary(Dictionary<string, InstrumentReference> AllInstrumentReferences);

        List<JsonLibCommon.FinSwitchHD> GetEODWithDates(string symbol, DateTime startdt, DateTime endDT, string PriceType);
        List<JsonLibCommon.FinSwitchFundsDetails> AllFinSwitchMFList { get; }
        List<string> GetFundTypes();
        List<JsonLibCommon.Sector> GetSectors(string FType);
        List<JsonLibCommon.CISFunds> GetCISFunds(string Ftype, string SectorCode);
    }
}
