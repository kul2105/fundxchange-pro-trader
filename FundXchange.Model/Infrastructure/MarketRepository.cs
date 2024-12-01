using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using FundXchange.Data.McGregorBFA;
using FundXchange.Domain.Entities;
using FundXchange.DataProviderContracts;
using FundXchange.Domain.ValueObjects;
using FundXchange.Infrastructure;
using FundXchange.Domain.Enumerations;
using FundXchange.Model.AuthenticationService;
using FundXchange.Model.Exceptions;
using FundXchange.Model.Observer;
using FundXchange.Model.Agents;
using FundXchange.Orders;
using FundXchange.Orders.Agents;
using System.Text;
//using JsonLibCommon;


namespace FundXchange.Model.Infrastructure
{
    public class MarketRepository : IConnectionObserver, IMarketRepository
    {
        private readonly IRealTimeDataProvider _RealTimeDataProvider;
        private readonly ErrorService _ErrorService;
        private readonly UserDTO _User;

        public IOrderbook Orderbook { get; private set; }

        private readonly Dictionary<string, Instrument> _PortfolioWatchList;
        private readonly Dictionary<string, Instrument> _AllWatchList;
        private List<InstrumentReference> _allInstrumentReferences = new List<InstrumentReference>();
        public Dictionary<string, Index> IndexWatchList { get; private set; }
        private readonly List<Index> _allIndexReferences = new List<Index>();
        private readonly GlobalIndicatorRepository _globalIndicatorRepository;
        private List<JsonLibCommon.FinSwitchFundsDetails> _allFinSwitchMFList = new List<JsonLibCommon.FinSwitchFundsDetails>();

        public List<Index> AllIndexReferences
        {
            get
            {
                if (_allIndexReferences.Count == 0)
                {
                    lock (m_syncLockIndex)
                    {
                        _allIndexReferences.AddRange(
                            _RealTimeDataProvider.GetIndexReferenceForAllCurrentIndeces());
                    }
                }

                return _allIndexReferences;
            }
        }
        public List<InstrumentReference> AllInstrumentReferences
        {
            get
            {
                lock (m_syncLock)
                {
                    _allInstrumentReferences = _RealTimeDataProvider.GetInstrumentReferenceForAllCurrentInstruments();
                }

                //Commented By John
                //if (_allInstrumentReferences.Count == 0)
                //{
                    //lock (m_syncLock)
                    //{
                    //    _allInstrumentReferences.AddRange(
                    //        _RealTimeDataProvider.GetInstrumentReferenceForAllCurrentInstruments());
                    //}
                //}

                return _allInstrumentReferences;
            }
        }
        public List<Order> Orders { get; set; }
        private Thread _IndexThread;
        private readonly object m_syncLock = new object();
        private readonly object m_syncLockIndex = new object();
        public event InstrumentsChangedDelegate PortfolioWatchListChangedEvent;
        public event InstrumentUpdatedDelegate InstrumentUpdatedEvent;
        public event TradeOccurredDelegate TradeOccurredEvent;
        public event IndexUpdatedDelegate IndexUpdatedEvent;
        public event QuoteOccurredDelegate QuoteOccurredEvent;
        public event ErrorOccurredDelegate ErrorOccurred;
        public event Level2SymbolChangedDelegate Level2SymbolChanged;
        public event HeartbeatMessageReceivedDelegate HeartbeatReceivedEvent;      
        public event SensNewsDelegate OnSensNewsEvent;

        public List<string> LevelTwoSymbol { get; private set; }
        public List<Instrument> LevelTwoInstrument { get; private set; }
        private readonly StopLossOrderAgent _stopLossOrderAgent;
        //public event Action<ddfplus.BookQuote> OnLevel2BarChart;

        public Dictionary<string, Instrument> PortfolioWatchList
        {
            get
            {
                return _PortfolioWatchList;
            }
        }
     
        public MarketRepository(UserDTO user)
        {
            _User = user;
            bool isRestricted = (_User.Account.Product == Product.TraderLite || _User.Account.Product == Product.QuoteBoardLite);
            _RealTimeDataProvider = new RealtimeDataProvider(
                        _User.Account.Username,
                        _User.Account.Password,
                        _User.Account.Product.ToString(),
                        isRestricted);

            ListenToEvents();

            _ErrorService = IoC.Resolve<ErrorService>();

            _PortfolioWatchList = new Dictionary<string, Instrument>();
            _AllWatchList = new Dictionary<string, Instrument>();
            IndexWatchList = new Dictionary<string, Index>();

            LevelTwoSymbol = new List<string>();
            LevelTwoInstrument = new List<Instrument>();
            _globalIndicatorRepository = new GlobalIndicatorRepository(_ErrorService);

            if (_User.Account.Product == Product.TraderLite || _User.Account.Product == Product.TraderPro)
            {
                //OrderService orderService = new OrderService(_User.Account.UserId);
                //IoC.RegisterInstance(orderService);
                _stopLossOrderAgent = new StopLossOrderAgent();
                _stopLossOrderAgent.StopLossOrdersToSubscribeEvent += StopLossOrderAgentStopLossOrdersToSubscribeEvent;

                _stopLossOrderAgent.Start();
            }
           
        }

