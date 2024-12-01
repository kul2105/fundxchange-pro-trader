using System;
using System.ServiceModel;
using System.Collections.Generic;

namespace FundXchange.Orders.OrderProviderService
{
    // NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in App.config.
    [ServiceContract]
    public interface IOrderService
    {
        [OperationContract]
        string InsertTradingAccount(int userId);

        [OperationContract]
        void DeleteTradingAccount(int tradingAccountId);

        [OperationContract]
        Dictionary<int, string> GetTradingAccounts(int userId);

        [OperationContract]
        string PlaceOrder(OrderDTO orderDTO);

        [OperationContract]
        string UpdateOrder(OrderDTO orderDTO);

        [OperationContract]
        void DeleteOrder(int orderId);

        [OperationContract]
        List<OrderDTO> GetOrders(int userId);
    }
}
