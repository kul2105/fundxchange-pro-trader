using System;
using FundXchange.Model.Infrastructure;
using FundXchange.Domain.ValueObjects;
using FundXchange.DataProviderContracts;
using FundXchange.Domain.Entities;
using FundXchange.Infrastructure;
using FundXchange.Domain;
using System.Collections.Generic;
using System.Windows.Forms;
using FundXchange.Model.ViewModels.Charts;
using FundXchange.Domain.Enumerations;

namespace FundXchange.Model.Controllers
{
    public class ChartController
    {
        private IMarketRepository _Repository;
        private string _Symbol;
        private string _Exchange;
        private readonly MessageHandler<object> _MessageHandler;

        public event TradeOccurredDelegate TradeOccurredEvent;
        public event InstrumentUpdatedDelegate InstrumentUpdatedEvent;
        public event HeartbeatMessageReceivedDelegate HeartBeatReceivedEvent;        

        public ChartController(string id)
        {
            _Repository = IoC.Resolve<IMarketRepository>();
            IoC.Resolve<ErrorService>();

            _MessageHandler = new MessageHandler<object>("Charts_" + id);
            _MessageHandler.MessageReceived += _MessageHandler_MessageReceived;
        }

        public List<JsonLibCommon.FinSwitchHD> GetMFEODWithDates(string symb, DateTime Startdt, DateTime EndDt, string pType)
        {
            return _Repository.GetEODWithDates(symb, Startdt, EndDt, pType);
        }
        public void StartReceive()
        {
            _MessageHandler.Start();
        }

        public void PauseReceive(bool resume)
        {
            if (_MessageHandler == null) return;

            if(resume)
                _MessageHandler.Start();
            else
                _MessageHandler.Pause();
        }

        public Instrument SubscribeSymbol(string symbol, string exchange)
        {
            _Exchange = exchange;
            _Symbol = symbol;

            if (!String.IsNullOrEmpty(symbol))
            {
                Instrument instrument = null;

                if (_Repository.IndexWatchList.ContainsKey(symbol))
                {
                    if (!_Repository.PortfolioWatchList.ContainsKey(symbol))
                        instrument = _Repository.SubscribeIndex(symbol, exchange);
                    else
                        instrument = _Repository.PortfolioWatchList[symbol];

                    instrument.IsIndex = true;

                    _Repository.IndexUpdatedEvent += _Repository_IndexUpdatedEvent;
                }
                else
                {
                    instrument = _Repository.SubscribeLevelOneWatch(symbol, exchange, false);

                    if (instrument != null)
                    instrument.IsIndex = false;

                    _Repository.TradeOccurredEvent += _Repository_TradeOccurredEvent;
                }

                _Repository.InstrumentUpdatedEvent += _Repository_InstrumentUpdatedEvent;
                _Repository.HeartbeatReceivedEvent += _Repository_HeartbeatReceivedEvent;

                return instrument;
            }
            return null;
        }

        public void Stop()
        {
            _MessageHandler.Dispose();
            _Repository.InstrumentUpdatedEvent -= _Repository_InstrumentUpdatedEvent;
            _Repository.TradeOccurredEvent -= _Repository_TradeOccurredEvent;
        }

        //public List<BarData> GetHistory(string symbol, Periodicity BarType, int BarSize, int BarCount)
        //{
        //    List<BarData> bars = new List<BarData>();
        //    if (BarSize > 500 || BarSize < 1) 
        //        return bars;

        //    int interval = BarSize;
        //    switch (BarType)
        //    {
        //        case Periodicity.Minutely:
        //            interval = interval * 1;
        //            break;
        //        case Periodicity.Hourly:
        //            interval = interval * 60;
        //            break;
        //        case Periodicity.Daily:
        //            interval = interval * 60 * 8;
        //            break;
        //        case Periodicity.Weekly:
        //            interval = interval * 60 * 8 * 5;
        //            break;
        //    }

        //    try
        //    {
        //        List<Candlestick> snapshots = _Repository.GetEquityCandlesticks(symbol, GlobalDeclarations.EXCHANGE, interval, BarCount);
        //        for (int i = snapshots.Count - 1; i >= 0; i--)
        //        {
        //            BarData bar = new BarData();

        //            bar.OpenPrice = snapshots[i].Open;
        //            bar.ClosePrice = snapshots[i].Close;
        //            bar.HighPrice = snapshots[i].High;
        //            bar.LowPrice = snapshots[i].Low;
        //            bar.TradeDateTime = snapshots[i].TimeOfClose;
        //            bar.StartDateTime = snapshots[i].TimeOfStart;
        //            bar.Volume = snapshots[i].Volume;

        //            bars.Add(bar);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error: " + ex.Message);
        //    }
        //    return bars;
        //}

