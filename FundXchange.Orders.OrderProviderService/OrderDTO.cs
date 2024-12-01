using System;
using System.Runtime.Serialization;

namespace FundXchange.Orders.OrderProviderService
{
    [DataContract]
    public class OrderDTO
    {
        [DataMember]
        public int OrderId { get; set; }
        [DataMember]
        public int TradingAccountId { get; set; }
        [DataMember]
        public string Exchange { get; set; }
        [DataMember]
        public string Symbol { get; set; }
        [DataMember]
        public string OrderSide { get; set; }
        [DataMember]
        public int Price { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public DateTime ExpiryDate { get; set; }
        [DataMember]
        public string OrderStatus { get; set; }
        [DataMember]
        public string OrderType { get; set; }
        [DataMember]
        public DateTime LastModifiedDate { get; set; }
    }
}
