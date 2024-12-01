using System;

namespace M4.DataManagerItems
{
    public class SymbolCache
    {
        public string Symbol { get; set; }
        public double Price { get; set; }
        public SymbolCache(string symbol, double price)
        {
            Symbol = symbol;
            Price = price;
        }
    }
}