        void _Repository_InstrumentUpdatedEvent(Instrument instrument)
        {
            if (instrument.Symbol == _Symbol )//Commented By John   //&& instrument.Exchange == _Exchange)
            {
                _MessageHandler.AddMessage(instrument);
            }
        }

        private void ProcessInstrument(Instrument instrument)
        {
            if (InstrumentUpdatedEvent != null)
            {
                InstrumentUpdatedEvent(instrument);
            }
        }

        void _Repository_TradeOccurredEvent(Trade trade)
        {
            if (trade.Symbol == _Symbol /*&& trade.Exchange == _Exchange*/)
            {
                _MessageHandler.AddMessage(trade);
            }
        }

        private void ProcessTrade(Trade trade)
        {
            if (TradeOccurredEvent != null)
            {
                TradeOccurredEvent(trade);
            }
        }

        void _MessageHandler_MessageReceived(object message)
        {
            if (message is Instrument)
            {
                ProcessInstrument((Instrument)message);
            }
            else if (message is Trade)
            {
                ProcessTrade((Trade)message);
            }
            else if (message is DateTime)
            {
                ProcessHeartbeatMessage((DateTime) message);                
            }
            else if(message is Index)
            {
                ProcessIndexValue((Index) message);
            }
        }

        void _Repository_HeartbeatReceivedEvent(DateTime servertime)
        {
            _MessageHandler.AddMessage(servertime);
        }

        private void ProcessHeartbeatMessage(DateTime servertime)
        {
            if (HeartBeatReceivedEvent != null)
                HeartBeatReceivedEvent(servertime);
        }

        void _Repository_IndexUpdatedEvent(Index index)
        {
            _MessageHandler.AddMessage(index);
        }

        private void ProcessIndexValue(Index index)
        {
            if(index.Symbol == _Symbol && index.Exchange == _Exchange)
            {
                Trade trade = new Trade();
                trade.Symbol = index.Symbol;
                trade.Price = index.Value;
                trade.TimeStamp = index.TimeStamp;
                trade.SequenceNumber = index.SequenceNumber;

                Instrument instrument = new Instrument();
                instrument.Symbol = index.Symbol;
                instrument.PercentageMoved = index.PercentageMoved;

                if (TradeOccurredEvent != null)
                    TradeOccurredEvent(trade);

                if (InstrumentUpdatedEvent != null)
                    InstrumentUpdatedEvent(instrument);
            }
        }

        public List<string> GetSymbols()
        {
            List<string> lst = new List<string>();
            foreach (var item in _Repository.AllInstrumentReferences)
            {
                if (!lst.Contains(item.Symbol))
                    lst.Add(item.Symbol);
            }
            return lst;
        }

        public List<BarData> GetHistory(string m_Symbol, Periodicity m_Periodicity, int interval, int BarCount)
        {
            List<BarData> lstBars = new List<BarData>();           
            try
            {
                FundXchange.Domain.Enumerations.PeriodEnum period = GetPeriodFromPeriodicity(m_Periodicity);
                List<FundXchange.Domain.ValueObjects.Candlestick> snapshots = _Repository.GetEquityCandlesticks(m_Symbol, GlobalDeclarations.EXCHANGE, interval, BarCount, period);
                for (int i = snapshots.Count - 1; i >= 0; i--)
                {
                    BarData bar = new BarData();
                    bar.OpenPrice = snapshots[i].Open;
                    bar.ClosePrice = snapshots[i].Close;
                    bar.HighPrice = snapshots[i].High;
                    bar.LowPrice = snapshots[i].Low;
                    bar.TradeDateTime = snapshots[i].TimeOfClose;
                    bar.StartDateTime = snapshots[i].TimeOfStart;
                    bar.Volume = snapshots[i].Volume;

                    // For Local Datetime
                    TimeZoneInfo cdt = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                    DateTime targetTime = TimeZoneInfo.ConvertTime(bar.StartDateTime, cdt, TimeZoneInfo.Local);
                    bar.StartDateTime = bar.TradeDateTime = targetTime;

                    lstBars.Add(bar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return lstBars;
        }

        internal static PeriodEnum GetPeriodFromPeriodicity(Periodicity periodicity)
        {
            switch (periodicity)
            {
                case Periodicity.Minutely:
                    return PeriodEnum.Minute;
                case Periodicity.Hourly:
                    return PeriodEnum.Hour;
                case Periodicity.Daily:
                    return PeriodEnum.Day;
                case Periodicity.Weekly:
                    return PeriodEnum.Week;
                case Periodicity.Monthly:
                    return PeriodEnum.Month;
                default:
                    return PeriodEnum.Minute;
            }

        }
    }
}
