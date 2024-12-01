using System;
using System.Linq;
using System.Collections.Generic;
using FundXchange.Model.Infrastructure;
using FundXchange.Model.ViewModels;
using FundXchange.Domain.Entities;
using FundXchange.Model.ViewModels.DataManager;
using FundXchange.Domain.ValueObjects;
using FundXchange.Infrastructure;
using FundXchange.Domain.Enumerations;
using FundXchange.Model.Gateways;
using FundXchange.Domain;

namespace FundXchange.Model.Controllers
{
    public class DataManagerController
    {
        private int _UserId;
        private string _CurrentDataPortfolio;
        private IMarketRepository _Repository;
        private ErrorService _ErrorService;
        private DataManagerViewModel _ViewModel;
        private PortfolioGateway _PortfolioGateway;
        private MessageHandler<object> _MessageHandler;

        public delegate void DataManagerInstrumentUpdatedDelegate(DataManagerInstrument instrument);
        public event DataManagerInstrumentUpdatedDelegate InstrumentUpdatedEvent;


        public DataManagerController(int userId, DataManagerViewModel viewModel)
        {
            _UserId = userId;
            _ViewModel = viewModel;
            _Repository = IoC.Resolve<IMarketRepository>();
            _ErrorService = IoC.Resolve<ErrorService>();

            _MessageHandler = new MessageHandler<object>("DataManager");
            _MessageHandler.MessageReceived += _MessageHandler_MessageReceived;
            _MessageHandler.Start();
            

            _ViewModel.Instruments = new Dictionary<string, DataManagerInstrument>();
            _ViewModel.Instruments = GetInstrumentsFromWatchList(_ViewModel.Instruments, _Repository.PortfolioWatchList);

            _PortfolioGateway = new PortfolioGateway(_ErrorService);

            _Repository.InstrumentUpdatedEvent += _Repository_InstrumentUpdatedEvent;
            _Repository.IndexUpdatedEvent += _Repository_IndexUpdatedEvent;                       
        }

        
        public void CreateDataPortfolio(string portfolioName)
        {
            _PortfolioGateway.SaveDataPortfolio(_UserId, portfolioName);
        }

        public void DeleteDataPortfolio(string portfolioName)
        {
            _PortfolioGateway.DeleteDataPortfolio(_UserId, portfolioName);
        }

        public void ChangeDataPortfolio(string newPortfolioName)
        {
            _CurrentDataPortfolio = newPortfolioName;
        }

        public List<DataManagerInstrument> GetInstrumentSnapshots()
        {
            return _ViewModel.Instruments.Values.ToList();
        }

        public void AddSymbolToPortfolioWatch(string symbol, string exchange)
        {
            symbol = symbol.Trim();
            if (_ViewModel.Instruments == null)
            {
                _ViewModel.Instruments = new Dictionary<string, DataManagerInstrument>();
            }

            if (!_ViewModel.Instruments.ContainsKey(symbol))
            {
                Instrument instrument = new Instrument();
                
                //if(_Repository.IndexWatchList.Count == 0)// By John
                //    _Repository.GetIndices();
                if (_Repository.IndexWatchList.ContainsKey(symbol))
                {
                    Index index = _Repository.IndexWatchList[symbol];
                    instrument = _Repository.SubscribeIndex(index.Symbol, index.Exchange);
                }
                else
                {
                    instrument = _Repository.AddInstrumentToPortfolioWatch(symbol, exchange);
                }
                //if (instrument == null)
                //{
                //    if (!string.IsNullOrEmpty(symbol))
                //    {
                //        throw new DomainException(String.Format("Symbol ({0}) not found", symbol));
                //    }
                //    return;
                //}

                var newInstrument = new DataManagerInstrument
                {
                    Symbol = symbol,
                    //InstrumentID = symbol,
                    Exchange = exchange,                    
                };
                GetDataManagerInstrumentFromInstrument(instrument, newInstrument);
                //if (_ViewModel == null)
                //{
                //    Console.WriteLine("DataManager ViewModel is null.");
                //    return;
                //}
                //if (!_ViewModel.Instruments.ContainsKey(newInstrument.Symbol))
                {
                    try
                    {
                        // TODO [AvdM]: NullReferenceException occurs here sometimes

                        newInstrument.Symbol = symbol;//instrument.Symbol;
                        _ViewModel.Instruments.Add(newInstrument.Symbol, newInstrument);
                        RaiseInstrumentUpdateEvent(newInstrument);
                       
                        //Commented By John
                        //_PortfolioGateway.AddDataPortfolioInstrument(_UserId, _CurrentDataPortfolio, exchange, symbol);
                    }
                    catch(NullReferenceException e)
                    {
                        Console.WriteLine("DataManagerController NullReference exception occurred. Cause of this exception is currently unknown.");
                    }
                }
            }
        }

