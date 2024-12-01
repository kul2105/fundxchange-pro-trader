using System;
using System.Collections.Generic;
using System.Linq;
using FundXchange.Model.Infrastructure;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.ViewModels.Matrix;
using FundXchange.Model.Agents;
using FundXchange.Domain;
using System.Drawing;
using FundXchange.Domain.Enumerations;

namespace FundXchange.Model.Controllers
{
    public class MatrixController
    {
        public Instrument Instrument { get; private set; }

        public static Color OpenColor = Color.Turquoise;
        public static Color CloseColor = Color.Gray;
        public static Color LastTradeColor = Color.FromArgb(255, 204, 0); //dark yellow
        public static Color HighColor = Color.Green;
        public static Color LowColor = Color.Orange;
        public static Color DefaultColor = Color.Black;
        

        private readonly IMarketRepository _repository;
        private readonly IMatrixView _view;
        private readonly IMatrixViewItemList _items;
        private readonly IErrorService _errorService;

        private readonly MessageHandler<object> _messageHandler;

        public int MaxTradingHour = 17;
        public int MinTradingHour = 9;

        public MatrixController(IMarketRepository marketRepository, IMatrixView view)
            : this (marketRepository, view, new MatrixViewItemList(), new ErrorService())
        {
        }

        public MatrixController(IMarketRepository marketRepository, IMatrixView view, IMatrixViewItemList itemList, IErrorService errorService)
        {
            _repository = marketRepository;
            _view = view;
            _errorService = errorService;

            _messageHandler = new MessageHandler<object>("Matrix");
            _messageHandler.MessageReceived += MessageHandlerMessageReceived;
            _messageHandler.Start();

            _items = itemList;
            _items.DepthInitialized += _Items_DepthInitialized;

            SubscribeToLevel1Events();
        }

        public IList<MatrixViewItem> GetItems()
        {
            IList<MatrixViewItem> items = _items.GetItems();
            if (items == null)
                items = new List<MatrixViewItem>();
            return items;
        }

        public void Subscribe(string symbol, string exchange)
        {
            if (String.IsNullOrEmpty(symbol))
                throw new ApplicationException("Symbol cannot be Null or Empty when subscribing to Matrix");

            if (String.IsNullOrEmpty(exchange))
                throw new ApplicationException("Exchange cannot be Null or Empty when subscribing to Matrix");

            int count = _repository.AllInstrumentReferences.Where(i => i.Symbol == symbol).Count();
            //if (count == 0)
            //    throw new ApplicationException("Invalid symbol specified with subscribe to Matrix");

            if (Instrument == null || symbol != Instrument.Symbol)
            {
                UnsubscribeLevelTwoEvents();
                UnsubscribeListEvents();
                _items.ClearItems();

                //for changing symbol
                FirstInit = true;

                try
                {
                    Instrument instrument = _repository.SubscribeLevelOneWatch(symbol, exchange);
                    Instrument = instrument;
                    _items.AddInstrument(instrument);
                    if (_repository.LevelTwoInstrument != null && _repository.LevelTwoInstrument.Exists(i=>i.Symbol==symbol))// == symbol)
                    {
                        List<DepthByPrice> bids = _repository.Orderbook.GetBids();
                        List<DepthByPrice> offers = _repository.Orderbook.GetOffers();
                        Orderbook_OrderbookInitializedEvent(bids, offers);
                        UpdateViewWithInstrument(Instrument);
                    }
                    else
                    {
                        _repository.SubscribeLevelTwoWatch(symbol, exchange);
                        UpdateViewWithInstrument(Instrument);
                    }
                    SubscribeToLevel1Events();
                    SubscribeToLevelTwoEvents();

                    if (DateTime.UtcNow.AddHours(2).Hour >= MinTradingHour && DateTime.UtcNow.AddHours(2).Hour < MaxTradingHour)
                    {
                        List<Trade> trades = _repository.GetTrades(exchange, symbol,(int) instrument.TotalTrades);
                        if (trades.Count != 0)
                            _items.AddTrades(trades);
                    }
                }
                catch (Exception ex)
                {
                    _errorService.LogError(ex.Message, ex);
                    throw;
                }
            }
        }


        private void SubscribeToLevel1Events()
        {
            _repository.InstrumentUpdatedEvent += MarketRepositoryInstrumentUpdatedEvent;
            _repository.TradeOccurredEvent += MarketRepositoryTradeOccurredEvent;
        }

        private void SubscribeToLevelTwoEvents()
        {
            _repository.Orderbook.OrderbookInitializedEvent += Orderbook_OrderbookInitializedEvent;
            _repository.Orderbook.DepthByPriceItemAddedEvent += Orderbook_DepthByPriceItemAddedEvent;
            _repository.Orderbook.DepthByPriceItemUpdatedEvent += Orderbook_DepthByPriceItemUpdatedEvent;
            _repository.Orderbook.DepthByPriceItemDeletedEvent += Orderbook_DepthByPriceItemDeletedEvent;

            
        }

