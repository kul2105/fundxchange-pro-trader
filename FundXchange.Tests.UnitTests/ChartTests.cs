using System;
using System.Collections.Generic;
using FundXchange.Domain.Entities;
using FundXchange.Domain.Enumerations;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.Controllers;
using FundXchange.Model.Infrastructure;
using FundXchange.Model.ViewModels.Charts;
using NUnit.Framework;
using Moq;
using Candlestick = FundXchange.Domain.ValueObjects.Candlestick;

namespace FundXchange.Tests.UnitTests
{
    [TestFixture]
    public class chart_not_subscribed_to_symbol
    {
        private ChartController _chartController;
        private Mock<IMarketRepository> _mockMarketRepository;
        private Mock<IChartView> _mockChartView;
        private Periodicity _periodicity;
        private int _interval;
        private int _numberOfCandles;

        [SetUp]
        public void Setup()
        {
            _mockMarketRepository = new Mock<IMarketRepository>();
            _mockChartView = new Mock<IChartView>();
            _periodicity = Periodicity.Minutely;
            _interval = 1;
            _numberOfCandles = 200;

            //setup NPN as known instrument
            List<InstrumentReference> references = new List<InstrumentReference>();
            InstrumentReference instrument = new InstrumentReference();
            instrument.Symbol = "NPN";
            references.Add(instrument);

            _mockMarketRepository.Setup(a => a.AllInstrumentReferences).Returns(references);
            //_chartController = new ChartController(_mockMarketRepository.Object, _mockChartView.Object);
        }

