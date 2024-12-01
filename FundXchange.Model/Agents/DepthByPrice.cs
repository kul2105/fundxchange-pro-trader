using System;
using System.Collections.Generic;
using FundXchange.Domain.Entities;

namespace FundXchange.Model.Agents
{
    public class DepthByPrice : ICloneable
    {
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        //public long Price { get; set; }
        public double Price { get; set; }

        public long Volume { get; set; }
        public List<OrderbookItem> Orders { get; private set; }
        public string Condition { get; set; }

        public DepthByPrice(string symbol, string exchange)
        {
            Symbol = symbol;
            Exchange = exchange;
            Orders = new List<OrderbookItem>();
        }

        public object Clone()
        {
            DepthByPrice clone= new DepthByPrice( Symbol, Exchange);
            clone.Price = Price;
            clone.Volume = Volume;

            clone.Orders= new List<OrderbookItem>();
            foreach (OrderbookItem orderbookItem in Orders)
            {
                clone.Orders.Add((OrderbookItem)orderbookItem.Clone());
            }

            return clone;

        }
    }
}
