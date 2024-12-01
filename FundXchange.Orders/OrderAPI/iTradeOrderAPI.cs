using System;
using System.Data;
using System.Collections.Generic;
using FundXchange.Orders.Entities;
using FundXchange.Orders.FX_OrderService;
using Brokerages = FundXchange.Orders.Enumerations.Brokerages;

namespace FundXchange.Orders.OrderAPI
{
    public class iTradeOrderAPI : IOrderAPI
    {
        private int _cashOnHand = -1;
        private const string Url = "http://www.sanlamitrade.co.za/FundXchange/FundXchange.asmx";

        private readonly int _userId;

        public iTradeOrderAPI(int userId)
        {
            _userId = userId;
        }

        public string AddTradingAccount(string accountName)
        {
            throw new NotImplementedException();
        }

        public void RemoveTradingAccount(string accountNumber, string username, string password)
        {
            throw new NotImplementedException();
        }

        public List<TradingAccount> GetTradingAccounts(string username, string password)
        {
            List<TradingAccount>  tradingAccounts = new List<TradingAccount>();
            using (var service = new iTradeService.FundXchange())
            {
                service.Url = Url;
                DataTable dt = service.VerifyUser(username, password);
                if (dt == null)
                    return new List<TradingAccount>();

                foreach (DataRow row in dt.Rows)
                {
                    string accountNumber = row["AccCde"].ToString();
                    TradingAccount tradingAccount = new TradingAccount(username, password, Brokerages.Sanlam_iTrade, accountNumber, accountNumber);
                    tradingAccounts.Add(tradingAccount);
                }
            }
            return tradingAccounts;
        }


        public string PlaceMarketOrder(TradingAccount tradingAccount, string exchange, string symbol, OrderSide orderSide, int price, int quantity, 
            DateTime? expiryDate)
        {
            using (iTradeService.FundXchange proxy = new iTradeService.FundXchange())
            {
                proxy.Url = Url;
                string side = "b";
                if (orderSide == OrderSide.Sell)
                    side = "c";

                DataTable dt = proxy.PlaceOrder(tradingAccount.Username, tradingAccount.AccountNumber, symbol, side, price, quantity, expiryDate.Value);

                if (dt == null)
                    return "";
                return dt.Rows[0][0].ToString();
            }
        }

        public void UpdateMarketOrder(TradingAccount tradingAccount, string orderId, int price, int quantity, DateTime? expiryDate)
        {
            throw new NotImplementedException();
        }

        public void CancelMarketOrder(TradingAccount tradingAccount, string orderId)
        {
            using (iTradeService.FundXchange proxy = new iTradeService.FundXchange())
            {
                DataTable dt = proxy.CancelOrder(tradingAccount.AccountNumber, orderId);
            }
        }

        public Dictionary<string, MarketOrder> GetMarketOrders(TradingAccount tradingAccount)
        {
            Dictionary<string, MarketOrder> orders = new Dictionary<string, MarketOrder>();

            if (tradingAccount != null)
            {
                using (iTradeService.FundXchange proxy = new iTradeService.FundXchange())
                {
                    proxy.Url = Url;
                    DataTable dt = proxy.GetOrders(tradingAccount.AccountNumber);
                    foreach (DataRow row in dt.Rows)
                    {
                        MarketOrder order = GetMarketOrderFromDataRow(row);
                        orders.Add(Guid.NewGuid().ToString(), order);
                    }

                    DataTable dtPortfolio = proxy.GetPortfolio(tradingAccount.AccountNumber);
                    foreach (DataRow row in dtPortfolio.Rows)
                    {
                        MarketOrder order = GetMarketOrderFromPortfolioDataRow(row);
                        orders.Add(Guid.NewGuid().ToString(), order);
                    }
                }
            }

            return orders;
        }

        public void MarketOrderExpired(TradingAccount tradingAccount, string orderId)
        {
            //throw new NotImplementedException();
        }

        public void MarketOrderMatched(TradingAccount tradingAccount, string orderId, int quantity)
        {
            //throw new NotImplementedException();
        }

        public void DeleteMarketOrder(TradingAccount tradingAccount, string orderId)
        {
            throw new NotImplementedException();
        }

