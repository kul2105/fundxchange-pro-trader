using System;
using FundXchange.Domain.Enumerations;
using NUnit.Framework;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.Infrastructure;
using FundXchange.Domain.Entities;
using System.Threading;
using System.Collections.Generic;
using FundXchange.Model.Agents;
using Moq;

namespace FundXchange.Tests.UnitTests
{
    [TestFixture]
    public class given_candlestick_agent_has_no_symbol_subscribed
    {
        private CandlestickAgent _Agent;
        private string _Symbol = "NPN";
        private string _Exchange = "JSE";
        private Mock<IMarketRepository> _RepositoryMock;

        [SetUp]
        public void Setup()
        {
            _RepositoryMock = new Mock<IMarketRepository>();

            //setup NPN as known instrument
            List<InstrumentReference> references = new List<InstrumentReference>();
            InstrumentReference instrument = new InstrumentReference();
            instrument.Symbol = _Symbol;
            instrument.Exchange = _Exchange;
            references.Add(instrument);

            _RepositoryMock.Setup(a => a.AllInstrumentReferences).Returns(references);

            _Agent = new CandlestickAgent(_RepositoryMock.Object);
        }

        [Test]
        public void when_get_items_called_then_empty_list_returned()
        {
            Assert.AreEqual(0, _Agent.GetItems().Count, "Candles must be empty");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_empty_symbol_then_exception_expected()
        {
            _Agent.Subscribe("", "JSE", 1, 1, PeriodEnum.Minute);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_empty_exchange_then_exception_expected()
        {
            _Agent.Subscribe("NPN", "", 1, 1, PeriodEnum.Minute);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_unknown_symbol_then_exception_expected()
        {
            _Agent.Subscribe("AAAAAAA", "JSE", 1, 1, PeriodEnum.Minute);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_less_than_10_bars_then_exception_expected()
        {
            _Agent.Subscribe(_Symbol, _Exchange, 9, 1, PeriodEnum.Minute);
        }

        [Test]
        public void when_subscribe_called_with_known_symbol_10_bars_1_interval_then_level1_registered_and_candles_fetched()
        {
            int numBars = 10;
            int barInterval = 1;

            //setup instrument to expect
            Instrument expectedInstrument = InstrumentFactory.CreateInstrument(_Symbol, _Exchange);
            _RepositoryMock.Setup(r => r.SubscribeLevelOneWatch(_Symbol, _Exchange)).Returns(expectedInstrument);

            //setup candlesticks to expect
            List<Candlestick> expectedCandlesticks = InstrumentFactory.CreateCandlesticks(_Symbol, _Exchange, numBars, barInterval);
            _RepositoryMock.Setup(r => r.GetEquityCandlesticks(_Symbol, _Exchange, barInterval, numBars, PeriodEnum.Minute)).Returns(expectedCandlesticks);

            _Agent.Subscribe(_Symbol, _Exchange, numBars, barInterval, PeriodEnum.Minute);

            _RepositoryMock.Verify(r => r.SubscribeLevelOneWatch(_Symbol, _Exchange), Times.Exactly(1));
            _RepositoryMock.Verify(r => r.GetEquityCandlesticks(_Symbol, _Exchange, barInterval, numBars, PeriodEnum.Minute), Times.Exactly(1));

            Assert.AreEqual(expectedCandlesticks.Count, _Agent.GetItems().Count);
        }
    }
}