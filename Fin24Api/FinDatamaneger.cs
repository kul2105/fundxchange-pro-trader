using System;
using System.Collections.Generic;
using System.Text;
using Fin24.Markets.ClientSubscription;
using Fin24.Markets.State;
using fin24.Patterns.DomainPatterns.IoC;
using System.Threading;
using fin24.Patterns.InfrastructurePatterns.Logging;
using System.Windows.Forms;
namespace Fin24Api
{
    public class FinDatamaneger
    {
#region "Variables"
        private Object m_SubscribeListLockObject = new Object();
        SubscriptionService m_Service = null;

        public event EventHandler<MarketEventArgs> MarketChange = null;
        public event EventHandler<LevelTwoMarketEventArgs> LevelTwoMarketChange = null;
        public event EventHandler<LevelTwoLastTradesEventArgs> LevelTwoLastTradesChange = null;
        public event EventHandler<TradeOccurredEventArgs> TradeOccuredChange = null;
#endregion

        class UILogger : ILog
        {
            TextBox _TextBoxToLog;
            Queue<string> _Messages;
            bool _Processing;
            public UILogger(TextBox textBoxToLog)
            {
                //_TextBoxToLog = textBoxToLog;
                //_Messages = new Queue<string>();
                //_Processing = true;
                //Thread logger = new Thread(Log);
                //logger.IsBackground = true;
                //logger.Start();
            }

            private void Log()
            {
                //while (_Processing)
                //{


                //    if (_Messages.Count > 0)
                //    {
                //        _TextBoxToLog.Invoke(new CrossAppDomainDelegate(delegate()
                //        {
                //            if (_TextBoxToLog.TextLength > 10000)
                //            {
                //                _TextBoxToLog.Clear();
                //            }
                //        }));
                //        string message = _Messages.Dequeue();
                //        _TextBoxToLog.Invoke(new CrossAppDomainDelegate(delegate()
                //        {
                //            _TextBoxToLog.AppendText(message + "\n");
                //        }));
                //    }
                //}
            }

            public bool LogInfo(string message, LogType logType, LogPriority priority)
            {
              //  _Messages.Enqueue(message);
                return true;
            }

            public bool LogError(string message, Exception ex)
            {
                //_Messages.Enqueue(ex.ToString());
                return true;
            }
        }

        public class BasicEventArgs : EventArgs
        {
            public String Symbol;
            public String Exchange;
        }

        public class MarketEventArgs : BasicEventArgs
        {
            public  Double Bid;
            public  Double Ask;
            public  Int32 AskSize;
            public  Int32 BidSize;
            public long Volume;
            public Double High;
            public Double Low;
            public Double YesterdayClose;
            public Double Move;
            public Decimal MovePercent;
            public long Deals;
            public Double DailyValue;
            public Double DailyVolume;
            public Double Last;
            public Double LastWeekClosePrice;
            public Double LastMonthClosePrice;
            public Double LastYearClosePrice;
            public Double Last12MonthClosePrice;
            public Decimal MovePercentLastWeek;
            public Decimal MovePercentLastMonth;
            public Decimal MovePercentLastYear;
            public Decimal MovePercentLast12MonthAgo;
            public Double OpenPrice;


            public MarketEventArgs(String aSymbol, Double aBid, Double aAsk, Int32 aAskSize, Int32 aBidSize)
            {
                this.Symbol = aSymbol;
                this.Bid = aBid;
                this.Ask = aAsk;
                this.AskSize = aAskSize;
                this.BidSize = aBidSize;
            }

            public static MarketEventArgs Empty()
            {
                MarketEventArgs ret = new MarketEventArgs(string.Empty, 0, 0, 0, 0);
                return ret;
            }
 
        }

        public class LevelTwoMarketEventArgs : BasicEventArgs
        {
            public enum MarketTypeEnum
            {
                Bid = 0,
                Ask
            }

            public MarketTypeEnum MarketType;
            public List<Double> Bid, Ask;
            public List<long> BidSize, AskSize;

