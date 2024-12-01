using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClientDLL_Model.Cls;
using ClientDLL_Model.Cls.Order;
using ClientDLL_NET.Manager;
using ClientDLL_NET.Manager.Interfaces;
//using Logging;
using PALSA.DS;
using ClientDLL_Model.Cls.Contract;
//using CommonLibrary.Cls;
using System.Threading;

namespace PALSA.Cls
{
    public class ClsTWSOrderManager : IOrderManager
    {
        #region "        MEMBERS          "

        public string StatusMessage { get; set; }
        public readonly Dictionary<string, AccountInfo> _DDAccountInfo = new Dictionary<string, AccountInfo>();
        public Dictionary<int, DataRow> _DDAccounts = new Dictionary<int, DataRow>();
        public Dictionary<string, string> _DDMessages = new Dictionary<string, string>();
        public List<KeyValuePair<string, DataRow>> _DDNetPos = new List<KeyValuePair<string, DataRow>>();
        public Dictionary<string, DataRow> _DDNetPosRow = new Dictionary<string, DataRow>();
        public Dictionary<long, DataRow> _DDOrderRow = new Dictionary<long, DataRow>();
        public Dictionary<long, DataRow> _DDTradeRow = new Dictionary<long, DataRow>();
        public bool IsDemo = false;

        public double Lotsize = 0;
        public List<string> PosContracts = new List<string>();
        public DS4MessageLog messageLogDS = new DS4MessageLog();
        public DS4AccountInfo accountInfoDS = new DS4AccountInfo();
        public DS4NetPosition netpositionDS = new DS4NetPosition();
        public DS4OrderHistory orderHistoryDS = new DS4OrderHistory();
        public DS4TradeHistory tradeHistoryDS = new DS4TradeHistory();
        public Dictionary<long, DataRow> _DDPendingOrderRow = new Dictionary<long, DataRow>();
        public Dictionary<long, DataRow> _DDPositionOpenRow = new Dictionary<long, DataRow>();
        public Dictionary<long, DataRow> _DDPositionCloseRow = new Dictionary<long, DataRow>();
        public DS4PendingOrders pendingOrdersDS = new DS4PendingOrders();
        public DS4PositionClose positionCloseDS = new DS4PositionClose();
        public DS4PositionOpen positionOpenDS = new DS4PositionOpen();

        private static ClsTWSOrderManager _instance;
        //private static List<string> _productTypes = new List<string>();
        private readonly OrderManager _objOrderManager = new OrderManager();
        private bool _flagFirstParticipantListResponce;
        Dictionary<int, string> _DDLeverage = new Dictionary<int, string>();
        Dictionary<int, string> _DDCountry = new Dictionary<int, string>();
        MailBoxData obj = new MailBoxData();
        //private List<OrderHistory> _lstOrderHistory = new List<OrderHistory>();
        //private List<Position> _lstPositions = new List<Position>();
        //private List<ExecutionReport> _lstTradeHistory = new List<ExecutionReport>();

        #endregion "      MEMBERS          "

        private ClsTWSOrderManager()
        {
            orderHistoryDS = new DS4OrderHistory();
            tradeHistoryDS = new DS4TradeHistory();
            _objOrderManager = new OrderManager();
            _DDOrderRow = new Dictionary<long, DataRow>();
            netpositionDS = new DS4NetPosition();
            _DDNetPosRow = new Dictionary<string, DataRow>();
            _DDMessages = new Dictionary<string, string>();
        }

        #region "       PROPERTIES         "

        public static ClsTWSOrderManager INSTANCE
        {
            get { return _instance ?? (_instance = new ClsTWSOrderManager()); }
        }

        public bool IsOrderMgrLoaded { get; private set; }

        #endregion "     PROPERTIES        "

        #region "         EVENTS           "
        public event Action<string, string> OnBalanceMarginUpdate = delegate { };
        public event Action<string, string, bool> OnChangePasswordResponse = delegate { };
        public event Action<string, string, string> OnLogonResponse = delegate { };
        public event Action<ExecutionReport> OnOrderResponse;
        public event Action<ExecutionReport> DoHandleExecutionReport = delegate { };
        public event Action<List<ExecutionReport>> DoHandleTradeHistoryResponse = delegate { };
        public event Action<ExecutionReport> OnOrderPendingNew = delegate { };
        public event Action<string> OnOrderServerConnectionEvnt = delegate { };
        public event Action<string> OnTradeLog;
        public event Action<string> OnOrderLog = delegate { };
        public event Action<DataRow, BusinessReject> OnBusinessLevelReject = delegate { };
        public event Action<List<AccountInfo>> OnAccountResponse = delegate { };
        public event Action<string> OnBothServerConnectionEvnt = delegate { };
        public event Action<string> SendMessage = delegate { };
        //public event Action<Dictionary<string, AccountInfo>> OnParticipantResponse =
        //    delegate { };
        public event Action<Dictionary<int, DataRow>> OnParticipantResponse = delegate { };
        public event Action<List<Position>> OnPositionResponse = delegate { };
        public event Action OnOrderHistoryResponse = delegate { };
        public event Action onPendingOrder = delegate { };
        public event Action onPositionClosed = delegate { };
        public event Action onPositionOpened = delegate { };
        //bool flag = false;
        #endregion

        #region IOrderManager Members

        public void onManagerStatus(eCLIENTDLL status)
        {
            //FileHandling.WriteInOutLog("Order Server : ConnectionStatus " + status.ToString());
            //FileHandling.WriteAllLog("Order Server : ConnectionStatus " + status.ToString());
            switch (status)
            {
                case eCLIENTDLL.AgentBusy:
                    break;
                case eCLIENTDLL.AgentConnected:
                    {
                        IsOrderMgrLoaded = true;
                        if (!_DDMessages.ContainsKey("Connected"))
                        {
                            _DDMessages["Connected"] = "Order Server";
                        }
                        if (OnOrderServerConnectionEvnt != null)
                        {
                            OnOrderServerConnectionEvnt("Connected");
                        }
                        //flag = false;
                    }
                    break;
                case eCLIENTDLL.AgentConnecting:
                    break;
                case eCLIENTDLL.AgentDisconnected:
                    {
                        //if (flag == false)
                        //{
                        IsOrderMgrLoaded = false;
                        if (!_DDMessages.ContainsKey("DisConnected"))
                        {
                            _DDMessages["DisConnected"] = "Order Server";
                        }
                        if (OnOrderServerConnectionEvnt != null)
                        {
                            OnOrderServerConnectionEvnt("DisConnected");
                        }
                        //flag = true;
                        //}

                    }
                    break;
                case eCLIENTDLL.AgentDisconnecting:
                    break;
                case eCLIENTDLL.AgentHealthBad:
                    break;
                case eCLIENTDLL.AgentHealthGood:
                    break;
                case eCLIENTDLL.AgentIdle:
                    break;
                case eCLIENTDLL.AgentLoaded:
                    IsOrderMgrLoaded = true;
                    break;
                case eCLIENTDLL.AgentLoading:
                    break;
                case eCLIENTDLL.ManagerBusy:
                    break;
                case eCLIENTDLL.ManagerHealthBad:
                    break;
                case eCLIENTDLL.ManagerHealthGood:
                    break;
                case eCLIENTDLL.ManagerIdle:
                    break;
                case eCLIENTDLL.ManagerLoaded:
                    break;
                case eCLIENTDLL.ManagerLoading:
                    break;
                default:
                    break;
            }
            LogOrderStatus("Order Server : ConnectionStatus " + status.ToString());
        }

        public void onError(string error)
        {
            //try
            //{
            //FileHandling.WriteInOutLog("Order Server : Server Error " + error);
            //FileHandling.WriteAllLog("Order Server : Server Error " + error);
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
            //FileHandling.WriteInOutLog("Order Server : Sending Error " + error);
            //FileHandling.WriteAllLog("Order Server : Sending Error " + error);
            //}
            //catch (Exception ex)
            //{
            //    //throw new Exception(ex.Message + " : " + ex.InnerException);
            //}
        }

        public void onAccountResponse(List<AccountInfo> lstAccountInfo, bool islastPck)
        {
            foreach (AccountInfo item in lstAccountInfo)
            {
                if (_DDAccountInfo.Keys.Contains(Convert.ToString(item.Account)))
                    _DDAccountInfo[Convert.ToString(item.Account)] = item;
                else
                    _DDAccountInfo.Add(Convert.ToString(item.Account), item);
            }

            if (islastPck)
            {

                OnAccountResponse(lstAccountInfo);
            }
        }

