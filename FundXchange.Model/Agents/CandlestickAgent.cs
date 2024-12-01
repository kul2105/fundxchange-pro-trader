using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using FundXchange.Domain.Enumerations;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.Infrastructure;
using FundXchange.Domain.Entities;

namespace FundXchange.Model.Agents
{
    public delegate void CandleChangedDelegate(string identifier, Candlestick candle);
    public delegate void CandleInitializedDelegate(string identifier, List<Candlestick> candles);

    public interface ICandlestickAgent
    {
        string Identifier { get; }
        event CandleChangedDelegate CandleAdded;
        event CandleChangedDelegate CandleUpdated;
        event CandleChangedDelegate CandleClosed;
        event CandleInitializedDelegate CandleInitialized;
        void Subscribe(string symbol, string exchange, int numBars, int barIntervalInMinutes, PeriodEnum period);
        List<Candlestick> GetItems();
        bool Equals(object obj);
        Candlestick LatestCandle { get; }
    }

    public class CandlestickAgent : ICandlestickAgent
    {
        private IMarketRepository _repository;
        private Instrument _instrument;
        private SortedList<DateTime, Candlestick> _Items = new SortedList<DateTime, Candlestick>();

        //in minutes
        private int _barInterval;
        private Candlestick _currentCandle;
        private List<Trade> _replayTrades = new List<Trade>();
        private Thread _candleGenerationThread;
        private bool _hasOpen = true;

        public string Identifier { get; private set; }
        public event Action<bool> OHLC;

        public Candlestick LatestCandle
        {
            get
            {
                return _currentCandle;
            }
        }

        public event CandleChangedDelegate CandleAdded;
        public event CandleChangedDelegate CandleUpdated;
        public event CandleChangedDelegate CandleClosed;
        public event CandleInitializedDelegate CandleInitialized;

        public CandlestickAgent(IMarketRepository marketRepository)
            : this(marketRepository, Guid.NewGuid().ToString()) { }

        public CandlestickAgent(IMarketRepository marketRepository, string identifier)
        {
            _repository = marketRepository;
            SubscribeToLevel1Events();
            Identifier = identifier;
        }

        public void Subscribe(string symbol, string exchange, int numBars, int barIntervalInMinutes, PeriodEnum period)
        {
            //if (String.IsNullOrEmpty(symbol))
            //    throw new ApplicationException("Symbol cannot be Null or Empty when subscribing to Candlestick Agent");

            //if (String.IsNullOrEmpty(exchange))
            //    throw new ApplicationException("Exchange cannot be Null or Empty when subscribing to Candlestick Agent");
            ////John
            //symbol = symbol.Replace("/", "");
            //int count = _repository.AllInstrumentReferences.Where(i => i.Symbol == symbol).Count();//_repository.AllInstrumentReferences.Where(i => i.Symbol == symbol).Count();
            //if (count == 0)
            //    throw new ApplicationException("Invalid symbol specified with subscribe to Candlestick Agent");

            //if (numBars < 10)
            //    throw new ApplicationException("Invalid number of bars specified. At least 10 bars needed.");

            //if (_instrument == null || symbol != _instrument.Symbol)
            //{
            //    _barInterval = barIntervalInMinutes;
            //    _Items = new SortedList<DateTime, Candlestick>();

            //    Instrument instrument = _repository.SubscribeLevelOneWatch(symbol, exchange);
            //    _instrument = instrument;

            //    //Chnged By John

            //    string periodicity = string.Empty;
            //    int interval = 0;

            //    switch (period)
            //    {
            //        case PeriodEnum.Minute:
            //            periodicity = "Bar";
            //            break;
            //        case PeriodEnum.Hour:
            //            periodicity = "Bar";
            //            interval = interval * 60;
            //            break;
            //        case PeriodEnum.Day:
            //            periodicity = "DayBar";
            //            break;
            //        case PeriodEnum.Week:
            //            periodicity = "WeeklyBar";
            //            break;
            //        case PeriodEnum.Month:
            //            periodicity = "MonthlyBar";
            //            break;
            //        case PeriodEnum.Year:
            //            break;
            //        default:
            //            break;
            //    }

            //    _repository.SubscribeOHLC(symbol, periodicity, barIntervalInMinutes, numBars, true);

                //Commented By John
                //List<Candlestick> candlesticks = _repository.GetEquityCandlesticks(symbol, exchange, barIntervalInMinutes, numBars, period);
                //ProcessCandlesticks(candlesticks);

                //if (CandleInitialized != null)
                //{
                //    CandleInitialized(symbol, _Items.Values.ToList());
                //}
                //CreateCandlestickThread();
            //}

            int count = _repository.AllInstrumentReferences.Where(i => i.Symbol == symbol).Count();
            if (count == 0)
                throw new ApplicationException("Invalid symbol specified with subscribe to Candlestick Agent");

            if (numBars < 10)
                throw new ApplicationException("Invalid number of bars specified. At least 10 bars needed.");

            if (_instrument == null || symbol != _instrument.Symbol)
            {
                _barInterval = barIntervalInMinutes;
                _Items = new SortedList<DateTime, Candlestick>();

                Instrument instrument = _repository.SubscribeLevelOneWatch(symbol, exchange);
                _instrument = instrument;

                List<Candlestick> candlesticks = _repository.GetEquityCandlesticks(symbol, exchange, barIntervalInMinutes, numBars, period);
                ProcessCandlesticks(candlesticks);

                if (CandleInitialized != null)
                {
                    CandleInitialized(symbol, _Items.Values.ToList());
                }
                CreateCandlestickThread();
            }
        }

