using System;
using System.Collections.Generic;
using System.Linq;
using fin24.Patterns.DomainPatterns.IoC;
using fin24.Patterns.InfrastructurePatterns.Logging;
using Fin24.Markets.ClientSubscription;
using Fin24.Markets.Client.StateRequest;
using Fin24.Markets.State;
using FundXchange.Model.Infrastructure;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using FundXchange.DataProviderContracts;
using FundXchange.Domain.Enumerations;
using System.Windows.Forms;
using FundXchange.Model.Exceptions;
using System.ServiceModel;

namespace FundXchange.DataProvider
{
    public class RealTimeDataProvider : IRealTimeDataProvider
    {
        #region Private Members

        private SubscriptionService _SubscriptionService = null;
        private ErrorService _ErrorService;

        private IList<Fin24.Markets.Client.StateRequest.OrderDTO> tmpOrders = new List<Fin24.Markets.Client.StateRequest.OrderDTO>();
        private IList<BidDTO> tmpBids = new List<BidDTO>();
        private IList<OfferDTO> tmpOffers = new List<OfferDTO>();

        private List<string> levelOneSymbols = new List<string>();
        private List<string> indexSymbols = new List<string>();
        private string levelTwoSymbol = "";
        private string _RoyaltyAgreementIdentifier;

        #endregion

        #region Events Declarations

        public event InstrumentUpdatedDelegate InstrumentUpdatedEvent;
        public event IndexUpdatedDelegate IndexUpdatedEvent;
        public event TradeOccurredDelegate TradeOccurredEvent;
        public event BidsChangedDelegate BidsChangedEvent;
        public event OffersChangedDelegate OffersChangedEvent;
        public event OrdersChangedDelegate OrdersChangedEvent;
        public event QuoteOccurredDelegate QuoteOccurredEvent;

        #endregion

        #region Constructors

        public RealTimeDataProvider(string royaltyAgreementIdentifier)
        {
            _ErrorService = FundXchange.Infrastructure.IoC.Resolve<ErrorService>();
            _RoyaltyAgreementIdentifier = royaltyAgreementIdentifier;

            Connect();
        }

        #endregion

        #region IRealTimeDataProvider Members

        public string VendorName
        {
            get
            {
                return "McGregor BFA";
            }
        }

        public void Connect()
        {
            var resolver = new DependencyResolver();

            resolver.Register<ILog>(new Logger());
            fin24.Patterns.DomainPatterns.IoC.IoC.Initialize(resolver);

            _SubscriptionService = SubscriptionServiceFactory.CreateSubscriptionService(resolver);
            ListenToSubscriptionServiceEvents();
            _SubscriptionService.Start(_RoyaltyAgreementIdentifier, "TV", true);
        }

        public void Disconnect()
        {
            _SubscriptionService.Stop();
        }

        private void ListenToSubscriptionServiceEvents()
        {
            _SubscriptionService.TradeOccured += subscriptionService_TradeOccured;
            _SubscriptionService.BidsChanged += subscriptionService_BidsChanged;
            _SubscriptionService.InstrumentUpdate += subscriptionService_InstrumentUpdate;
            _SubscriptionService.NewQuote += subscriptionService_NewQuote;
            _SubscriptionService.OffersChanged += subscriptionService_OffersChanged;
            _SubscriptionService.OrdersChanged += _SubscriptionService_OrdersChanged;
            _SubscriptionService.IndexUpdate += subscriptionService_IndexUpdate;
        }

        public Instrument SubscribeLevelOneWatch(string symbol, string exchange)
        {
            try
            {
                Fin24.Markets.State.InstrumentDTO instrumentDTO;
                IList<Fin24.Markets.State.TradeDTO> tradesDTO;
                _SubscriptionService.RegisterForInstrumentViewEvents(symbol, exchange, out instrumentDTO, out tradesDTO);
                Instrument instrument = DataAdapter.GetInstrumentsFromDTOs(instrumentDTO);

                if (!levelOneSymbols.Contains(symbol))
                {
                    levelOneSymbols.Add(symbol);
                }

                RaiseInstrumentUpdated(instrument);
                return instrument;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not a valid identification"))
                {
                    throw new SymbolNotValidException(symbol, exchange);
                }
                else
                {
                    throw new SymbolNotFoundException(symbol, exchange);
                }
            }
        }