        public void onTradeHistoryResponse(List<ExecutionReport> lstExecutionReport, bool islastPck)
        {
            if (lstExecutionReport.Count > 0)
            {
                //if (System.Threading.Monitor.TryEnter(INSTANCE.orderHistoryDS.dtOrderHistory))
                //{
                try
                {
                    foreach (ExecutionReport item in lstExecutionReport)
                    {
                        //FileHandling.WriteInOutLog("Order Server : Trade History Respose Account" + item.Account + " Contract" + item.Contract);
                        //FileHandling.WriteAllLog("Order Server : Trade History Respose Account" + item.Account + " Contract" + item.Contract);
                        string orderStatus = string.Empty;
                        if (ClsGlobal.DDReverseOrderStatus.Keys.Contains(Convert.ToSByte(item.OrderStatus)))
                            orderStatus = ClsGlobal.DDReverseOrderStatus[Convert.ToSByte(item.OrderStatus)].ToUpper();

                        if (!orderStatus.ToUpper().Contains("FILLED"))
                            continue;
                        string productType = string.Empty;
                        string side = string.Empty;
                        string orderType = string.Empty;
                        string timeInForce = string.Empty;
                        if (ClsGlobal.DDReverseProductType.Keys.Contains(Convert.ToSByte(item.ProductType)))
                            productType = ClsGlobal.DDReverseProductType[Convert.ToSByte(item.ProductType)];
                        if (ClsGlobal.DDReverseSide.Keys.Contains(Convert.ToSByte(item.Side)))
                            side = ClsGlobal.DDReverseSide[Convert.ToSByte(item.Side)];
                        if (ClsGlobal.DDReverseOrderType.Keys.Contains(Convert.ToSByte(item.OrderType)))
                            orderType = ClsGlobal.DDReverseOrderType[Convert.ToSByte(item.OrderType)];
                        if (ClsGlobal.DDReverseValidity.Keys.Contains(Convert.ToSByte(item.TimeInForce)))
                            timeInForce = ClsGlobal.DDReverseValidity[Convert.ToSByte(item.TimeInForce)];
                        DataRow dr = tradeHistoryDS.dtTradeHistory.NewRow();
                        string transactTime = string.Empty;
                        //var expireTime = string.Empty;

                        if (item.TransactTime != string.Empty)
                            transactTime = GetDateTime(item.TransactTime);

                        string pType = string.Empty;
                        switch (productType)
                        {
                            case "EQ":
                                pType = "Equity";
                                break;
                            case "FUT":
                                pType = "FUTURE";
                                break;
                            case "FX":
                                pType = "FOREX";
                                break;
                            case "OPT":
                                pType = "OPTION";
                                break;
                            case "SP":
                                pType = "SPOT";
                                break;
                            case "PH":
                                pType = "PHYSICAL";
                                break;
                            case "AU":
                                pType = "AUCTION";
                                break;
                            case "BON":
                                pType = "BOND";
                                break;
                            default:
                                break;
                        }
                        ;
                        dr["TradeNo"] = item.ExecID;
                        dr["OrderNo"] = item.OrderID;
                        dr["ProductType"] = pType;
                        dr["Commission"] = Math.Round(item.Commission, 2);
                        dr["Tax"] = Math.Round(item.Tax, 2);
                        dr["ProductName"] = item.Product;
                        dr["Contract"] = item.Contract;
                        dr["BS"] = side;
                        dr["Quantity"] = item.OrdQty;
                        if (item.Price != 0M)
                            dr["Price"] = item.Price;
                        dr["AvgPrice"] = item.AvgPx;
                        dr["FillPrice"] = item.LastPx;
                        dr["OrderType"] = orderType;
                        dr["Status"] = orderStatus;
                        dr["TradeTime"] = transactTime;
                        dr["LastUpdatedTime"] = transactTime;
                        dr["LeaveQty"] = item.LeavesQty;
                        //dr["CounterClOrdId"] = item.CounterClOrdId;
                        //dr["CounterAvgPx"] = item.CounterAvgPx;
                        InstrumentSpec objInstrumentSpec = ClsTWSContractManager.INSTANCE.GetContractSpec(item.Contract, pType, item.Product);
                        double grosspl = 0;
                        //if (side == "BUY")
                        //{
                        //    grosspl = Math.Round(Convert.ToDouble(item.CounterAvgPx - item.AvgPx) * item.OrdQty * objInstrumentSpec.ContractSize, 2);
                        //}
                        //else
                        //{
                        //    grosspl = Math.Round(Convert.ToDouble(item.AvgPx - item.CounterAvgPx) * item.OrdQty * objInstrumentSpec.ContractSize, 2);
                        //}
                        dr["GrossPL"] = grosspl;
                        if (!_DDTradeRow.Keys.Contains(item.OrderID))
                        {
                            tradeHistoryDS.dtTradeHistory.Rows.Add(dr);
                            _DDTradeRow.Add(item.OrderID, dr);
                        }
                        //string message = string.Empty;
                        //Color color = Color.LightGray;
                        //message = "Your " + side + " " + orderType + " Order From Account >" + Convert.ToString(item.Account) + " with OrderID > " + item.OrderID + " for Symbol > " + item.Contract + " Qty > " + item.OrdQty + " Price > " + item.Price + " was " + orderStatus + " with Trade No. > " + item.ExecID + " on Date > " + ClsTWSOrderManager.INSTANCE.GetDateTime(item.TransactTime) + " .";
                        //DataRow dr1 = ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.NewRow();
                        //DateTime dt = DateTime.UtcNow;
                        //string str;
                        //if (Properties.Settings.Default.TimeFormat.Contains("24"))
                        //{
                        //    str = string.Format("{0:HH:mm:ss tt dd/MM/yyyy}", dt);
                        //}
                        //else
                        //{
                        //    str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                        //}
                        //dr1["Time"] = dt;
                        //dr1["MessageType"] = "TRADE HISTORY";
                        //dr1["Message"] =  message;
                        //dr1["Account"] = 0;
                        //dr1["StrDateTime"] = str;
                        //dr1["Color"] = color.Name;
                        //if (System.Threading.Monitor.TryEnter(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog, 1000))
                        //{
                        //    try
                        //    {
                        //        ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr1,0);
                        //        ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.AcceptChanges();
                        //    }
                        //    finally
                        //    {
                        //        System.Threading.Monitor.Exit(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog);
                        //    }
                        //}
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
                //}


                if (tradeHistoryDS != null && islastPck)
                {
                    string message = string.Empty;
                    Color color = Color.LightGray;
                    message = "Trade History for Account >" + Convert.ToString(lstExecutionReport[0].Account) + " Received.";
                    DataRow dr1 = INSTANCE.messageLogDS.dtMessageLog.NewRow();
                    DateTime dt = DateTime.UtcNow;
                    string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                    dr1["Time"] = dt;
                    dr1["MessageType"] = "TRADE HISTORY";
                    dr1["Message"] = message;
                    dr1["Account"] = 0;
                    dr1["StrDateTime"] = str;
                    dr1["Color"] = color.Name;
                    if (System.Threading.Monitor.TryEnter(INSTANCE.messageLogDS.dtMessageLog, 1000))
                    {
                        try
                        {
                            INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr1, 0);
                            INSTANCE.messageLogDS.dtMessageLog.AcceptChanges();
                        }
                        finally
                        {
                            System.Threading.Monitor.Exit(INSTANCE.messageLogDS.dtMessageLog);
                        }
                    }
                    OnOrderLog("TRADE");
                    tradeHistoryDS.dtTradeHistory.AcceptChanges();
                    DoHandleTradeHistoryResponse(lstExecutionReport);
                }
            }
        }
        //string pstrUserName, List<AccountInfo> lstAccountInfo, bool flag
        public void onParticipantResponse(string pstrUserName,List<AccountInfo> lstAccountInfo, bool islastPck)
        {

            if (lstAccountInfo.Count > 0)
            {
                foreach (AccountInfo accountInf in lstAccountInfo)
                {
                    lock (accountInfoDS.dtAccountInfo)
                    {
                        if (accountInfoDS.dtAccountInfo.Select("AccountId=" + Convert.ToString(accountInf.Account)).Count() == 0)
                        {
                            DataRow dr = accountInfoDS.dtAccountInfo.NewRow();
                            dr["AccountId"] = (int)accountInf.Account;
                            dr["Balance"] = accountInf.Balance;
                            dr["Group"] = accountInf.Group;
                            dr["TraderName"] = accountInf.TraderName;
                            dr["BuySideTurnover"] = accountInf.BuySideTurnOver;
                            dr["SellSideTurnover"] = accountInf.SellSideturnOver;
                            dr["HedgeAllowed"] = accountInf.HedgingType;
                            dr["Leverage"] = accountInf.Leverage;
                            dr["FreeMargin"] = accountInf.FreeMargin;
                            dr["Margin"] = accountInf.Margin;
                            dr["MarginCall1"] = accountInf.MarginCall1;
                            dr["MarginCall2"] = accountInf.MarginCall2;
                            dr["MarginCall3"] = accountInf.MarginCall3;
                            dr["UsedMargin"] = accountInf.UsedMargin;
                            dr["ReservedMargin"] = accountInf.ReservedMargin;
                            dr["AccountType"] = accountInf.AccountType;
                            dr["LotSize"] = accountInf.LotSize;

                            accountInfoDS.dtAccountInfo.Rows.Add(dr);
                            _DDAccounts.Add((int)accountInf.Account, dr);
                            if (_flagFirstParticipantListResponce == false)
                            {
                                //    _objOrderManager.getPositionResponse(
                                //        (uint)_DDAccounts.Keys.ToArray()[0]);                            
                                Lotsize = accountInf.LotSize;
                                _objOrderManager.getOrderHistoryResponse((uint)accountInf.Account);
                                //    _objOrderManager.getTradesResponse(
                                //        (uint)_DDAccounts.Keys.ToArray()[0]);
                                int x = _DDAccounts.Keys.First();
                                _flagFirstParticipantListResponce = true;
                            }
                        }
                        else
                        {
                            DataRow[] drArr = accountInfoDS.dtAccountInfo.Select("AccountId=" + Convert.ToString(accountInf.Account));
                            int i = accountInfoDS.dtAccountInfo.Rows.IndexOf(drArr[0]);
                            accountInfoDS.dtAccountInfo.Rows[i]["Balance"] = accountInf.Balance;
                            accountInfoDS.dtAccountInfo.Rows[i]["BuySideTurnover"] = accountInf.BuySideTurnOver;
                            accountInfoDS.dtAccountInfo.Rows[i]["SellSideTurnover"] = accountInf.SellSideturnOver;
                            accountInfoDS.dtAccountInfo.Rows[i]["HedgeAllowed"] = accountInf.HedgingType;
                            accountInfoDS.dtAccountInfo.Rows[i]["Leverage"] = accountInf.Leverage;
                            accountInfoDS.dtAccountInfo.Rows[i]["FreeMargin"] = accountInf.FreeMargin;
                            accountInfoDS.dtAccountInfo.Rows[i]["Margin"] = accountInf.Margin;
                            accountInfoDS.dtAccountInfo.Rows[i]["MarginCall1"] = accountInf.MarginCall1;
                            accountInfoDS.dtAccountInfo.Rows[i]["MarginCall2"] = accountInf.MarginCall2;
                            accountInfoDS.dtAccountInfo.Rows[i]["MarginCall3"] = accountInf.MarginCall3;
                            accountInfoDS.dtAccountInfo.Rows[i]["UsedMargin"] = accountInf.UsedMargin;
                            DataRow dr = accountInfoDS.dtAccountInfo.Rows[i];
                            _DDAccounts[(int)accountInf.Account] = dr;
                        }
                    }
                }

                int acc = 0;
                //BOChanges
                //if (Properties.Settings.Default.DefaultTradingAccount != string.Empty)
                //    acc = Convert.ToInt32(Properties.Settings.Default.DefaultTradingAccount);

                IEnumerable<AccountInfo> lstaccounts = lstAccountInfo.Where(accInf => acc != 0 && Convert.ToInt32(accInf.Account) == acc);

                //if (lstaccounts.Count() == 0)
                //    Properties.Settings.Default.DefaultTradingAccount = Convert.ToString(lstAccountInfo.ToArray()[0].Account);
                //BOChanges



                //foreach (AccountInfo accInf in lstAccountInfo)
                //{
                //    if (_DDAccountInfo.Keys.Contains(Convert.ToString(accInf.Account)))
                //    {
                //        _DDAccountInfo[Convert.ToString(accInf.Account)] = accInf;
                //    }
                //    else
                //    {
                //        _DDAccountInfo.Add(Convert.ToString(accInf.Account), accInf);        
                //    }
                //}

                if (islastPck)
                {
                    string message = string.Empty;
                    Color color = Color.White;
                    message = "Participant list for Account >" + lstAccountInfo[0].Account + " Received.";
                    //message = "Net Position for Account >" + positionInfo.Account + " for Symbol > " + positionInfo.Contract + " is BUY_Qty > " + positionInfo.BuyQty + " , BUY_Value > " + positionInfo.BuyVal + " , BUY_Avg > " + positionInfo.BuyAvg + " , SELL_Qty >" + positionInfo.SellQty + " , SELL_Value > " + positionInfo.SellVal + " , SELL_Avg > " + positionInfo.SellAvg + " , NET_Qty > " + positionInfo.NetQty + " , NET_Value > " + positionInfo.NetVal + " , NET_Price > " + positionInfo.NetPrice + " .";
                    DataRow dr = messageLogDS.dtMessageLog.NewRow();
                    DateTime dt = DateTime.UtcNow;
                    string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                    dr["Time"] = dt;
                    dr["MessageType"] = "PARTICIPANT RESPONSE";
                    dr["Message"] = "PARTICIPANT RESPONSE :- " + message;
                    dr["Account"] = (int)lstAccountInfo[0].Account;
                    dr["StrDateTime"] = str;
                    dr["Color"] = color.Name;
                    if (System.Threading.Monitor.TryEnter(messageLogDS.dtMessageLog, 1000))
                    {
                        try
                        {
                            messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                            messageLogDS.dtMessageLog.AcceptChanges();
                        }
                        finally
                        {
                            System.Threading.Monitor.Exit(messageLogDS.dtMessageLog);
                        }
                    }
                    foreach (KeyValuePair<int, DataRow> account in _DDAccounts)
                    {
                        switch (account.Value["AccountType"].ToString().ToUpper().Trim())
                        {
                            case "BROKER":
                                //BOChanges Properties.Settings.Default.DefaultTradingAccount = Convert.ToString(account.Key);
                                ClsGlobal.BrokerAccountId = account.Key;
                                ClsGlobal.MarketMakerAccountId = 0;
                                break;
                            case "MARKETMAKER":
                                //BOChanges Properties.Settings.Default.DefaultTradingAccount = Convert.ToString(account.Key);
                                ClsGlobal.MarketMakerAccountId = account.Key;
                                ClsGlobal.BrokerAccountId = 0;
                                break;
                        }
                    }
                    if (lstAccountInfo.Count > 1 && (ClsGlobal.BrokerAccountId > 0 || ClsGlobal.MarketMakerAccountId > 0))
                    {
                        foreach (KeyValuePair<int, DataRow> account in _DDAccounts)
                        {
                            //BOChanges if (Properties.Settings.Default.DefaultTradingAccount != Convert.ToString(account.Key))
                                RequestForNetPositionOfAccount(Convert.ToInt32(account.Key), false);
                        }
                    }
                    lock (accountInfoDS.dtAccountInfo)
                    {
                        accountInfoDS.dtAccountInfo.AcceptChanges();
                    }
                    OnParticipantResponse(_DDAccounts);
                }


                //FileHandling.WriteInOutLog("Order Server : Participant Response AccountList " + lstAccountInfo);
                //FileHandling.WriteAllLog("Order Server : Participant Response AccountList " + lstAccountInfo);
            }
            else
            {
                MessageBox.Show("No Trading account is associated with participant.");
            }
            #region "   Commented code   "

            //List<ClientDLL_Model.Cls.Order.AccountInfo> lstAccountInfo = null;
            //ClientDLL_Model.Cls.Order.ParticipantList lstPartList = null;
            //if (this._IsOrderMgrLoaded)
            //{
            //    lstPartList = objOrderManager.getParticipantList();
            //    lstAccountInfo = new List<ClientDLL_Model.Cls.Order.AccountInfo>();
            //}
            //if (lstPartList != null)
            //{
            //    foreach (ClientDLL_Model.Cls.Order.AccountInfo acInfo in lstPartList._lstAccoutInfo)
            //    {
            //        lstAccountInfo.Add(acInfo);
            //    }
            //}
            //return lstAccountInfo;

            #endregion
        }

        public void onPositionResponse(List<Position> lstPosition, bool islastPck)
        {
            //FileHandling.WriteInOutLog("Order Server : Position Response" + lstPosition);
            //FileHandling.WriteAllLog("Order Server : Position Response" + lstPosition);

            //_lstPositions = lstPosition;
            for (int i = 0; i < lstPosition.Count; i++)
            {
                Position positionInfo = lstPosition[i];
                Symbol sym = Symbol.GetSymbol(positionInfo.Gateway + Symbol._Seperator + positionInfo.ProductType + Symbol._Seperator + positionInfo.Product + Symbol._Seperator + positionInfo.Contract);
                if (positionInfo != null)
                {
                    string productType = ClsGlobal.DDReverseProductType[Convert.ToSByte(positionInfo.ProductType)];
                    string pType = string.Empty;
                    switch (productType)
                    {
                        case "EQ":
                            pType = "Equity";
                            break;
                        case "FUT":
                            pType = "FUTURE";
                            break;
                        case "FX":
                            pType = "FOREX";
                            break;
                        case "OPT":
                            pType = "OPTION";
                            break;
                        case "SP":
                            pType = "SPOT";
                            break;
                        case "PH":
                            pType = "PHYSICAL";
                            break;
                        case "AU":
                            pType = "AUCTION";
                            break;
                        case "BON":
                            pType = "BOND";
                            break;
                        default:
                            break;
                    }
                    ;
                    InstrumentSpec objInstrumentSpec = ClsTWSContractManager.INSTANCE.GetContractSpec(positionInfo.Contract, pType, positionInfo.Product);
                    //InstrumentSpec objInstrumentSpec =ClsGlobal.DDContractInfo[sym.KEY];
                    DataRow drT = null;
                    List<Symbol> lst = new List<Symbol>();
                    if (_DDNetPosRow.TryGetValue(positionInfo.Account.ToString() + ":" + positionInfo.Contract, out drT))
                    {
                    }
                    if (drT == null)
                    {

                        DataRow row = netpositionDS.dtNetPosition.NewRow();
                        row["AccountNo"] = Convert.ToString(positionInfo.Account);
                        row["ProductType"] = pType;
                        row["Contract"] = Convert.ToString(positionInfo.Contract);
                        if (objInstrumentSpec != null)
                            row["TradingCurrency"] = objInstrumentSpec.TradingCurrency;
                        row["ProductName"] = Convert.ToString(positionInfo.Product);
                        row["BuyQty"] = Convert.ToInt32(positionInfo.BuyQty);
                        row["BuyVal"] = Math.Round(positionInfo.BuyVal, 2);
                        row["BuyAvg"] = Math.Round(positionInfo.BuyAvg, 2);
                        row["SellQty"] = Convert.ToInt32(positionInfo.SellQty);
                        row["SellVal"] = Math.Round(positionInfo.SellVal, 2);
                        row["SellAvg"] = positionInfo.SellAvg;
                        row["NetQty"] = Convert.ToInt32(positionInfo.NetQty);
                        row["NetVal"] = Math.Round(positionInfo.NetVal, 2);
                        row["NetPrice"] = Math.Round(positionInfo.NetPrice, 2);
                        if (pType != "FOREX")
                        {
                            if (ClsGlobal.DDLTP.Keys.Contains(row["Contract"].ToString()))
                                row["MarketPrice"] =
                                    Math.Round(Convert.ToDecimal(ClsGlobal.DDLTP[row["Contract"].ToString()]), 2);
                        }
                        else
                        {
                            if (positionInfo.NetQty >= 0) //left//bid
                            {
                                if (ClsGlobal.DDLeftZLevel.Keys.Contains(row["Contract"].ToString()))
                                    row["MarketPrice"] =
                                        Math.Round(Convert.ToDecimal(ClsGlobal.DDLeftZLevel[row["Contract"].ToString()]), 2);
                            }
                            else //right
                            {
                                if (ClsGlobal.DDRightZLevel.Keys.Contains(row["Contract"].ToString()))
                                    row["MarketPrice"] =
                                        Math.Round(Convert.ToDecimal(ClsGlobal.DDRightZLevel[row["Contract"].ToString()]), 2);
                            }
                        }
                        decimal marketPrice = 0;
                        if (!string.IsNullOrEmpty(row["MarketPrice"].ToString()))
                            marketPrice = Math.Round(Convert.ToDecimal(row["MarketPrice"].ToString()), 2);

                        if (objInstrumentSpec != null)
                        {
                            if (positionInfo.BuyQty > positionInfo.SellQty)
                                row["UR_PL"] = Math.Round(positionInfo.NetQty * (marketPrice - positionInfo.BuyAvg) * objInstrumentSpec.ContractSize, 2);
                            else if (positionInfo.BuyQty < positionInfo.SellQty)
                                row["UR_PL"] = Math.Round(positionInfo.NetQty * (positionInfo.SellAvg - marketPrice) * objInstrumentSpec.ContractSize, 2);
                            else
                                row["UR_PL"] = Math.Round(0d, 2);
                        }
                        if (System.Threading.Monitor.TryEnter(ClsTWSOrderManager.INSTANCE.netpositionDS.dtNetPosition))
                        {
                            try
                            {
                                ClsTWSOrderManager.INSTANCE.netpositionDS.dtNetPosition.Rows.InsertAt(row, 1);
                            }
                            finally
                            {
                                System.Threading.Monitor.Exit(ClsTWSOrderManager.INSTANCE.netpositionDS.dtNetPosition);
                            }
                        }

                        if (_DDNetPosRow != null)
                        {
                            if (!_DDNetPosRow.Keys.Contains(positionInfo.Account.ToString() + ":" + positionInfo.Contract))
                                _DDNetPosRow.Add(positionInfo.Account.ToString() + ":" + positionInfo.Contract, row);
                        }


                        lst.Add(sym);
                        ClsTWSDataManager.INSTANCE.SubscribeForQuotes(true, eMarketRequest.MARKET_QUOTE_REQUEST, lst);
                        var x = new KeyValuePair<string, DataRow>(positionInfo.Contract, row);
                        if (!PosContracts.Contains(positionInfo.Contract))
                            PosContracts.Add(positionInfo.Contract);
                        _DDNetPos.Add(x);
                    }
                    else
                    {
                        drT["AccountNo"] = Convert.ToString(positionInfo.Account);
                        drT["ProductType"] = pType;
                        drT["Contract"] = Convert.ToString(positionInfo.Contract);
                        if (objInstrumentSpec != null)
                            drT["TradingCurrency"] = objInstrumentSpec.TradingCurrency;
                        drT["ProductName"] = Convert.ToString(positionInfo.Product);
                        drT["BuyQty"] = Convert.ToString(positionInfo.BuyQty);
                        drT["BuyVal"] = Convert.ToString(Math.Round(positionInfo.BuyVal, 2));
                        drT["BuyAvg"] = Math.Round(positionInfo.BuyAvg, 2);
                        drT["SellQty"] = Convert.ToString(positionInfo.SellQty);
                        drT["SellVal"] = Convert.ToString(Math.Round(positionInfo.SellVal, 2));
                        drT["SellAvg"] = Math.Round(positionInfo.SellAvg, 2);
                        drT["NetQty"] = Convert.ToString(positionInfo.NetQty);
                        drT["NetVal"] = Convert.ToString(Math.Round(positionInfo.NetVal, 2));
                        drT["NetPrice"] = Math.Round(positionInfo.NetPrice, 2);
                    }

                }


                if (islastPck && ((i + 1) == lstPosition.Count))
                {
                    if (System.Threading.Monitor.TryEnter(netpositionDS.dtNetPosition, 1000))
                    {
                        try
                        {
                            netpositionDS.dtNetPosition.AcceptChanges();
                        }
                        finally
                        {
                            System.Threading.Monitor.Exit(netpositionDS.dtNetPosition);
                        }
                    }
                    string message = string.Empty;
                    Color color = Color.White;
                    message = "Net Position for Account >" + lstPosition[0].Account + " Received.";
                    //message = "Net Position for Account >" + positionInfo.Account + " for Symbol > " + positionInfo.Contract + " is BUY_Qty > " + positionInfo.BuyQty + " , BUY_Value > " + positionInfo.BuyVal + " , BUY_Avg > " + positionInfo.BuyAvg + " , SELL_Qty >" + positionInfo.SellQty + " , SELL_Value > " + positionInfo.SellVal + " , SELL_Avg > " + positionInfo.SellAvg + " , NET_Qty > " + positionInfo.NetQty + " , NET_Value > " + positionInfo.NetVal + " , NET_Price > " + positionInfo.NetPrice + " .";
                    DataRow dr = messageLogDS.dtMessageLog.NewRow();
                    DateTime dt = DateTime.UtcNow;
                    string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                    dr["Time"] = dt;
                    dr["MessageType"] = "POSITION RESPONSE";
                    dr["Message"] = "POSITION RESPONSE :- " + message;
                    dr["Account"] = (int)lstPosition[0].Account;
                    dr["StrDateTime"] = str;
                    dr["Color"] = color.Name;
                    if (System.Threading.Monitor.TryEnter(messageLogDS.dtMessageLog, 1000))
                    {
                        try
                        {
                            messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                            messageLogDS.dtMessageLog.AcceptChanges();
                        }
                        finally
                        {
                            System.Threading.Monitor.Exit(messageLogDS.dtMessageLog);
                        }
                    }
                    OnPositionResponse(lstPosition);
                }
            }
        }

        public void onExecutionReport(ExecutionReport executionReport)
        {

        }

        public void onOrderHistoryResponse(List<OrderHistory> lstOrderHistory, bool islastPck)
        {
            //if (lstOrderHistory.Count > 0)
            //{
            //    //if (System.Threading.Monitor.TryEnter(INSTANCE.orderHistoryDS.dtOrderHistory))
            //    //{
            //        try
            //        {
            //            orderHistoryDS.dtOrderHistory.Columns["ClientOrderId"].DataType = Type.GetType("System.Int32");
            //            foreach (OrderHistory ordH in lstOrderHistory)
            //            {
            //                //ordH.LnkOrdID
            //                    //ordH.PositionEffect
            //                //ordH.QtyClosed
            //                DataRow dr = orderHistoryDS.dtOrderHistory.NewRow();
            //                string orderStatus = string.Empty;
            //                string productType = string.Empty;
            //                string side = string.Empty;
            //                string orderType = string.Empty;
            //                string timeInForce = string.Empty;
            //                if (ClsGlobal.DDReverseOrderStatus.Keys.Contains(Convert.ToSByte(ordH.OrderStatus)))
            //                    orderStatus = ClsGlobal.DDReverseOrderStatus[Convert.ToSByte(ordH.OrderStatus)].ToUpper();


            //                //if (orderStatus == "WORKING")
            //                //{
            //                    if (ClsGlobal.DDReverseProductType.Keys.Contains(Convert.ToSByte(ordH.ProductType)))
            //                        productType = ClsGlobal.DDReverseProductType[Convert.ToSByte(ordH.ProductType)];
            //                    if (ClsGlobal.DDReverseSide.Keys.Contains(Convert.ToSByte(ordH.Side)))
            //                        side = ClsGlobal.DDReverseSide[Convert.ToSByte(ordH.Side)];
            //                    if (ClsGlobal.DDReverseOrderType.Keys.Contains(Convert.ToSByte(ordH.OrderType)))
            //                        orderType = ClsGlobal.DDReverseOrderType[Convert.ToSByte(ordH.OrderType)];
            //                    if (ClsGlobal.DDReverseValidity.Keys.Contains(Convert.ToSByte(ordH.TimeInForce)))
            //                        timeInForce = ClsGlobal.DDReverseValidity[Convert.ToSByte(ordH.TimeInForce)];
            //                    string transactionTime = GetDateTime(ordH.TransactTime);
            //                    string pType = string.Empty;
            //                    switch (productType)
            //                    {
            //                        case "EQ":
            //                            pType = "Equity";
            //                            break;
            //                        case "FUT":
            //                            pType = "FUTURE";
            //                            break;
            //                        case "FX":
            //                            pType = "FOREX";
            //                            break;
            //                        case "OPT":
            //                            pType = "OPTION";
            //                            break;
            //                        case "SP":
            //                            pType = "SPOT";
            //                            break;
            //                        case "PH":
            //                            pType = "PHYSICAL";
            //                            break;
            //                        case "AU":
            //                            pType = "AUCTION";
            //                            break;
            //                        case "BON":
            //                            pType = "BOND";
            //                            break;
            //                        default:
            //                            break;
            //                    }
            //                    ;
            //                    dr["Account"] = ordH.Account;
            //                    dr["Author"] = ordH.author;
            //                    dr["AvgPrice"] = ordH.AvgPx;
            //                    dr["ClientOrderID"] = ordH.ClOrdId;
            //                    dr["Contract"] = ordH.Contract;
            //                    dr["CumQty"] = ordH.CumQty;
            //                    //dr["ExecID"] = ordH.ExecID;
            //                    //dr["ExecTransType"] = ordH.ExecTransType;
            //                    //dr["ExecType"] = ordH.ExecType;
            //                    //dr["ExpireDate"] = ordH.ExpireDate;
            //                    dr["Industry"] = ordH.industry;
            //                    dr["LeavesQty"] = ordH.LeavesQty;
            //                    dr["OrderID"] = Convert.ToInt32(ordH.OrderID);
            //                    dr["OrderStatus"] = orderStatus;
            //                    dr["OrderType"] = orderType;
            //                    dr["OrderQty"] = ordH.OrdQty;
            //                    dr["OrigClientOrderID"] = ordH.OrigClOrdId;
            //                    //dr["PositionEffect"] = ordH.PositionEffect;
            //                    //dr["ClosedQty"] = ordH.QtyClosed;
            //                    //dr["LnkOrdID"] = ordH.LnkOrdID;
            //                    //dr["CounterClOrdId"] = ordH.CounterClOrdId;
            //                    //dr["CounterAvgPx"] = ordH.CounterAvgPx;


            //                    switch (orderStatus)
            //                    {
            //                        case "PARTIALLY_FILLED":
            //                        case "FILLED":
            //                            dr["Commission"] = ordH.Commission;
            //                            dr["Tax"] = ordH.Tax;
            //                            dr["Color"] = Color.White.Name;
            //                            break;
            //                        case "WORKING":
            //                            dr["Color"] = Color.DarkGreen.Name;
            //                            break;
            //                        case "PENDING_NEW":
            //                            dr["Color"] = Color.LightPink.Name;
            //                            break;
            //                        case "CANCELED":
            //                            dr["Color"] = Color.Red.Name;
            //                            break;
            //                        case "REJECTED":
            //                            dr["Color"] = Color.LightYellow.Name;
            //                            break;
            //                        default:
            //                            dr["Color"] = Color.White.Name;
            //                            break;
            //                    }

            //                    dr["Product"] = ordH.Product;
            //                    dr["ProductType"] = pType;
            //                    InstrumentSpec objInstrumentSpec = ClsTWSContractManager.INSTANCE.GetContractSpec(ordH.Contract, pType, ordH.Product);
            //                    string key = ordH.Gateway + Symbol._Seperator + ordH.ProductType + Symbol._Seperator + ordH.Product + Symbol._Seperator + ordH.Contract;
            //                    //InstrumentSpec objInstrumentSpec = ClsGlobal.DDContractInfo[key];
            //                    if (objInstrumentSpec != null)
            //                    {
            //                        double grosspl = 0;
            //                        grosspl = side == "BUY" ? Math.Round(Convert.ToDouble(ordH.CounterAvgPx - ordH.AvgPx) * ordH.OrdQty * objInstrumentSpec.ContractSize, 2) : Math.Round(Convert.ToDouble(ordH.AvgPx - ordH.CounterAvgPx) * ordH.OrdQty * objInstrumentSpec.ContractSize, 2);
            //                        dr["GrossPL"] = grosspl;
            //                    }
            //                    dr["Sector"] = ordH.sector;
            //                    dr["Side"] = side;
            //                    dr["StopPx"] = ordH.StopPx;
            //                    dr["TimeInForce"] = timeInForce;
            //                    if (objInstrumentSpec != null)
            //                        dr["TradingCurrency"] = objInstrumentSpec.TradingCurrency;
            //                    if (ordH.Price != 0M)
            //                        dr["Price"] = ordH.Price;
            //                    dr["Text"] = string.Empty;
            //                    dr["Gateway"] = ordH.Gateway;
            //                    dr["TransactTime"] = transactionTime;
            //                    if (orderHistoryDS.dtOrderHistory != null)
            //                        orderHistoryDS.dtOrderHistory.Rows.Add(dr);
            //                    if (_DDOrderRow != null)
            //                    {
            //                        if (!_DDOrderRow.Keys.Contains(ordH.OrderID))
            //                            _DDOrderRow.Add(ordH.OrderID, dr);
            //                        else
            //                            _DDOrderRow[ordH.OrderID] = dr;
            //                    }

            //                    //FileHandling.WriteInOutLog("Order Server : Order History Response of Account > " + ordH.Account +
            //                    " Contract > " + ordH.Contract + " OrderId > " + ordH.ClOrdId + " ProductType > " + pType
            //                    + " ProductName > " + ordH.Product
            //                    + " GatewayName > " + ordH.Gateway
            //                    + " Quantity > " + ordH.OrdQty
            //                    + " OrderType > " + orderType + " Price > " + ordH.Price + " StopPX > " + ordH.StopPx
            //                    + " Side > " + side + " OrderStatus > " + orderStatus + " TIF > " + timeInForce
            //                    + " Transaction Time > " + transactionTime);
            //                    //FileHandling.WriteAllLog("Order Server : Order History Response of Account > " + ordH.Account
            //                    + " Contract > " + ordH.Contract + " OrderId > " + ordH.ClOrdId
            //                    + " ProductType > " + pType
            //                    + " ProductName > " + ordH.Product
            //                    + " GatewayName > " + ordH.Gateway
            //                    + " Quantity > " + ordH.OrdQty
            //                    + " OrderType > " + orderType + " Price > " + ordH.Price + " StopPX > " + ordH.StopPx
            //                    + " Side > " + side + " OrderStatus > " + orderStatus + " TIF > " + timeInForce
            //                    + " Transaction Time > " + transactionTime);
            //                //}
            //            }


            //            if (islastPck)
            //            {
            //                string message = string.Empty;
            //                Color color = Color.White;
            //                message = "Order History for Account >" + Convert.ToString(lstOrderHistory[0].Account) + " Received.";
            //                DataRow dr1 = INSTANCE.messageLogDS.dtMessageLog.NewRow();
            //                DateTime dt = DateTime.UtcNow;
            //                string str = string.Format(Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
            //                dr1["Time"] = dt;
            //                dr1["MessageType"] = "ORDER HISTORY";
            //                dr1["Message"] = message;
            //                dr1["Account"] = 0;
            //                dr1["StrDateTime"] = str;
            //                dr1["Color"] = color.Name;
            //                if (System.Threading.Monitor.TryEnter(INSTANCE.messageLogDS.dtMessageLog, 1000))
            //                {
            //                    try
            //                    {
            //                        INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr1, 0);
            //                        INSTANCE.messageLogDS.dtMessageLog.AcceptChanges();
            //                    }
            //                    finally
            //                    {
            //                        System.Threading.Monitor.Exit(INSTANCE.messageLogDS.dtMessageLog);
            //                    }
            //                }
            //                OnOrderLog("ORDERHISTORY");
            //                //orderHistoryDS.dtOrderHistory.DefaultView.Sort = "ClientOrderId Desc";
            //                //orderHistoryDS.dtOrderHistory.AcceptChanges(); 
            //                OnOrderHistoryResponse();

            //            }
            //        }
            //        catch(Exception ex)
            //        {

            //        }
            //        finally
            //        {

            //        }

            //    //}
            //}
        }

        public void onOrderPendingNew(ExecutionReport ptrExecutionReport)
        {
            ////FileHandling.WriteInOutLog("Order Server : PendingOrder of Account" + ptrExecutionReport.Account +
            //" Contract" +
            //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
            //" ProductType" + ptrExecutionReport.ProductType
            //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
            //ptrExecutionReport.ExpireDate
            //+ " GatewayName" + ptrExecutionReport.Gateway
            //+ " Quantity" + ptrExecutionReport.OrdQty
            //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
            //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
            //" Side" + ptrExecutionReport.Side + " TIF" +
            //ptrExecutionReport.TimeInForce
            //+ " Transaction Time" + ptrExecutionReport.TransactTime);
            ////FileHandling.WriteAllLog("Order Server : PendingOrder of Account" + ptrExecutionReport.Account +
            //" Contract" +
            //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
            //" ProductType" + ptrExecutionReport.ProductType
            //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
            //ptrExecutionReport.ExpireDate
            //+ " GatewayName" + ptrExecutionReport.Gateway
            //+ " Quantity" + ptrExecutionReport.OrdQty
            //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
            //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
            //" Side" + ptrExecutionReport.Side + " TIF" + ptrExecutionReport.TimeInForce
            //+ " Transaction Time" + ptrExecutionReport.TransactTime);

            OnOrderPendingNew(ptrExecutionReport);
        }

        public void onOrderCancelled(ExecutionReport executionReport)
        {
            //try
            //{

            //InvokeOrderResponseEvent(executionReport);
            DataRow drO = null;
            if (_DDPendingOrderRow != null && _DDPendingOrderRow.TryGetValue(executionReport.OrderID, out drO))
            {
            }
            if (drO != null)
            {
                if (pendingOrdersDS.dtPendingOrders.Rows.IndexOf(drO) >= 0)
                {
                    pendingOrdersDS.dtPendingOrders.Rows.Remove(drO);
                    _DDPendingOrderRow.Remove(executionReport.OrderID);
                    onPendingOrder();
                }
            }

            if (executionReport != null)
            {
                string message = string.Empty;
                string orderStatus = PALSA.Cls.ClsGlobal.DDReverseOrderStatus[Convert.ToSByte(executionReport.OrderStatus)];
                string side = PALSA.Cls.ClsGlobal.DDReverseSide[Convert.ToSByte(executionReport.Side)];
                string orderType = PALSA.Cls.ClsGlobal.DDReverseOrderType[Convert.ToSByte(executionReport.OrderType)];
                Color color = Color.White;
                //if (orderStatus.ToUpper() != "FILLED")
                //{
                //    message = "Your " + side + " " + orderType + " Order From Account >" + Convert.ToString(executionReport.Account) + " with OrderID > " + executionReport.OrderID + " for Symbol > " + executionReport.Contract + " Qty > " + executionReport.OrdQty + " Price > " + executionReport.Price + " is " + orderStatus + " on Date > " + ClsTWSOrderManager.INSTANCE.GetDateTime(executionReport.TransactTime) + " .";
                //    if (orderType.ToUpper() == "LIMIT")
                //        color = Color.Yellow;
                //    else
                //    {
                //        if (side.ToUpper() == "BUY")
                //            color = Properties.Settings.Default.BuyOrderColor;
                //        else if (side.ToUpper() == "SELL")
                //            color = Properties.Settings.Default.SellOrderColor;
                //    }
                //}
                //else
                //{
                message = "#" + executionReport.OrderID + " " + side.ToLower() + " " + executionReport.OrdQty + " " + executionReport.Contract + " is Deleted.";
                //message = "Your " + side + " " + orderType + " Order From Account >" + Convert.ToString(executionReport.Account) + " with OrderID > " + executionReport.OrderID + " for Symbol > " + executionReport.Contract + " Qty > " + executionReport.OrdQty + " Price > " + executionReport.Price + " is " + orderStatus + " with Trade No. > " + executionReport.ExecID + " on Date > " + ClsTWSOrderManager.INSTANCE.GetDateTime(executionReport.TransactTime) + " .";
                //}
                SendMessage(message);
                DataRow dr = messageLogDS.dtMessageLog.NewRow();
                DateTime dt = DateTime.UtcNow;
                string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                dr["Time"] = dt;
                dr["MessageType"] = "ORDER RESPONSE";
                dr["Message"] = "ORDER RESPONSE :- " + message;
                dr["Account"] = (int)executionReport.Account;
                dr["StrDateTime"] = str;
                lock (messageLogDS.dtMessageLog)
                {
                    messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                    messageLogDS.dtMessageLog.AcceptChanges();
                }
            }
            //SetOrderLog("Order Server : OrderCancelled Account" + executionReport.Account + " Contract" +
            //           executionReport.Contract + " OrderId" + executionReport.ClOrdId + " ProductType" +
            //           executionReport.ProductType
            //           + " ProductName" + executionReport.Product + " ExpiryDate" +
            //           executionReport.ExpireDate
            //           + " GatewayName" + executionReport.Gateway
            //           + " Quantity" + executionReport.OrdQty
            //           + " OrderType" + executionReport.OrderType + " Price" + executionReport.Price +
            //           " StopPX" + executionReport.StopPx +
            //           " Side" + executionReport.Side + " TIF" + executionReport.TimeInForce
            //           + " Transaction Time" + executionReport.TransactTime);
            ////FileHandling.WriteInOutLog("Order Server : OrderCancelled Account" + ptrExecutionReport.Account +
            //" Contract" +
            //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
            //" ProductType" + ptrExecutionReport.ProductType
            //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
            //ptrExecutionReport.ExpireDate
            //+ " GatewayName" + ptrExecutionReport.Gateway
            //+ " Quantity" + ptrExecutionReport.OrdQty
            //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
            //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
            //" Side" + ptrExecutionReport.Side + " TIF" +
            //ptrExecutionReport.TimeInForce
            //+ " Transaction Time" + ptrExecutionReport.TransactTime);
            ////FileHandling.WriteAllLog("Order Server : OrderCancelled Account" + ptrExecutionReport.Account +
            //" Contract" +
            //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
            //" ProductType" + ptrExecutionReport.ProductType
            //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
            //ptrExecutionReport.ExpireDate
            //+ " GatewayName" + ptrExecutionReport.Gateway
            //+ " Quantity" + ptrExecutionReport.OrdQty
            //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
            //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
            //" Side" + ptrExecutionReport.Side + " TIF" + ptrExecutionReport.TimeInForce
            //+ " Transaction Time" + ptrExecutionReport.TransactTime);
            //}
            //catch (Exception ex)
            //{
            //    //throw new Exception(ex.Message + " : " + ex.InnerException);
            //}
        }

        public void onOrderRejected(ExecutionReport ptrExecutionReport)
        {
            //try
            //{
            InvokeOrderResponseEvent(ptrExecutionReport);
            SetOrderLog("Order Server : Order Rejected Account" + ptrExecutionReport.Account +
                        " Contract" + ptrExecutionReport.Contract);

            ////FileHandling.WriteInOutLog("Order Server : Order Rejected of Account" + ptrExecutionReport.Account +
            //" Contract" +
            //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
            //" ProductType" + ptrExecutionReport.ProductType
            //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
            //ptrExecutionReport.ExpireDate
            //+ " GatewayName" + ptrExecutionReport.Gateway
            //+ " Quantity" + ptrExecutionReport.OrdQty
            //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
            //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
            //" Side" + ptrExecutionReport.Side + " TIF" +
            //ptrExecutionReport.TimeInForce
            //+ " Transaction Time" + ptrExecutionReport.TransactTime);
            ////FileHandling.WriteAllLog("Order Server : Order Rejected of Account" + ptrExecutionReport.Account +
            //" Contract" +
            //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
            //" ProductType" + ptrExecutionReport.ProductType
            //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
            //ptrExecutionReport.ExpireDate
            //+ " GatewayName" + ptrExecutionReport.Gateway
            //+ " Quantity" + ptrExecutionReport.OrdQty
            //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
            //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
            //" Side" + ptrExecutionReport.Side + " TIF" + ptrExecutionReport.TimeInForce
            //+ " Transaction Time" + ptrExecutionReport.TransactTime);
            //}
            //catch (Exception ex)
            //{
            //    //throw new Exception(ex.Message + " : " + ex.InnerException);
            //}
        }

        public void onOrderReplaced(ExecutionReport ptrExecutionReport)
        {
            InvokeOrderResponseEvent(ptrExecutionReport);
            SetOrderLog("Order Server : Order Replaced Account" + ptrExecutionReport.Account +
                        " Contract" + ptrExecutionReport.Contract);

            ////FileHandling.WriteInOutLog("Order Server : Order Replaced of Account" + ptrExecutionReport.Account +
            //" Contract" +
            //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
            //" ProductType" + ptrExecutionReport.ProductType
            //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
            //ptrExecutionReport.ExpireDate
            //+ " GatewayName" + ptrExecutionReport.Gateway
            //+ " Quantity" + ptrExecutionReport.OrdQty
            //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
            //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
            //" Side" + ptrExecutionReport.Side + " TIF" +
            //ptrExecutionReport.TimeInForce
            //+ " Transaction Time" + ptrExecutionReport.TransactTime);
            ////FileHandling.WriteAllLog("Order Server : Order Replaced of Account" + ptrExecutionReport.Account +
            //" Contract" +
            //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
            //" ProductType" + ptrExecutionReport.ProductType
            //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
            //ptrExecutionReport.ExpireDate
            //+ " GatewayName" + ptrExecutionReport.Gateway
            //+ " Quantity" +
            //ptrExecutionReport.OrdQty
            //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
            //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
            //" Side" + ptrExecutionReport.Side + " TIF" + ptrExecutionReport.TimeInForce
            //+ " Transaction Time" + ptrExecutionReport.TransactTime);
        }

        public void onOrderWorking(ExecutionReport ptrExecutionReport)
        {
            InvokeOrderResponseEvent(ptrExecutionReport);
            SetOrderLog("Order Server : Order Working Account" + ptrExecutionReport.Account +
                        " Contract" + ptrExecutionReport.Contract);

            ////FileHandling.WriteInOutLog("Order Server : Order Working of Account" + ptrExecutionReport.Account +
            //" Contract" +
            //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
            //" ProductType" + ptrExecutionReport.ProductType
            //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
            //ptrExecutionReport.ExpireDate
            //+ " GatewayName" + ptrExecutionReport.Gateway
            //+ " Quantity" +
            //ptrExecutionReport.OrdQty
            //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
            //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
            //" Side" + ptrExecutionReport.Side + " TIF" +
            //ptrExecutionReport.TimeInForce
            //+ " Transaction Time" + ptrExecutionReport.TransactTime);
            ////FileHandling.WriteAllLog("Order Server : Order Working of Account" + ptrExecutionReport.Account +
            //" Contract" +
            //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
            //" ProductType" + ptrExecutionReport.ProductType
            //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
            //ptrExecutionReport.ExpireDate
            //+ " GatewayName" + ptrExecutionReport.Gateway
            //+ " Quantity" +
            //ptrExecutionReport.OrdQty
            //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
            //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
            //" Side" + ptrExecutionReport.Side + " TIF" + ptrExecutionReport.TimeInForce
            //+ " Transaction Time" + ptrExecutionReport.TransactTime);
        }

        public void onBusinessLevelReject(BusinessReject ptrBusinessReject)
        {
            try
            {
                string msg = "Order rejected reason " + ptrBusinessReject.Text;
                SendMessage(msg);
                DataRow drO = null;
                if (ptrBusinessReject.RefMsgType == (int)ClientDLL_Model.Cls.MessageTypes.NEW_ORDER_REQUEST ||
                    ptrBusinessReject.RefMsgType == (int)ClientDLL_Model.Cls.MessageTypes.ORDER_CANCEL_REQUEST ||
                    ptrBusinessReject.RefMsgType == (int)ClientDLL_Model.Cls.MessageTypes.ORDER_REPLACE_REQUEST ||
                    ptrBusinessReject.RefMsgType == (int)ClientDLL_Model.Cls.MessageTypes.ORDER_CANCEL_REJECT_RESPONSE)
                {

                    double num;
                    bool isNum = double.TryParse(ptrBusinessReject.BusinessRejectRefID, out num);
                    if (isNum)
                    {
                        lock (orderHistoryDS.dtOrderHistory)
                        {
                            if (_DDOrderRow.TryGetValue(Convert.ToUInt32(ptrBusinessReject.BusinessRejectRefID), out drO))
                            {
                            }
                            if (drO != null)
                            {
                                drO["Text"] = ptrBusinessReject.Text;
                                drO["Color"] = Color.LightYellow.Name;
                                drO["OrderStatus"] = "REJECTED";
                            }
                            else
                            {
                                //FileHandling.WriteDevelopmentLog("Error: Functionality error Execution report for OrderId =" + ptrBusinessReject.BusinessRejectRefID + " should Come before bussiness reject for the same order.");
                            }
                            orderHistoryDS.dtOrderHistory.AcceptChanges();
                        }
                        string message = string.Empty;
                        if (drO != null)
                        {

                            string orderStatus = drO["OrderStatus"].ToString();
                            string side = drO["Side"].ToString();
                            string orderid = drO["OrderId"].ToString();
                            string contract = drO["Contract"].ToString();
                            string ordQty = drO["orderQty"].ToString();
                            string price = drO["price"].ToString();
                            string reason = drO["Text"].ToString();
                            string date = drO["TransactTime"].ToString();
                            Color color = Color.FromName(drO["Color"].ToString());

                            message = "Your " + side + " Order with OrderID > " + orderid + " for Symbol > " + contract + " Qty > " + ordQty + " Price > " + price + " is " + orderStatus + " due to reason > " + reason + " on Date > " + date + " .";
                            DataRow dr = messageLogDS.dtMessageLog.NewRow();
                            DateTime dt = DateTime.UtcNow;
                            string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                            dr["Time"] = dt;
                            dr["MessageType"] = "ORDER RESPONSE";
                            dr["Message"] = message;
                            dr["Account"] = (int)drO["Account"];
                            dr["StrDateTime"] = str;
                            dr["Color"] = drO["Color"].ToString();
                            if (System.Threading.Monitor.TryEnter(messageLogDS.dtMessageLog, 1000))
                            {
                                try
                                {
                                    messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                                    messageLogDS.dtMessageLog.AcceptChanges();
                                }
                                finally
                                {
                                    System.Threading.Monitor.Exit(messageLogDS.dtMessageLog);
                                }
                            }
                        }
                        else
                        {
                            DateTime date = DateTime.UtcNow;
                            string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", date);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", date);

                            string reason = Enum.GetName(typeof(BusinessRejectReason), ptrBusinessReject.BusinessRejectReason);
                            string text = ptrBusinessReject.Text;
                            DataRow dr = messageLogDS.dtMessageLog.NewRow();
                            message = "Your New Order with OrderID > " + ptrBusinessReject.BusinessRejectRefID + " is REJECTED due to reason > " + reason + " on Date > " + str + " .";
                            dr["Time"] = date;
                            dr["MessageType"] = "ORDER RESPONSE";
                            dr["Message"] = message;
                            dr["Account"] = 0;
                            dr["StrDateTime"] = str;
                            dr["Color"] = Color.Yellow;

                            if (System.Threading.Monitor.TryEnter(messageLogDS.dtMessageLog, 1000))
                            {
                                try
                                {
                                    messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                                    messageLogDS.dtMessageLog.AcceptChanges();
                                }
                                finally
                                {
                                    System.Threading.Monitor.Exit(messageLogDS.dtMessageLog);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var message = Enum.GetName(typeof(ClientDLL_Model.Cls.BusinessRejectReason), ptrBusinessReject.BusinessRejectReason);
                    //MessageBox.Show(message);
                }
                OnBusinessLevelReject(drO, ptrBusinessReject);
            }
            catch (NullReferenceException ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public void onMatchedOrderResponse(List<MatchedOrder> lstMatchedOrder)
        {
            //throw new NotImplementedException();
        }

        public void onOrderBookResponse(List<OrderHistory> lstOrderHistory, bool islastPck)
        {
            //throw new NotImplementedException();
        }

        public void onStopOrderResponse(List<OrderHistory> lstOrderHistory)
        {
            //throw new NotImplementedException();
        }

        public void onChangePasswordResponse(string userName, string reason, bool isPasswordChanged)
        {

            DateTime date = DateTime.UtcNow;
            string message = string.Empty;
            string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", date);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", date);
            DataRow dr = messageLogDS.dtMessageLog.NewRow();
            if (isPasswordChanged)
            {
                message = userName + " Your Password changed successfuly .";
            }
            else
            {
                message = userName + " Your Powssord can not be changed due to " + reason + " .";
            }
            dr["Time"] = date;
            dr["MessageType"] = "PASSWORD CHANGE RESPONSE";
            dr["Message"] = message;
            dr["Account"] = 0;
            dr["StrDateTime"] = str;
            dr["Color"] = Color.Yellow;
            if (System.Threading.Monitor.TryEnter(messageLogDS.dtMessageLog, 1000))
            {
                try
                {
                    messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                    messageLogDS.dtMessageLog.AcceptChanges();
                }
                finally
                {
                    System.Threading.Monitor.Exit(messageLogDS.dtMessageLog);
                }
            }
            OnChangePasswordResponse(userName, reason, isPasswordChanged);
        }

        public void onLogonResponse(string UserName, string reason, string brokerName, string AccountType, bool isLive)
        {
            if (reason == "VALID")
            {
                if (isLive)
                {
                    IsDemo = false;
                }
                else
                {
                    IsDemo = true;
                }

                if (IsOrderMgrLoaded)
                {
                    if (!_DDMessages.ContainsKey("Connected"))
                    {
                        _DDMessages["Connected"] = "Order Server";
                    }
                    if (OnOrderServerConnectionEvnt != null)
                    {
                        OnOrderServerConnectionEvnt("Connected");
                    }

                }

                //BOChanges Properties.Settings.Default.UserName = " " + clsResourceMGR.INSTANCE.resourceManager.GetString("MainFormTitle") + " , " + brokerName;
            }
            else
            {
                if (OnBothServerConnectionEvnt != null)
                {
                    //OnBothServerConnectionEvnt("DisConnected");
                    OnBothServerConnectionEvnt(reason);
                }

                //MessageBox.Show("Login Failed " + reason + " .");
            }
            if (System.Threading.Monitor.TryEnter(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog, 1000))
            {
                try
                {
                    DataRow dr = ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.NewRow();
                    DateTime dt = DateTime.UtcNow;
                    string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                    dr["Time"] = dt;
                    dr["MessageType"] = "ORDER SERVER";
                    if (IsOrderMgrLoaded)
                    {
                        dr["Message"] = "Order Server Connected.";
                    }
                    else
                    {
                        dr["Message"] = "Order Server Disconnected.";
                    }
                    dr["Account"] = 0;
                    dr["StrDateTime"] = str;
                    dr["Color"] = "White";
                    DataRow dr1 = INSTANCE.messageLogDS.dtMessageLog.NewRow();
                    dr["Time"] = dt;
                    dr1["MessageType"] = "LOGIN RESPONCE";
                    if (reason == "VALID")
                    {
                        dr1["Message"] = "You are Login Successfully.";
                    }
                    else
                    {
                        dr1["Message"] = "Login Failed.";
                    }
                    dr1["Account"] = 0;
                    dr1["StrDateTime"] = str;
                    dr1["Color"] = "White";
                    INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr1, 0);
                    INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                    INSTANCE.messageLogDS.dtMessageLog.AcceptChanges();

                }
                finally
                {
                    System.Threading.Monitor.Exit(INSTANCE.messageLogDS.dtMessageLog);
                }
            }

            OnLogonResponse(reason, brokerName, AccountType);
        }

        public void onClientError(int error)
        {
            //throw new NotImplementedException();
        }

        public void OnPendingOrder(ExecutionReport ptrExecutionReport)
        {

            string orderStatus = string.Empty;
            string productType = string.Empty;
            string side = string.Empty;
            string orderType = string.Empty;
            string timeInForce = string.Empty;
            if (ClsGlobal.DDReverseOrderStatus.Keys.Contains(Convert.ToSByte(ptrExecutionReport.OrderStatus)))
                orderStatus = ClsGlobal.DDReverseOrderStatus[Convert.ToSByte(ptrExecutionReport.OrderStatus)].ToUpper();
            if (ClsGlobal.DDReverseProductType.Keys.Contains(Convert.ToSByte(ptrExecutionReport.ProductType)))
                productType = ClsGlobal.DDReverseProductType[Convert.ToSByte(ptrExecutionReport.ProductType)];
            if (ClsGlobal.DDReverseSide.Keys.Contains(Convert.ToSByte(ptrExecutionReport.Side)))
                side = ClsGlobal.DDReverseSide[Convert.ToSByte(ptrExecutionReport.Side)];
            if (ClsGlobal.DDReverseOrderType.Keys.Contains(Convert.ToSByte(ptrExecutionReport.OrderType)))
                orderType = ClsGlobal.DDReverseOrderType[Convert.ToSByte(ptrExecutionReport.OrderType)];
            if (ClsGlobal.DDReverseValidity.Keys.Contains(Convert.ToSByte(ptrExecutionReport.TimeInForce)))
                timeInForce = ClsGlobal.DDReverseValidity[Convert.ToSByte(ptrExecutionReport.TimeInForce)];

            lock (ClsTWSOrderManager.INSTANCE.pendingOrdersDS.dtPendingOrders)
            {
                string reason = ptrExecutionReport.Text;
                DataRow drO = null; //for Order
                string transactTime = string.Empty;
                transactTime = GetDateTime(ptrExecutionReport.TransactTime);
                string pType = string.Empty;
                switch (productType)
                {
                    case "EQ":
                        pType = "Equity";
                        break;
                    case "FUT":
                        pType = "FUTURE";
                        break;
                    case "FX":
                        pType = "FOREX";
                        break;
                    case "OPT":
                        pType = "OPTION";
                        break;
                    case "SP":
                        pType = "SPOT";
                        break;
                    case "PH":
                        pType = "PHYSICAL";
                        break;
                    case "AU":
                        pType = "AUCTION";
                        break;
                    case "BON":
                        pType = "BOND";
                        break;
                    default:
                        break;
                }
                ;
                InstrumentSpec objInstrumentSpec = Cls.ClsTWSContractManager.INSTANCE.GetContractSpec(ptrExecutionReport.Contract, pType, ptrExecutionReport.Product);

                string format = "0.";
                for (int i = 0; i < objInstrumentSpec.Digits; i++)
                {
                    format += "0";
                }

                if (_DDPendingOrderRow != null && _DDPendingOrderRow.TryGetValue(ptrExecutionReport.OrderID, out drO))
                {
                }
                if (drO == null)
                {
                    drO = INSTANCE.pendingOrdersDS.dtPendingOrders.NewRow();

                    drO["Account"] = ptrExecutionReport.Account;
                    drO["ClientOrderID"] = ptrExecutionReport.ClOrdId;
                    drO["Contract"] = ptrExecutionReport.Contract;
                    drO["CumQty"] = 0;
                    drO["ExecID"] = ptrExecutionReport.ExecID;
                    drO["ExecTransType"] = ptrExecutionReport.ExecTransType;
                    drO["ExecType"] = ptrExecutionReport.ExecType;
                    drO["OrderID"] = Convert.ToInt32(ptrExecutionReport.OrderID);
                    drO["OrderType"] = orderType;
                    drO["OrderQty"] = ptrExecutionReport.OrdQty;
                    drO["TransactTime"] = transactTime;
                    switch (orderStatus.ToUpper())
                    {
                        case "PARTIALLY_FILLED":
                        case "FILLED":
                            drO["Commission"] = ptrExecutionReport.Commission;
                            drO["Tax"] = ptrExecutionReport.Tax;
                            drO["Color"] = Color.White.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "WORKING":
                            drO["Color"] = Color.DarkGreen.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "PENDING_NEW":
                            drO["Color"] = Color.LightPink.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "CANCELED":
                            drO["Color"] = Color.Red.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "REJECTED":
                            drO["Color"] = Color.LightYellow.Name;
                            drO["Text"] = reason;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        default:
                            drO["Color"] = Color.White.Name;
                            break;
                    }

                    drO["Product"] = ptrExecutionReport.Product;
                    drO["ProductType"] = pType;
                    drO["Side"] = side;
                    drO["Gateway"] = ptrExecutionReport.Gateway;
                    drO["StopPx"] = ptrExecutionReport.StopPx.ToString(format);
                    drO["TimeInForce"] = timeInForce;
                    drO["PositionEffect"] = ptrExecutionReport.PositionEffect;
                    drO["ClosedQty"] = ptrExecutionReport.QtyFilled;
                    drO["LnkOrdID"] = ptrExecutionReport.LinkedOrdID;
                    if (objInstrumentSpec != null)
                        drO["TradingCurrency"] = objInstrumentSpec.TradingCurrency;
                    drO["Price"] = ptrExecutionReport.Price.ToString(format);

                    if (pendingOrdersDS != null)
                    {
                        pendingOrdersDS.dtPendingOrders.Rows.InsertAt(drO, 0);
                        if (_DDPendingOrderRow != null)
                        {
                            if (!_DDPendingOrderRow.Keys.Contains(ptrExecutionReport.OrderID))
                                _DDPendingOrderRow.Add(ptrExecutionReport.OrderID, drO);
                        }
                    }
                }
                else
                {
                    drO["TransactTime"] = transactTime;
                    switch (orderStatus.ToUpper())
                    {
                        case "PARTIALLY_FILLED":
                        case "FILLED":
                            drO["Commission"] = ptrExecutionReport.Commission;
                            drO["Tax"] = ptrExecutionReport.Tax;
                            drO["Color"] = Color.White.Name;
                            drO["AvgPrice"] = ((Convert.ToDecimal(drO["AvgPrice"].ToString()) *
                                                Convert.ToInt32(drO["CumQty"].ToString())) +
                                               (ptrExecutionReport.CumQty * ptrExecutionReport.AvgPx)) /
                                              (ptrExecutionReport.CumQty + Convert.ToInt32(drO["CumQty"].ToString()));

                            drO["CumQty"] = ptrExecutionReport.QtyFilled + Convert.ToInt32(drO["CumQty"].ToString());

                            drO["OrderStatus"] = ptrExecutionReport.OrdQty == Convert.ToInt32(drO["CumQty"].ToString()) ? "FILLED" : orderStatus;
                            break;
                        case "WORKING":
                            drO["Color"] = Color.DarkGreen.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "PENDING_NEW":
                            drO["Color"] = Color.LightPink.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "CANCELED":
                            drO["Color"] = Color.Red.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "REJECTED":
                            drO["Color"] = Color.LightYellow.Name;
                            drO["Text"] = reason;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        default:
                            drO["Color"] = Color.White.Name;
                            break;
                    };
                    decimal grosspl = 0;
                    grosspl = side == "BUY" ?
                        Math.Round(Convert.ToDecimal(ptrExecutionReport.CounterAvgPx - ptrExecutionReport.AvgPx) * ptrExecutionReport.OrdQty * objInstrumentSpec.ContractSize, 2)
                        : Math.Round(Convert.ToDecimal(ptrExecutionReport.AvgPx - ptrExecutionReport.CounterAvgPx) * ptrExecutionReport.OrdQty * objInstrumentSpec.ContractSize, 2);
                    drO["GrossPL"] = grosspl;
                }
                if (ptrExecutionReport != null)
                {
                    string message = string.Empty;
                    Color color = Color.White;
                    //#17086 buy 1.00 AUDUSD at 1.04681 successful
                    message = "#" + ptrExecutionReport.OrderID + " " + side.ToLower() + " " + ptrExecutionReport.OrdQty + " " + ptrExecutionReport.Contract + " at " + ptrExecutionReport.Price + " is Working.";
                    DataRow dr = messageLogDS.dtMessageLog.NewRow();
                    DateTime dt = DateTime.UtcNow;
                    string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                    dr["Time"] = dt;
                    dr["MessageType"] = "ORDER RESPONSE";
                    dr["Message"] = message;
                    dr["Account"] = (int)ptrExecutionReport.Account;
                    dr["StrDateTime"] = str;
                    dr["Color"] = color.Name;
                    if (System.Threading.Monitor.TryEnter(messageLogDS.dtMessageLog, 1000))
                    {
                        try
                        {
                            messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                            messageLogDS.dtMessageLog.AcceptChanges();
                        }
                        finally
                        {
                            System.Threading.Monitor.Exit(messageLogDS.dtMessageLog);
                        }
                    }
                    SendMessage(message);
                }
            }

            if (DoHandleExecutionReport == null) return;
            DoHandleExecutionReport(ptrExecutionReport);
            OnOrderLog("ExecutionReport");

            onPendingOrder();
        }

        public void OnPositionClosed(ExecutionReport ptrExecutionReport)
        {
            try
            {
                DataRow drO = null;
                if (_DDPositionOpenRow != null && _DDPositionOpenRow.TryGetValue(Convert.ToInt32(ptrExecutionReport.LinkedOrdID), out drO))
                {
                }
                if (drO != null)
                {
                    DataRow dr1 = positionCloseDS.dtPositionClose.NewRow();
                    dr1.ItemArray = drO.ItemArray;
                    dr1["PositionEffect"] = ptrExecutionReport.PositionEffect;
                    dr1["ClosedQty"] = ptrExecutionReport.QtyFilled;
                    dr1["ClosedPrice"] = ptrExecutionReport.LastPx.ToString("0.00000");
                    dr1["Profit"] = ptrExecutionReport.Profit.ToString("0.00");
                    positionCloseDS.dtPositionClose.Rows.Add(dr1);
                    if (!_DDPositionCloseRow.ContainsKey(ptrExecutionReport.OrderID))
                    {
                        _DDPositionCloseRow.Add(ptrExecutionReport.OrderID, dr1);
                    }
                    else
                    {
                        _DDPositionCloseRow[ptrExecutionReport.OrderID] = dr1;
                    }

                    if (ptrExecutionReport != null)
                    {
                        string message = string.Empty;
                        Color color = Color.White;

                        //#17088 buy 0.30 AUDUSD at 1.04627 closed 0.30 at 1.04265
                        message = "#" + drO["OrderID"].ToString() + " " + drO["Side"].ToString().ToLower() + " " + drO["OrderQty"].ToString() + " " + ptrExecutionReport.Contract + " at " + drO["Price"].ToString() + " closed " + ptrExecutionReport.QtyFilled + " at " + ptrExecutionReport.LastPx + " .";

                        DataRow dr = messageLogDS.dtMessageLog.NewRow();
                        DateTime dt = DateTime.UtcNow;
                        string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                        dr["Time"] = dt;
                        dr["MessageType"] = "ORDER RESPONSE";
                        dr["Message"] = message;
                        dr["Account"] = (int)ptrExecutionReport.Account;
                        dr["StrDateTime"] = str;
                        dr["Color"] = color.Name;
                        if (System.Threading.Monitor.TryEnter(messageLogDS.dtMessageLog, 1000))
                        {
                            try
                            {
                                messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                                messageLogDS.dtMessageLog.AcceptChanges();
                            }
                            finally
                            {
                                System.Threading.Monitor.Exit(messageLogDS.dtMessageLog);
                            }
                        }
                        SendMessage(message);
                    }
                    onPositionClosed();

                    int index = positionOpenDS.dtPositionOpen.Rows.IndexOf(drO);
                    if (index >= 0)
                    {
                        positionOpenDS.dtPositionOpen.Rows.RemoveAt(index);
                        if (_DDPositionOpenRow.ContainsKey(ptrExecutionReport.OrderID))
                        {
                            _DDPositionOpenRow.Remove(ptrExecutionReport.OrderID);
                        }
                        onPositionOpened();
                    }

                }
            }
            catch
            {

            }
        }

        public void OnPositionOpened(ExecutionReport ptrExecutionReport)
        {
            string orderStatus = string.Empty;
            string productType = string.Empty;
            string side = string.Empty;
            string orderType = string.Empty;
            string timeInForce = string.Empty;
            string transactTime = string.Empty;
            transactTime = GetDateTime(ptrExecutionReport.TransactTime);
            if (ClsGlobal.DDReverseOrderStatus.Keys.Contains(Convert.ToSByte(ptrExecutionReport.OrderStatus)))
                orderStatus = ClsGlobal.DDReverseOrderStatus[Convert.ToSByte(ptrExecutionReport.OrderStatus)].ToUpper();
            if (ClsGlobal.DDReverseProductType.Keys.Contains(Convert.ToSByte(ptrExecutionReport.ProductType)))
                productType = ClsGlobal.DDReverseProductType[Convert.ToSByte(ptrExecutionReport.ProductType)];
            if (ClsGlobal.DDReverseSide.Keys.Contains(Convert.ToSByte(ptrExecutionReport.Side)))
                side = ClsGlobal.DDReverseSide[Convert.ToSByte(ptrExecutionReport.Side)];
            if (ClsGlobal.DDReverseOrderType.Keys.Contains(Convert.ToSByte(ptrExecutionReport.OrderType)))
                orderType = ClsGlobal.DDReverseOrderType[Convert.ToSByte(ptrExecutionReport.OrderType)];
            if (ClsGlobal.DDReverseValidity.Keys.Contains(Convert.ToSByte(ptrExecutionReport.TimeInForce)))
                timeInForce = ClsGlobal.DDReverseValidity[Convert.ToSByte(ptrExecutionReport.TimeInForce)];
            string pType = string.Empty;
            switch (productType)
            {
                case "EQ":
                    pType = "Equity";
                    break;
                case "FUT":
                    pType = "FUTURE";
                    break;
                case "FX":
                    pType = "FOREX";
                    break;
                case "OPT":
                    pType = "OPTION";
                    break;
                case "SP":
                    pType = "SPOT";
                    break;
                case "PH":
                    pType = "PHYSICAL";
                    break;
                case "AU":
                    pType = "AUCTION";
                    break;
                case "BON":
                    pType = "BOND";
                    break;
                default:
                    break;
            }
            ;
            DataRow drO = null;
            if (_DDPendingOrderRow != null && _DDPendingOrderRow.TryGetValue(ptrExecutionReport.OrderID, out drO))
            {
            }
            if (drO != null)
            {

                DataRow drO1 = INSTANCE.positionOpenDS.dtPositionOpen.NewRow();
                InstrumentSpec objInstrumentSpec = Cls.ClsTWSContractManager.INSTANCE.GetContractSpec(ptrExecutionReport.Contract, pType, ptrExecutionReport.Product);
                string format = "0.";
                for (int i = 0; i < objInstrumentSpec.Digits; i++)
                {
                    format += "0";
                }
                drO1["Account"] = ptrExecutionReport.Account;
                drO1["AvgPrice"] = ptrExecutionReport.LastPx.ToString(format);
                drO1["ClientOrderID"] = ptrExecutionReport.ClOrdId;
                drO1["Contract"] = ptrExecutionReport.Contract;
                drO1["CumQty"] = 0;
                drO1["ExecID"] = ptrExecutionReport.ExecID;
                drO1["ExecTransType"] = ptrExecutionReport.ExecTransType;
                drO1["ExecType"] = ptrExecutionReport.ExecType;
                drO1["OrderID"] = Convert.ToInt32(ptrExecutionReport.OrderID);
                drO1["OrderType"] = orderType;
                drO1["OrderQty"] = ptrExecutionReport.OrdQty;
                drO1["TransactTime"] = transactTime;
                switch (orderStatus.ToUpper())
                {
                    case "PARTIALLY_FILLED":
                    case "FILLED":
                        drO1["Commission"] = ptrExecutionReport.Commission;
                        drO1["Tax"] = ptrExecutionReport.Tax;
                        drO1["Color"] = Color.White.Name;
                        drO1["OrderStatus"] = orderStatus;
                        break;
                    case "WORKING":
                        drO1["Color"] = Color.DarkGreen.Name;
                        drO1["OrderStatus"] = orderStatus;
                        break;
                    case "PENDING_NEW":
                        drO1["Color"] = Color.LightPink.Name;
                        drO1["OrderStatus"] = orderStatus;
                        break;
                    case "CANCELED":
                        drO1["Color"] = Color.Red.Name;
                        drO1["OrderStatus"] = orderStatus;
                        break;
                    case "REJECTED":
                        drO1["Color"] = Color.LightYellow.Name;
                        drO1["OrderStatus"] = orderStatus;
                        break;
                    default:
                        drO1["Color"] = Color.White.Name;
                        break;
                }

                drO1["Product"] = ptrExecutionReport.Product;
                drO1["ProductType"] = pType;
                drO1["Side"] = side;
                drO1["Gateway"] = ptrExecutionReport.Gateway;
                drO1["StopPx"] = ptrExecutionReport.StopPx.ToString(format);
                drO1["TimeInForce"] = timeInForce;
                if (objInstrumentSpec != null)
                    drO1["TradingCurrency"] = objInstrumentSpec.TradingCurrency;
                drO1["Price"] = ptrExecutionReport.Price.ToString(format);
                drO1["PositionEffect"] = ptrExecutionReport.PositionEffect;
                drO1["LnkOrdID"] = ptrExecutionReport.LinkedOrdID;
                drO1["Profit"] = ptrExecutionReport.Profit.ToString("0.00");
                drO1["SLPrice"] = format;
                drO1["TPPrice"] = format;
                if (positionOpenDS.dtPositionOpen.Rows.Count > 0)
                    positionOpenDS.dtPositionOpen.Rows.InsertAt(drO1, 0);
                else
                    positionOpenDS.dtPositionOpen.Rows.Add(drO1);
                _DDPositionOpenRow.Add(ptrExecutionReport.OrderID, drO1);

                if (pendingOrdersDS.dtPendingOrders.Rows.IndexOf(drO) >= 0)
                {
                    pendingOrdersDS.dtPendingOrders.Rows.Remove(drO);
                }
                _DDPendingOrderRow.Remove(ptrExecutionReport.OrderID);

            }
            if (ptrExecutionReport != null)
            {
                string message = string.Empty;
                Color color = Color.White;
                //#17086 buy 1.00 AUDUSD at 1.04681 successful
                message = "#" + ptrExecutionReport.ExecID + " " + side.ToLower() + " " + ptrExecutionReport.QtyFilled + " " + ptrExecutionReport.Contract + " at " + ptrExecutionReport.LastPx + " successful.";
                DataRow dr = messageLogDS.dtMessageLog.NewRow();
                DateTime dt = DateTime.UtcNow;
                string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);//Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                dr["Time"] = dt;
                dr["MessageType"] = "ORDER RESPONSE";
                dr["Message"] = message;
                dr["Account"] = (int)ptrExecutionReport.Account;
                dr["StrDateTime"] = str;
                dr["Color"] = color.Name;
                if (System.Threading.Monitor.TryEnter(messageLogDS.dtMessageLog, 1000))
                {
                    try
                    {
                        messageLogDS.dtMessageLog.Rows.InsertAt(dr, 0);
                        messageLogDS.dtMessageLog.AcceptChanges();
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(messageLogDS.dtMessageLog);
                    }
                }
                SendMessage(message);
            }
            onPositionOpened();
        }

        public void OnSLModified(string primaryClOrdId, string newClOrdId, decimal price)
        {
            DataRow drO = null;
            if (_DDPositionOpenRow != null && _DDPositionOpenRow.TryGetValue(Convert.ToInt32(primaryClOrdId), out drO))
            {
            }
            if (drO != null)
            {
                int index = positionOpenDS.dtPositionOpen.Rows.IndexOf(drO);
                if (index >= 0)
                {
                    positionOpenDS.dtPositionOpen.Rows[index]["SLClOrdId"] = newClOrdId;
                    string format = "0.";
                    try
                    {
                        InstrumentSpec objInstrumentSpec = Cls.ClsTWSContractManager.INSTANCE.GetContractSpec(positionOpenDS.dtPositionOpen.Rows[index]["Contract"].ToString(), positionOpenDS.dtPositionOpen.Rows[index]["ProductType"].ToString(), positionOpenDS.dtPositionOpen.Rows[index]["Product"].ToString());

                        for (int i = 0; i < objInstrumentSpec.Digits; i++)
                        {
                            format += "0";
                        }
                    }
                    catch
                    {
                        format = "0.00000";
                    }
                    positionOpenDS.dtPositionOpen.Rows[index]["SLPrice"] = price.ToString(format);
                    //SendMessage("#" + newClOrdId + "  Modified successfuly.");
                    onPositionOpened();

                }
                else
                {
                    _DDPositionOpenRow.Remove(Convert.ToInt32(primaryClOrdId));
                }
            }
        }

        public void OnSLWorking(string clOrdId, string slClOrdId, decimal price)
        {
            DataRow drO = null;
            if (_DDPositionOpenRow != null && _DDPositionOpenRow.TryGetValue(Convert.ToInt32(clOrdId), out drO))
            {
            }
            if (drO != null)
            {
                int index = positionOpenDS.dtPositionOpen.Rows.IndexOf(drO);
                if (index >= 0)
                {
                    positionOpenDS.dtPositionOpen.Rows[index]["SLClOrdId"] = slClOrdId;
                    string format = "0.";
                    try
                    {
                        InstrumentSpec objInstrumentSpec = Cls.ClsTWSContractManager.INSTANCE.GetContractSpec(positionOpenDS.dtPositionOpen.Rows[index]["Contract"].ToString(), positionOpenDS.dtPositionOpen.Rows[index]["ProductType"].ToString(), positionOpenDS.dtPositionOpen.Rows[index]["Product"].ToString());
                        for (int i = 0; i < objInstrumentSpec.Digits; i++)
                        {
                            format += "0";
                        }
                    }
                    catch
                    {
                        format = "0.00000";
                    }
                    positionOpenDS.dtPositionOpen.Rows[index]["SLPrice"] = price.ToString(format);
                    SendMessage("#" + slClOrdId + "  Modified successfuly.");
                    onPositionOpened();

                }
                else
                {
                    _DDPositionOpenRow.Remove(Convert.ToInt32(clOrdId));
                }
            }

        }

        public void OnTPModified(string primaryClOrdId, string newClOrdId, decimal price)
        {
            DataRow drO = null;
            if (_DDPositionOpenRow != null && _DDPositionOpenRow.TryGetValue(Convert.ToInt32(primaryClOrdId), out drO))
            {
            }
            if (drO != null)
            {
                int index = positionOpenDS.dtPositionOpen.Rows.IndexOf(drO);
                if (index >= 0)
                {
                    positionOpenDS.dtPositionOpen.Rows[index]["TPClOrdId"] = newClOrdId;
                    InstrumentSpec objInstrumentSpec = Cls.ClsTWSContractManager.INSTANCE.GetContractSpec(positionOpenDS.dtPositionOpen.Rows[index]["Contract"].ToString(), positionOpenDS.dtPositionOpen.Rows[index]["ProductType"].ToString(), positionOpenDS.dtPositionOpen.Rows[index]["Product"].ToString());
                    string format = "0.";
                    for (int i = 0; i < objInstrumentSpec.Digits; i++)
                    {
                        format += "0";
                    }
                    positionOpenDS.dtPositionOpen.Rows[index]["TPPrice"] = price.ToString(format);
                    //SendMessage("#" + newClOrdId + "  Modified successfuly.");
                    onPositionOpened();
                }
                else
                {
                    _DDPositionOpenRow.Remove(Convert.ToInt32(primaryClOrdId));
                }
            }
        }

        public void OnTPWorking(string clOrdId, string tpClOrdId, decimal price)
        {
            DataRow drO = null;
            if (_DDPositionOpenRow != null && _DDPositionOpenRow.TryGetValue(Convert.ToInt32(clOrdId), out drO))
            {
            }
            if (drO != null)
            {
                int index = positionOpenDS.dtPositionOpen.Rows.IndexOf(drO);
                if (index >= 0)
                {
                    positionOpenDS.dtPositionOpen.Rows[index]["TPClOrdId"] = tpClOrdId;
                    SendMessage("#" + tpClOrdId + "  Modified successfuly.");
                    string format = "0.";
                    try
                    {
                        InstrumentSpec objInstrumentSpec = Cls.ClsTWSContractManager.INSTANCE.GetContractSpec(positionOpenDS.dtPositionOpen.Rows[index]["Contract"].ToString(), positionOpenDS.dtPositionOpen.Rows[index]["ProductType"].ToString(), positionOpenDS.dtPositionOpen.Rows[index]["Product"].ToString());

                        for (int i = 0; i < objInstrumentSpec.Digits; i++)
                        {
                            format += "0";
                        }
                    }
                    catch
                    {
                        format = "0.00000";
                    }
                    positionOpenDS.dtPositionOpen.Rows[index]["TPPrice"] = price.ToString(format);

                    onPositionOpened();
                }
            }
            else
            {
                _DDPositionOpenRow.Remove(Convert.ToInt32(clOrdId));
            }
        }

        public void onMailDeliveryResponse(string userName, string body, string subject, string to, string time)
        {
            //throw new NotImplementedException();
        }

        public void onOrderCloseHistoryResponse(List<OrderHistory> lstOrderHistory, bool flag)
        {
            if (lstOrderHistory.Count > 0)
            {
                foreach (OrderHistory OrderHistory in lstOrderHistory)
                {
                    string orderStatus = string.Empty;
                    string productType = string.Empty;
                    string side = string.Empty;
                    string orderType = string.Empty;
                    string timeInForce = string.Empty;
                    string transactTime = string.Empty;
                    transactTime = GetDateTime(OrderHistory.TransactTime);
                    if (ClsGlobal.DDReverseOrderStatus.Keys.Contains(Convert.ToSByte(OrderHistory.OrderStatus)))
                        orderStatus = ClsGlobal.DDReverseOrderStatus[Convert.ToSByte(OrderHistory.OrderStatus)].ToUpper();
                    if (ClsGlobal.DDReverseProductType.Keys.Contains(Convert.ToSByte(OrderHistory.ProductType)))
                        productType = ClsGlobal.DDReverseProductType[Convert.ToSByte(OrderHistory.ProductType)];
                    if (ClsGlobal.DDReverseSide.Keys.Contains(Convert.ToSByte(OrderHistory.Side)))
                        side = ClsGlobal.DDReverseSide[Convert.ToSByte(OrderHistory.Side)];
                    if (ClsGlobal.DDReverseOrderType.Keys.Contains(Convert.ToSByte(OrderHistory.OrderType)))
                        orderType = ClsGlobal.DDReverseOrderType[Convert.ToSByte(OrderHistory.OrderType)];
                    if (ClsGlobal.DDReverseValidity.Keys.Contains(Convert.ToSByte(OrderHistory.TimeInForce)))
                        timeInForce = ClsGlobal.DDReverseValidity[Convert.ToSByte(OrderHistory.TimeInForce)];
                    string pType = string.Empty;
                    switch (productType)
                    {
                        case "EQ":
                            pType = "Equity";
                            break;
                        case "FUT":
                            pType = "FUTURE";
                            break;
                        case "FX":
                            pType = "FOREX";
                            break;
                        case "OPT":
                            pType = "OPTION";
                            break;
                        case "SP":
                            pType = "SPOT";
                            break;
                        case "PH":
                            pType = "PHYSICAL";
                            break;
                        case "AU":
                            pType = "AUCTION";
                            break;
                        case "BON":
                            pType = "BOND";
                            break;
                        default:
                            break;
                    }
                    ;
                    DataRow drO1 = INSTANCE.positionCloseDS.dtPositionClose.NewRow();
                    InstrumentSpec objInstrumentSpec = Cls.ClsTWSContractManager.INSTANCE.GetContractSpec(OrderHistory.Contract, pType, OrderHistory.Product);
                    string format = "0.";
                    for (int i = 0; i < objInstrumentSpec.Digits; i++)
                    {
                        format += "0";
                    }
                    drO1["Account"] = OrderHistory.Account;
                    drO1["Author"] = OrderHistory.author;
                    drO1["AvgPrice"] = OrderHistory.AvgPx.ToString(format);
                    drO1["ClientOrderID"] = OrderHistory.ClOrdId;
                    drO1["Contract"] = OrderHistory.Contract;
                    drO1["CumQty"] = OrderHistory.CumQty;
                    drO1["Industry"] = OrderHistory.industry;
                    drO1["OrderID"] = Convert.ToInt32(OrderHistory.OrderID);
                    drO1["OrderType"] = orderType;
                    drO1["OrderQty"] = OrderHistory.OrdQty;
                    drO1["TransactTime"] = transactTime;
                    switch (orderStatus.ToUpper())
                    {
                        case "PARTIALLY_FILLED":
                        case "FILLED":
                            drO1["Commission"] = OrderHistory.Commission;
                            drO1["Tax"] = OrderHistory.Tax;
                            drO1["Color"] = Color.White.Name;
                            drO1["OrderStatus"] = orderStatus;
                            break;
                        case "WORKING":
                            drO1["Color"] = Color.DarkGreen.Name;
                            drO1["OrderStatus"] = orderStatus;
                            break;
                        case "PENDING_NEW":
                            drO1["Color"] = Color.LightPink.Name;
                            drO1["OrderStatus"] = orderStatus;
                            break;
                        case "CANCELED":
                            drO1["Color"] = Color.Red.Name;
                            drO1["OrderStatus"] = orderStatus;
                            break;
                        case "REJECTED":
                            drO1["Color"] = Color.LightYellow.Name;
                            drO1["OrderStatus"] = orderStatus;
                            break;
                        default:
                            drO1["Color"] = Color.White.Name;
                            break;
                    }

                    drO1["Product"] = OrderHistory.Product;
                    drO1["ProductType"] = pType;
                    drO1["Sector"] = OrderHistory.sector;
                    drO1["Side"] = side;
                    drO1["Gateway"] = OrderHistory.Gateway;
                    drO1["StopPx"] = OrderHistory.StopPx;
                    drO1["TimeInForce"] = timeInForce;
                    if (objInstrumentSpec != null)
                        drO1["TradingCurrency"] = objInstrumentSpec.TradingCurrency;
                    drO1["Price"] = OrderHistory.Price.ToString(format);
                    drO1["PositionEffect"] = OrderHistory.PositionEffect;
                    drO1["ClosedQty"] = OrderHistory.ClosedQuantity;
                    drO1["LnkOrdID"] = OrderHistory.LinkedOrdID;
                    drO1["SLClOrdId"] = OrderHistory.StopLossId;
                    drO1["TPClOrdId"] = OrderHistory.TakeProfitId;
                    drO1["SLPrice"] = OrderHistory.StopLoss.ToString(format);
                    drO1["TPPrice"] = OrderHistory.TakeProfit.ToString(format);
                    drO1["ClosedPrice"] = OrderHistory.ClosePrice.ToString(format);
                    drO1["Profit"] = OrderHistory.Profit.ToString("0.00");
                    if (System.Threading.Monitor.TryEnter(positionCloseDS.dtPositionClose))
                    {
                        try
                        {
                            lock (positionCloseDS.dtPositionClose)
                            {
                                positionCloseDS.dtPositionClose.Rows.Add(drO1);
                            }
                        }
                        finally
                        {
                            System.Threading.Monitor.Exit(positionCloseDS.dtPositionClose);
                        }
                    }
                    if (System.Threading.Monitor.TryEnter(_DDPositionCloseRow))
                    {
                        try
                        {
                            _DDPositionCloseRow.Add(OrderHistory.OrderID, drO1);
                        }
                        finally
                        {
                            System.Threading.Monitor.Exit(_DDPositionCloseRow);
                        }
                    }
                }

            }
        }

        public void onOrderOpenHistoryResponse(List<OrderHistory> lstOrderHistory, bool flag)
        {
            if (lstOrderHistory.Count > 0)
            {
                foreach (OrderHistory OrderHistory in lstOrderHistory)
                {
                    string orderStatus = string.Empty;
                    string productType = string.Empty;
                    string side = string.Empty;
                    string orderType = string.Empty;
                    string timeInForce = string.Empty;
                    string transactTime = string.Empty;
                    transactTime = GetDateTime(OrderHistory.TransactTime);
                    if (ClsGlobal.DDReverseOrderStatus.Keys.Contains(Convert.ToSByte(OrderHistory.OrderStatus)))
                        orderStatus = ClsGlobal.DDReverseOrderStatus[Convert.ToSByte(OrderHistory.OrderStatus)].ToUpper();
                    if (ClsGlobal.DDReverseProductType.Keys.Contains(Convert.ToSByte(OrderHistory.ProductType)))
                        productType = ClsGlobal.DDReverseProductType[Convert.ToSByte(OrderHistory.ProductType)];
                    if (ClsGlobal.DDReverseSide.Keys.Contains(Convert.ToSByte(OrderHistory.Side)))
                        side = ClsGlobal.DDReverseSide[Convert.ToSByte(OrderHistory.Side)];
                    if (ClsGlobal.DDReverseOrderType.Keys.Contains(Convert.ToSByte(OrderHistory.OrderType)))
                        orderType = ClsGlobal.DDReverseOrderType[Convert.ToSByte(OrderHistory.OrderType)];
                    if (ClsGlobal.DDReverseValidity.Keys.Contains(Convert.ToSByte(OrderHistory.TimeInForce)))
                        timeInForce = ClsGlobal.DDReverseValidity[Convert.ToSByte(OrderHistory.TimeInForce)];
                    string pType = string.Empty;
                    switch (productType)
                    {
                        case "EQ":
                            pType = "Equity";
                            break;
                        case "FUT":
                            pType = "FUTURE";
                            break;
                        case "FX":
                            pType = "FOREX";
                            break;
                        case "OPT":
                            pType = "OPTION";
                            break;
                        case "SP":
                            pType = "SPOT";
                            break;
                        case "PH":
                            pType = "PHYSICAL";
                            break;
                        case "AU":
                            pType = "AUCTION";
                            break;
                        case "BON":
                            pType = "BOND";
                            break;
                        default:
                            break;
                    }
                    ;
                    DataRow drO1 = INSTANCE.positionOpenDS.dtPositionOpen.NewRow();
                    InstrumentSpec objInstrumentSpec = Cls.ClsTWSContractManager.INSTANCE.GetContractSpec(OrderHistory.Contract, pType, OrderHistory.Product);
                    string format = "0.";
                    for (int i = 0; i < objInstrumentSpec.Digits; i++)
                    {
                        format += "0";
                    }
                    drO1["Account"] = OrderHistory.Account;
                    drO1["Author"] = OrderHistory.author;
                    drO1["AvgPrice"] = OrderHistory.AvgPx.ToString(format);
                    drO1["ClientOrderID"] = OrderHistory.ClOrdId;
                    drO1["Contract"] = OrderHistory.Contract;
                    drO1["CumQty"] = 0;
                    drO1["Industry"] = OrderHistory.industry;
                    drO1["OrderID"] = Convert.ToInt32(OrderHistory.OrderID);
                    drO1["OrderType"] = orderType;
                    drO1["OrderQty"] = OrderHistory.OrdQty;
                    drO1["TransactTime"] = transactTime;
                    switch (orderStatus.ToUpper())
                    {
                        case "PARTIALLY_FILLED":
                        case "FILLED":
                            drO1["Commission"] = OrderHistory.Commission;
                            drO1["Tax"] = OrderHistory.Tax;
                            drO1["Color"] = Color.White.Name;
                            drO1["OrderStatus"] = orderStatus;
                            break;
                        case "WORKING":
                            drO1["Color"] = Color.DarkGreen.Name;
                            drO1["OrderStatus"] = orderStatus;
                            break;
                        case "PENDING_NEW":
                            drO1["Color"] = Color.LightPink.Name;
                            drO1["OrderStatus"] = orderStatus;
                            break;
                        case "CANCELED":
                            drO1["Color"] = Color.Red.Name;
                            drO1["OrderStatus"] = orderStatus;
                            break;
                        case "REJECTED":
                            drO1["Color"] = Color.LightYellow.Name;
                            drO1["OrderStatus"] = orderStatus;
                            break;
                        default:
                            drO1["Color"] = Color.White.Name;
                            break;
                    }
                    drO1["Product"] = OrderHistory.Product;
                    drO1["ProductType"] = pType;
                    drO1["Sector"] = OrderHistory.sector;
                    drO1["Side"] = side;
                    drO1["Gateway"] = OrderHistory.Gateway;
                    drO1["StopPx"] = OrderHistory.StopPx.ToString(format);
                    drO1["TimeInForce"] = timeInForce;
                    if (objInstrumentSpec != null)
                        drO1["TradingCurrency"] = objInstrumentSpec.TradingCurrency;
                    drO1["Price"] = OrderHistory.Price.ToString(format);
                    drO1["PositionEffect"] = OrderHistory.PositionEffect;
                    drO1["LnkOrdID"] = OrderHistory.LinkedOrdID;
                    drO1["SLClOrdId"] = OrderHistory.StopLossId;
                    drO1["TPClOrdId"] = OrderHistory.TakeProfitId;
                    drO1["SLPrice"] = OrderHistory.StopLoss.ToString(format);
                    drO1["TPPrice"] = OrderHistory.TakeProfit.ToString(format);
                    drO1["ClosedPrice"] = OrderHistory.ClosePrice.ToString(format);
                    if (System.Threading.Monitor.TryEnter(positionOpenDS.dtPositionOpen))
                    {
                        try
                        {
                            lock (positionOpenDS.dtPositionOpen)
                            {
                                positionOpenDS.dtPositionOpen.Rows.Add(drO1);
                                _DDPositionOpenRow.Add(OrderHistory.OrderID, drO1);
                            }
                        }
                        finally
                        {
                            System.Threading.Monitor.Exit(positionOpenDS.dtPositionOpen);
                        }
                    }
                }
            }
        }

        public void onOrderPendingHistoryResponse(List<OrderHistory> lstOrderHistory, bool flag)
        {
            if (lstOrderHistory.Count > 0)
            {
                foreach (OrderHistory orderHistory in lstOrderHistory)
                {
                    string orderStatus = string.Empty;
                    string productType = string.Empty;
                    string side = string.Empty;
                    string orderType = string.Empty;
                    string timeInForce = string.Empty;
                    if (ClsGlobal.DDReverseOrderStatus.Keys.Contains(Convert.ToSByte(orderHistory.OrderStatus)))
                        orderStatus = ClsGlobal.DDReverseOrderStatus[Convert.ToSByte(orderHistory.OrderStatus)].ToUpper();
                    if (ClsGlobal.DDReverseProductType.Keys.Contains(Convert.ToSByte(orderHistory.ProductType)))
                        productType = ClsGlobal.DDReverseProductType[Convert.ToSByte(orderHistory.ProductType)];
                    if (ClsGlobal.DDReverseSide.Keys.Contains(Convert.ToSByte(orderHistory.Side)))
                        side = ClsGlobal.DDReverseSide[Convert.ToSByte(orderHistory.Side)];
                    if (ClsGlobal.DDReverseOrderType.Keys.Contains(Convert.ToSByte(orderHistory.OrderType)))
                        orderType = ClsGlobal.DDReverseOrderType[Convert.ToSByte(orderHistory.OrderType)];
                    if (ClsGlobal.DDReverseValidity.Keys.Contains(Convert.ToSByte(orderHistory.TimeInForce)))
                        timeInForce = ClsGlobal.DDReverseValidity[Convert.ToSByte(orderHistory.TimeInForce)];
                    string reason = orderHistory.Text;
                    DataRow drO = null;
                    string transactTime = string.Empty;
                    transactTime = GetDateTime(orderHistory.TransactTime);
                    string pType = string.Empty;
                    switch (productType)
                    {
                        case "EQ":
                            pType = "Equity";
                            break;
                        case "FUT":
                            pType = "FUTURE";
                            break;
                        case "FX":
                            pType = "FOREX";
                            break;
                        case "OPT":
                            pType = "OPTION";
                            break;
                        case "SP":
                            pType = "SPOT";
                            break;
                        case "PH":
                            pType = "PHYSICAL";
                            break;
                        case "AU":
                            pType = "AUCTION";
                            break;
                        case "BON":
                            pType = "BOND";
                            break;
                        default:
                            break;
                    }
                    ;
                    InstrumentSpec objInstrumentSpec = Cls.ClsTWSContractManager.INSTANCE.GetContractSpec(orderHistory.Contract, pType, orderHistory.Product);
                    string format = "0.";
                    for (int i = 0; i < objInstrumentSpec.Digits; i++)
                    {
                        format += "0";
                    }
                    drO = pendingOrdersDS.dtPendingOrders.NewRow();
                    drO["Account"] = orderHistory.Account;
                    drO["Author"] = orderHistory.author;
                    drO["ClientOrderID"] = orderHistory.ClOrdId;
                    drO["Contract"] = orderHistory.Contract;
                    drO["CumQty"] = 0;
                    drO["Industry"] = orderHistory.industry;
                    drO["OrderID"] = Convert.ToInt32(orderHistory.OrderID);
                    drO["OrderType"] = orderType;
                    drO["OrderQty"] = orderHistory.OrdQty;
                    drO["TransactTime"] = transactTime;
                    switch (orderStatus.ToUpper())
                    {
                        case "PARTIALLY_FILLED":
                        case "FILLED":
                            drO["Commission"] = orderHistory.Commission;
                            drO["Tax"] = orderHistory.Tax;
                            drO["Color"] = Color.White.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "WORKING":
                            drO["Color"] = Color.DarkGreen.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "PENDING_NEW":
                            drO["Color"] = Color.LightPink.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "CANCELED":
                            drO["Color"] = Color.Red.Name;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        case "REJECTED":
                            drO["Color"] = Color.LightYellow.Name;
                            drO["Text"] = reason;
                            drO["OrderStatus"] = orderStatus;
                            break;
                        default:
                            drO["Color"] = Color.White.Name;
                            break;
                    }
                    drO["Product"] = orderHistory.Product;
                    drO["ProductType"] = pType;
                    drO["Sector"] = orderHistory.sector;
                    drO["Side"] = side;
                    drO["Gateway"] = orderHistory.Gateway;
                    drO["StopPx"] = orderHistory.StopPx;
                    drO["TimeInForce"] = timeInForce;
                    drO["PositionEffect"] = orderHistory.PositionEffect;
                    drO["ClosedQty"] = orderHistory.ClosedQuantity;
                    drO["LnkOrdID"] = orderHistory.LinkedOrdID;
                    if (objInstrumentSpec != null)
                        drO["TradingCurrency"] = objInstrumentSpec.TradingCurrency;
                    drO["Price"] = orderHistory.Price.ToString(format);
                    drO["SLClOrdId"] = orderHistory.StopLossId;
                    drO["TPClOrdId"] = orderHistory.TakeProfitId;
                    drO["SLPrice"] = orderHistory.StopLoss.ToString(format);
                    drO["TPPrice"] = orderHistory.TakeProfit.ToString(format);
                    if (System.Threading.Monitor.TryEnter(pendingOrdersDS.dtPendingOrders))
                    {
                        try
                        {
                            lock (pendingOrdersDS.dtPendingOrders)
                            {
                                pendingOrdersDS.dtPendingOrders.Rows.Add(drO);
                                if (!_DDPendingOrderRow.Keys.Contains(orderHistory.OrderID))
                                    _DDPendingOrderRow.Add(orderHistory.OrderID, drO);
                            }
                        }
                        finally
                        {
                            System.Threading.Monitor.Exit(pendingOrdersDS.dtPendingOrders);
                        }
                    }

                }
            }

        }

        public void OnSLCancelled(string clOrdId, string slClOrdId)
        {
            //throw new NotImplementedException();
        }

        public void OnTPCancelled(string clOrdId, string tpClOrdId)
        {
            //DataRow drO = null;
            //if (_DDPositionOpenRow != null && _DDPositionOpenRow.TryGetValue(Convert.ToInt32(ClOrdID), out drO))
            //{
            //}
            //if (drO != null)
            //{
            //    positionOpenDS.dtPositionOpen.Rows.RemoveAt(positionOpenDS.dtPositionOpen.Rows.IndexOf(drO));
            //    _DDPositionOpenRow.Remove(Convert.ToInt32(ClOrdID));
            //}
            //positionOpenDS.dtPositionOpen.AcceptChanges();
        }

        public void onOrderHistoryLastPacketReceived(bool flag)
        {
            if (flag)
            {
                onPendingOrder();
                onPositionOpened();
                onPositionClosed();
            }
        }

        #endregion


        public void Init(string username, string pwd, string serverIp, string hostIp, int port)
        {
            _objOrderManager.UnregisterForEvents(this);
            _objOrderManager.RegisterForEvents(this);
            _objOrderManager.Start();
            string senderId = clsResourceMGR.INSTANCE.resourceManager.GetString("SenderID");
            //string senderId = "55";
            _objOrderManager.Login(username, pwd, serverIp, hostIp, port, senderId);

            //FileHandling.WriteInOutLog("Order Server : Send Connection Request");
            //FileHandling.WriteAllLog("Order Server : Send Connection Request");
        }

        public void SendMail(DateTime time, string subject, string username, string to, string body)
        {
            _objOrderManager.mailRequest(time, subject, username, to, body);
        }

        public void ModifySL(uint account, string clOrdId, string contractName, string productType, sbyte producttype, string productName, DateTime expiryDate, string gateway, double qty, sbyte OrderType, double price, double stopPx, sbyte side, uint OrderID, sbyte tif, uint minOrDisclosedQty, sbyte positionEffect, double transactTime, double slipage, string newClOrdId, string newContractName, string newProductType, sbyte newProducttype, string newProductName, DateTime newExpiryDate, string newGateway, double newQty, sbyte newOrderType, double newprice, double newstopPx, sbyte newside, sbyte newtif, double newminOrDisclosedQty, sbyte newpositionEffect, double newtransactTime, double newSlipage, double oldStopLoss, double newStopLoss, string origClOrdId, string lnkdOrdId)
        {
            _objOrderManager.modifySLOrder(account, clOrdId, contractName, productType, producttype, productName, expiryDate, gateway, qty, OrderType, price, stopPx, side, OrderID, tif, minOrDisclosedQty, positionEffect, transactTime, slipage, newClOrdId, newContractName, newProductType, newProducttype, newProductName, newExpiryDate, newGateway, newQty, newOrderType, newprice, newstopPx, newside, newtif, newminOrDisclosedQty, newpositionEffect, newtransactTime, newSlipage, oldStopLoss, newStopLoss, origClOrdId, lnkdOrdId);
        }

        public void ModifyTP(uint Account, string ClOrdId, string ContractName, string ProductType, sbyte Producttype, string ProductName, DateTime ExpiryDate, string Gateway, double Qty, sbyte OrderType, double price, double stopPx, sbyte side, uint OrderID, sbyte tif, uint minOrDisclosedQty, sbyte positionEffect, double transactTime, double Slipage, string NewClOrdId, string NewContractName, string NewProductType, sbyte NewProducttype, string NewProductName, DateTime NewExpiryDate, string NewGateway, double NewQty, sbyte NewOrderType, double Newprice, double NewstopPx, sbyte Newside, sbyte Newtif, double NewminOrDisclosedQty, sbyte NewpositionEffect, double NewtransactTime, double NewSlipage, double OldTakeProfit, double NewTakeProfit, string origClOrdId, string LnkdOrdId)
        {
            _objOrderManager.modifyTPOrder(Account, ClOrdId, ContractName, ProductType, Producttype, ProductName, ExpiryDate, Gateway, Qty, OrderType, price, stopPx, side, OrderID, tif, minOrDisclosedQty, positionEffect, transactTime, Slipage, NewClOrdId, NewContractName, NewProductType, NewProducttype, NewProductName, NewExpiryDate, NewGateway, NewQty, NewOrderType, Newprice, NewstopPx, Newside, Newtif, NewminOrDisclosedQty, NewpositionEffect, NewtransactTime, NewSlipage, OldTakeProfit, NewTakeProfit, origClOrdId, LnkdOrdId);
        }

        public List<string> GetLeverageList()
        {
            if (_DDLeverage != null && _DDLeverage.Keys.Count > 0)
            {
                return _DDLeverage.Values.ToList();
            }
            TradeAccount _objTradeAccount = new TradeAccount();
            _DDLeverage = _objTradeAccount.GetLeverageList();
            if (_DDLeverage == null)
                return null;

            return _DDLeverage.Values.ToList();
        }

        public List<string> GetCountryList()
        {
            if (_DDCountry != null && _DDCountry.Keys.Count > 0)
            {
                return _DDCountry.Values.ToList();
            }
            TradeAccount _objTradeAccount = new TradeAccount();
            _DDCountry = _objTradeAccount.GetCountryList();
            if (_DDCountry == null)
                return null;
            return _DDCountry.Values.ToList();
        }

        private int? GetLeverageId(string leverageName)
        {
            return _DDLeverage.FirstOrDefault(a => a.Value == leverageName).Key;
        }

        private int? GetCountryKey(string conuntryName)
        {
            return _DDCountry.FirstOrDefault(a => a.Value == conuntryName).Key;
        }

        internal TradeAccount.LoginCredent CreateDemoAccount(string firstName, decimal balence, string country, string email, string leverage, string currencyName, bool isTradeEnable)
        {
            TradeAccount.PersonalInfo objPersonalInfo = new TradeAccount.PersonalInfo();

            //BOChanges
            //objPersonalInfo.firstName = firstName;
            ////objPersonalInfo.middleName = firstName;
            ////objPersonalInfo.lastName = firstName;
            //objPersonalInfo.FKAccountTypeID = 1;
            //objPersonalInfo.balanace = balence;
            //objPersonalInfo.FKAccountGroupID = Convert.ToInt32(clsResourceMGR.INSTANCE.resourceManager.GetString("AccountGroupID"));
            //objPersonalInfo.WhiteLevelName = clsResourceMGR.INSTANCE.resourceManager.GetString("WhiteLevelName");
            //objPersonalInfo.Gender = "Male";
            //objPersonalInfo.FKCountryID = GetCountryKey(country);
            //objPersonalInfo.FK_NationalityID = GetCountryKey(country);
            ////objPersonalInfo.Address1 = PIAddress1;
            ////objPersonalInfo.Zip = ZipCode;
            ////objPersonalInfo.City = City;
            ////objPersonalInfo.State = State;
            //objPersonalInfo.primaryEmailAddress = email;
            ////objPersonalInfo.PrimaryPhone = Phone;
            ////objPersonalInfo.Mobile = Mobile;
            //objPersonalInfo.fkleverage = GetLeverageId(leverage);
            //objPersonalInfo.FK_Currency = 2;
            //objPersonalInfo.isTradeEnable = isTradeEnable;
            //objPersonalInfo.bankID = 2;
            //objPersonalInfo.buySideTurnOver = 0;
            //objPersonalInfo.ClientId = 2;
            //objPersonalInfo.deleted = false;
            //objPersonalInfo.DOB = DateTime.UtcNow;
            //objPersonalInfo.equity = 0;
            //objPersonalInfo.hedgeTypeID = 2;
            //objPersonalInfo.FKParticipantType = 1;
            //objPersonalInfo.isHeadgingAllowed = false;
            //objPersonalInfo.IsLive = false;
            //objPersonalInfo.MarkUpValue = 0;
            //objPersonalInfo.participantId = 0;
            //objPersonalInfo.PKAccountID = 0;
            //objPersonalInfo.registrationDate = DateTime.UtcNow;
            //objPersonalInfo.sellSideTurnOver = 0;
            //objPersonalInfo.status = false;
            //objPersonalInfo.usedMargin = 0;

            //objPersonalInfo.IP_Name = clsResourceMGR.INSTANCE.resourceManager.GetString("LPName"); //This is actually LP Name
            ////objPersonalInfo.WhiteLevelName = "JAGS";

            TradeAccount _objTradeAccount = new TradeAccount();
            TradeAccount.LoginCredent objLogin = _objTradeAccount.InsertPersonalInfo(objPersonalInfo);
            return objLogin;
        }

        public List<MailData> GetMailBoxInfo(string user, string pwd, int participantId, DateTime todate, DateTime fromDate)
        {
            List<MailData> lst = new List<MailData>();
            //lst = obj.GetMailBoxInfo("daw", "dawish1", 2, Convert.ToDateTime("1970-01-01 00:00:00.000"), Convert.ToDateTime("2013-01-01 00:00:00.000"));
            lst = obj.GetMailBoxInfo(user, pwd, participantId, todate, fromDate);
            return lst;
        }
        public void Refresh()
        {
            PosContracts.Clear();
            _flagFirstParticipantListResponce = false;
            _DDAccountInfo.Clear();
            _DDAccounts.Clear();
            _DDMessages.Clear();
            _DDNetPos.Clear();
            _DDNetPosRow.Clear();
            _DDOrderRow.Clear();
            _DDTradeRow.Clear();
            _DDPositionOpenRow.Clear();
            _DDPendingOrderRow.Clear();
            _DDPositionCloseRow.Clear();
            positionCloseDS.dtPositionClose.Rows.Clear();
            pendingOrdersDS.dtPendingOrders.Rows.Clear();
            positionOpenDS.dtPositionOpen.Rows.Clear();
            //onPositionClosed();
            //onPositionOpened();
            //onPendingOrder();
            messageLogDS.dtMessageLog.Rows.Clear();
            messageLogDS.dtMessageLog.AcceptChanges();
            accountInfoDS.dtAccountInfo.Rows.Clear();
            accountInfoDS.dtAccountInfo.AcceptChanges();
            netpositionDS.dtNetPosition.Rows.Clear();
            netpositionDS.dtNetPosition.AcceptChanges();
            orderHistoryDS.dtOrderHistory.Rows.Clear();
            orderHistoryDS.dtOrderHistory.AcceptChanges();
            tradeHistoryDS.dtTradeHistory.Rows.Clear();
            orderHistoryDS.dtOrderHistory.AcceptChanges();

        }

        private void LogOrderStatus(string x)
        {
            //try
            //{
            SetOrderLog(x);
            //}
            //catch (Exception ex)
            //{
            //    //throw new Exception(ex.Message + " : " + ex.InnerException);
            //}
        }

        private void InvokeOrderResponseEvent(ExecutionReport ptrExecutionReport)
        {
            if (OnOrderResponse != null)
            {
                OnOrderResponse(ptrExecutionReport);

                ////FileHandling.WriteInOutLog("Order Server : Received Order Response of Account" +
                //ptrExecutionReport.Account + " Contract" +
                //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
                //" ProductType" + ptrExecutionReport.ProductType
                //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
                //ptrExecutionReport.ExpireDate
                //+ " GatewayName" + ptrExecutionReport.Gateway
                //+ " Quantity" + ptrExecutionReport.OrdQty
                //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
                //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
                //" Side" + ptrExecutionReport.Side + " TIF" +
                //ptrExecutionReport.TimeInForce
                //+ " Transaction Time" + ptrExecutionReport.TransactTime);
                ////FileHandling.WriteAllLog("Order Server : Received Order Response of Account" +
                //ptrExecutionReport.Account + " Contract" +
                //ptrExecutionReport.Contract + " OrderId" + ptrExecutionReport.ClOrdId +
                //" ProductType" + ptrExecutionReport.ProductType
                //+ " ProductName" + ptrExecutionReport.Product + " ExpiryDate" +
                //ptrExecutionReport.ExpireDate
                //+ " GatewayName" + ptrExecutionReport.Gateway
                //+ " Quantity" +
                //ptrExecutionReport.OrdQty
                //+ " OrderType" + ptrExecutionReport.OrderType + " Price" +
                //ptrExecutionReport.Price + " StopPX" + ptrExecutionReport.StopPx +
                //" Side" + ptrExecutionReport.Side + " TIF" +
                //ptrExecutionReport.TimeInForce
                //+ " Transaction Time" + ptrExecutionReport.TransactTime);
            }
        }

        public List<string> GetParticipants()
        {
            var lstAccountIds = new List<int>();
            if (_DDAccounts != null && _DDAccounts.Count > 0)
            {
                lstAccountIds.AddRange(_DDAccounts.Keys);
            }
            List<string> lstAccountNos = lstAccountIds.ConvertAll<string>(delegate(int i) { return i.ToString(); });
            return lstAccountNos;
        }

        public Dictionary<int, DataRow> GetParticipantsList()
        {
            return _DDAccounts;
        }

        private void SetOrderLog(string msg)
        {
            if (OnOrderLog != null)
            {
                OnOrderLog(msg);
            }
        }

        public bool SubmitNewOrder(ClsNewOrder newOrder)
        {
            bool flag = false;
            //BOChanges if (Convert.ToUInt32(Properties.Settings.Default.DefaultTradingAccount) != 0 && IsOrderMgrLoaded && ClsTWSDataManager.INSTANCE.IsDataMgrConnected)
            {

                string oriClOrdID = string.Empty;
                string LnkdOrdId = string.Empty;
                string message = string.Empty;
                string side = PALSA.Cls.ClsGlobal.DDReverseSide[Convert.ToSByte(newOrder.Side)];
                string orderType = PALSA.Cls.ClsGlobal.DDReverseOrderType[Convert.ToSByte(newOrder.OrderType)];
                _objOrderManager.placeOrder
                    (newOrder.Account, newOrder.ClOrderID, newOrder.ContractName,
                                           newOrder.StrProductType, newOrder.SbProductType, newOrder.ProductName,
                                           newOrder.ExpiryDate, newOrder.GatewayName, newOrder.Qty,
                                           newOrder.OrderType, newOrder.Price, newOrder.StopPX,
                                           newOrder.Side, newOrder.Tif, newOrder.MinorDisclosedQty,
                                           newOrder.PositionEffect, DateTime.UtcNow.ToOADate(), newOrder.Slipage, oriClOrdID, LnkdOrdId, newOrder.StopLoss, newOrder.TakeProfit);

                message = "Your New Order of " + side + " " + orderType + " From Account >" + Convert.ToString(newOrder.Account) + " for Symbol > " + newOrder.ContractName + " Qty > " + newOrder.Qty + " Price > " + newOrder.Price + " is submited .";

                DataRow dr1 = ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.NewRow();
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
                dr1["Time"] = dt;
                dr1["MessageType"] = "NEW ORDER REQUEST";
                dr1["Message"] = message;
                dr1["Account"] = 0;
                dr1["StrDateTime"] = str;
                dr1["Color"] = "White";
                if (System.Threading.Monitor.TryEnter(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog, 1000))
                {
                    try
                    {
                        ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr1, 0);
                        ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.AcceptChanges();
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog);
                    }
                }

                flag = true;

                //FileHandling.WriteInOutLog("Order Server : Send New Order From Account > " + newOrder.Account +
                //                       " for Contract > " + newOrder.ContractName +
                //                       " ProductType > " + newOrder.StrProductType
                //                       + " ProductName > " + newOrder.ProductName + " GatewayName > " + newOrder.GatewayName
                //                       + " Quantity > " + newOrder.Qty
                //                       + " OrderType > " + orderType + " Price > " + newOrder.Price + " SL > " + newOrder.StopLoss + " TP> " + newOrder.TakeProfit +
                //                       " StopPX > " + newOrder.StopPX +
                //                       " Side > " + side +
                //                       " DateTime > " + DateTime.UtcNow.ToShortDateString()
                //);
                //FileHandling.WriteAllLog("Order Server : Send New Order Account" + newOrder.Account +
                //" for Contract > " + newOrder.ContractName +
                //                       " ProductType > " + newOrder.StrProductType
                //                       + " ProductName > " + newOrder.ProductName + " GatewayName > " + newOrder.GatewayName
                //                       + " Quantity > " + newOrder.Qty
                //                       + " OrderType > " + orderType + " Price > " + newOrder.Price + " SL > " + newOrder.StopLoss + " TP> " + newOrder.TakeProfit +
                //                       " StopPX > " + newOrder.StopPX +
                //                       " Side > " + side +
                //                       " DateTime > " + DateTime.UtcNow.ToShortDateString()
                //);
            }
            //else
            //{
            //    //ClsCommonMethods.ShowInformation("Order server or Quote server Disconnected New Order request failed .");
            //}
            return flag;

        }

        public void CloseOrder(uint account, string clOrderId, string contractName, string productType,
                        sbyte producttype, string productName, DateTime expiryDate,
                        string gatewayName, string origClOrdId, sbyte Side, double orderQty, sbyte OrderType,
                        double price, double stopPx, sbyte tif, int minorDisclosedQty, sbyte positionEffect,
                        double transactTime, double slipage, string closeClOrdId, string closeOrderId, bool ocOrder)
        {
            if (IsOrderMgrLoaded && ClsTWSDataManager.INSTANCE.IsDataMgrConnected)
            {

                string LnkdOrdId = string.Empty;
                string message = string.Empty;
                string side = PALSA.Cls.ClsGlobal.DDReverseSide[Side];
                string orderType = "MARKET"; //PALSA.Cls.ClsGlobal.DDReverseOrderType[Convert.ToSByte(OrderType)];
                _objOrderManager.closePosition(account, clOrderId, contractName, productType, producttype, productName,
                                            expiryDate, gatewayName, orderQty, OrderType, price, stopPx, Side, tif, 0, positionEffect, transactTime, slipage,
                                            origClOrdId, clOrderId, closeClOrdId, closeOrderId, ocOrder);

                //_objOrderManager.cancelOrder(account, clOrderID, contractName, productType, producttype, productName,
                //                            expiryDate, gatewayName, orderId, Side, orderQty, OrderType, price,
                //                            stopPx, tif, 0, positionEffect, transactTime);
                message = "Your Request to Close " + side + " " + orderType + " Order From Account >" + Convert.ToString(account) + " for Symbol > " + contractName + " Qty > " + orderQty + " Price > " + price + " is submited .";

                DataRow dr1 = ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.NewRow();
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
                dr1["Time"] = dt;
                dr1["MessageType"] = "ORDER CANCEL REQUEST";
                dr1["Message"] = message;
                dr1["Account"] = 0;
                dr1["StrDateTime"] = str;
                dr1["Color"] = "White";
                if (System.Threading.Monitor.TryEnter(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog, 1000))
                {
                    try
                    {
                        ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr1, 0);
                        ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.AcceptChanges();
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog);
                    }
                }
                //BOChanges
                ////FileHandling.WriteInOutLog("Order Server : Send Order Close Request for OrderID > " + clOrderId +
                //                       " Contract > " +
                //                       contractName + " ProductType > " + productType + " ProductName > " +
                //                       productName + " ExpiryDate > " + expiryDate +
                //                       " GatewayName > " + gatewayName +
                //                       " OrderID > " + origClOrdId + " Side > " + side
                //);
                ////FileHandling.WriteAllLog("Order Server : Send Order Close Request OrderID > " + clOrderId +
                //                     " Contract > " +
                //                     contractName + " ProductType > " + productType + " ProductName > " +
                //                     productName + " ExpiryDate > " + expiryDate +
                //                     " GatewayName > " + gatewayName +
                //                     " OrderID > " + origClOrdId + " Side > " + side
                //);
            }
            else
            {
                ////BOChanges ClsCommonMethods.ShowInformation("Order server or Quote Server disconnected Canceling order failed .");
            }
        }

