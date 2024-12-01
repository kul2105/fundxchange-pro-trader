using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FundXchange.Model.ViewModels.Charts;

namespace M4.ClientData
{
    [Serializable]
    public class ClientQuotes : BarData
    {
        private double _openInterest;
        public double OpenInterest
        {
            get { return _openInterest; }
            set { _openInterest = value; }
        }
        //private string _Symbol;
        //public string Symbol
        //{
        //    get { return _Symbol; }
        //    set { _Symbol = value; }
        //}
        public ClientQuotes()
        {
            TradeDateTime = DateTime.Now;
            StartDateTime = DateTime.Now;
            CloseSequenceNumber = 0;
            CloseDateTime = DateTime.Now;
            ClosePrice = 0.0;
            OpenPrice = 0.0;
            HighPrice = 0.0;
            LowPrice = 0.0;
            Volume = 0.0;
            _openInterest = 0.0;

        }
    }
}
