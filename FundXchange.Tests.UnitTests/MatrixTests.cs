using System;
using System.Collections.Generic;
using NUnit.Framework;
using FundXchange.Model.Controllers;
using FundXchange.Model.ViewModels.Matrix;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.Infrastructure;
using FundXchange.Domain.Entities;
using Moq;
using FundXchange.Model.Agents;

namespace FundXchange.Tests.UnitTests
{
    [TestFixture]
    public class given_matrix_not_subscribed_to_symbol
    {
        private MatrixController _matrixController;
        private Mock<IMarketRepository> _RepositoryMock;
        private Mock<IMatrixView> _ViewMock;
        private Mock<IMatrixViewItemList> _ItemListMock;
        private Mock<IErrorService> _ErrorServiceMock;
        
        [SetUp]
        public void Setup()
        {
            _RepositoryMock = new Mock<IMarketRepository>();
            _ViewMock = new Mock<IMatrixView>();
            _ItemListMock = new Mock<IMatrixViewItemList>();
            _ErrorServiceMock = new Mock<IErrorService>();

            //setup NPN as known instrument
            List<InstrumentReference> references = new List<InstrumentReference>();
            InstrumentReference instrument = new InstrumentReference();
            instrument.Symbol = "NPN";
            references.Add(instrument);

            var orderbook = new Orderbook(instrument.Symbol, instrument.Symbol, instrument.Exchange, false);

            _RepositoryMock.Setup(a => a.AllInstrumentReferences).Returns(references);
            _RepositoryMock.Setup(a => a.Orderbook).Returns(orderbook);

            _matrixController = new MatrixController(_RepositoryMock.Object, _ViewMock.Object, _ItemListMock.Object, _ErrorServiceMock.Object);
        }

        [Test]
        public void when_get_items_called_then_none_returned()
        {
            IList<MatrixViewItem> items = _matrixController.GetItems();
            Assert.AreEqual(0, items.Count);
        }

