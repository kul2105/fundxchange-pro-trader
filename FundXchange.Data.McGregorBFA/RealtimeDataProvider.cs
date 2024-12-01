using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using Fin24.LiveData.Candlestick.Services.API.DTOs;
using Fin24.LiveData.Common.MarketModel.MarketMessages;
using Fin24.LiveData.Common.Protocol.DataFeed;
using Fin24.Util.General.Ipc.Tcp;
using Fin24.Util.General.Ipc;
using FundXchange.Data.McGregorBFA.LiveProtoCandlestickService;
using FundXchange.Data.McGregorBFA.ServiceDesk;
using FundXchange.Data.McGregorBFA.SubscriptionManagementService;
using FundXchange.DataProviderContracts;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;
using FundXchange.Domain.Enumerations;
using FundXchange.Domain;
using OrderbookDto = Fin24.LiveData.Common.DTOs.OrderbookDto;
using ResultStatus = FundXchange.Data.McGregorBFA.ServiceDesk.ResultStatus;
using System.Threading;
using Fin24.LiveData.Common.DTOs;
using InstrumentTypeDto = FundXchange.Data.McGregorBFA.ServiceDesk.InstrumentTypeDto;
using PeriodEnum = FundXchange.Domain.Enumerations.PeriodEnum;
using JSEServersAPI;
using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;
using JsonLibCommon;
//using JsonLibCommon;
using System.Web.Script.Serialization;
using MQTTAPI;

namespace FundXchange.Data.McGregorBFA
{
    public class RealtimeDataProvider : IRealTimeDataProvider
    {
        private const string SESSION_INVALID_EX = "sessioninvalidexception:";

        //private BarChartAPI.BarChart barchart = null;
        private MitchJSEClient mitchJSEClient = null;
        private MQTT mQTTAPI = null;
        private Dictionary<string, InstrumentReference> _AllInstrumentReferences;
        private Dictionary<string, Index> _AllIndicesReferences;
        private Dictionary<string, Instrument> _AllInstruments;
        private Dictionary<string, string> _ISINToSymbolLookup;
        private Dictionary<string, FundXchange.Domain.ValueObjects.Quote> _quoteMappings = new Dictionary<string, FundXchange.Domain.ValueObjects.Quote>();
        private Dictionary<string, long> _tradePair = new Dictionary<string, long>();

        private FinSwitch _FinSwitchService = null;
        //Mutual Funds FinSwitch
        //private List<JSONdataLib.FundDetails> _AllFinSwitchMFList;
        //public List<JSONdataLib.FundDetails> AllFinSwitchMFList { get => _AllFinSwitchMFList; set => _AllFinSwitchMFList = value; }

        private List<Sens> _SensList;

        private readonly string DELIVERY_AGENT_ADDRESS = ConfigurationManager.AppSettings["DELIVERY_AGENT_ADDRESS"];
        private readonly int DELIVERY_AGENT_PORT = Convert.ToInt32(ConfigurationManager.AppSettings["DELIVERY_AGENT_PORT"]);

        private string _Username;
        private string _Password;
        private string _Product;
        private readonly ProtocolParser _parser = new ProtocolParser();

        private string _sessionId;
        private bool _IsRestricted;
        private bool shouldGetEquitiesFromService;
        private bool shouldGetIndexFromService;
        private MessageHandler<HeartbeatMessage> _messageHandlerHeartbeat = new MessageHandler<HeartbeatMessage>("RealtimeDataProvider_Hearbeats");

        private MessageHandler<TradeReport> _messageHandlerTradeReports = new MessageHandler<TradeReport>("RealtimeDataProvider_TradeReports");
        private MessageHandler<CumulativeNumberAndVolumeOfTrades> _messageHandlerCumulativeNumberAndVolumeOfTrades =
            new MessageHandler<CumulativeNumberAndVolumeOfTrades>(
                "RealtimeDataProvider_CumulativeNumberAndVolumeOfTrades");
        private MessageHandler<ClosingPrice> _messageHandlerClosingPrice = new MessageHandler<ClosingPrice>("RealtimeDataProvider_ClosingPrice");
        private MessageHandler<UncrossingPriceAndVolume> _messageHandlerUncrossingPriceAndVolume = new MessageHandler<UncrossingPriceAndVolume>("RealtimeDataProvider_UncrossingPriceAndVolume");
        private MessageHandler<EnhancedBestPrice> _messageHandlerEnhancedBestPrice = new MessageHandler<EnhancedBestPrice>("RealtimeDataProvider_EnhancedBestPrice");

        private MessageHandler<OrderbookEvent> _messageHandlerOrderbookEvent = new MessageHandler<OrderbookEvent>("RealtimeDataProvider_OrderbookEvent");

        private MessageHandler<IndexValue> _messageHandleIndexValue = new MessageHandler<IndexValue>("RealtimeDataProvider_IndexValue");
        private TcpChannel _channel;
        private List<InstrumentTypeDto> _instrumentTypes = new List<InstrumentTypeDto>();

        public event InstrumentUpdatedDelegate InstrumentUpdatedEvent;
        public event IndexUpdatedDelegate IndexUpdatedEvent;
        public event TradeOccurredDelegate TradeOccurredEvent;
        public event OrderAddedDelegate OrderAddedEvent;
        public event OrderbookInitializedDelegate OrderbookInitializedEvent;
        public event OrderDeletedDelegate OrderDeletedEvent;
        public event ErrorOccurredDelegate ErrorOccurredEvent;
        public event QuoteOccurredDelegate QuoteOccurredEvent;
        public event DataProviderConnectionChangedDelegate DataProviderConnectionChangedEvent;
        public event HeartbeatMessageReceivedDelegate HeartbeatReceivedEvent;
        public event SensNewsDelegate OnSensNewsEvent;

        private bool _isConnected = false;

        Dictionary<uint, string> _InstIDToSymbolLookup = new Dictionary<uint, string>();
        

        public List<InstrumentReference> GetInstrumentReferenceForAllCurrentInstruments()
        {
            List<InstrumentReference> instrumentRefs = new List<InstrumentReference>();
            //John BO
            lock (_AllInstrumentReferences)
            {
                foreach (KeyValuePair<string, InstrumentReference> keyValuePair in _AllInstrumentReferences)
                {
                    if (!instrumentRefs.Contains(keyValuePair.Value))
                        instrumentRefs.Add(keyValuePair.Value);
                }
            }
            return instrumentRefs;
        }

        public RealtimeDataProvider(string username, string password, string product, bool isRestricted)
        {
            _IsRestricted = isRestricted;
            _Username = username;
            _Password = password;
            _Product = product;

            _AllInstrumentReferences = new Dictionary<string, InstrumentReference>();
           
            _AllIndicesReferences = new Dictionary<string, Index>();
            _AllInstruments = new Dictionary<string, Instrument>();
            _ISINToSymbolLookup = new Dictionary<string, string>();
            _SensList = new List<Sens>();
            _instrumentTypes.Add(InstrumentTypeDto.Ordinary_Share);

            shouldGetEquitiesFromService = true; //GetEquitiesFromFile(); John
            shouldGetIndexFromService = true; //GetIndexListFromFile();

            //mitchJSEClient = new MitchJSEClient();
            //mitchJSEClient.Start("1", "1");
            //mitchJSEClient.OnSymbolDirectoryResponse += MitchJSEClient_OnSymbolDirectoryResponse;
            //mitchJSEClient.OnQuoteResponse += MitchJSEClient_OnQuoteResponse;
            //mitchJSEClient.OnLevel2Response += MitchJSEClient_OnLevel2Response;
            //mitchJSEClient.OnTradeResponse += MitchJSEClient_OnTradeResponse;

            mQTTAPI = new MQTT();
            mQTTAPI.Connect("", "");
            MQTTAPI_OnSymbolDirectoryResponse(new List<string>());
            mQTTAPI.OnSymbolDirectoryResponse += MQTTAPI_OnSymbolDirectoryResponse;
            mQTTAPI.OnSensNews += MQTTAPI_OnSensNews;
            mQTTAPI.OnQuoteResponse += MQTTAPI_OnQuoteResponse;
            mQTTAPI.OnLevel2Response += MQTTAPI_OnLevel2Response;
            mQTTAPI.OnTradeResponse += MQTTAPI_OnTradeResponse;
            mQTTAPI.OnOHLVC += MQTTAPI_OnOHLVC;

            //Jservers.GetHistoricalResponseFS("", "EURUSD", DateTime.Now);
            //barchart = new BarChartAPI.BarChart(_Username, _Password);
            //barchart.onNewQuote += (barchart_onNewQuote);
            //barchart.onLevel2 += (barchart_onLevel2);
            //_messageHandlerHeartbeat.MessageReceived += _messageHandlerHeartbeat_MessageReceived;
            //_messageHandlerTradeReports.MessageReceived += _messageHandlerTradeReports_MessageReceived;
            //_messageHandlerEnhancedBestPrice.MessageReceived += _messageHandlerEnhancedBestPrice_MessageReceived;
            //_messageHandlerUncrossingPriceAndVolume.MessageReceived += _messageHandlerUncrossingPriceAndVolume_MessageReceived;
            //_messageHandlerClosingPrice.MessageReceived += _messageHandlerClosingPrice_MessageReceived;
            //_messageHandlerCumulativeNumberAndVolumeOfTrades.MessageReceived += _messageHandlerCumulativeNumberAndVolumeOfTrades_MessageReceived;

            //_messageHandlerOrderbookEvent.MessageReceived += _messageHandlerOrderbookEvent_MessageReceived;

            //_messageHandleIndexValue.MessageReceived += _messageHandleIndexValue_MessageReceived;

            //_messageHandlerTradeReports.Start();
            //_messageHandlerEnhancedBestPrice.Start();
            //_messageHandlerOrderbookEvent.Start();
            //_messageHandlerUncrossingPriceAndVolume.Start();
            //_messageHandleIndexValue.Start();
            //_messageHandlerHeartbeat.Start();

        }

        private void MQTTAPI_OnSensNews(FullSensNews obj)
        {
            if (OnSensNewsEvent != null)
                OnSensNewsEvent(obj);
        }

        private void MQTTAPI_OnSymbolDirectoryResponse(SymbolDirectory obj)
        {
           
            InstrumentReference instrument = new InstrumentReference();
            //instrument.InstrumentID = item.ToString();
            instrument.Symbol = obj.Symbol;
            instrument.Exchange = "JSE";
            instrument.InstrumentType = obj.InstrumentSubCategory;
            instrument.ISIN = obj.ISIN;
            instrument.CompanyShortName = obj.Underlying;//obj.Issuer;
            instrument.CompanyLongName = obj.Underlying;
            instrument.MarketSegment = obj.Segment;
            instrument.Security = obj.TIDM;

            lock (_AllInstrumentReferences)
            {
                if (!_AllInstrumentReferences.ContainsKey(instrument.Symbol))
                    _AllInstrumentReferences.Add(instrument.Symbol, instrument);
                else
                    _AllInstrumentReferences[instrument.Symbol] = instrument;

            }            
        }
        
