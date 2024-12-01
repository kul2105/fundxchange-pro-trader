using System;
using System.Collections.Generic;
using System.Linq;
using ClientDLL_Model.Cls;
using ClientDLL_Model.Cls.Contract;
using ClientDLL_Model.Cls.Market;
using ClientDLL_NET.Manager;
using ClientDLL_NET.Manager.Interfaces;
using System.Data;
//using Logging;

namespace PALSA.Cls
{
    public class ClsTWSDataManager : IDataManager, IMsgDataReceived<Dictionary<string, Quote>>
    {
        #region  "        MEMBERS         "

        private static ClsTWSDataManager _instance;
        private readonly List<string> _lstSubscribeDoms = new List<string>();
        private readonly List<string> _lstSubscribeSnap = new List<string>();
        public List<string> _lstSubscribeSymbols = new List<string>();
        //public  Dictionary<string, List<string>> dicSymbolFirstList;
        //public  Dictionary<string, List<string>> dicSymbolSecondList;
        public Dictionary<string, List<ClientDLL_Model.Cls.Market.OHLC>> DDSymbolsOHLC = new Dictionary<string, List<ClientDLL_Model.Cls.Market.OHLC>>();
        private readonly DataManager objDataManager = new DataManager();
        public Dictionary<string, string> _DDMessages = new Dictionary<string, string>();

        public ClsQueue<Dictionary<string, Quote>> ClsQueueHandler;


        #endregion

        #region "        EVENTS          "

        public event Action<Dictionary<string, Quote>> onPriceUpdate = delegate { };
        public Action<string, string> OnLogonResponse = delegate { };
        public event Action<Dictionary<string, QuoteSnapshot>> onSnapShotUpdate = delegate { };
        public event Action<List<News>> OnNews = delegate { };
        public event Action<string> OnNewQuoteRequest = delegate { };
        public event Action<string> OnDataServerConnectionEvnt = delegate { };
        public event Action<string> OnSubscribeReuqest = delegate { };
        public event Action<string, double> OnLTPChange = delegate { };

        #endregion "   EVENTS       "

        #region "      CONSTRUCTOR       "

        private ClsTWSDataManager()
        {
            ClsQueueHandler = new ClsQueue<Dictionary<string, Quote>>(this);
        }

        #endregion

        #region "      PROPERTIES        "

        public bool IsDataMgrConnected { get; private set; }

        public static ClsTWSDataManager INSTANCE
        {
            get { return _instance ?? (_instance = new ClsTWSDataManager()); }
        }

        #endregion "     PROPERTIES        "

        #region "        METHODS         "

        public void SubscribeForDom(bool isForSubscribe, eMarketRequest marketRequest,
                                    List<Symbol> lstSymbol)
        {

            //foreach (ClientDLL_Model.Cls.Contract.Symbol symbol in lstSymbol)
            //{
            //    if (symbol!=null && !_lstSubscribeDoms.Contains(symbol.KEY))
            //    {
            //        lstTemp.Add(symbol);
            //        _lstSubscribeDoms.Add(symbol.KEY);
            //    }
            //}
            //if (lstTemp.Count > 0)
            //SubscribeMarket(isForSubscribe, marketRequest, lstTemp);

            if (lstSymbol.Count > 0)
                SubscribeMarket(isForSubscribe, marketRequest, lstSymbol);
        }


        public void SubscribeForQuotes(bool isForSubscribe, eMarketRequest marketRequest,
                                       List<Symbol> lstSymbol)
        {
            try
            {
                var lstTemp = new List<Symbol>();
                foreach (Symbol symbol in lstSymbol.Where(symbol => symbol != null && !_lstSubscribeSymbols.Contains(symbol.KEY)))
                {
                    lstTemp.Add(symbol);
                    _lstSubscribeSymbols.Add(symbol.KEY);

                }
                if (lstTemp.Count > 0)
                {
                    SubscribeMarket(isForSubscribe, marketRequest, lstTemp);
                    SubscribeMarket(isForSubscribe, eMarketRequest.MARKET_QUOTE_SNAP, lstTemp);
                }
                //LoadDictinary();
            }
            catch
            {

            }
        }

