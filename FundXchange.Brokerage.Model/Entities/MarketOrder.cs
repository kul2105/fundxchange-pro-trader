using System;
using FundXchange.Brokerage.Model.Enumerations;
using FundXchange.Brokerage.Model.ValueObjects;

namespace FundXchange.Brokerage.Model.Entities
{
    public class MarketOrder : Order
    {
        public OrderSide OrderSide { get; set; }
        public OrderStatusTypes OrderStatus { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string OrderReferenceNumber { get; set; }
        public OrderDetail OrderDetail { get; set; }
    }
}
