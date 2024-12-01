using System;
using System.Collections.Generic;
using NUnit.Framework;
using FundXchange.Model.Controllers;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.Infrastructure;
using FundXchange.Domain.Entities;
using Moq;
using FundXchange.Model.ViewModels.TimeSales;

namespace FundXchange.Tests.UnitTests
{
    [TestFixture]
    public class given_time_and_sales_not_subscribed_to_symbol
    {
        private TimeSalesController _controller;
        private Mock<IMarketRepository> _RepositoryMock;
        private Mock<ITimeAndSalesView> _ViewMock;

        [SetUp]
        public void Setup()
        {
            _RepositoryMock = new Mock<IMarketRepository>();
            _ViewMock = new Mock<ITimeAndSalesView>();

            //setup NPN as known instrument
            List<InstrumentReference> references = new List<InstrumentReference>();
            InstrumentReference instrument = new InstrumentReference();
            instrument.Symbol = "NPN";
            references.Add(instrument);

            _RepositoryMock.Setup(a => a.AllInstrumentReferences).Returns(references);
            _controller = new TimeSalesController(_RepositoryMock.Object, _ViewMock.Object);
        }

        [Test]
        public void when_subscribed_symbol_referenced_then_empty_returned()
        {
            Assert.AreEqual(null, _controller.Instrument);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_empty_symbol_then_exception_expected()
        {
            _controller.Subscribe("", "JSE");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_empty_exchange_then_exception_expected()
        {
            _controller.Subscribe("NPN", "");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_unknown_symbol_then_exception_expected()
        {
            _controller.Subscribe("AAAAAAA", "JSE");
        }

        [Test]
        public void when_subscribe_called_with_known_symbol_then_level1_watch_registered_and_trade_retrieved()
        {
            string symbol = "NPN";
            string exchange = "JSE";
            int numTrades = 5;

            //setup instrument to expect
            Instrument expectedInstrument = InstrumentFactory.CreateInstrument(symbol, exchange);
            _RepositoryMock.Setup(r => r.SubscribeLevelOneWatch(symbol, exchange)).Returns(expectedInstrument);

            //setup trades to expect
            List<Trade> expectedTrades = InstrumentFactory.CreateTrades(symbol, exchange, numTrades);
            _RepositoryMock.Setup(r => r.GetTrades(exchange, symbol, 50)).Returns(expectedTrades);

            _controller.Subscribe(symbol, exchange);

            _RepositoryMock.Verify(r => r.SubscribeLevelOneWatch(symbol, exchange), Times.Exactly(1));
            _RepositoryMock.Verify(r => r.GetTrades(exchange, symbol, 50), Times.Exactly(1));

            _ViewMock.Verify(v => v.UpdateGridWithInstrument(expectedInstrument), Times.Exactly(1));
            _ViewMock.Verify(v => v.AddTrades(expectedTrades), Times.Exactly(1));

            Assert.AreEqual(expectedInstrument, _controller.Instrument);
        }
    }

    [TestFixture]
    public class given_time_and_sales_subscribed_to_symbol
    {
        private TimeSalesController _controller;
        private Mock<IMarketRepository> _RepositoryMock;
        private Mock<ITimeAndSalesView> _ViewMock;
        private InstrumentReference _Instrument;

        [SetUp]
        public void Setup()
        {
            _RepositoryMock = new Mock<IMarketRepository>();
            _ViewMock = new Mock<ITimeAndSalesView>();

            //setup NPN as known instrument
            List<InstrumentReference> references = new List<InstrumentReference>();
            _Instrument = new InstrumentReference();
            _Instrument.Symbol = "NPN";
            _Instrument.Exchange = "JSE";

            InstrumentReference differentInstrument = new InstrumentReference();
            differentInstrument.Symbol = "AGL";
            differentInstrument.Exchange = "JSE";

            references.Add(_Instrument);
            references.Add(differentInstrument);

            _RepositoryMock.Setup(a => a.AllInstrumentReferences).Returns(references);

            Instrument expectedInstrument = InstrumentFactory.CreateInstrument(_Instrument.Symbol, _Instrument.Exchange);
            _RepositoryMock.Setup(r => r.SubscribeLevelOneWatch(_Instrument.Symbol, _Instrument.Exchange)).Returns(expectedInstrument);

            List<Trade> expectedTrades = InstrumentFactory.CreateTrades(_Instrument.Symbol, _Instrument.Exchange, 5);
            _RepositoryMock.Setup(r => r.GetTrades(_Instrument.Exchange, _Instrument.Symbol, 50)).Returns(expectedTrades);

            _controller = new TimeSalesController(_RepositoryMock.Object, _ViewMock.Object);
            _controller.Subscribe(_Instrument.Symbol, _Instrument.Exchange);
        }

        [Test]
        public void when_repository_raises_instrument_update_event_for_current_symbol_then_view_updated_with_instrument()
        {
            Instrument instrument = InstrumentFactory.CreateInstrument(_Instrument.Symbol, _Instrument.Exchange);
            _RepositoryMock.Raise(r => r.InstrumentUpdatedEvent += null, instrument);
            _ViewMock.Verify(v => v.UpdateGridWithInstrument(instrument), Times.Exactly(1));
        }

        [Test]
        public void when_repository_raises_instrument_update_event_for_different_symbol_then_view_not_updated()
        {
            Instrument instrument = InstrumentFactory.CreateInstrument("AGL", _Instrument.Exchange);
            _RepositoryMock.Raise(r => r.InstrumentUpdatedEvent += null, instrument);
            _ViewMock.Verify(v => v.UpdateGridWithInstrument(instrument), Times.Never());
        }

        [Test]
        public void when_repository_raises_trade_event_for_current_symbol_then_view_updated_with_trade()
        {
            Trade trade = InstrumentFactory.CreateTrade(_Instrument.Symbol, _Instrument.Exchange);
            _RepositoryMock.Raise(r => r.TradeOccurredEvent += null, trade);
            _ViewMock.Verify(v => v.AddTrades(new List<Trade>() { trade }), Times.Exactly(1));
        }

        [Test]
        public void when_repository_raises_trade_event_for_different_symbol_then_item_list_not_updated()
        {
            Trade trade = InstrumentFactory.CreateTrade("AGL", _Instrument.Exchange);
            _RepositoryMock.Raise(r => r.TradeOccurredEvent += null, trade);
            _ViewMock.Verify(v => v.AddTrades(new List<Trade>() { trade }), Times.Never());
        }

        [Test]
        public void when_subscribe_called_with_new_symbol_then_level2_watch_registered_and_trade_events_raised()
        {
            string symbol = "AGL";
            string exchange = "JSE";
            int numTrades = 5;

            //setup instrument to expect
            Instrument expectedInstrument = InstrumentFactory.CreateInstrument(symbol, exchange);
            _RepositoryMock.Setup(r => r.SubscribeLevelOneWatch(symbol, exchange)).Returns(expectedInstrument);

            //setup trades to expect
            List<Trade> expectedTrades = InstrumentFactory.CreateTrades(symbol, exchange, numTrades);
            _RepositoryMock.Setup(r => r.GetTrades(exchange, symbol, 50)).Returns(expectedTrades);

            _controller.Subscribe(symbol, exchange);

            _RepositoryMock.Verify(r => r.SubscribeLevelOneWatch(symbol, exchange), Times.Exactly(1));
            _RepositoryMock.Verify(r => r.GetTrades(exchange, symbol, 50), Times.Exactly(1));

            _ViewMock.Verify(v => v.UpdateGridWithInstrument(expectedInstrument), Times.Exactly(1));
            _ViewMock.Verify(v => v.AddTrades(expectedTrades), Times.Exactly(1));
            Assert.AreEqual(expectedInstrument, _controller.Instrument);
        }
    }
}