        //private void LoadDictinary()
        //{           
        //    dicSymbolFirstList = new Dictionary<string, List<string>>();           
        //    dicSymbolSecondList = new Dictionary<string, List<string>>();
        //    foreach (var symbol in _lstSubscribeSymbols)
        //    {
        //        // now add item accordingly to dicSymbolFirstList or dicSymbolSecondList
        //        string Symbol = symbol.Insert(3, "/");
        //        string firstCur = Symbol.Split('/')[0];
        //        string SecondCur = Symbol.Split('/')[1];
        //        lock (dicSymbolFirstList)
        //        {
        //            if (dicSymbolFirstList.ContainsKey(firstCur) == true) //Key is present
        //            {
        //                List<string> lstSymbol = dicSymbolFirstList[firstCur];
        //                if (lstSymbol.Contains(SecondCur) == false)
        //                {
        //                    lstSymbol.Add(SecondCur);
        //                    dicSymbolFirstList[firstCur] = lstSymbol;
        //                }
        //            }
        //            else
        //            {
        //                List<string> lstSymbol = new List<string>();
        //                lstSymbol.Add(SecondCur);
        //                dicSymbolFirstList.Add(firstCur, lstSymbol);
        //            }
        //        }
        //        lock (dicSymbolSecondList)
        //        {
        //            if (dicSymbolSecondList.ContainsKey(SecondCur) == true) //Key is present
        //            {
        //                List<string> lstSymbol = dicSymbolSecondList[SecondCur];
        //                if (lstSymbol.Contains(firstCur) == false)
        //                {
        //                    lstSymbol.Add(firstCur);
        //                    dicSymbolSecondList[SecondCur] = lstSymbol;
        //                }
        //            }
        //            else
        //            {
        //                List<string> lstSymbol = new List<string>();
        //                lstSymbol.Add(firstCur);
        //                dicSymbolSecondList.Add(SecondCur, lstSymbol);
        //            }

        //        }
        //    }
        //}

        //public double GetConvertionFactor(string ConversionPair, string targetCurrency)
        //{          
        //    double res = 0;
        //    int a = 0;
        //    //Calculations
        //    string CurrencyPair = ConversionPair.Insert(3, "/");
        //    string firstCur = CurrencyPair.Split('/')[0].Trim();
        //    string SecondCur = CurrencyPair.Split('/')[1].Trim();
        //    double price = (ClsGlobal.GetZeroLevelAskPrice(ConversionPair) + ClsGlobal.GetZeroLevelBidPrice(ConversionPair)) / 2;
        //    if (firstCur == targetCurrency) // if tradingCurrency is in first part
        //    {
        //        //Reverse pair       
        //        a = 1;                
        //        res = 1/price;

        //    }
        //    else if (SecondCur == targetCurrency) // if tradingCurrency is in second part
        //    {
        //        //Direct pair  
        //        a = 2;
        //        res = 1;

        //    }
        //    else
        //    {
        //        double result = price;
        //        if (firstCur == "USD" && a == 0)
        //        {
        //            a = 3;
        //            res = 1 / result;
        //        }
        //        if (SecondCur == "USD" && a == 0)
        //        {
        //            a = 4;
        //            res = 1;
        //        }
        //        if (dicSymbolSecondList["USD"].Contains(SecondCur) == true && a == 0)
        //        {
        //            a = 5;
        //            res = result * ((ClsGlobal.GetZeroLevelAskPrice(SecondCur + "USD") + ClsGlobal.GetZeroLevelBidPrice(SecondCur + "USD")) / 2);
        //        }
        //        if (dicSymbolFirstList["USD"].Contains(firstCur) == true && a == 0)
        //        {
        //            a = 6;
        //            res = 1/(result * ((ClsGlobal.GetZeroLevelAskPrice("USD" + firstCur) + ClsGlobal.GetZeroLevelBidPrice("USD" + firstCur)) / 2));
        //        }
        //        if (dicSymbolFirstList["USD"].Contains(SecondCur) == true && a == 0)
        //        {
        //            a = 7;
        //            res = result  * (1 / ((ClsGlobal.GetZeroLevelAskPrice("USD" + SecondCur) + ClsGlobal.GetZeroLevelBidPrice("USD" + SecondCur)) / 2)); 
        //        }
        //        if (dicSymbolSecondList["USD"].Contains(firstCur) == true && a == 0)
        //        {
        //            a = 8;
        //            res = 1/ (  result  * ( 1 / ((ClsGlobal.GetZeroLevelAskPrice(firstCur + "USD") + ClsGlobal.GetZeroLevelBidPrice(firstCur + "USD")) / 2)));
        //        }

