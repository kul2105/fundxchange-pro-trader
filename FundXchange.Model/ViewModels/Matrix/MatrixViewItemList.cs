using System;
using System.Collections.Generic;
using System.Linq;
using FundXchange.Domain.ValueObjects;
using FundXchange.Domain.Entities;
using FundXchange.Model.Agents;
using System.Drawing;
using FundXchange.Domain.Enumerations;

namespace FundXchange.Model.ViewModels.Matrix
{
    public delegate void MatrixViewItemChangedDelegate(List<MatrixViewItem> items);

    public interface IMatrixViewItemList
    {
        event MatrixViewItemChangedDelegate ItemsAdded;
        event MatrixViewItemChangedDelegate ItemsUpdated;
        event MatrixViewItemChangedDelegate ItemsRemoved;
        event MatrixViewItemChangedDelegate DepthInitialized;
        void AddInstrument(Instrument instrument);
        void AddTrades(List<Trade> trades);
        void InitializeDepth(List<DepthByPrice> bids, List<DepthByPrice> offers);
        void UpdateDepth(DepthByPrice depth, bool isBuy);
        //void RemoveDepth(long price, bool isBuy); John
        //void RemoveDepth(double price, bool isBuy);
        void RemoveDepth(double price, string condition);
        void ClearItems();
        long GetTotalTradeVolume();
        long GetTotalBidSize();
        long GetTotalOfferSize();
        IList<MatrixViewItem> GetItems();
    }
    public class MatrixViewItemList : SortedList<double, MatrixViewItem>, IMatrixViewItemList
    {
        public event MatrixViewItemChangedDelegate ItemsAdded;
        public event MatrixViewItemChangedDelegate ItemsUpdated;
        public event MatrixViewItemChangedDelegate ItemsRemoved;
        public event MatrixViewItemChangedDelegate DepthInitialized;

        private double _highPrice = -1;
        private double _lowPrice = -1;
        private double _lastTrade = -1;
        private long _totalTradeVolume = 0;
        private long _totalBidSize = 0;
        private long _totalOfferSize = 0;
        private bool _instrumentAdded;

        private double tickSize = -1;
        //private DateTime currentTime = DateTime.UtcNow.AddHours(2);
        private DateTime currentTime = new DateTime(DateTime.UtcNow.AddHours(2).Year, DateTime.UtcNow.AddHours(2).Month, DateTime.UtcNow.AddHours(2).Day, 9, 15, 0);
        public static Color OutOfRangeColor = Color.Gray;

        public long GetTotalTradeVolume()
        {
            return _totalTradeVolume;
        }
        public long GetTotalBidSize()
        {
            return _totalBidSize;
        }
        public long GetTotalOfferSize()
        {
            return _totalOfferSize;
        }
        public void AddInstrument(Instrument instrument)
        {
            if (_instrumentAdded)
                throw new ApplicationException("Instrument already added to this list.");

            if (instrument != null)
            {
                var itemsAdded = new List<MatrixViewItem>();
                var itemsUpdated = new List<MatrixViewItem>();
                _totalTradeVolume = 0;
                _totalBidSize = 0;
                _totalOfferSize = 0;
                
                //By John
                //AddUpdateDefaultPrice(instrument.Open, ref itemsAdded, ref itemsUpdated);
                //AddUpdateDefaultPrice(instrument.YesterdayClose, ref itemsAdded, ref itemsUpdated);
                //AddUpdateDefaultPrice(instrument.High, ref itemsAdded, ref itemsUpdated);
                //AddUpdateDefaultPrice(instrument.Low, ref itemsAdded, ref itemsUpdated);
                //AddUpdateDefaultPrice(instrument.LastTrade, ref itemsAdded, ref itemsUpdated);

                AddUpdateDefaultPrice(instrument.Open, ref itemsAdded, ref itemsUpdated);
                AddUpdateDefaultPrice(instrument.YesterdayClose, ref itemsAdded, ref itemsUpdated);
                AddUpdateDefaultPrice(instrument.High, ref itemsAdded, ref itemsUpdated);
                AddUpdateDefaultPrice(instrument.Low, ref itemsAdded, ref itemsUpdated);
                AddUpdateDefaultPrice(instrument.LastTrade, ref itemsAdded, ref itemsUpdated);

                _highPrice = instrument.High;
                _lowPrice = instrument.Low;
                _instrumentAdded = true;
                //tickSize=100/Math.Pow(100

                RaiseItemsAdded(itemsAdded);
                RaiseItemsUpdated(itemsUpdated);
            }
        }

