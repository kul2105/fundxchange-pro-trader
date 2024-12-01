using System;
using System.Linq;
using System.Collections.Generic;
using FundXchange.Model.Infrastructure;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.ViewModels.TimeSales;
using FundXchange.Domain;

namespace FundXchange.Model.Controllers
{
    public class TimeSalesController
    {
        private IMarketRepository _repository;
        private ITimeAndSalesView _view;
        private long _lastSequenceNumber = 0;
        private MessageHandler<object> _MessageHandler;

        public Instrument Instrument { get; private set; }
        
        public TimeSalesController(IMarketRepository repository, ITimeAndSalesView view)
        {
            _repository = repository;
            _view = view;

            _MessageHandler = new MessageHandler<object>("TimeAndSales");
            _MessageHandler.MessageReceived += _MessageHandler_MessageReceived;

            SubscribeToLevel1Events();
        }

        public void Subscribe(string symbol, string exchange)
        {
            symbol = symbol.ToUpper();
            if (String.IsNullOrEmpty(symbol))
                throw new ApplicationException("Symbol cannot be Null or Empty when subscribing to TimeAndSales");

            if (String.IsNullOrEmpty(exchange))
                throw new ApplicationException("Exchange cannot be Null or Empty when subscribing to TimeAndSales");

            int count = _repository.AllInstrumentReferences.Where(i => i.Symbol == symbol).Count();
            if (count == 0)
                throw new ApplicationException("Invalid symbol specified with subscribe to TimeAndSales");

            //if (_MessageHandler != null)
            //    _MessageHandler.Dispose();

            _MessageHandler = new MessageHandler<object>("TimeAndSales");
            _MessageHandler.MessageReceived -= _MessageHandler_MessageReceived;
            _MessageHandler.MessageReceived += _MessageHandler_MessageReceived;
            _MessageHandler.Start();

            if (Instrument == null || symbol != Instrument.Symbol)
            {
                Instrument instrument = _repository.SubscribeLevelOneWatch(symbol, exchange);
                Instrument = instrument;
                
                UpdateViewWithInstrument(Instrument);

                List<Trade> trades = _repository.GetTrades(exchange, symbol, 50);
                trades = trades.OrderByDescending(t => t.SequenceNumber).ToList();
                if (trades.Count > 0)
                    _lastSequenceNumber = trades[trades.Count - 1].SequenceNumber;

              
                UpdateViewWithTrades(trades);
            }
        }

        public void Stop()
        {
            _repository.InstrumentUpdatedEvent -= _marketRepository_InstrumentUpdatedEvent;
            _repository.TradeOccurredEvent -= _marketRepository_TradeOccurredEvent;
            _MessageHandler.Dispose();
        }

        private void UpdateViewWithInstrument(Instrument instrument)
        {
            _view.UpdateGridWithInstrument(instrument);
        }

        private void UpdateViewWithTrades(List<Trade> trades)
        {
            _view.AddTrades(trades);
        }

        private void SubscribeToLevel1Events()
        {
            _repository.InstrumentUpdatedEvent += _marketRepository_InstrumentUpdatedEvent;
            _repository.TradeOccurredEvent += _marketRepository_TradeOccurredEvent;
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
        }

        void _marketRepository_TradeOccurredEvent(Trade trade)
        {
            _MessageHandler.AddMessage(trade);
        }

        private void ProcessTrade(Trade trade)
        {
            //if (trade != null && Instrument != null)
            {
                if (trade.Symbol == Instrument.Symbol)
                {
                    //if (trade.SequenceNumber > _lastSequenceNumber)// John
                    //{
                        _lastSequenceNumber = trade.SequenceNumber;

                        UpdateViewWithTrades(new List<Trade>() { trade });
                    //}
                }
            }
        }

        void _marketRepository_InstrumentUpdatedEvent(Instrument instrument)
        {
            _MessageHandler.AddMessage(instrument);
        }

















































































































        private void ProcessInstrument(Instrument instrument)
        {
            //if (instrument != null && Instrument != null)
            {
                if (instrument.Symbol == Instrument.Symbol)
                {
                    UpdateViewWithInstrument(instrument);
                }
            }           
          
        }

        public void ChangeInstrumnet(string m_Symbol)
        {

        }
    }
}