        //        if (a == 0)
        //        {
        //            res = 0;
        //        }
        //        else
        //        {
        //            if (targetCurrency == "USD")
        //            {

        //            }
        //            else
        //            {
        //                double Result = res;
        //                if (dicSymbolFirstList["USD"].Contains(targetCurrency) == true)
        //                {
        //                    res = Result * ((ClsGlobal.GetZeroLevelAskPrice("USD" + targetCurrency) + ClsGlobal.GetZeroLevelBidPrice("USD" + targetCurrency)) / 2); 
        //                }
        //                else if (dicSymbolSecondList["USD"].Contains(targetCurrency) == true)
        //                {
        //                    res = Result  * ( 1/  ((ClsGlobal.GetZeroLevelAskPrice(targetCurrency + "USD") + ClsGlobal.GetZeroLevelBidPrice(targetCurrency + "USD")) / 2));  
        //                }
        //                else
        //                {
        //                    res = 0;
        //                }

        //            }
        //        }
        //    }
        //    return res;
        //}

        public void SubscribeForQuoteSnapShot(bool isForSubscribe, eMarketRequest marketRequest,
                                              List<Symbol> lstSymbol)
        {
            var lstTemp = new List<Symbol>();
            foreach (
                Symbol symbol in lstSymbol.Where(symbol => symbol != null && !_lstSubscribeSnap.Contains(symbol.KEY)))
            {
                lstTemp.Add(symbol);
                _lstSubscribeSnap.Add(symbol.KEY);
            }

            //SubscribeMarket(isForSubscribe, marketRequest, lstTemp);

            //if(lstTemp.Count>0)
            //    SubscribeMarket(isForSubscribe, marketRequest, lstTemp);
            if (lstTemp.Count > 0)
                SubscribeMarket(isForSubscribe, marketRequest, lstTemp);
        }

