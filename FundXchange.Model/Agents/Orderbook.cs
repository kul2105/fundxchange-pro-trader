using System;
using System.Linq;
using System.Collections.Generic;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using System.Text;

namespace FundXchange.Model.Agents
{
    public delegate void OrderbookInitialized(List<DepthByPrice> bids, List<DepthByPrice> offers);
    public delegate void OrderbookItemAdded(OrderbookItem order);
    public delegate void OrderbookItemUpdated(OrderbookItem order);
    public delegate void OrderbookItemDeleted(OrderbookItem order);
    public delegate void DepthByPriceItemAdded(DepthByPrice depthByPrice, bool isBuy);
    public delegate void DepthByPriceItemUpdated(DepthByPrice depthByPrice, bool isBuy);
    public delegate void DepthByPriceItemDeleted(DepthByPrice depthByPrice, bool isBuy);
    public delegate void DepthByPriceChanged(List<DepthByPrice> bids, List<DepthByPrice> offers);


    public interface IOrderbook
    {
        //Dictionary<string, OrderbookItem> Orders { get; set; }
        //SortedList<long, DepthByPrice> Bids { get; }
        //SortedList<long, DepthByPrice> Offers { get; }
        event OrderbookInitialized OrderbookInitializedEvent;
        event OrderbookItemAdded OrderbookItemAddedEvent;
        event OrderbookItemUpdated OrderbookItemUpdatedEvent;
        event OrderbookItemDeleted OrderbookItemDeletedEvent;
        event DepthByPriceItemAdded DepthByPriceItemAddedEvent;
        event DepthByPriceItemUpdated DepthByPriceItemUpdatedEvent;
        event DepthByPriceItemDeleted DepthByPriceItemDeletedEvent;
        event DepthByPriceChanged DepthByPriceChangedEvent;
        void AddOrder(OrderbookItem order, bool shouldRaiseEvent);
        void RemoveOrder(string orderCode);
        void InitializeOrderbook(List<OrderbookItem> orders);
        int GetIndexOfDepthItem(DepthByPrice depthByPrice, bool isBuy);
        bool IsTop5(OrderbookItem order);
        OrderbookItem GetOrder(string OriginalOrderCode);
        void AddQuote(Quote quote);

        List<DepthByPrice> GetBids();
        List<DepthByPrice> GetOffers();
        Dictionary<string, OrderbookItem> GetOrders();

    }

    public class Orderbook : IOrderbook
    {
        private readonly string _isin;
        private readonly string _symbol;
        private readonly string _exchange;
        private bool _isRestricted;

        private Dictionary<string, OrderbookItem> Orders { get; set; }
        private SortedList<double, DepthByPrice> Bids { get; set; }
        private SortedList<double, DepthByPrice> Offers { get; set; }

        private readonly object m_lock = new object();

        public event OrderbookItemAdded OrderbookItemAddedEvent;
        public event OrderbookItemUpdated OrderbookItemUpdatedEvent;
        public event OrderbookItemDeleted OrderbookItemDeletedEvent;

        public event DepthByPriceItemAdded DepthByPriceItemAddedEvent;
        public event DepthByPriceItemUpdated DepthByPriceItemUpdatedEvent;
        public event DepthByPriceItemDeleted DepthByPriceItemDeletedEvent;
        public event DepthByPriceChanged DepthByPriceChangedEvent;

        public event OrderbookInitialized OrderbookInitializedEvent;

        public Orderbook(string isin, string symbol, string exchange, bool isRestricted)
        {
            _isRestricted = isRestricted;
            _isin = isin;
            _symbol = symbol;
            _exchange = exchange;
            Bids = new SortedList<double, DepthByPrice>();
            Offers = new SortedList<double,DepthByPrice>();
            Orders = new Dictionary<string, OrderbookItem>();
        }

        public List<DepthByPrice> GetBids()
        {
            lock (m_lock)
            {
                return Bids.Values.ToList();
            }
        }

        public List<DepthByPrice> GetOffers()
        {
            lock (m_lock)
            {
                return Offers.Values.ToList();
            }
        }