        private void RaiseItemsAdded(List<MatrixViewItem> items)
        {
            if (items.Count > 0)
            {
                if (ItemsAdded != null)
                    ItemsAdded(items);
            }
        }

        private void RaiseItemsUpdated(List<MatrixViewItem> items)
        {
            if (items.Count > 0)
            {
                if (ItemsUpdated != null)
                    ItemsUpdated(items);
            }
        }

        private void AddUpdateDefaultPrice(double price, ref List<MatrixViewItem> itemsAdded, ref List<MatrixViewItem> itemsUpdated)
        {
            if (!this.ContainsKey(price))
            {
                MatrixViewItem item = new MatrixViewItem();
                item.Price = price;
                this.Add(price, item);
                itemsAdded.Add(item);
            }
            else
            {
                itemsUpdated.Add(this[price]);
            }
        }

        public void AddTrades(List<Trade> trades)
        {
            var itemsAdded = new List<MatrixViewItem>();
            var pricesAdded = new List<double>();
            var pricesUpdated = new List<double>();

            foreach (Trade trade in trades)
            {
                if (!this.ContainsKey(trade.Price))
                {
                    var item = new MatrixViewItem {Price = trade.Price, TradeVolume = trade.Volume,TimeTradeVolume = new List<long>()};
                    if (trade.TradeStatus == TradeStatus.AtBid)
                        item.TradeAtBidSize += trade.Volume;
                    else if (trade.TradeStatus == TradeStatus.AtOffer)
                        item.TradeAtOfferSize += trade.Volume;
                    else if (trade.TradeStatus == TradeStatus.BetweenBidAndOffer)
                        item.TradeAtBetSize += trade.Volume;
                    else
                    { }
                    if (trade.TimeStamp<=currentTime)
                    {
                        item.TimeTradeVolume.Add(trade.Volume);
                    }
                    else
                    {
                        int n = ((int)(trade.TimeStamp - currentTime).TotalMinutes / 15) + 1;
                        for (int i = 0; i < n; i++)
                        {
                            item.TimeTradeVolume.Add(0);
                        }
                        item.TimeTradeVolume.Add(trade.Volume);
                         
                    }
                    UpdateHighPrice(trade.Price, ref pricesUpdated);
                    UpdateLowPrice(trade.Price, ref pricesUpdated);
                    UpdateLastTrade(trade.Price, ref pricesUpdated);
                    _totalTradeVolume += trade.Volume;
                    this.Add(trade.Price, item);
                    UpdateOutOfRangeColor(item);
                    itemsAdded.Add(item);
                    pricesAdded.Add(item.Price);

                }
                else
                {
                    this[trade.Price].TradeVolume += trade.Volume;
                    if (trade.TradeStatus == TradeStatus.AtBid)
                        this[trade.Price].TradeAtBidSize += trade.Volume;
                    else if (trade.TradeStatus == TradeStatus.AtOffer)
                        this[trade.Price].TradeAtOfferSize += trade.Volume;
                    else if (trade.TradeStatus == TradeStatus.BetweenBidAndOffer)
                        this[trade.Price].TradeAtBetSize += trade.Volume;
                    else
                    { }
                    if (trade.TimeStamp <= currentTime)
                    {
                        if (this[trade.Price].TimeTradeVolume == null)
                        {
                            this[trade.Price].TimeTradeVolume = new List<long>();
                            this[trade.Price].TimeTradeVolume.Add(0);
                        }
                        else if (this[trade.Price].TimeTradeVolume.Count == 0)
                        {
                            this[trade.Price].TimeTradeVolume.Add(0);
                        }
                        this[trade.Price].TimeTradeVolume[0] += trade.Volume;
                    }
                    else
                    {
                        int n = ((int)(trade.TimeStamp - currentTime).TotalMinutes / 15) + 1;
                        if (this[trade.Price].TimeTradeVolume == null)
                        {
                            this[trade.Price].TimeTradeVolume = new List<long>();
                            this[trade.Price].TimeTradeVolume.Add(0);
                        }
                        else if (this[trade.Price].TimeTradeVolume.Count == 0)
                        {
                            this[trade.Price].TimeTradeVolume.Add(0);
                        }
                        for (int i = this[trade.Price].TimeTradeVolume.Count-1; i <n; i++)
                        {
                            this[trade.Price].TimeTradeVolume.Add(0);
                        }
                        this[trade.Price].TimeTradeVolume[n] += trade.Volume;
                    }
                    _totalTradeVolume = trade.Volume += trade.Volume;
                    UpdateLastTrade(trade.Price, ref pricesUpdated);
                    UpdateOutOfRangeColor(this[trade.Price]);
                    pricesUpdated.Add(trade.Price);
                }
            }
           
            //for (double i = _lowPrice; i <= _highPrice; i=i+500)//i=i+.25)
            //{
            //    lock (this)
            //    {
            //        if (!this.ContainsKey(i))
            //        {
            //            var item = new MatrixViewItem
            //                           {
            //                               BidOrderCount = 0,
            //                               BidSize = 0,
            //                               OfferOrderCount = 0,
            //                               OfferSize = 0,
            //                               TradeVolume = 0,
            //                               TradeAtOfferSize = 0,
            //                               TradeAtBidSize = 0,
            //                               TradeAtBetSize = 0,
            //                               TimeTradeVolume = new List<long>(),
            //                               Price = i
            //                           };

            //            UpdateOutOfRangeColor(item);

            //            this.Add(i, item);
            //            itemsAdded.Add(item);
            //            pricesAdded.Add(item.Price);
            //        }
            //        else
            //        {
 
            //        }
            //    }
            //}
            
            for (int i = 0; i < itemsAdded.Count; i++)
            {
                itemsAdded[i].BidOrderCount = this[itemsAdded[i].Price].BidOrderCount;
                itemsAdded[i].BidSize = this[itemsAdded[i].Price].BidSize;
                itemsAdded[i].OfferOrderCount = this[itemsAdded[i].Price].OfferOrderCount;
                itemsAdded[i].OfferSize = this[itemsAdded[i].Price].OfferSize;
                itemsAdded[i].TradeVolume = this[itemsAdded[i].Price].TradeVolume;
                itemsAdded[i].TradeAtBetSize = this[itemsAdded[i].Price].TradeAtBetSize;
                itemsAdded[i].TradeAtBidSize = this[itemsAdded[i].Price].TradeAtBidSize;
                itemsAdded[i].TradeAtOfferSize = this[itemsAdded[i].Price].TradeAtOfferSize;
                itemsAdded[i].TimeTradeVolume = this[itemsAdded[i].Price].TimeTradeVolume;
            }
            
            List<double> pricesNeedingUpdate = pricesUpdated.Distinct().Where(i => !pricesAdded.Contains(i)).ToList();

            List<MatrixViewItem> itemsToUpdate = new List<MatrixViewItem>();
            foreach (double price in pricesNeedingUpdate)
            {
                itemsToUpdate.Add(this[price]);
            }
            RaiseItemsAdded(itemsAdded);
            RaiseItemsUpdated(itemsToUpdate);
        }