            public LevelTwoMarketEventArgs(MarketTypeEnum aType)
            {
                MarketType = aType;

                if (aType == MarketTypeEnum.Ask)
                {
                    Ask = new List<Double>();
                    AskSize = new List<long>();
                }
                else 
                {
                    Bid = new List<Double>();
                    BidSize = new List<long>();
                }
            }

        }

        public class LevelTwoLastTradesEventArgs : BasicEventArgs
        {
            public List<OrderDTO> Orders = new List<OrderDTO>();

            public void AddNewOrder(OrderDTO or)
            {
                Orders.Add(new OrderDTO(or.Symbol,or.Exchange,or.Id,or.Side,or.Price,or.Volume,or.LastUpdated));
            }
        }

          public class TradeOccurredEventArgs : BasicEventArgs
            {
            public long Price;
            public DateTime Date;
              public long Volume;
              public long BidVolume;
              public long OfferVolume;
              public long SequenceNumber;
          }

        #region "Private"
        
        public void Initialize(TextBox aTextBox)
        {

            //var resolver = new DependencyResolver();

            //resolver.Register<ILog>(new UILogger(aTextBox));
            //IoC.Initialize(resolver);

            //m_Service = SubscriptionServiceFactory.CreateSubscriptionService();
            //m_Service.TradeOccured += m_Service_TradeOccured;
            //m_Service.BidsChanged += OnService_BidsChanged;
            //m_Service.InstrumentUpdate += OnService_InstrumentUpdate;
            //m_Service.NewQuote += OnService_NewQuote;
            //m_Service.OffersChanged += OnService_OffersChanged;
            //m_Service.OrdersChanged += m_Service_OrdersChanged;
            ////PricesTradedChanged += new SubscriptionService.PricesTradedCollectionChangedEventHandler(OnService_PricesTradedChanged);
            //m_Service.Start();

            
        }

       

        private void OnMarketChange(MarketEventArgs args)
        {
            if (MarketChange != null)
                MarketChange(this, args);
        }

        private void OnLevelTwoMarketChange(LevelTwoMarketEventArgs args)
        {
            if (LevelTwoMarketChange != null)
                LevelTwoMarketChange(this, args);
        }

        private void OnTradeOccuredChange(TradeOccurredEventArgs args)
        {
            if (TradeOccuredChange != null)
                TradeOccuredChange(this, args);
        }

        private void OnLevelTwoLastTradesChange(LevelTwoLastTradesEventArgs args)
        {
            if (LevelTwoLastTradesChange != null)
                LevelTwoLastTradesChange(this, args);
        }

        #endregion

        #region "Public"
        
        public IList<BidDTO> Bidlist;
        public IList<OfferDTO> Asklist;
        public IList<OrderDTO> PriceList;

        public void Subscribe(String aSymbol, String aExchange)
        {
              //InstrumentDTO tdto;
              //IList<TradeDTO> list;
              //m_Service.RegisterLevelOneWatch(aSymbol,aExchange, out tdto, out list);
              ////m_Service.RegisterLevelTwoWatch(aSymbol,aExchange,out Bidlist, out Asklist, out PriceList);

              //if (tdto != null)
              //    OnService_InstrumentUpdate(null, tdto);

              ////SubscribeLevel2(aSymbol, aExchange);
        }

        public void Subscribe(String aSymbol, String aExchange, out InstrumentDTO instrument, out IList<TradeDTO> trades)
        {
            instrument = null;
            trades = null;
           // m_Service.RegisterLevelOneWatch(aSymbol, aExchange, out instrument, out trades);

            //m_Service.RegisterLevelTwoWatch(aSymbol,aExchange,out Bidlist, out Asklist, out PriceList);

            //if (tdto != null)
            //    OnService_InstrumentUpdate(null, tdto);

            //SubscribeLevel2(aSymbol, aExchange);
        }
        
        public void UnSubscribe(String aSymbol, String aExchange)
        {
            //m_Service.UnregisterLevelOneWatch(aSymbol,aExchange);
        }

