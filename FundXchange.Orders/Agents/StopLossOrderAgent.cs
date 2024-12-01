using System;
using System.Collections.Generic;
using System.Linq;
using FundXchange.Infrastructure;
using FundXchange.Orders.Entities;
using FundXchange.Orders.FX_OrderService;
using Brokerages = FundXchange.Orders.Enumerations.Brokerages;

namespace FundXchange.Orders.Agents
{
    public class StopLossOrderAgent
    {
        private readonly OrderService _orderService;
        private readonly Dictionary<string, StopLossOrder> _orders;
        private List<KeyValuePair<TradingAccount, StopLossOrder>> _tradingAccountOrders;

        public delegate void StopLossOrdersToSubscribeDelegate(string exchange, string symbol);
        public event StopLossOrdersToSubscribeDelegate StopLossOrdersToSubscribeEvent;

        private readonly object m_lock = new object();

        public StopLossOrderAgent()
        {
            _orders = new Dictionary<string, StopLossOrder>();
            _orderService = IoC.Resolve<OrderService>();

            _orderService.StopLossOrdersChanged += OrderServiceStopLossOrdersChanged;
        }

        public void Start()
        {
            GetStopLossOrdersForUser();
        }

        void OrderServiceStopLossOrdersChanged()
        {
            GetStopLossOrdersForUser();
        }

        private void GetStopLossOrdersForUser()
        {
            _tradingAccountOrders = new List<KeyValuePair<TradingAccount, StopLossOrder>>();
            var brokerageAssociations = _orderService.GetBrokerageAssociations().ToList();

            BrokerageAssociation fxAssocation = new BrokerageAssociation();
            fxAssocation.Brokerage = FX_OrderService.Brokerages.FundXchange;
            brokerageAssociations.Add(fxAssocation);

            foreach (var association in brokerageAssociations)
            {
                Brokerages brokerage = (Brokerages) Enum.Parse(typeof (Brokerages), association.Brokerage.ToString());
                var tradingAccounts = _orderService.GetTradingAccounts(brokerage);
                foreach (var tradingAccount in tradingAccounts)
                {
                    lock (m_lock)
                    {
                        Dictionary<string, StopLossOrder> orders =
                            _orderService.GetStopLossOrders(tradingAccount.Brokerage, tradingAccount);

                        foreach (var order in orders)
                        {
                            if (!_orders.ContainsKey(order.Key))
                            {
                                _tradingAccountOrders.Add(new KeyValuePair<TradingAccount, StopLossOrder>(tradingAccount, order.Value));
                                _orders.Add(order.Key, order.Value);

                                if (StopLossOrdersToSubscribeEvent != null)
                                {
                                    StopLossOrdersToSubscribeEvent(order.Value.Exchange, order.Value.Symbol);
                                }
                            }
                        }
                    }
                }
                
            }
        }

        public void QuoteReceived(string exchange, string symbol, int bestBidPrice, int bestBidVolume, int bestOfferPrice, int bestOfferVolume)
        {
            EvaluatePrice(symbol, bestBidPrice);
        }

        public void TradeReceived(string exchange, string symbol, int tradePrice, int tradeVolume)
        {
            EvaluatePrice(symbol, tradePrice);
        }

        private void EvaluatePrice(string symbol, int price)
        {
            lock (m_lock)
            {
                foreach (var order in _orders)
                {
                    if (order.Value.Symbol == symbol)
                    {
                        Brokerages brokerage = (Brokerages)Enum.Parse(typeof(Brokerages), order.Value.Brokerage.ToString());
                        var tradingOrder = _tradingAccountOrders.FirstOrDefault(x => x.Value.OrderId == order.Value.OrderId);

                        if (tradingOrder.Key != null)
                        {
                            if (price <= order.Value.CancelStopLossPrice)
                            {
                                //cancel stop loss order
                                _orderService.RemoveStopLossOrder(brokerage, tradingOrder.Key, order.Value.OrderId);
                            }
                            else if (price <= order.Value.TriggerPrice)
                            {
                                //place market order
                                _orderService.PlaceMarketOrder(brokerage, tradingOrder.Key, order.Value.Exchange,
                                                               order.Value.Symbol, OrderSide.Sell,
                                                               order.Value.StopLossPrice.Value,
                                                               order.Value.Quantity.Value, order.Value.ExpiryDate);

                                _orderService.RemoveStopLossOrder(brokerage, tradingOrder.Key, order.Value.OrderId);

                                
                            }
                        }
                    }
                }
            }
        }
    }
}