        private void UpdateHighPrice(double price, ref List<double> itemsUpdated)
        {
            if (price > _highPrice)
            {
                if (this.ContainsKey(_highPrice))
                {
                    itemsUpdated.Add(_highPrice);
                }
                _highPrice = price;
            }
        }

        private void UpdateLowPrice(double price, ref List<double> itemsUpdated)
        {
            if (price < _lowPrice || _lowPrice == -1)
            {
                if (this.ContainsKey(_lowPrice))
                {
                    itemsUpdated.Add(_lowPrice);
                }
                _lowPrice = price;
            }
        }

        private void UpdateLastTrade(double price, ref List<double> itemsUpdated)
        {
            if (this.ContainsKey(_lastTrade))
            {
                itemsUpdated.Add(_lastTrade);
            }
            _lastTrade = price;
        }

        public void InitializeDepth(List<DepthByPrice> bids, List<DepthByPrice> offers)
        {
            foreach (DepthByPrice depth in bids)
            {
                UpdateDepthItem(depth, true, false);
            }
            foreach (DepthByPrice depth in offers)
            {
                UpdateDepthItem(depth, false, false);
            }
            //AddDefaultValues();
            if (DepthInitialized != null)
                DepthInitialized(this.Values.ToList());
        }

        private void AddDefaultValues()
        {
            //fill in default values
            //var vals = this.GetItems();
            double diff = _highPrice - _lowPrice;
            for (double i = _lowPrice; i <= _highPrice; i+=1)//i++)
            {
                if (i!=0 && !this.ContainsKey(i))
                {
                    MatrixViewItem item = new MatrixViewItem();
                    item.BidOrderCount = 0;
                    item.BidSize = 0;
                    item.OfferOrderCount = 0;
                    item.OfferSize = 0;
                    item.TradeVolume = 0;
                    item.Price = i;
                    item.TradeAtBetSize = 0;
                    item.TradeAtBidSize = 0;
                    item.TradeAtOfferSize = 0;
                    item.TimeTradeVolume = new List<long>();
                    UpdateOutOfRangeColor(item);

                    this.Add(i, item);

                    //i = i + 0.25;
                }
            }
        }
        private void UpdateOutOfRangeColor(MatrixViewItem item)
        {
            if (item.Price > _highPrice)
            {
                item.PriceBackgroundColor = OutOfRangeColor;
            }
            else if (item.Price < _lowPrice)
            {
                item.PriceBackgroundColor = OutOfRangeColor;
            }
            else
            {
                item.PriceBackgroundColor = Color.Black;
            }
        }