        void StopLossOrderAgentStopLossOrdersToSubscribeEvent(string exchange, string symbol)
        {
            SubscribeLevelOneWatch(symbol, exchange);
        }

        public void ConnectionChanged(bool connected)
        {
            if (connected)
            {
                ListenToEvents();

                foreach (Instrument instrument in _PortfolioWatchList.Values)
                {
                    if (instrument != null)
                        AddInstrumentToPortfolioWatch(instrument.Symbol, instrument.Exchange);
                }
                //if (LevelTwoInstrument != null && !String.IsNullOrEmpty(LevelTwoInstrument.Symbol))
                //    SubscribeLevelTwoWatch(LevelTwoInstrument.Symbol, LevelTwoInstrument.Exchange);
                if (LevelTwoInstrument != null)
                {
                    foreach (var item in LevelTwoInstrument)
                    {
                        SubscribeLevelTwoWatch(item.Symbol, item.Exchange);
                    }
                }
            }
        }

        #region Watch List Methods

        public Instrument AddInstrumentToPortfolioWatch(string symbol, string exchange)
        {
            if (symbol == null) return new Instrument();

            if (_PortfolioWatchList.ContainsKey(symbol))
            {
                return _PortfolioWatchList[symbol];
            }

            if (_AllWatchList.ContainsKey(symbol))
            {
                if (!_PortfolioWatchList.ContainsKey(symbol))
                {
                    _PortfolioWatchList.Add(symbol, _AllWatchList[symbol]);
                    if (null != PortfolioWatchListChangedEvent)
                        PortfolioWatchListChangedEvent(_PortfolioWatchList.Values.ToList());
                }
                return _AllWatchList[symbol];
            }

            Instrument instrument = SubscribeLevelOneWatch(symbol, exchange, false);
            TryAddToWatchList(symbol, instrument, _PortfolioWatchList);

            return instrument;
        }

        public List<InstrumentReference> GetInstrumentReferencesForType(InstrumentType type)
        {
            return _RealTimeDataProvider.GetInstrumentReferencesForType(type);
        }
        
        #endregion

        public string GetIndexSymbol(string shortName)
        {
            Index index = IndexWatchList.Values.FirstOrDefault(i => i.ShortName == shortName);
            if (index == null)
                return String.Empty;
            return index.Symbol;
        }

        public string GetSensBody(string id)
        {
            return _RealTimeDataProvider.GetSensBody(id);
        }

        public List<Instrument> GetWinners(string exchange)
        {
            return _RealTimeDataProvider.GetWinners(exchange);
        }

        public List<Instrument> GetLosers(string exchange)
        {
            return _RealTimeDataProvider.GetLosers(exchange);
        }

        public List<Instrument> GetLeadersByValue(string exchange)
        {
            return _RealTimeDataProvider.GetTop20InstrumentsByValue(exchange);
        }

        public List<Instrument> GetLeadersByVolume(string exchange)
        {
            return _RealTimeDataProvider.GetTop20InstrumentsByVolume(exchange);
        }

        public List<Instrument> GetTop40Instruments(string exchange)
        {
            return _RealTimeDataProvider.GetTop40Instruments(exchange);
        }

        public Instrument GetInstrument(string symbol, string exchange)
        {
            if (_AllWatchList.ContainsKey(symbol))
                return _AllWatchList[symbol];
            return null;
        }

        #region Subscribe Methods

        public Instrument SubscribeLevelOneWatch(string symbol, string exchange)
        {
            return SubscribeLevelOneWatch(symbol, exchange, false);
        }

        public Instrument SubscribeLevelOneWatch(string symbol, string exchange, bool forceGet)
        {
            if (String.IsNullOrEmpty(symbol)) return null;

            symbol = symbol.ToUpper().Trim();
            ValidateSymbolAndExchange(symbol, exchange);
            Instrument instrument = new Instrument();//null;

            if (_AllWatchList.ContainsKey(symbol) && !forceGet)
            {
                instrument = _AllWatchList[symbol];
                instrument.Symbol = symbol;
            }
            else
            {
                instrument = TrySubscribeLevelOneWatch(symbol, exchange);
                TryAddToWatchList(symbol, instrument, _AllWatchList);
            }

            return instrument;
        }

        public Instrument SubscribeIndex(string indexIdentfier, string exchange)
        {
            if(string.IsNullOrEmpty(indexIdentfier)) return null;

            if (_PortfolioWatchList.ContainsKey(indexIdentfier))
            {
                return _PortfolioWatchList[indexIdentfier];
            }

            if (_AllWatchList.ContainsKey(indexIdentfier))
            {
                if (!_PortfolioWatchList.ContainsKey(indexIdentfier))
                    _PortfolioWatchList.Add(indexIdentfier, _AllWatchList[indexIdentfier]);
                return _AllWatchList[indexIdentfier];
            }

            Instrument instrument = _RealTimeDataProvider.SubscribeIndex(indexIdentfier, exchange);
            TryAddToWatchList(indexIdentfier, instrument, _AllWatchList);            
            TryAddToWatchList(indexIdentfier, instrument, _PortfolioWatchList);

            return instrument;
        }