        public void UnsubscribeLevelTwoEvents()
        {
            if (null != _repository.Orderbook)
            {
                _repository.Orderbook.OrderbookInitializedEvent -= Orderbook_OrderbookInitializedEvent;
                _repository.Orderbook.DepthByPriceItemAddedEvent -= Orderbook_DepthByPriceItemAddedEvent;
                _repository.Orderbook.DepthByPriceItemUpdatedEvent -= Orderbook_DepthByPriceItemUpdatedEvent;
                _repository.Orderbook.DepthByPriceItemDeletedEvent -= Orderbook_DepthByPriceItemDeletedEvent;
                _repository.InstrumentUpdatedEvent -= MarketRepositoryInstrumentUpdatedEvent;
                _repository.TradeOccurredEvent -= MarketRepositoryTradeOccurredEvent;
                
            }
        }

        private void SubscribeToListEvents()
        {
            _items.ItemsAdded += _Items_ItemAdded;
            _items.ItemsUpdated += _Items_ItemUpdated;
        }

        private void UnsubscribeListEvents()
        {
            _items.ItemsAdded -= _Items_ItemAdded;
            _items.ItemsUpdated -= _Items_ItemUpdated;
        }

        public bool FirstInit = true;

        void MessageHandlerMessageReceived(object message)
        {
            if (message is Trade)
            {
                ProcessTrade((Trade)message);
            }
            else if (message is Instrument)
            {
                ProcessInstrument((Instrument)message);
            }
            else if (message is OrderbookMDInitEvent)
            {
                var orders = (OrderbookMDInitEvent)message;

                if (orders.Bids.Count == 0 || orders.Offers.Count == 0) return;
                if (orders.Bids.FirstOrDefault().Symbol == Instrument.Symbol && orders.Offers.FirstOrDefault().Symbol == Instrument.Symbol)
                {
                    if (FirstInit)
                    {
                        _items.InitializeDepth(orders.Bids, orders.Offers);
                        FirstInit = false; //for time being
                    }
                    else
                    {
                        foreach (var item in orders.Bids)
                        {
                            ProcessDepthUpdate(item, true);
                        }
                        foreach (var item in orders.Offers)
                        {
                            ProcessDepthUpdate(item, false);
                        }
                    }
                }
            }
            else if (message is DepthEvent)
            {
                var depth = (DepthEvent)message;
                switch (depth.DepthType)
                {
                    case DepthEvent.DepthTypes.Add:
                        ProcessDepthAdd(depth.Item, depth.IsBuy);
                        break;
                    case DepthEvent.DepthTypes.Delete:
                        ProcessDepthDelete(depth.Item, depth.IsBuy);
                        break;
                    case DepthEvent.DepthTypes.Update:
                        ProcessDepthUpdate(depth.Item, depth.IsBuy);
                        break;
                    default:
                        break;
                }

            }
        }

        void MarketRepositoryTradeOccurredEvent(Trade trade)
        {
            _messageHandler.AddMessage(trade);
        }