        public Dictionary<string, OrderbookItem> GetOrders()
        {
            lock (m_lock)
            {
                return Orders;
            }
        }

        public void InitializeOrderbook(List<OrderbookItem> orders)
        {
            if (orders.FirstOrDefault(i => i.Symbol == _symbol) == null) 
                return;
            List<DepthByPrice> DepthBid = new List<DepthByPrice>();
            List<DepthByPrice> DepthAsk = new List<DepthByPrice>();
            Bids.Clear();
            Offers.Clear();
            foreach (OrderbookItem order in orders)
            {
                //New Code for JSE MItch
                DepthByPrice objDepthBid = new DepthByPrice(order.Symbol, "JSE");
                DepthByPrice objDepthAsk = new DepthByPrice(order.Symbol, "JSE");

                if (order.Side == OrderSide.Buy)
                {
                    objDepthBid.Price = order.Price;
                    objDepthBid.Volume = order.Size;
                    objDepthBid.Orders.Add(order);
                    DepthBid.Add(objDepthBid);

                    AddOrder(order, Bids, false);
                }
                if (order.Side == OrderSide.Sell)
                {
                    objDepthAsk.Price = order.Price;
                    objDepthAsk.Volume = order.Size;
                    objDepthAsk.Orders.Add(order);
                    DepthAsk.Add(objDepthAsk);

                    AddOrder(order, Offers, false);
                }               

            }

            lock (m_lock)
            {
                if (OrderbookInitializedEvent != null)
                    //OrderbookInitializedEvent(DepthBid, DepthAsk);
                OrderbookInitializedEvent(Bids.Values.OrderByDescending(i => i.Price).ToList(), Offers.Values.OrderBy(i => i.Price).ToList());
                //RaiseDepthByPriceChanged(DepthBid, DepthAsk);
            }

            //foreach (OrderbookItem order in orders)
            //{

            //    AddOrder(order, false); //By John
            //    //AddOrder(order, true);
            //}

            //lock (m_lock)
            //{
            //    if (OrderbookInitializedEvent != null)
            //        OrderbookInitializedEvent(Bids.Values.OrderByDescending(i => i.Price).ToList(), Offers.Values.OrderBy(i => i.Price).ToList());
          
            //}
        }

        public void AddOrder(OrderbookItem order, bool shouldRaiseEvent)
        {
            lock (m_lock)
            {
                if (order.Side == OrderSide.Buy)
                {
                    AddOrder(order, Bids, shouldRaiseEvent);
                }
                else
                {
                    AddOrder(order, Offers, shouldRaiseEvent);
                }

                if (OrderbookItemAddedEvent != null && shouldRaiseEvent)
                    OrderbookItemAddedEvent(order);
                //if (order.ISIN != _isin) return;
                //if (Orders.ContainsKey(order.OriginalOrderCode))
                //{
                //    Orders[order.OriginalOrderCode] = order;
                //    if (order.Side == OrderSide.Buy)
                //    {
                //        UpdateOrder(order, Bids, shouldRaiseEvent);
                //    }
                //    else
                //    {
                //        UpdateOrder(order, Offers, shouldRaiseEvent);
                //    }

                //    if (OrderbookItemUpdatedEvent != null && shouldRaiseEvent)
                //        OrderbookItemUpdatedEvent(order);
                //}
                //else
                //{
                //    Orders.Add(order.OriginalOrderCode, order);
                //    if (order.Side == OrderSide.Buy)
                //    {
                //        AddOrder(order, Bids, shouldRaiseEvent);
                //    }
                //    else
                //    {
                //        AddOrder(order, Offers, shouldRaiseEvent);
                //    }

                //    if (OrderbookItemAddedEvent != null && shouldRaiseEvent)
                //        OrderbookItemAddedEvent(order);
                //}
            }
        }

