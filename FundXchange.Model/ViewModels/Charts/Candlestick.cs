using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.ViewModels.Charts;

namespace FundXchange.Model.ViewModels.Charts
{
    public delegate void CandleStickClosedDelegate(Candlestick candleStick);

    public class Candlestick
    {
        public Candlestick(string symbol, double openingPrice)
        {
            Symbol = symbol;
            Open = openingPrice;
            High = openingPrice;
            Low = openingPrice;
            Close = openingPrice;
            LastTraded = openingPrice;
        }

        private Timer _UpdateTimer;

        public string Symbol { get; private set; }
        public double Open { get; private set; }
        public double Close { get; private set; }
        public double High { get; private set; }
        public double Low { get; private set; }
        public long Volume { get; private set; }
        public double LastTraded { get; private set; }
        public DateTime CloseDate { get; private set; }
        public event CandleStickClosedDelegate Closed;

        public void SetTimer(int interval)
        {
            _UpdateTimer = new Timer(interval);
            _UpdateTimer.Elapsed += UpdateTimer_Elapsed;
            _UpdateTimer.Start();
        }

        public void UpdateWith(Trade trade)
        {
            LastTraded = trade.Price;
            Volume += trade.Volume;

            if (trade.Price > this.High)
                this.High = trade.Price;
            if (trade.Price < this.Low)
                this.Low = trade.Price;
        }

        void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CloseDate = e.SignalTime;
            Close = LastTraded;
            
            if (null != Closed)
                Closed(this.Copy());
        }

        private Candlestick Copy()
        {
            return new Candlestick(this.Symbol, this.LastTraded);
        }
    }
}
