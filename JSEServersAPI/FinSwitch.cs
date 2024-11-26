using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using JSEServersAPI.FinSwitchUAT;
using JsonLibCommon;
using JSONMaitlandLib;

namespace JSEServersAPI
{
    public class FinSwitch
    {
        #region Fields

        Service objService;
        WebSocketFeed webSocket;
        private const string login = "FundX@123";
        private const string passwd = "1Trader1";
        private const string CompanyCode = "FundX";
        private FinSwitchWebService FinService;
        //static db_GetAllMancoCodesDataContext SP_GetAllMancoCodes;

        #endregion Fields

        #region Delegates

        //delegate void UpdateStatusHandler(Status status);
        //delegate void NewMessageHandler(byte[] message);
        //delegate void NewQuoteHandler(Quote quote);
        //delegate void NewBookQuoteHandler(BookQuote quote);
        //delegate void NewOHLCQuoteHandler(OHLCQuote quote);

        //public event Action<Instrument> onNewQuote = delegate { };
        //public event Action<BookQuote> onLevel2 = delegate { };

        public event Action<FeedResponse> OnDataFeedsJSE = delegate { };

        #endregion Delegates    

        public FinSwitch(string username, string password)
        {
            //objService = new Service();
            //webSocket = new WebSocketFeed("");
            //webSocket.OnDataFeeds += WebSocket_OnDataFeeds;
            FinService = new FinSwitchWebService();
            
        }

        private void WebSocket_OnDataFeeds(FeedResponse obj)
        {
            if (OnDataFeedsJSE != null)
                OnDataFeedsJSE(obj);
        }

        //public List<FundDetails> GetFinSwitchFundsList()
        //{
        //    var url = @"http://129.232.181.61:99/HistoricalDataService.svc/GetFundsRecords";