        [Test]
        public void when_subscribed_symbol_referenced_then_empty_returned()
        {
            Assert.AreEqual(null, _matrixController.Instrument);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_empty_symbol_then_exception_expected()
        {
            _matrixController.Subscribe("", "JSE");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_empty_exchange_then_exception_expected()
        {
            _matrixController.Subscribe("NPN", "");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_subscribe_called_with_unknown_symbol_then_exception_expected()
        {
            _matrixController.Subscribe("AAAAAAA", "JSE");
        }

        [Test]
        public void when_subscribe_called_with_known_symbol_then_level2_watch_registered_and_trade_events_raised()
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

            _matrixController.Subscribe(symbol, exchange);

            _RepositoryMock.Verify(r => r.SubscribeLevelOneWatch(symbol, exchange), Times.Exactly(1));
            _RepositoryMock.Verify(r => r.SubscribeLevelTwoWatch(symbol, exchange), Times.Exactly(1));
            _RepositoryMock.Verify(r => r.GetTrades(exchange, symbol, 50), Times.Exactly(1));

            _ViewMock.Verify(v => v.UpdateGridWithInstrument(expectedInstrument), Times.Exactly(1));

            _ItemListMock.Verify(c => c.AddInstrument(expectedInstrument), Times.Exactly(1));
            _ItemListMock.Verify(c => c.AddTrades(expectedTrades), Times.Exactly(1));
            Assert.AreEqual(expectedInstrument, _matrixController.Instrument);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void when_subscribe_called_and_repository_raises_exception_with_level1_subscribe_then_exception_expected()
        {
            Exception exception = new Exception("An unknown error occurred in subscribe level 1");
            _RepositoryMock.Setup(r => r.SubscribeLevelOneWatch("NPN", "JSE")).Throws(exception);
            _matrixController.Subscribe("NPN", "JSE");
            _ErrorServiceMock.Verify(e => e.LogError(exception.Message, exception), Times.Exactly(1));
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void when_subscribe_called_and_repository_raises_exception_with_get_trades_then_exception_expected()
        {
            string symbol = "NPN";
            string exchange = "JSE";
            Exception exception = new Exception("An unknown error occurred in get trades");

            //setup instrument to expect
            Instrument expectedInstrument = InstrumentFactory.CreateInstrument(symbol, exchange);
            _RepositoryMock.Setup(r => r.SubscribeLevelOneWatch(symbol, exchange)).Returns(expectedInstrument);
            _RepositoryMock.Setup(r => r.GetTrades(exchange, symbol, 50)).Throws(exception);
            _matrixController.Subscribe("NPN", "JSE");
            _ErrorServiceMock.Verify(e => e.LogError(exception.Message, exception), Times.Exactly(1));
        }
    }

    [TestFixture]
    public class given_matrix_subscribed_to_symbol
    {
        private MatrixController _matrixController;
        private Mock<IMarketRepository> _RepositoryMock;
        private Mock<IMatrixView> _ViewMock;
        private Mock<IMatrixViewItemList> _ItemListMock;
        private Mock<IOrderbook> _OrderbookMock;
        private Mock<IErrorService> _ErrorServiceMock;

        private InstrumentReference _Instrument;

        [SetUp]
        public void Setup()
        {
            _RepositoryMock = new Mock<IMarketRepository>();
            _ViewMock = new Mock<IMatrixView>();
            _ItemListMock = new Mock<IMatrixViewItemList>();
            _OrderbookMock = new Mock<IOrderbook>();
            _ErrorServiceMock = new Mock<IErrorService>();

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
            _RepositoryMock.Setup(a => a.Orderbook).Returns(_OrderbookMock.Object);

            Instrument expectedInstrument = InstrumentFactory.CreateInstrument(_Instrument.Symbol, _Instrument.Exchange);
            _RepositoryMock.Setup(r => r.SubscribeLevelOneWatch(_Instrument.Symbol, _Instrument.Exchange)).Returns(expectedInstrument);

            List<Trade> expectedTrades = InstrumentFactory.CreateTrades(_Instrument.Symbol, _Instrument.Exchange, 5);
            _RepositoryMock.Setup(r => r.GetTrades(_Instrument.Exchange, _Instrument.Symbol, 50)).Returns(expectedTrades);

            _matrixController = new MatrixController(_RepositoryMock.Object, _ViewMock.Object, _ItemListMock.Object, _ErrorServiceMock.Object);
            _matrixController.Subscribe(_Instrument.Symbol, _Instrument.Exchange);
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
        public void when_repository_raises_trade_event_for_current_symbol_then_item_list_updated_with_trade()
        {
            Trade trade = InstrumentFactory.CreateTrade(_Instrument.Symbol, _Instrument.Exchange);
            _RepositoryMock.Raise(r => r.TradeOccurredEvent += null, trade);
            _ItemListMock.Verify(i => i.AddTrades(new List<Trade>() { trade }), Times.Exactly(1));
        }

        [Test]
        public void when_repository_raises_trade_event_for_different_symbol_then_item_list_not_updated()
        {
            Trade trade = InstrumentFactory.CreateTrade("AGL", _Instrument.Exchange);
            _RepositoryMock.Raise(r => r.TradeOccurredEvent += null, trade);
            _ItemListMock.Verify(i => i.AddTrades(new List<Trade>() { trade }), Times.Never());
        }

        [Test]
        public void when_orderbook_raises_depth_price_added_event_for_current_symbol_then_item_list_updated_with_depth()
        {
            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice(_Instrument.Symbol, _Instrument.Exchange, 1, true);
            _OrderbookMock.Raise(r => r.DepthByPriceItemAddedEvent += null, depth, true);
            _ItemListMock.Verify(i => i.UpdateDepth(depth, true), Times.Exactly(1));
        }

        [Test]
        public void when_orderbook_raises_depth_price_added_event_for_different_symbol_then_item_list_not_updated()
        {
            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice("AGL", _Instrument.Exchange, 1, true);
            _OrderbookMock.Raise(r => r.DepthByPriceItemAddedEvent += null, depth, true);
            _ItemListMock.Verify(i => i.UpdateDepth(depth, true), Times.Never());
        }

        [Test]
        public void when_orderbook_raises_depth_price_updated_event_for_current_symbol_then_item_list_updated_with_depth()
        {
            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice(_Instrument.Symbol, _Instrument.Exchange, 1, true);
            _OrderbookMock.Raise(r => r.DepthByPriceItemUpdatedEvent += null, depth, true);
            _ItemListMock.Verify(i => i.UpdateDepth(depth, true), Times.Exactly(1));
        }

        [Test]
        public void when_orderbook_raises_depth_price_updated_event_for_different_symbol_then_item_list_not_updated()
        {
            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice("AGL", _Instrument.Exchange, 1, true);
            _OrderbookMock.Raise(r => r.DepthByPriceItemUpdatedEvent += null, depth, true);
            _ItemListMock.Verify(i => i.UpdateDepth(depth, true), Times.Never());
        }

        [Test]
        public void when_orderbook_raises_depth_price_deleted_event_for_current_symbol_then_remove_from_item_list()
        {
            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice(_Instrument.Symbol, _Instrument.Exchange, 1, true);
            _OrderbookMock.Raise(r => r.DepthByPriceItemDeletedEvent += null, depth, true);
            _ItemListMock.Verify(i => i.RemoveDepth(depth.Price, true), Times.Exactly(1));
        }

        [Test]
        public void when_orderbook_raises_depth_price_deleted_event_for_different_symbol_then_do_nothing()
        {
            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice("AGL", _Instrument.Exchange, 1, true);
            _OrderbookMock.Raise(r => r.DepthByPriceItemDeletedEvent += null, depth, true);
            _ItemListMock.Verify(i => i.RemoveDepth(depth.Price, true), Times.Never());
        }

        [Test]
        public void when_item_list_raises_matrix_item_added_event_then_view_updated()
        {
            MatrixViewItem item = InstrumentFactory.CreateMatrixViewItem();
            _ItemListMock.Raise(r => r.ItemsAdded += null, new List<MatrixViewItem>() { item });
            _ViewMock.Verify(i => i.AddGridRowItems(new List<MatrixViewItem>() { item }), Times.Exactly(1));
        }

        [Test]
        public void when_item_list_raises_matrix_item_updated_event_then_view_updated()
        {
            MatrixViewItem item = InstrumentFactory.CreateMatrixViewItem();
            _ItemListMock.Raise(r => r.ItemsUpdated += null, new List<MatrixViewItem>() { item });
            _ViewMock.Verify(i => i.UpdateGridRowItems(new List<MatrixViewItem>() { item }), Times.Exactly(1));
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

            _matrixController.Subscribe(symbol, exchange);

            _RepositoryMock.Verify(r => r.SubscribeLevelOneWatch(symbol, exchange), Times.Exactly(1));
            _RepositoryMock.Verify(r => r.SubscribeLevelTwoWatch(symbol, exchange), Times.Exactly(1));
            _RepositoryMock.Verify(r => r.GetTrades(exchange, symbol, 50), Times.Exactly(1));

            _ViewMock.Verify(v => v.UpdateGridWithInstrument(expectedInstrument), Times.Exactly(1));

            _ItemListMock.Verify(c => c.AddInstrument(expectedInstrument), Times.Exactly(1));
            _ItemListMock.Verify(c => c.AddTrades(expectedTrades), Times.Exactly(1));

            Assert.AreEqual(expectedInstrument, _matrixController.Instrument);
        }
    }
}