        [Test]
        [Ignore("Fix")]
        public void when_subscribed_symbol_referenced_then_null_returned()
        {
            //Assert.AreEqual(null, _chartController.Instrument);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_empty_symbol_then_exception_expected()
        {
            //_chartController.Subscribe("", "JSE", _periodicity, _interval, _numberOfCandles);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_empty_exchange_then_exception_expected()
        {
            //_chartController.Subscribe("NPN", "", _periodicity, _interval, _numberOfCandles);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_unknown_symbol_then_exception_expected()
        {
            //_chartController.Subscribe("AAAAAAA", "JSE", _periodicity, _interval, _numberOfCandles);
        }

        [Test]
        [Ignore("Fix")]
        public void when_subscribe_called_with_known_symbol_then_level1_watch_registered()
        {
            string symbol = "NPN";
            string exchange = "JSE";

            //setup instrument to expect
            Instrument expectedInstrument = InstrumentFactory.CreateInstrument(symbol, exchange);
            List<Candlestick> candlesticks = new List<Candlestick>();
            candlesticks.Add(new Candlestick
                            {
                                Close = 100,
                                High = 120,
                                Low = 90,
                                Open = 95,
                                TimeOfClose = DateTime.Now,
                                Volume = 1000
                            });
            _mockMarketRepository.Setup(r => r.SubscribeLevelOneWatch(symbol, exchange)).Returns(expectedInstrument);
            _mockMarketRepository.Setup(r => r.GetEquityCandlesticks(symbol, exchange, _interval, _numberOfCandles, PeriodEnum.Minute)).Returns(candlesticks);

            //_chartController.Subscribe(symbol, exchange, _periodicity, _interval, _numberOfCandles);

            _mockMarketRepository.Verify(r => r.SubscribeLevelOneWatch(symbol, exchange), Times.Exactly(1));
            _mockMarketRepository.Verify(r => r.GetEquityCandlesticks(symbol, exchange, _interval, _numberOfCandles, PeriodEnum.Minute), Times.Exactly(1));

            _mockChartView.Verify(v => v.RefreshInstrumentHeader(expectedInstrument), Times.Exactly(1));

            //Assert.AreEqual(expectedInstrument, _chartController.Instrument);
        }
    }

    [TestFixture]
    public class given_chart_subscribed_to_symbol
    {
        private ChartController _chartController;
        private Mock<IMarketRepository> _mockMarketRepository;
        private Mock<IChartView> _mockChartView;
        private InstrumentReference _instrumentReference;
        private Periodicity _periodicity;
        private int _interval;
        private int _numberOfCandles;

        [SetUp]
        public void Setup()
        {
            _periodicity = Periodicity.Minutely;
            _interval = 1;
            _numberOfCandles = 200;

            _mockMarketRepository = new Mock<IMarketRepository>();
            _mockChartView = new Mock<IChartView>();

            List<InstrumentReference> references = new List<InstrumentReference>();
            
            _instrumentReference = new InstrumentReference();
            _instrumentReference.Symbol = "NPN";
            _instrumentReference.Exchange = "JSE";

            InstrumentReference differentInstrument = new InstrumentReference();
            differentInstrument.Symbol = "AGL";
            differentInstrument.Exchange = "JSE";

            references.Add(_instrumentReference);
            references.Add(differentInstrument);

            List<Candlestick> candlesticks = new List<Candlestick>();
            candlesticks.Add(new Candlestick
            {
                Close = 100,
                High = 120,
                Low = 90,
                Open = 95,
                TimeOfClose = DateTime.Now,
                Volume = 1000
            });

            _mockMarketRepository.Setup(a => a.AllInstrumentReferences).Returns(references);

            Instrument expectedInstrument = InstrumentFactory.CreateInstrument(_instrumentReference.Symbol, _instrumentReference.Exchange);
            _mockMarketRepository.Setup(r => r.SubscribeLevelOneWatch(_instrumentReference.Symbol, _instrumentReference.Exchange)).Returns(expectedInstrument);
            _mockMarketRepository.Setup(r => r.GetEquityCandlesticks(_instrumentReference.Symbol, _instrumentReference.Exchange, _interval, _numberOfCandles, PeriodEnum.Minute)).Returns(candlesticks);

            //_chartController = new ChartController(_mockMarketRepository.Object, _mockChartView.Object);
            //_chartController.Subscribe(_instrumentReference.Symbol, _instrumentReference.Exchange, _periodicity, _interval, _numberOfCandles);
        }

        [Test]
        public void when_repository_raises_instrument_update_event_for_current_symbol_then_view_refreshes_instrument_header()
        {
            Instrument instrument = InstrumentFactory.CreateInstrument(_instrumentReference.Symbol, _instrumentReference.Exchange);
            _mockMarketRepository.Raise(r => r.InstrumentUpdatedEvent += null, instrument);
            _mockChartView.Verify(v => v.RefreshInstrumentHeader(instrument), Times.Exactly(1));
            _mockMarketRepository.Verify(r => r.GetEquityCandlesticks(_instrumentReference.Symbol, _instrumentReference.Exchange, _interval, _numberOfCandles, PeriodEnum.Minute), Times.Exactly(1));
        }

        [Test]
        public void when_repository_raises_instrumentReference_update_event_for_different_symbol_then_view_not_updated()
        {
            Instrument instrument = InstrumentFactory.CreateInstrument("AGL", _instrumentReference.Exchange);
            _mockMarketRepository.Raise(r => r.InstrumentUpdatedEvent += null, instrument);
            _mockChartView.Verify(v => v.RefreshInstrumentHeader(instrument), Times.Never());
            _mockMarketRepository.Verify(r => r.GetEquityCandlesticks(instrument.Symbol, instrument.Exchange, _interval, _numberOfCandles, PeriodEnum.Minute), Times.Never());
        }

        [Test]
        [Ignore("Fix")]
        public void when_repository_raises_trade_event_for_current_symbol_then_view_updates_latest_candlestick_with_trade()
        {
            //Trade trade = InstrumentFactory.CreateTrade(_instrumentReference.Symbol, _instrumentReference.Exchange);
            //_mockMarketRepository.Raise(r => r.TradeOccurredEvent += null, trade);
            //_mockChartView.Verify(v => v.UpdateLatestCandlestick(trade), Times.Exactly(1));
        }

        [Test]
        [Ignore("Fix")]
        public void when_repository_raises_trade_event_for_different_symbol_then_latest_candlestick_not_updated()
        {
            //Trade trade = InstrumentFactory.CreateTrade("AGL", _instrumentReference.Exchange);
            //_mockMarketRepository.Raise(r => r.TradeOccurredEvent += null, trade);
            //_mockChartView.Verify(v => v.UpdateLatestCandlestick(trade), Times.Never());
        }

        [Test]
        [Ignore("Fix")]
        public void when_subscribe_called_with_new_symbol_then_level1_watch_registered_and_trade_events_raised()
        {
            string symbol = "AGL";
            string exchange = "JSE";

            List<Candlestick> candlesticks = new List<Candlestick>();
            candlesticks.Add(new Candlestick
            {
                Close = 100,
                High = 120,
                Low = 90,
                Open = 95,
                TimeOfClose = DateTime.Now,
                Volume = 1000
            });

            //setup instrument to expect
            Instrument expectedInstrument = InstrumentFactory.CreateInstrument(symbol, exchange);
            _mockMarketRepository.Setup(r => r.SubscribeLevelOneWatch(symbol, exchange)).Returns(expectedInstrument);
            _mockMarketRepository.Setup(r => r.GetEquityCandlesticks(symbol, exchange, _interval, _numberOfCandles, PeriodEnum.Minute)).Returns(candlesticks);

            //_chartController.Subscribe(symbol, exchange, _periodicity, _interval, _numberOfCandles);

            _mockMarketRepository.Verify(r => r.SubscribeLevelOneWatch(symbol, exchange), Times.Exactly(1));
            _mockMarketRepository.Verify(r => r.GetEquityCandlesticks(symbol, exchange, _interval, _numberOfCandles, PeriodEnum.Minute), Times.Exactly(1));

            _mockChartView.Verify(v => v.RefreshInstrumentHeader(expectedInstrument), Times.Exactly(1));
           
            //Assert.AreEqual(expectedInstrument, _chartController.Instrument);
        }
    }
}
