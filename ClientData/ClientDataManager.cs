using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using VTigerApi;
using System.Data;

namespace M4.ClientData
{
    [Serializable]
    public class ClientDataManager
    {
        private static ClientDataManager instance;
        private List<ClientPorfolio> portfolioList;
        private List<ClientIndex> indexList;
        public Dictionary<string, Client> clientList;
        private Dictionary<string, string> groups;
        private Dictionary<string, string> clientGroups;
        private List<string> AssetClassList;
        private List<string> indexSecurityTypeList;
        //private List<string> SecurityTypeList;
        private List<string> industryList;
        private List<string> sectorList;
        private List<string> accountTypeList;
        private List<string> CashAccountList;
        private List<string> SymbolSecurityTypeList;
        private List<string> ExchangeList;

        private List<string> riskCountryList;
        private List<string> issueCountryList;
        private List<string> clientCountryList;
        private List<string> clientGroupList;

        public List<long> UniqueAccountList;        
        public static long UniqueAccountNum = 10000;
        public static long UniqueClientID = 5000;
        public ClientPorfolio INVESTMENT;
        private List<Asset> AssetList;
        private List<IntAsset> IntAssetList;
        private List<Geographic> GeographyList;
        private List<Currency> CurrencyList;
        private List<Risk> RiskList;
        private List<RiskProfile> RiskProfileList;
        private string EODPath = System.Windows.Forms.Application.StartupPath + "\\ClientDataManager\\Investment";

        private string URL_CRM;      
        private string CRM_USERNAME;
        private string CRM_ACCESSKEY;
        private RiskProfile DefaultRP;

        public List<ClientPorfolio> PortfolioDMList;

        
        public List<Strategy> ListStrategy;
        private bool _HideVolBar4Chart;
        public bool HideVolBar4Chart
        {
            get { return _HideVolBar4Chart; }
            set { _HideVolBar4Chart = value; }
        }
        public List<string> lstAssetReturn;
        
        public Dictionary<string, List<object>> dicBreachAsset;
        public Dictionary<string, List<string>> dicBreachPort;
        public Dictionary<string, Dictionary<string, List<string>>> dicBreachInstrument;
        private ClientDataManager()
        {
            portfolioList = new List<ClientPorfolio>();
            indexList = new List<ClientIndex>();
            clientList = new Dictionary<string, Client>();
            groups = new Dictionary<string, string>();
            clientGroups = new Dictionary<string, string>();
            AssetClassList = new List<string>();
            indexSecurityTypeList = new List<string>();
            industryList = new List<string>();
            sectorList = new List<string>();
            ExchangeList = new List<string>();
            accountTypeList = new List<string>();
            riskCountryList = new List<string>();
            issueCountryList = new List<string>();
            clientCountryList = new List<string>();
            clientGroupList = new List<string>();
            UniqueAccountList = new List<long>();
            CashAccountList = new List<string>();
            SymbolSecurityTypeList = new List<string>();
            SymbolSecurityTypeList.Add("Forex");
            SymbolSecurityTypeList.Add("Futures");

            //To create default non exchange instruments
            INVESTMENT = new ClientPorfolio();
            INVESTMENT.PorfolioName = "Investment";
            AddPorfolio(INVESTMENT);

            AssetList = new List<Asset>();
            IntAssetList = new List<IntAsset>();
            GeographyList = new List<Geographic>();
            CurrencyList = new List<Currency>();
            RiskList = new List<Risk>();
            RiskProfileList = new List<RiskProfile>();

            URL_CRM = Properties.Settings.Default.vTigerURL.Trim();
            CRM_USERNAME = Properties.Settings.Default.vTigerUsername.Trim();
            CRM_ACCESSKEY = Properties.Settings.Default.vTigerAccessKey.Trim();
            DefaultRP = new RiskProfile();
            DefaultRP.Name = "DefaultRP";
            AddRiskProfile(DefaultRP);
            AddDefaultQuestionsinRP();

            PortfolioDMList = new List<ClientPorfolio>();
            ListStrategy = new List<Strategy>();
            lstAssetReturn = new List<string>();

            dicBreachAsset = new Dictionary<string, List<object>>();
            dicBreachPort = new Dictionary<string, List<string>>();
            dicBreachInstrument = new Dictionary<string, Dictionary<string, List<string>>>();

            
        }       
     
