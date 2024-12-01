using System;
using System.Collections.Generic;
using FundXchange.Brokerage.Model.Entities;
using FundXchange.Brokerage.Model.Enumerations;
using FundXchange.Brokerage.Model.ValueObjects;

namespace FundXchange.Brokerage.Model.Gateways
{
    public interface IBrokerageGateway
    {
        BrokerageTypes BrokerageType { get; }
        bool VerifyUser(BrokerageAccount brokerageAccount, out List<TradingAccount> tradingAccounts);
        bool PlaceOrder(TradingAccount tradingAccount, MarketOrder order, out string orderReferenceNumber);
        bool UpdateOrder(TradingAccount tradingAccount, string orderReferenceNumber, long price, int quantity);
        OrderStatusTypes GetOrderStatus(TradingAccount tradingAccount, string orderReferenceNumber);
        bool CancelOrder(TradingAccount tradingAccount, string orderReferenceNumber);
        Portfolio GetPortfolio(string portfolioName);
        List<MarketOrder> GetOrders(TradingAccount tradingAccount);
    }
}

