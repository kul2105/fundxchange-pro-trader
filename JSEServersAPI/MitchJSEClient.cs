using System;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.IO;
using InsightCapital.STTAPI.Sockets.Classes;
using InsightCapital.STTAPI.MessageLibrary;
using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;
using InsightCapital.STTAPI.MessageLibrary.Utility;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace JSEServersAPI
{
    // public event Action<SymbolDirectoryMessage> OnSymbolDirectoryResponse = delegate { };
    //MitchJSEClient
    public class MitchJSEClient
    {
        public Client objClient = new Client(DataType.BinaryType);
        TimerCallback cbPingServer;
        Timer tmrPingServer;
        public DateTime LastHeartBeatTime = DateTime.UtcNow;

        TimerCallback cbPingStatus;
        Timer tmrCheckPingStatus;
        double HeartbeatInterval = 30;


        IPAddress ServerIPAddress;
        int ServerPort;
        string UserName, Password;
        //Client24x7 obj24x7;
        //bool IsConnected;

        public bool IsConnected
        {
            get { return objClient.Connected; }
        }
        //public TimeSpan TotalElapsedTime;
        public event Action<SymbolMasterResponse> OnSymbolDirectoryResponse = delegate { };
        public event Action<SymbolData> OnQuoteResponse = delegate { };
        public event Action<Level2Quotes> OnLevel2Response = delegate { };
        public event Action<TradeMessage> OnTradeResponse = delegate { };

        public void Start(string User, string Pwd)
        {
            UserName = User;
            Password = Pwd;
            StreamReader re = File.OpenText("ClientSetting.txt");
            string strData = "";
            while ((strData = re.ReadLine()) != null)
            {
                string[] str = strData.Split(',');
                if (strData.StartsWith("IPAddress"))
                    ServerIPAddress = IPAddress.Parse(str[1]);
                else if (strData.StartsWith("Port"))
                    ServerPort = Convert.ToInt32(str[1]);
            }
            re.Close();
            objClient.DataReceived += new Client.DataReceivedHandler(objClient_DataReceived);
            objClient.OnDisconnection += new Client.OnDisconnectHandler(objClient_OnDisconnection);
            objClient.OnException += new Client.OnExceptionHandler(objClient_OnException);
            LoginRequestPS objLoginRequest = new LoginRequestPS();
            objLoginRequest.LoginRequest_.UserName_ = UserName;
            objLoginRequest.LoginRequest_.PassWord_ = Password;
            objLoginRequest.LoginRequest_.SenderCompID_ = "ABC";
            objLoginRequest.LoginRequest_.TargetCompID_ = "ABCC";

            cbPingStatus = new TimerCallback(CheckPingStatusCallBack);
            tmrCheckPingStatus = new Timer(cbPingStatus, null, 0, 2000);

            try
            {
                RegisterStructures();
                objClient.Connect(ServerIPAddress, ServerPort);
                objClient.SendStruct(objLoginRequest, () => { });
                Console.WriteLine("Connected");
                objClient.StartReceiving();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                Console.WriteLine(ex.ToString());
            }
        }

        void RegisterStructures()
        {
            objClient.RegisterStructure(ProtocolStructIDS.HeartBeat_ID, typeof(HeartBeatPS));
            objClient.RegisterStructure(ProtocolStructIDS.Login_RequestPS_ID, typeof(LoginRequestPS));
            objClient.RegisterStructure(ProtocolStructIDS.Login_ResponsePS_ID, typeof(LoginResponsePS));
            objClient.RegisterStructure(ProtocolStructIDS.Quote_Request_ID, typeof(QuoteRequestPS));
            objClient.RegisterStructure(ProtocolStructIDS.Quote_ResponsePS_ID, typeof(QuoteResponsePS));
            objClient.RegisterStructure(ProtocolStructIDS.SymbolMasterResponsePS_ID, typeof(SymbolMasterResponsePS));
            objClient.RegisterStructure(ProtocolStructIDS.TradeResponsePS_ID, typeof(TradeResponsePS));
            objClient.RegisterStructure(ProtocolStructIDS.Level2QuotesResponsePS_ID, typeof(Level2QuotesResponsePS));
            objClient.RegisterStructure(ProtocolStructIDS.Level2QuotesRequestPS_ID, typeof(Level2QuoteRequestPS));
        }

        void objClient_OnException(Client sender, Exception exception, bool? okToContinue)
        {
            objClient.Dispose();
        }

        void objClient_OnDisconnection(Client sender, Exception exception, bool isDisposing)
        {
            objClient.Dispose();
        }

        void objClient_DataReceived(Client sender, IProtocolStruct structure)
        {
            switch (structure.ID)
            {
                case ProtocolStructIDS.SymbolMasterResponsePS_ID:
                    DisplayMessage(sender, (SymbolMasterResponsePS)structure);
                    break;
                case ProtocolStructIDS.Quote_ResponsePS_ID:
                    DisplayMessage(sender, (QuoteResponsePS)structure);
                    break;
                case ProtocolStructIDS.HeartBeat_ID:
                    DS_DOHandleHeartBeat(sender, structure as HeartBeatPS);
                    break;
                case ProtocolStructIDS.Login_ResponsePS_ID:
                    DisplayMessage(sender, (LoginResponsePS)structure);
                    break;
                case ProtocolStructIDS.TradeResponsePS_ID:
                    DisplayMessage(sender, (TradeResponsePS)structure);
                    break;
                case ProtocolStructIDS.Level2QuotesResponsePS_ID:
                    DisplayMessage(sender, (Level2QuotesResponsePS)structure);
                    break;
            }
        }

        private void DisplayMessage(Client Sender, Level2QuotesResponsePS structure)
        {
            //Console.WriteLine(structure.level2Quotes.ToString());
            LastHeartBeatTime = DateTime.UtcNow;
            if (OnLevel2Response != null)
                OnLevel2Response(structure.level2Quotes);
        }

        private void DisplayMessage(Client Sender, TradeResponsePS structure)
        {
            //Console.WriteLine(structure.response_.ToString());
            LastHeartBeatTime = DateTime.UtcNow;
            if (OnTradeResponse != null)
                OnTradeResponse(structure.response_);
        }

        private void DisplayMessage(Client Sender, SymbolMasterResponsePS structure)
        {
            //Console.WriteLine(structure.SymbolResponse.ToString());
            LastHeartBeatTime = DateTime.UtcNow;
            if (OnSymbolDirectoryResponse != null)
                OnSymbolDirectoryResponse(structure.SymbolResponse);

            //QuoteRequestPS quoteRequest = new QuoteRequestPS();
            //quoteRequest.QuoteRequest.instrumentID = structure.SymbolResponse.InstrumentId;
            //quoteRequest.QuoteRequest.ClientID = objClient.Id;
            //objClient.SendStruct(quoteRequest, () => { });
        }

        private void DisplayMessage(Client Sender, QuoteResponsePS structure)
        {
            //Console.WriteLine(structure.symbolData.ToString());
            LastHeartBeatTime = DateTime.UtcNow;
            if (OnQuoteResponse != null)
                OnQuoteResponse(structure.symbolData);
        }

        private void DisplayMessage(Client Sender, LoginResponsePS structure)
        {
            Console.WriteLine(structure.LoginResponse_.ToString());
            if (structure.LoginResponse_.AuthenticationStatus == Authentication.Authenticated)
            {
                objClient.Id = structure.LoginResponse_.AuthenticationID;
                cbPingServer = new TimerCallback(PongServerCallBack);
                tmrPingServer = new Timer(cbPingServer, null, 0, structure.LoginResponse_.PingingTimeInterval);
                LastHeartBeatTime = DateTime.UtcNow;
            }
        }

        public void Disconnect()
        {
            objClient.Dispose();
        }

        public void Reconnect()
        {
            objClient = new Client(DataType.BinaryType);
            objClient.DataReceived += new Client.DataReceivedHandler(objClient_DataReceived);
            objClient.OnDisconnection += new Client.OnDisconnectHandler(objClient_OnDisconnection);
            objClient.OnException += new Client.OnExceptionHandler(objClient_OnException);
            cbPingStatus = new TimerCallback(CheckPingStatusCallBack);
            tmrCheckPingStatus = new System.Threading.Timer(cbPingStatus, null, 0, 4000);

            try
            {
                objClient.Connect(ServerIPAddress, ServerPort);
                RegisterStructures();
                objClient.StartReceiving();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                Console.WriteLine(ex.ToString());
            }
        }

        private void DS_DOHandleHeartBeat(Client socketClient, HeartBeatPS HeartBeat)
        {
            //Console.WriteLine(HeartBeat.response_.ToString());
            //TotalElapsedTime = DateTime.UtcNow - LastHeartBeatTime;
            LastHeartBeatTime = DateTime.UtcNow;
            //Console.WriteLine("Data From Server :----->>>" + LastHeartBeatTime);
            clsUtility.WriteLineColor4Client(LastHeartBeatTime);
        }

        private void PongServerCallBack(object state)
        {
            try
            {
                HeartBeatPS objHeartBeatPs = new HeartBeatPS();
                objClient.SendStruct(objHeartBeatPs, () => { });
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                Debug.WriteLine(ex.ToString());
            }
        }

        private void CheckPingStatusCallBack(object state)
        {
            try
            {
                TimeSpan TotalTimeElapsed = DateTime.UtcNow - LastHeartBeatTime;
                if (TotalTimeElapsed.TotalSeconds > HeartbeatInterval && !IsConnected)
                {
                    LastHeartBeatTime = DateTime.UtcNow;
                    Disconnect();
                    Reconnect();
                }
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                Debug.WriteLine(ex.ToString());
            }
        }

        public void SubscribeLevel1Feed(uint instrumentID)
        {
            QuoteRequestPS quoteRequest = new QuoteRequestPS();
            quoteRequest.QuoteRequest.instrumentID = instrumentID;
            quoteRequest.QuoteRequest.ClientID = objClient.Id;
            objClient.SendStruct(quoteRequest, () => { });
        }

        public void SubscribeLevel2Feed(uint instrumentID)
        {
            // Level2QuoteRequest
            Level2QuoteRequestPS lvl2quoteRequest = new Level2QuoteRequestPS();
            lvl2quoteRequest.lvl2QuoteRequest.instrumentID = instrumentID;
            lvl2quoteRequest.lvl2QuoteRequest.ClientID = objClient.Id;
            objClient.SendStruct(lvl2quoteRequest, () => { });
        }

        public string RequestOHLC(string symbol, string periodicity, int candleInterval, int numberOfCandles)
        {
            //http://129.232.181.61:5003/HistoricalDataService.svc/GetOHLC/J055/D/1/365
           
            try
            {
                string footer = symbol + "/" + periodicity + "/" + candleInterval + "/" + numberOfCandles;
                var url = @"http://41.76.211.216:75/HistoricalDataService.svc/GetOHLC" + "/" + footer;

                var syncClient = new WebClient();
                var content = syncClient.DownloadString(url);//DownloadData(url);
                return content.ToString();
                //if (content == string.Empty)
                //    return new List<ExpenseSummary>();
                //JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                //OHLCResponse _socketResponce = serializer.Deserialize<OHLCResponse>((content));
                //return _socketResponce.lstOHLCData;

                
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

    }



    
}