        public void RemoveOrder(string orderCode)
        {
            lock (m_lock)
            {
                OrderbookItem order = null;
                if (!Orders.ContainsKey(orderCode))
                {
                    Console.WriteLine(string.Format("OrderCode:{0} not found.", orderCode));
                    return;
                }

                order = Orders[orderCode];
                Orders.Remove(orderCode);

                if (order.Side == OrderSide.Buy)
                {
                    RemoveOrder(order, Bids);
                }
                else
                {
                    RemoveOrder(order, Offers);
                }
                if (OrderbookItemDeletedEvent != null)
                    OrderbookItemDeletedEvent(order);
            }
        }

        public int GetIndexOfDepthItem(DepthByPrice depthByPrice, bool isBuy)
        {
            if (isBuy)
            {
                return Bids.IndexOfValue(depthByPrice);
            }
            else
            {
                return Offers.IndexOfValue(depthByPrice);
            }
        }

        public bool IsTop5(OrderbookItem order)
        {
            double orderPrice = order.Price;

            if (order.Side == OrderSide.Buy)
            {
                if (Bids.Count < 5)
                    return true;
                if (Bids.ContainsKey(orderPrice))
                {
                    int index = Bids.IndexOfKey(orderPrice);
                    return index <= 5;
                }
                else
                {
                    return orderPrice > Bids.Keys[4];
                }
            }
            else
            {
                if (Offers.Count < 5)
                    return true;
                if (Offers.ContainsKey(orderPrice))
                {
                    int index = Offers.IndexOfKey(orderPrice);
                    return index <= 5;
                }
                else
                {
                    return orderPrice > Offers.Keys[4];
                }
            }
        }

        // TODO: [LeeM]: This is a hack
        public void AddQuote(Quote quote)
        {
            if (quote.Symbol == _symbol)
            {
                try
                {
                    UpdateBidsWithQuote(quote);
                    UpdateOffersWithQuote(quote);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in Orderbook Add Quote Hack: " + ex.ToString());
                }
            }
        }

