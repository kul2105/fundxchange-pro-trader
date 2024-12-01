using System;
using System.Collections.Generic;
using FundXchange.Brokerage.Model.Entities;
using FundXchange.Brokerage.Model.Enumerations;
using System.Data;
using FundXchange.Brokerage.Model.ValueObjects;

namespace FundXchange.Brokerage.Model.Gateways
{
    public class Sanlam_iTrade_Brokerage_Gateway : IBrokerageGateway
    {
        #region Public Properties 

        public BrokerageTypes BrokerageType
        {
            get
            {
                return BrokerageTypes.Sanlam_iTrade;
            }
        }

        #endregion

        #region Public Methods

        public bool VerifyUser(BrokerageAccount brokerageAccount, out List<TradingAccount> tradingAccounts)
        {
            tradingAccounts = new List<TradingAccount>();

            using (var service = new Sanlam_iTradeService.FundXchange())
            {
                DataTable dt = service.VerifyUser(brokerageAccount.Username, brokerageAccount.Password);
                if (dt == null)
                {
                    return false;
                }
                foreach (DataRow row in dt.Rows) 
                {
                    string accountNumber = row["AccCde"].ToString();
                    TradingAccount tradingAccount = new TradingAccount
                    {
                        AccountNumber = accountNumber,
                        BrokerageAccount = brokerageAccount
                    };

                    tradingAccounts.Add(tradingAccount);
                }
            }

            return true;
        }

        public bool PlaceOrder(TradingAccount tradingAccount, MarketOrder order, out string orderReferenceNumber)
        {
            orderReferenceNumber = "";
            using (var service = new Sanlam_iTradeService.FundXchange())
            {
                string side = "b";
                if (order.OrderSide == OrderSide.Sell)
                    side = "c";

                DataTable dt = service.PlaceOrder(tradingAccount.BrokerageAccount.Username, tradingAccount.AccountNumber, order.Symbol,
                    side, (double)order.Price, (int)order.Volume, order.ExpiryDate);

                if (dt == null)
                {
                    return false;
                }

                orderReferenceNumber = dt.Rows[0][0].ToString();
                order.OrderReferenceNumber = orderReferenceNumber;
            }
            return true;
        }

        public bool UpdateOrder(TradingAccount tradingAccount, string orderReferenceNumber, long price, int quantity)
        {
            return false;
        }

        public OrderStatusTypes GetOrderStatus(TradingAccount tradingAccount, string orderReferenceNumber)
        {
            //do service call

            return OrderStatusTypes.Awaiting_Authorization;
        }

        public bool CancelOrder(TradingAccount tradingAccount, string orderReferenceNumber)
        {
            using (var service = new Sanlam_iTradeService.FundXchange())
            {
                DataTable dt = service.CancelOrder(tradingAccount.AccountNumber, orderReferenceNumber);
                if (dt != null)
                    return true;
            }
            return false;
        }

        public Portfolio GetPortfolio(string portfolioName)
        {
            //do service call
            return new Portfolio();
        }

        public List<MarketOrder> GetOrders(TradingAccount tradingAccount)
        {
            List<MarketOrder> orders = new List<MarketOrder>();

            using (var service = new Sanlam_iTradeService.FundXchange())
            {
                DataTable dt = service.GetOrders(tradingAccount.AccountNumber);

                foreach (DataRow row in dt.Rows)
                {
                    MarketOrder order = GetOrderFromDataRow(row);
                    orders.Add(order);
                }
            }

            return orders;
        }

        #endregion

        #region Private Methods

        private static MarketOrder GetOrderFromDataRow(DataRow row)
        {
            MarketOrder order = new MarketOrder();

            order.ExpiryDate = Convert.ToDateTime(row["EXP_DATE"].ToString());
            order.Symbol = row["INSTR_CODE"].ToString();

            order.OrderDetail = new OrderDetail();
            order.OrderDetail.MatchedToday = Convert.ToInt32(row["MATCHED_TODAY"].ToString()); 

            order.Price = Convert.ToInt64(row["PRICE"].ToString());
            order.Volume = Convert.ToInt64(row["QUANTITY"].ToString());

            string side = row["SIDE"].ToString();
            order.OrderSide = GetOrderSide(side);

            string status = row["STATUS"].ToString();
            order.OrderStatus = GetOrderStatus(status);

            order.OrderDetail.VWAP = Convert.ToInt32(row["VWAP"].ToString()); //done
            
            return order;
        }

        private static OrderSide GetOrderSide(string side)
        {
            if (side == "b")
                return OrderSide.Buy;
            else
                return OrderSide.Sell;
        }

        private static OrderStatusTypes GetOrderStatus(string status)
        {
            switch (status)
            {
                case "O":
                    return OrderStatusTypes.Open;
                case "X":
                    return OrderStatusTypes.Cancelled;
                case "F":
                    return OrderStatusTypes.Partially_Filled;
                case "PX":
                    return OrderStatusTypes.Pending_Cancel;
                case "A":
                    return OrderStatusTypes.Awaiting_Authorization;
                default:
                    return OrderStatusTypes.Awaiting_Authorization;
            }
        }

        #endregion
    }
}
