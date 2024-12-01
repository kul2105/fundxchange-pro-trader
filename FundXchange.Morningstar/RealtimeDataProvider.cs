using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using FundXchange.DataProviderContracts;
using FundXchange.Domain.ValueObjects;
using FundXchange.Domain.Entities;
using MorningStar.InteractiveAPI.Wrapper;
using FundXchange.Morningstar.Commands;
using FundXchange.Morningstar.Messages;
using FundXchange.Morningstar.Enumerations;
using FundXchange.Model.ExchangeService;

namespace FundXchange.Morningstar
{
    public class RealtimeDataProvider : IRealTimeDataProvider
    {
        #region Private Members

        private readonly string _royaltyAgreementIdentifier;
        private readonly string _userName;
        private readonly string _userPassword;
        private readonly string _ipAddress;
        private readonly int _port;

        private const int MaxBuffer = 4000;

        private bool _isConnected;
        private Dictionary<string, InstrumentDTO> _AllInstruments;


        private Repository _Repository;

        #endregion

        public string VendorName { get; set; }

        #region Event declarations

        public event InstrumentUpdatedDelegate InstrumentUpdatedEvent;
        public event IndexUpdatedDelegate IndexUpdatedEvent;
        public event TradeOccurredDelegate TradeOccurredEvent;
        public event OrderAddedDelegate OrderAddedEvent;
        public event OrderUpdatedDelegate OrderUpdatedEvent;
        public event OrderDeletedDelegate OrderDeletedEvent;
        public event QuoteOccurredDelegate QuoteOccurredEvent;
        public event ErrorOccurredDelegate ErrorOccurredEvent;

        #endregion

        #region Constructors

        public RealtimeDataProvider(string royaltyAgreementIdentifier, string username, string password, 
            string ipAddress, int port, Dictionary<string, InstrumentDTO> instruments)
        {
            _royaltyAgreementIdentifier = royaltyAgreementIdentifier;
            _userName = username; 
            _userPassword = password;
            _ipAddress = ipAddress;
            _port = port;
            _AllInstruments = instruments;

            _Repository = new Repository();

            Connect();
        }

        #endregion

        public void Connect()
        {
            int connected = ServerApi.SApiInitialize(_userName, _userPassword, _ipAddress, _port, MaxBuffer);

            if (connected > 0)
            {
                _isConnected = true;
                StartListenerThread();
            }
            else
                throw new Exception("Could not connect to MorningStar Server. Please contact your System Administrator");
        }

        public void Disconnect()
        {
            ServerApi.SApiShutDown();
            _isConnected = false;
        }

        public Instrument SubscribeLevelOneWatch(string symbol, string exchange)
        {
            if (_isConnected)
            {
                var setOptionCommand = new Command(InteractiveApiCommands.SetOption)
                    .With(new CommandParameter("Orderbook", "false"));

                TickerApi.TApiSendCommand(setOptionCommand);

                var command = new Command(InteractiveApiCommands.SymbolRequest)
                    .With(new CommandParameter("symbol", symbol, true))
                    .With(new CommandParameter("Mkt", ("" + ((int)Enum.Parse(typeof(Exchanges), exchange)))))
                    .With(new CommandParameter("Sec", "1"))
                    .With(CommandSwitches.ActivateRealtimeUpdates)
                    .With(CommandSwitches.RequestRecap)
                    .With(CommandSwitches.RequestAllFields);

                TickerApi.TApiSendCommand(command);
            }

            return (new Instrument { Symbol = symbol, Exchange = exchange });
        }

        public void UnsubscribeLevelOneWatch(string symbol, string exchange)
        {
            if(_isConnected)
            {
                var command = new Command(InteractiveApiCommands.SymbolRequest)
                    .With(new CommandParameter("symbol", symbol, true))
                    .With(CommandSwitches.DeActivateRealtimeUpdates);

                TickerApi.TApiSendCommand(command);
            }
        }

        public bool SubscribeLevelTwoWatch(string symbol, string exchange, out List<Bid> bids, out List<Offer> offers, out List<Order> orders)
        {
            bids = new List<Bid>();
            offers = new List<Offer>();
            orders = new List<Order>();

            if(!_isConnected) return false;

            var setOptionCommand = new Command(InteractiveApiCommands.SetOption)
                .With(new CommandParameter("Orderbook", "true"));

            var command = new Command(InteractiveApiCommands.SymbolRequest)
                .With(new CommandParameter("symbol", symbol, true))
                .With(new CommandParameter("Mkt", ("" + ((int) Enum.Parse(typeof (Exchanges), exchange)))))
                .With(new CommandParameter("Sec", "1"))
                .With(CommandSwitches.ActivateRealtimeUpdates)
                .With(CommandSwitches.RequestRecap)
                .With(CommandSwitches.RequestAllFields);

            return (TickerApi.TApiSendCommand(command) > 0);
        }