        private void ProcessTrade(Trade trade)
        {
            try
            {
                if (Instrument != null)
                {
                    if (trade.Symbol == Instrument.Symbol)
                    {
                        _items.AddTrades(new List<Trade>() { trade });

                        //Depth Remove Event
                        DepthByPrice depth = new DepthByPrice(trade.Symbol, trade.Exchange);
                        if (trade.TradeStatus == TradeStatus.AtBid)//if (trade.Price==trade.Quote.BestBidPrice)
                        {
                            depth.Price = trade.Price;
                            depth.Condition = "BUY";
                            Orderbook_DepthByPriceItemDeletedEvent(depth, true);
                        }
                        else if (trade.TradeStatus == TradeStatus.AtOffer)//(trade.Price == trade.Quote.BestAskPrice)
                        {
                            depth.Price = trade.Price;
                            depth.Condition = "SELL";
                            Orderbook_DepthByPriceItemDeletedEvent(depth, false);
                        }
                        else if (trade.TradeStatus == TradeStatus.BetweenBidAndOffer)
                        {
                            depth.Price = trade.Price;
                            depth.Condition = "Between";
                            Orderbook_DepthByPriceItemDeletedEvent(depth, false);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
            }
        }

        void MarketRepositoryInstrumentUpdatedEvent(Instrument instrument)
        {
            _messageHandler.AddMessage(instrument);
        }

        private void ProcessInstrument(Instrument instrument)
        {
            try
            {
                if (Instrument != null)
                {
                    if (instrument.Symbol == Instrument.Symbol)
                    {
                        UpdateViewWithInstrument(instrument);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
            }
        }

        void Orderbook_OrderbookInitializedEvent(List<DepthByPrice> bids, List<DepthByPrice> offers)
        {
            var ev = new OrderbookMDInitEvent
                         {
                             Bids = bids,
                             Offers = offers
                         };

            _messageHandler.AddMessage(ev); 
        }

        void Orderbook_DepthByPriceItemDeletedEvent(DepthByPrice depthByPrice, bool isBuy)
        {
            var ev = new DepthEvent
                                {
                                    DepthType = DepthEvent.DepthTypes.Delete,
                                    IsBuy = isBuy,
                                    Item = depthByPrice
                                };

            _messageHandler.AddMessage(ev);
        }

        private void ProcessDepthDelete(DepthByPrice depthByPrice, bool isBuy)
        {
            if (depthByPrice.Symbol == Instrument.Symbol)
            {
                //_items.RemoveDepth(depthByPrice.Price, isBuy);
                _items.RemoveDepth(depthByPrice.Price, depthByPrice.Condition);
            }
        }

        void Orderbook_DepthByPriceItemUpdatedEvent(DepthByPrice depthByPrice, bool isBuy)
        {
            var ev = new DepthEvent
                         {
                             DepthType = DepthEvent.DepthTypes.Update,
                             IsBuy = isBuy,
                             Item = depthByPrice
                         };

            _messageHandler.AddMessage(ev);
        }

        private void ProcessDepthUpdate(DepthByPrice depthByPrice, bool isBuy)
        {
            if (depthByPrice.Symbol == Instrument.Symbol)
            {
                _items.UpdateDepth(depthByPrice, isBuy);
            }
        }

        void Orderbook_DepthByPriceItemAddedEvent(DepthByPrice depthByPrice, bool isBuy)
        {
            var ev = new DepthEvent
                                {
                                    DepthType = DepthEvent.DepthTypes.Add,
                                    IsBuy = isBuy,
                                    Item = depthByPrice
                                };

            _messageHandler.AddMessage(ev);
        }

        private void ProcessDepthAdd(DepthByPrice depthByPrice, bool isBuy)
        {
            if (depthByPrice.Symbol == Instrument.Symbol)
            {
                _items.UpdateDepth(depthByPrice, isBuy);
            }
        }

        private void UpdateViewWithInstrument(Instrument instrument)
        {
            _view.UpdateGridWithInstrument(instrument);
        }

        void _Items_DepthInitialized(List<MatrixViewItem> items)
        {
            _view.ClearGrid();
            _view.AddGridRowItems(items);
            _view.UpdateGridWithInstrument(Instrument);
            _view.UpdateTotalTradeVolume(_items.GetTotalTradeVolume());
            _view.UpdateTotalBidSize(_items.GetTotalBidSize());
            _view.UpdateTotalOfferSize(_items.GetTotalOfferSize());
            SubscribeToListEvents();
        }

        void _Items_ItemUpdated(List<MatrixViewItem> items)
        {
            _view.UpdateGridRowItems(items);
            _view.UpdateTotalTradeVolume(_items.GetTotalTradeVolume());
            _view.UpdateTotalBidSize(_items.GetTotalBidSize());
            _view.UpdateTotalOfferSize(_items.GetTotalOfferSize());
        }

        void _Items_ItemAdded(List<MatrixViewItem> items)
        {
            _view.AddGridRowItems(items);
            _view.UpdateTotalTradeVolume(_items.GetTotalTradeVolume());
            _view.UpdateTotalBidSize(_items.GetTotalBidSize());
            _view.UpdateTotalOfferSize(_items.GetTotalOfferSize());
        }

        public void Stop()
        {
            _messageHandler.Dispose();
        }


        public void UnsubscribeLevelTwoWatch(string _Symbol)
        {
            //_repository.UnsubscribeLevelTwoWatch(sym, "JSE");
            //_repository.InstrumentUpdatedEvent -= _Repository_InstrumentUpdatedEvent;

            _repository.UnsubscribeLevelTwoWatch(_Symbol, "JSE");
            _repository.InstrumentUpdatedEvent -= MarketRepositoryInstrumentUpdatedEvent;
            _repository.TradeOccurredEvent -= MarketRepositoryTradeOccurredEvent;
            //_repository.OrderbookInitializedEvent -= Orderbook_OrderbookInitializedEvent;
        }

        public void UnSubcribeRepositoryEvents()
        {
            
        }
    }

    internal class OrderbookMDInitEvent
    {
        internal List<DepthByPrice> Bids;
        internal List<DepthByPrice> Offers;
    }

    internal class DepthEvent
    {
        internal DepthByPrice Item;
        internal bool IsBuy;
        internal bool IsBetween;
        internal enum DepthTypes
        {
            Add,
            Update,
            Delete
        }
        internal DepthTypes DepthType;
    }
}
