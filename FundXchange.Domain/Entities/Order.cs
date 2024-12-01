using System;
using FundXchange.Domain.Base;
using System.ComponentModel;

namespace FundXchange.Domain.Entities
{
    public enum OrderSide
    {
        Buy,
        Sell
    }

    public class Order : InstrumentBase, INotifyPropertyChanged
    {
        private string id;
        private double price;
        private long volume;
        private DateTime time;
        private OrderSide side;

        public long SequenceNumber { get; set; }

        public string Id 
        {
            get
            {
                return this.id;
            }
            set
            {
                if (value != this.id)
                {
                    this.id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }
        public double Price
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
        public long Volume
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
        public DateTime Time
        {
            get
            {
                return this.time;
            }
            set
            {
                this.time = value;
                NotifyPropertyChanged("Time");
            }
        }
        public OrderSide Side
        {
            get
            {
                return this.side;
            }
            set
            {
                this.side = value;
                NotifyPropertyChanged("Side");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
