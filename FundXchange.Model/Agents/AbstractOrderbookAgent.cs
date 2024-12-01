using System;
using System.Linq;
using System.Collections.Generic;
using FundXchange.Domain.Entities;

namespace FundXchange.Model.Agents
{
    public abstract class AbstractOrderbookAgent
    {
        protected int _LastMarketByPriceId = 0;
        protected int _Depth;
        public SortedDictionary<double , MarketByPrice> _MarketByPriceItems;
        public Dictionary<string, OrderbookItem> _OrdersByOrderCode;
        private string _Symbol;
        private string _Exchange;

        public AbstractOrderbookAgent(string symbol, string exchange, int depth)
        {
            _Exchange = exchange;
            _Symbol = symbol;
            if (depth == -1)
            {
                depth = Int32.MaxValue;
            }
            _Depth = depth;
            _MarketByPriceItems = new SortedDictionary<double, MarketByPrice>();
            _OrdersByOrderCode = new Dictionary<string, OrderbookItem>();
        }

        private int GetNextMarketByPriceId()
        {
            return ++_LastMarketByPriceId;
        }

        public void AddOrder(OrderbookItem order)
        {
            if (order.Symbol == _Symbol)
            {
                if (!_OrdersByOrderCode.ContainsKey(order.OriginalOrderCode))
                {
                    _OrdersByOrderCode.Add(order.OriginalOrderCode, order);

                    RaiseOrderbookItemAddedEvent(order);
                }
                else
                {
                    Console.WriteLine(string.Format("Order:{0} already exists in Orders for {1}", order.OriginalOrderCode, order.Symbol));
                }

                if(!_MarketByPriceItems.ContainsKey(order.Price))
                {
                    MarketByPrice marketByPrice = CreateMarketByPrice(order);

                    if(_MarketByPriceItems.Count < _Depth)
                    {
                        _MarketByPriceItems.Add(order.Price, marketByPrice);
                        RaiseMarketByPriceItemAddedEvent(marketByPrice);
                    }
                    else
                    {
                        _MarketByPriceItems.Add(order.Price, marketByPrice);

                        //bids sorted highest to lowest - check lowest value
                        MarketByPrice lastItem = GetLastItem();

                        if (lastItem != marketByPrice)
                        {
                            _MarketByPriceItems.Remove(lastItem.Price);
                            RaiseMarketByPriceDeletedEvent(lastItem);

                            RemoveRelatedOrders(lastItem);

                            RaiseMarketByPriceItemAddedEvent(marketByPrice);
                            _OrdersByOrderCode.Add(order.OriginalOrderCode, order);
                            RaiseOrderbookItemAddedEvent(order);
                        }
                        else
                        {
                            _MarketByPriceItems.Remove(marketByPrice.Price);
                            RemoveRelatedOrders(lastItem);
                        }
                    }
                }
                //if (!_MarketByPriceItems.ContainsKey(order.Price))
                //{
                //    MarketByPrice marketByPrice = new MarketByPrice();

                //    marketByPrice.Id = GetNextMarketByPriceId();
                //    marketByPrice.Exchange = order.Exchange;
                //    marketByPrice.OrderCount = 1;
                //    marketByPrice.Price = order.Price;
                //    marketByPrice.Side = OrderSide.Buy;
                //    marketByPrice.Symbol = order.Symbol;
                //    marketByPrice.Size = order.Size;

                //    if (_MarketByPriceItems.Count < _Depth)
                //    {
                //        _MarketByPriceItems.Add(order.Price, marketByPrice);
                //        _Orders.Add(order.OriginalOrderCode, order);
                //        RaiseMarketByPriceItemAddedEvent(marketByPrice);
                //        RaiseOrderbookItemAddedEvent(order);
                //    }
                //    else
                //    {
                //        _MarketByPriceItems.Add(order.Price, marketByPrice);

                //        //bids sorted highest to lowest - check lowest value
                //        MarketByPrice lastItem = GetLastItem();

                //        if (lastItem != marketByPrice)
                //        {
                //            _MarketByPriceItems.Remove(lastItem.Price);
                //            RaiseMarketByPriceDeletedEvent(lastItem);

                //            RemoveRelatedOrders(lastItem);

                //            RaiseMarketByPriceItemAddedEvent(marketByPrice);
                //            _Orders.Add(order.OriginalOrderCode, order);
                //            RaiseOrderbookItemAddedEvent(order);
                //        }
                //        else
                //        {
                //            _MarketByPriceItems.Remove(marketByPrice.Price);
                //            RemoveRelatedOrders(lastItem);
                //        }
                //    }
                //}
            }
        }

