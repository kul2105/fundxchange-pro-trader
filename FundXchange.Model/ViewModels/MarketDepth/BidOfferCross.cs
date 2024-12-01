using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FundXchange.Model.ViewModels.MarketDepth
{
    public class BidOfferCross : INotifyPropertyChanged
    {
        #region Private Members

        private string price;
        private string depth;
        private string volume;
        private string spread;

        private const string STRING_FORMAT = "{0} * {1}";

        #endregion

        #region Properties

        public string Price
        {
            get
            {
                return this.price;
            }
            set
            {
                if (value != this.price)
                {
                    this.price = value;
                    NotifyPropertyChanged("Price");
                }
            }
        }
        public string Depth
        {
            get
            {
                return this.depth;
            }
            set
            {
                if (value != this.depth)
                {
                    this.depth = value;
                    NotifyPropertyChanged("Depth");
                }
            }
        }
        public string Volume
        {
            get 
            { 
                return this.volume; 
            }
            set 
            {
                if (value != this.volume)
                {
                    this.volume = value;
                    NotifyPropertyChanged("Volume");
                }
            }
        }
        public string Spread
        {
            get
            {
                return this.spread;
            }
            set
            {
                if (value != this.spread)
                {
                    this.spread = value;
                    NotifyPropertyChanged("Spread");
                }
            }
        }

        #endregion

        #region Constructors

        public BidOfferCross()
        {   }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler  PropertyChanged;

        #endregion

        #region Private Methods

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Internal Methods

        internal void Update(Domain.Entities.Bid bid, Domain.Entities.Offer offer)
        {
            UpdatePrice(bid.Price, offer.Price);
            UpdateDepth(bid.OrderCount, offer.OrderCount);
            UpdateVolume(bid.Size, offer.Size);
            UpdateSpread(bid.Price, offer.Price);
        }

        #endregion

        #region Methods

        void UpdatePrice(long bidPrice, long offerPrice)
        {
            this.Price = string.Format(STRING_FORMAT, bidPrice.ToString(), offerPrice.ToString());
        }

        void UpdateDepth(int bidOrderCount, int offerOrderCount)
        {
            this.Depth = string.Format(STRING_FORMAT, bidOrderCount.ToString(), offerOrderCount.ToString());
        }

        void UpdateVolume(long bidSize, long offerSize)
        {
            this.Volume = string.Format(STRING_FORMAT, bidSize.ToString(), offerSize.ToString());
        }

        void UpdateSpread(long bidPrice, long offerPrice)
        {
            this.Spread = Math.Abs(bidPrice - offerPrice).ToString();
        }

        #endregion
    }
}
