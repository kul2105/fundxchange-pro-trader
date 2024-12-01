///////REVISION HISTORY/////////////////////////////////////////////////////////////////////////////////////////////////////
//DATE			INITIALS	DESCRIPTION	
//14/02/2012	VP		    1.Added TWSContactMnager class for ContactManger information Handling
//              		    2.Coding in TWSContactManger for contract information handling
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Data;
using System.Linq;
using ClientDLL_Model.Cls;
using ClientDLL_Model.Cls.Contract;
using ClientDLL_NET.Manager;
using ClientDLL_NET.Manager.Interfaces;
//using Logging;
using CommonLibrary.Cls;
using PALSA.DS;
using System;

namespace PALSA.Cls
{
    public class ClsTWSContractManager : IContractManager
    {
        #region "         MEMBERS          "


        private static ClsTWSContractManager _instance;
        private static List<string> _productTypes = new List<string>();
        private readonly ContractManager objContractManager = ContractManager.GetSingleInstance();
        public DS4ContractsInfo contractsInfoDS = new DS4ContractsInfo();

        #endregion "      MEMBERS          "

        #region "         PROPERTIES         "

        public static ClsTWSContractManager INSTANCE
        {
            get { return _instance ?? (_instance = new ClsTWSContractManager()); }
        }


        public bool IsContractMgrLoaded { get; private set; }

        #endregion "     PROPERTIES        "

        #region "         METHODS          "

        public void Init(string username, string pwd, string serverIp, string hostIp, int port)
        {
            objContractManager.RegisterForEvents(this);
            objContractManager.Start();
            objContractManager.Login(username, pwd, serverIp, hostIp, port);

            //FileHandling.WriteInOutLog("Contract Manager : Send Connection Request");
            //FileHandling.WriteAllLog("Contract Manager : Send Connection Request");
        }

        public string GetProductNameForContract(string productType, string contractName)
        {
            string productName = string.Empty;
            List<string> products = INSTANCE.GetAllProducts(productType);
            foreach (string product in from product in products
                                       let contracts = INSTANCE.GetAllContracts(productType, product)
                                       where contracts.Contains(contractName)
                                       select product)
            {
                productName = product;
                break;
            }
            return productName;
        }

        public List<string> GetAllProductTypes()
        {
            return objContractManager.GetAllProductTypes();
        }

        public IEnumerable<string> GetProductsInfo(string productType, string query, eSEARCH_CRITERIA searchCriteria)
        {
            return objContractManager.QueryForProducts(query, productType, searchCriteria);
        }

        public List<string> GetAllContracts(string productType, string productName) //instrument type ==product type
        {
            return objContractManager.GetAllContracts(productType, productName);
        }

        public InstrumentSpec GetInstrumentSpecification(string productType, string contactName, string productName)
        {
            return objContractManager.GetContractSpec(contactName, productType, productName);
        }

        public List<string> GetProductTypes()
        {
            return _productTypes;
        }

        public List<string> QueryForContracts(string queryString, string productType, string productName,
                                                eSEARCH_CRITERIA eSearchCriteria)
        {
            return objContractManager.QueryForContracts(queryString, productType, productName, eSearchCriteria);
        }

        public InstrumentSpec GetContractSpec(string contractName, string productType, string productName)
        {
            return objContractManager.GetContractSpec(contractName, productType, productName);
        }

        public List<string> GetAllProducts(string productType)
        {
            return objContractManager.GetAllProducts(productType);
        }

        public void Close()
        {
            if (objContractManager != null)
                objContractManager.Dispose();

            //FileHandling.WriteInOutLog("Contract Manager : Send DisConnection Request");
            //FileHandling.WriteAllLog("Contract Manager : Send DisConnection Request");
        }

        #endregion "         METHODS          "


        #region IContractManager Members

