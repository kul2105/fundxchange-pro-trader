using System;
using FundXchange.Model.Infrastructure;
using System.Collections.Generic;
using FundXchange.Infrastructure;
using FundXchange.Model.AuthenticationService;
using FundXchange.Orders;
using FundXchange.Domain;
using FundXchange.Domain.Entities;
using FundXchange.Model.ViewModels.OrderManager;
using FundXchange.Orders.Entities;
using FundXchange.Orders.FX_OrderService;
using FundXchange_OrderServiceAlias = FundXchange.Orders.FX_OrderService;

namespace FundXchange.Model.Controllers
{
    public class OrderManagerController
    {        
        private ErrorService _errorService;
        private readonly IMarketRepository _repository;
        private readonly UserDTO _user;

        private readonly OrderService _orderService;
        private readonly MessageHandler<Instrument> _messageHandler;

        private const string BROKERAGE_SANLAM_ITRADE = "Sanlam i-trade";
        private const string BROKERAGE_FUNDXCHANGE = "FundXchange";
        private const string BROKERAGE_FINSWITCH = "FinSwitch";

        private readonly List<string> _symbols = new List<string>();
        private List<OrderManagerItem> _items = new List<OrderManagerItem>();

        public delegate void ItemChangedDelegate(OrderManagerItem item);
        public event ItemChangedDelegate ItemChanged;

        public event OrderService.StopLossOrdersChangedDelegate StopLossOrdersChanged;

        private Orders.Enumerations.Brokerages _selectedBrokerage;
        private TradingAccount _selectedTradingAccount;


        public OrderManagerController()
        {
            _messageHandler = new MessageHandler<Instrument>("OrderManagement");
            _messageHandler.MessageReceived += _MessageHandler_MessageReceived;
            _messageHandler.Start();

            _errorService = IoC.Resolve<ErrorService>();
            _user = IoC.Resolve<UserDTO>();

            var brokerageCredentials = new List<BrokerageCredentials>();

            //Commented for authentication
            //foreach (BrokerageAccountDTO dto in _user.BrokerageAccounts)
            //{
            //    var cred = new BrokerageCredentials(dto.Username, dto.Password,
            //        (Orders.Enumerations.Brokerages)Enum.Parse(typeof(Orders.Enumerations.Brokerages), dto.BrokerageType.ToString()));
            //    brokerageCredentials.Add(cred);
            //}

            _orderService = IoC.Resolve<OrderService>();
            if (null != _orderService)
                _orderService.StopLossOrdersChanged += OrderServiceStopLossOrdersChanged;

            _repository = IoC.Resolve<IMarketRepository>();
            _repository.InstrumentUpdatedEvent += RepositoryInstrumentUpdatedEvent;           
        }

        void OrderServiceStopLossOrdersChanged()
        {
            if (StopLossOrdersChanged != null)
            {
                StopLossOrdersChanged();
            }
        }

        void RepositoryInstrumentUpdatedEvent(Instrument instrument)
        {
            _messageHandler.AddMessage(instrument);
        }

        void _MessageHandler_MessageReceived(Instrument instrument)
        {
            foreach (OrderManagerItem item in _items.ToArray())
            {
                string symbol = item.Symbol;
                if (instrument.Symbol == symbol)
                {
                    item.LastTrade = (int)instrument.LastTrade;
                    item.Bid = (int)instrument.BestBid;
                    item.Offer = (int)instrument.BestOffer;

                    if (ItemChanged != null)
                    {
                        ItemChanged(item);
                    }
                }
            }
        }

        public List<string> GetBrokerages()
        {
            var brokerages = _orderService.GetBrokerageAssociations();
            List<string> datasource = GetDescriptiveBrokerages(brokerages);
            return datasource;
        }

        private static List<string> GetDescriptiveBrokerages(IEnumerable<BrokerageAssociation> brokerageAccounts)
        {
            List<string> brokerages = new List<string>();
            brokerages.Add(BROKERAGE_FUNDXCHANGE);

            foreach (var dto in brokerageAccounts)
            {
                switch (dto.Brokerage.ToString())
                {
                    case "Sanlam_iTrade":
                        brokerages.Add(BROKERAGE_SANLAM_ITRADE);
                        break;
                }
            }
            //brokerages.Add(BROKERAGE_FINSWITCH);
            return brokerages;
        }

