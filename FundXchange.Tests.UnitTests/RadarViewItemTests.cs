using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.Infrastructure;
using FundXchange.Model.Repositories;
using FundXchange.Model.ViewModels.Charts;
using FundXchange.Model.ViewModels.Indicators;
using Moq;
using NUnit.Framework;
using FundXchange.Model.ViewModels.RadarView;

namespace FundXchange.Tests.UnitTests
{
    [TestFixture]
    public class given_radar_view_item_initialized
    {
        private Mock<IMarketRepository> _repositoryMock;
        private Mock<IIndicatorRepository> _indicatorRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IMarketRepository>();
            _indicatorRepositoryMock = new Mock<IIndicatorRepository>();

            //Set known instruments
            List<InstrumentReference> references = new List<InstrumentReference>();
            InstrumentReference naspersRef = new InstrumentReference();
            naspersRef.Symbol = "NPN";
            references.Add(naspersRef);

            InstrumentReference angloRef = new InstrumentReference();
            angloRef.Symbol = "AGL";
            references.Add(angloRef);

            Instrument naspers = new Instrument()
            {
                Symbol = "NPN",
                Exchange = "JSE"
            };

            Instrument anglo = new Instrument()
            {
                Symbol = "AGL",
                Exchange = "JSE"
            };

            _repositoryMock.Setup(a => a.AllInstrumentReferences).Returns(references);
            _repositoryMock.Setup(s => s.SubscribeLevelOneWatch("NPN", "JSE")).Returns(naspers);
            _repositoryMock.Setup(s => s.SubscribeLevelOneWatch("AGL", "JSE")).Returns(anglo);

            // Set known indicators
            List<Indicator> indicators = new List<Indicator>(1);
            indicators.Add(new Indicator("Accumulative Swing Index", "ASI", "TREND(ASI(1)) > UP"));
            _indicatorRepositoryMock.SetupGet(r => r.AvailableIndicators).Returns(indicators);
        }

        [Test]
        public void given_new_radar_view_item_associated_instrument_is_set()
        {
            Instrument instrument = _repositoryMock.Object.SubscribeLevelOneWatch("NPN", "JSE");
            RadarItem item = new RadarItem(Guid.NewGuid(), instrument, _repositoryMock.Object, Periodicity.Minutely, 5, 100);

            //Assert.AreEqual(item.AssociatedInstrument, instrument);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void given_radar_view_item_is_initialized_update_will_except_if_updated_with_different_instrument()
        {
            Instrument instrument = _repositoryMock.Object.SubscribeLevelOneWatch("NPN", "JSE");
            RadarItem item = new RadarItem(Guid.NewGuid(), instrument, _repositoryMock.Object, Periodicity.Minutely, 5, 100);
            
            //Assert.AreEqual(item.AssociatedInstrument, instrument);

            Instrument instrument2 = _repositoryMock.Object.SubscribeLevelOneWatch("AGL", "JSE");
            item.UpdateWith(instrument2);
        }

        [Test]
        public void given_radar_view_item_contains_at_least_one_indicator_remove_indicator_will_remove_indicator_from_associated_indicators()
        {
            Instrument instrument = _repositoryMock.Object.SubscribeLevelOneWatch("NPN", "JSE");
            RadarItem item = new RadarItem(Guid.NewGuid(), instrument, _repositoryMock.Object, Periodicity.Minutely, 5, 100);
            item.AddIndicator(_indicatorRepositoryMock.Object.AvailableIndicators.First());

            Assert.Equals(item.GetIndicators().Count(), 1);

            item.RemoveIndicator(_indicatorRepositoryMock.Object.AvailableIndicators.First());

            Assert.Equals(item.GetIndicators().Count(), 0);
        }
    }
}