        private void MQTTAPI_OnOHLVC(OHLCV ohlc)
        {
            if (_AllInstruments.ContainsKey(ohlc.Symbol))
            {
                Instrument inst = _AllInstruments[ohlc.Symbol];
                inst.Open = ohlc.Open;
                inst.High = ohlc.High;
                inst.Low = ohlc.Low;
                inst.YesterdayClose = ohlc.PreviousClose;
                inst.TotalVolume = ohlc.Volume;
                //inst.PercentageMoved = ohlc.OpenInterest;
                inst.ISIN = ohlc.Segment;
                inst.TotalValue = Convert.ToInt64(ohlc.VWAP);

                _AllInstruments[ohlc.Symbol] = inst;
                //InstrumentUpdatedEvent(inst);//BroadCast Here  
            }
        }

        private void MQTTAPI_OnTradeResponse(MQTTAPI.Trade obj)
        {
            Domain.ValueObjects.Trade trdMsg = new Domain.ValueObjects.Trade();
            string Symbol = obj.Symbol;
            trdMsg.Symbol = Symbol;
            trdMsg.SymbolCode = obj.SecurityId.ToString();

            trdMsg.Price = (obj.Price);
            trdMsg.Volume = obj.Quantity;
            trdMsg.TimeStamp = obj.TradeTime;
            trdMsg.Quote = new Domain.ValueObjects.Quote();
            trdMsg.Exchange = "JSE";

            //DateTime epochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //DateTime result = epochTime.AddTicks(obj.Nanosecond / 100);
            //trdMsg.TimeStamp = result;



            if (_AllInstruments.ContainsKey(obj.Symbol))
            {
                Instrument inst = _AllInstruments[obj.Symbol];
                double val = 0;
                if (inst.LastTrade != 0)
                    val = Math.Round((((obj.Price - inst.LastTrade) / inst.LastTrade) * 100), 4);

                if(val!=0)
                {
                    inst.PercentageMoved = val;
                }
                else
                {
                    
                }
                inst.CentsMoved = obj.Price - inst.LastTrade;
                //inst.PercentageMoved = val;
                inst.LastTrade = obj.Price;

                inst.LastTradeTime = obj.TradeTime;
                inst.LastTradeSequenceNumber = obj.TradeId;

                if (inst.BestBid == obj.Price)
                    trdMsg.Quote.BestBidPrice = Convert.ToDouble(obj.Price);
                else if (inst.BestOffer == obj.Price)
                    trdMsg.Quote.BestAskPrice = Convert.ToDouble(obj.Price);

                _AllInstruments[obj.Symbol] = inst;//Update Here also
                if (TradeOccurredEvent != null)
                    TradeOccurredEvent(trdMsg);
                InstrumentUpdatedEvent(inst);//BroadCast Here  
            }
        }

        private void MQTTAPI_OnLevel2Response(OrderBook obj)
        {            
            if (obj.Symbol == string.Empty)
                return;
            List<OrderbookItem> bidOrders = ProcessInitialLevel2(obj.Symbol, obj.SecurityId, obj.BuyBook, OrderSide.Buy);
            IEnumerable<OrderbookItem> offerOrders = ProcessInitialLevel2(obj.Symbol, obj.SecurityId, obj.SellBook, OrderSide.Sell);

            List<OrderbookItem> items = bidOrders;
            items.AddRange(offerOrders);

            //if (obj.orderTransType == InsightCapital.STTAPI.MessageLibrary.OrderTransType.Initial || obj.orderTransType == InsightCapital.STTAPI.MessageLibrary.OrderTransType.NA)
            {

                if (OrderbookInitializedEvent != null)
                    OrderbookInitializedEvent(items);
            }
            //else if (obj.orderTransType == InsightCapital.STTAPI.MessageLibrary.OrderTransType.Add)
            //{
            //    InsightCapital.STTAPI.MessageLibrary.ProtocolStructs.Order ord = new InsightCapital.STTAPI.MessageLibrary.ProtocolStructs.Order();
            //    OrderSide Side = OrderSide.Buy;
            //    if (obj.BuySide.Count > 0)
            //    {
            //        ord = obj.BuySide.FirstOrDefault();
            //    }
            //    else
            //    {
            //        ord = obj.SellSide.FirstOrDefault();
            //        Side = OrderSide.Sell;
            //    }
            //    OrderbookItem order = new OrderbookItem();
            //    order.Symbol = Symbol;
            //    order.Exchange = "JSE";
            //    order.OriginalOrderCode = ord.OrderId.ToString();
            //    order.Price = Convert.ToDouble(ord.Price);
            //    order.Side = Side;
            //    order.Size = Convert.ToInt64(ord.Volume);
            //    order.TradeDate = ord.TimeStamp;

            //    if (OrderAddedEvent != null)
            //        OrderAddedEvent(order);
            //}
            //else if (obj.orderTransType == InsightCapital.STTAPI.MessageLibrary.OrderTransType.Delete)
            //{
            //    // ProcessOrderDeletion((OrderDeletion)message.Message);
            //}
            //else if (obj.orderTransType == InsightCapital.STTAPI.MessageLibrary.OrderTransType.Update)
            //{
            //    // ProcessOrderDeletion((OrderDeletion)message.Message);
            //}
        }

        private void MQTTAPI_OnQuoteResponse(Book quote)
        {
            
            if (_AllInstrumentReferences.ContainsKey(quote.Symbol))
            {
                //InstrumentReference instr = _AllInstrumentReferences[quote.Symbol.Trim()];
                //inst.CompanyLongName = instr.CompanyLongName;
                if (!_AllInstruments.ContainsKey(quote.Symbol))
                {
                    Instrument inst = new Instrument();
                    inst.Symbol = quote.Symbol;
                    inst.InstrumentID = quote.SecurityId.ToString();
                    inst.BestBid = quote.BidPrice;
                    inst.BestOffer = quote.OfferPrice;
                    inst.BidVolume = Convert.ToInt64(quote.BidQty);
                    inst.OfferVolume = Convert.ToInt64(quote.OfferQty);
                    inst.Exchange = "JSE";
                    _AllInstruments.Add(quote.Symbol, inst);
                    InstrumentUpdatedEvent(inst);//BroadCast Here  
                }
                else
                {
                    Instrument oldInst = _AllInstruments[quote.Symbol];
                    oldInst.BestBid = quote.BidPrice;
                    oldInst.BestOffer = quote.OfferPrice;
                    oldInst.BidVolume = Convert.ToInt64(quote.BidQty);
                    oldInst.OfferVolume = Convert.ToInt64(quote.OfferQty);
                    _AllInstruments[quote.Symbol] = oldInst;
                    InstrumentUpdatedEvent(oldInst);//BroadCast Here  
                }
                
            }
        }
        private List<OrderbookItem> ProcessInitialLevel2(string symbol, int SecId, List<OrderLine> lst, OrderSide side)
        {
            List<OrderbookItem> lstItem = new List<OrderbookItem>();
            foreach (var item in lst)
            {
                OrderbookItem BookItem = new OrderbookItem();
                BookItem.Symbol = symbol;
                BookItem.Exchange = item.Broker;//"JSE";
                BookItem.OriginalOrderCode = SecId.ToString();
                BookItem.Price = item.Price;
                BookItem.Side = side;
                BookItem.Size = Convert.ToInt64(item.Qty);
                BookItem.TradeDate = DateTime.Now;//item.TimeStamp;
                lstItem.Add(BookItem);
            }
            return lstItem;
        }

        string[] arr = {"NPN", "AGL", "BHP", "PRX", "DSY", "SOL", "CFR", "WHL", "VOD", "SBK",
        "GLDSBF","AMSSBR","CHP","ILRP2","JDGN","OMLSBD","KAP","ABLP","ABGSBH","ESPIBS","ABCT01",
            "ASN438","J153","ALH","OMLSBN","OCEN","ASN468","BLUSBE","DLVP","AGLSBY","FSRSBB","IBLIIH",
            "LPWA08","NEWGBP","HARSBP","J275","APNSBF","ESPIBN","MTNSBU","MAPPSG"
        };
        private void MQTTAPI_OnSymbolDirectoryResponse(List<string> lstSymbols)
        {
            //foreach (var item in lstSymbols)
            if (_AllInstrumentReferences.Count > 0) return;
            foreach (var item in arr)
            {
                InstrumentReference instrument = new InstrumentReference();
                //instrument.InstrumentID = item.ToString();
                instrument.Symbol = item.Trim();
                instrument.Exchange = "JSE";
                //instrument.InstrumentType = obj.Segment;
                //instrument.ISIN = obj.ISIN;
                //instrument.CompanyShortName = obj.Underlying;//obj.Issuer;
                //instrument.CompanyLongName = obj.Underlying;

                lock (_AllInstrumentReferences)
                {
                    if (!_AllInstrumentReferences.ContainsKey(instrument.Symbol))
                        _AllInstrumentReferences.Add(instrument.Symbol, instrument);
                }
            }
        }

        #region OLD APIs
        private void MitchJSEClient_OnTradeResponse(TradeMessage obj)
        {
            Domain.ValueObjects.Trade trdMsg = new Domain.ValueObjects.Trade();
            string Symbol = GetSymbolFromInstID(obj.InstrumentId);
            trdMsg.Symbol = Symbol;
            trdMsg.Price = Convert.ToDouble(obj.Price);
            trdMsg.Volume = obj.Quantity;
            trdMsg.TimeStamp = obj.TradeTime;
            if (obj.BuySell == "B")
                trdMsg.Quote.BestBidPrice = Convert.ToDouble(obj.Price);
            else if (obj.BuySell == "S")
                trdMsg.Quote.BestAskPrice = Convert.ToDouble(obj.Price);
            else
                trdMsg.Quote.BestBidPrice = trdMsg.Quote.BestAskPrice = 0;

            //DateTime epochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //DateTime result = epochTime.AddTicks(obj.Nanosecond / 100);
            //trdMsg.TimeStamp = result;

            trdMsg.Exchange = "JSE";
            //trdMsg.TradeStatus=TradeStatus.

            if (TradeOccurredEvent != null)
                TradeOccurredEvent(trdMsg);

        }

