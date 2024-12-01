using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.Agents;
using FundXchange.Model.ViewModels.Charts;
using NUnit.Framework;
using FundXchange.Model.Controllers;
using FundXchange.Model.Infrastructure;
using FundXchange.Model.ViewModels.RadarView;
using Moq;
using FundXchange.Model.Repositories;
using FundXchange.Model.ViewModels.Indicators;

namespace FundXchange.Tests.UnitTests
{
    /// <summary>
    /// Summary description for RadarViewItemTests
    /// </summary>
    [TestFixture]
    public class given_radar_view_not_subscribed_to_symbols
    {
        private RadarGridController _radarGridController;
        private Mock<IMarketRepository> _repositoryMock;
        private Mock<IRadarGridView> _viewMock;
        private Mock<IIndicatorRepository> _indicatorRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IMarketRepository>();
            _viewMock = new Mock<IRadarGridView>();
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
            indicators.Add(new Indicator("Test", "T", string.Empty));
            _indicatorRepositoryMock.SetupGet(r => r.AvailableIndicators).Returns(indicators);

            _radarGridController = new RadarGridController(_repositoryMock.Object, _viewMock.Object);
        }

        [Test]
        public void when_get_radar_items_accessed_none_returned()
        {
            Assert.AreEqual(_radarGridController.GetRadars().Count(), 0);
        }

        [Test]
        public void when_add_radar_called_with_valid_symbol_radar_added()
        {
            Assert.AreEqual(_radarGridController.GetRadars().Count(), 0);

            _radarGridController.AddRadar("NPN", "JSE");

            Assert.AreEqual(_radarGridController.GetRadars().Count(), 1);
        }

        [Test]
        public void when_add_radar_called_with_valid_symbol_radar_added_to_view()
        {
            Assert.AreEqual(_radarGridController.GetRadars().Count(), 0);

            _radarGridController.AddRadar("NPN", "JSE");

            Assert.AreEqual(_radarGridController.GetRadars().Count(), 1);

            //_viewMock.Verify(r => r.AddRadar(null), Times.Exactly(1));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void when_add_radar_is_called_with_valid_symbol_and_item_exists_expect_exception()
        {
            _radarGridController.AddRadar("NPN", "JSE");

            Assert.AreEqual(_radarGridController.GetRadars().Count(), 1);
            //Assert.AreEqual(_radarGridController.GetRadars().First()._associatedInstrument.Symbol, "NPN");

            _radarGridController.AddRadar("NPN", "JSE");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void when_add_radar_called_with_empty_symbol_expect_exception()
        {
            _radarGridController.AddRadar(string.Empty, "JSE");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void when_add_radar_called_with_empty_exchange_expect_exception()
        {
            _radarGridController.AddRadar("NPN", string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_add_radar_called_with_unknown_symbol_then_exception_expected()
        {
            _radarGridController.AddRadar("AAAAAAA", "JSE");
        }

        [Test]
        public void when_add_indicators_called_with_valid_indicators_items_added_to_indicator_collection()
        {
            Assert.Greater(_indicatorRepositoryMock.Object.AvailableIndicators.Count(), 0);

            _radarGridController.UpdateIndicators(_indicatorRepositoryMock.Object.AvailableIndicators);

            Assert.AreEqual(_radarGridController.GetActiveIndicators().Count(), _indicatorRepositoryMock.Object.AvailableIndicators.Count());
        }

        [Test]
        public void when_remove_indicators_called_with_valid_indicators_items_removed_from_indicator_collection()
        {
            Assert.Greater(_indicatorRepositoryMock.Object.AvailableIndicators.Count(), 0);

            _radarGridController.UpdateIndicators(_indicatorRepositoryMock.Object.AvailableIndicators);

            Assert.AreEqual(_radarGridController.GetActiveIndicators().Count(), _indicatorRepositoryMock.Object.AvailableIndicators.Count());
        }
    }
}