        public void SubscribeLevel2(String aSymbol, String aExchange)
        {
            //m_Service.RegisterLevelTwoWatch(aSymbol, aExchange, out Bidlist, out Asklist, out PriceList);

            //LevelTwoMarketEventArgs lvlMarketEventArgs = new LevelTwoMarketEventArgs(LevelTwoMarketEventArgs.MarketTypeEnum.Bid);
            //if (Bidlist.Count > 0)
            //    lvlMarketEventArgs.Symbol = Bidlist[0].Symbol;

            //for (Int32 idx = 0; idx < Bidlist.Count; idx++)
            //{
            //    lvlMarketEventArgs.Bid.Add(Bidlist[idx].Price);
            //    lvlMarketEventArgs.BidSize.Add(Bidlist[idx].Size);
            //}
            //OnLevelTwoMarketChange(lvlMarketEventArgs);

            //lvlMarketEventArgs = new LevelTwoMarketEventArgs(LevelTwoMarketEventArgs.MarketTypeEnum.Ask);
            //if (Asklist.Count > 0)
            //    lvlMarketEventArgs.Symbol = Asklist[0].Symbol;

            //for (Int32 idx = 0; idx < Asklist.Count; idx++)
            //{
            //    lvlMarketEventArgs.Ask.Add(Asklist[idx].Price);
            //    lvlMarketEventArgs.AskSize.Add(Asklist[idx].Size);
            //}
            //OnLevelTwoMarketChange(lvlMarketEventArgs);

           /* if (PriceList.Count > 0)
            {
                LevelTwoLastTradesEventArgs args = new LevelTwoLastTradesEventArgs();
                args.Symbol = PriceList[0].Symbol;
                for (Int32 Idx = 0; Idx < PriceList.Count; Idx++)
                {
                    args.Prices.Add(PriceList[Idx].Price);
                    args.PriceSizes.Add(PriceList[Idx].Volume);
                }
                OnLevelTwoLastTradesChange(args);
            }*/
        }

        public void UnSubscribeLevel2(String aSymbol, String aExchange)
        {
            //try
            //{
            //    m_Service.UnregisterLevelTwoWatch(aSymbol, aExchange);
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("Exception (UnSubscribeLevel2): " + ex);
            //}
        }


        public IList<SnapshotDTO> GetHistory(String aSymbol, String aExchange,Int32 aInterval, Int32 aBarsCount)
        {
            IList<SnapshotDTO> RetList = new List<SnapshotDTO>();
            //long res = m_Service.GetSnapshotHistory(aSymbol.ToUpper(),aExchange,aInterval, aBarsCount, out RetList);
            return RetList;
        }
#endregion

        #region "OnService_XXX"
        #region "LevelOneData"
        void OnService_InstrumentUpdate(object sender, InstrumentDTO instrument)
        {
            MarketEventArgs arg = MarketEventArgs.Empty();
            arg.Symbol = instrument.Symbol;
            arg.Exchange = instrument.Exchange;
            arg.Volume = instrument.TotalVolume;
            arg.High = instrument.HighTradedPrice;
            arg.Low = instrument.LowTradedPrice;
            arg.Move = instrument.CentsMoved;
            arg.MovePercent = instrument.PercentageMoved;
            arg.YesterdayClose = instrument.YesterdayClosePrice;
            arg.Deals = instrument.TotalTrades;
            arg.DailyValue = instrument.TotalValue;
            arg.DailyVolume = instrument.TotalVolume;
            arg.Last = instrument.LastTradedPrice;

            arg.LastWeekClosePrice = instrument.LastWeekClosePrice;
            arg.LastMonthClosePrice = instrument.LastMonthClosePrice;
            arg.LastYearClosePrice = instrument.LastYearClosePrice;
            arg.Last12MonthClosePrice = instrument.TwelveMonthsAgoClosePrice;
            arg.MovePercentLastWeek = instrument.PercentageMovedLastWeek;
            arg.MovePercentLastMonth = instrument.PercentageMovedLastMonth;
            arg.MovePercentLastYear = instrument.PercentageMovedLastYear;
            arg.MovePercentLast12MonthAgo = instrument.PercentageMovedTwelveMonthsAgo;
            arg.OpenPrice = instrument.YesterdayClosePrice;
            OnMarketChange(arg);
        }

