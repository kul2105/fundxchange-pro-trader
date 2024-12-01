using System;
using System.Collections.Generic;
using System.Linq;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.ViewModels;
using FundXchange.Model.Infrastructure;
using FundXchange.Infrastructure;
using FundXchange.Domain;
using FundXchange.Model.Agents;
using FundXchange.Model.ViewModels.MarketDepth;
using System.Windows.Forms;
using System.ComponentModel;

namespace FundXchange.Model.Controllers
{
    public class MarketDepthController
    {
        private MessageHandler<object> _MessageHandler;
        private IMarketRepository _Repository;
        private IMarketDepthView _view;
        private string m_Symbol;
        public string Symbol
        {
            get { return m_Symbol; }
            set
            {
                m_Symbol = value;
                ChangeLevel2WatchSymbol(Symbol);                
            }
        }
        private string _CurrentlySubscribedSymbol = string.Empty;

        public event Action<List<DepthByPrice>, List<DepthByPrice>> OnInitializedDepth = delegate { };
        public event Action<List<DepthByPrice>, List<DepthByPrice>> OnDepthPriceChanged = delegate { };
        
        public MarketDepthController()
        {
 
        }
        public MarketDepthController(string symbol,IMarketDepthView view)
        {
            _view = view;          
            Symbol = symbol;                                
        }
        public void ChangeLevel2WatchSymbol(string symbol)
        {
            //_MainForm.ShowStatus(string.Format("Market Depth: Adding level 2 watch for symbol: {0}", symbol));          
            try
            {
                _CurrentlySubscribedSymbol = symbol;  
                _Repository = IoC.Resolve<IMarketRepository>();
                if (_MessageHandler != null)
                    _MessageHandler.Dispose();
                _MessageHandler = new MessageHandler<object>("MarketDepth");
                _MessageHandler.MessageReceived += _MessageHandler_MessageReceived;
                _MessageHandler.Start();
                //_Repository.InstrumentUpdatedEvent += _Repository_InstrumentUpdatedEvent;
                if (_Repository.Orderbook != null && _Repository.LevelTwoSymbol.Contains(symbol))// == Symbol)
                {
                    Orderbook_OrderbookInitializedEvent(_Repository.Orderbook.GetBids(), _Repository.Orderbook.GetOffers());
                }
                if (_Repository.IndexWatchList.ContainsKey(symbol))
                {
                    MessageBox.Show("Level 2 watch cannot be added for an Index");
                    return;
                }               
                UnsubscribeFromOrderbookEvents();                  
                Instrument instrument = _Repository.SubscribeLevelOneWatch(symbol, "JSE");
                _view.UpdateInstrumentTable(instrument);
                _Repository.InstrumentUpdatedEvent += _Repository_InstrumentUpdatedEvent;
                _Repository.SubscribeLevelTwoWatch(symbol, "JSE");
                SubscribeToOrderbookEvents();                
                _view.ResetBidAndOfferGrids();
                _view.UpdateSymbol();
            }
            catch (DomainException dex)
            {
                //Utils.DisplayError(dex.Message);
            }
            //_MainForm.ShowStatus("Connected");
        }

        private void SubscribeToOrderbookEvents()
        {
            _Repository.Orderbook.DepthByPriceChangedEvent += Orderbook_DepthByPriceChangedEvent;
            _Repository.Orderbook.OrderbookItemAddedEvent += Orderbook_OrderbookItemAddedEvent;
            _Repository.Orderbook.OrderbookItemUpdatedEvent += Orderbook_OrderbookItemUpdatedEvent;
            _Repository.Orderbook.OrderbookItemDeletedEvent += Orderbook_OrderbookItemDeletedEvent;
            _Repository.Orderbook.OrderbookInitializedEvent += Orderbook_OrderbookInitializedEvent;           
        }

        public void UnsubscribeFromOrderbookEvents()
        {
            if (null != _Repository && null != _Repository.Orderbook)
            {
                _Repository.Orderbook.DepthByPriceChangedEvent -= Orderbook_DepthByPriceChangedEvent;
                _Repository.Orderbook.OrderbookItemAddedEvent -= Orderbook_OrderbookItemAddedEvent;
                _Repository.Orderbook.OrderbookItemUpdatedEvent -= Orderbook_OrderbookItemUpdatedEvent;
                _Repository.Orderbook.OrderbookItemDeletedEvent -= Orderbook_OrderbookItemDeletedEvent;
                _Repository.Orderbook.OrderbookInitializedEvent -= Orderbook_OrderbookInitializedEvent;
                
                _Repository.InstrumentUpdatedEvent -= _Repository_InstrumentUpdatedEvent;
            }
        }

        void _Repository_InstrumentUpdatedEvent(Instrument instrument)
        {
            _MessageHandler.AddMessage(instrument);
        }

        void Orderbook_OrderbookItemDeletedEvent(OrderbookItem order)
        {
            OrderbookMDEvent newEvent = new OrderbookMDEvent();
            newEvent.Item = order;
            newEvent.OrderbookType = OrderbookMDEvent.OrderbookTypes.Delete;

            _MessageHandler.AddMessage(newEvent);
        }