        internal void SubscribeMarket(bool isForSubscribe, eMarketRequest marketRequest,
                                      List<Symbol> lstSymbol)
        {
            try
            {
                if (IsDataMgrConnected)
                {
                    try
                    {
                        objDataManager.SubscribeMarket(isForSubscribe, marketRequest, lstSymbol);
                    }
                    catch { }
                    List<Symbol> syms = lstSymbol;
                    foreach (Symbol item in syms)
                    {
                        OnSubscribeReuqest("Quote Server : " + isForSubscribe.ToString() + " Send" +
                                           marketRequest.ToString() + " Request of Contract " + item._ContractName);
                        //FileHandling.WriteInOutLog("Quote Server : " + isForSubscribe.ToString() + " Send" + marketRequest.ToString() + " Request of Contract " + item._ContractName);
                        //FileHandling.WriteAllLog("Quote Server : " + isForSubscribe.ToString() + " Send" + marketRequest.ToString() + " Request of Contract " + item._ContractName);
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message + " : " + ex.InnerException);
            }
        }

        public void Init(string username, string pwd, string serverIp, string hostIp, int port)
        {
            //objDataManager.UnregisterForEvents(this);//Namo
            objDataManager.RegisterForEvents(this);
            objDataManager.Start();
            string senderId = clsResourceMGR.INSTANCE.resourceManager.GetString("SenderID");
            //string senderId = "55";
            objDataManager.Login(username, pwd, serverIp, hostIp, port, senderId);

            //FileHandling.WriteInOutLog("Quote Server : Send Connection Request");
            //FileHandling.WriteAllLog("Quote Server : Send Connection Request");
        }

        public void Close()
        {
            if (objDataManager != null)
                objDataManager.Dispose();

            //FileHandling.WriteInOutLog("Quote Server : Send DisConnection Request");
            //FileHandling.WriteAllLog("Quote Server : Send DisConnection Request");
        }

        #endregion "         METHODS          "

        #region IDataManager Members

        public void onManagerStatus(eCLIENTDLL status)
        {
            //FileHandling.WriteInOutLog("Quote Server : ConnectionStatus " + status.ToString());
            //FileHandling.WriteAllLog("Quote Server : ConnectionStatus " + status.ToString());

            switch (status)
            {
                case eCLIENTDLL.AgentConnected:
                    {

                        IsDataMgrConnected = true;
                        if (!_DDMessages.ContainsKey("Connected"))
                        {
                            _DDMessages["Connected"] = "Data Server";
                        }
                        if (OnDataServerConnectionEvnt != null)
                        {
                            OnDataServerConnectionEvnt("Connected");

                        }
                    }
                    break;
                case eCLIENTDLL.AgentDisconnected:
                    {
                        if (!_DDMessages.ContainsKey("DisConnected"))
                        {
                            _DDMessages["DisConnected"] = "Data Server";
                        }

                        if (OnDataServerConnectionEvnt != null)
                        {
                            OnDataServerConnectionEvnt("DisConnected");
                        }
                    }
                    break;
            }
            //try
            //{
            //}
            //catch (Exception ex)
            //{
            //    //throw new Exception(ex.Message + " : " + ex.InnerException);
            //}
        }

        public void OnBars(HistoricalData lstOHLC)
        {
            ////FileHandling.WriteInOutLog("Quote Server : Received Bars of Key" + lstOHLC._SymbolKey);
            ////FileHandling.WriteAllLog("Quote Server : Received Bars of Key" + lstOHLC._SymbolKey);
        }

        public void OnBarUpdate(string contract, string productType, string product, DateTime OHLCTime,
                                DateTime nextOHLCTime, double open, double high, double low, double close, int volume)
        {
            ////FileHandling.WriteInOutLog("Quote Server : Received Bar Updates of ProductType" + productType +" Product" + product + "of Contract" + contract);
            ////FileHandling.WriteAllLog("Quote Server : Received Bar Updates of ProductType" + productType + " Product" + product + "of Contract" + contract);
        }

        public void OnNewsUpdate(List<News> lstNews)
        {
            //try
            //{
            foreach (News item in lstNews)
            {
                if (item != null)
                {
                    //FileHandling.WriteInOutLog("Quote Server : NEWS Received " + item._NewsTopic + " : " + item._BodyText);
                    //FileHandling.WriteAllLog("Quote Server : NEWS Received " + item._NewsTopic + " : " + item._BodyText);
                }
            }
            //if (Properties.Settings.Default.MessageBarMessageType.Contains("News"))
            //{
            //   if (uctlMessagLog1.ui_uctlGridMessageLog.Rows.Count >Convert.ToInt32(Properties.Settings.Default.MaxMessageInMessageBox))
            //        {
            //            uctlMessagLog1.ui_uctlGridMessageLog.RemoveAt(0);
            //        }
            if (System.Threading.Monitor.TryEnter(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog, 1000))
            {
                try
                {
                    foreach (News item in lstNews)
                    {
                        DataRow dr = ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.NewRow();
                        //DateTime 
                        string str = ClsTWSOrderManager.INSTANCE.GetDateTime(item._TimeStamp);
                        //if (Properties.Settings.Default.TimeFormat.Contains("24"))
                        //{
                        //    str = string.Format("{0:HH:mm:ss tt dd/MM/yyyy}", dt);
                        //}
                        //else
                        //{
                        //    str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                        //}
                        dr["Time"] = ClsTWSOrderManager.INSTANCE.GetDateTime(item._TimeStamp);//dt;
                        dr["MessageType"] = "NEWS";
                        dr["Message"] = "NEWS :-" + item._NewsTopic + " , " + item._BodyText; ;
                        dr["Account"] = 0;
                        dr["StrDateTime"] = str;
                        dr["Color"] = "White";
                        ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                    }
                    ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.AcceptChanges();
                }
                finally
                {
                    System.Threading.Monitor.Exit(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog);
                }
            }
            OnNews(lstNews);
            //}
            //catch (Exception ex)
            //{
            //    //throw new Exception(ex.Message + " : " + ex.InnerException);
            //}
        }
        public string GetDateTime(string datetime)
        {
            if (datetime != string.Empty)
            {
                string[] x = datetime.Split('-');
                string dat = x[1].ToString() + "/" + x[2].ToString() + "/" + x[0].ToString() + " " + x[3].ToString() + ":" + x[4].ToString() + ":" + x[5].ToString();
                DateTime dtx = DateTime.MinValue;
                DateTime.TryParse(dat, out dtx);
                string date = string.Empty;
                if (dtx != null && dtx != DateTime.MinValue)
                {
                    date = string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", dtx);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:dd/MM/yyyy HH:mm:ss}" : "{0:dd/MM/yyyy hh:mm:ss tt}", dtx);
                }
                return date;
            }
            else
                return datetime;
        }
        public void OnPriceUpdate(string UserName, Dictionary<string, Quote> DDQuote)
        {
            int x = onPriceUpdate.GetInvocationList().Count();
            if (x > 0)
            {
                foreach (var quotes in DDQuote)
                {
                    foreach (QuoteItem item2 in quotes.Value._lstItem)
                    {
                        if (item2._MarketLevel >= 1)
                            continue;
                        Symbol sym = Symbol.GetSymbol(quotes.Key);
                        //To Test
                        switch (item2._quoteType)
                        {
                            case QuoteStreamType.ASK:
                                {
                                    if (ClsGlobal.DDRightZLevel.Keys.Contains(sym._ContractName))
                                    {
                                        ClsGlobal.DDRightZLevel[sym._ContractName] = item2._Price;
                                    }
                                    else
                                    {
                                        ClsGlobal.DDRightZLevel.Add(sym._ContractName, item2._Price);
                                    }
                                }
                                OnLTPChange(sym._ContractName, item2._Price);
                                break;
                            case QuoteStreamType.BID:
                                {
                                    if (ClsGlobal.DDLeftZLevel.Keys.Contains(sym._ContractName))
                                    {
                                        ClsGlobal.DDLeftZLevel[sym._ContractName] = item2._Price;
                                    }
                                    else
                                    {
                                        ClsGlobal.DDLeftZLevel.Add(sym._ContractName, item2._Price);
                                    }
                                }
                                break;
                            case QuoteStreamType.LAST:
                                {
                                    if (ClsGlobal.DDLTP.Keys.Contains(sym._ContractName))
                                    {
                                        ClsGlobal.DDLTP[sym._ContractName] = item2._Price;
                                    }
                                    else
                                    {
                                        ClsGlobal.DDLTP.Add(sym._ContractName, item2._Price);
                                    }
                                }
                                break;
                            default:
                                break;
                        }


                    }

                    ////FileHandling.WriteInOutLog("Quote Server : Quotes Received For > " + quotes.Key);
                    ////FileHandling.WriteAllLog("Quote Server : Quotes Received For > " + quotes.Key);
                }
                try
                {

                    ClsQueueHandler.EnqueueInReceiveQueue(DDQuote);

                }
                catch
                {

                }
            }
            else
            {
                objDataManager.UnregisterForEvents(this);
            }
        }
        int count = 0;
        public void OnSnapshotUpdate(string UserName, Dictionary<string, QuoteSnapshot> lstSnapShot)
        {
            count += lstSnapShot.Count;
            foreach (var item in lstSnapShot)
            {
                Symbol sym = Symbol.GetSymbol(item.Key);
                bool contains = ClsGlobal.DDLTP.Keys.Contains(sym._ContractName);
                if (contains)
                {
                    ClsGlobal.DDLTP[sym._ContractName] = item.Value._LastPrice;
                }
                else
                {
                    ClsGlobal.DDLTP.Add(sym._ContractName, item.Value._LastPrice);
                }

                //for(int i=MarketDepthItem item1 in item.Value._lstMarketDepthItem)
                //{
                if (item.Value._lstMarketDepthItem.Count > 0)
                {
                    MarketDepthItem item1 = item.Value._lstMarketDepthItem[0];
                    //if (item1._Level >= 1)
                    //    continue;
                    if (ClsGlobal.DDLeftZLevel.Keys.Contains(sym._ContractName))
                        ClsGlobal.DDLeftZLevel[sym._ContractName] = item1._BidPrice;
                    else
                        ClsGlobal.DDLeftZLevel.Add(sym._ContractName, item1._BidPrice);
                    if (ClsGlobal.DDLeftZLevelQty.Keys.Contains(sym._ContractName))
                        ClsGlobal.DDLeftZLevelQty[sym._ContractName] = (int)item1._BidSize;
                    else
                        ClsGlobal.DDLeftZLevelQty.Add(sym._ContractName, (int)item1._BidSize);

                    if (ClsGlobal.DDRightZLevelQty.Keys.Contains(sym._ContractName))
                        ClsGlobal.DDRightZLevelQty[sym._ContractName] = (int)item1._AskSize;
                    else
                        ClsGlobal.DDRightZLevelQty.Add(sym._ContractName, (int)item1._AskSize);

                    if (ClsGlobal.DDRightZLevel.Keys.Contains(sym._ContractName))
                        ClsGlobal.DDRightZLevel[sym._ContractName] = item1._AskPrice;
                    else
                        ClsGlobal.DDRightZLevel.Add(sym._ContractName, item1._AskPrice);

                    OnLTPChange(sym._ContractName, item1._AskPrice);
                }
                //}
                ////FileHandling.WriteInOutLog("Quote Server : QuoteSnapShot Received For > " + item.Key + " Values Open > " +
                //                           item.Value._Open +
                //                           " High > " + item.Value._High + " Low > " + item.Value._Low + "Close" +
                //                           item.Value._Close + " Time > " + item.Value._LastUpdatedTime);
                ////FileHandling.WriteAllLog("Quote Server : QuoteSnapShot Received " + item.Key + " Values Open > " +
                //                         item.Value._Open +
                //                         " High > " + item.Value._High + " Low > " + item.Value._Low + " Close > " +
                //                         item.Value._Close + " Time > " + item.Value._LastUpdatedTime);
            }
            onSnapShotUpdate(lstSnapShot);

            if (count == _lstSubscribeSnap.Count)
            {
                objDataManager.UnregisterForEvents(this);
                count = 0;
            }
        }

