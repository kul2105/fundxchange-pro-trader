using System;
using System.Linq;
using FundXchange.Domain.Entities;

namespace FundXchange.Model.Agents
{
    public class SellOrderbookAgent : AbstractOrderbookAgent
    {
        public delegate void SellOrderbookItemAdded(OrderbookItem order);
        public delegate void SellOrderbookItemUpdated(OrderbookItem order);
        public delegate void SellOrderbookItemDeleted(OrderbookItem order);

        public event SellOrderbookItemAdded SellOrderbookItemAddedEvent;
        public event SellOrderbookItemUpdated SellOrderbookItemUpdatedEvent;
        public event SellOrderbookItemDeleted SellOrderbookItemDeletedEvent;

        public delegate void OfferItemAdded(MarketByPrice marketByPrice);
        public delegate void OfferItemUpdated(MarketByPrice marketByPrice);
        public delegate void OfferItemDeleted(MarketByPrice marketByPrice);

        public event OfferItemAdded OfferItemAddedEvent;
        public event OfferItemUpdated OfferItemUpdatedEvent;
        public event OfferItemDeleted OfferItemDeletedEvent;

        public SellOrderbookAgent(string symbol, string exchange, int depth) : base(symbol, exchange, depth)
        {
        }

        protected override MarketByPrice GetLastItem()
        {
            MarketByPrice lastItem = _MarketByPriceItems.Values.Last();
            return lastItem;
        }

        #region Raise Event Helpers

        protected override void RaiseOrderbookItemAddedEvent(OrderbookItem order)
        {
            if (SellOrderbookItemAddedEvent != null)
            {
                SellOrderbookItemAddedEvent(order);
            }
        }

        protected override void RaiseOrderbookItemUpdatedEvent(OrderbookItem order)
        {
            if (SellOrderbookItemUpdatedEvent != null)
            {
                SellOrderbookItemUpdatedEvent(order);
            }
        }

        protected override void RaiseOrderbookItemDeletedEvent(OrderbookItem order)
        {
            if (SellOrderbookItemDeletedEvent != null)
            {
                SellOrderbookItemDeletedEvent(order);
            }
        }

        protected override void RaiseMarketByPriceItemAddedEvent(MarketByPrice marketByPrice)
        {
            if (OfferItemAddedEvent != null)
            {
                OfferItemAddedEvent(marketByPrice);
            }
        }

        protected override void RaiseMarketByPriceUpdatedEvent(MarketByPrice marketByPrice)
        {
            if (OfferItemUpdatedEvent != null)
            {
                OfferItemUpdatedEvent(marketByPrice);
            }
        }

        protected override void RaiseMarketByPriceDeletedEvent(MarketByPrice marketByPrice)
        {
            if (OfferItemDeletedEvent != null)
            {
                OfferItemDeletedEvent(marketByPrice);
            }
        }

        #endregion

    }
}