        void Orderbook_OrderbookItemUpdatedEvent(OrderbookItem order)
        {
            OrderbookMDEvent newEvent = new OrderbookMDEvent();
            newEvent.Item = order;
            newEvent.OrderbookType = OrderbookMDEvent.OrderbookTypes.Update;

            _MessageHandler.AddMessage(newEvent);
        }

        void Orderbook_OrderbookItemAddedEvent(OrderbookItem order)
        {
            OrderbookMDEvent newEvent = new OrderbookMDEvent();
            newEvent.Item = order;
            newEvent.OrderbookType = OrderbookMDEvent.OrderbookTypes.Add;

            _MessageHandler.AddMessage(newEvent);
        }

        void Orderbook_OrderbookInitializedEvent(List<DepthByPrice> bids, List<DepthByPrice> offers)
        {
            OrderbookMDInitEvent newEvent = new OrderbookMDInitEvent();
            newEvent.Bids = bids;
            newEvent.Offers = offers;
            _MessageHandler.AddMessage(newEvent);
        }

        void Orderbook_DepthByPriceChangedEvent(List<DepthByPrice> bids, List<DepthByPrice> offers)
        {
            EnqueueDepthByPriceChangedEvent(bids, offers);
        }
        public void EnqueueDepthByPriceChangedEvent(List<DepthByPrice> bids, List<DepthByPrice> offers)
        {
            DepthByPriceMDEvent newEvent = new DepthByPriceMDEvent();
            newEvent.Bids = bids;//.OrderByDescending(i => i.Price).ToList();
            newEvent.Offers = offers;//.OrderBy(i => i.Price).ToList();
            _MessageHandler.AddMessage(newEvent);
        }
        void _MessageHandler_MessageReceived(object message)
        {
            if (message is Instrument)
            {
                Instrument inst = message as Instrument;
                _view.ProcessInstrument(inst);
            }
            else if (message is OrderbookMDEvent)
            {
                OrderbookMDEvent order = (OrderbookMDEvent)message;

                switch (order.OrderbookType)
                {
                    case OrderbookMDEvent.OrderbookTypes.Add:
                        _view.ProcessOrderbookAdd(order.Item);
                        break;
                    case OrderbookMDEvent.OrderbookTypes.Delete:
                        _view.ProcessOrderbookDelete(order.Item);
                        break;
                    case OrderbookMDEvent.OrderbookTypes.Update:
                        _view.ProcessOrderbookUpdate(order.Item);
                        break;
                    default:
                        break;
                }

            }
            else if (message is OrderbookMDInitEvent)
            {

                OrderbookMDInitEvent orders = (OrderbookMDInitEvent)message;
                _view.ProcessOrderbookInitEvent(orders.Bids, orders.Offers);
            }
            else if (message is DepthByPriceMDEvent)
            {
                DepthByPriceMDEvent depth = (DepthByPriceMDEvent)message;
                _view.ProcessDepthByPriceChanged(depth.Bids, depth.Offers);
            }
        }

        internal class OrderbookMDInitEvent
        {
            internal List<DepthByPrice> Bids;
            internal List<DepthByPrice> Offers;
        }

        internal class DepthByPriceMDEvent
        {
            internal List<DepthByPrice> Bids { get; set; }
            internal List<DepthByPrice> Offers { get; set; }
        }

        internal class OrderbookMDEvent
        {
            internal OrderbookItem Item;
            internal enum OrderbookTypes
            {
                Add,
                Update,
                Delete
            }
            internal OrderbookTypes OrderbookType;
        }


        public void UnsubscribeLevelTwoWatch(string sym)
        {
            _Repository.UnsubscribeLevelTwoWatch(sym, "JSE");
            _Repository.InstrumentUpdatedEvent -= _Repository_InstrumentUpdatedEvent;
            //_Repository.OrderbookInitializedEvent -= Orderbook_OrderbookInitializedEvent;
        }
    }
    //public class MarketDepthController
    //{
    //    #region Private Members

    //    private MarketRepository _Repository;

    //    #endregion

    //    #region Properties

    //    public string Exchange { get; set; }
    //    public int MaxMarketDepth { get; private set; }
    //    public int MaxBidOfferCross { get; private set; }
    //    public Instrument Instrument { get; private set; }
    //    public MarketDepthViewModel ViewModel { get; private set; }

    //    #endregion

    //    #region Events Declarations

    //    public delegate void ChangedDelegate();
    //    public event ChangedDelegate InstrumentUpdatedEvent;
    //    public event ChangedDelegate OrdersChangedEvent;
    //    public event ChangedDelegate BidsChangedEvent;
    //    public event ChangedDelegate OffersChangedEvent;
    //    public event ChangedDelegate TradeOccurredEvent;

    //    #endregion

    //    #region Constructors

    //    public MarketDepthController(string exchange, int maxMarketDepth, MarketDepthViewModel viewModel)
    //    {
    //        ViewModel = viewModel;
    //        Exchange = exchange;
    //        MaxMarketDepth = maxMarketDepth;