        public void onError(string error)
        {
            //try
            //{
            //FileHandling.WriteInOutLog("Quote Server : Server Error " + error);
            //FileHandling.WriteAllLog("Quote Server : Server Error " + error);
            //}
            //catch (Exception ex)
            //{
            //    //throw new Exception(ex.Message + " : " + ex.InnerException);
            //}
        }

        public void onErrorInSending(string error)
        {
            //try
            //{
            //FileHandling.WriteInOutLog("Quote Server : Sending Error " + error);
            //FileHandling.WriteAllLog("Quote Server : Sending Error " + error);
            //}
            //catch (Exception ex)
            //{
            //    //throw new Exception(ex.Message + " : " + ex.InnerException);
            //}
        }

        #endregion

        //for other LTP
        //    for forex +leftgrid -rightgrid
        //unrealized profit=netqty*marketprice
        //Dictionary<string,string> DD

        #region IMsgDataReceived<Dictionary<string,Quote>> Members

        public void OnDataReceived(Dictionary<string, Quote> DDQuote)
        {
            onPriceUpdate(DDQuote);
        }

        #endregion

        public void onLogonResponse(string UserName, string reason, string brokerName, string AccountType, bool IsLive)
        {
            if (reason == "VALID")
            {
                if (IsDataMgrConnected)
                {
                    if (_lstSubscribeSymbols.Count > 0)
                    {
                        var lstTemp = new List<Symbol>();
                        foreach (string x in _lstSubscribeSymbols)
                        {
                            Symbol symbol = Symbol.GetSymbol(x);
                            lstTemp.Add(symbol);
                        }
                        SubscribeMarket(true, eMarketRequest.MARKET_QUOTE_SNAP, lstTemp);
                        SubscribeMarket(true, eMarketRequest.MARKET_QUOTE_REQUEST, lstTemp);
                        SubscribeMarket(true, eMarketRequest.MARKET_DEPTH, lstTemp);
                    }
                    if (!_DDMessages.ContainsKey("Connected"))
                    {
                        _DDMessages["Connected"] = "Data Server";
                    }
                    if (OnDataServerConnectionEvnt != null)
                    {
                        OnDataServerConnectionEvnt("Connected");
                    }
                    //List<Symbol> symbolForDom = _lstSubscribeDoms.Select(x => Symbol.GetSymbol(x)).ToList();
                    //List<Symbol> symbolForQuote = _lstSubscribeSymbols.Select(x => Symbol.GetSymbol(x)).ToList();
                    //List<Symbol> symbolForSnap = _lstSubscribeSnap.Select(x => Symbol.GetSymbol(x)).ToList();
                    //if (symbolForDom.Count > 0)

                    //if (symbolForQuote.Count > 0)
                    //    SubscribeMarket(true, eMarketRequest.MARKET_QUOTE_REQUEST, symbolForQuote);
                    //if (symbolForSnap.Count > 0)
                    //    SubscribeMarket(true, eMarketRequest.MARKET_QUOTE_SNAP, symbolForSnap);
                }
            }
            else
            {
                if (OnDataServerConnectionEvnt != null)
                {
                    //OnDataServerConnectionEvnt("DisConnected");
                    OnDataServerConnectionEvnt(reason);
                }
            }
            if (System.Threading.Monitor.TryEnter(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog, 1000))
            {
                try
                {

                    DataRow dr = ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.NewRow();
                    DateTime dt = DateTime.UtcNow;
                    string str;
                    //if (Properties.Settings.Default.TimeFormat.Contains("24"))//BOChanges
                    //{
                        str = string.Format("{0:HH:mm:ss tt dd/MM/yyyy}", dt);
                    //}
                    //else
                    //{
                    //    str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                    //}
                    dr["Time"] = dt;
                    dr["MessageType"] = "DATA SERVER";
                    if (IsDataMgrConnected)
                    {
                        dr["Message"] = "Data Server Connected.";
                    }
                    else
                    {
                        dr["Message"] = "Data Server Disconnected.";
                    }
                    dr["Account"] = 0;
                    dr["StrDateTime"] = str;
                    dr["Color"] = "White";
                    ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                    ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.AcceptChanges();
                }
                finally
                {
                    System.Threading.Monitor.Exit(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog);
                }
            }
            OnLogonResponse(reason, brokerName);

            //Properties.Settings.Default.UserName = "ECX-TRADER (Powered by Intellitrade Technologies) , " + brokerName;
        }

