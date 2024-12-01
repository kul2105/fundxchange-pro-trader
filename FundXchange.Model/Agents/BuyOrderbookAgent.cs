using System;
using System.Linq;
using FundXchange.Domain.Entities;

namespace FundXchange.Model.Agents
{
    public class BuyOrderbookAgent : AbstractOrderbookAgent
    {
        public delegate void BuyOrderbookItemAdded(OrderbookItem order);
        public delegate void BuyOrderbookItemUpdated(OrderbookItem order);
        public delegate void BuyOrderbookItemDeleted(OrderbookItem order);

        public event BuyOrderbookItemAdded BuyOrderbookItemAddedEvent;
        public event BuyOrderbookItemUpdated BuyOrderbookItemUpdatedEvent;
        public event BuyOrderbookItemDeleted BuyOrderbookItemDeletedEvent;

        public delegate void BidItemAdded(MarketByPrice marketByPrice);
        public delegate void BidItemUpdated(MarketByPrice marketByPrice);
        public delegate void BidItemDeleted(MarketByPrice marketByPrice);

        public event BidItemAdded BidItemAddedEvent;
        public event BidItemUpdated BidItemUpdatedEvent;
        public event BidItemDeleted BidItemDeletedEvent;

        public BuyOrderbookAgent(string symbol, string exchange, int depth) : base(symbol, exchange, depth)
        {
        }

        protected override MarketByPrice GetLastItem()
        {
            MarketByPrice lastItem = _MarketByPriceItems.Values.First();
            return lastItem;
        }

        #region Raise Event Helpers

        protected override void RaiseOrderbookItemAddedEvent(OrderbookItem order)
        {
            if (BuyOrderbookItemAddedEvent != null)
            {
                BuyOrderbookItemAddedEvent(order);
            }
        }

        protected override void RaiseOrderbookItemUpdatedEvent(OrderbookItem order)
        {
            if (BuyOrderbookItemUpdatedEvent != null)
            {
                BuyOrderbookItemUpdatedEvent(order);
            }
        }

        protected override void RaiseOrderbookItemDeletedEvent(OrderbookItem order)
        {
            if (BuyOrderbookItemDeletedEvent != null)
            {
                BuyOrderbookItemDeletedEvent(order);
            }
        }

        protected override void RaiseMarketByPriceItemAddedEvent(MarketByPrice marketByPrice)
        {
            if (BidItemAddedEvent != null)
            {
                BidItemAddedEvent(marketByPrice);
            }
        }

        protected override void RaiseMarketByPriceUpdatedEvent(MarketByPrice marketByPrice)
        {
            if (BidItemUpdatedEvent != null)
            {
                BidItemUpdatedEvent(marketByPrice);
            }
        }

        protected override void RaiseMarketByPriceDeletedEvent(MarketByPrice marketByPrice)
        {
            if (BidItemDeletedEvent != null)
            {
                BidItemDeletedEvent(marketByPrice);
            }
        }

        #endregion
    }
}
