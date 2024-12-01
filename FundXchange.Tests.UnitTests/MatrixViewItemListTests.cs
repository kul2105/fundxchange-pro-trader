using System;
using NUnit.Framework;
using FundXchange.Model.ViewModels.Matrix;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.Infrastructure;
using FundXchange.Domain.Entities;
using System.Threading;
using System.Collections.Generic;
using FundXchange.Model.Agents;

namespace FundXchange.Tests.UnitTests
{
    [TestFixture]
    public class given_matrix_view_item_list_is_empty
    {
        private MatrixViewItemList _ListUnderTest;
        private string _Symbol = "NPN";
        private string _Exchange = "JSE";

        [SetUp]
        public void Setup()
        {
            _ListUnderTest = new MatrixViewItemList();
        }

        [Test]
        public void when_no_items_then_empty_list_returned()
        {
            Assert.AreEqual(0, _ListUnderTest.Values.Count, "List count must be empty");
        }

        [Test]
        public void when_instrument_added_with_different_values_then_ohlc_set_and_events_raised()
        {
            Instrument instrument = InstrumentFactory.CreateInstrument(_Symbol, _Exchange);
            instrument.High = 10;
            instrument.Low = 1;
            instrument.YesterdayClose = 3;
            instrument.Open = 5;
            instrument.LastTrade = 8;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsAdded += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected) 
                {
                    are.Set();
                }
            };

            _ListUnderTest.AddInstrument(instrument);
            are.WaitOne();

            Assert.AreEqual(5, _ListUnderTest.Values.Count, "List count expected to be 5");