        private void RaiseInstrumentUpdated(Instrument instrument)
        {
            if (InstrumentUpdatedEvent != null)
            {
                InstrumentUpdatedEvent(instrument);
            }
        }

        public void UnsubscribeLevelOneWatch(string symbol, string exchange)
        {
            try
            {
                _SubscriptionService.UnRegisterForInstrumentViewEvents(symbol, exchange);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("Exception: (UnsubscribeLevelOneWatch)", ex);
            }
        }

        public void SubscribeIndexWatch(string symbol, string exchange)
        {
            if (!indexSymbols.Contains(symbol))
            {
                indexSymbols.Add(symbol);
            }

            try
            {
                _SubscriptionService.RegisterForIndexViewEvents(symbol, exchange);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("Exception: (SubscribeIndexWatch)", ex);
            }
        }

        public void UnsubscribeIndexWatch(string symbol, string exchange)
        {
            try
            {
                _SubscriptionService.UnRegisterForIndexViewEvents(symbol, exchange);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("Exception: (UnsubscribeIndexWatch)", ex);
            }
        }

        public bool SubscribeLevelTwoWatch(string symbol, string exchange, out List<Bid> bids, out List<Offer> offers, out List<Order> orders)
        {
            levelTwoSymbol = symbol;

            IList<BidDTO> bidDTOs = null;
            IList<OfferDTO> offerDTOs = null;
            IList<Fin24.Markets.State.OrderDTO> orderDTOs = null;

            _SubscriptionService.RegisterForOrderBookAndDepthViewEvents(symbol, exchange, out bidDTOs, out offerDTOs, out orderDTOs);

            bids = DataAdapter.GetBidsFromDTOs(bidDTOs);
            offers = DataAdapter.GetOffersFromDTOs(offerDTOs);
            orders = DataAdapter.GetOrdersFromDTOs(orderDTOs);

            return true;
        }

        public bool UnsubscribeLevelTwoWatch(string symbol, string exchange)
        {
            try
            {
                _SubscriptionService.UnRegisterForOrderBookAndDepthViewEvents(symbol, exchange);

                tmpOffers.Clear();
                tmpOrders.Clear();
                tmpBids.Clear();

                return true;
            }
            catch (Exception ex)
            {
                _ErrorService.LogError("Exception: (UnsubscribeLevelTwoWatch)", ex);
            }
            return false;
        }

        public List<InstrumentSnapshot> GetInstrumentSnapshotHistory(string symbol, string exchange, int interval, int barsCount)
        {
            IList<Fin24.Markets.State.SnapshotDTO> retList;
            List<InstrumentSnapshot> snapshots = new List<InstrumentSnapshot>();

            try
            {
                long result = _SubscriptionService.GetSnapshotHistoryView(symbol, exchange, interval, barsCount, out retList);
                snapshots.AddRange(DataAdapter.GetSnapshotsFromDTOs(retList));
            }
            catch (FaultException<Fin24.Markets.Client.StateRequest.DomainFaultException> ex)
            {
                throw new Exception(ex.Detail.Reason);
            }
            return snapshots;
        }

        public List<InstrumentSnapshot> GetIndexSnapshotHistory(string index, string exchange, int interval, int barsCount)
        {
            return new List<InstrumentSnapshot>();
        }

        public List<Instrument> GetTop20InstrumentsByValue(string exchange)
        {
            try
            {
                InstrumentSummaryDTO[] dtos = _SubscriptionService.GetTop20InstrumentsByValue();
                List<Instrument> instruments = DataAdapter.GetInstrumentsFromInstrumentSummaryDTOs(dtos);
                return instruments;
            }
            catch (Exception)
            {
                return new List<Instrument>();
            }
        }

        public List<Instrument> GetTop20InstrumentsByVolume(string exchange)
        {
            try
            {
                InstrumentSummaryDTO[] dtos = _SubscriptionService.GetTop20InstrumentsByVolume();
                List<Instrument> instruments = DataAdapter.GetInstrumentsFromInstrumentSummaryDTOs(dtos);
                return instruments;
            }
            catch (Exception)
            {
                return new List<Instrument>();
            }
        }