        private void UpdateOffersWithQuote(Quote quote)
        {
            lock (m_lock)
            {
                if (Offers.Count > 0)
                {
                    while (Offers.IndexOfKey(quote.BestAskPrice) != 0)
                    {
                        DepthByPrice[] prices = Offers.Values.ToArray();
                        foreach (DepthByPrice price in prices)
                        {
                            if (price.Price >= quote.BestAskPrice)
                                return;

                            OrderbookItem[] orders = price.Orders.ToArray();
                            foreach (OrderbookItem order in orders)
                            {
                                RemoveOrder(order.OriginalOrderCode);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateBidsWithQuote(Quote quote)
        {
            lock (m_lock)
            {
                if (Bids.Count > 0)
                {   
                    while (Bids.IndexOfKey(quote.BestBidPrice) != 0)
                    {
                        DepthByPrice[] prices = Bids.Values.ToArray();
                        foreach (DepthByPrice price in prices)
                        {
                            if (price.Price <= quote.BestBidPrice)
                                return;

                            OrderbookItem[] orders = price.Orders.ToArray();
                            foreach (OrderbookItem order in orders)
                            {
                                RemoveOrder(order.OriginalOrderCode);
                            }
                        }
                    }
                }
            }
        }

        public OrderbookItem GetOrder(string OriginalOrderCode)
        {
            if (Orders.ContainsKey(OriginalOrderCode))
                return Orders[OriginalOrderCode];

            return null;
        }

        private void AddOrder(OrderbookItem order, SortedList<double, DepthByPrice> ordersByPrice, bool shouldRaiseEvent)
        {
            double orderPrice = order.Price;
            bool isAdd = false;

            if (!ordersByPrice.ContainsKey(orderPrice))
            {
                DepthByPrice depthItem = new DepthByPrice(_symbol, _exchange);
                depthItem.Price = orderPrice;
                ordersByPrice.Add(orderPrice, depthItem);

                isAdd = true;
            }
            ordersByPrice[orderPrice].Orders.Add(order);
            ordersByPrice[orderPrice].Volume += order.Size;

            if (shouldRaiseEvent)
            {
                if (isAdd)
                {
                    RaiseDepthByPriceItemAdded(ordersByPrice[orderPrice], (order.Side == OrderSide.Buy));
                }
                else
                {
                    RaiseDepthByPriceItemUpdated(ordersByPrice[orderPrice], (order.Side == OrderSide.Buy));
                }
                RaiseDepthByPriceChanged(Bids.Values.ToList(), Offers.Values.ToList());
            }
        }

        private void UpdateOrder(OrderbookItem order, SortedList<double, DepthByPrice> ordersByPrice, bool shouldRaiseEvent)
        {
            double orderPrice = order.Price;

            if (!ordersByPrice.ContainsKey(orderPrice)) return;

            OrderbookItem currentOrderDetails = ordersByPrice[orderPrice].Orders.Find(o => o.OriginalOrderCode == order.OriginalOrderCode);

            if (currentOrderDetails != null)
            {
                long volumeDifference = order.Size - currentOrderDetails.Size;

                ordersByPrice[orderPrice].Orders[ordersByPrice[orderPrice].Orders.IndexOf(currentOrderDetails)] = order;
                ordersByPrice[orderPrice].Volume += volumeDifference;

                if (shouldRaiseEvent)
                {
                    RaiseDepthByPriceItemUpdated(ordersByPrice[orderPrice], (order.Side == OrderSide.Buy));
                    RaiseDepthByPriceChanged(Bids.Values.ToList(), Offers.Values.ToList());
                }
            }
        }

        private void RemoveOrder(OrderbookItem order, SortedList<double, DepthByPrice> ordersByPrice)
        {
            double orderPrice = order.Price;

            if (!ordersByPrice.ContainsKey(orderPrice)) return;

            ordersByPrice[orderPrice].Orders.Remove(order);
            ordersByPrice[orderPrice].Volume -= order.Size;

            if (ordersByPrice[orderPrice].Volume <= 0)
            {
                DepthByPrice depthByPrice = ordersByPrice[orderPrice];

                ordersByPrice.Remove(orderPrice);

                RaiseDepthByPriceItemDeleted(depthByPrice, (order.Side == OrderSide.Buy));
            }
            else
            {
                RaiseDepthByPriceItemUpdated(ordersByPrice[orderPrice], (order.Side == OrderSide.Buy));
            }

            RaiseDepthByPriceChanged(Bids.Values.ToList(), Offers.Values.ToList());
        }

        private void RaiseDepthByPriceItemAdded(DepthByPrice depthItem, bool isBuy)
        {
            if (null != DepthByPriceItemAddedEvent)
            {
                DepthByPrice clone = (DepthByPrice)depthItem.Clone();
                DepthByPriceItemAddedEvent(clone, isBuy);
            }
        }

        private void RaiseDepthByPriceItemUpdated(DepthByPrice depthItem, bool isBuy)
        {
            if (null != DepthByPriceItemUpdatedEvent)
            {
                DepthByPrice clone = (DepthByPrice)depthItem.Clone();
                DepthByPriceItemUpdatedEvent(clone, isBuy);
            }
        }

        private void RaiseDepthByPriceItemDeleted(DepthByPrice depthItem, bool isBuy)
        {
            if (null != DepthByPriceItemDeletedEvent)
            {
                DepthByPrice clone = (DepthByPrice)depthItem.Clone();
                DepthByPriceItemDeletedEvent(clone, isBuy);
            }
        }

        private void RaiseDepthByPriceChanged(IEnumerable<DepthByPrice> bids, IEnumerable<DepthByPrice> offers)
        {
            if (null != DepthByPriceChangedEvent)
            {
                List<DepthByPrice> clonedBids = CopyIt(bids);
                List<DepthByPrice> clonedOffers = CopyIt(offers);

                DepthByPriceChangedEvent(clonedBids, clonedOffers);
            }
        }

        private static List<DepthByPrice> CopyIt(IEnumerable<DepthByPrice> src)
        {
            List<DepthByPrice> copy = new List<DepthByPrice>();
            foreach (DepthByPrice depthByPrice in src)
            {
                copy.Add((DepthByPrice)depthByPrice.Clone());
            }
            return copy;
        }    

    }
    

    public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
   {
      public int Compare(T x, T y)
      {
         return y.CompareTo(x);
      }
    }

    
}