        private void MitchJSEClient_OnLevel2Response(InsightCapital.STTAPI.MessageLibrary.ProtocolStructs.Level2Quotes obj)
        {
            string Symbol = GetSymbolFromInstID(obj.InstrumentID);
            if (Symbol == string.Empty)
                return;
            List<OrderbookItem> bidOrders = ProcessInitialLevel2JSE(Symbol, obj.BuySide, OrderSide.Buy);
            IEnumerable<OrderbookItem> offerOrders = ProcessInitialLevel2JSE(Symbol, obj.SellSide, OrderSide.Sell);

            List<OrderbookItem> items = bidOrders;
            items.AddRange(offerOrders);

            if (obj.orderTransType == InsightCapital.STTAPI.MessageLibrary.OrderTransType.Initial || obj.orderTransType == InsightCapital.STTAPI.MessageLibrary.OrderTransType.NA)
            {
               
                if (OrderbookInitializedEvent != null)
                    OrderbookInitializedEvent(items);
            }
            else if (obj.orderTransType == InsightCapital.STTAPI.MessageLibrary.OrderTransType.Add)
            {
                InsightCapital.STTAPI.MessageLibrary.ProtocolStructs.Order ord = new InsightCapital.STTAPI.MessageLibrary.ProtocolStructs.Order();
                OrderSide Side = OrderSide.Buy;
                if (obj.BuySide.Count > 0)
                {
                    ord = obj.BuySide.FirstOrDefault();
                }
                else
                {
                    ord = obj.SellSide.FirstOrDefault();
                    Side = OrderSide.Sell;
                }
                    OrderbookItem order = new OrderbookItem();                
                order.Symbol = Symbol;
                order.Exchange = "JSE";
                order.OriginalOrderCode = ord.OrderId.ToString();
                order.Price = Convert.ToDouble(ord.Price);
                order.Side = Side;
                order.Size = Convert.ToInt64(ord.Volume);
                order.TradeDate = ord.TimeStamp;

                if (OrderAddedEvent != null)
                    OrderAddedEvent(order);
            }
            else if (obj.orderTransType == InsightCapital.STTAPI.MessageLibrary.OrderTransType.Delete)
            {
               // ProcessOrderDeletion((OrderDeletion)message.Message);
            }
            else if (obj.orderTransType == InsightCapital.STTAPI.MessageLibrary.OrderTransType.Update)
            {
                // ProcessOrderDeletion((OrderDeletion)message.Message);
            }


        }
        private List<OrderbookItem> ProcessInitialLevel2JSE(string Symbol,List<InsightCapital.STTAPI.MessageLibrary.ProtocolStructs.Order> lst, OrderSide side)
        {
            List<OrderbookItem> lstItem = new List<OrderbookItem>();
            foreach (var item in lst)
            {
                OrderbookItem BookItem = new OrderbookItem();
                BookItem.Symbol = Symbol;
                BookItem.Exchange = "JSE";
                BookItem.OriginalOrderCode = item.OrderId.ToString();
                BookItem.Price = Convert.ToDouble(item.Price);
                BookItem.Side = side;
                BookItem.Size = Convert.ToInt64(item.Volume);
                BookItem.TradeDate = item.TimeStamp;
                lstItem.Add(BookItem);
            }
            return lstItem;
        }

