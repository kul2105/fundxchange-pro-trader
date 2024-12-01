using System;
using System.Collections.Generic;
using FundXchange.Orders.Entities;
using FundXchange.Orders.FX_OrderService;

namespace FundXchange.Orders.OrderAPI
{
    public interface IOrderAPI
    {
        string AddTradingAccount(string accountName);
        void RemoveTradingAccount(string accountNumber, string username, string password);
        List<TradingAccount> GetTradingAccounts(string username, string password);

        string PlaceMarketOrder(TradingAccount tradingAccount, string exchange, string symbol, OrderSide orderSide, int price, int quantity, 
            DateTime? expiryDate);
        void UpdateMarketOrder(TradingAccount tradingAccount, string orderId, int price, int quantity, DateTime? expiryDate);
        void MarketOrderExpired(TradingAccount tradingAccount, string orderId);
        void MarketOrderMatched(TradingAccount tradingAccount, string orderId, int quantity);
        void CancelMarketOrder(TradingAccount tradingAccount, string orderId);
        void DeleteMarketOrder(TradingAccount tradingAccount, string orderId);
        Dictionary<string, MarketOrder> GetMarketOrders(TradingAccount tradingAccount);

        string AddStopLossOrder(TradingAccount tradingAccount, string exchange, string symbol, int triggerPrice, int stopLossPrice, int cancelPrice,
            PriceTypes priceType, int quantity, OrderSide orderSide, DateTime? expiryDate);
        void RemoveStopLossOrder(string orderId);
        void UpdateStopLossOrder(string orderId, int quantity, int triggerPrice, int stopLossPrice, int cancelPrice, PriceTypes priceType,
            DateTime? expiryDate);
        string UpgradeStopLossOrderToMarketOrder(TradingAccount tradingAccount, string orderId, int price, int quantity);
        Dictionary<string, StopLossOrder> GetStopLossOrders(TradingAccount tradingAccount);

        int GetCashOnHand();
        int GetStartingBalance();

        //FinSwitch Methods
        int UploadFile(string loginID, string pswd, string fileName, string CompanyCode);
        string GetProcessStatus(string loginID, string pswd, int processID);
    }
}
