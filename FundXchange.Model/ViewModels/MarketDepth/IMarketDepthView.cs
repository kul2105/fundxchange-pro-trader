using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FundXchange.Domain.Entities;
using FundXchange.Model.Agents;

namespace FundXchange.Model.ViewModels.MarketDepth
{
    public interface IMarketDepthView
    {
        void ProcessInstrument(Instrument instrument);
        void UpdateInstrumentTable(Instrument instrument);
        void ResetBidAndOfferGrids();
        void UpdateSymbol();
        void ProcessOrderbookAdd(OrderbookItem order);
        void ProcessOrderbookUpdate(OrderbookItem order);
        void ProcessOrderbookDelete(OrderbookItem order);
        void ProcessDepthByPriceChanged(List<DepthByPrice> bids, List<DepthByPrice> offers);
        void ProcessOrderbookInitEvent(List<DepthByPrice> bids, List<DepthByPrice> offers);
    }
}