        public List<Instrument> GetWinners(string exchange)
        {
            try
            {
                MoversDTO[] dtos = _SubscriptionService.GetWinnersAndLosers();
                List<Instrument> instuments = DataAdapter.GetInstrumentsFromMoversDTOs(dtos);
                List<Instrument> winners = instuments.Take(20).ToList();
                List<Instrument> positive = winners.Where(i => i.PercentageMoved >= 0).ToList();
                return positive;
            }
            catch (Exception)
            {
                return new List<Instrument>();
            }
        }

        public List<Instrument> GetLosers(string exchange)
        {
            try
            {
                MoversDTO[] dtos = _SubscriptionService.GetWinnersAndLosers();
                List<Instrument> instuments = DataAdapter.GetInstrumentsFromMoversDTOs(dtos);
                List<Instrument> losers = instuments.Skip(20).Take(20).ToList();
                List<Instrument> negative = losers.Where(i => i.PercentageMoved <= 0).ToList();
                return negative;
            }
            catch (Exception)
            {
                return new List<Instrument>();
            }
        }

        public List<Index> GetIndexes(string exchange)
        {
            List<Index> indexes = new List<Index>();
            try
            {
                Fin24.Markets.Client.StateRequest.InstrumentWithDescriptionDTO[] dtos = _SubscriptionService.GetInstrumentsForType(EnumerationsInstrumentType.Index);
                foreach (Fin24.Markets.Client.StateRequest.InstrumentWithDescriptionDTO instrument in dtos)
                {
                    instrument.ShareShortName = instrument.ShareShortName.Trim();
                    Index index = DataAdapter.GetIndexFromInstrumentDTO(instrument);
                    indexes.Add(index);
                }
            }
            catch (Exception)
            {
            }
            return indexes;
        }

        public List<Trade> GetTradesForInstrument(string symbol)
        {
            List<Trade> trades = new List<Trade>();
            InstrumentAndTradesDTO dto = GetInstrumentsWithTrades(symbol);

            foreach (Fin24.Markets.Client.StateRequest.TradeDTO tradeDTO in dto.Trades)
            {
                Trade trade = DataAdapter.GetTradeFromDTO(tradeDTO);
                trades.Add(trade);
            }
            return trades;
        }

        private InstrumentAndTradesDTO GetInstrumentsWithTrades(string symbol)
        {
            InstrumentAndTradesDTO instrumentsWithTrades = null;
            try
            {
                instrumentsWithTrades = _SubscriptionService.GetInstrumentWithTrades(symbol);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
            }
            return instrumentsWithTrades;
        }

        public List<InstrumentReference> GetInstrumentReferencesForType(InstrumentType type)
        {
            EnumerationsInstrumentType enumType = (EnumerationsInstrumentType) Enum.Parse(typeof(EnumerationsInstrumentType), type.ToString());
            InstrumentReferenceDTO[] dtos = null;
            try
            {
                dtos = _SubscriptionService.GetInstrumentReferenceForType(enumType);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
            }
            return DataAdapter.GetInstrumentReferencesFromDTOs(dtos);
        }

        public List<Instrument> GetTop40Instruments(string exchange)
        {
            MoversDTO[] dtos = null;
            try
            {
                dtos = _SubscriptionService.GetTop40Instruments();
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
            }
            List<Instrument> instuments = DataAdapter.GetInstrumentsFromMoversDTOs(dtos);
            return instuments;
        }

        public List<FundXchange.Domain.ValueObjects.InstrumentStatistics> GetMarketsHomeData(string exchange)
        {
            List<Fin24.Markets.Client.StateRequest.InstrumentStatistics> instrumentList = new List<Fin24.Markets.Client.StateRequest.InstrumentStatistics>();

            try
            {
                instrumentList.AddRange(_SubscriptionService.GetInstrumentStatistics(InstrumentStatisticType.Currencies, exchange));
                instrumentList.AddRange(_SubscriptionService.GetInstrumentStatistics(InstrumentStatisticType.JSEIndices, exchange));
                instrumentList.AddRange(_SubscriptionService.GetInstrumentStatistics(InstrumentStatisticType.IntIndices, exchange));
                instrumentList.AddRange(_SubscriptionService.GetInstrumentStatistics(InstrumentStatisticType.Bonds, exchange));
                instrumentList.AddRange(_SubscriptionService.GetInstrumentStatistics(InstrumentStatisticType.Commodities, exchange));
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
            }
            List<FundXchange.Domain.ValueObjects.InstrumentStatistics> statistics = DataAdapter.GetInstrumentStatistics(instrumentList);

            return statistics;
        }

