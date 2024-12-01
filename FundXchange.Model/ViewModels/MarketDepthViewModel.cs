using System;
using System.Collections.Generic;
using System.ComponentModel;
using FundXchange.Domain.Entities;
using FundXchange.Model.ViewModels.MarketDepth;

namespace FundXchange.Model.ViewModels
{
    public class MarketDepthViewModel
    {
        #region Types

        public class SymbolTable
        {
            public const int SYMBOL_COLUMN = 0;
            public const int BID_TICK_COLUMN = 1;
            public const int LAST_COLUMN = 2;
            public const int NET_CHANGE_COLUMN = 3;
            public const int OPEN_COLUMN = 4;
            public const int HIGH_COLUMN = 5;
            public const int LOW_COLUMN = 6;
            public const int CLOSE_COLUMN = 7;
            public const int TRADE_SIZE_COLUMN = 8;
            public const int PRICE_INTERVALS_COLUMN = 9;
        }

        #endregion

        #region Public Properties

        public Instrument Instrument { get; internal set; }
        public List<Instrument> SymbolWatchList { get; set; }

        public List<Order> BuyOrders { get; internal set; }
        public List<Order> SellOrders { get; internal set; }
        public List<Bid> Bids { get; internal set; }
        public List<Offer> Offers { get; internal set; }
        public BindingList<BidOfferCross> BidOfferCrossBinding { get; internal set; }

        #endregion

        #region Constructors

        public MarketDepthViewModel()
        {
            Instrument = new Instrument();
            SymbolWatchList = new List<Instrument>();
            BuyOrders = new List<Order>();
            SellOrders = new List<Order>();
            Bids = new List<Bid>();
            Offers = new List<Offer>();
            BidOfferCrossBinding = new BindingList<BidOfferCross>();
        }

        #endregion
    }
}
