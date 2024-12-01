using System;
using System.Collections.Generic;
using FundXchange.DataProviderContracts;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using FundXchange.NutcrackerAPI.JSEPublic;
using FundXchange.Domain.Enumerations;
using Nutcracker.FeederAPI;
using Nutcracker.CMS.Interapp.Client;
using System.Configuration;

namespace FundXchange.NutcrackerAPI
{
    public class RealtimeDataProvider : IRealTimeDataProvider
    {
        private string _Username = "";
        private Host _Host;

        public event InstrumentUpdatedDelegate InstrumentUpdatedEvent;
        public event IndexUpdatedDelegate IndexUpdatedEvent;
        public event TradeOccurredDelegate TradeOccurredEvent;
        public event OrderAddedDelegate OrderAddedEvent;
        public event OrderUpdatedDelegate OrderUpdatedEvent;
        public event OrderDeletedDelegate OrderDeletedEvent;
        public event QuoteOccurredDelegate QuoteOccurredEvent;
        public event ErrorOccurredDelegate ErrorOccurredEvent;

        public RealtimeDataProvider(string username)
        {
            _Username = username;
            _Host = new Host();

            _Host.DoInstrumentDepthUpdated += new InstrumentDepthUpdated(_Host_DoInstrumentDepthUpdated);
            _Host.DoInstrumentUpdated += new InstrumentUpdated(_Host_DoInstrumentUpdated);
            _Host.DoSmokeSignalsConnected += new ConnectionChanged(_Host_DoSmokeSignalsConnected);
            _Host.DoSmokeSignalsDisconnected += new ConnectionChanged(_Host_DoSmokeSignalsDisconnected);
        }

        void _Host_DoSmokeSignalsDisconnected(object Sender, EventArgs e)
        {
            
        }

        void _Host_DoSmokeSignalsConnected(object Sender, EventArgs e)
        {
            
        }

        void _Host_DoInstrumentUpdated(object Sender, InstrumentData Instrument)
        {
            Instrument instrument = DataAdapter.GetInstrumentFromInstrumentData(Instrument);
            if (InstrumentUpdatedEvent != null)
            {
                InstrumentUpdatedEvent(instrument);
            }
        }

        void _Host_DoInstrumentDepthUpdated(object Sender, Depth InstrumentDepth)
        {
            
        }

        public Instrument SubscribeLevelOneWatch(string symbol, string exchange)
        {
            Instrument instrument = new Instrument();

            using (JSEPublic.JSEPublicClient proxy = new FundXchange.NutcrackerAPI.JSEPublic.JSEPublicClient())
            {
                WatchlistShareRow[] result = proxy.GetMultipleShareData(_Username, new string[] { symbol }, true);
                instrument = DataAdapter.GetInstrumentsFromWatchlistShareRow(result)[0];
            }
            _Host.Subscribe(symbol);

            return instrument;
        }

        public void UnsubscribeLevelOneWatch(string symbol, string exchange)
        {
            _Host.UnSubscribe(symbol);
        }

        public void SubscribeIndexWatch(string symbol, string exchange)
        {

        }

        public void UnsubscribeIndexWatch(string symbol, string exchange)
        {

        }

        public bool SubscribeLevelTwoWatch(string symbol, string exchange, out List<Bid> bids, out List<Offer> offers, out List<Order> orders)
        {
            bids = new List<Bid>();
            offers = new List<Offer>();
            orders = new List<Order>();

            _Host.SubscribeDepth(symbol);

            return true;
        }

        public bool UnsubscribeLevelTwoWatch(string symbol, string exchange)
        {
            _Host.UnSubscribeDepth(symbol);
            return true;
        }

        public string VendorName
        {
            get { return "Nutcracker"; }
        }

        public void Connect()
        {
            string errorString = "";
            _Host.NTVoyagerUrl = ConfigurationManager.AppSettings["PubSubURL"];
            _Host.Login(_Username, "Nut123", ref errorString);
        }

        public void Disconnect()
        {

        }

        public List<InstrumentSnapshot> GetInstrumentSnapshotHistory(string symbol, string exchange, int interval, int barsCount)
        {
            return new List<InstrumentSnapshot>();
        }

        public List<InstrumentSnapshot> GetIndexSnapshotHistory(string index, string exchange, int interval, int barsCount)
        {
            return new List<InstrumentSnapshot>();
        }