        public void SubscribeLevelOneWatches(List<string> symbols, string exchange)
        {
            List<string> symbolsToSubscribe = new List<string>();
            foreach (string symbol in symbols)
            {
                if (!String.IsNullOrEmpty(symbol))
                {
                    string symbolToSubscribe = symbol.ToUpper();

                    if (_AllWatchList.ContainsKey(symbolToSubscribe))
                    {
                        TryRaiseInstrumentUpdated(_AllWatchList[symbolToSubscribe]);
                    }
                    else
                    {
                        symbolsToSubscribe.Add(symbolToSubscribe);
                    }
                }
            }
            if (symbolsToSubscribe.Count > 0)
            {
                List<Instrument> instruments = TrySubscribeLevelOneWatches(symbolsToSubscribe, exchange);
                if (instruments != null)
                {
                    foreach (Instrument instrument in instruments)
                    {
                        TryAddToWatchList(instrument.Symbol, instrument, _AllWatchList);
                        TryRaiseInstrumentUpdated(instrument);
                    }
                }
            }
        }

        public void UnsubscribeLevelOneWatch(string symbol, string exchange)
        {
            try
            {
                ValidateSymbolAndExchange(symbol, exchange);
                TryUnsubscribeLevelOneWatch(symbol, exchange);

                if (_PortfolioWatchList.ContainsKey(symbol))
                {
                    _PortfolioWatchList.Remove(symbol);

                    if (null != PortfolioWatchListChangedEvent)
                        PortfolioWatchListChangedEvent(_PortfolioWatchList.Values.ToList());
                }
                if (_AllWatchList.ContainsKey(symbol))
                {
                    _AllWatchList.Remove(symbol);
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
            }
        }

        public void SubscribeIndexWatch(string symbol, string shortName, string exchange)
        {
            if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(exchange))
                return;

            TrySubscribeIndexWatch(symbol, exchange);

            if (!IndexWatchList.ContainsKey(symbol))
            {
                Index index = new Index();
                index.Symbol = symbol;
                index.ShortName = shortName.Trim();
                index.Exchange = exchange;

                IndexWatchList.Add(symbol, index);
            }
        }

        public void UnsubscribeLevelOneWatches(List<string> symbols, string exchange)
        {
            List<string> symbolList = new List<string>();
            foreach (string symbol in symbols)
            {
                if (!_PortfolioWatchList.ContainsKey(symbol))
                {
                    _AllWatchList.Remove(symbol);
                    symbolList.Add(symbol);
                }

                _RealTimeDataProvider.UnsubscribeLevelOneWatch(symbol, exchange);
            }
           
            //Commented By John
            //_RealTimeDataProvider.UnsubscribeLevelOneWatches(symbolList, exchange);
        }

        public void SubscribeIndexWatches(List<string> symbols, List<string> shortNames, string exchange)
        {
            string symbolList = "";
            foreach (string symbol in symbols)
            {
                if (symbolList.Length > 0)
                    symbolList += ",";
                symbolList += symbol;
            }

            _RealTimeDataProvider.SubscribeIndexWatch(symbolList, exchange);

            for (int i = 0; i < symbols.Count; i++)
            {
                if (!IndexWatchList.ContainsKey(symbols[i]))
                {
                    Index index = new Index();
                    index.Symbol = symbols[i];
                    index.ShortName = shortNames[i].Trim();
                    index.Exchange = exchange;

                    IndexWatchList.Add(symbols[i], index);
                }
            } 
        }

        public void UnsubscribeIndexWatch(string symbol, string exchange)
        {
            ValidateSymbolAndExchange(symbol, exchange);
            TryUnsubscribeIndexWatch(symbol, exchange);
        }

        public void UnsubscribeIndexWatches(string exchange)
        {
            string symbolList = "";
            foreach (var key in IndexWatchList)
            {
                if (!_PortfolioWatchList.ContainsKey(key.Key))
                {
                    _AllWatchList.Remove(key.Key);
                    //IndexWatchList.Remove(key.Key);
                    if (symbolList.Length > 0)
                        symbolList += ",";
                    symbolList += key.Key;
                }
            }
            _RealTimeDataProvider.UnsubscribeIndexWatch(symbolList, exchange);
        }

        public bool SubscribeLevelTwoWatch(string symbol, string exchange)
        {
            ValidateSymbolAndExchange(symbol, exchange);
            if (IndexWatchList.ContainsKey(symbol))
            {
                MessageBox.Show("Level 2 Watch cannot be added for an Index.");
                return false;
            }
            //UnsubscribeLevelTwoIfSubscribed(symbol,exchange);
            //LevelTwoSymbol = symbol;
            bool ret = false;

            //Changed By John

            List<InstrumentReference> LstInst= _RealTimeDataProvider.GetInstrumentReferenceForAllCurrentInstruments();
            InstrumentReference instrumentReference = LstInst.Find(i => i.Symbol == symbol);
            //InstrumentReference instrumentReference = AllInstrumentReferences.Find(i => i.Symbol == symbol);
            if (instrumentReference != null)
            {
                bool isRestricted = (_User.Account.Product == Product.QuoteBoardLite || _User.Account.Product == Product.TraderLite);
                Orderbook = new Orderbook(instrumentReference.ISIN, symbol, exchange, isRestricted);
                ret = TrySubscribeLevelTwoWatch(symbol, exchange);

                if (Level2SymbolChanged != null)
                {
                    Level2SymbolChanged(symbol);
                }
            }
            return ret;
        }

