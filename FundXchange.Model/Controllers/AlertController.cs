using System;
using System.Collections.Generic;
using System.Timers;
using FundXchange.Model.Infrastructure;
using FundXchange.Model.AlertService;
using FundXchange.Model.Gateways;
using FundXchange.Infrastructure;
using FundXchange.Model.ViewModels.Charts;

namespace FundXchange.Model.Controllers
{
    public class AlertController
    {
        public AlertController(int userId)
        {
            _UserId = userId;
            _ErrorService = IoC.Resolve<ErrorService>();
            _Repository = IoC.Resolve<IMarketRepository>();
            _Subscriptions = new List<Candlestick>();

            _Repository.TradeOccurredEvent += _Repository_TradeOccurredEvent;
        }

        private int _UserId;
        private ErrorService _ErrorService;
        private IMarketRepository _Repository;
        private readonly List<Candlestick> _Subscriptions;

        public event CandleStickClosedDelegate CandleStickClosed;

        public void RegisterScanInstrument(string symbol, string exchange, Periodicity periodicity, int interval, DateTime lastCandleTimestamp)
        {
            Candlestick candleStick;
            if (!TryGetCandleStick(symbol, out candleStick))
            {
                var subscribedInstrument = _Repository.SubscribeLevelOneWatch(symbol, exchange);
                candleStick = new Candlestick(symbol, subscribedInstrument.LastTrade);
                _Subscriptions.Add(candleStick);

                foreach (var trade in subscribedInstrument.Trades.FindAll(t => t.TimeStamp >= lastCandleTimestamp))
                    candleStick.UpdateWith(trade);

                var intervalInSeconds = GetIntervalInSeconds(periodicity, interval);
                var latestBarEndTime = lastCandleTimestamp.AddSeconds(intervalInSeconds);
                TimeSpan difference = DateTime.Now - latestBarEndTime;

                if (DateTime.Now.Hour < 17)
                {
                    if (intervalInSeconds > difference.TotalSeconds)
                    {
                        var waitTimer = new Timer(intervalInSeconds - difference.TotalSeconds);
                        waitTimer.Elapsed += (sender, signalTime) =>
                            {
                                WireUpCandlestick(periodicity, interval, candleStick);
                            };
                    }
                    else
                    {
                        WireUpCandlestick(periodicity, interval, candleStick);
                    }
                }
            }
        }

        public void RemoveScanInstrument(string symbol)
        {
            Candlestick scanItem;
            if (TryGetCandleStick(symbol, out scanItem))
                _Subscriptions.Remove(scanItem);
        }

        public void SaveAlertScript(AlertScriptDTO alertScriptDTO)
        {
            AlertServiceGateway gateway = new AlertServiceGateway(_ErrorService);
            gateway.SaveAlertScript(_UserId, alertScriptDTO);
        }

        public void RemoveAlertScript(string alertName)
        {
            AlertServiceGateway gateway = new AlertServiceGateway(_ErrorService);
            gateway.RemoveAlertScript(_UserId, alertName);
        }

        public void AddAlert(AlertDTO alertDTO)
        {
            AlertServiceGateway gateway = new AlertServiceGateway(_ErrorService);
            gateway.AddAlert(_UserId, alertDTO);
        }

        public void SaveScanner(ScannerDTO scannerDTO)
        {
            AlertServiceGateway gateway = new AlertServiceGateway(_ErrorService);
            gateway.SaveScanner(_UserId, scannerDTO);
        }

        public void RemoveScanner(string scannerName)
        {
            AlertServiceGateway gateway = new AlertServiceGateway(_ErrorService);
            gateway.RemoveScanner(_UserId, scannerName);
        }

        void _Repository_TradeOccurredEvent(FundXchange.Domain.ValueObjects.Trade trade)
        {
            Candlestick scanItem;
            if (TryGetCandleStick(trade.Symbol, out scanItem))
            {
                scanItem.UpdateWith(trade);
            }
        }

        void CandleStickHasClosed(Candlestick candleStick)
        {
            if (null != CandleStickClosed)
                CandleStickClosed(candleStick);
        }

        private bool TryGetCandleStick(string symbol, out Candlestick candleStick)
        {
            candleStick = null;
            candleStick = _Subscriptions.Find(s => s.Symbol == symbol);
            return candleStick != null;
        }

        private void WireUpCandlestick(Periodicity periodicity, int interval, Candlestick candleStick)
        {
            candleStick.SetTimer(GetIntervalInSeconds(periodicity, interval) * 1000);
            candleStick.Closed += CandleStickHasClosed;
        }

        private int GetIntervalInSeconds(Periodicity periodicity, int interval)
        {
            int multiplier = 0;
            switch (periodicity)
            {
                case Periodicity.Secondly:
                    multiplier = 1;
                    break;
                case Periodicity.Minutely:
                    multiplier = 60;
                    break;
                case Periodicity.Hourly:
                    multiplier = 60 * 60;
                    break;
                case Periodicity.Daily:
                    multiplier = (60 * 60) * 24;
                    break;
                case Periodicity.Weekly:
                    multiplier = ((60 * 60) * 24) * 7;
                    break;
            }
            return interval * multiplier;
        }
    }
}