        private static MarketOrder GetMarketOrderFromDataRow(DataRow row)
        {
            MarketOrder order = new MarketOrder();

            order.OrderId = row["external_ref"].ToString();
            order.ExpiryDate = Convert.ToDateTime(row["EXP_DATE"].ToString());
            order.Symbol = row["INSTR_CODE"].ToString();

            order.Price = Convert.ToInt32(row["PRICE"].ToString());
            order.Quantity = Convert.ToInt32(row["QUANTITY"].ToString());

            string side = row["SIDE"].ToString();
            order.Side = GetOrderSide(side);

            string status = row["STATUS"].ToString();
            order.OrderStatus = GetOrderStatus(status);

            //order.Properties.Add("MatchedToday", Convert.ToInt32(row["MATCHED_TODAY"].ToString()));
            //order.Properties.Add("VWAP", Convert.ToInt32(row["VWAP"].ToString()));
           
            return order;
        }

        private MarketOrder GetMarketOrderFromPortfolioDataRow(DataRow row)
        {
            MarketOrder order = new MarketOrder();

            order.OrderId = "Sanlam i-Trade";//????????????
            order.ExpiryDate = null;
            order.Symbol = row["Symbol"].ToString();

            
            int quantity =  Convert.ToInt32(row["Quantity"].ToString());
            if (quantity == 0)
            {
                order.Quantity = Convert.ToInt32(row["AtHomeQuantity"].ToString());
                order.Price = Convert.ToInt32(row["AtHomeCost"].ToString());
            }
            else
            {
                decimal totalCost = Convert.ToDecimal(row["TotalCost"].ToString());
                order.Price = (int)(totalCost * 100) / quantity;
                order.Quantity = quantity;
            }

            string side = "b";
            order.Side = GetOrderSide(side);
            order.OrderStatus = OrderStatusses.Filled;

            _cashOnHand = (int) Convert.ToDecimal(row["CashOnHand"].ToString()) * 100;

            return order;
        }

        private static OrderSide GetOrderSide(string side)
        {
            if (side.ToLower() == "b")
                return OrderSide.Buy;
            return OrderSide.Sell;
        }

        private static OrderStatusses GetOrderStatus(string status)
        {
            switch (status)
            {
                case "O":
                    return OrderStatusses.Open;
                case "X":
                    return OrderStatusses.Cancelled;
                case "F":
                    return OrderStatusses.Partially_Filled;
                case "PX":
                    return OrderStatusses.Pending_Cancel;
                case "A":
                    return OrderStatusses.Awaiting_Authorization;
                default:
                    return OrderStatusses.Awaiting_Authorization;
            }
        }

        #region Stop Loss Orders


        public string AddStopLossOrder(TradingAccount tradingAccount, string exchange, string symbol, int triggerPrice, int stopLossPrice,
            int cancelPrice, PriceTypes priceType, int quantity, OrderSide orderSide, DateTime? expiryDate)
        {
            using (var proxy = new OrderServiceClient())
            {
                FX_OrderService.Brokerages brokerage =
                    (FX_OrderService.Brokerages)
                    Enum.Parse(typeof(FX_OrderService.Brokerages), tradingAccount.Brokerage.ToString());

                OrderCreateResult result = proxy.AddStopLossOrder(brokerage, tradingAccount.AccountNumber, _userId, exchange,
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
                var result = proxy.RemoveStopLossOrder(_userId, orderId);
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
                var result = proxy.UpdateStopLossOrder(_userId, orderId, quantity, triggerPrice, stopLossPrice, priceType, cancelPrice, expiryDate);
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

                OrderCreateResult result = proxy.UpgradeStopLossOrderToMarketOrder(_userId, brokerage, tradingAccount.AccountNumber, orderId, price, quantity);

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
            if (tradingAccount != null)
            {
                using (var proxy = new OrderServiceClient())
                {
                    FX_OrderService.Brokerages brokerage =
                        (FX_OrderService.Brokerages)
                        Enum.Parse(typeof (FX_OrderService.Brokerages), tradingAccount.Brokerage.ToString());

                    var result = proxy.GetStopLossOrders(brokerage, tradingAccount.AccountNumber, _userId);
                    if (result.Result == ResultTypes.Failure)
                    {
                        throw new ApplicationException("Failure retrieving Stop Loss Orders: " + result.ErrorMessage);
                    }
                    foreach (StopLossOrder order in result.Object)
                    {
                        orders.Add(order.OrderId, order);
                    }
                }
            }
            return orders;
        }

        #endregion

        public int GetCashOnHand()
        {
            return _cashOnHand;
        }

        public int GetStartingBalance()
        {
            return -1;
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
