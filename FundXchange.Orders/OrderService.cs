using System;
using System.Collections.Generic;
using System.Linq;
using FundXchange.Orders.OrderAPI;
using FundXchange.Orders.Entities;
using FundXchange.Orders.Exceptions;
using FundXchange.Orders.FX_OrderService;

namespace FundXchange.Orders
{
    public class OrderService
    {
        private readonly Dictionary<Enumerations.Brokerages, IOrderAPI> _orderApIs;
        //private readonly List<BrokerageCredentials> _brokerageCredentials;
        private readonly int _userId;
      
        public delegate void StopLossOrdersChangedDelegate();
        public event StopLossOrdersChangedDelegate StopLossOrdersChanged;

        private IList<BrokerageAssociation> _brokerageAssociations;

        //public event OrderChangedDelegate OrderChanged;
        //public event OrderRemovedDelegate OrderRemoved;
        private bool IsOECConnected = false;

        public OrderService(int userId)
        {
            _orderApIs = new Dictionary<Enumerations.Brokerages, IOrderAPI>();
            IOrderAPI iTradeAPI = new iTradeOrderAPI(userId);
            IOrderAPI fundxchangeAPI = new FundXchangeOrderAPI(userId);
            IOrderAPI FinSwitchAPI = new FinSwitchOrderAPI();
            _userId = userId;

            _orderApIs.Add(Enumerations.Brokerages.Sanlam_iTrade, iTradeAPI);
            _orderApIs.Add(Enumerations.Brokerages.FundXchange, fundxchangeAPI);
            _orderApIs.Add(Enumerations.Brokerages.FinSwitch, FinSwitchAPI);

            //GetBrokerageAssociations();Commented By John


            //SetupOrderAgents();

        }

        public IList<BrokerageAssociation> GetBrokerageAssociations()
        {
            using (var service = new OrderServiceClient())
            {
                var result = service.GetBrokerageAssociations(_userId);
                if (result.Result == ResultTypes.Success)
                {
                    _brokerageAssociations = result.Object;
                    return result.Object;
                }
            }
            return new List<BrokerageAssociation>();
        }

        public int UploadFinSwitchOrder(Enumerations.Brokerages brokerage, string loginID, string pswd, string fileName, string CompanyCodee)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            int orderId = orderAPI.UploadFile(loginID, pswd, fileName, CompanyCodee);
            return orderId;
        }

        public List<TradingAccount> GetTradingAccounts(Enumerations.Brokerages brokerage)
        {
            if (!_orderApIs.ContainsKey(brokerage))
            {
                throw new BrokerageNotFoundException(brokerage);
            }
            IOrderAPI orderAPI = _orderApIs[brokerage];

            if (brokerage != Enumerations.Brokerages.FundXchange)
            {
                var credentials = _brokerageAssociations.FirstOrDefault(x => x.Brokerage == 
                    (Brokerages)Enum.Parse(typeof(Brokerages), brokerage.ToString()));

                //BrokerageCredentials credentials = _brokerageCredentials.FirstOrDefault(x => x.Brokerage == brokerage);
                if (credentials == null)
                    throw new ApplicationException("Credentials could not be found for the specified brokerage: " + brokerage);

                return orderAPI.GetTradingAccounts(credentials.Username, credentials.Password);
            }
            else
            {
                return orderAPI.GetTradingAccounts("", "");
            }
        }

        public string CreateTradingAccount(Enumerations.Brokerages brokerage, string accountName)
        {
            if (!_orderApIs.ContainsKey(brokerage))
            {
                throw new BrokerageNotFoundException(brokerage);
            }
            IOrderAPI orderAPI = _orderApIs[brokerage];
            string accountNumber = orderAPI.AddTradingAccount(accountName);
            return accountNumber;
        }

        public void RemoveTradingAccount(Enumerations.Brokerages brokerage, string accountNumber)
        {
            if (!_orderApIs.ContainsKey(brokerage))
            {
                throw new BrokerageNotFoundException(brokerage);
            }
            IOrderAPI orderAPI = _orderApIs[brokerage];

            if (brokerage != Enumerations.Brokerages.FundXchange)
            {
                throw new ApplicationException("Not allowed!!! : " + brokerage);
            }
            orderAPI.RemoveTradingAccount(accountNumber, "", "");
        }