        void OnService_NewQuote(object sender, QuoteDTO quote)
        {
            MarketEventArgs args = new MarketEventArgs(quote.Symbol, quote.BestBidPrice, (Int32)quote.BestAskPrice, (Int32)quote.BestAskSize, (Int32)quote.BestBidSize);
            args.Volume = - 1;
            OnMarketChange(args);
        }

        void m_Service_TradeOccured(object sender, TradeDTO trade)
        {
            TradeOccurredEventArgs args = new TradeOccurredEventArgs();

            args.Symbol = trade.Symbol;
            args.Exchange = trade.Exchange;
            args.Date = trade.TradeTimeStamp;
            args.Price = trade.Price;
            args.Volume = trade.Volume;
            args.BidVolume = trade.BidVolume;
            args.OfferVolume = trade.OfferVolume;
            args.SequenceNumber = trade.SequenceNumber;

            OnTradeOccuredChange(args);
        }
        #endregion

        #region "LevelTwoData"
        void OnService_BidsChanged(object sender, IList<BidDTO> bids)
        {
            LevelTwoMarketEventArgs lvlMarketEventArgs = new LevelTwoMarketEventArgs(LevelTwoMarketEventArgs.MarketTypeEnum.Bid);
            if (bids.Count > 0)
            {
                lvlMarketEventArgs.Symbol = bids[0].Symbol;
                lvlMarketEventArgs.Exchange = bids[0].Exchange;
            }

            for (Int32 idx = 0; idx < bids.Count; idx++)
            {
                lvlMarketEventArgs.Bid.Add(bids[idx].Price);
                lvlMarketEventArgs.BidSize.Add(bids[idx].Size);
            }

            OnLevelTwoMarketChange(lvlMarketEventArgs);
        }

        void OnService_OffersChanged(object sender, IList<OfferDTO> offers)
        {
            LevelTwoMarketEventArgs lvlMarketEventArgs = new LevelTwoMarketEventArgs(LevelTwoMarketEventArgs.MarketTypeEnum.Ask);
            if (offers.Count > 0)
            {
                lvlMarketEventArgs.Symbol = offers[0].Symbol;
                lvlMarketEventArgs.Exchange = offers[0].Exchange;
            }

            for (Int32 idx = 0; idx < offers.Count; idx++)
            {
                lvlMarketEventArgs.Ask.Add(offers[idx].Price);
                lvlMarketEventArgs.AskSize.Add(offers[idx].Size);
            }

            OnLevelTwoMarketChange(lvlMarketEventArgs);
        }

        // not us now
        void OnService_PricesTradedChanged(object sender, IList<PriceTradedDTO> pricesTraded)
        {
            if (pricesTraded.Count > 0)
            {
                LevelTwoLastTradesEventArgs args = new LevelTwoLastTradesEventArgs();
                args.Symbol = pricesTraded[0].Symbol;
                //for (Int32 Idx = 0; Idx < pricesTraded.Count; Idx++)
                //{
                //    //args.Prices.Add(pricesTraded[Idx].Price);
                //    //args.PriceSizes.Add(pricesTraded[Idx].VolumeTraded);
                //}
                OnLevelTwoLastTradesChange(args);
            }
        }



        void m_Service_OrdersChanged(object sender, IList<OrderDTO> orders)
        {
            if (orders.Count > 0)
            {
                LevelTwoLastTradesEventArgs args = new LevelTwoLastTradesEventArgs();
                args.Symbol = orders[0].Symbol;
                args.Exchange = orders[0].Exchange;
                for (Int32 Idx = 0; Idx < orders.Count; Idx++)
                {
                    args.AddNewOrder(orders[Idx]);
                }
                OnLevelTwoLastTradesChange(args);
            }
        }

        #endregion
        #endregion
    }
}