        private string GetSymbolFromInstID(uint ID)
        {
            try
            {
                if (_InstIDToSymbolLookup.ContainsKey(ID))
                {
                    return _InstIDToSymbolLookup[ID];
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Exception in GetSymbolFromInstID: " + ex.ToString());
            }
            return "";
        }
        private void MitchJSEClient_OnQuoteResponse(InsightCapital.STTAPI.MessageLibrary.ProtocolStructs.SymbolData item)
        {
            Instrument inst = new Instrument();
            inst.Symbol = item.Symbol;
            inst.InstrumentID = item.InstrumentID.ToString();
            inst.BestBid = Convert.ToDouble(item.BidPx);
            inst.BestOffer = Convert.ToDouble(item.AskPx);//Convert.ToDouble(q.FormatValue(q.Ask, NumericFormat.Decimal));
            inst.BidVolume = item.BidSize;
            inst.OfferVolume = item.AskSize;
            inst.Exchange = item.Exchange;
            inst.ISIN = item.ISIN;
            //string cents = (q.FormatValue(q.Change, NumericFormat.Decimal));
            //inst.CentsMoved = !string.IsNullOrEmpty(cents) ? Convert.ToDouble(cents) : 0;
            //inst.PercentageMoved = inst.CentsMoved;
            inst.LastTradeTime = item.LastTradeDateTime;
            inst.LastTrade = Convert.ToDouble(item.LastTradePrice);
            inst.High = Convert.ToDouble(item.High);//Convert.ToDouble(q.FormatValue(c.High, NumericFormat.Decimal));
            inst.Low = Convert.ToDouble(item.Low);//Convert.ToDouble(q.FormatValue(c.Low, NumericFormat.Decimal));
            inst.Open = Convert.ToDouble(item.Open);//Convert.ToDouble(q.FormatValue(c.Open1, NumericFormat.Decimal));
            inst.YesterdayClose = Convert.ToDouble(item.PreviousClose);
            //inst.LastTrade = Convert.ToDouble(item.);
            inst.TotalTrades = item.NoOfTrades;
            inst.LastTradeSequenceNumber = 1000;
           // inst.TotalValue = Convert.ToInt64(item.Turnover.Price);
            inst.TotalVolume = item.Volume;

            //if (dictLevel1Map.ContainsKey(item.InstrumentID))
            //{
            //    inst.Symbol = item.InstrumentID.ToString();
            //}

            if (_AllInstrumentReferences.ContainsKey(item.Symbol.ToString()))
            {
                InstrumentReference instr = _AllInstrumentReferences[item.Symbol.ToString().Trim()];
                inst.CompanyLongName = instr.CompanyLongName;
                if (!_AllInstruments.ContainsKey(item.Symbol))
                {
                    _AllInstruments.Add(item.Symbol, inst);
                    InstrumentUpdatedEvent(inst);//BroadCast Here  
                }
                else
                {


                }

            }
        }

       // string JSESymbolsPath= @"Data\JSESymbols.txt";//"E:\\Projects Only\\Pro_Trader\\fundxchange-pro-trader\\bin\\x86\\Debug\\Res\\JSESymbols.txt";
        private void MitchJSEClient_OnSymbolDirectoryResponse(InsightCapital.STTAPI.MessageLibrary.ProtocolStructs.SymbolMasterResponse obj)
        {
            InstrumentReference instrument = new InstrumentReference();
            instrument.InstrumentID = obj.InstrumentId;
            instrument.Symbol = obj.Symbol.Trim();
            instrument.Exchange = "JSE";
            instrument.InstrumentType = obj.Segment;
            instrument.ISIN = obj.ISIN;
            instrument.CompanyShortName = obj.Underlying;//obj.Issuer;
            instrument.CompanyLongName = obj.Underlying;

            lock (_AllInstrumentReferences)
            {
                if (!_AllInstrumentReferences.ContainsKey(instrument.Symbol))
                    _AllInstrumentReferences.Add(instrument.Symbol, instrument);

                if (!_InstIDToSymbolLookup.ContainsKey(obj.InstrumentId))
                    _InstIDToSymbolLookup.Add(obj.InstrumentId, obj.Symbol);
            }
        }

        public void SetSymbolDictionary(Dictionary<string, InstrumentReference> AllInstrumentReferences)
        {
            this._AllInstrumentReferences = AllInstrumentReferences;
        }

        #endregion
        public Instrument SubscribeLevelOneWatch(string symbol, string exchange)//Changed By John
        {
            Instrument instrument = new Instrument();
            //instrument.Symbol = symbol;

            if (_AllInstrumentReferences.ContainsKey(symbol))
            {
                InstrumentReference inst = _AllInstrumentReferences[symbol];
                instrument.CompanyLongName = inst.CompanyLongName;
                //instrument.InstrumentID = symbol;
                instrument.Symbol = inst.Symbol;

                //mitchJSEClient.SubscribeLevel1Feed(inst.InstrumentID);
                mQTTAPI.HandleLevel1Request(symbol);
            }
            return instrument;
        }

        public bool SubscribeLevelTwoWatch(string symbol, string exchange)
        {
            if (_AllInstrumentReferences.ContainsKey(symbol))
            {
                InstrumentReference inst = _AllInstrumentReferences[symbol];
                string SYM = inst.SymbolCode;
                //mitchJSEClient.SubscribeLevel2Feed(inst.InstrumentID);
                mQTTAPI.HandleLevel2Request(symbol);
                return true;
            }
            return false;

            //Commented By John
            //while (!_isConnected)
            //{
            //    Thread.Sleep(1000);
            //}

            //using (SubscriptionManagementServiceClient proxy = new SubscriptionManagementServiceClient())
            //{
            //    string isin = GetISIN(symbol.Trim());
            //    if (string.IsNullOrEmpty(isin)) return false;

            //    var parameterList = new ParameterList();
            //    List<Parameter> parameters = new List<Parameter>();
            //    Parameter param1 = new Parameter();
            //    param1.Key = "Exchange";
            //    param1.Value = exchange;

            //    Parameter param2 = new Parameter();
            //    param2.Key = "ISIN";
            //    param2.Value = isin;

            //    parameters.Add(param1);
            //    parameters.Add(param2);

            //    parameterList.Parameters = parameters.ToArray();

            //var result = proxy.Subscribe(_sessionId, SubscriptionType.Level2, parameterList);
            //    if (result.Status == SubscriptionManagementService.ResultStatus.Failure)
            //    {
            //        string errorMessage = result.ErrorResult.ErrorList[0].Message;

            //        HandleSessionInvalidException(errorMessage);
            //    }
            //}

            //return true;
        }

        public void UnsubscribeLevelOneWatch(string symbol, string exchange)
        {
            mQTTAPI.UnsusbcribeLevel1(symbol);
            //barchart.UnsubsribeQuotes(symbol);
            //Commented By John
            //while (!_isConnected)
            //{
            //    Thread.Sleep(1000);
            //}

            //using(SubscriptionManagementServiceClient proxy  = new SubscriptionManagementServiceClient())
            //{
            //    string isin = GetISIN(symbol.Trim());
            //    if (string.IsNullOrEmpty(isin)) return;

            //    var parameterList = new ParameterList();
            //    List<Parameter> parameters = new List<Parameter>();
            //    Parameter param1 = new Parameter();
            //    param1.Key = "Exchange";
            //    param1.Value = exchange;

            //    Parameter param2 = new Parameter();
            //    param2.Key = "ISIN";
            //    param2.Value = isin;

            //    parameters.Add(param1);
            //    parameters.Add(param2);

            //    parameterList.Parameters = parameters.ToArray();

            //    var result = proxy.Unsubscribe(_sessionId, FundXchange.Data.McGregorBFA.SubscriptionManagementService.SubscriptionType.Level1, parameterList);
            //    if (result.Status == SubscriptionManagementService.ResultStatus.Failure)
            //    {
            //        string errorMessage = result.ErrorResult.ErrorList[0].Message;

            //        HandleSessionInvalidException(errorMessage);
            //    }
            //}
        }
        public bool UnsubscribeLevelTwoWatch(string symbol, string exchange)
        {
            mQTTAPI.UnsusbcribeLevel2(symbol);
            return true;
            //barchart.UnsubscribeLevel2(symbol);
            //Commented By John
            //while (!_isConnected)
            //{
            //    Thread.Sleep(1000);
            //}

            //using (SubscriptionManagementServiceClient proxy = new SubscriptionManagementServiceClient())
            //{
            //    string isin = GetISIN(symbol.Trim());

            //    var parameterList = new ParameterList();
            //    List<Parameter> parameters = new List<Parameter>();
            //    Parameter param1 = new Parameter();
            //    param1.Key = "Exchange";
            //    param1.Value = exchange;

            //    Parameter param2 = new Parameter();
            //    param2.Key = "ISIN";
            //    param2.Value = isin;

            //    parameters.Add(param1);
            //    parameters.Add(param2);

            //    parameterList.Parameters = parameters.ToArray();

            //    var result = proxy.Unsubscribe(_sessionId, FundXchange.Data.McGregorBFA.SubscriptionManagementService.SubscriptionType.Level2, parameterList);
            //    if (result.Status == SubscriptionManagementService.ResultStatus.Failure)
            //    {
            //        string errorMessage = result.ErrorResult.ErrorList[0].Message;

            //        HandleSessionInvalidException(errorMessage);
            //    }
            //}

        }
        public List<Candlestick> GetEquityCandlesticks(string symbol, string exchange, int candleInterval, int numberOfCandles, PeriodEnum period)
        {
            List<Candlestick> lst = new List<Candlestick>();
            string periodicity = GetBarChartPeriodicity(period, ref candleInterval);
            //result = barchart.RequestOHLC(symbol, periodicity, candleInterval, numberOfCandles);           
            string result = mQTTAPI.RequestOHLC(symbol, periodicity, candleInterval, numberOfCandles);
            if (result == string.Empty)
                return new List<Candlestick>();
            JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            OHLCResponse _socketResponce = serializer.Deserialize<OHLCResponse>((result));
            lst = CreateCandlesOHLC(_socketResponce.lstOHLCData);
            return lst;

            #region obsolete
            //using(LiveProtoCandlestickServicesClient proxy = new LiveProtoCandlestickServicesClient())
            //{
            //    string isin = GetISIN(symbol);

            //    if(string.IsNullOrEmpty(isin)) return new List<Candlestick>();

            //    CredentialDto credentials = new CredentialDto
            //                                    {
            //                                        UserName = _Username,
            //                                        Password = _Password,
            //                                        ProductId = _Product,
            //                                        VendorId = VendorId
            //                                    };
            //    InstrumentRequestDto instrumentRequest = new InstrumentRequestDto
            //                                                 {
            //                                                     ExchangeCode = exchange,
            //                                                     Identifier = isin
            //                                                 };
            //    CandleRequestDto candleRequest = new CandleRequestDto
            //                                         {
            //                                             Interval = candleInterval,
            //                                             NumberOfCandles = numberOfCandles,
            //                                             Period = (LiveProtoCandlestickService.PeriodEnum) period
            //                                         };

            //    var candlesResult = proxy.GetEquityCandlesticks(credentials, instrumentRequest, candleRequest);

            //    if (candlesResult.Status != LiveProtoCandlestickService.ResultStatus.Success)
            //    {
            //        string errorMessage = candlesResult.ErrorResult.ErrorList[0].Message;

            //        HandleSessionInvalidException(errorMessage);
            //    }
            //    else
            //    {
            //        ProtocolParser parser = new ProtocolParser();
            //        IList<object> data = parser.DeserializeFromWrapper(candlesResult.Result);
            //        if ((data != null) && (data.Count > 0) && _AllInstrumentReferences.ContainsKey(symbol))
            //        {
            //            var collection = (CandlestickCollectionDto)data[0];
            //            result = DataAdapter.GetCandlesticksFromDtos(collection, _AllInstrumentReferences[symbol]);
            //        }

            //    }
            //}
            #endregion
        }

        private List<Candlestick> CreateCandlesOHLC(List<Domain.OHLCData> lstOHLCData)
        {
            List<Candlestick> lstCandles = new List<Candlestick>();
            foreach (var item in lstOHLCData)
            {
                Candlestick candle = new Candlestick();
                candle.Open = Convert.ToDouble(item.Open.Replace(",", "."));
                candle.High = Convert.ToDouble(item.High.Replace(",", "."));
                candle.Low = Convert.ToDouble(item.Low.Replace(",", "."));
                candle.Close = Convert.ToDouble(item.Close.Replace(",", "."));
                candle.Volume = item.Volume;
                candle.TimeOfStart = candle.TimeOfClose = Convert.ToDateTime(item.feedDateTime);
                lstCandles.Add(candle);
            }
            return lstCandles;
        }

        private string GetBarChartPeriodicity(PeriodEnum period, ref int Interval)
        {
            switch (period)
            {
                case PeriodEnum.Minute:
                    return "Yearly";
                    return "Minutely";
                case PeriodEnum.Hour:
                    Interval = Interval * 60;
                    return "Minute";
                case PeriodEnum.Day:
                    return "Daily";
                    //return "D";
                case PeriodEnum.Week:
                    return "Weekly";
                case PeriodEnum.Month:
                    return "Monthly";
                case PeriodEnum.Year:
                    return "Yearly";                    
                default:
                    break;
            }
            return string.Empty;
        }


        private bool First = true;

        int Askcount = 0;
        int Bidcount = 0;

        int AskLevel = 0;
        int BidLevel = 0;

        private void Connect()
        {
            SocketHandshakeRequest req = CreateHandshakeRequest();
            byte[] binaryMessage = ProtocolParser.SerializeWithWrapper(req);

            _channel = new TcpChannel();
            _channel.ChannelDataReceived += new ChannelActionHandler(channel_ChannelDataReceived);
            _channel.Connect(DELIVERY_AGENT_ADDRESS, DELIVERY_AGENT_PORT);

            _channel.Send(binaryMessage);
        }

        private void GetEquitiesFromService()
        {
            try
            {
                using (ServiceDeskServicesClient proxy = new ServiceDeskServicesClient())
                {
                    var result = proxy.GetEquitiesListAsProto(_Username, _Password, _Product, VendorId, "JSE");

                    if (result.Status == ResultStatus.Failure)
                    {
                        string errors = "";
                        foreach (var error in result.ErrorResult.ErrorList)
                        {
                            errors += string.Format("({0}) {1}", error.Code, error.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            ProtocolParser parser = new ProtocolParser();
                            IList<object> data = parser.DeserializeFromWrapper(result.Result);
                            if ((data != null) && (data.Count > 0))
                            {
                                EquityDtoCollection collection = (EquityDtoCollection)data[0];

                                _AllInstrumentReferences = new Dictionary<string, InstrumentReference>();
                                _ISINToSymbolLookup = new Dictionary<string, string>();

                                if (collection.Equities.Count == 0) return;

                                if (File.Exists(@"Data\Equities.txt"))
                                    File.Delete(@"Data\Equities.txt");

                                using (TextWriter tw = new StreamWriter(@"Data\Equities.txt"))
                                {
                                    tw.WriteLine(DateTime.Now.ToString("u"));

                                    foreach (var equity in collection.Equities)
                                    {
                                        tw.Write(equity.ISIN);
                                        tw.Write("|");
                                        tw.Write(equity.ExchangeCode);
                                        tw.Write("|");
                                        tw.Write(equity.InstrumentType);
                                        tw.Write("|");
                                        tw.Write(equity.LongName);
                                        tw.Write("|");
                                        tw.Write(equity.ShortName);
                                        tw.Write("|");
                                        tw.Write(equity.Symbol);
                                        tw.WriteLine();

                                        InstrumentReference instrument = new InstrumentReference();

                                        instrument.Symbol = equity.Symbol;
                                        instrument.Exchange = equity.ExchangeCode;
                                        instrument.InstrumentType = equity.InstrumentType.ToString();
                                        instrument.ISIN = equity.ISIN;
                                        instrument.CompanyShortName = equity.ShortName;
                                        instrument.CompanyLongName = equity.LongName;

                                        if (!_AllInstrumentReferences.ContainsKey(instrument.Symbol.Trim()))
                                            _AllInstrumentReferences.Add(instrument.Symbol.Trim(), instrument);

                                        if (!_ISINToSymbolLookup.ContainsKey(instrument.ISIN.Trim()))
                                            _ISINToSymbolLookup.Add(instrument.ISIN.Trim(), instrument.Symbol);
                                    }
                                    tw.Close();
                                }
                            }


                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.ToString());
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private bool GetEquitiesFromFile()
        {
            bool shouldFetchFromService = false;

            _AllInstrumentReferences = new Dictionary<string, InstrumentReference>();
            _ISINToSymbolLookup = new Dictionary<string, string>();

            try
            {
                // create reader & open file
                using (TextReader tr = new StreamReader(@"Data\Equities.txt"))
                {
                    string lastDate = tr.ReadLine();
                    DateTime readDate = Convert.ToDateTime(lastDate);

                    if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                        shouldFetchFromService = (readDate.Date < DateTime.Now.Date);

                    string fileContents = tr.ReadToEnd();
                    string[] lines = fileContents.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(lines[i]))
                            {
                                string[] pipeSplit = lines[i].Replace('\r', ' ').Split('|');

                                InstrumentReference instrument = new InstrumentReference();

                                instrument.ISIN = pipeSplit[0];
                                instrument.Exchange = pipeSplit[1];
                                instrument.InstrumentType = pipeSplit[2];
                                instrument.CompanyLongName = pipeSplit[3];
                                instrument.CompanyShortName = pipeSplit[4];
                                instrument.Symbol = pipeSplit[5].Replace(" ", "");

                                if (!_AllInstrumentReferences.ContainsKey(instrument.Symbol.Trim()))
                                    _AllInstrumentReferences.Add(instrument.Symbol.Trim(), instrument);

                                if (!_ISINToSymbolLookup.ContainsKey(instrument.ISIN.Trim()))
                                    _ISINToSymbolLookup.Add(instrument.ISIN.Trim(), instrument.Symbol);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                    }

                    tr.Close();
                }
            }
            catch (Exception outer)
            {
                Console.WriteLine(outer.ToString());
            }

            return shouldFetchFromService;
        }

        private void GetIndexListFromService()
        {
            using (ServiceDeskServicesClient proxy = new ServiceDeskServicesClient())
            {
                var result = proxy.GetIndicesListAsProto(_Username, _Password, _Product, VendorId, "JSE");
                if (result.Status == ResultStatus.Failure)
                {
                    string errorMessage = result.ErrorResult.ErrorList[0].Message;

                    HandleSessionInvalidException(errorMessage);
                }
                else
                {
                    ProtocolParser parser = new ProtocolParser();
                    IList<object> data = parser.DeserializeFromWrapper(result.Result);
                    if ((data != null) && (data.Count > 0))
                    {
                        IndexDtoCollection collection = (IndexDtoCollection)data[0];

                        if (collection.Indices.Count == 0) return;

                        if (File.Exists(@"Data\Indexes.txt"))
                            File.Delete(@"Data\Indexes.txt");

                        using (TextWriter tw = new StreamWriter(@"Data\Indexes.txt"))
                        {
                            tw.WriteLine(DateTime.Now.ToString("u"));

                            foreach (var dto in collection.Indices)
                            {
                                tw.Write(dto.Code);
                                tw.Write("|");
                                tw.Write(dto.ExchangeCode);
                                tw.Write("|");
                                tw.Write(dto.Alpha);
                                tw.Write("|");
                                tw.Write(dto.LongName);
                                tw.Write("|");
                                tw.Write(dto.ShortName);
                                tw.WriteLine();

                                Index index = new Index();

                                index.Exchange = "JSE";
                                index.Alpha = dto.Alpha;
                                index.CompanyLongName = dto.LongName;
                                index.CompanyShortName = dto.ShortName;
                                index.ShortName = dto.ShortName;
                                index.Symbol = dto.Code;

                                if (index.Symbol != "J251" && index.Symbol != "J330" && index.Symbol != "J331" && index.Symbol != "J374" && index.Symbol != "J230")
                                {
                                    if (!_AllIndicesReferences.ContainsKey(index.Symbol))
                                        _AllIndicesReferences.Add(index.Symbol, index);
                                }
                            }
                            tw.Close();
                        }
                    }
                }
            }
        }

        private bool GetIndexListFromFile()
        {
            bool shouldFetchFromService = false;
            _AllIndicesReferences = new Dictionary<string, Index>();

            try
            {
                // create reader & open file
                using (TextReader tr = new StreamReader(@"Data\Indexes.txt"))
                {

                    string lastDate = tr.ReadLine();
                    DateTime readDate = Convert.ToDateTime(lastDate);
                    shouldFetchFromService = readDate.Date != DateTime.Now.Date;

                    string fileContents = tr.ReadToEnd();
                    string[] lines = fileContents.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(lines[i]))
                            {
                                string[] pipeSplit = lines[i].Replace('\r', ' ').Split('|');

                                Index index = new Index();

                                index.Exchange = pipeSplit[1];
                                index.Alpha = pipeSplit[2];
                                index.CompanyLongName = pipeSplit[3];
                                index.CompanyShortName = pipeSplit[4];
                                index.ShortName = pipeSplit[4];
                                index.Symbol = pipeSplit[0].Replace(" ", "");

                                if (index.Symbol != "J251" && index.Symbol != "J330" && index.Symbol != "J331" && index.Symbol != "J374" && index.Symbol != "J230")
                                {
                                    if (!_AllIndicesReferences.ContainsKey(index.Symbol))
                                        _AllIndicesReferences.Add(index.Symbol, index);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }

                    tr.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return shouldFetchFromService;
        }

        public List<Index> GetIndicesStatistics(string exchange)
        {
            List<Index> indexes = new List<Index>();

            try
            {
                using (ServiceDeskServicesClient proxy = new ServiceDeskServicesClient())
                {
                    var result = proxy.GetIndicesSnaphotsAsProto(_Username, _Password, _Product, VendorId, exchange);

                    ProtocolParser parser = new ProtocolParser();
                    IList<object> data = parser.DeserializeFromWrapper(result.Result);
                    if ((data != null) && (data.Count > 0))
                    {
                        IndexValueDtoCollection collection = (IndexValueDtoCollection)data[0];
                        foreach (var dto in _AllIndicesReferences)
                        {
                            Index index = new Index();

                            index.Exchange = exchange;
                            index.CompanyLongName = dto.Value.CompanyLongName;
                            index.CompanyShortName = dto.Value.CompanyShortName;
                            index.ShortName = dto.Value.CompanyShortName;
                            index.Symbol = dto.Value.Symbol;

                            var indexValueDto = collection.Indices.FirstOrDefault(i => i.IndexIdentifier == index.Symbol);
                            if (indexValueDto != null)
                            {
                                long move = indexValueDto.IndexValue - indexValueDto.YesterdayClose;
                                index.CentsMoved = move;
                                index.Last3MonthClosePrice = indexValueDto.Last3MonthClose;
                                index.LastMonthClosePrice = indexValueDto.LastMonthClose;
                                index.LastWeekClosePrice = indexValueDto.LastWeekClose;
                                index.LastYearClosePrice = indexValueDto.LastYearClose;
                                index.PercentageMoved = 0;

                                if (indexValueDto.YesterdayClose > 0)
                                {
                                    double percentageMoved = ((double)move * 100) / (double)indexValueDto.YesterdayClose;
                                    index.PercentageMoved = percentageMoved;
                                }
                                index.Value = indexValueDto.IndexValue;
                                index.YesterdayClose = indexValueDto.YesterdayClose;
                            }

                            indexes.Add(index);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception fetching indexes: " + ex.ToString());
            }

            return indexes;

        }

        private SocketHandshakeRequest CreateHandshakeRequest()
        {
            _Product = "RDLive";
            SocketHandshakeRequest _User = new SocketHandshakeRequest();
            _User.Username = _Username;
            _User.Password = _Password;
            _User.ProductId = _Product;
            _User.CustomCredentials = "";
            _User.Context = _Product;
            _User.VendorId = VendorId;
            _User.ForceSessionOverride = true;

            return _User;
        }

        private string GetISIN(string symbol)
        {
            if (_AllInstrumentReferences.ContainsKey(symbol))
            {
                return _AllInstrumentReferences[symbol].ISIN;
            }
            return "";
        }

        private string GetSymbolFromISIN(string isin)
        {
            try
            {
                if (_ISINToSymbolLookup.ContainsKey(isin))
                {
                    return _ISINToSymbolLookup[isin];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in GetSymbolFromISIN: " + ex.ToString());
            }
            return "";
        }

        public Instrument SubscribeIndex(string indexIdentifier, string exchange)
        {
            if (string.IsNullOrEmpty(indexIdentifier))
            {
                return null;
            }

            Instrument instrument;

            SubscribeIndexWatch(indexIdentifier, exchange);
            GetIndexSnapshot(indexIdentifier, exchange, out instrument);

            return _AllInstruments[instrument.ISIN];
        }

        public List<Instrument> SubscribeLevelOneWatches(List<string> symbols, string exchange)
        {
            string isinList = "";
            List<string> isins = new List<string>();
            foreach (string symbol in symbols)
            {
                string isin = GetISIN(symbol.Trim());
                if (!String.IsNullOrEmpty(isin))
                {
                    if (isinList.Length > 0)
                        isinList += ",";
                    isinList += isin;
                    isins.Add(isin);
                }
            }
            SubscribeLevel1RealtimeAPI(exchange, isinList);
            List<Instrument> instruments = new List<Instrument>();
            GetEquitySnapshots(isins, exchange, out instruments);
            return instruments;
        }

        private void SubscribeLevel1RealtimeAPI(string exchange, string isin)
        {
            while (!_isConnected)
            {
                Thread.Sleep(1000);
            }

            using (SubscriptionManagementServiceClient proxy = new SubscriptionManagementServiceClient())
            {
                var parameterList = new ParameterList();
                List<Parameter> parameters = new List<Parameter>();
                Parameter param1 = new Parameter();
                param1.Key = "Exchange";
                param1.Value = exchange;

                Parameter param2 = new Parameter();
                param2.Key = "ISIN";
                param2.Value = isin;

                parameters.Add(param1);
                parameters.Add(param2);

                parameterList.Parameters = parameters.ToArray();

                var result = proxy.Subscribe(_sessionId, FundXchange.Data.McGregorBFA.SubscriptionManagementService.SubscriptionType.Level1, parameterList);
                if (result.Status == SubscriptionManagementService.ResultStatus.Failure)
                {
                    string errorMessage = result.ErrorResult.ErrorList[0].Message;

                    HandleSessionInvalidException(errorMessage);
                }
            }
        }

        private void HandleSessionInvalidException(string errorMessage)
        {
            if (errorMessage.ToLower().Contains(SESSION_INVALID_EX))
            {
                _isConnected = false;



                Connect();
            }
            else
            {
                if (ErrorOccurredEvent != null)
                {
                    ErrorOccurredEvent(errorMessage);
                }
            }
        }

        private void GetEquitySnapshot(string symbol, string isin, string exchange, out Instrument instrument)
        {
            instrument = new Instrument();

            using (ServiceDeskServicesClient servicesClient = new ServiceDeskServicesClient())
            {
                var result = servicesClient.GetEquitySnapshotAsProto(_Username, _Password, _Product, VendorId, exchange, isin);

                instrument.Exchange = exchange;
                instrument.Symbol = symbol;
                instrument.ISIN = isin;

                if (result.Status == ResultStatus.Success && result.Result != null)
                {
                    ProtocolParser parser = new ProtocolParser();
                    IList<object> data = parser.DeserializeFromWrapper(result.Result);
                    if ((data != null) && (data.Count > 0))
                    {
                        var equity = (Fin24.LiveData.Common.DTOs.EquityDetailsDto)data[0];

                        instrument.BestBid = equity.BestBidPrice;
                        instrument.BestOffer = equity.BestOfferPrice;
                        instrument.BidVolume = equity.VolumeOrdersAtBestBidPrice;
                        instrument.CentsMoved = equity.CentsMoved;
                        instrument.High = equity.High;
                        instrument.LastTradeSequenceNumber = equity.LastTrade_SequenceNumber;
                        instrument.LastTrade = equity.LastTrade_TradePrice;
                        instrument.LastTradeTime = new DateTime(equity.LastTrade_TradeDateAsTicks);
                        instrument.Low = equity.Low;
                        instrument.OfferVolume = equity.VolumeOrdersAtBestOfferPrice;
                        instrument.Open = equity.Open;
                        instrument.CentsMoved = equity.CentsMoved;
                        if (equity.YesterdayClose > 0)
                            instrument.PercentageMoved = (instrument.CentsMoved * 100) / equity.YesterdayClose; //equityResult.Result.PercentageMoved;
                        instrument.YesterdayClose = equity.YesterdayClose;
                        instrument.TotalTrades = equity.NumberOfDeals;
                        instrument.TotalValue = equity.DailyValue;
                        instrument.TotalVolume = equity.DailyVolume;

                        if (_AllInstrumentReferences.ContainsKey(equity.Symbol))
                        {
                            InstrumentReference instrumentReference = _AllInstrumentReferences[equity.Symbol];
                            instrument.CompanyShortName = instrumentReference.CompanyShortName;
                            instrument.CompanyLongName = instrumentReference.CompanyLongName;
                        }
                    }
                }

                if (instrument.ISIN != null && !_AllInstruments.ContainsKey(instrument.ISIN))
                {
                    _AllInstruments.Add(instrument.ISIN, instrument);
                }
                else
                {
                    _AllInstruments[instrument.ISIN] = instrument;
                }
            }
        }

        private void GetIndexSnapshot(string indexIndetifier, string exchange, out Instrument instrument)
        {
            instrument = new Instrument();
            Index index = new Index();

            using (ServiceDeskServicesClient proxy = new ServiceDeskServicesClient())
            {
                var result = proxy.GetIndicesSnaphotAsProto(_Username, _Password, _Product, VendorId, exchange, indexIndetifier);

                instrument.Exchange = exchange;
                instrument.Symbol = indexIndetifier;
                instrument.ISIN = indexIndetifier;

                index.Exchange = exchange;
                index.Symbol = indexIndetifier;
                index.SymbolCode = indexIndetifier;

                if (result.Result != null && result.Status == ResultStatus.Success)
                {
                    ProtocolParser parser = new ProtocolParser();
                    IList<object> data = parser.DeserializeFromWrapper(result.Result);
                    if ((data != null) && (data.Count > 0))
                    {
                        var indexDto = (Fin24.LiveData.Common.DTOs.IndexValueDto)data[0];

                        instrument.LastTrade = indexDto.IndexValue;
                        instrument.YesterdayClose = indexDto.YesterdayClose;
                        long move = indexDto.IndexValue - indexDto.YesterdayClose;
                        instrument.CentsMoved = move;
                        if (indexDto.YesterdayClose > 0)
                            instrument.PercentageMoved = (instrument.CentsMoved * 100) / indexDto.YesterdayClose;
                        instrument.LastTradeSequenceNumber = indexDto.SequenceNumber;

                        index = CreateIndexFromInstrument(instrument);
                    }
                }

                if (!_AllIndicesReferences.ContainsKey(index.Symbol))
                    _AllIndicesReferences.Add(index.Symbol, index);
                else
                    _AllIndicesReferences[index.Symbol] = index;

                if (!_AllInstruments.ContainsKey(instrument.Symbol))
                    _AllInstruments.Add(instrument.ISIN, instrument);
                else
                    _AllInstruments[instrument.ISIN] = instrument;
            }
        }

        private Index CreateIndexFromInstrument(Instrument instrument)
        {
            Index index = new Index();

            index.Exchange = instrument.Exchange;
            index.Symbol = instrument.Symbol;
            index.SymbolCode = instrument.Symbol;

            if (_AllIndicesReferences.ContainsKey(instrument.Symbol))
            {
                index.CompanyLongName = _AllIndicesReferences[instrument.Symbol].CompanyLongName;
                index.CompanyShortName = _AllIndicesReferences[instrument.Symbol].CompanyShortName;
                index.ShortName = _AllIndicesReferences[instrument.Symbol].ShortName;
            }
            else
            {
                index.CompanyLongName = "";
                index.CompanyShortName = "";
                index.ShortName = "";
            }

            index.Value = (long)instrument.LastTrade;
            index.YesterdayClose = (long)instrument.YesterdayClose;
            index.CentsMoved = (long)instrument.CentsMoved;
            index.PercentageMoved = instrument.PercentageMoved;

            return index;
        }

        private void GetEquitySnapshots(List<string> isins, string exchange, out List<Instrument> instruments)
        {
            instruments = new List<Instrument>();

            using (ServiceDeskServicesClient servicesClient = new ServiceDeskServicesClient())
            {
                var equityResult = servicesClient.GetEquitySnapshotForISINsAsProto(_Username, _Password, _Product, VendorId, exchange, isins.ToArray());

                if (equityResult.Status == ResultStatus.Success && equityResult.Result != null)
                {
                    ProtocolParser parser = new ProtocolParser();
                    IList<object> data = parser.DeserializeFromWrapper(equityResult.Result);
                    if ((data != null) && (data.Count > 0))
                    {
                        var collection = (EquityDetailsDtoCollection)data[0];
                        foreach (var result in collection.Equities)
                        {
                            Instrument instrument = new Instrument();

                            instrument.Exchange = exchange;
                            instrument.Symbol = result.Symbol;
                            instrument.ISIN = result.ISIN;
                            instrument.BestBid = result.BestBidPrice;
                            instrument.BestOffer = result.BestOfferPrice;
                            instrument.BidVolume = result.VolumeOrdersAtBestBidPrice;
                            instrument.CentsMoved = result.CentsMoved;
                            instrument.High = result.High;
                            instrument.LastTradeSequenceNumber = result.LastTrade_SequenceNumber;
                            instrument.LastTrade = result.LastTrade_TradePrice;
                            instrument.LastTradeTime = new DateTime(result.LastTrade_TradeDateAsTicks);
                            instrument.Low = result.Low;
                            instrument.OfferVolume = result.VolumeOrdersAtBestOfferPrice;
                            instrument.Open = result.Open;
                            instrument.CentsMoved = result.CentsMoved;
                            if (result.YesterdayClose > 0)
                                instrument.PercentageMoved = (instrument.CentsMoved * 100) / result.YesterdayClose; //equityResult.Result.PercentageMoved;

                            instrument.YesterdayClose = result.YesterdayClose;
                            instrument.TotalTrades = result.NumberOfDeals;
                            instrument.TotalValue = result.DailyValue;
                            instrument.TotalVolume = result.DailyVolume;

                            instruments.Add(instrument);

                            if (!_AllInstruments.ContainsKey(instrument.ISIN))
                            {
                                _AllInstruments.Add(instrument.ISIN, instrument);
                            }
                            else
                            {
                                _AllInstruments[instrument.ISIN] = instrument;
                            }
                        }
                    }

                }
            }
        }

        public void SubscribeIndexWatch(string symbol, string exchange)
        {
            while (!_isConnected)
            {
                Thread.Sleep(1000);
            }

            using (SubscriptionManagementServiceClient proxy = new SubscriptionManagementServiceClient())
            {
                var parameterList = new ParameterList();
                List<Parameter> parameters = new List<Parameter>();
                Parameter param1 = new Parameter();
                param1.Key = "Exchange";
                param1.Value = exchange;

                Parameter param2 = new Parameter();
                param2.Key = "IndexIdentifier";
                param2.Value = symbol;

                parameters.Add(param1);
                parameters.Add(param2);

                parameterList.Parameters = parameters.ToArray();

                var result = proxy.Subscribe(_sessionId, FundXchange.Data.McGregorBFA.SubscriptionManagementService.SubscriptionType.Index, parameterList);
                if (result.Status == SubscriptionManagementService.ResultStatus.Failure)
                {
                    string errorMessage = result.ErrorResult.ErrorList[0].Message;

                    HandleSessionInvalidException(errorMessage);
                }
            }
        }

        public void UnsubscribeIndexWatch(string symbol, string exchange)
        {
            while (!_isConnected)
            {
                Thread.Sleep(1000);
            }

            using (SubscriptionManagementServiceClient proxy = new SubscriptionManagementServiceClient())
            {
                var parameterList = new ParameterList();
                List<Parameter> parameters = new List<Parameter>();
                Parameter param1 = new Parameter();
                param1.Key = "Exchange";
                param1.Value = exchange;

                Parameter param2 = new Parameter();
                param2.Key = "IndexIdentifier";
                param2.Value = symbol;

                parameters.Add(param1);
                parameters.Add(param2);

                parameterList.Parameters = parameters.ToArray();

                var result = proxy.Unsubscribe(_sessionId, FundXchange.Data.McGregorBFA.SubscriptionManagementService.SubscriptionType.Index, parameterList);
                if (result.Status == SubscriptionManagementService.ResultStatus.Failure)
                {
                    string errorMessage = result.ErrorResult.ErrorList[0].Message;

                    HandleSessionInvalidException(errorMessage);
                }
            }
        }

        public void UnsubscribeLevelOneWatches(List<string> symbols, string exchange)
        {
            while (!_isConnected)
            {
                Thread.Sleep(1000);
            }

            string symbolList = "";
            foreach (string symbol in symbols)
            {
                string isin = GetISIN(symbol.Trim());
                if (symbolList.Length > 0)
                    symbolList += ",";
                symbolList += isin;
            }

            using (SubscriptionManagementServiceClient proxy = new SubscriptionManagementServiceClient())
            {
                var parameterList = new ParameterList();
                List<Parameter> parameters = new List<Parameter>();
                Parameter param1 = new Parameter();
                param1.Key = "Exchange";
                param1.Value = exchange;

                Parameter param2 = new Parameter();
                param2.Key = "ISIN";
                param2.Value = symbolList;

                parameters.Add(param1);
                parameters.Add(param2);

                parameterList.Parameters = parameters.ToArray();

                var result = proxy.Unsubscribe(_sessionId, FundXchange.Data.McGregorBFA.SubscriptionManagementService.SubscriptionType.Level1, parameterList);
                if (result.Status == SubscriptionManagementService.ResultStatus.Failure)
                {
                    string errorMessage = result.ErrorResult.ErrorList[0].Message;

                    HandleSessionInvalidException(errorMessage);
                }
            }
        }

        public string VendorId
        {
            get { return ConfigurationManager.AppSettings["VendorID"]; }
        }

        

        public List<Candlestick> GetIndexCandlesticks(string index, string exchange, int candleInterval, int numberOfCandles, PeriodEnum period)
        {
            List<Candlestick> result = new List<Candlestick>();

            using (LiveProtoCandlestickServicesClient proxy = new LiveProtoCandlestickServicesClient())
            {
                if (string.IsNullOrEmpty(index)) return new List<Candlestick>();

                CredentialDto credentials = new CredentialDto
                {
                    UserName = _Username,
                    Password = _Password,
                    ProductId = _Product,
                    VendorId = VendorId
                };
                InstrumentRequestDto instrumentRequest = new InstrumentRequestDto
                {
                    ExchangeCode = exchange,
                    Identifier = index
                };
                CandleRequestDto candleRequest = new CandleRequestDto
                {
                    Interval = candleInterval,
                    NumberOfCandles = numberOfCandles,
                    Period = (LiveProtoCandlestickService.PeriodEnum)period
                };

                var candlesResult = proxy.GetIndexCandlesticks(credentials, instrumentRequest, candleRequest);

                if (candlesResult.Status != LiveProtoCandlestickService.ResultStatus.Success)
                {
                    string errorMessage = candlesResult.ErrorResult.ErrorList[0].Message;

                    HandleSessionInvalidException(errorMessage);
                }
                else
                {
                    ProtocolParser parser = new ProtocolParser();
                    IList<object> data = parser.DeserializeFromWrapper(candlesResult.Result);
                    if ((data != null) && (data.Count > 0))
                    {
                        var collection = (CandlestickCollectionDto)data[0];
                        result = DataAdapter.GetCandlesticksFromDtos(collection, _AllIndicesReferences[index]);
                    }
                }
            }

            return result;
        }

        public List<Instrument> GetTop20InstrumentsByValue(string exchange)
        {
            List<Instrument> instruments = new List<Instrument>();
            try
            {
                using (ServiceDeskServicesClient proxy = new ServiceDeskServicesClient())
                {
                    var result = proxy.GetTop20EquitiesByValueAsProto(_Username, _Password, _Product, VendorId, exchange, _instrumentTypes.ToArray());
                    if (result.Status == ResultStatus.Failure)
                    {
                        string errorMessage = result.ErrorResult.ErrorList[0].Message;

                        HandleSessionInvalidException(errorMessage);
                    }
                    else
                    {
                        ProtocolParser parser = new ProtocolParser();
                        IList<object> data = parser.DeserializeFromWrapper(result.Result);
                        if ((data != null) && (data.Count > 0))
                        {
                            var collection = (EquityDetailsDtoCollection)data[0];
                            instruments = DataAdapter.GetInstrumentsFromDtos(collection);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception with GetTop20InstrumentsByValue: " + ex.ToString());
            }
            return instruments;
        }

        public List<Instrument> GetTop20InstrumentsByVolume(string exchange)
        {
            List<Instrument> instruments = new List<Instrument>();
            try
            {
                using (ServiceDeskServicesClient proxy = new ServiceDeskServicesClient())
                {
                    var result = proxy.GetTop20EquitiesByVolumeAsProto(_Username, _Password, _Product, VendorId, exchange, _instrumentTypes.ToArray());
                    if (result.Status == ResultStatus.Failure)
                    {
                        string errorMessage = result.ErrorResult.ErrorList[0].Message;

                        HandleSessionInvalidException(errorMessage);
                    }
                    else
                    {
                        ProtocolParser parser = new ProtocolParser();
                        IList<object> data = parser.DeserializeFromWrapper(result.Result);
                        if ((data != null) && (data.Count > 0))
                        {
                            var collection = (EquityDetailsDtoCollection)data[0];
                            instruments = DataAdapter.GetInstrumentsFromDtos(collection);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception with GetTop20InstrumentsByVolume: " + ex.ToString());
            }
            return instruments;
        }

        public List<Instrument> GetWinners(string exchange)
        {
            List<Instrument> instruments = new List<Instrument>();
            try
            {
                using (ServiceDeskServicesClient proxy = new ServiceDeskServicesClient())
                {
                    var result = proxy.GetWinnersAsProto(_Username, _Password, _Product, VendorId, exchange, _instrumentTypes.ToArray());
                    if (result.Status == ResultStatus.Failure)
                    {
                        string errorMessage = result.ErrorResult.ErrorList[0].Message;

                        HandleSessionInvalidException(errorMessage);
                    }
                    else
                    {
                        ProtocolParser parser = new ProtocolParser();
                        IList<object> data = parser.DeserializeFromWrapper(result.Result);
                        if ((data != null) && (data.Count > 0))
                        {
                            var collection = (EquityDetailsDtoCollection)data[0];
                            instruments = DataAdapter.GetInstrumentsFromDtos(collection);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception with GetWinners: " + ex.ToString());
            }
            return instruments;
        }

        public List<Instrument> GetLosers(string exchange)
        {
            List<Instrument> instruments = new List<Instrument>();
            try
            {
                using (ServiceDeskServicesClient proxy = new ServiceDeskServicesClient())
                {
                    var result = proxy.GetLosersAsProto(_Username, _Password, _Product, VendorId, exchange, _instrumentTypes.ToArray());
                    if (result.Status == ResultStatus.Failure)
                    {
                        string errorMessage = result.ErrorResult.ErrorList[0].Message;

                        HandleSessionInvalidException(errorMessage);
                    }
                    else
                    {
                        ProtocolParser parser = new ProtocolParser();
                        IList<object> data = parser.DeserializeFromWrapper(result.Result);
                        if ((data != null) && (data.Count > 0))
                        {
                            var collection = (EquityDetailsDtoCollection)data[0];
                            instruments = DataAdapter.GetInstrumentsFromDtos(collection);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception with GetLosers: " + ex.ToString());
            }
            return instruments;
        }

        public List<Index> GetIndices(string exchange)
        {
            while (!_isConnected)
            {
                Thread.Sleep(1000);
            }
            return _AllIndicesReferences.Values.ToList();
        }

        public List<Instrument> GetTop40Instruments(string exchange)
        {
            List<Instrument> instruments = new List<Instrument>();
            return instruments;

            using (ServiceDeskServicesClient proxy = new ServiceDeskServicesClient())
            {
                var result = proxy.GetTop40EquitiesStatisticsAsProto(_Username, _Password, _Product, VendorId, exchange);
                if (result.Status == ResultStatus.Failure)
                {
                    string errorMessage = result.ErrorResult.ErrorList[0].Message;

                    HandleSessionInvalidException(errorMessage);
                }
                else
                {
                    ProtocolParser parser = new ProtocolParser();
                    IList<object> data = parser.DeserializeFromWrapper(result.Result);
                    if ((data != null) && (data.Count > 0))
                    {
                        EquityDetailsDtoCollection collection = (EquityDetailsDtoCollection)data[0];
                        instruments = DataAdapter.GetInstrumentsFromDtos(collection.Equities.ToArray());
                    }

                }

            }

            return instruments;
        }

        public List<InstrumentStatistics> GetMarketsHomeData(string exchange)
        {
            return new List<InstrumentStatistics>();
        }

        public List<Sens> GetSensData()
        {
            List<Sens> result = new List<Sens>();

            //By John
            return result;




            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                string sensUrl = ConfigurationManager.AppSettings["SensUrl"].Replace('|', '&');
                xmlDocument.Load(sensUrl);

                XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("record");

                foreach (XmlNode node in xmlNodeList)
                {
                    Sens sens = new Sens();

                    sens.NewsId = Guid.NewGuid().ToString();
                    sens.Symbol = node.ChildNodes.Item(0).InnerText;
                    sens.Description = node.ChildNodes.Item(1).InnerText;
                    sens.Story = node.ChildNodes.Item(2).InnerText;
                    sens.Time = node.ChildNodes.Item(3).InnerText + " " + node.ChildNodes.Item(4).InnerText;

                    result.Add(sens);
                }

                _SensList.Clear();
                _SensList = result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception with GetSensData: " + ex.ToString());
            }

            return result;
        }

        public string GetSensBody(string id)
        {
            Sens sens = _SensList.Find(s => s.NewsId == id);

            if (sens == null) return "";
            return sens.Story;
        }

        public List<InstrumentReference> GetInstrumentReferencesForType(InstrumentType type)
        {
            return new List<InstrumentReference>();
        }

        public List<Index> GetIndexReferenceForAllCurrentIndeces()
        {
            List<Index> indexRefs = new List<Index>();

            foreach (KeyValuePair<string, Index> keyValuePair in _AllIndicesReferences)
            {
                if (!indexRefs.Contains(keyValuePair.Value))
                    indexRefs.Add(keyValuePair.Value);
            }

            return indexRefs;
        }
        public List<Domain.ValueObjects.Trade> GetTrades(string exchange, string symbol, int pageSize)
        {
            List<Domain.ValueObjects.Trade> trades = new List<Domain.ValueObjects.Trade>();
            //trades[0].

            return trades;
            //Commented By John
            //using (ServiceDeskServicesClient proxy = new ServiceDeskServicesClient()) 
            //{
            //    string isin = GetISIN(symbol.Trim());
            //    var result = proxy.GetTradesForPageSizeAsProto(_Username, _Password, _Product, VendorId, exchange, isin, pageSize);
            //    if (result.Status == ResultStatus.Failure)
            //    {
            //        string errorMessage = result.ErrorResult.ErrorList[0].Message;

            //        HandleSessionInvalidException(errorMessage);
            //    }
            //    else
            //    {
            //        ProtocolParser parser = new ProtocolParser();
            //        IList<object> data = parser.DeserializeFromWrapper(result.Result);
            //        if ((data != null) && (data.Count > 0))
            //        {
            //            var collection = (TradeDtoCollection)data[0];
            //            collection.Trades.Reverse();
            //            var dtos = collection.Trades.Take(pageSize).ToArray();
            //            foreach (var dto in dtos)
            //            {
            //                Trade trade = new Trade();

            //                trade.Exchange = exchange;
            //                trade.Symbol = symbol;

            //                trade.Price = dto.TradePrice;
            //                trade.TimeStamp = new DateTime(dto.TradeTimeAsTicks);
            //                trade.Volume = dto.TradeSize;

            //                trade.Quote = DataAdapter.GetQuoteFromDto(dto.Quote);
            //                trades.Add(trade);
            //            }
            //        }

            //    }
            //}
            //return trades;
        }

        public List<Domain.ValueObjects.Quote> GetQuotes(string exchange, string symbol, DateTime fromDate)
        {
            return new List<Domain.ValueObjects.Quote>();
        }

        void channel_ChannelDataReceived(object sender, ChannelActionEventArgs evtArgs)
        {
            IList<object> dataList = _parser.DeserializeFromWrapper(evtArgs.ChannelData);

            if (dataList == null)
            {
                return;
            }

            foreach (object data in dataList)
            {
                if (data is SocketHandshakeResponse)
                {
                    SocketHandshakeResponse connectionConfirmMsg = (SocketHandshakeResponse)data;

                    _sessionId = connectionConfirmMsg.SessionId;
                    if (string.IsNullOrEmpty(_sessionId))
                    {
                        if (ErrorOccurredEvent != null)
                        {
                            ErrorOccurredEvent(connectionConfirmMsg.ResultMessage);
                        }
                    }
                    else
                    {
                        _isConnected = true;

                        if (shouldGetEquitiesFromService)
                            GetEquitiesFromService();
                        if (shouldGetIndexFromService)
                            GetIndexListFromService();

                        if (DataProviderConnectionChangedEvent != null)
                            DataProviderConnectionChangedEvent(true);
                    }
                }
                else
                {
                    ProcessDataReceived(data);
                }
            }
        }

        void _messageHandlerHeartbeat_MessageReceived(HeartbeatMessage message)
        {
            ProcessHeartbeatMessage(message);
        }

        void _messageHandleIndexValue_MessageReceived(IndexValue message)
        {
            ProcessIndexValue(message);
        }

        void _messageHandlerOrderbookEvent_MessageReceived(OrderbookEvent message)
        {
            if (message.Type == OrderbookEvent.OrderbookType.OrderInitialize)
            {
                ProcessOrderbookDto((OrderbookDto)message.Message);
            }
            else if (message.Type == OrderbookEvent.OrderbookType.OrderAdd)
            {
                ProcessOrderDetails((OrderDetails)message.Message);
            }
            else if (message.Type == OrderbookEvent.OrderbookType.OrderDelete)
            {
                ProcessOrderDeletion((OrderDeletion)message.Message);
            }
        }

        void _messageHandlerUncrossingPriceAndVolume_MessageReceived(UncrossingPriceAndVolume message)
        {
            ProcessUncrossingPriceAndVolume(message);
        }

        void _messageHandlerEnhancedBestPrice_MessageReceived(EnhancedBestPrice message)
        {
            ProcessEnhancedBestPrice(message);
        }

        void _messageHandlerTradeReports_MessageReceived(TradeReport message)
        {
            ProcessTradeReport(message);
        }

        void _messageHandlerCumulativeNumberAndVolumeOfTrades_MessageReceived(CumulativeNumberAndVolumeOfTrades message)
        {
            ProcessCumulativeNumberAndVolumeOfTrades(message);
        }

        void _messageHandlerClosingPrice_MessageReceived(ClosingPrice message)
        {
            ProcessClosingPrice(message);
        }

        private void ProcessDataReceived(object data)
        {
            try
            {
                if (data is TradeReport) // 5OZ
                {
                    //trade
                    _messageHandlerTradeReports.AddMessage((TradeReport)data);

                }
                else if (data is EnhancedBestPrice) // 5SF
                {
                    //quote
                    _messageHandlerEnhancedBestPrice.AddMessage((EnhancedBestPrice)data);
                }
                else if (data is OrderbookDto)
                {
                    _messageHandlerOrderbookEvent.AddMessage(new OrderbookEvent(OrderbookEvent.OrderbookType.OrderInitialize, data));
                }
                else if (data is OrderDetails) // 5OO
                {
                    _messageHandlerOrderbookEvent.AddMessage(new OrderbookEvent(OrderbookEvent.OrderbookType.OrderAdd, data));
                }
                else if (data is OrderDeletion) // 5OE
                {
                    _messageHandlerOrderbookEvent.AddMessage(new OrderbookEvent(OrderbookEvent.OrderbookType.OrderDelete, data));
                }
                else if (data is UncrossingPriceAndVolume) // 5UD
                {
                    _messageHandlerUncrossingPriceAndVolume.AddMessage((UncrossingPriceAndVolume)data);
                }
                else if (data is UncrossingCompleted) // 5SX
                {
                    // Notifies the end of the uncrossing period (quotes and orders can be processed again).
                    // This has no current implementation.
                }
                else if (data is ClosingPrice) // 5SK
                {
                    _messageHandlerClosingPrice.AddMessage((ClosingPrice)data);
                }
                else if (data is CumulativeNumberAndVolumeOfTrades) // 5DV
                {
                    _messageHandlerCumulativeNumberAndVolumeOfTrades.AddMessage((CumulativeNumberAndVolumeOfTrades)data);
                }
                else if (data is IndexValue)
                {
                    _messageHandleIndexValue.AddMessage((IndexValue)data);
                }
                else if (data is HeartbeatMessage)
                {
                    _messageHandlerHeartbeat.AddMessage((HeartbeatMessage)data);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("Something went wrong here: {0}", exception.ToString()));
            }
        }

        private void ProcessIndexValue(IndexValue msg)
        {
            if (_AllIndicesReferences.ContainsKey(msg.IndexIdentifier))
            {
                _AllIndicesReferences[msg.IndexIdentifier].Value = msg.Value;
                if (_AllIndicesReferences[msg.IndexIdentifier].YesterdayClose > 0)
                {
                    long centsMoved = msg.Value - _AllIndicesReferences[msg.IndexIdentifier].YesterdayClose;
                    _AllIndicesReferences[msg.IndexIdentifier].CentsMoved = centsMoved;
                    _AllIndicesReferences[msg.IndexIdentifier].PercentageMoved = ((double)centsMoved * 100) / (double)_AllIndicesReferences[msg.IndexIdentifier].YesterdayClose;
                    _AllIndicesReferences[msg.IndexIdentifier].SequenceNumber = msg.Header.SequenceNumber;
                    _AllIndicesReferences[msg.IndexIdentifier].TimeStamp = DataAdapter.ParseTime(msg.TimeOfIndexValueAsString);
                }
                if (IndexUpdatedEvent != null)
                    IndexUpdatedEvent(_AllIndicesReferences[msg.IndexIdentifier]);
            }
        }

        private void ProcessOrderbookDto(OrderbookDto msg)
        {
            List<OrderbookItem> bidOrders = ProcessInitialBids(msg, _IsRestricted);
            IEnumerable<OrderbookItem> offerOrders = ProcessInitialOffers(msg, _IsRestricted);

            List<OrderbookItem> items = bidOrders;
            items.AddRange(offerOrders);

            if (OrderbookInitializedEvent != null)
                OrderbookInitializedEvent(items);
        }

        private IEnumerable<OrderbookItem> ProcessInitialOffers(OrderbookDto msg, bool isRestricted)
        {
            List<OrderbookItem> items = new List<OrderbookItem>();
            var offers = msg.Offers;

            if (isRestricted)
                offers = msg.Offers.Take(5).ToList();

            foreach (Fin24.LiveData.Common.DTOs.DepthItemDto depthItemDto in offers)
            {
                foreach (Fin24.LiveData.Common.DTOs.OrderDto orderDto in depthItemDto.Orders)
                {
                    try
                    {
                        string symbol = GetSymbolFromISIN(orderDto.TradableInstrumentCode);
                        OrderbookItem order = DataAdapter.GetOrderFromDto(orderDto, symbol);
                        items.Add(order);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(string.Format("Initial Offers Error :{0}", exception));
                    }
                }
            }
            return items;
        }

        private List<OrderbookItem> ProcessInitialBids(OrderbookDto msg, bool isRestricted)
        {
            List<OrderbookItem> items = new List<OrderbookItem>();
            var bids = msg.Bids;

            if (isRestricted)
                bids = msg.Bids.Take(5).ToList();

            foreach (Fin24.LiveData.Common.DTOs.DepthItemDto depthItemDto in bids)
            {
                foreach (Fin24.LiveData.Common.DTOs.OrderDto orderDto in depthItemDto.Orders)
                {
                    try
                    {
                        string symbol = GetSymbolFromISIN(orderDto.TradableInstrumentCode);
                        OrderbookItem order = DataAdapter.GetOrderFromDto(orderDto, symbol);
                        items.Add(order);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(string.Format("Initial Bids Error :{0}", exception));
                    }
                }
            }
            return items;
        }

        private void ProcessHeartbeatMessage(HeartbeatMessage msg)
        {
            DateTime heartbeatTime = DateTime.MinValue;

            DateTime.TryParse(msg.HeartbeatDateTime, out heartbeatTime);

            if (HeartbeatReceivedEvent != null && heartbeatTime != DateTime.MinValue)
                HeartbeatReceivedEvent(heartbeatTime);
        }

        private void ProcessOrderDeletion(OrderDeletion msg)
        {
            string symbol = GetSymbolFromISIN(msg.Header.TradableInstrumentCode);

            if (OrderDeletedEvent != null)
                OrderDeletedEvent(msg.OrderCode);
        }

        private void ProcessOrderDetails(OrderDetails msg)
        {
            string symbol = GetSymbolFromISIN(msg.Header.TradableInstrumentCode);
            OrderbookItem order = DataAdapter.GetOrderFromDto(msg, symbol);

            if (OrderAddedEvent != null)
                OrderAddedEvent(order);
        }

        private void ProcessEnhancedBestPrice(EnhancedBestPrice msg)
        {
            if (_AllInstruments.ContainsKey(msg.TradableInstrumentCode))
            {
                long bestBidPrice = msg.BestBidPrice / 100000000;
                long bestOfferPrice = msg.BestOfferPrice / 100000000;

                _AllInstruments[msg.TradableInstrumentCode].BestBid = bestBidPrice;
                _AllInstruments[msg.TradableInstrumentCode].BestOffer = bestOfferPrice;
                _AllInstruments[msg.TradableInstrumentCode].BidVolume = msg.VolumeOfOrdersAtBestBidPrice;
                _AllInstruments[msg.TradableInstrumentCode].OfferVolume = msg.VolumeOfOrdersAtBestOfferPrice;

                //Quote quote
                Domain.ValueObjects.Quote quote = DataAdapter.GetQuoteFromDto(msg);
                quote.Symbol = _AllInstruments[msg.TradableInstrumentCode].Symbol;
                quote.Exchange = _AllInstruments[msg.TradableInstrumentCode].Exchange;

                if (!_quoteMappings.ContainsKey(msg.TradableInstrumentCode))
                {
                    _quoteMappings.Add(msg.TradableInstrumentCode, quote);
                }
                _quoteMappings[msg.TradableInstrumentCode] = quote;

                if (QuoteOccurredEvent != null)
                {
                    QuoteOccurredEvent(quote);
                }
                if (InstrumentUpdatedEvent != null)
                {
                    InstrumentUpdatedEvent(_AllInstruments[msg.TradableInstrumentCode]);
                }
            }
        }

        private void ProcessTradeReport(TradeReport msg)
        {
            if (msg.TradeTimeIndicator != "N") return;

            if (_AllInstruments.ContainsKey(msg.TradableInstrumentCode))
            {
                Domain.ValueObjects.Trade trade = DataAdapter.GetTradeFromDto(msg);
                if (_tradePair.ContainsKey(msg.TradableInstrumentCode))
                {
                    if (trade.SequenceNumber <= _tradePair[msg.TradableInstrumentCode])
                        return;
                }

                long tradePrice = msg.TradePrice / 100000000;
                Instrument instrument = _AllInstruments[msg.TradableInstrumentCode];

                if (instrument.Open == 0)
                    instrument.Open = tradePrice;

                if (instrument.YesterdayClose > 0)
                    instrument.CentsMoved = tradePrice - instrument.YesterdayClose;

                if (tradePrice > instrument.High)
                {
                    instrument.High = tradePrice;
                }
                instrument.LastTrade = tradePrice;
                instrument.LastTradeSequenceNumber = trade.SequenceNumber;

                if (instrument.Low == 0 || tradePrice < instrument.Low)
                {
                    instrument.Low = tradePrice;
                }
                if (instrument.YesterdayClose > 0)
                    instrument.PercentageMoved = (instrument.CentsMoved * 100) / instrument.YesterdayClose;

                instrument.TotalTrades += 1;
                instrument.TotalValue += (tradePrice * trade.Volume);
                instrument.TotalVolume += trade.Volume;

                trade.Symbol = instrument.Symbol;
                trade.Exchange = instrument.Exchange;

                trade.Quote = DataAdapter.GetQuoteFromDto(msg.Quote);
                if (trade.Quote == null)
                {
                    if (_quoteMappings.ContainsKey(msg.TradableInstrumentCode))
                    {
                        trade.Quote = _quoteMappings[msg.TradableInstrumentCode];
                    }
                }

                if (TradeOccurredEvent != null)
                {
                    TradeOccurredEvent(trade);
                }
                if (InstrumentUpdatedEvent != null)
                {
                    InstrumentUpdatedEvent(instrument);
                }
            }
        }

        private void ProcessUncrossingPriceAndVolume(UncrossingPriceAndVolume msg)
        {
            if (_AllInstruments.ContainsKey(msg.TradableInstrumentCode))
            {
                long tradePrice = msg.UncrossingPrice / 100000000;
                Instrument instrument = _AllInstruments[msg.TradableInstrumentCode];

                if (instrument.Open == 0)
                    instrument.Open = tradePrice;

                if (instrument.YesterdayClose > 0)
                    instrument.CentsMoved = tradePrice - instrument.YesterdayClose;

                if (tradePrice > instrument.High)
                {
                    instrument.High = tradePrice;
                }
                instrument.LastTrade = tradePrice;

                if (instrument.Low == 0 || tradePrice < instrument.Low)
                {
                    instrument.Low = tradePrice;
                }

                instrument.BestBid = msg.BestBidPrice / 100000000;
                instrument.BestOffer = msg.BestOfferPrice / 100000000;

                if (InstrumentUpdatedEvent != null)
                {
                    InstrumentUpdatedEvent(_AllInstruments[msg.TradableInstrumentCode]);
                }
            }
        }

        private void ProcessClosingPrice(ClosingPrice msg)
        {
            if (_AllInstruments.ContainsKey(msg.TradableInstrumentCode))
            {
                long tradePrice = msg.ClosingMidTradePrice / 100000000;
                Instrument instrument = _AllInstruments[msg.TradableInstrumentCode];

                if (instrument.Open == 0)
                    instrument.Open = tradePrice;

                if (instrument.YesterdayClose > 0)
                    instrument.CentsMoved = tradePrice - instrument.YesterdayClose;

                if (tradePrice > instrument.High)
                {
                    instrument.High = tradePrice;
                }
                instrument.LastTrade = tradePrice;

                if (instrument.Low == 0 || tradePrice < instrument.Low)
                {
                    instrument.Low = tradePrice;
                }
                instrument.BestBid = msg.ClosingBidPrice / 100000000;
                instrument.BestOffer = msg.ClosingOfferPrice / 100000000;

                if (InstrumentUpdatedEvent != null)
                {
                    InstrumentUpdatedEvent(_AllInstruments[msg.TradableInstrumentCode]);
                }
            }
        }

        private void ProcessCumulativeNumberAndVolumeOfTrades(CumulativeNumberAndVolumeOfTrades msg)
        {
            if (_AllInstruments.ContainsKey(msg.TradableInstrumentCode))
            {
                _AllInstruments[msg.TradableInstrumentCode].TotalVolume = msg.VolumeOfTrades;
                _AllInstruments[msg.TradableInstrumentCode].TotalTrades = msg.NumberOfTrades;

                if (InstrumentUpdatedEvent != null)
                {
                    InstrumentUpdatedEvent(_AllInstruments[msg.TradableInstrumentCode]);
                }
            }
        }

        public List<JsonLibCommon.FinSwitchHD> GetEODWithDates(string symbol, DateTime startdt, DateTime endDT, string PriceType)
        {
            //if (Jservers != null)
            List<JsonLibCommon.FinSwitchHD> lst = _FinSwitchService.GetEODWithDates(symbol,startdt,endDT, PriceType);
            return lst;
            //else
            //    return new List<JSONdataLib.FinSwitchHD>();

        }

        public List<string> GetFundTypes()
        {
            return _FinSwitchService.GetFundTypes();
        }

        public List<JsonLibCommon.Sector> GetSectors(string FType)
        {
            return _FinSwitchService.GetSectors(FType);
        }

        public List<JsonLibCommon.CISFunds> GetCISFunds(string Ftype, string SectorCode)
        {
            return _FinSwitchService.GetCISFunds(Ftype, SectorCode);
        }

        public List<FinSwitchFundsDetails> GetFinSwitchMFList()
        {
            return null;
        }
    }

}