        private MarketByPrice CreateMarketByPrice(OrderbookItem order)
        {
            MarketByPrice marketByPrice = new MarketByPrice();

            marketByPrice.Id = GetNextMarketByPriceId();
            marketByPrice.Exchange = order.Exchange;
            marketByPrice.OrderCount = 1;
            marketByPrice.Price = order.Price;
            marketByPrice.Side = OrderSide.Buy;
            marketByPrice.Symbol = order.Symbol;
            marketByPrice.Size = order.Size;

            return marketByPrice;
        }

        public void UpdateOrder(OrderbookItem order)
        {
            if (order.Symbol == _Symbol)
            {
                if(_OrdersByOrderCode.ContainsKey(order.OriginalOrderCode))
                {
                    _OrdersByOrderCode[order.OriginalOrderCode] = order;

                    RaiseOrderbookItemUpdatedEvent(order);
                }
                else
                {
                    Console.WriteLine(string.Format("Order:{0} does not exist in Orders for {1}", order.OriginalOrderCode, order.Symbol));                    
                }
                //DeleteOrder(order.OriginalOrderCode);
                //AddOrder(order);
            }
        }

        public void DeleteOrder(string orderId)
        {
            if (_OrdersByOrderCode.ContainsKey(orderId))
            {
                OrderbookItem orderToDelete = _OrdersByOrderCode[orderId];

                RaiseOrderbookItemDeletedEvent(orderToDelete);
                //OrderbookItem order = _Orders[orderId];
                //_Orders.Remove(order.OriginalOrderCode);
                //RaiseOrderbookItemDeletedEvent(order);

                //if (_MarketByPriceItems.ContainsKey(order.Price))
                //{
                //    _MarketByPriceItems[order.Price].OrderCount -= 1;
                //    _MarketByPriceItems[order.Price].Size -= order.Size;
                //    if (_MarketByPriceItems[order.Price].OrderCount == 0)
                //    {
                //        RaiseMarketByPriceDeletedEvent(_MarketByPriceItems[order.Price]);
                //        _MarketByPriceItems.Remove(order.Price);
                //    }
                //    else
                //    {
                //        RaiseMarketByPriceUpdatedEvent(_MarketByPriceItems[order.Price]);
                //    }
                //}
            }
            else
            {
                Console.WriteLine(string.Format("Order:{0} does not exist in Orders for {1}", orderId, _Symbol));                
            }
        }

        protected abstract MarketByPrice GetLastItem();

        private void RemoveRelatedOrders(MarketByPrice marketByPrice)
        {
            var ordersToRemove = _OrdersByOrderCode.Where(o => o.Value.Price == marketByPrice.Price);
            foreach (var orderItem in ordersToRemove)
            {
                _OrdersByOrderCode.Remove(orderItem.Key);
                RaiseOrderbookItemDeletedEvent(orderItem.Value);
            }
        }

        #region Raise Event Helpers

        protected abstract void RaiseOrderbookItemAddedEvent(OrderbookItem order);
        protected abstract void RaiseOrderbookItemUpdatedEvent(OrderbookItem order);
        protected abstract void RaiseOrderbookItemDeletedEvent(OrderbookItem order);
        protected abstract void RaiseMarketByPriceItemAddedEvent(MarketByPrice marketByPrice);
        protected abstract void RaiseMarketByPriceUpdatedEvent(MarketByPrice marketByPrice);
        protected abstract void RaiseMarketByPriceDeletedEvent(MarketByPrice marketByPrice);

        #endregion

    }
}