        public void UnsubscribeLevelTwoWatch(string symbol, string exchange)
        {
            //ValidateSymbolAndExchange(symbol, exchange);
            TryUnsubscribeLevelTwoWatch(symbol, exchange);
        }

        #endregion

        #region Instrument Methods

        public Instrument GetInstrument(string symbol)
        {
            if (_PortfolioWatchList.ContainsKey(symbol))
            {
                return _PortfolioWatchList[symbol];
            }
            return null;
        }

        public List<InstrumentStatistics> GetMarketsHomeData()
        {
            List<InstrumentStatistics> instruments = new List<InstrumentStatistics>();

            List<InstrumentStatistics> currencies = _globalIndicatorRepository.GetCurrencies();
            List<InstrumentStatistics> intIndicies = _globalIndicatorRepository.GetIntIndicies();
            List<InstrumentStatistics> bonds = _globalIndicatorRepository.GetBonds();
            List<InstrumentStatistics> commodities = _globalIndicatorRepository.GetCommodities();

            instruments.AddRange(currencies);
            instruments.AddRange(intIndicies);
            instruments.AddRange(bonds);
            instruments.AddRange(commodities);

            return instruments;
        }

        public List<Sens> GetSensData()
        {
            return _RealTimeDataProvider.GetSensData();
        }

        public List<Candlestick> GetEquityCandlesticks(string symbol, string exchange, int interval, int barsCount, PeriodEnum period)
        {
            List<Candlestick> snapshots = _RealTimeDataProvider.GetEquityCandlesticks(symbol.Trim(), exchange, interval,
                                                                                      barsCount, period);
            return snapshots;
        }

        public IEnumerable<Candlestick> GetIndexCandlesticks(string index, string exchange, int interval, int barsCount, PeriodEnum period)
        {
            List<Candlestick> snapshots = _RealTimeDataProvider.GetIndexCandlesticks(index, exchange, interval,
                                                                                     barsCount, period);

            if (snapshots.Count < 3)
                throw new InsufficientDataException(index, exchange);

            return snapshots; 
        }

        public List<Trade> GetTrades(string exchange, string symbol, int pageSize)
        {
            return _RealTimeDataProvider.GetTrades(exchange, symbol, pageSize);
        }

        public List<Quote> GetQuotes(string exchange, string symbol, DateTime fromDate)
        {
            return _RealTimeDataProvider.GetQuotes(exchange, symbol, fromDate);
        }

        public void StopReceiving()
        {
        }

        #endregion

        private void TryRaiseInstrumentUpdated(Instrument instrument)
        {
            if (instrument.Symbol != null)
            {
                // check if an alternative symbol exisits for this stock
                if (AllInstrumentReferences.Find(s => s.AlternativeSymbol == instrument.Symbol) != null)
                    instrument.Symbol =
                        AllInstrumentReferences.Find(s => s.AlternativeSymbol == instrument.Symbol).Symbol;

                if (InstrumentUpdatedEvent != null)
                {
                    InstrumentUpdatedEvent(instrument);
                }

            }
        }

        #region Thread Methods

        private void SetupThreads()
        {
            _IndexThread = new Thread(GetIndexListing);
            _IndexThread.IsBackground = true;
            _IndexThread.Start();
        }