        public int UploadFinSwitchOrder(string loginID, string pswd, string fileName, string CompanyCodee)
        {            
            return _orderService.UploadFinSwitchOrder(Orders.Enumerations.Brokerages.FinSwitch, loginID, pswd, fileName, CompanyCodee);           
        }

        public void CancelOrder(string orderType, string orderId)
        {
            if (orderType == "Market")
            {
                _orderService.CancelMarketOrder(_selectedBrokerage, _selectedTradingAccount, orderId);
            }
            else if (orderType == "StopLimit")
            {
                _orderService.RemoveStopLossOrder(_selectedBrokerage, _selectedTradingAccount, orderId);
            }
        }

        public void DeleteOrder(string orderType, string orderId)
        {
            if (orderType == "Market")
            {
                _orderService.DeleteMarketOrder(_selectedBrokerage, _selectedTradingAccount, orderId);
            }
            else if (orderType == "StopLimit")
            {
                _orderService.RemoveStopLossOrder(_selectedBrokerage, _selectedTradingAccount, orderId);
            }
        }

        public void UpdateOrder(string orderType, string orderId, int price, int quantity, DateTime? expiry, int triggerPrice, int stopLossPrice,
            int cancelPrice, PriceTypes priceType)
        {
            if (orderType == "Market")
            {
                _orderService.UpdateMarketOrder(_selectedBrokerage, _selectedTradingAccount, orderId, price, quantity, expiry);
            }
            else if (orderType == "StopLimit")
            {
                _orderService.UpdateStopLossOrder(_selectedBrokerage, _selectedTradingAccount, orderId, quantity, triggerPrice, stopLossPrice,
                    cancelPrice, priceType, expiry);
            }
        }

        public List<TradingAccount> ChangeBrokerage(Orders.Enumerations.Brokerages brokerage)
        {
            _selectedBrokerage = brokerage;
            List<TradingAccount> tradingAccounts = _orderService.GetTradingAccounts(brokerage);
            return tradingAccounts;
        }

        public List<OrderManagerItem> GetMarketOrders()
        {
            //get market orders
            var marketOrders = _orderService.GetMarketOrders(_selectedBrokerage, _selectedTradingAccount);
            List<OrderManagerItem> itemsToReturn = new List<OrderManagerItem>();

            foreach (var orderPair in marketOrders)
            {
                var instrument = _repository.AddInstrumentToPortfolioWatch(orderPair.Value.Symbol, "JSE");

                OrderManagerItem item = new OrderManagerItem();
                item.Exchange = "JSE";
                item.Symbol = instrument.Symbol;
                item.LastTrade = (int)instrument.LastTrade;
                if (item.LastTrade == 0)
                    item.LastTrade = (int)instrument.YesterdayClose;
                item.Bid = (int)instrument.BestBid;
                item.Offer = (int)instrument.BestOffer;
                item.Entry = orderPair.Value.Price.Value;
                item.Quantity = orderPair.Value.Quantity.Value;
                item.Side = orderPair.Value.Side.Value;
                item.ExpiryDate = orderPair.Value.ExpiryDate;
                item.OrderId = orderPair.Value.OrderId;
                item.OrderStatus = orderPair.Value.OrderStatus.Value;
                item.OrderType = "Market";
                item.OrderDate = orderPair.Value.OrderDate;
                item.PriceType = "N/A";

                if (item.ExpiryDate.HasValue && DateTime.Now > item.ExpiryDate)
                {
                    //order expired
                    item.OrderStatus = OrderStatusses.Expired;
                    _orderService.MarketOrderExpired(_selectedBrokerage, _selectedTradingAccount, item.OrderId);
                }

                _items.Add(item);
                itemsToReturn.Add(item);
            }
            return itemsToReturn;
        }

