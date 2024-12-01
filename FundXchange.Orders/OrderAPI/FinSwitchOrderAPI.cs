using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FundXchange.Orders.Entities;
using FundXchange.Orders.FX_OrderService;
using Brokerages = FundXchange.Orders.Enumerations.Brokerages;

namespace FundXchange.Orders.OrderAPI
{
    class FinSwitchOrderAPI :IOrderAPI
    {
        //private const string _url = "https://Finswitch2.FinSwitch.com/webservices/FinSwitchwebservice.asmx";        
        //private const string login = "FundX@123";
        //private const string passwd = "1Trader1";
        //private const string CompanyCode = "FundX";

        FundXchange.Orders.FinSwitchUAT.FinSwitchWebService _service;
        //FundXchange.Orders.FinSwitchLIVE.FinSwitchWebService _service;

        //One of: “Busy”, “Error”, “Finished”, “Pending”, “Aborted” 
        //If “Error”, all errors will be appended to the output string 
        //If “Finished”, any warnings will be appended to the output string

        public FinSwitchOrderAPI()
        {
            _service = new FinSwitchUAT.FinSwitchWebService();
            //_service = new FinSwitchLIVE.FinSwitchWebService();
            _service.UploadFileCompleted += _service_UploadFileCompleted;
            _service.GetProcessStatusCompleted += _service_GetProcessStatusCompleted;
            
            
        }

        private void _service_GetProcessStatusCompleted(object sender, FinSwitchUAT.GetProcessStatusCompletedEventArgs e)
        {
          
        }

        private void _service_UploadFileCompleted(object sender, FinSwitchUAT.UploadFileCompletedEventArgs e)
        {
            //_service.DownloadFileAsString()
        }

        public int UploadFile(string loginID, string pswd, string fileName, string CompanyCode)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(fileName);            
            //var orders = new FundXchange.Orders.FinSwitchUAT.FinSwitchWebService();
            int res= _service.UploadFile(loginID, pswd, Path.GetFileName(fileName), "text/plain", bytes, CompanyCode);
            return res;
        }

        public string GetProcessStatus(string loginID, string pswd, int processID)
        {
            return _service.GetProcessStatus(loginID, pswd, processID);
        }


        #region IOrderMethods
        public string AddStopLossOrder(TradingAccount tradingAccount, string exchange, string symbol, int triggerPrice, int stopLossPrice, int cancelPrice, PriceTypes priceType, int quantity, OrderSide orderSide, DateTime? expiryDate)
        {
            throw new NotImplementedException();
        }

        public string AddTradingAccount(string accountName)
        {
            throw new NotImplementedException();
        }

        public void CancelMarketOrder(TradingAccount tradingAccount, string orderId)
        {
            throw new NotImplementedException();
        }

        public void DeleteMarketOrder(TradingAccount tradingAccount, string orderId)
        {
            throw new NotImplementedException();
        }

        public int GetCashOnHand()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, MarketOrder> GetMarketOrders(TradingAccount tradingAccount)
        {
            throw new NotImplementedException();
        }

        public int GetStartingBalance()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, StopLossOrder> GetStopLossOrders(TradingAccount tradingAccount)
        {
            throw new NotImplementedException();
        }

        public List<TradingAccount> GetTradingAccounts(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void MarketOrderExpired(TradingAccount tradingAccount, string orderId)
        {
            throw new NotImplementedException();
        }

        public void MarketOrderMatched(TradingAccount tradingAccount, string orderId, int quantity)
        {
            throw new NotImplementedException();
        }

        public string PlaceMarketOrder(TradingAccount tradingAccount, string exchange, string symbol, OrderSide orderSide, int price, int quantity, DateTime? expiryDate)
        {
            throw new NotImplementedException();
        }

        public void RemoveStopLossOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        public void RemoveTradingAccount(string accountNumber, string username, string password)
        {
            throw new NotImplementedException();
        }

        public void UpdateMarketOrder(TradingAccount tradingAccount, string orderId, int price, int quantity, DateTime? expiryDate)
        {
            throw new NotImplementedException();
        }

        public void UpdateStopLossOrder(string orderId, int quantity, int triggerPrice, int stopLossPrice, int cancelPrice, PriceTypes priceType, DateTime? expiryDate)
        {
            throw new NotImplementedException();
        }

        public string UpgradeStopLossOrderToMarketOrder(TradingAccount tradingAccount, string orderId, int price, int quantity)
        {
            throw new NotImplementedException();
        }
       
        #endregion
    }
}