        public List<Instrument> GetTop20InstrumentsByValue(string exchange)
        {
            List<Instrument> instruments = new List<Instrument>();
            using (JSEPublic.JSEPublicClient proxy = new FundXchange.NutcrackerAPI.JSEPublic.JSEPublicClient())
            {
                WatchlistShareRow[] result = proxy.MarketMoversValue(_Username, true, false, 20);
                instruments = DataAdapter.GetInstrumentsFromWatchlistShareRow(result);
            }
            return instruments;
        }

        public List<Instrument> GetTop20InstrumentsByVolume(string exchange)
        {
            List<Instrument> instruments = new List<Instrument>();
            using (JSEPublic.JSEPublicClient proxy = new FundXchange.NutcrackerAPI.JSEPublic.JSEPublicClient())
            {
                WatchlistShareRow[] result = proxy.MarketMoversVolume(_Username, true, false, 20);
                instruments = DataAdapter.GetInstrumentsFromWatchlistShareRow(result);
            }
            return instruments;
        }

        public List<Instrument> GetWinners(string exchange)
        {
            List<Instrument> instruments = new List<Instrument>();
            using (JSEPublic.JSEPublicClient proxy = new FundXchange.NutcrackerAPI.JSEPublic.JSEPublicClient())
            {
                WatchlistShareRow[] result = proxy.MarketMoversUp(_Username, true, false, 20);
                instruments = DataAdapter.GetInstrumentsFromWatchlistShareRow(result);
            }
            return instruments;
        }

        public List<Instrument> GetLosers(string exchange)
        {
            List<Instrument> instruments = new List<Instrument>();
            using (JSEPublic.JSEPublicClient proxy = new FundXchange.NutcrackerAPI.JSEPublic.JSEPublicClient())
            {
                WatchlistShareRow[] result = proxy.MarketMoversDown(_Username, true, false, 20);
                instruments = DataAdapter.GetInstrumentsFromWatchlistShareRow(result);
            }
            return instruments;
        }

        public List<Index> GetIndexes(string exchange)
        {
            List<Index> indexes = new List<Index>();
            using (JSEPublic.JSEPublicClient proxy = new FundXchange.NutcrackerAPI.JSEPublic.JSEPublicClient())
            {
                IndexValue[] result = proxy.GetIndexValues(_Username, true);
                indexes = DataAdapter.GetIndexesFromIndexValues(result);
            }
            return indexes;
        }

        public List<Instrument> GetTop40Instruments(string exchange)
        {
            return new List<Instrument>();
        }

        public List<InstrumentStatistics> GetMarketsHomeData(string exchange)
        {
            return new List<InstrumentStatistics>();
        }

        public List<Sens> GetSensData()
        {
            List<Sens> sens = new List<Sens>();
            using (JSEPublic.JSEPublicClient proxy = new FundXchange.NutcrackerAPI.JSEPublic.JSEPublicClient())
            {
                NewsMessage[] result = proxy.GetSensNewsMessages(_Username, true);
                sens = DataAdapter.GetSensFromNewsMessages(result);
            }
            return sens;
        }

        public string GetSensBody(string id)
        {
            using (JSEPublic.JSEPublicClient proxy = new FundXchange.NutcrackerAPI.JSEPublic.JSEPublicClient())
            {
                NewsBody result = proxy.GetSensNewsBody(_Username, id, true);
                return result.MessageBody;
            }
        }

        public List<InstrumentReference> GetInstrumentReferencesForType(InstrumentType type)
        {
            return new List<InstrumentReference>();
        }

        public List<InstrumentReference> GetInstrumentReferenceForAllCurrentInstruments()
        {
            return new List<InstrumentReference>();
        }

        public List<Trade> GetTradesForInstrument(string symbol)
        {
            List<Trade> trades = new List<Trade>();
            using (JSEPublic.JSEPublicClient proxy = new FundXchange.NutcrackerAPI.JSEPublic.JSEPublicClient())
            {
                PublicTrade[] result = proxy.GetPublicTradesForSymbol(_Username, symbol, true, 100);
                trades = DataAdapter.GetTradesFromPublicTrades(symbol, result);
            }
            return trades;
        }

        public List<Trade> GetTrades(string exchange, string symbol, int pageSize)
        {
            List<Trade> trades = new List<Trade>();
            using (JSEPublic.JSEPublicClient proxy = new FundXchange.NutcrackerAPI.JSEPublic.JSEPublicClient())
            {
                PublicTrade[] result = proxy.GetPublicTradesForSymbol(_Username, symbol, true, pageSize);
                trades = DataAdapter.GetTradesFromPublicTrades(symbol, result);
            }
            return trades;
        }

        public List<Quote> GetQuotes(string exchange, string symbol, DateTime fromDate)
        {
            return new List<Quote>();
        }
    }
}
