using System;
using FundXchange.Domain.Base;

namespace FundXchange.Domain.Entities
{
    public enum OrderEventType
    {
        Add,
        Update,
        Delete
    }

    public class OrderEvent
    {
        public OrderEventType Operation { get; set; }
        public OrderbookItem Order { get; set; }
    }
}