        public List<FundXchange.Domain.ValueObjects.Sens> GetSensData()
        {
            Fin24.Markets.Client.StateRequest.Sens[] sensList = null;
            try
            {
                sensList = _SubscriptionService.GetSens();
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
            } 
            List<FundXchange.Domain.ValueObjects.Sens> sens = DataAdapter.GetSens(sensList);
            return sens;
        }

        public List<InstrumentReference> GetInstrumentReferenceForAllCurrentInstruments()
        {
            InstrumentReferenceDTO[] dtos = null;
            try
            {
                dtos = _SubscriptionService.GetAllInstrumentReferencesForExchange();
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
            }
            return DataAdapter.GetInstrumentReferencesFromDTOs(dtos);
        }

        public List<Trade> GetTrades(string exchange, string symbol, int pageSize)
        {
            try
            {
                List<Fin24.Markets.State.TradeDTO> trades =_SubscriptionService.GetLastTrades(new Fin24.Markets.InstrumentIdentifier(symbol, exchange), pageSize);
                return DataAdapter.GetTradesFromDTOs(trades);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
            }
            return new List<Trade>();
        }

        public List<Quote> GetQuotes(string exchange, string symbol, DateTime fromDate)
        {
            try
            {
                List<Fin24.Markets.State.QuoteDTO> quotes = _SubscriptionService.GetQuotes(new Fin24.Markets.InstrumentIdentifier(symbol, exchange), fromDate);
                return DataAdapter.GetQuotesFromDTOs(quotes);
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
            }
            return new List<Quote>();
        }

        #endregion

        #region Events

        void subscriptionService_InstrumentUpdate(object sender, Fin24.Markets.State.InstrumentDTO instrument)
        {
            Instrument newInstrument = DataAdapter.GetInstrumentsFromDTOs(instrument);
            
            if (InstrumentUpdatedEvent != null)
            {
                InstrumentUpdatedEvent(newInstrument);
            }
        }

        void subscriptionService_IndexUpdate(object sender, IndexDTO index)
        {
            Index newIndex = DataAdapter.GetIndexFromDTOs(index);

            if (IndexUpdatedEvent != null)
            {
                IndexUpdatedEvent(newIndex);
            }
        }

        void subscriptionService_NewQuote(object sender, Fin24.Markets.State.QuoteDTO quote)
        {
            Quote newQuote = DataAdapter.GetQuoteFromQuoteDTO(quote);

            if (QuoteOccurredEvent != null)
            {
                QuoteOccurredEvent(newQuote);
            }
        }

        void subscriptionService_TradeOccured(object sender, Fin24.Markets.State.TradeDTO tradeDTO)
        {
            Trade trade = DataAdapter.GetTradeFromDTO(tradeDTO);

            if (TradeOccurredEvent != null)
            {
                TradeOccurredEvent(trade);
            }
        }

        void _SubscriptionService_OrdersChanged(object sender, IList<Fin24.Markets.State.OrderDTO> orderDTOs)
        {
            List<Order> orders = DataAdapter.GetOrdersFromDTOs(orderDTOs);
            if (OrdersChangedEvent != null)
            {
                OrdersChangedEvent(orders);
            }
        }

        void subscriptionService_OffersChanged(object sender, IList<OfferDTO> offerDTOs)
        {
            List<Offer> offers = DataAdapter.GetOffersFromDTOs(offerDTOs);
            if (OffersChangedEvent != null)
            {
                OffersChangedEvent(offers);
            }
        }

        void subscriptionService_BidsChanged(object sender, IList<BidDTO> bidDTOs)
        {
            List<Bid> bids = DataAdapter.GetBidsFromDTOs(bidDTOs);
            if (BidsChangedEvent != null)
            {
                BidsChangedEvent(bids);
            }
        }

        #endregion

    }
}