        public bool UnsubscribeLevelTwoWatch(string symbol, string exchange)
        {
            if (!_isConnected) return false;

            var command = new Command(InteractiveApiCommands.SymbolRequest)
                    .With(new CommandParameter("symbol", symbol, true))
                    .With(CommandSwitches.DeActivateRealtimeUpdates);

            return (TickerApi.TApiSendCommand(command) > 0);
        }

        public void SubscribeIndexWatch(string symbol, string exchange)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeIndexWatch(string symbol, string exchange)
        {
            throw new NotImplementedException();
        }

        public List<InstrumentSnapshot> GetInstrumentSnapshotHistory(string symbol, string exchange, int interval, int barsCount)
        {
            throw new NotImplementedException();
        }

        public List<InstrumentSnapshot> GetIndexSnapshotHistory(string index, string exchange, int interval, int barsCount)
        {
            throw new NotImplementedException();
        }

        public List<Instrument> GetTop20InstrumentsByValue(string exchange)
        {
            return new List<Instrument>();
        }

        public List<Instrument> GetTop20InstrumentsByVolume(string exchange)
        {
            return new List<Instrument>();
        }

        public List<Instrument> GetWinners(string exchange)
        {
            return (new List<Instrument>());
        }

        public List<Instrument> GetLosers(string exchange)
        {
            return (new List<Instrument>());
        }

        public List<Index> GetIndexes(string exchange)
        {
            return (new List<Index>());
        }

        public List<Instrument> GetTop40Instruments(string exchange)
        {
            return new List<Instrument>();
        }

        public List<InstrumentStatistics> GetMarketsHomeData(string exchange)
        {
            return new List<InstrumentStatistics>();
        }

        public List<Sens> GetSensData()
        {
            return new List<Sens>();
            //Fin24.Markets.Client.StateRequest.Sens[] sensList = null;
            //try
            //{
            //    sensList = _subscriptionService.GetSens();
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.ToString());
            //} 
            //List<Sens> sens = DataAdapter.GetSens(sensList);
            //return sens;
        }

        public string GetSensBody(string id)
        {
            return "";
        }

        public List<InstrumentReference> GetInstrumentReferencesForType(Domain.Enumerations.InstrumentType type)
        {
            return (new List<InstrumentReference>());
        }

        public List<InstrumentReference> GetInstrumentReferenceForAllCurrentInstruments()
        {
            var instrumentsReferences = new List<InstrumentReference>();

            string symbolListPath = string.Format("{0}Documents\\Johannesburg Symbol List.txt", AppDomain.CurrentDomain.BaseDirectory);
            
            if(File.Exists(symbolListPath))
            {
                string[] stocks = File.ReadAllLines(symbolListPath);

                instrumentsReferences.AddRange(DataAdapter.GetInstrumentReferencesFromFileValues(stocks.ToList()));
            }
            
            Debug.WriteLine(string.Format("GetInstrumentReferenceForAllCurrentInstruments: {0}", instrumentsReferences.Count));

            instrumentsReferences.Sort();

            return instrumentsReferences;
        }

        public List<Trade> GetTradesForInstrument(string symbol)
        {
            return new List<Trade>();
        }

        public List<Trade> GetTrades(string exchange, string symbol, int pageSize)
        {
            return new List<Trade>();
        }

        public List<Quote> GetQuotes(string exchange, string symbol, DateTime fromDate)
        {
            return new List<Quote>();
        }

        #region Private Methods

        private void StartListenerThread()
        {
            WaitCallback listenerThreadCallback = delegate { ListenForMessages(); };
            ThreadPool.QueueUserWorkItem(listenerThreadCallback);
        }

