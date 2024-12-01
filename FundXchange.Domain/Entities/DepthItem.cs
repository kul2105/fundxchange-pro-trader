using System;
using System.Collections.Generic;

namespace FundXchange.Domain.Entities
{
    public class DepthItem
    {
        public long Price { get; set; }
        public long Volume { get; set; }
        public List<Order> Orders { get; set; }
    }
}