        public void RemoveSymbolFromPortfolioWatch(string symbol, string exchange)
        {
            if (_Repository.IndexWatchList.ContainsKey(symbol))
                _Repository.UnsubscribeIndexWatch(symbol, exchange);
            else
                _Repository.UnsubscribeLevelOneWatch(symbol, exchange);

            _PortfolioGateway.RemoveDataPortfolioInstrument(_UserId, _CurrentDataPortfolio, exchange, symbol);

            if (_ViewModel.Instruments.ContainsKey(symbol))
                _ViewModel.Instruments.Remove(symbol);
        }

        public void ClearPortfolioWatch()
        {
            _ViewModel.Instruments.Clear();
            _Repository.PortfolioWatchList.Clear();
        }

        public List<InstrumentReference> GetInstrumentReferencesForType(InstrumentType type)
        {
            return _Repository.GetInstrumentReferencesForType(type);
        }

        void _Repository_InstrumentUpdatedEvent(Instrument instrument)
        {
            _MessageHandler.AddMessage(instrument);
        }

        void _MessageHandler_MessageReceived(object message)
        {
            if (message is Instrument)
            {
                ProcessInstrument((Instrument)message);
            }
            else if (message is Index)
            {
                ProcessIndex((Index)message);
            }
        }

        private void ProcessInstrument(Instrument instrument)// Changed By John
        {
           
            if (_Repository.PortfolioWatchList.ContainsKey(instrument.Symbol))
            {
                lock (_Repository.PortfolioWatchList)
                {
                    if (!_ViewModel.Instruments.ContainsKey(instrument.Symbol))
                    {

                        _ViewModel.Instruments = GetInstrumentsFromWatchList(_ViewModel.Instruments, _Repository.PortfolioWatchList);

                        GetDataManagerInstrumentFromInstrument(instrument, _ViewModel.Instruments[instrument.Symbol]);
                        RaiseInstrumentUpdateEvent(_ViewModel.Instruments[instrument.Symbol]);


                    }
                    else if (_ViewModel.Instruments.ContainsKey(instrument.Symbol))
                    {
                        GetDataManagerInstrumentFromInstrument(instrument, _ViewModel.Instruments[instrument.Symbol]);
                        RaiseInstrumentUpdateEvent(_ViewModel.Instruments[instrument.Symbol]);
                    }
                }
            }


            //Commented By John for the time being
            //if (_Repository.PortfolioWatchList.ContainsKey(instrument.Symbol))
            //{
            //    if (_ViewModel.Instruments.ContainsKey(instrument.Symbol))
            //    {
            //        GetDataManagerInstrumentFromInstrument(instrument, _ViewModel.Instruments[instrument.Symbol]);
            //        RaiseInstrumentUpdateEvent(_ViewModel.Instruments[instrument.Symbol]);
            //    }
            //}

             // Added by John for the time being
            //if (!_ViewModel.Instruments.ContainsKey(instrument.Symbol))
            //{
            //    var newInstrument = new DataManagerInstrument
            //    {
            //        Symbol = instrument.Symbol,
            //        Exchange = instrument.Exchange
            //    };

            //    _ViewModel.Instruments.Add(newInstrument.Symbol, newInstrument);
            //    MapInstrumentToDataManagerInstrument(_ViewModel.Instruments, instrument);


            //    GetDataManagerInstrumentFromInstrument(instrument, _ViewModel.Instruments[instrument.Symbol]);
            //    RaiseInstrumentUpdateEvent(_ViewModel.Instruments[instrument.Symbol]);

            //    //IsFirstInstrument = false;
            //}
            //else if (_ViewModel.Instruments.ContainsKey(instrument.Symbol))
            //{
            //    GetDataManagerInstrumentFromInstrument(instrument, _ViewModel.Instruments[instrument.Symbol]);
            //    RaiseInstrumentUpdateEvent(_ViewModel.Instruments[instrument.Symbol]);
            //}
        }

        void _Repository_IndexUpdatedEvent(Index index)
        {
            _MessageHandler.AddMessage(index);
        }