        private void ListenForMessages()
        {
            if (_isConnected)
            {
                byte[] messageBuffer = new byte[10 * 1024];

                while (_isConnected)
                {
                    if (TickerApi.TApiReadRecord(messageBuffer) > 0)
                    {
                        byte[] destinationBuffer = new byte[10 * 1024];

                        int convertedToXml = TickerApi.TApiDataToXml(messageBuffer, destinationBuffer, destinationBuffer.Length);

                        if (convertedToXml > 0)
                        {
                            string resultXmlString = Encoding.ASCII.GetString(destinationBuffer);

                            var rawMessageObject = MorningstarMessage.DeserializeFrom(resultXmlString.Replace("&", " "));

                            ProcessMessage(rawMessageObject);
                        }
                    }
                    else
                        Thread.Sleep(1);
                }

                var deactivateCommand = new Command(InteractiveApiCommands.SymbolRequest)
                    .With(CommandSwitches.DeActivateRealtimeUpdates);

                TickerApi.TApiSendCommand(deactivateCommand);

                StopCurrentThread();
            }
            else
            {
                StopCurrentThread();
            }
        }

        private void StopCurrentThread()
        {
            try
            {
                Thread.CurrentThread.Abort();
            }
            catch { }

            Debug.WriteLine("Listener Thread Aborted");
        }

        private void ProcessMessage(MorningstarMessage rawMessage)
        {
            WaitCallback messageProcessingWork = null;

            switch (rawMessage.MessageType)
            {
                case MessageTypes.TF_MSG_TYPE_UNDEF:
                    break;
                case MessageTypes.TF_MSG_TYPE_TRADE:
                    messageProcessingWork = delegate { TradeOccurred(rawMessage); };
                    break;
                case MessageTypes.TF_MSG_TYPE_QUOTE:
                    //int count = rawMessage._Fields.Count(f => f.Identifier == FieldIdentifiers.TF_FIELD_LSE_DEPTH_AMD);
                    //if (count > 0)
                    //{
                    //    messageProcessingWork = delegate { OrderIssued(rawMessage); };
                    //}
                    //else
                    //{
                    //    messageProcessingWork = delegate { QuoteIssued(rawMessage); };
                    //}
                    messageProcessingWork = delegate { QuoteIssued(rawMessage); };
                    
                    break;
                case MessageTypes.TF_MSG_TYPE_RECAP:
                    messageProcessingWork = delegate { UpdateInstrument(rawMessage); };
                    break;
                case MessageTypes.TF_MSG_TYPE_ADMIN:
                    break;
                case MessageTypes.TF_MSG_TYPE_CONTROL:
                    break;
                case MessageTypes.TF_MSG_TYPE_STATIC:
                    break;
                case MessageTypes.TF_MSG_TYPE_DYNAMIC:
                    break;
                case MessageTypes.TF_MSG_TYPE_OTHERS:
                    break;
                case MessageTypes.TF_MSG_TYPE_CLOSE:
                    break;
                case MessageTypes.TF_MSG_TYPE_NEWS:
                    break;
                case MessageTypes.TF_MSG_TYPE_CHART:
                    //messageProcessingWork = delegate { CreateChart(rawMessage); };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (messageProcessingWork != null)
                ThreadPool.QueueUserWorkItem(messageProcessingWork);
        }

        private void UpdateInstrument(MorningstarMessage message)
        {
            Instrument instrument = DataAdapter.GetInstrumentFromRawMessage(message, ref _Repository);

            if(InstrumentUpdatedEvent != null && instrument != null)
            {
                InstrumentUpdatedEvent(instrument);
            }
        }

        private void TradeOccurred(MorningstarMessage message)
        {
            Trade trade = DataAdapter.GetTradeFromRawMessage(message);
            if(TradeOccurredEvent != null)
            {
                TradeOccurredEvent(trade);
            }
        }

        private void QuoteIssued(MorningstarMessage message)
        {
            Quote quote = DataAdapter.GetQuoteFromRawMessage(message, ref _Repository);
            if(QuoteOccurredEvent != null)
            {
                QuoteOccurredEvent(quote);
            }
        }

        private void OrderIssued(MorningstarMessage message)
        {
            OrderEvent orderEvent = DataAdapter.GetOrderEventFromRawMessage(message, _AllInstruments);
            if (orderEvent.Operation == OrderEventType.Add)
            {
                if (OrderAddedEvent != null)
                {
                    OrderAddedEvent(orderEvent.Order);
                }
            }
            else if (orderEvent.Operation == OrderEventType.Update)
            {
                if (OrderUpdatedEvent != null)
                {
                    OrderUpdatedEvent(orderEvent.Order);
                }
            }
            else if (orderEvent.Operation == OrderEventType.Delete) 
            {
                if (OrderDeletedEvent != null)
                {
                    OrderDeletedEvent(orderEvent.Order);
                }
            }

        }

        private void CreateChart(MorningstarMessage message)
        {
            
        }

        #endregion
    }
}