        public void Unsubscribe()
        {
            if(null != _candleGenerationThread && _candleGenerationThread.IsAlive)
                _candleGenerationThread.Abort();
        }

        private void CreateCandlestickThread()
        {
            if (_instrument != null)
            {
                _candleGenerationThread = new Thread(GenerateCandles);
                _candleGenerationThread.Name = String.Format("CandleGenerationThread_{0}", _instrument.Symbol);
                _candleGenerationThread.IsBackground = true;
                _candleGenerationThread.Start();
            }
        }

        private void GenerateCandles()
        {
            while (true)
            {
                //if (!IsTradingDay())//By John for Creating new candle and getting Indicator value
                //{
                //    Thread.Sleep(30000);
                //}
                //else
                {
                    if(_currentCandle!=null)
                    {
                    DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                    DateTime beginningOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);
                    TimeSpan timeDiff = currentDate - beginningOfDay;
                    if (timeDiff.TotalMinutes % _barInterval == 0)
                    {
                        int interval = _barInterval;
                        if (interval == 480)
                            interval = (480 * 3);

                        _currentCandle.TimeOfClose = _currentCandle.TimeOfStart.AddMinutes(interval);
                        
                        _Items[_currentCandle.TimeOfStart] = _currentCandle;
                        
                        if (CandleClosed != null)
                        {
                            CandleClosed(_currentCandle.Symbol, _currentCandle);
                        }

                        _currentCandle.TimeOfStart = _currentCandle.TimeOfClose;
                        _currentCandle.Open = _currentCandle.Close;
                        _currentCandle.High = _currentCandle.Close;
                        _currentCandle.Low = _currentCandle.Close;
                        _currentCandle.Close = _currentCandle.Close;
                        _currentCandle.Volume = 0;
                        _hasOpen = false;

                        if (!_Items.ContainsKey(_currentCandle.TimeOfStart))
                        {
                            _Items.Add(_currentCandle.TimeOfStart, _currentCandle);                            
                        }

                        if (CandleAdded != null)
                        {
                            CandleAdded(_currentCandle.Symbol, _currentCandle);
                        }
                    }
                }}
                Thread.Sleep(30000);
            }
        }