        //    var syncClient = new WebClient();
        //    var content = syncClient.DownloadString(url);
        //    JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
        //    FundRecordResponse _socketResponce = serializer.Deserialize<FundRecordResponse>(content);
        //    return _socketResponce.lstData;
        //    //return new List<FundDetails>();
        //}
        public List<FinSwitchHD> GetEODWithDates(string symbol, DateTime startdt, DateTime endDT, string PriceType)
        {
            try
            {
                Int32 unixTimestampStart = (Int32)(startdt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                Int32 unixTimestampEnd = (Int32)(endDT.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;


                string footer = symbol + "/" + unixTimestampStart + "/" + unixTimestampEnd + "/" + PriceType;

                var url = @"http://129.232.181.61:99/FinDBAccess.svc/GetFinSwitchPriceFeedByTypeCode/" + footer;//IMMA1/1485625559/1517161560";
                //var url = @"http://localhost:52585/FinDBAccess.svc/GetFinSwitchPriceFeedByTypeCode/" + footer;

                var syncClient = new WebClient();
                var content = syncClient.DownloadData(url);
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                FinSwitchHDResponse _socketResponce = serializer.Deserialize<FinSwitchHDResponse>(System.Text.Encoding.UTF8.GetString(content));
                return _socketResponce.lstData;
            }
            catch (Exception ex)
            {
                return new List<FinSwitchHD>();
            }


        }
        public List<string> GetFundTypes()
        {
            List<string> strLst = new List<string>();
            strLst.Add("Retail");//Institutional
            strLst.Add("Institutional");
            return strLst;

            
            var url = @"http://129.232.181.61:99/FinDBAccess.svc/GetFundTypes";

            var syncClient = new WebClient();
            var content = syncClient.DownloadData(url);
            JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            FundTypesResponse _socketResponce = serializer.Deserialize<FundTypesResponse>(System.Text.Encoding.UTF8.GetString(content));
            //FundTypesResponse _socketResponce = serializer.Deserialize<FundTypesResponse>(content);
            return _socketResponce.lstData;
            //return new List<string>();
        }
        public List<Sector> GetSectors(string fType)
        {
            var url = @"http://129.232.181.61:99/FinDBAccess.svc/GetSectors/" + fType;

            var syncClient = new WebClient();
            var content = syncClient.DownloadData(url);
            JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            SectorsResponse _socketResponce = serializer.Deserialize<SectorsResponse>(System.Text.Encoding.UTF8.GetString(content));
            // SectorsResponse _socketResponce = serializer.Deserialize<SectorsResponse>(content);
            return _socketResponce.lstData;
            //return new List<Sector>();
        }
        public List<CISFunds> GetCISFunds(string ftype, string sectorCode)
        {
            var url = @"http://129.232.181.61:99/FinDBAccess.svc/GetCISFunds" + "/" + ftype + "/" + sectorCode;

            var syncClient = new WebClient();
            var content = syncClient.DownloadString(url);
            JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            CISFundsResponse _socketResponce = serializer.Deserialize<CISFundsResponse>(content);
            return _socketResponce.lstData;
            //return new List<CISFunds>();
        }

        //public List<FinSwitchFundsDetails> GetFinSwitchFundDetails(string FundMancoCode, bool IsFundCode)
        //{
        //    //var url = @"http://197.242.148.230:99/FinDBAccess.svc/GetCISFunds" + "/" + FundMancoCode;

        //    //var syncClient = new WebClient();
        //    //var content = syncClient.DownloadString(url);
        //    //JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
        //    //CISFundsResponse _socketResponce = serializer.Deserialize<CISFundsResponse>(content);
        //    //return _socketResponce.lstData;
        //    //return new List<CISFunds>();
        //}


        private void DownloadFile(DateTime dt)
        {
            //var ccc = new FinSwitchWebService();
            //string Date = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            //// ccc.FundStaticDataDownload(login,passwd,CompanyCode,Date,true,"",true,)
            //string res = ccc.FundAccountFileDownload(login, passwd, CompanyCode, "", "", "", CompanyCode, "", true);
        }

        public void UploadFile(string path, string type)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            string fname = Path.GetFileName(path);
            
            string Date = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            int processId = FinService.UploadFile(login, passwd, Path.GetFileName(path), "text/plain", bytes, CompanyCode);
            string response = FinService.GetProcessStatus(login, passwd, processId);

            string res = FinService.DownloadFileAsString(login, passwd, type, DateTime.Parse("18-03-2019"), false, CompanyCode, "0");

            //string res = FinService.TRPreviewDownloadAsString(login, passwd, CompanyCode, processId);
            //string res = FinService.TRCustomDownload(login, passwd, CompanyCode, processId);

            //ccc.()
        }
        private void FPDCustomeDownload()
        {
            FPDDownloadOptions fpd = new FPDDownloadOptions();
            fpd.Login = login;
            fpd.Password = passwd;
            fpd.CompanyCode = CompanyCode;
            fpd.DateKind = DateSwitch.CycleDate;
            fpd.FromDate = new DateTime(2019, 03, 01);
            fpd.ToDate = new DateTime(2019, 03, 31);
            fpd.MancoCodes = GetAllMancos();
            fpd.StatusOption = PriceDistributionDownloadStatusOption.AllStatus;

            CustomDownloadResult res = FinService.FPDCustomDownload(fpd);
            string result = GetProcesResult(res.ProcessLogID);
        }

        private string[] GetAllMancos()
        {
            return new string[0];
            //var url = @"http://129.232.181.61:99/HistoricalDataService.svc/GetSectors/" + fType;

            //var syncClient = new WebClient();
            //var content = syncClient.DownloadData(url);
            //JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            //SectorsResponse _socketResponce = serializer.Deserialize<SectorsResponse>(System.Text.Encoding.UTF8.GetString(content));
            //// SectorsResponse _socketResponce = serializer.Deserialize<SectorsResponse>(content);
            //return _socketResponce.lstData;
        }

        private string GetProcesResult(int processLogID)
        {
            return FinService.GetFileDownload(login, passwd, processLogID);
        }

        private void PDCustomeDownload()
        {
            PDDownloadOptions pd = new PDDownloadOptions();
            pd.Login = login;
            pd.Password = passwd;
            pd.CompanyCode = CompanyCode;
            pd.DateKind = DateSwitch.CycleDate;
            pd.FromDate = new DateTime(2019, 03, 01);
            pd.ToDate = new DateTime(2019, 03, 31);
            //pd.Funds = empty;
            pd.FundTypes = new string[] { "AN", "AS", "AY", "BP", "CP", "CS", "FS", "IC", "LP", "NP", "PP", "PR", "TN", "TS", "TY" };
            pd.IncludeISINNumber = true;
            pd.MancoCodes = GetAllMancos();
            pd.PriceTypes = new string[] { "NP", "CP", "IC" };
            pd.StatusOption = PriceDistributionDownloadStatusOption.AllStatus;
            CustomDownloadResult res = FinService.PDCustomDownload(pd);
            string result = GetProcesResult(res.ProcessLogID);

        }
        #region //obsolete 

        public List<FinSwitchHD> GetHistoricalResponseFS(string srvURL, string symbol,  DateTime dt)
        {
            string response = string.Empty;
                  

            var url = @"http://197.242.148.230:99/HistoricalDataService.svc/GetFinSwitchHD/1502017542";

            //var url = @"http://197.242.148.230:99/HistoricalDataService.svc/GetFundsRecords";
            //var url = @"http://197.242.148.230:99/HistoricalDataService.svc/GetFinSwitchPriceFeed/IMMA1/1485625559/1517161560";

            var syncClient = new WebClient();
            var content = syncClient.DownloadString(url);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FinSwitchHDResponse _socketResponce = serializer.Deserialize<FinSwitchHDResponse>(content);
            return _socketResponce.lstData;
            //foreach (var obj in _socketResponce.lstData)
            //{

            //    Console.WriteLine(obj.FundCode + "," + obj.FundName + "," + obj.ManCoCode + "," + obj.ManCoName + "," + obj.MessageTypeCode
            //        + "," + obj.MessageTypeName + "," + obj.Price + "," + obj.Status + "," + obj.TypeCode + "," + obj.TypeName + "," + obj.ValueDate + "," + obj.DateSubmitted);
            //}
            //bjService.strMethodName = string.Format("GetFinSwitchHD" + "/" + "1502017542");
            //var Result = objService.RestServiceResponce();
            //JavaScriptSerializer objSer = new JavaScriptSerializer(); //{ MaxJsonLength = Int32.MaxValue };
            //FinSwitchHDResponse objResult = objSer.Deserialize<FinSwitchHDResponse>(Result);
            //List<FinSwitchHDResponse> objResult2 = objSer.Deserialize<List<FinSwitchHDResponse>>(Result);
        }
        
        public class Service
        {
            public Service()
            {
                ServiceUrl = "http://197.242.148.230:99/HistoricalDataService.svc";//GetFinSwitchHD/1502017542";
            }
            public Service(string ServiceUrl)
            {
                this.ServiceUrl = ServiceUrl;
            }

            readonly string ServiceUrl;
            public string strMethodName { get; set; }
            public string strMethodArgument { get; set; }
            public string strMethodType { get; set; }
            public string RestServiceResponce()
            {
                string strResult = string.Empty;
                try
                {
                    Uri objUrl = new Uri(string.Format("{0}/{1}", ServiceUrl, strMethodName));

                    HttpWebRequest objRequest = WebRequest.Create(objUrl) as HttpWebRequest;
                    objRequest.Method = string.IsNullOrWhiteSpace(strMethodType) ? "GET" : strMethodType;
                    objRequest.ContentType = "application/json";
                    objRequest.Accept = "application/json";

                    WebProxy objWebProxy = new WebProxy();
                    objWebProxy.IsBypassed(objUrl);
                    objRequest.Proxy = objWebProxy;

                    if (!string.IsNullOrEmpty(strMethodArgument))
                    {
                        try
                        {
                            using (var streamWriter = new StreamWriter(objRequest.GetRequestStream()))
                            {
                                streamWriter.Write(strMethodArgument);
                            }
                        }
                        catch { }
                    }

                    using (HttpWebResponse objResponce = objRequest.GetResponse() as HttpWebResponse)
                    {
                        StreamReader objReader = new StreamReader(objResponce.GetResponseStream());
                        strResult = objReader.ReadToEnd();
                        objRequest.KeepAlive = false;
                        objResponce.Close();
                        objReader.Dispose();
                        return strResult;
                    }
                }
                catch { }
                return strResult;
            }
        }

        #endregion

       
    }
}