        public void UpdateDepth(DepthByPrice depth, bool isBuy)
        {
            UpdateDepthItem(depth, isBuy, true);
        }

        private void UpdateDepthItem(DepthByPrice depth, bool isBuy, bool shouldRaiseEvent)
        {
            var pricesAdded = new List<double>();
            if (!this.ContainsKey(depth.Price))
            {
                MatrixViewItem item = new MatrixViewItem();
                if (isBuy)
                {
                    item.BidOrderCount = depth.Orders.Count;
                    item.BidSize = depth.Volume;
                    _totalBidSize += item.BidSize;
                }
                else
                {
                    item.OfferOrderCount = depth.Orders.Count;
                    item.OfferSize = depth.Volume;
                    _totalOfferSize += item.OfferSize;
                }

                //John
                item.Price = depth.Price;
                UpdateOutOfRangeColor(item);

                this.Add(item.Price, item);
                if (shouldRaiseEvent)
                    RaiseItemsAdded(new List<MatrixViewItem>() { item });
                //pricesAdded.Add(item.Price);
            }
            else
            {
                if (isBuy)
                {
                    _totalBidSize += depth.Volume - this[depth.Price].BidSize;
                    this[depth.Price].BidOrderCount = depth.Orders.Count;
                    this[depth.Price].BidSize = depth.Volume;
                }
                else
                {
                    //John
                   // _totalBidSize += depth.Volume - this[depth.Price].OfferSize;

                    _totalOfferSize += depth.Volume - this[depth.Price].OfferSize;
                    this[depth.Price].OfferOrderCount = depth.Orders.Count;
                    this[depth.Price].OfferSize = depth.Volume;
                }
                UpdateOutOfRangeColor(this[depth.Price]);
                if (shouldRaiseEvent)
                    RaiseItemsUpdated(new List<MatrixViewItem>() { this[depth.Price] });
            }
            RaiseItemsUpdated(new List<MatrixViewItem>() { this[depth.Price] });
            UpdateHighPrice(depth.Price, ref pricesAdded);
            UpdateLowPrice(depth.Price, ref pricesAdded);
        }

        public void RemoveDepth(double price, string cond /*bool isBuy*/)
        {
            if (!this.ContainsKey(price))
                return;
                //throw new ApplicationException("List does not contain specified price.");

            switch (cond)
            {
                case "BUY":
                    this[price].BidOrderCount = 0;
                    this[price].BidSize = 0;
                    break;
                case "SELL":
                    this[price].OfferOrderCount = 0;
                    this[price].OfferSize = 0;
                    break;
                case "Between":
                    this[price].BidOrderCount = 0;
                    this[price].BidSize = 0;
                    this[price].OfferOrderCount = 0;
                    this[price].OfferSize = 0;
                    break;
                default:
                    break;
            }
            RaiseItemsUpdated(new List<MatrixViewItem>() { this[price] });


            //if (isBuy)
            //{
            //    this[price].BidOrderCount = 0;
            //    this[price].BidSize = 0;

            //    RaiseItemsUpdated(new List<MatrixViewItem>() { this[price] });
            //}
            //else
            //{
            //    this[price].OfferOrderCount = 0;
            //    this[price].OfferSize = 0;

            //    RaiseItemsUpdated(new List<MatrixViewItem>() { this[price] });
            //}


            //only remove price if depth is zero and no trades at that price
            //if (this[price].TradeVolume == 0 && this[price].BidOrderCount == 0 && this[price].BidSize == 0 && this[price].OfferOrderCount == 0
            //    && this[price].OfferSize == 0)
            //{
            //    MatrixViewItem item = this[price];
            //    this.Remove(price);

            //    if (ItemsRemoved != null)
            //        ItemsRemoved(new List<MatrixViewItem>() { item });
            //}
            //else
            //{
            //    RaiseItemsUpdated(new List<MatrixViewItem>() { this[price] });
            //}
        }

        public IList<MatrixViewItem> GetItems()
        {
            return this.Values;
        }

        public void ClearItems()
        {
            _instrumentAdded = false;
            this.Clear();
        }
    }

}