            Assert.AreEqual(instrument.Low, _ListUnderTest.Values[0].Price, String.Format("Price ({0}) expected to be equal to Low ({1})", _ListUnderTest.Values[0].Price, instrument.Low));
            Assert.AreEqual(instrument.YesterdayClose, _ListUnderTest.Values[1].Price, String.Format("Price ({0}) expected to be equal to YesterdayClose ({1})", _ListUnderTest.Values[0].Price, instrument.YesterdayClose));
            Assert.AreEqual(instrument.Open, _ListUnderTest.Values[2].Price, String.Format("Price ({0}) expected to be equal to Open ({1})", _ListUnderTest.Values[0].Price, instrument.Open));
            Assert.AreEqual(instrument.LastTrade, _ListUnderTest.Values[3].Price, String.Format("Price ({0}) expected to be equal to LastTrade ({1})", _ListUnderTest.Values[0].Price, instrument.Low));
            Assert.AreEqual(instrument.High, _ListUnderTest.Values[4].Price, String.Format("Price ({0}) expected to be equal to High ({1})", _ListUnderTest.Values[0].Price, instrument.High));
        }

        [Test]
        public void when_instrument_added_with_same_values_then_ohlc_set_and_events_raised()
        {
            Instrument instrument = InstrumentFactory.CreateInstrument(_Symbol, _Exchange);
            instrument.High = 1;
            instrument.Low = 1;
            instrument.YesterdayClose = 1;
            instrument.Open = 1;
            instrument.LastTrade = 1;

            int numAddEventsExpected = 1;
            int numUpdateEventsExpected = 1;
            int addEventsReceived = 0;
            int updateEventsReceived = 0;

            AutoResetEvent addReset = new AutoResetEvent(false);
            AutoResetEvent updateReset = new AutoResetEvent(false);

            _ListUnderTest.ItemsAdded += delegate
            {
                addEventsReceived++;
                if (addEventsReceived == numAddEventsExpected)
                {
                    addReset.Set();
                }
            };
            _ListUnderTest.ItemsUpdated += delegate
            {
                updateEventsReceived++;
                if (updateEventsReceived == numUpdateEventsExpected)
                {
                    updateReset.Set();
                }
            };

            _ListUnderTest.AddInstrument(instrument);
            addReset.WaitOne();
            updateReset.WaitOne();

            Assert.AreEqual(1, _ListUnderTest.Values.Count, "List count expected to be equal to 1");
            Assert.AreEqual(instrument.Low, _ListUnderTest.Values[0].Price, String.Format("Price ({0}) expected to be equal to Low ({1})", _ListUnderTest.Values[0].Price, instrument.Low));
        }

        [Test]
        public void when_trade_added_then_one_item_added_event_raised()
        {
            Trade trade = InstrumentFactory.CreateTrade(_Symbol, _Exchange);
            trade.Price = 5;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsAdded += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.AddTrades(new List<Trade>() { trade });
            are.WaitOne();

            Assert.AreEqual(1, _ListUnderTest.Values.Count, "List count expected to be 1");

            Assert.AreEqual(trade.Price, _ListUnderTest.Values[0].Price, String.Format("Price ({0}) expected to be equal to Trade Price ({1})", _ListUnderTest.Values[0].Price, trade.Price));
            Assert.AreEqual(trade.Volume, _ListUnderTest.Values[0].TradeVolume, String.Format("Volume ({0}) expected to be equal to Trade Volume ({1})", _ListUnderTest.Values[0].TradeVolume, trade.Volume));
            Assert.AreEqual(0, _ListUnderTest.Values[0].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest.Values[0].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest.Values[0].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(0, _ListUnderTest.Values[0].BidOrderCount, "Bid Order Count expected to be zero");
            Assert.AreEqual(0, _ListUnderTest.Values[0].BidSize, "Bid Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest.Values[0].BidSizeCell.Text, "Bid Size Display expected to be ''");
        }

        [Test]
        public void when_depth_item_added_then_one_item_added_event_raised()
        {
            int numOrders = 1;
            bool isBuy = true;

            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice(_Symbol, _Exchange, numOrders, isBuy);
            depth.Price = 5;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsAdded += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.UpdateDepth(depth, isBuy);
            are.WaitOne();

            Assert.AreEqual(1, _ListUnderTest.Values.Count, "List count expected to be 1");

            Assert.AreEqual(depth.Price, _ListUnderTest.Values[0].Price, String.Format("Price ({0}) expected to be equal to Depth Price ({1})", _ListUnderTest.Values[0].Price, depth.Price));
            Assert.AreEqual(0, _ListUnderTest.Values[0].TradeVolume, "TradeVolume expected to be zero");
            Assert.AreEqual(0, _ListUnderTest.Values[0].OfferOrderCount, "OfferOrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest.Values[0].OfferSize, "OfferSize expected to be zero");
            Assert.AreEqual("", _ListUnderTest.Values[0].OfferSizeCell.Text, "OfferSizeDisplay expected to be ''");
            Assert.AreEqual(1, _ListUnderTest.Values[0].BidOrderCount, "BidOrderCount expected to be 1");
            Assert.AreEqual(depth.Volume, _ListUnderTest.Values[0].BidSize, String.Format("BidSize expected to be {0}", depth.Volume));
            Assert.AreEqual(String.Format("{0} (1)", depth.Volume), _ListUnderTest.Values[0].BidSizeCell.Text, String.Format("BidSizeDisplay expected to be '{0} (1)'", depth.Volume));
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_item_removed_then_exception_expected()
        {
            _ListUnderTest.RemoveDepth(9, true);
        }
    }

    [TestFixture]
    public class given_matrix_view_item_list_contains_instrument_ohlc
    {
        private MatrixViewItemList _ListUnderTest;
        private Instrument _Instrument;
        private string _Symbol = "NPN";
        private string _Exchange = "JSE";

        private long _OpenPrice = 3;
        private long _LowPrice = 2;
        private long _HighPrice = 10;
        private long _YesterdayClosePrice = 5;
        private long _LastTradePrice = 8;

        [SetUp]
        public void Setup()
        {
            _ListUnderTest = new MatrixViewItemList();

            _Instrument = InstrumentFactory.CreateInstrument(_Symbol, _Exchange);
            _Instrument.Low = _LowPrice;
            _Instrument.High = _HighPrice;
            _Instrument.Open = _OpenPrice;
            _Instrument.YesterdayClose = _YesterdayClosePrice;
            _Instrument.LastTrade = _LastTradePrice;

            _ListUnderTest.AddInstrument(_Instrument);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void when_another_instrument_added_then_exception_expected()
        {
             Instrument newInstrument = InstrumentFactory.CreateInstrument(_Symbol, _Exchange);
            _ListUnderTest.AddInstrument(newInstrument);
        }

        [Test]
        public void when_new_low_trade_added_then_last_trade_and_low_set_and_one_item_added_event_raised()
        {
            Trade trade = InstrumentFactory.CreateTrade(_Symbol, _Exchange);
            trade.Price = 1;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsAdded += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.AddTrades(new List<Trade>() { trade });
            are.WaitOne();

            Assert.AreEqual(6, _ListUnderTest.Values.Count, "List count expected to be 6");

            Assert.AreEqual(trade.Price, _ListUnderTest.Values[0].Price, String.Format("Low Price ({0}) expected to be equal to Trade Price ({1})", _ListUnderTest.Values[0].Price, trade.Price));
            Assert.AreEqual(trade.Volume, _ListUnderTest.Values[0].TradeVolume, String.Format("Volume ({0}) expected to be equal to Trade Volume ({1})", _ListUnderTest.Values[0].TradeVolume, trade.Volume));
            Assert.AreEqual(0, _ListUnderTest.Values[0].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest.Values[0].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest.Values[0].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(0, _ListUnderTest.Values[0].BidOrderCount, "Bid Order Count expected to be zero");
            Assert.AreEqual(0, _ListUnderTest.Values[0].BidSize, "Bid Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest.Values[0].BidSizeCell.Text, "Bid Size Display expected to be ''");
        }

        [Test]
        public void when_new_high_trade_added_then_last_trade_and_high_set_and_one_item_added_event_raised()
        {
            Trade trade = InstrumentFactory.CreateTrade(_Symbol, _Exchange);
            trade.Price = 11;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsAdded += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.AddTrades(new List<Trade>() { trade });
            are.WaitOne();

            Assert.AreEqual(6, _ListUnderTest.Values.Count, "List count expected to be 6");

            Assert.AreEqual(trade.Price, _ListUnderTest.Values[_ListUnderTest.Values.Count - 1].Price, String.Format("Low Price ({0}) expected to be equal to Trade Price ({1})", _ListUnderTest.Values[_ListUnderTest.Values.Count - 1].Price, trade.Price));

            Assert.AreEqual(trade.Volume, _ListUnderTest.Values[_ListUnderTest.Values.Count - 1].TradeVolume, String.Format("Volume ({0}) expected to be equal to Trade Volume ({1})", _ListUnderTest.Values[0].TradeVolume, trade.Volume));
            Assert.AreEqual(0, _ListUnderTest.Values[_ListUnderTest.Values.Count - 1].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest.Values[_ListUnderTest.Values.Count - 1].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest.Values[_ListUnderTest.Values.Count - 1].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(0, _ListUnderTest.Values[_ListUnderTest.Values.Count - 1].BidOrderCount, "Bid Order Count expected to be zero");
            Assert.AreEqual(0, _ListUnderTest.Values[_ListUnderTest.Values.Count - 1].BidSize, "Bid Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest.Values[_ListUnderTest.Values.Count - 1].BidSizeCell.Text, "Bid Size Display expected to be ''");
        }

        [Test]
        public void when_trade_equal_to_open_added_then_last_trade_set_and_one_item_updated_event_raised()
        {
            Trade trade = InstrumentFactory.CreateTrade(_Symbol, _Exchange);
            trade.Price = _OpenPrice;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsUpdated += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.AddTrades(new List<Trade>() { trade });
            are.WaitOne();

            Assert.AreEqual(5, _ListUnderTest.Values.Count, "List count expected to be 5");

            Assert.AreEqual(true, _ListUnderTest.ContainsKey(trade.Price), "List expected to contain new price");

            Assert.AreEqual(trade.Volume, _ListUnderTest[trade.Price].TradeVolume, String.Format("Volume ({0}) expected to be equal to Trade Volume ({1})", _ListUnderTest.Values[0].TradeVolume, trade.Volume));
            Assert.AreEqual(0, _ListUnderTest[trade.Price].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[trade.Price].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].BidOrderCount, "Bid Order Count expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].BidSize, "Bid Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[trade.Price].BidSizeCell.Text, "Bid Size Display expected to be ''");
        }

        [Test]
        public void when_trade_equal_to_high_added_then_last_trade_set_and_one_item_updated_event_raised()
        {
            Trade trade = InstrumentFactory.CreateTrade(_Symbol, _Exchange);
            trade.Price = _HighPrice;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsUpdated += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.AddTrades(new List<Trade>() { trade });
            are.WaitOne();

            Assert.AreEqual(5, _ListUnderTest.Values.Count, "List count expected to be 5");

            Assert.AreEqual(true, _ListUnderTest.ContainsKey(trade.Price), "List expected to contain new price");

            Assert.AreEqual(trade.Volume, _ListUnderTest[trade.Price].TradeVolume, String.Format("Volume ({0}) expected to be equal to Trade Volume ({1})", _ListUnderTest.Values[0].TradeVolume, trade.Volume));
            Assert.AreEqual(0, _ListUnderTest[trade.Price].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[trade.Price].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].BidOrderCount, "Bid Order Count expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].BidSize, "Bid Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[trade.Price].BidSizeCell.Text, "Bid Size Display expected to be ''");
        }

        [Test]
        public void when_trade_equal_to_low_added_then_last_trade_set_and_one_item_updated_event_raised()
        {
            Trade trade = InstrumentFactory.CreateTrade(_Symbol, _Exchange);
            trade.Price = _LowPrice;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsUpdated += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.AddTrades(new List<Trade>() { trade });
            are.WaitOne();

            Assert.AreEqual(5, _ListUnderTest.Values.Count, "List count expected to be 5");

            Assert.AreEqual(true, _ListUnderTest.ContainsKey(trade.Price), "List expected to contain new price");

            Assert.AreEqual(trade.Volume, _ListUnderTest[trade.Price].TradeVolume, String.Format("Volume ({0}) expected to be equal to Trade Volume ({1})", _ListUnderTest.Values[0].TradeVolume, trade.Volume));
            Assert.AreEqual(0, _ListUnderTest[trade.Price].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[trade.Price].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].BidOrderCount, "Bid Order Count expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].BidSize, "Bid Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[trade.Price].BidSizeCell.Text, "Bid Size Display expected to be ''");
        }

        [Test]
        public void when_trade_equal_to_close_added_then_last_trade_set_and_one_item_updated_event_raised()
        {
            Trade trade = InstrumentFactory.CreateTrade(_Symbol, _Exchange);
            trade.Price = _YesterdayClosePrice;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsUpdated += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.AddTrades(new List<Trade>() { trade });
            are.WaitOne();

            Assert.AreEqual(5, _ListUnderTest.Values.Count, "List count expected to be 5");

            Assert.AreEqual(true, _ListUnderTest.ContainsKey(trade.Price), "List expected to contain new price");

            Assert.AreEqual(trade.Volume, _ListUnderTest[trade.Price].TradeVolume, String.Format("Volume ({0}) expected to be equal to Trade Volume ({1})", _ListUnderTest.Values[0].TradeVolume, trade.Volume));
            Assert.AreEqual(0, _ListUnderTest[trade.Price].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[trade.Price].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].BidOrderCount, "Bid Order Count expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].BidSize, "Bid Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[trade.Price].BidSizeCell.Text, "Bid Size Display expected to be ''");
        }

        [Test]
        public void when_trade_added_then_last_trade_set_and_one_item_added_event_raised()
        {
            Trade trade = InstrumentFactory.CreateTrade(_Symbol, _Exchange);
            trade.Price = 7;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsAdded += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.AddTrades(new List<Trade>() { trade });
            are.WaitOne();

            Assert.AreEqual(6, _ListUnderTest.Values.Count, "List count expected to be 5");

            Assert.AreEqual(true, _ListUnderTest.ContainsKey(trade.Price), "List expected to contain new price");

            Assert.AreEqual(trade.Volume, _ListUnderTest[trade.Price].TradeVolume, String.Format("Volume ({0}) expected to be equal to Trade Volume ({1})", _ListUnderTest.Values[0].TradeVolume, trade.Volume));
            Assert.AreEqual(0, _ListUnderTest[trade.Price].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[trade.Price].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].BidOrderCount, "Bid Order Count expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[trade.Price].BidSize, "Bid Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[trade.Price].BidSizeCell.Text, "Bid Size Display expected to be ''");
        }

        [Test]
        public void when_new_depth_added_then_one_item_added_event_raised()
        {
            int orderCount = 2;
            bool isBuy = true;

            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice(_Symbol, _Exchange, orderCount, isBuy);
            depth.Price = 7;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsAdded += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.UpdateDepth(depth, isBuy);
            are.WaitOne();

            Assert.AreEqual(6, _ListUnderTest.Values.Count, "List count expected to be 6");

            Assert.AreEqual(true, _ListUnderTest.ContainsKey(depth.Price), "List expected to contain new Depth Price");

            Assert.AreEqual(0, _ListUnderTest[depth.Price].TradeVolume, "Trade Volume expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[depth.Price].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[depth.Price].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[depth.Price].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(orderCount, _ListUnderTest[depth.Price].BidOrderCount, String.Format("Bid Order Count ({0}) expected to be {1}", _ListUnderTest[depth.Price].BidOrderCount, orderCount));
            Assert.AreEqual(depth.Volume, _ListUnderTest[depth.Price].BidSize, String.Format("Bid Size ({0}) expected to be {1}", _ListUnderTest[depth.Price].BidSize, depth.Volume));
            Assert.AreEqual(String.Format("{0} ({1})", depth.Volume, orderCount), _ListUnderTest[depth.Price].BidSizeCell.Text, String.Format("Bid Size Display expected to be '{0} ({1})'", depth.Volume, orderCount));
        }

        [Test]
        public void when_new_depth_added_equal_to_previous_trade_then_one_item_updated_event_raised()
        {
            int orderCount = 2;
            bool isBuy = true;

            Trade trade = InstrumentFactory.CreateTrade(_Symbol, _Exchange);
            trade.Price = 7;

            _ListUnderTest.AddTrades(new List<Trade>() { trade });

            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice(_Symbol, _Exchange, orderCount, isBuy);
            depth.Price = 7;

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsUpdated += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.UpdateDepth(depth, isBuy);
            are.WaitOne();

            Assert.AreEqual(6, _ListUnderTest.Values.Count, "List count expected to be 6");

            Assert.AreEqual(true, _ListUnderTest.ContainsKey(depth.Price), "List expected to contain Depth Price");

            Assert.AreEqual(trade.Volume, _ListUnderTest[depth.Price].TradeVolume, "Trade Volume expected to be " + trade.Volume);
            Assert.AreEqual(0, _ListUnderTest[depth.Price].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[depth.Price].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[depth.Price].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(orderCount, _ListUnderTest[depth.Price].BidOrderCount, String.Format("Bid Order Count ({0}) expected to be {1}", _ListUnderTest[depth.Price].BidOrderCount, orderCount));
            Assert.AreEqual(depth.Volume, _ListUnderTest[depth.Price].BidSize, String.Format("Bid Size ({0}) expected to be {1}", _ListUnderTest[depth.Price].BidSize, depth.Volume));
            Assert.AreEqual(String.Format("{0} ({1})", depth.Volume, orderCount), _ListUnderTest[depth.Price].BidSizeCell.Text, String.Format("Bid Size Display expected to be '{0} ({1})'", depth.Volume, orderCount));
        }

        [Test]
        public void when_depth_removed_where_trade_exists_then_one_item_updated_event_raised()
        {
            int orderCount = 2;
            bool isBuy = true;

            Trade trade = InstrumentFactory.CreateTrade(_Symbol, _Exchange);
            trade.Price = 7;

            _ListUnderTest.AddTrades(new List<Trade>() { trade });

            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice(_Symbol, _Exchange, orderCount, isBuy);
            depth.Price = 7;

            _ListUnderTest.UpdateDepth(depth, isBuy);

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsUpdated += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.RemoveDepth(depth.Price, isBuy);
            are.WaitOne();

            Assert.AreEqual(6, _ListUnderTest.Values.Count, "List count expected to be 6");

            Assert.AreEqual(true, _ListUnderTest.ContainsKey(depth.Price), "List expected to contain Depth Price");

            Assert.AreEqual(trade.Volume, _ListUnderTest[depth.Price].TradeVolume, "Trade Volume expected to be " + trade.Volume);
            Assert.AreEqual(0, _ListUnderTest[depth.Price].OfferOrderCount, "Offer OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[depth.Price].OfferSize, "Offer Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[depth.Price].OfferSizeCell.Text, "Offer Size Display expected to be ''");
            Assert.AreEqual(0, _ListUnderTest[depth.Price].BidOrderCount, "Bid OrderCount expected to be zero");
            Assert.AreEqual(0, _ListUnderTest[depth.Price].BidSize, "Bid Size expected to be zero");
            Assert.AreEqual("", _ListUnderTest[depth.Price].BidSizeCell.Text, "Bid Size Display expected to be ''");
        }

        [Test]
        public void when_depth_removed_where_no_trade_exists_then_one_item_removed_event_raised()
        {
            int orderCount = 2;
            bool isBuy = true;

            DepthByPrice depth = InstrumentFactory.CreateDepthByPrice(_Symbol, _Exchange, orderCount, isBuy);
            depth.Price = 7;

            _ListUnderTest.UpdateDepth(depth, isBuy);

            int numEventsExpected = 1;
            int eventsReceived = 0;
            AutoResetEvent are = new AutoResetEvent(false);

            _ListUnderTest.ItemsRemoved += delegate
            {
                eventsReceived++;
                if (eventsReceived == numEventsExpected)
                {
                    are.Set();
                }
            };

            _ListUnderTest.RemoveDepth(depth.Price, isBuy);
            are.WaitOne();

            Assert.AreEqual(5, _ListUnderTest.Values.Count, "List count expected to be 5");
        }
    }
}
