using System;
using FundXchange.Orders.Enumerations;

namespace FundXchange.Orders.Exceptions
{
    public class BrokerageNotFoundException : ApplicationException
    {
        public BrokerageNotFoundException(Brokerages brokerage)
            : base("Brokerage not found: " + brokerage)
        {
        }
    }
}
