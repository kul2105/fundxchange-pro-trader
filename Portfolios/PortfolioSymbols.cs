using System;
using M4.DataManagerItems;

namespace M4.Portfolios
{
    public class PortfolioSymbols
    {
        public string m_sname;
        public Double m_ask;
        public Double m_bid;
        public Double m_volume;
        public DateTime m_date;
        public Double m_price;

        public PortfolioSymbols(string name)
        {
            m_ask = 0;
            m_bid = 0;
            m_volume = 0;
            m_price = 1;
            m_date = DateTime.Now;
            m_sname = name;
        }

        public void update(PriceUpdates pu)
        {
            m_sname = pu.Symbol;
            m_ask = pu.Ask;
            m_bid = pu.Bid;
            m_price = pu.Price;
            m_volume = pu.Volume;
            m_date = pu.TradeDateTime;
        }
    }
}