        public string PlaceMarketOrder(Enumerations.Brokerages brokerage, TradingAccount tradingAccount, string exchange, string symbol, OrderSide orderSide,
            int price, int quantity, DateTime? expiryDate)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            string orderId = orderAPI.PlaceMarketOrder(tradingAccount, exchange, symbol, orderSide, price, quantity, expiryDate);
            return orderId;
        }

        public void UpdateMarketOrder(Enumerations.Brokerages brokerage, TradingAccount tradingAccount, string orderId, int price, int quantity, DateTime? expiryDate)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            orderAPI.UpdateMarketOrder(tradingAccount, orderId, price, quantity, expiryDate);
        }

        public void CancelMarketOrder(Enumerations.Brokerages brokerage, TradingAccount tradingAccount, string orderId)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            orderAPI.CancelMarketOrder(tradingAccount, orderId);
        }

        public Dictionary<string, MarketOrder> GetMarketOrders(Enumerations.Brokerages brokerage, TradingAccount tradingAccount)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            return orderAPI.GetMarketOrders(tradingAccount);
        }

        public void MarketOrderExpired(Enumerations.Brokerages brokerage, TradingAccount tradingAccount, string orderId)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            orderAPI.MarketOrderExpired(tradingAccount, orderId);
        }

        public void MarketOrderMatched(Enumerations.Brokerages brokerage, TradingAccount tradingAccount, string orderId, int quantity)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            orderAPI.MarketOrderMatched(tradingAccount, orderId, quantity);
        }

        public void DeleteMarketOrder(Enumerations.Brokerages brokerage, TradingAccount tradingAccount, string orderId)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            orderAPI.DeleteMarketOrder(tradingAccount, orderId);
        }

        public string AddStopLossOrder(Enumerations.Brokerages brokerage, TradingAccount tradingAccount, string exchange, string symbol, 
            int triggerPrice, int stopLossPrice, int cancelPrice, PriceTypes priceType, int quantity, OrderSide orderSide, DateTime? expiryDate)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            string orderId = orderAPI.AddStopLossOrder(tradingAccount, exchange, symbol, triggerPrice, stopLossPrice, cancelPrice, priceType, 
                quantity, orderSide, expiryDate);

            if (StopLossOrdersChanged != null)
            {
                StopLossOrdersChanged();
            }

            return orderId;
        }

        public void RemoveStopLossOrder(Enumerations.Brokerages brokerage, TradingAccount tradingAccount, string orderId)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            orderAPI.RemoveStopLossOrder(orderId);

            if (StopLossOrdersChanged != null)
            {
                StopLossOrdersChanged();
            }
        }

        public void UpdateStopLossOrder(Enumerations.Brokerages brokerage, TradingAccount tradingAccount, string orderId, int quantity, 
            int triggerPrice, int stopLossPrice, int cancelPrice, PriceTypes priceType, DateTime? expiryDate)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            orderAPI.UpdateStopLossOrder(orderId, quantity, triggerPrice, stopLossPrice, cancelPrice, priceType, expiryDate);

            if (StopLossOrdersChanged != null)
            {
                StopLossOrdersChanged();
            }
        }

        public string UpgradeStopLossOrderToMarketOrder(Enumerations.Brokerages brokerage, TradingAccount tradingAccount, string orderId, 
            int price, int quantity)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            string newOrderId = orderAPI.UpgradeStopLossOrderToMarketOrder(tradingAccount, orderId, price, quantity);

            if (StopLossOrdersChanged != null)
            {
                StopLossOrdersChanged();
            }

            return newOrderId;
        }

        public Dictionary<string, StopLossOrder> GetStopLossOrders(Enumerations.Brokerages brokerage, TradingAccount tradingAccount)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            return orderAPI.GetStopLossOrders(tradingAccount);
        }

        public int GetStartingBalance(Enumerations.Brokerages brokerage)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            return orderAPI.GetStartingBalance();
        }

        public int GetCashOnHand(Enumerations.Brokerages brokerage)
        {
            IOrderAPI orderAPI = _orderApIs[brokerage];
            return orderAPI.GetCashOnHand();
        }


















        //public OrderService(Dictionary<Brokerages, IOrderAPI> orderAPIs)
        //{
        //    _OrderAPIs = orderAPIs;
        //    SetupOrderAgents();
        //}

        //private void SetupOrderAgents()
        //{
        //    _OrderAgents = new Dictionary<OrderType, IOrderAgent>();
        //    IOrderAgent marketOrderAgent = new MarketOrderAgent(_OrderAPIs, _UserId);
        //    _OrderAgents.Add(OrderType.Market, marketOrderAgent);
        //    IoC.RegisterInstance<IOrderAgent>(marketOrderAgent);
        //    IOrderAgent stopLossOrderAgent = new StopLossOrderAgent(_OrderAPIs, _UserId);
        //    _OrderAgents.Add(OrderType.StopLoss, stopLossOrderAgent);
            
        //    foreach (KeyValuePair<OrderType, IOrderAgent> agent in _OrderAgents)
        //    {
        //        agent.Value.OrderChanged += Agent_OrderChanged;
        //        agent.Value.OrderRemoved += Agent_OrderRemoved;
        //    }
        //    RegisterTradingAccounts();
        //}

        //private void RegisterTradingAccounts()
        //{
        //    foreach (BrokerageCredentials credentials in _BrokerageCredentials)
        //    {
        //        List<TradingAccount> tradingAccounts = GetTradingAccounts(credentials.Brokerage, credentials.Username, credentials.Password);
        //        foreach (TradingAccount tradingAccount in tradingAccounts)
        //        {
        //            foreach (KeyValuePair<OrderType, IOrderAgent> pair in _OrderAgents)
        //            {
        //                pair.Value.Register(tradingAccount);
        //            }
        //        }
        //    }
        //}

        //void Agent_OrderChanged(TradingAccount tradingAccount, Order order)
        //{
        //    if (OrderChanged != null)
        //    {
        //        OrderChanged(tradingAccount, order);
        //    }
        //}

        //void Agent_OrderRemoved(TradingAccount tradingAccount, string orderId)
        //{
        //    if (OrderRemoved != null)
        //    {
        //        OrderRemoved(tradingAccount, orderId);
        //    }
        //}


        

        //public string PlaceOrder(TradingAccount tradingAccount, Order order)
        //{
        //    IOrderAgent orderAgent = GetOrderAgent(order.OrderType);
        //    return orderAgent.PlaceOrder(tradingAccount, order);
        //}

        //private IOrderAgent GetOrderAgent(OrderType orderType)
        //{
        //    if (!_OrderAgents.ContainsKey(orderType))
        //    {
        //        throw new OrderTypeNotSupportedException(orderType);
        //    }
        //    return _OrderAgents[orderType];
        //}

        //public void CancelOrder(TradingAccount tradingAccount, string orderId)
        //{
        //    bool cancelled = false;
        //    foreach (KeyValuePair<OrderType, IOrderAgent> agent in _OrderAgents)
        //    {
        //        if (agent.Value.OrderExists(tradingAccount, orderId))
        //        {
        //            agent.Value.CancelOrder(tradingAccount, orderId);
        //            cancelled = true;
        //            break;
        //        }
        //    }
        //    if (!cancelled)
        //        throw new OrderDoesNotExistException(orderId);
        //}

        //public void UpdateOrder(TradingAccount tradingAccount, Order order)
        //{
        //    IOrderAgent orderAgent = GetOrderAgent(order.OrderType);
        //    if (!orderAgent.OrderExists(tradingAccount, order.OrderId))
        //    {
        //        throw new OrderDoesNotExistInOrderAgentException(order.OrderId, orderAgent.Name);
        //    }
        //    orderAgent.UpdateOrder(tradingAccount, order);
        //}

        //public Dictionary<string, Order> GetOrders(TradingAccount tradingAccount)
        //{
        //    Dictionary<string, Order> orders = new Dictionary<string, Order>();

        //    foreach (KeyValuePair<OrderType, IOrderAgent> agent in _OrderAgents)
        //    {
        //        Dictionary<string, Order> agentOrders = agent.Value.GetOrders(tradingAccount);
        //        foreach (KeyValuePair<string, Order> order in agentOrders)
        //        {
        //            orders.Add(order.Key, order.Value);
        //        }
        //    } 
        //    return orders;
        //}

        //public Order GetOrder(TradingAccount tradingAccount, string orderId, OrderType orderType)
        //{
        //    IOrderAgent orderAgent = GetOrderAgent(orderType);
        //    if (!orderAgent.OrderExists(tradingAccount, orderId))
        //    {
        //        throw new OrderDoesNotExistInOrderAgentException(orderId, orderAgent.Name);
        //    }
        //    return orderAgent.GetOrder(tradingAccount, orderId);
        //}
    }
}