        public List<OrderManagerItem> GetStopLossOrders()
        {
            List<OrderManagerItem> itemsToReturn = new List<OrderManagerItem>();

            //get stop loss orders
            var stopLossOrders = _orderService.GetStopLossOrders(_selectedBrokerage, _selectedTradingAccount);
            foreach (var orderPair in stopLossOrders)
            {
                var instrument = _repository.AddInstrumentToPortfolioWatch(orderPair.Value.Symbol, "JSE");

                OrderManagerItem item = new OrderManagerItem();
                item.Exchange = "JSE";
                item.Symbol = instrument.Symbol;
                item.LastTrade = (int)instrument.LastTrade;
                item.Bid = (int)instrument.BestBid;
                item.Offer = (int)instrument.BestOffer;
                if (item.LastTrade == 0)
                    item.LastTrade = (int)instrument.YesterdayClose;
                item.TriggerPrice = orderPair.Value.TriggerPrice.Value;
                item.CancelPrice = orderPair.Value.CancelStopLossPrice.Value;
                item.StopLossPrice = orderPair.Value.StopLossPrice.Value;
                item.Quantity = orderPair.Value.Quantity.Value;
                item.Side = orderPair.Value.Side.Value;
                item.ExpiryDate = orderPair.Value.ExpiryDate;
                item.OrderId = orderPair.Value.OrderId;
                item.OrderStatus = OrderStatusses.Open;
                item.OrderDate = orderPair.Value.OrderDate.Value;
                item.OrderType = "StopLoss";
                item.PriceType = orderPair.Value.PriceType.ToString();

                if (item.ExpiryDate.HasValue && DateTime.Now > item.ExpiryDate)
                {
                    _orderService.RemoveStopLossOrder(_selectedBrokerage, _selectedTradingAccount, item.OrderId);
                }
                else
                {
                    _items.Add(item);
                    itemsToReturn.Add(item);
                }
            }

            return itemsToReturn;
        }

        public void ChangePortfolio(TradingAccount tradingAccount)
        {
            _selectedTradingAccount = tradingAccount;
            _items = new List<OrderManagerItem>();
        }

        public void DeletePortfolio(string accountNumber)
        {
            _orderService.RemoveTradingAccount(_selectedBrokerage, accountNumber);
        }

        public PortfolioSummary GetPortfolioSummary()
        {
            decimal cashUsed = 0.0M;
            decimal totalPnL = 0.0M;

            foreach (var item in _items)
            {
                OrderStatusses status = item.OrderStatus;
                if (status == OrderStatusses.Cancelled && status != OrderStatusses.Pending_Cancel && status != OrderStatusses.Expired)
                    continue;

                cashUsed += item.Entry * item.Quantity;
                totalPnL += item.ProfitLoss;
            }
            PortfolioSummary summary = new PortfolioSummary();

            int startingBalance = _orderService.GetStartingBalance(_selectedBrokerage);
            int cashOnHand = _orderService.GetCashOnHand(_selectedBrokerage);

            if (startingBalance > 0)
                summary.PortfolioValue = (startingBalance + totalPnL) / 100;
            summary.TotalProfitLoss = totalPnL / 100;
            if (startingBalance > 0)
                summary.AccountBalance = (startingBalance - cashUsed) / 100;
            else
                summary.AccountBalance = (decimal)cashOnHand / 100;
            summary.Trades = _items.Count;

            return summary;
        }
        public SymbolSummary GetSymbolSummary(string symbol)
        {
            decimal totalPostion = 0.0M;
            decimal totalPL = 0.0M;
            foreach (var item in _items)
            {
                if (symbol == item.Symbol)
                {
                    OrderStatusses status = item.OrderStatus;
                    if (status == OrderStatusses.Open)
                    {
                        totalPostion += item.Quantity;
                        totalPL += item.ProfitLoss;
                    }
                }
            }
            SymbolSummary summary = new SymbolSummary();
            summary.Postion = totalPostion;
            summary.TotalProfitLoss = totalPL;
            return summary;
        }

        public string AddOrder(string exchange, string symbol, FundXchange_OrderServiceAlias.OrderSide orderSide, DateTime? expiryDate,
            int price, int quantity, int triggerPrice, int stopLossPrice, int cancelPrice, PriceTypes priceType, string orderType)
        {
            var instrument = _repository.AddInstrumentToPortfolioWatch(symbol.ToUpper(), exchange);
            string orderId=string.Empty;
            if (orderType == "StopLoss")
            {
                 orderId = _orderService.AddStopLossOrder(_selectedBrokerage, _selectedTradingAccount, exchange, symbol.ToUpper(),
                    triggerPrice, stopLossPrice, cancelPrice, priceType, quantity, orderSide, expiryDate);
            }
            else
            {
                 orderId = _orderService.PlaceMarketOrder(_selectedBrokerage, _selectedTradingAccount, exchange, symbol.ToUpper(),
                    orderSide, price, quantity, expiryDate);
            }

            return orderId;
        }

        public string CreateTradingAccount(string name, Orders.Enumerations.Brokerages brokerage)
        {
            string accountNumber = _orderService.CreateTradingAccount(brokerage, name);
            return accountNumber;
        }
    }
}