        private void GetIndexListing()
        {
            try
            {
                List<Index> indexes = _RealTimeDataProvider.GetIndices(GlobalDeclarations.EXCHANGE);

                indexes = indexes.OrderBy(i => i.ShortName).ToList();
                IndexWatchList.Clear();

                foreach (Index index in indexes)
                {
                    index.ShortName = index.ShortName.Trim();
                    IndexWatchList.Add(index.Symbol, index);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        public void GetIndices()
        {
            List<Index> indexes = _RealTimeDataProvider.GetIndices(GlobalDeclarations.EXCHANGE);
            List<Index> indexStatistics = _RealTimeDataProvider.GetIndicesStatistics(GlobalDeclarations.EXCHANGE);
            foreach (Index index in indexes)
            {
                Index foundIndex = indexStatistics.FirstOrDefault(i => i.Symbol == index.Symbol);
                if (foundIndex != null)
                {
                    index.CentsMoved = foundIndex.CentsMoved;
                    index.Last3MonthClosePrice = foundIndex.Last3MonthClosePrice;
                    index.LastMonthClosePrice = foundIndex.LastMonthClosePrice;
                    index.LastWeekClosePrice = foundIndex.LastWeekClosePrice;
                    index.LastYearClosePrice = foundIndex.LastYearClosePrice;
                    index.PercentageMoved = foundIndex.PercentageMoved;
                    index.Value = foundIndex.Value;
                    index.YesterdayClose = foundIndex.YesterdayClose;
                }
            }

            indexes = indexes.OrderBy(i => i.ShortName).ToList();
            IndexWatchList.Clear();

            List<string> symbols = new List<string>();
            List<string> shortNames = new List<string>();

            foreach (Index index in indexes)
            {
                if(index.ShortName != null)
                    index.ShortName = index.ShortName.Trim();
                
                if(!IndexWatchList.ContainsKey(index.Symbol))
                    IndexWatchList.Add(index.Symbol, index);

                if(!symbols.Contains(index.Symbol))
                    symbols.Add(index.Symbol);

                shortNames.Add(index.ShortName);

                if (IndexUpdatedEvent != null)
                    IndexUpdatedEvent(index);
            }

            SubscribeIndexWatches(symbols, shortNames, GlobalDeclarations.EXCHANGE);
        }

        #endregion
        
        #region Private Methods

        private void ListenToEvents()
        {
            _RealTimeDataProvider.InstrumentUpdatedEvent += _RealTimeDataProvider_InstrumentUpdatedEvent;
            _RealTimeDataProvider.TradeOccurredEvent += _RealTimeDataProvider_TradeOccurredEvent;
            _RealTimeDataProvider.IndexUpdatedEvent += _RealTimeDataProvider_IndexUpdatedEvent;
            _RealTimeDataProvider.QuoteOccurredEvent += _RealTimeDataProvider_QuoteOccurredEvent;
            _RealTimeDataProvider.OrderAddedEvent += _RealTimeDataProvider_OrderAddedEvent;
            _RealTimeDataProvider.OrderbookInitializedEvent += _RealTimeDataProvider_OrderbookInitializedEvent;
            _RealTimeDataProvider.OrderDeletedEvent += _RealTimeDataProvider_OrderDeletedEvent;
            _RealTimeDataProvider.ErrorOccurredEvent += _RealTimeDataProvider_ErrorOccurredEvent;
            _RealTimeDataProvider.DataProviderConnectionChangedEvent += RealTimeDataProviderDataProviderConnectionChangedEvent;
            _RealTimeDataProvider.HeartbeatReceivedEvent += _RealTimeDataProvider_HeartbeatReceivedEvent;
            _RealTimeDataProvider.OnSensNewsEvent += _RealTimeDataProvider_OnSensNewsEvent;

            //_RealTimeDataProvider.OnLevel2BarChart += new Action<ddfplus.BookQuote>(_RealTimeDataProvider_OnLevel2BarChart);
        }

        private void _RealTimeDataProvider_OnSensNewsEvent(MQTTAPI.FullSensNews sensNews)
        {
            if (OnSensNewsEvent != null)
                OnSensNewsEvent(sensNews);
        }

        //void _RealTimeDataProvider_OnLevel2BarChart(ddfplus.BookQuote q)
        //{
        //    if (!LevelTwoSymbol.Contains(q.Symbol)) return;            
        //    //if (Orderbook != null)
        //    //    Orderbook.OnLevel2BarChart(obj);
        //    List<DepthByPrice> DepthBid = new List<DepthByPrice>();
        //    List<DepthByPrice> DepthAsk = new List<DepthByPrice>();
        //    string source = Encoding.ASCII.GetString(q.Source);
        //    int askCount = Math.Min(q.AskPrices.Length, q.AskPrices.Length);
        //    int bidCount = Math.Min(q.BidPrices.Length, q.BidPrices.Length);

        //    for (int i = 0; i < bidCount; i++)
        //    {
        //        DepthByPrice objDepthBid = new DepthByPrice(q.Symbol, "JSE");
        //        OrderbookItem Ord = new OrderbookItem();

        //        objDepthBid.Symbol = q.Symbol;
        //        objDepthBid.Volume = q.BidSizes[i];
        //        objDepthBid.Price = Convert.ToDouble(q.FormatValue(q.BidPrices[i], ddfplus.NumericFormat.Default));

        //        Ord.OriginalOrderCode = i.ToString();//(BidId + 1).ToString();//"5000";
        //        Ord.Price = objDepthBid.Price;
        //        Ord.Size = objDepthBid.Volume;
        //        Ord.OrderDate = q.Timestamp;
        //        Ord.Side = OrderSide.Buy;

        //        objDepthBid.Orders.Add(Ord);

        //        DepthBid.Add(objDepthBid);
        //        //BidId++;
        //    }
        //    for (int i = 0; i < askCount; i++)
        //    {
        //        DepthByPrice objDepthAsk = new DepthByPrice(q.Symbol, "JSE");
        //        OrderbookItem Ord = new OrderbookItem();

        //        objDepthAsk.Symbol = q.Symbol;
        //        objDepthAsk.Volume = q.AskSizes[i];
        //        objDepthAsk.Price = Convert.ToDouble(q.FormatValue(q.AskPrices[i], ddfplus.NumericFormat.Default));

        //        Ord.OriginalOrderCode = i.ToString();//(BidId + 1).ToString();//"5000";
        //        Ord.Price = objDepthAsk.Price;
        //        Ord.Size = objDepthAsk.Volume;
        //        Ord.OrderDate = q.Timestamp;
        //        Ord.Side = OrderSide.Sell;

        //        objDepthAsk.Orders.Add(Ord);
        //        DepthAsk.Add(objDepthAsk);
        //        //BidId++;
        //    }
        //    if (OrderbookInitializedEvent != null)
        //        OrderbookInitializedEvent(DepthBid, DepthAsk);
        //}

        void RealTimeDataProviderDataProviderConnectionChangedEvent(bool isConnected)
        {
            if (isConnected)
                SetupThreads();
        }

        private void TryAddToWatchList(string symbol, Instrument instrument, Dictionary<string, Instrument> watchList)
        {
            try
            {
                if (instrument != null)
                {
                    //Added to handle null exception
                    instrument.Exchange = "BARCHART";

                    if (watchList.ContainsKey(symbol))
                        watchList[symbol] = instrument;
                    else
                          watchList.Add(symbol, instrument);
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("MarketRepository (TryAddToWatchList)", ex);
            }
        }

        private Instrument TrySubscribeLevelOneWatch(string symbol, string exchange)
        {
            try
            {
                Instrument ret = _RealTimeDataProvider.SubscribeLevelOneWatch(symbol, exchange);
                return ret;
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("MarketRepository : TrySubscribeLevelOneWatch", ex);
            }
            return null;
        }

        private List<Instrument> TrySubscribeLevelOneWatches(List<string> symbols, string exchange)
        {
            try
            {
                List<Instrument> ret = _RealTimeDataProvider.SubscribeLevelOneWatches(symbols, exchange);
                return ret;
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("MarketRepository : TrySubscribeLevelOneWatch", ex);
            }
            return null;
        }

        private void TryUnsubscribeLevelOneWatch(string symbol, string exchange)
        {
            try
            {
                _RealTimeDataProvider.UnsubscribeLevelOneWatch(symbol, exchange);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("MarketRepository : TryUnsubscribeLevelOneWatch", ex);
            }
        }

        private void TrySubscribeIndexWatch(string symbol, string exchange)
        {
            try
            {
                _RealTimeDataProvider.SubscribeIndexWatch(symbol, exchange);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("MarketRepository : TrySubscribeIndexWatch", ex);
            }
        }

        private void TryUnsubscribeIndexWatch(string symbol, string exchange)
        {
            try
            {
                _RealTimeDataProvider.UnsubscribeIndexWatch(symbol, exchange);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("MarketRepository : TryUnsubscribeIndexWatch", ex);
            }
        }

        private bool TrySubscribeLevelTwoWatch(string symbol, string exchange)
        {
            try
            {
                if (LevelTwoInstrument != null && !LevelTwoInstrument.Exists(i => i.Symbol == symbol))
                {
                    List<Bid> bids = null;
                    List<Offer> offers = null;
                    List<Order> orders = null;

                    Instrument LevelTwo = AddInstrumentToPortfolioWatch(symbol, exchange);
                    
                    LevelTwo.Bids = bids;
                    LevelTwo.Offers = offers;
                    LevelTwo.Orders = orders;
                    LevelTwoInstrument.Add(LevelTwo);
                    if (!LevelTwoSymbol.Contains(symbol))
                        LevelTwoSymbol.Add(symbol);
                    _RealTimeDataProvider.SubscribeLevelTwoWatch(symbol, exchange);
                }
                return true;
            }
            catch (DomainException dex)
            {
                _ErrorService.LogError("MarketRepository Domain Exception : TrySubscribeLevelTwoWatch", dex);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("MarketRepository : TrySubscribeLevelTwoWatch", ex);
            }
            return false;
        }

        private void TryUnsubscribeLevelTwoWatch(string symbol, string exchange)
        {
            try
            {
                //LevelTwoSymbol = String.Empty;
                //LevelTwoInstrument = new Instrument();
                if (LevelTwoInstrument.Count == 0) return;
                if (LevelTwoSymbol.Contains(symbol)) LevelTwoSymbol.Remove(symbol);
                if (LevelTwoInstrument.Exists(i => i.Symbol == symbol))
                {
                    int ind = LevelTwoInstrument.FindIndex(i => i.Symbol == symbol);
                    LevelTwoInstrument.RemoveAt(ind);
                }
                _RealTimeDataProvider.UnsubscribeLevelTwoWatch(symbol, exchange);
               
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("MarketRepository : TryUnsubscribeLevelTwoWatch", ex);
            }
        }

        private void UnsubscribeLevelTwoIfSubscribed(string symbol,string exchange)
        {
            //if (!String.IsNullOrEmpty(LevelTwoSymbol))
            //{
            //    TryUnsubscribeLevelTwoWatch(LevelTwoSymbol, exchange);
            //}
            if (LevelTwoSymbol.Count > 0)
            {
                TryUnsubscribeLevelTwoWatch(symbol, exchange);
            }
        }

        private void ValidateSymbolAndExchange(string symbol, string exchange)
        {
            if (String.IsNullOrEmpty(symbol.Trim()))
            {
                throw new ArgumentException("Symbol cannot be null or empty", "symbol");
            }
            //if (String.IsNullOrEmpty(exchange.Trim()))// John
            //{
            //    throw new ArgumentException("Exchange cannot be null or empty", "exchange");
            //}
        }

        private void UpdateWatchListWithTrade(Dictionary<string, Instrument> watchList, Trade trade)
        {
            if (watchList.ContainsKey(trade.Symbol))
            {
                try
                {
                    watchList[trade.Symbol].AddTrade(trade);
                    //TryRaiseInstrumentUpdated(watchList[trade.Symbol]);//Commented by john for 01/01/0001 type dateTime
                }
                catch (Exception ex)
                {
                    _ErrorService.LogError("Error with update watchlist with trade: " + ex.Message, ex);
                }
            }
        }

        private void UpdateWatchListWithQuote(Dictionary<string, Instrument> watchList, Quote quote)
        {
            if (watchList.ContainsKey(quote.Symbol))
            {
                try
                {
                    watchList[quote.Symbol].AddQuote(quote);
                    TryRaiseInstrumentUpdated(watchList[quote.Symbol]);
                }
                catch (Exception ex)
                {
                    _ErrorService.LogError("Error with update watchlist with quote: " + ex.Message, ex);
                }
            }
        }

        private void UpdateWatchListsWithTrade(Trade trade)
        {
            UpdateWatchListWithTrade(_AllWatchList, trade);
            UpdateWatchListWithTrade(_PortfolioWatchList, trade);
        }

        private void UpdateWatchListsWithQuote(Quote quote)
        {
            UpdateWatchListWithQuote(_AllWatchList, quote);
            UpdateWatchListWithQuote(_PortfolioWatchList, quote);
        }

        private void UpdateWatchListsWithInstrument(Instrument instrument)
        {
            if (!_AllWatchList.ContainsKey(instrument.Symbol))
            {
                _AllWatchList.Add(instrument.Symbol, instrument);
               
            }
            else if (_AllWatchList.ContainsKey(instrument.Symbol))
            {
                UpdateInstrument(_AllWatchList[instrument.Symbol], instrument);
            }

            if (!_PortfolioWatchList.ContainsKey(instrument.Symbol))
            {
                _PortfolioWatchList.Add(instrument.Symbol, instrument);                
            }
            else if (_PortfolioWatchList.ContainsKey(instrument.Symbol))
            {
                UpdateInstrument(_PortfolioWatchList[instrument.Symbol], instrument);
            }
            
            // Commented By John
            //if (_AllWatchList.ContainsKey(instrument.Symbol))
            //{
            //    UpdateInstrument(_AllWatchList[instrument.Symbol], instrument);
            //}
            //if (_PortfolioWatchList.ContainsKey(instrument.Symbol))
            //{
            //    UpdateInstrument(_PortfolioWatchList[instrument.Symbol], instrument);
            //}
        }

        private void UpdateInstrument(Instrument instrumentToUpdate, Instrument instrument)
        {
            if (instrumentToUpdate != null)
            {
                if (instrumentToUpdate.Symbol == instrument.Symbol)
                {
                    instrumentToUpdate.CentsMoved = instrument.CentsMoved;
                    instrumentToUpdate.High = instrument.High;
                    instrumentToUpdate.LastTrade = instrument.LastTrade;
                    instrumentToUpdate.Low = instrument.Low;
                    instrumentToUpdate.Open = instrument.Open;
                    instrumentToUpdate.PercentageMoved = instrument.PercentageMoved;
                    instrumentToUpdate.TotalTrades = instrument.TotalTrades;
                    instrumentToUpdate.TotalValue = instrument.TotalValue;
                    instrumentToUpdate.TotalVolume = instrument.TotalVolume;
                    instrumentToUpdate.YesterdayClose = instrument.YesterdayClose;
                    instrumentToUpdate.BestBid = instrument.BestBid;
                    instrumentToUpdate.BestOffer = instrument.BestOffer;
                    instrumentToUpdate.BidVolume = instrument.BidVolume;
                    instrumentToUpdate.OfferVolume = instrument.OfferVolume;
                }
            }
        }

        #endregion

        #region Events

        void _RealTimeDataProvider_ErrorOccurredEvent(string error)
        {
            if (ErrorOccurred != null)
                ErrorOccurred(error);
        }

        void _RealTimeDataProvider_InstrumentUpdatedEvent(Instrument instrument)
        {
            //if (instrument.Symbol != null)
            {
                if (InstrumentUpdatedEvent != null)
                {
                    InstrumentUpdatedEvent(instrument);
                }
                // check if an alternative symbol exisits for this stock
                //if (AllInstrumentReferences.Find(s => s.AlternativeSymbol == instrument.Symbol) != null)
                //    instrument.Symbol = AllInstrumentReferences.Find(s => s.AlternativeSymbol == instrument.Symbol).Symbol;

                UpdateWatchListsWithInstrument(instrument);
                //UpdateInstrument(LevelTwoInstrument, instrument);

                if (LevelTwoInstrument.Exists(i => i.Symbol == instrument.Symbol))
                    UpdateInstrument(LevelTwoInstrument.FirstOrDefault(i => i.Symbol == instrument.Symbol), instrument);

                //if (_AllWatchList.ContainsKey(instrument.Symbol))
                //{
                //    if (InstrumentUpdatedEvent != null)
                //    {
                //        InstrumentUpdatedEvent(instrument);
                //    }
                //}
            }
        }

        void _RealTimeDataProvider_QuoteOccurredEvent(Quote quote)
        {
            if (quote.Symbol != null)
            {
                lock (m_syncLock)
                {
                    InstrumentReference alternateInstrument =
                        AllInstrumentReferences.FirstOrDefault(i => i.AlternativeSymbol == quote.Symbol);
                    if (null != alternateInstrument)
                    {
                        quote.Symbol = alternateInstrument.Symbol;
                    }

                    UpdateWatchListsWithQuote(quote);

                    if (_AllWatchList.ContainsKey(quote.Symbol))
                    {
                        if (QuoteOccurredEvent != null)
                        {
                            QuoteOccurredEvent(quote);
                        }
                    }

                    if (Orderbook != null && LevelTwoSymbol.Contains(quote.Symbol))// == quote.Symbol)
                    {
                        Orderbook.AddQuote(quote);
                    }
                    if (_stopLossOrderAgent != null)
                    {
                        _stopLossOrderAgent.QuoteReceived(quote.Exchange, quote.Symbol, (int)quote.BestBidPrice, (int)quote.BestBidSize,
                            (int) quote.BestAskPrice, (int) quote.BestAskSize);
                    }
                }
            }
        }

        void _RealTimeDataProvider_TradeOccurredEvent(Trade trade)
        {
            if (trade.Symbol != null)
            {
                // TODO [AvdM]: IndexOutOfRangeException
                InstrumentReference alternateInstrument = AllInstrumentReferences.FirstOrDefault(i => i.AlternativeSymbol == trade.Symbol);
                if(null != alternateInstrument)
                {
                    trade.Symbol = alternateInstrument.Symbol;
                }

                UpdateWatchListsWithTrade(trade);

                if (_AllWatchList.ContainsKey(trade.Symbol))
                {
                    if (TradeOccurredEvent != null)
                        TradeOccurredEvent(trade);
                }
                if (_stopLossOrderAgent != null)
                {
                    _stopLossOrderAgent.TradeReceived(trade.Exchange, trade.Symbol, (int) trade.Price, (int) trade.Volume);
                }
            }
        }

        void _RealTimeDataProvider_IndexUpdatedEvent(Index index)
        {
            if (IndexWatchList.ContainsKey(index.Symbol))
            {
                IndexWatchList[index.Symbol].CentsMoved = index.CentsMoved;
                IndexWatchList[index.Symbol].PercentageMoved = index.PercentageMoved;
                IndexWatchList[index.Symbol].Value = index.Value;
                IndexWatchList[index.Symbol].SequenceNumber = index.SequenceNumber;
                IndexWatchList[index.Symbol].TimeStamp = index.TimeStamp;

                if (IndexUpdatedEvent != null)
                    IndexUpdatedEvent(IndexWatchList[index.Symbol]);
            }
        }

        void _RealTimeDataProvider_OrderDeletedEvent(string orderCode)
        {
            if (Orderbook != null)
            Orderbook.RemoveOrder(orderCode);
        }

        void _RealTimeDataProvider_OrderAddedEvent(OrderbookItem order)
        {
            if (Orderbook != null)
            Orderbook.AddOrder(order, true);
        }

        void _RealTimeDataProvider_OrderbookInitializedEvent(List<OrderbookItem> orders)
        {
            if (Orderbook != null)
            Orderbook.InitializeOrderbook(orders);
        }

        void _RealTimeDataProvider_HeartbeatReceivedEvent(DateTime servertime)
        {
            if(servertime.Hour == 09 && servertime.Minute == 00)
            {
                ConnectionChanged(true);
            }
            if (HeartbeatReceivedEvent != null)
                HeartbeatReceivedEvent(servertime);
        }

        #endregion

        #region IMarketRepository Members


        public void SetSymbolDictionary(Dictionary<string, InstrumentReference> AllInstrumentReferences)
        {
            _RealTimeDataProvider.SetSymbolDictionary(AllInstrumentReferences);
        }

        public List<JsonLibCommon.FinSwitchHD> GetEODWithDates(string symbol, DateTime startdt, DateTime endDT, string PriceType)
        {
            return _RealTimeDataProvider.GetEODWithDates(symbol,startdt,endDT, PriceType);
        }

        public List<string> GetFundTypes()
        {
            return _RealTimeDataProvider.GetFundTypes();
        }

        public List<JsonLibCommon.Sector> GetSectors(string FType)
        {
            return _RealTimeDataProvider.GetSectors(FType);
        }

        public List<JsonLibCommon.CISFunds> GetCISFunds(string Ftype, string SectorCode)
        {
            return _RealTimeDataProvider.GetCISFunds(Ftype, SectorCode); 
        }

        public List<JsonLibCommon.FinSwitchFundsDetails> AllFinSwitchMFList
        {
            get
            {
                lock (m_syncLock)
                {
                    _allFinSwitchMFList = _RealTimeDataProvider.GetFinSwitchMFList();
                }


                return _allFinSwitchMFList;
            }
        }

        #endregion

    }
}
