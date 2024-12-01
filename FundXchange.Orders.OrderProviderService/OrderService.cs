using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FundXchange.Orders.OrderProviderService
{
    // NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
    public class OrderService : IOrderService
    {
        private OrderRepository _Repository;

        public OrderService()
        {
            _Repository = new OrderRepository();
        }

        public string InsertTradingAccount(int userId)
        {
            throw new NotImplementedException();
        }

        public void DeleteTradingAccount(int tradingAccountId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, string> GetTradingAccounts(int userId)
        {
            throw new NotImplementedException();
        }

        public string PlaceOrder(OrderDTO orderDTO)
        {
            throw new NotImplementedException();
        }

        public string UpdateOrder(OrderDTO orderDTO)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public List<OrderDTO> GetOrders(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