        public void onManagerStatus(eManagerStatus status)
        {
            //FileHandling.WriteInOutLog("Contract Manager : Contract Manager Status" + status.ToString());
            //FileHandling.WriteAllLog("Contract Manager : Contract Manager Status" + status.ToString());

            if (status == eManagerStatus.Loaded)
            {
                IsContractMgrLoaded = true;
                string lstInstruments = string.Empty;

                _productTypes = objContractManager.GetAllProductTypes();
                lstInstruments = _productTypes.Aggregate(lstInstruments,
                                                         (current, productType) => current + "," + productType);
                //BOChanges
                //Properties.Settings.Default.LstInstruments = lstInstruments;
                //Properties.Settings.Default.Save();
            }
        }

        public void onContractChanged(InstrumentSpec contractList)
        {
            //FileHandling.WriteInOutLog("Contract Manager : Contract Changed" + contractList.Contract);
            //FileHandling.WriteAllLog("Contract Manager : Contract Changed" + contractList.Contract);
        }

        public void onContractRemoved(InstrumentSpec contractList)
        {
            //FileHandling.WriteInOutLog("Contract Manager : Contract Removed" + contractList.Contract);
            //FileHandling.WriteAllLog("Contract Manager : Contract Removed" + contractList.Contract);
        }

        public void onNewContractSpec(InstrumentSpec contractList)
        {
            //FileHandling.WriteInOutLog("Contract Manager : New Contract" + contractList.Contract);
            //FileHandling.WriteAllLog("Contract Manager : New Contract" + contractList.Contract);
        }

        public void onError(string error)
        {
            //FileHandling.WriteInOutLog("Contract Manager : error" + error);
            //FileHandling.WriteAllLog("Contract Manager : error" + error);
        }

        #endregion


        public void onClientError(int error)
        {
            //ClsCommonMethods.ShowInformation("The");
        }

        public bool GetSymbolsFromWebService(string fileName,int participantId)
        {
           bool result = false;
            try
            {
                result = objContractManager.DownloadContractFromWebService(fileName, participantId);
            }
            catch
            {
            }
           //if(result)
           //{
           //    List<string> productTypes = ClsTWSContractManager.INSTANCE.GetAllProductTypes();
           //    foreach (string productType in productTypes)
           //    {
           //        List<string> products = ClsTWSContractManager.INSTANCE.GetAllProducts(productType);
           //        foreach (string product in products)
           //        {
           //            List<string> symbols = ClsTWSContractManager.INSTANCE.GetAllContracts(productType, product);
           //            foreach (string symbol in symbols)
           //            {
           //                InstrumentSpec instspec = ClsTWSContractManager.INSTANCE.GetContractSpec(symbol,productType,product);
           //                List<string> key =Symbol.getKey(instspec);
           //                foreach (var symKey in key)
           //                {
           //                    if (!ClsGlobal.DDContractInfo.Keys.Contains(symKey))
           //                    {
           //                        ClsGlobal.DDContractInfo.Add(symKey, instspec);
           //                    }
           //                    else
           //                    {
           //                        ClsGlobal.DDContractInfo[symKey] = instspec;
           //                    }
           //                }                          
           //            }
           //        }
           //    }
           //}
            return result;
        }

        public bool ReadSymbolsFromFile(string fileName)
        {
           return objContractManager.ReadContractFromXML(fileName);
        }

        public InstrumentSpec GetContractSpecBySymbolName(string symbol)
        {
            InstrumentSpec inst=null;
            List<string> lstProductTypes = objContractManager.GetAllProductTypes();
            foreach (string pt in lstProductTypes)
            {
               List<string> lstProducts=objContractManager.GetAllProducts(pt);
               foreach (string p in lstProducts)
               {
                   List<string> lstSymbols = objContractManager.GetAllContracts(pt, p);
                   foreach (string s in lstSymbols)
                   {
                       if (s.ToUpper()==symbol.Replace("/", "").ToUpper())
                       {
                           inst = objContractManager.GetContractSpec(s, pt, p);
                       }
                   }
               }
            }
            return inst;
        }
    }
}