        private bool IsTradingDay()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                return false;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                return false;
            if (DateTime.Now.Hour > 17)
                return false;
            if (DateTime.Now.Hour == 17 && DateTime.Now.Minute > 0)
                return false;
            if (DateTime.Now.Hour < 9)
                return false;
            return true;
        }

        private void SubscribeToLevel1Events()
        {
            _repository.TradeOccurredEvent += _marketRepository_TradeOccurredEvent;            
        }

        bool IsFirst = true;
    
        void _marketRepository_TradeOccurredEvent(Trade trade)
        {
            if(_instrument == null) return;
            if (trade.Symbol == _instrument.Symbol)
            {
                ProcessTrade(trade);
            }
        }

        private void ProcessTrade(Trade trade)
        {
            if (null != _currentCandle)
            {
                double Price = trade.Price;

                if (trade.TimeStamp >= _currentCandle.TimeOfStart)
                {
                    
                    _currentCandle.TimeOfClose = trade.TimeStamp;
                    _currentCandle.Volume = trade.Volume;//+= trade.Volume;John

                    if (!_hasOpen)
                        _currentCandle.Open = Price;
                    if (Price > _currentCandle.High)
                        _currentCandle.High = Price;
                    if (Price < _currentCandle.Low || _currentCandle.Low == 0)
                        _currentCandle.Low = Price;
                    _currentCandle.Close = Price;
                    _currentCandle.LastTrade = Price;

                    if(!_Items.ContainsKey(_currentCandle.TimeOfStart))
                        _Items.Add(_currentCandle.TimeOfStart, _currentCandle);
                    else
                        _Items[_currentCandle.TimeOfStart] = _currentCandle;

                    if (CandleUpdated != null)
                    {
                        CandleUpdated(_currentCandle.Symbol, _currentCandle);
                    }
                }
                else
                {
                    DateTime previousCandleStartTime = _currentCandle.TimeOfStart.AddMinutes(-_barInterval);

                    if (!_Items.ContainsKey(_currentCandle.TimeOfStart))
                    {
                        if(!_Items.ContainsKey(previousCandleStartTime))//Added to remove exception of duplicate key by John
                        _Items.Add(previousCandleStartTime, _currentCandle);
                    }

                    if (Price > _Items[previousCandleStartTime].High)
                        _Items[previousCandleStartTime].High = Price;
                    if (Price < _Items[previousCandleStartTime].Low || _Items[previousCandleStartTime].Low == 0)
                        _Items[previousCandleStartTime].Low = Price;
                    _Items[previousCandleStartTime].Close = Price;
                    _Items[previousCandleStartTime].Volume = trade.Volume;//+= trade.Volume; John

                    if (CandleUpdated != null)
                    {
                        CandleUpdated(_Items[previousCandleStartTime].Symbol, _Items[previousCandleStartTime]);
                    }
                }
            }
        }

        private void ProcessCandlesticks(List<Candlestick> candles)
        {
            foreach (Candlestick candle in candles)
            {
                candle.TimeOfClose = candle.TimeOfStart.AddMinutes(_barInterval);

                if (!_Items.ContainsKey(candle.TimeOfClose))
                {
                    _Items.Add(candle.TimeOfClose, candle);
                }
            }
            if (candles.Count > 0)
            {
                Candlestick lastCandle = candles.Last();
                _currentCandle = lastCandle;
            }
        }

        public List<Candlestick> GetItems()
        {
            return _Items.Values.ToList();
        }

        public override bool Equals(object obj)
        {
            return (obj as CandlestickAgent).Identifier == this.Identifier;
        }


        // Added By John
        internal void SetCandles(List<Candlestick> candlesticks)
        {
            ProcessCandlesticks(candlesticks);

            if (CandleInitialized != null)
            {
                CandleInitialized(candlesticks[0].Symbol, _Items.Values.ToList());
            }
            CreateCandlestickThread();
        }
    }
}