    //        MaxBidOfferCross = maxMarketDepth;
    //        if (MaxMarketDepth == GlobalDeclarations.FULL_MARKET_DEPTH)
    //        {
    //            MaxBidOfferCross = GlobalDeclarations.MAX_BID_OFFER_CROSS;
    //        }
    //        _Repository = IoC.Resolve<IMarketRepository>();

    //        ListenToEvents();
    //    }

    //    #endregion

    //    #region Private Methods

    //    private void ListenToEvents()
    //    {
    //        _Repository.InstrumentUpdatedEvent += _Service_InstrumentUpdatedEvent;
    //        _Repository.TradeOccurredEvent += _Service_TradeOccurredEvent;
    //    }

    //    private void CalucateBidOfferCross()
    //    {
    //        List<Bid> topBids = new List<Bid>();
    //        List<Offer> topOffers = new List<Offer>();

    //        if (this.MaxBidOfferCross <= GlobalDeclarations.MAX_BID_OFFER_CROSS)
    //        {
    //            topBids = (from b in ViewModel.Bids
    //                       where ViewModel.BuyOrders.Count(bo => bo.Price == b.Price) > 0
    //                       select b).Take(this.MaxBidOfferCross).ToList();

    //            topOffers = (from b in ViewModel.Offers
    //                         where ViewModel.SellOrders.Count(bo => bo.Price == b.Price) > 0
    //                         select b).Take(this.MaxBidOfferCross).ToList();
    //        }
    //        //else
    //        //{
    //        //    topBids = (from b in ViewModel.Bids
    //        //               where ViewModel.BuyOrders.Count(bo => bo.Price == b.Price) > 0
    //        //               select b).Take(GlobalDeclarations.MAX_BID_OFFER_CROSS).ToList();

    //        //    topOffers = (from b in ViewModel.Offers
    //        //                 where ViewModel.SellOrders.Count(bo => bo.Price == b.Price) > 0
    //        //                 select b).Take(GlobalDeclarations.MAX_BID_OFFER_CROSS).ToList();
    //        //}

    //        int itemsToProcess = topOffers.Count < topBids.Count ? topOffers.Count : topBids.Count;

    //        for (int i = 0; i < itemsToProcess; i++)
    //        {
    //            if (i >= ViewModel.BidOfferCrossBinding.Count)
    //                ViewModel.BidOfferCrossBinding.AddNew();

    //            topBids[i].OrderCount = ViewModel.BuyOrders.Count(b => b.Price == (double) topBids[i].Price);
    //            topOffers[i].OrderCount = ViewModel.SellOrders.Count(o => o.Price == (double) topOffers[i].Price);

    //            ViewModel.BidOfferCrossBinding[i].Update(topBids[i], topOffers[i]);
    //        }
    //    }

    //    #region Raise Events

    //    private void RaiseInstrumentUpdatedEvent()
    //    {
    //        if (InstrumentUpdatedEvent != null)
    //        {
    //            InstrumentUpdatedEvent();
    //        }
    //    }

    //    private void RaiseTradeOccurredEvent()
    //    {
    //        if (TradeOccurredEvent != null)
    //        {
    //            TradeOccurredEvent();
    //        }
    //    }

    //    #endregion

    //    #endregion

    //    #region Events

    //    void _Service_InstrumentUpdatedEvent(Instrument instrument)
    //    {
    //        if (ViewModel.Instrument.Symbol == instrument.Symbol)
    //        {
    //            ViewModel.Instrument = instrument;
    //            RaiseInstrumentUpdatedEvent();
    //        }
    //    }

    //    void _Service_TradeOccurredEvent(Trade trade)
    //    {
    //        if (ViewModel.Instrument.Symbol == trade.Symbol)
    //        {
    //            ViewModel.Instrument.AddTrade(trade);
    //            RaiseTradeOccurredEvent();
    //        }
    //    }

    //    //void _Service_OrdersChangedEvent(List<Order> orders)
    //    //{
    //    //    List<Order> buyOrders = (from ord in orders
    //    //                             where ord.Side == OrderSide.Buy
    //    //                             select ord).OrderByDescending(o => o.Price).ToList();

    //    //    List<Order> sellOrders = (from ord in orders
    //    //                              where ord.Side == OrderSide.Sell
    //    //                              select ord).OrderBy(o => o.Price).ToList();

    //    //    ViewModel.BuyOrders = buyOrders;
    //    //    ViewModel.SellOrders = sellOrders;
    //    //    RaiseOrdersChangedEvent();
    //    //}

    //    //void _Service_OffersChangedEvent(List<Offer> offers)
    //    //{
    //    //    offers = offers.OrderBy(a => a.Price).ToList();
    //    //    ViewModel.Offers = offers;
    //    //    RaiseOffersChangedEvent();
    //    //    CalucateBidOfferCross();
    //    //}

    //    //void _Service_BidsChangedEvent(List<Bid> bids)
    //    //{
    //    //    bids = bids.OrderByDescending(a => a.Price).ToList();
    //    //    ViewModel.Bids = bids;
    //    //    RaiseBidsChangedEvent();
    //    //    CalucateBidOfferCross();
    //    //}

    //    #endregion
    //}
}