        public static ClientDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    try
                    {
                        bool isFileExists = false;
                        string dataManagerPath = System.Windows.Forms.Application.StartupPath + "\\ClientDataManager\\SaveData";
                        if (File.Exists(dataManagerPath))
                        {
                            isFileExists = true;
                        }
                        if (isFileExists)
                        {
                            instance = new ClientDataManager();
                            System.IO.Stream streamRead = System.IO.File.OpenRead(dataManagerPath);
                            BinaryFormatter binaryRead = new BinaryFormatter();
                            instance = binaryRead.Deserialize(streamRead) as ClientDataManager;
                            streamRead.Close();
                        }
                        else
                        {
                            instance = new ClientDataManager();
                        }
                    }
                    catch
                    {
                        instance = new ClientDataManager();
                    }
                }
                return instance;
            }
        }

        public void SaveDataManager()
        {            
            string dataManagerPath = System.Windows.Forms.Application.StartupPath + "\\ClientDataManager";
            DirectoryInfo dirInf = new DirectoryInfo(dataManagerPath);
            if (!dirInf.Exists)
            {
                dirInf.Create();
            }
            using (FileStream stream = new FileStream(dataManagerPath + "\\SaveData", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                BinaryFormatter sf = new BinaryFormatter();
                sf.Serialize(stream, instance);
            } 
        }
        public string GetEODPath()
        {
            return EODPath;
        }

        private void AddDefaultQuestionsinRP()
        {
            if (DefaultRP != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    DefaultRP.lstQuest.Add(new RiskQuestion());
                }

                DefaultRP.lstQuest[0].QNum = 1;
                DefaultRP.lstQuest[0].Quest = "A younger person can usually afford to take more risk. What is your age?";
                DefaultRP.lstQuest[0].Ans1 = "0-40_25";
                DefaultRP.lstQuest[0].Ans2 = "40-50_20";
                DefaultRP.lstQuest[0].Ans3 = "50-60_10";
                DefaultRP.lstQuest[0].Ans4 = "60+_0";

                DefaultRP.lstQuest[1].QNum = 2;
                DefaultRP.lstQuest[1].Quest = "A healthier person can on average take more risk than a person with a health problem. \n How would you assess your health against the average health of your age group:";
                DefaultRP.lstQuest[1].Ans1 = "Average_5";
                DefaultRP.lstQuest[1].Ans2 = "Better_10";
                DefaultRP.lstQuest[1].Ans3 = "Worse_0";                

                DefaultRP.lstQuest[2].QNum = 3;
                DefaultRP.lstQuest[2].Quest = "What percentage of your overall savings does this constitute?";
                DefaultRP.lstQuest[2].Ans1 = "Less than 25%_15";
                DefaultRP.lstQuest[2].Ans2 = "25% TO 50%_10";
                DefaultRP.lstQuest[2].Ans3 = "50% TO 75%_5";
                DefaultRP.lstQuest[2].Ans4 = "More than 75%_0";

                DefaultRP.lstQuest[3].QNum = 4;
                DefaultRP.lstQuest[3].Quest = " Which of the following best describes your reaction should your investment(s) decline significantly in value in the short term?";
                DefaultRP.lstQuest[3].Ans1 = "I am focussed on long term growth annd accept short-term fluctuations as a matter of course_20";
                DefaultRP.lstQuest[3].Ans2 = "I would not be too concerned, as I am investing for the long term_15";
                DefaultRP.lstQuest[3].Ans3 = "I would be very concerned as I do not like short term losses in value._10";
                DefaultRP.lstQuest[3].Ans4 = "I would be extremely disappointed and consider dis-investing._0";

                DefaultRP.lstQuest[4].QNum = 5;
                DefaultRP.lstQuest[4].Quest = "How many years away from your planned retirement date are you?";
                DefaultRP.lstQuest[4].Ans1 = "Already Retired_5";
                DefaultRP.lstQuest[4].Ans2 = "Less than Five years_0";
                DefaultRP.lstQuest[4].Ans3 = "5-15 years_10";
                DefaultRP.lstQuest[4].Ans4 = "15 years_20";

                DefaultRP.lstQuest[5].QNum = 6;
                DefaultRP.lstQuest[5].Quest = "What is the primary goal of this investment?";
                DefaultRP.lstQuest[5].Ans1 = "Retirement Savings_5";
                DefaultRP.lstQuest[5].Ans2 = "Wealth Preservation_0";
                DefaultRP.lstQuest[5].Ans3 = "Wealth Creation_15";                

                DefaultRP.lstQuest[6].QNum = 7;
                DefaultRP.lstQuest[6].Quest = "Except for the income, do you foresee any other major expense? \n If so, will this be substantial (e.g. greater than 25% of this investment)?";
                DefaultRP.lstQuest[6].Ans1 = "No,I do not foresee any other major expenses_15";
                DefaultRP.lstQuest[6].Ans2 = "Yes.However, I have separate savings to cover this _10";
                DefaultRP.lstQuest[6].Ans3 = "Yes.However, this will be less than 25% of these savings _5";
                DefaultRP.lstQuest[6].Ans4 = "Yes.This will be substantial and should be covered for by this investment_0";

                DefaultRP.lstQuest[7].QNum = 8;
                DefaultRP.lstQuest[7].Quest = "Which of the following best describes your reaction should your investment(s) decline significantly in value in the short term?";
                DefaultRP.lstQuest[7].Ans1 = "I am focussed on long term growth and accept short-term fluctuations as a matter of course._20";
                DefaultRP.lstQuest[7].Ans2 = "I would not be too concerned, as I am investing for the long term._15";
                DefaultRP.lstQuest[7].Ans3 = "I would be very concerned as I do not like short term losses in value._5";
                DefaultRP.lstQuest[7].Ans4 = "I would be extremely disappointed and consider dis-investing._0";

                //DefaultRP.lstQuest[8].QNum = 9;
                //DefaultRP.lstQuest[8].Quest = "Fill in the maximum that you are willing to loose in the following scenarios:";
                //DefaultRP.lstQuest[8].Ans1 = "What percentage of your total portfolio (all the investments) are you prepared to loose \n before you would want to get out of the market?_0";
                //DefaultRP.lstQuest[8].Ans2 = "What percentage of each portfolio are you prepared to loose before you want to get out of the market?_5";
                //DefaultRP.lstQuest[8].Ans3 = "What percentage are you willing to loose in any of the underlying instruments in the portfolios \n before you would want to get out of the market?_10";                

                //UpdateRiskProfile(DefaultRP);
                if (RiskProfileList.FirstOrDefault(i => i.Name == DefaultRP.Name) != null)//Means already there
                {
                    int index = RiskProfileList.FindIndex(i => i.Name == DefaultRP.Name);
                    RiskProfileList[index] = DefaultRP;
                }
                else
                {
                    RiskProfileList.Add(DefaultRP);
                }
                //ClientDataManager.Instance.SaveDataManager();
            }

        }
        public void AddPorfolio(ClientPorfolio portfolio)
        {
            if(!portfolioList.Contains(portfolio))
            portfolioList.Add(portfolio);
        }

        public void RemovePorfolio(string portfolioName)
        {
            if (portfolioList.Count > 0)
            {
                ClientPorfolio portfolio = portfolioList.FirstOrDefault(i => i.PorfolioName == portfolioName);
                if (portfolioList.Contains(portfolio))
                    portfolioList.Remove(portfolio);
            }
            
        }
        
        public void UpdatePortfolio(ClientPorfolio portfolio)
        {
            if (portfolioList.Exists(i => i.PorfolioName == portfolio.PorfolioName))
            {
                int index = portfolioList.FindIndex(i => i.PorfolioName == portfolio.PorfolioName);
                portfolioList[index] = portfolio;
            }
            else
            {
                portfolioList.Add(portfolio);
            }
        }
        
        public void AddIndex(ClientIndex index)
        {
            if (!indexList.Contains(index))
                indexList.Add(index);
        }
        
        public void RemoveIndex(string indexName)
        {
            if (indexList.Count > 0)
            {
                ClientIndex index = indexList.FirstOrDefault(i => i.IndexName == indexName);
                if (indexList.Contains(index))
                    indexList.Remove(index);
            }
            
        }
        
        public void UpdateIndex(ClientIndex index)
        {
            if (indexList.Exists(i => i.IndexName == index.IndexName))
            {
                int ind = indexList.FindIndex(i => i.IndexName == index.IndexName);
                indexList[ind] = index;
            }
            else
            {
                indexList.Add(index);
            }
        }
        
        public List<ClientPorfolio> GetAllPortfolioes()
        {
            return portfolioList;
        }
        
        public List<ClientIndex> GetAllIndexes()
        {
            return indexList;
        }

       

        public ClientPorfolio GetPortfolio(string portfolioName)
        {
            ClientPorfolio portfolio = null;
            if (portfolioList.Count > 0)
            {
                portfolio = portfolioList.FirstOrDefault(i => i.PorfolioName == portfolioName);
            }
            return portfolio;
        }
        
        public ClientIndex GetIndex(string indexName)
        {
            ClientIndex index = null;
            if (indexList.Count > 0)
            {
                index = indexList.FirstOrDefault(i => i.IndexName == indexName);
            }
            return index;
        }
        
        //public List<Client> GetAllClient()
        //{
        //    //return clientList;

        //}

        public Client GetClient(string clientName)
        {
            Client client = null;
            client = clientList.Values.FirstOrDefault(i => i.ClientName == clientName);
            //if (client != null)
            //{
 
            //}
            
            //if (clientList.Count > 0)
            //{
            //    client = clientList[clientNumber];
            //}
            return client;
        }

        public Client GetClient2(string clientNumber)
        {
            Client client = null;           
            if (clientList.Count > 0)
            {
                client = clientList[clientNumber];
            }
            return client;
        }

        public void AddClient(Client client)
        {
            long ID = CreateClientNumber();
            if (ID != 0)
            {
                client.ClientNumber = ID.ToString();
                clientList.Add(ID.ToString(), client);

                //Add Client in CRM
                //string clientnum = AddClientInCRM(client);
                //if (!string.IsNullOrEmpty(clientnum) && !clientList.ContainsKey(clientnum))
                //{
                //    //Update Local Dictionary
                //    clientList.Add(clientnum, client);
                //}
            }
        }

        public bool RemoveClient(string clientNumber)
        {
            if(clientList.ContainsKey(clientNumber))
            {
                Client client = clientList[(clientNumber)];
                
                //Remove from CRM first
                //RemoveClientInCRM(client);

                clientList.Remove(clientNumber);
                //foreach (var item in client.AccountList)//Remove each account number also associated with this client
                //{
                //    if (UniqueAccountList.Contains(item.AccountNumber))
                //        UniqueAccountList.Remove(item.AccountNumber);
                //}
                return true;
            }
            return false;
        }       

        public void UpdateClient(Client client)
        {
            if (clientList.ContainsKey((client.ClientNumber)))
            {                
                clientList[(client.ClientNumber)] = client;
            }
            //else
            //{
            //    client.ClientNumber = "SA" + (GenerateUniqueNo()).ToString();
            //    clientList.Add(client);
            //}
        }
        
        public void AddGroup(string groupName,string description)
        {
            if (!groups.ContainsKey(groupName) && !string.IsNullOrEmpty(groupName))
            {
                groups.Add(groupName,description);
            }
        }
        
        public void RemoveGroup(string groupName)
        {
            if (groups.ContainsKey(groupName) && !string.IsNullOrEmpty(groupName))
            {
                groups.Remove(groupName);
            }
        }

        public void AssignClientGroup(string groupName, string subGroupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                foreach (var item in groups)
                {
                    if(item.Key.Equals(groupName) )
                    {
                        if (!item.Value.Contains(subGroupName))
                        {
                            string csSubGroups = item.Value;
                            csSubGroups += subGroupName + ",";
                            groups[item.Key] = csSubGroups;
                        }
                        break;
                    }
                }
            }
        }

        public void RevokeClientGroup(string groupName, string clientGroupName)
        {
            if (groups[groupName].Contains(clientGroupName))
            {
                char[] separator = { ',' };
                string[] assignedSubgroups = groups[groupName].Split(separator);
                string updatedValue = "";
                foreach (string currentSubgroup in assignedSubgroups)
                {
                    if (!currentSubgroup.Equals(clientGroupName))
                    {
                        updatedValue += currentSubgroup + ",";
                    }
                }

                groups[groupName] = updatedValue;
            }
        }

        public void AddClientGroup(string clientGroupName, string description)
        {
            if (!clientGroups.ContainsKey(clientGroupName) && !string.IsNullOrEmpty(clientGroupName))
            {
                clientGroups.Add(clientGroupName, description);
            }
        }

        public void RemoveClientGroup(string clientGroupName)
        {
            if (clientGroups.ContainsKey(clientGroupName) && !string.IsNullOrEmpty(clientGroupName))
            {
                clientGroups.Remove(clientGroupName);
                // remove the references from groups
                //Dictionary<string, string> currentGroups;
                String [] groupNames = new String[groups.Keys.Count];
                int index = 0;
                foreach (var key in groups.Keys)
                {
                    groupNames[index++] = key.ToString();
                }

                foreach (String groupName in groupNames )
                {
                    RevokeClientGroup(groupName, clientGroupName);
                }
            }
        }

        public List<string> GetAllGroup()
        {
            List<string> retGroup = new List<string>();
            foreach (var item in groups)
            {
                retGroup.Add(item.Key);
            }
            return retGroup;
        }

        public string[] GetAssignedClientGroup(string groupName)
        {
            string[] assignedGroup = null;
            //if (!string.IsNullOrEmpty(item.Value))
            {
                char[] separator = { ',' };
                foreach (var item in groups)
                {
                    if (item.Key.Equals(groupName))
                    {
                        assignedGroup 
                        = item.Value.Split(separator
                                        , StringSplitOptions.RemoveEmptyEntries);
                        //assignedGroup.;
                        break;
                    }
                }
            }
            return assignedGroup;
        }

        public List<string> GetAllClientGroup()
        {
            List<string> retClientGroup = new List<string>();
            foreach (var item in clientGroups)
            {
                retClientGroup.Add(item.Key);
            }
            return retClientGroup;
        }

        public string GetGroupDescription(string groupName)
        {
            if (groups.ContainsKey(groupName))
            return groups[groupName];
            return null;
        }

        public string GetClientGroupDescription(string clientGroupName)
        {
            if (clientGroups.ContainsKey(clientGroupName))
                return clientGroups[clientGroupName];
            return null;
        }
        
        public List<string> GetAllAssetClass()
        {
            return AssetClassList;
        }
        
        public void AddAssetClass(string assetClass)
        {
            if (!AssetClassList.Contains(assetClass) && !string.IsNullOrEmpty(assetClass))
            {
                AssetClassList.Add(assetClass);
            }
        }

        public List<string> GetAllIndexSecurityType()
        {
            return indexSecurityTypeList;
        }

        public List<string> GetIndustries()
        {
            return industryList;
        }

        public List<string> GetSectors()
        {
            return sectorList;
        }

        public List<string> GetAcccountTypes()
        {
            return accountTypeList;
        }

        public List<string> GetRiskCountries()
        {
            return riskCountryList;
        }

        public List<string> GetIssueCountries()
        {
            return issueCountryList;
        }

        public List<string> GetClientCountries()
        {
            return clientCountryList;
        }

        public List<string> GetSymbolSecurityList()
        {
            return SymbolSecurityTypeList;
        }


        public void AddIndexSecurityType(string securityType)
        {
            if (indexSecurityTypeList == null)
            {
                indexSecurityTypeList = new List<string>();
            }
            if (!indexSecurityTypeList.Contains(securityType))
            {
                indexSecurityTypeList.Add(securityType);
            }
        }

        public void AddIndustry(string industry)
        {
            if (industryList == null)
            {
                industryList = new List<string>();
            }

            if (!industryList.Contains(industry))
            {
                industryList.Add(industry);
            }
        }

        public void AddSector(string sector)
        {
            if (sectorList == null)
            {
                sectorList = new List<string>();
            }

            if (!sectorList.Contains(sector))
            {
                sectorList.Add(sector);
            }
        }

        public void AddAccountType(string accountType)
        {
            if (accountTypeList == null)
            {
                accountTypeList = new List<string>();
            }

            if (!accountTypeList.Contains(accountType))
            {
                accountTypeList.Add(accountType);
            }
        }

        public void AddRiskCountry(string riskCountry)
        {
            if (riskCountryList == null)
            {
                riskCountryList = new List<string>();
            }

            if (!riskCountryList.Contains(riskCountry))
            {
                riskCountryList.Add(riskCountry);
            }
        }

        public void AddIssueCountry(string issueCountry)
        {
            if (issueCountryList == null)
            {
                issueCountryList = new List<string>();
            }

            if (!issueCountryList.Contains(issueCountry))
            {
                issueCountryList.Add(issueCountry);
            }
        }

        public void AddSecurityTypeList(string SecurityType)
        {
            if (SymbolSecurityTypeList == null)
            {
                SymbolSecurityTypeList = new List<string>();
                SymbolSecurityTypeList.Add("Forex");
                SymbolSecurityTypeList.Add("Futures");
            }

            if (!SymbolSecurityTypeList.Contains(SecurityType))
            {
                SymbolSecurityTypeList.Add(SecurityType);
            }
        }

        public void AddClientCountry(string clientCountry)
        {
            if (clientCountryList == null)
            {
                clientCountryList = new List<string>();
            }
            if (!clientCountryList.Contains(clientCountry))                
            {
                clientCountryList.Add(clientCountry);
            }
        }



        internal long AddUniqueAccountNum()
        {
            long UniqueAcc = 0;
            if (UniqueAccountList.Count > 0)
            {
                UniqueAccountNum = UniqueAccountList.Last() + 1;
                UniqueAcc = UniqueAccountNum;
                //return UniqueAcc;
            }
            else
            {
                UniqueAcc = Interlocked.Increment(ref UniqueAccountNum);//Here we assign First Unique Account Number
            }
            UniqueAccountList.Add(UniqueAcc);
            return UniqueAcc;
        }

        internal void AddAsset(Asset ass)
        {
            if (!AssetList.Contains(ass))
                AssetList.Add(ass);
        }


        internal void AddIntAsset(IntAsset IntAss)
        {
            if (!IntAssetList.Contains(IntAss))
                IntAssetList.Add(IntAss);
        }

        internal void AddGeographic(Geographic geo)
        {
            if (!GeographyList.Contains(geo))
                GeographyList.Add(geo);
        }

        internal void AddCurrency(Currency curr)
        {
            if (!CurrencyList.Contains(curr))
                CurrencyList.Add(curr);
        }

        internal void AddRisk(Risk rsk)
        {
            if (!RiskList.Contains(rsk))
                RiskList.Add(rsk);
        }

        internal List<Asset> GetAllAsset()
        {
            return AssetList;
        }

        internal List<IntAsset> GetAllIntAsset()
        {
            return IntAssetList;
        }
        internal List<Geographic> GetAllGeography()
        {
            return GeographyList;
        }
        internal List<Currency> GetAllCurrency()
        {
            return CurrencyList;
        }
        internal List<Risk> GetAllRisk()
        {
            return RiskList;
        }

        internal List<RiskProfile> GetAllRiskProfile()
        {
            return RiskProfileList;
        }

        internal RiskProfile GetRiskProfile(string risk)
        {
             RiskProfile profile = null;
            if (RiskProfileList.Count > 0)
            {
                profile = RiskProfileList.FirstOrDefault(i => i.Name == risk);
            }
            return profile;
        }

        internal void AddRiskProfile(RiskProfile rsk)
        {
            if (!RiskProfileList.Exists(i => i.Name == rsk.Name))
                RiskProfileList.Add(rsk);
        }

        internal void AddClientGroup(string groupname)
        {
            if (clientGroupList == null)
            {
                clientGroupList = new List<string>();
            }

            if (!clientGroupList.Contains(groupname))
            {
                clientGroupList.Add(groupname);
            }
        }

        internal List<string> GetClientGroups()
        {
            return clientGroupList;
        }

        internal long CreateClientNumber()
        {
            long num = 0;            
            if (clientList.Count > 0)
            {
                num = Convert.ToInt64(clientList.Keys.Last()) + 1;                
            }          
            else
            {
                num = Interlocked.Increment(ref UniqueClientID);//Here we assign First Unique Client Number
            }
            //clientList.Add(num, new Client ());
            return num;
        }

        internal string GetNameFormat(Client client)
        {
            string name = string.Empty;
            if (string.IsNullOrEmpty(client.NamePreference))
            {
                return client.FirstName + "," + client.Lastname;
            }
            string[] names = client.NamePreference.Split(',');
            foreach (var item in names)
            {
                switch (item.ToLower())
                {
                    case "firstname":
                        name += client.FirstName + " ";
                        break;
                    case "middlename":
                        name += client.MiddleName + " ";
                        break;
                    case "lastname":
                        name += client.Lastname + " ";
                        break;
                    default:
                        break;
                }
            }
            //name=name.Remove(name.Length-1);
            return name.Trim(); ;
        }

        #region CRM Operations
        private string AddClientInCRM(Client client)
        {
            VTiger vTiger = new VTiger(URL_CRM);
            vTiger.Login(CRM_USERNAME, CRM_ACCESSKEY);
            string usereId = vTiger.GetUserID(Properties.Settings.Default.vTigerUsername.Trim());
            string clientID = string.Empty;
            if (usereId != string.Empty)
            {

                VTigerAccount newUser = new VTigerAccount();
                newUser.accountname = client.ClientName;
                newUser.tks_firstname = client.FirstName;
                newUser.tks_middlename = client.MiddleName;
                newUser.tks_lastname = client.Lastname;
                newUser.cf_652 = client.Dateofbirth.ToString();
                newUser.tks_companyname = client.Company;
                newUser.tks_contactmethod = client.ContactMethod;
                newUser.tks_namepreferencefield = client.NamePreference;
                newUser.cf_655 = client.WorkPhone;
                newUser.cf_656 = client.HomePhone;
                newUser.cf_657 = client.CellPhone;
                newUser.cf_659 = client.ClientNumber;
                newUser.assigned_user_id = usereId;
                newUser.account_no = client.RealAccount;
                newUser.accounttype = Accounttype.Customer;
                newUser.bill_city = newUser.ship_city = client.City;
                newUser.bill_country = newUser.ship_country = client.Country;
                newUser.bill_state = newUser.ship_state = client.State;
                newUser.bill_street = newUser.ship_street = client.Address1;
                newUser.description = client.Notes;
                newUser.email1 = client.Email;
                newUser.phone = client.CellPhone;

                VTigerAccount RetUser = vTiger.Create(newUser);
                clientID = RetUser.account_no;
            }
            vTiger.Logout();
            return clientID;
        }
        private void RemoveClientInCRM(Client client)
        {
            VTiger vTiger = new VTiger(URL_CRM);
            vTiger.Login(CRM_USERNAME, CRM_ACCESSKEY);
            string usereId = vTiger.GetUserID(Properties.Settings.Default.vTigerUsername.Trim());
            string clientID = string.Empty;
            if (usereId != string.Empty)
            {
                
            }
        }

        //private bool LoggenInCRM()
        //{
 
        //}


         private bool NoClientExists(string name)
         {
             bool flag = false;
             VTiger vTiger = new VTiger(URL_CRM);
             vTiger.Login(CRM_USERNAME, CRM_ACCESSKEY);
             string usereId = vTiger.GetUserID(CRM_USERNAME);

             DataTable dt = vTiger.Retrieve(vTiger.GetAccountID(name));
             if (dt == null)// No duplicate Client exist so return true
             {
                 vTiger.Logout();
                 return true;
             }
             vTiger.Logout();
             return flag;
         }       

        #endregion


         internal void AddCashAccount(string str)
         {
             if (CashAccountList == null)
             {
                 CashAccountList = new List<string>();
             }

             if (!CashAccountList.Contains(str))
             {
                 CashAccountList.Add(str);
             }
         }

         internal List<string> GetCashAccounts()
         {
             return CashAccountList;
         }

         internal List<ClientQuotes> GetSymbolEOD(string currentSymbol)
         {
             List<ClientQuotes> lst = new List<ClientQuotes>();
             System.IO.Stream streamRead = System.IO.File.OpenRead(EODPath + "\\" + currentSymbol);
             BinaryFormatter binaryRead = new BinaryFormatter();
             lst = binaryRead.Deserialize(streamRead) as List<ClientQuotes>;
             streamRead.Close();
             return lst;
         }

         internal void UpdateRiskProfile(RiskProfile profile)
         {
             if (RiskProfileList.FirstOrDefault(i => i.Name == profile.Name) != null)
             {
                 //if (profile.Name != "DefaultRP")
                 {
                     int index = RiskProfileList.FindIndex(i => i.Name == profile.Name);
                     RiskProfileList[index] = profile;
                 }
             }      
         }

         internal Strategy GetStrategy(string str)
         {
             return ListStrategy.FirstOrDefault(i => i.Name == str);             
         }

         internal void AddStrategy(Strategy strNew)
         {             
             if (ListStrategy.Exists(i => i.Name == strNew.Name))
             {
                 int index = ListStrategy.FindIndex(i => i.Name == strNew.Name);
                 ListStrategy[index] = strNew;                 
             }
             else
             {
                 ListStrategy.Add(strNew);
             }
         }

         internal bool RemoveStrategy(string str)
         {
             if (ListStrategy.Exists(i => i.Name == str))
             {
                 int index = ListStrategy.FindIndex(i => i.Name == str);
                 ListStrategy.RemoveAt(index);
                 return true;
             }
             else
             {
                 return false;
             }
         }

         internal void AddExchange(string exch)
         {
             if (ExchangeList == null)
             {
                 ExchangeList = new List<string>();
             }

             if (!ExchangeList.Contains(exch))
             {
                 ExchangeList.Add(exch);
             }
         }

         internal List<string> GetExchanges()
         {
             return ExchangeList;
         }
         
         internal void SaveAssetReturnValues(List<string> lstSavedItems)
         {
             lstAssetReturn = lstSavedItems;
         }
    }
}