        public void Refresh()
        {
            _lstSubscribeDoms.Clear();
            _lstSubscribeSnap.Clear();
            _lstSubscribeSymbols.Clear();
            _DDMessages.Clear();
        }


        public void onClientError(int error)
        {
            //throw new NotImplementedException();
        }

        public List<ClientDLL_Model.Cls.Market.OHLC> GetOHLC(string enddate, string symbol, int interval, string noOfRecord, int periodicity)
        {
            List<ClientDLL_Model.Cls.Market.OHLC> lst = new List<ClientDLL_Model.Cls.Market.OHLC>();
            ClientDLL_Model.Cls.Market.LstOHLC lstOhlc = new LstOHLC();
            try
            {
                lst = lstOhlc.GetOHLC(enddate, symbol, interval, noOfRecord, periodicity);
            }
            catch
            { }
            if (lst != null && lst.Count > 0)
            {
                if (!DDSymbolsOHLC.ContainsKey(symbol + "_" + Convert.ToString(periodicity)))
                    DDSymbolsOHLC.Add(symbol + "_" + Convert.ToString(periodicity), lst);
                else
                    DDSymbolsOHLC[symbol + "_" + Convert.ToString(periodicity)] = lst;
            }
            return lst;
        }


        #region IDataManager Members


        //public void OnPriceUpdate(string UserName, Dictionary<string, Quote> DDQuote)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnSnapshotUpdate(string UserName, Dictionary<string, QuoteSnapshot> lstSnapShot)
        //{
        //    throw new NotImplementedException();
        //}

        //public void onLogonResponse(string UserName, string Reason, string BrokerName, string AccountType, bool IsLive)
        //{
        //    throw new NotImplementedException();
        //}

        public void onLogoutResponse(string UserName)
        {
            
        }

        #endregion
    }
}