        public void CancelOrder(uint account, string clOrderId, string contractName, string productType,
                                sbyte producttype, string productName, DateTime expiryDate,
                                string gatewayName, uint orderId, int Side, double orderQty, sbyte OrderType,
                                double price, double stopPx, sbyte tif, int minorDisclosedQty, sbyte positionEffect,
                                double transactTime, double slipage)
        {
            if (IsOrderMgrLoaded && ClsTWSDataManager.INSTANCE.IsDataMgrConnected)
            {
                string message = string.Empty;
                string side = PALSA.Cls.ClsGlobal.DDReverseSide[Convert.ToSByte(Side)];
                string orderType = PALSA.Cls.ClsGlobal.DDReverseOrderType[Convert.ToSByte(OrderType)];
                _objOrderManager.cancelOrder(account, clOrderId, contractName, productType, producttype, productName,
                                            expiryDate, gatewayName, orderId, Side, orderQty, OrderType, price,
                                            stopPx, tif, 0, positionEffect, transactTime, slipage, "", "");
                message = "Your Request to Cancel " + side + " " + orderType + " Order From Account >" + Convert.ToString(account) + " for Symbol > " + contractName + " Qty > " + orderQty + " Price > " + price + " is submited .";

                DataRow dr1 = ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.NewRow();
                DateTime dt = DateTime.UtcNow;
                string str;
                //BOChanges
                //if (Properties.Settings.Default.TimeFormat.Contains("24"))
                //{
                //    str = string.Format("{0:HH:mm:ss tt dd/MM/yyyy}", dt);
                //}
                //else
                {
                    str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                }
                dr1["Time"] = dt;
                dr1["MessageType"] = "ORDER CANCEL REQUEST";
                dr1["Message"] = message;
                dr1["Account"] = 0;
                dr1["StrDateTime"] = str;
                dr1["Color"] = "White";
                if (System.Threading.Monitor.TryEnter(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog, 1000))
                {
                    try
                    {
                        ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr1, 0);
                        ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog.AcceptChanges();
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(ClsTWSOrderManager.INSTANCE.messageLogDS.dtMessageLog);
                    }
                }
                //BOChanges
                ////FileHandling.WriteInOutLog("Order Server : Send Order Cancel Request for OrderID > " + clOrderId +
                //                       " Contract > " +
                //                       contractName + " ProductType > " + productType + " ProductName > " +
                //                       productName + " ExpiryDate > " + expiryDate +
                //                       " GatewayName > " + gatewayName +
                //                       " OrderID > " + orderId + " Side > " + side
                //);
                ////FileHandling.WriteAllLog("Order Server : Send Order Cancel Request OrderID > " + clOrderId +
                //                     " Contract > " +
                //                     contractName + " ProductType > " + productType + " ProductName > " +
                //                     productName + " ExpiryDate > " + expiryDate +
                //                     " GatewayName > " + gatewayName +
                //                     " OrderID > " + orderId + " Side > " + side
                //);
            }
            else
            {
                //ClsCommonMethods.ShowInformation("Order server or Quote Server disconnected Canceling order failed .");
            }
        }




        public void Close()
        {
            if (_objOrderManager != null)
                _objOrderManager.Dispose();
            //BOChanges
            ////FileHandling.WriteInOutLog("Order Server : Send DisConnection Request");
            ////FileHandling.WriteAllLog("Order Server : Send DisConnection Request");
        }

        public void GetOrderHistory(int accountNo)
        {
            _objOrderManager.getOrderHistoryResponse((uint)accountNo);
        }

        internal void RequestForNetPositionOfAccount(int accountno, bool check)
        {
            if (check)
            {
                netpositionDS.dtNetPosition.Clear();
                _DDNetPosRow.Clear();
            }
            switch (accountno)
            {
                case 0:
                    foreach (string account in _DDAccountInfo.Keys)
                    {
                        _objOrderManager.getPositionResponse(Convert.ToUInt32(account));
                    }
                    break;
                default:
                    _objOrderManager.getPositionResponse(Convert.ToUInt32(accountno));
                    break;
            }
        }

        internal void RequestForOrderHistoryOfAccount(int accountno)
        {

            if (System.Threading.Monitor.TryEnter(orderHistoryDS.dtOrderHistory, 1000))
            {
                try
                {
                    orderHistoryDS.dtOrderHistory.Clear();
                    _DDOrderRow.Clear();
                    orderHistoryDS.dtOrderHistory.AcceptChanges();
                    OnOrderHistoryResponse();
                }
                finally
                {
                    System.Threading.Monitor.Exit(orderHistoryDS.dtOrderHistory);
                }
            }
            if (accountno == 0)
            {
                foreach (int account in _DDAccounts.Keys.Where(account => _objOrderManager != null))
                {
                    _objOrderManager.getOrderHistoryResponse(Convert.ToUInt32(account));
                }
            }
            else
            {
                if (_objOrderManager != null) _objOrderManager.getOrderHistoryResponse(Convert.ToUInt32(accountno));
            }

        }

        internal bool ModifyOrder(ClsNewOrder objclsNewOrder, ClsNewOrder objclsOldOrder)
        {
            if (IsOrderMgrLoaded && ClsTWSDataManager.INSTANCE.IsDataMgrConnected)
            {
                string message = string.Empty;
                string side = PALSA.Cls.ClsGlobal.DDReverseSide[Convert.ToSByte(objclsOldOrder.Side)];
                string orderType = PALSA.Cls.ClsGlobal.DDReverseOrderType[Convert.ToSByte(objclsOldOrder.OrderType)];
                Color color = Color.White;
                _objOrderManager.cancelReplaceOrder(objclsOldOrder.Account, objclsOldOrder.ClOrderID,
                                                   objclsOldOrder.ContractName, objclsOldOrder.StrProductType,
                                                   objclsOldOrder.SbProductType, objclsOldOrder.ProductName,
                                                   objclsOldOrder.ExpiryDate,
                                                   objclsOldOrder.GatewayName,
                                                   objclsOldOrder.Qty,
                                                   objclsOldOrder.OrderType, objclsOldOrder.Price, objclsOldOrder.StopPX,
                                                   objclsOldOrder.Side, objclsOldOrder.OrderID,
                                                   objclsOldOrder.Tif, objclsOldOrder.MinorDisclosedQty,
                                                   objclsOldOrder.PositionEffect, clsUtility.GetDoubleDate(DateTime.UtcNow), objclsOldOrder.Slipage,
                                                   objclsNewOrder.ClOrderID, objclsNewOrder.ContractName,
                                                   objclsNewOrder.StrProductType, objclsNewOrder.SbProductType,
                                                   objclsNewOrder.ProductName, objclsNewOrder.ExpiryDate,
                                                   objclsNewOrder.GatewayName,
                                                   objclsNewOrder.Qty, objclsNewOrder.OrderType, objclsNewOrder.Price,
                                                   objclsNewOrder.StopPX, objclsNewOrder.Side,
                                                   objclsNewOrder.Tif, objclsNewOrder.MinorDisclosedQty,
                                                   objclsNewOrder.PositionEffect, clsUtility.GetDoubleDate(DateTime.UtcNow), objclsNewOrder.Slipage, "", "");
                message = "Your " + side + " " + orderType + " Order From Account >" + Convert.ToString(objclsOldOrder.Account) + " with OrderID > " + objclsOldOrder.OrderID + " for Symbol > " + objclsOldOrder.ContractName + " Qty > " + objclsOldOrder.Qty + " Price > " + objclsOldOrder.Price + " is requested to Modify with Qty > " + objclsNewOrder.Qty + " Price > " + objclsNewOrder.Price + " .";

                DataRow dr1 = INSTANCE.messageLogDS.dtMessageLog.NewRow();
                DateTime dt = DateTime.UtcNow;
                string str = string.Format("{0:hh:mm:ss tt dd/MM/yyyy}", dt);//BOChanges Properties.Settings.Default.TimeFormat.Contains("24") ? "{0:HH:mm:ss tt dd/MM/yyyy}" : "{0:hh:mm:ss tt dd/MM/yyyy}", dt);
                dr1["Time"] = dt;
                dr1["MessageType"] = "ORDER MODIFY REQUEST";
                dr1["Message"] = message;
                dr1["Account"] = 0;
                dr1["StrDateTime"] = str;
                dr1["Color"] = color.Name;
                if (System.Threading.Monitor.TryEnter(INSTANCE.messageLogDS.dtMessageLog, 1000))
                {
                    try
                    {
                        INSTANCE.messageLogDS.dtMessageLog.Rows.InsertAt(dr1, 0);
                        INSTANCE.messageLogDS.dtMessageLog.AcceptChanges();
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(INSTANCE.messageLogDS.dtMessageLog);
                    }
                }

                return true;
            }
            else
            {
                //BOChanges ClsCommonMethods.ShowInformation("Order server or Quote Server disconnected Order modification failed.");
                return false;
            }
        }

        public void ChangePassword(string userName, string oldMpassword, string newMPassword, string newTpassword)
        {
            _objOrderManager.changePassword(userName, oldMpassword, newMPassword, newTpassword);
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
                if (dtx != DateTime.MinValue)
                {
                    date = string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", dtx);
                                //kulboProperties.Settings.Default.TimeFormat.Contains("24")
                                    //? "{0:dd/MM/yyyy HH:mm:ss}"
                                    //: "{0:dd/MM/yyyy hh:mm:ss tt}", dtx);
                }
                return date;
            }
            else
                return datetime;
        }


        internal void SendMessageForStatus(string msg)
        {
            SendMessage(msg);
        }


        public void onAccountUpdateResponse(uint AccountNo, int ErrNo)
        {

        }

        #region IOrderManager Members


        //public void onLogonResponse(string UserName, string Reason, string BrokerName, string AccountType, bool IsLive)
        //{
        //    throw new NotImplementedException();
        //}

        public void onLogoutResponse(string UserName)
        {
            //throw new NotImplementedException();
        }

        //public void onParticipantResponse(string pstrUserName, List<AccountInfo> lstAccountInfo, bool flag)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}