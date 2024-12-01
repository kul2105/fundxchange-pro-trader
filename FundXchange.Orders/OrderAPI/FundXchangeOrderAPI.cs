using System;
using System.Collections.Generic;
using FundXchange.Orders.Entities;
using FundXchange.Orders.Enumerations;
using FundXchange.Orders.FX_OrderService;


namespace FundXchange.Orders.OrderAPI
{
    public class FundXchangeOrderAPI : IOrderAPI
    {
        private int _UserId;

        public FundXchangeOrderAPI(int userId)
        {
            _UserId = userId;
        }

        #region Trading Accounts

        public string AddTradingAccount(string accountName)
        {
            using (var proxy = new OrderServiceClient())
            {
                TradingAccountResult result = proxy.AddTradingAccount(_UserId, accountName);
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Failure creating Trading Account: " + result.ErrorMessage);
                }
                return result.Object.Key.ToString().PadLeft(9, '0');
            }
        }

        public void RemoveTradingAccount(string accountNumber, string username, string password)
        {
            using (var proxy = new OrderServiceClient())
            {
                string accountId = accountNumber;
                GeneralResult result = proxy.RemoveTradingAccount(Convert.ToInt32(accountId));
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Failure removing Trading Account: " + result.ErrorMessage);
                }
            }
        }

        public List<TradingAccount> GetTradingAccounts(string username, string password)
        {
            List<TradingAccount> tradingAccounts = new List<TradingAccount>();
            using (var proxy = new OrderServiceClient())
            {
                TradingAccountListResult result = proxy.GetTradingAccounts(_UserId);
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Failure retrieving Trading Accounts: " + result.ErrorMessage);
                }
                Dictionary<int, string> tradingAccountIds = result.Object;
                foreach (KeyValuePair<int, string> tradingAccountId in tradingAccountIds)
                {
                    TradingAccount account = new TradingAccount("", "", FundXchange.Orders.Enumerations.Brokerages.FundXchange, tradingAccountId.Key.ToString().PadLeft(9, '0'),
                        tradingAccountId.Value);
                    tradingAccounts.Add(account);
                }
            }
            return tradingAccounts;
        }

        #endregion

        #region Market Orders

       

        public string PlaceMarketOrder(TradingAccount tradingAccount, string exchange, string symbol, OrderSide orderSide, 
            int price, int quantity, DateTime? expiryDate)
        {
            using (var proxy = new OrderServiceClient())
            {
                string accountId = tradingAccount.AccountNumber.Substring(2);
                OrderCreateResult result = proxy.AddMarketOrder(Convert.ToInt32(accountId), exchange, symbol, orderSide,
                    price, quantity, expiryDate);

                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Place Market Order was not successful: " + result.ErrorMessage);
                }
                return result.Object.ToString();
            }
        }

        public void UpdateMarketOrder(TradingAccount tradingAccount, string orderId, int price, int quantity, DateTime? expiryDate)
        {
            using (var proxy = new OrderServiceClient())
            {
                string accountId = tradingAccount.AccountNumber.Substring(2);

                GeneralResult result = proxy.UpdateMarketOrder(Convert.ToInt32(accountId), orderId, price, quantity, expiryDate);
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Update Market Order was not successful: " + result.ErrorMessage);
                }
            }
        }

        public void CancelMarketOrder(TradingAccount tradingAccount, string orderId)
        {
            using (var proxy = new OrderServiceClient())
            {
                string accountId = tradingAccount.AccountNumber.Substring(2);

                GeneralResult result = proxy.CancelMarketOrder(Convert.ToInt32(accountId), orderId);
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Cancel Market Order was not successful: " + result.ErrorMessage);
                }
            }
        }

        public Dictionary<string, MarketOrder> GetMarketOrders(TradingAccount tradingAccount)
        {
            Dictionary<string, MarketOrder> orders = new Dictionary<string, MarketOrder>();
            using (var proxy = new OrderServiceClient())
            {
                string accountId = tradingAccount.AccountNumber;//.Substring(2);

                var result = proxy.GetMarketOrders(Convert.ToInt32(accountId));
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Failure retrieving Market Orders: " + result.ErrorMessage);
                }
                foreach (MarketOrder order in result.Object)
                {
                    orders.Add(order.OrderId, order);
                }
            }
            return orders;
        }

        public void MarketOrderExpired(TradingAccount tradingAccount, string orderId)
        {
            using (var proxy = new OrderServiceClient())
            {
                string accountId = tradingAccount.AccountNumber.Substring(2);

                var result = proxy.MarketOrderExpired(Convert.ToInt32(accountId), orderId);
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Failure marking Market Order as expired: " + result.ErrorMessage);
                }
            }
        }

        public void DeleteMarketOrder(TradingAccount tradingAccount, string orderId)
        {
            using (var proxy = new OrderServiceClient())
            {
                string accountId = tradingAccount.AccountNumber.Substring(2);

                var result = proxy.DeleteMarketOrder(Convert.ToInt32(accountId), orderId);
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Failure deleting Market Order: " + result.ErrorMessage);
                }
            }
        }

        public void MarketOrderMatched(TradingAccount tradingAccount, string orderId, int quantity)
        {
            using (var proxy = new OrderServiceClient())
            {
                string accountId = tradingAccount.AccountNumber.Substring(2);

                var result = proxy.MarketOrderMatched(Convert.ToInt32(accountId), orderId, quantity);
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Failure marking Market Order as matched: " + result.ErrorMessage);
                }
            }
        }

        #endregion

        #region Stop Loss Orders


        public string AddStopLossOrder(TradingAccount tradingAccount, string exchange, string symbol, int triggerPrice, int stopLossPrice, 
            int cancelPrice, PriceTypes priceType, int quantity, OrderSide orderSide, DateTime? expiryDate)
        {
            using (var proxy = new OrderServiceClient())
            {
                FX_OrderService.Brokerages brokerage =
                    (FX_OrderService.Brokerages)
                    Enum.Parse(typeof (FX_OrderService.Brokerages), tradingAccount.Brokerage.ToString());

                OrderCreateResult result = proxy.AddStopLossOrder(brokerage, tradingAccount.AccountNumber, _UserId, exchange, 
                    symbol, triggerPrice, stopLossPrice, cancelPrice, priceType, quantity, orderSide, expiryDate);

                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Place Stop Loss Order was not successful: " + result.ErrorMessage);
                }
                return result.Object;
            }
        }

        public void RemoveStopLossOrder(string orderId)
        {
            using (var proxy = new OrderServiceClient())
            {
                var result = proxy.RemoveStopLossOrder(_UserId, orderId);
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Failure Removing Stop Loss Order: " + result.ErrorMessage);
                }
            }
        }

        public void UpdateStopLossOrder(string orderId, int quantity, int triggerPrice, int stopLossPrice, int cancelPrice, 
            PriceTypes priceType, DateTime? expiryDate)
        {
            using (var proxy = new OrderServiceClient())
            {
                var result = proxy.UpdateStopLossOrder(_UserId, orderId, quantity, triggerPrice, stopLossPrice, priceType, cancelPrice, expiryDate);
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Failure Updating Stop Loss Order: " + result.ErrorMessage);
                }
            }
        }

        public string UpgradeStopLossOrderToMarketOrder(TradingAccount tradingAccount, string orderId, int price, int quantity)
        {
            using (var proxy = new OrderServiceClient())
            {
                FX_OrderService.Brokerages brokerage =
                    (FX_OrderService.Brokerages)
                    Enum.Parse(typeof(FX_OrderService.Brokerages), tradingAccount.Brokerage.ToString());

                OrderCreateResult result = proxy.UpgradeStopLossOrderToMarketOrder(_UserId, brokerage, tradingAccount.AccountNumber, orderId, price, quantity);

                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("UpgradeStopLossOrderToMarketOrder was not successful: " + result.ErrorMessage);
                }
                return result.Object;
            }
        }

        public Dictionary<string, StopLossOrder> GetStopLossOrders(TradingAccount tradingAccount)
        {
            Dictionary<string, StopLossOrder> orders = new Dictionary<string, StopLossOrder>();
            using (var proxy = new OrderServiceClient())
            {
                FX_OrderService.Brokerages brokerage =
                    (FX_OrderService.Brokerages)
                    Enum.Parse(typeof(FX_OrderService.Brokerages), tradingAccount.Brokerage.ToString());

                var result = proxy.GetStopLossOrders(brokerage, tradingAccount.AccountNumber, _UserId);
                if (result.Result == ResultTypes.Failure)
                {
                    throw new ApplicationException("Failure retrieving Stop Loss Orders: " + result.ErrorMessage);
                }
                foreach (StopLossOrder order in result.Object)
                {
                    orders.Add(order.OrderId, order);
                }
            }
            return orders;
        }

        #endregion

        public int GetCashOnHand()
        {
            return -1;
        }

        public int GetStartingBalance()
        {
            return 3000000;
        }

        public int UploadFile(string loginID, string pswd, string fileName, string CompanyCode)
        {
            throw new NotImplementedException();
        }

        public string GetProcessStatus(string loginID, string pswd, int processID)
        {
            throw new NotImplementedException();
        }
    }
}