        private void ProcessIndex(Index index)
        {
            if (_Repository.PortfolioWatchList.ContainsKey(index.Symbol))
            {
                if (_ViewModel.Instruments.ContainsKey(index.Symbol))
                {
                    GetDataManagerInstrumentFromIndex(index, _ViewModel.Instruments[index.Symbol]);
                    RaiseInstrumentUpdateEvent(_ViewModel.Instruments[index.Symbol]);
                }
            }
        }

        public string GetShortnameForIndexSymbol(string symbol)
        {
            foreach (KeyValuePair<string, Index> indexPair in _Repository.IndexWatchList)
            {
                if (indexPair.Value.Symbol == symbol)
                {
                    return indexPair.Value.ShortName;
                }
            }
            return null;
        }

        private void RaiseInstrumentUpdateEvent(DataManagerInstrument updatedInstrument)
        {
            if (InstrumentUpdatedEvent != null)
            {
                InstrumentUpdatedEvent(updatedInstrument);
            }
        }
        object lck = new object();
        private Dictionary<string, DataManagerInstrument> GetInstrumentsFromWatchList(Dictionary<string, DataManagerInstrument> instruments, Dictionary<string, Instrument> dictionary)
        {
            //lock (dictionary)
            //{
            foreach (Instrument instrument in dictionary.Values.ToList())
            {
                if (instrument != null)
                {
                    if (!instruments.ContainsKey(instrument.Symbol))
                    {
                        var newInstrument = new DataManagerInstrument
                        {
                            Symbol = instrument.Symbol,
                            Exchange = instrument.Exchange
                        };
                        instruments.Add(instrument.Symbol, newInstrument);
                    }
                    MapInstrumentToDataManagerInstrument(instruments, instrument);
                }
            }
            //}

            return instruments;
        }

        private static void MapInstrumentToDataManagerInstrument(Dictionary<string, DataManagerInstrument> instruments, Instrument instrument)
        {
            foreach (var dmInstrument in instruments.Values)
            {
                if (dmInstrument != null && dmInstrument.Symbol == instrument.Symbol)
                {
                    GetDataManagerInstrumentFromInstrument(instrument, dmInstrument);
                    break;
                }
            }
        }

        private static void GetDataManagerInstrumentFromInstrument(Instrument instrument, DataManagerInstrument dmInstrument)
        {
            if (instrument != null)
            {
                dmInstrument.Close = instrument.YesterdayClose;
                dmInstrument.DailyValue = instrument.TotalValue;
                dmInstrument.DailyVolume = instrument.TotalVolume;
                dmInstrument.Deals = instrument.TotalTrades;
                dmInstrument.High = instrument.High;
                dmInstrument.Low = instrument.Low;
                dmInstrument.Move = instrument.CentsMoved;
                dmInstrument.Open = instrument.Open;
                dmInstrument.Bid = instrument.BestBid;
                dmInstrument.BidVolume = instrument.BidVolume;
                dmInstrument.Offer = instrument.BestOffer;
                dmInstrument.OfferVolume = instrument.OfferVolume;
                dmInstrument.PercentageMove = instrument.PercentageMoved;
                dmInstrument.Time = DateTime.Now.ToString();
                dmInstrument.YesterdayClose = instrument.YesterdayClose;
                dmInstrument.Price = instrument.LastTrade;
                dmInstrument.CompanyName = instrument.CompanyLongName;
                dmInstrument.InstrumentID = instrument.InstrumentID;
                dmInstrument.Symbol = instrument.Symbol;
                
            }
        }

        private void GetDataManagerInstrumentFromIndex(Index index, DataManagerInstrument dmInstrument)
        {
            if (index != null)
            {
                DataManagerInstrument updatedInstrument = _ViewModel.Instruments[index.Symbol];

                dmInstrument.Close = index.YesterdayClose;
                dmInstrument.DailyValue = index.Value;
                dmInstrument.Move = index.CentsMoved;
                dmInstrument.PercentageMove = index.PercentageMoved;
                dmInstrument.Time = DateTime.Now.ToString();
                dmInstrument.YesterdayClose = index.YesterdayClose;
                dmInstrument.Price = index.Value;

                if (updatedInstrument != null)
                {
                    dmInstrument.DailyVolume = updatedInstrument.DailyVolume;
                    dmInstrument.Deals = updatedInstrument.Deals;
                    dmInstrument.High = updatedInstrument.High;
                    dmInstrument.Low = updatedInstrument.Low;
                    dmInstrument.Open = updatedInstrument.Open;
                }                   
            }
        }

        public List<InstrumentReference> GetInstrumentReferenceForAllCurrentInstruments()
        {
            return _Repository.AllInstrumentReferences;
        }
    }
}
