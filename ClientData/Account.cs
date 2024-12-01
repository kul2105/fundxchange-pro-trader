using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    public class Account
    {
        private long _investmentNumber;

        public long InvestmentNumber
        {
            get { return _investmentNumber; }
            set { _investmentNumber = value; }
        }
        private string _client;
        public string Client
        {
            get { return _client; }
            set { _client = value; }
        }
        private string _accountType;
        public string AccountType
        {
            get { return _accountType; }
            set { _accountType = value; }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _group;
        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }
        private string _cashAccount;
        public string CashAccount
        {
            get { return _cashAccount; }
            set { _cashAccount = value; }
        }
        private string _riskRating;
        public string RiskRating
        {
            get { return _riskRating; }
            set { _riskRating = value; }
        }
        private long _account;
        public long AccountNumber
        {
            get { return _account; }
            set { _account = value; }
        }        
        private DateTime _inceptionDate;
        public DateTime InceptionDate
        {
            get { return _inceptionDate; }
            set { _inceptionDate = value; }
        }
        private DateTime _performancestartDate;
        public DateTime PerformancestartDate
        {
            get { return _performancestartDate; }
            set { _performancestartDate = value; }
        }
        private string _assetAllocation;
        public string AssetAllocation
        {
            get { return _assetAllocation; }
            set { _assetAllocation = value; }
        }
        private string _benchmark;
        public string Benchmark
        {
            get { return _benchmark; }
            set { _benchmark = value; }
        }
        private List<ClientPorfolio> _LstBenchMark;
       
        private string _advisor;
        public string Advisor
        {
            get { return _advisor; }
            set { _advisor = value; }
        }
        private string _accountGoal;
        public string AccountGoal
        {
            get { return _accountGoal; }
            set { _accountGoal = value; }
        }
        private string _tradingAllocation;
        public string TradingAllocation
        {
            get { return _tradingAllocation; }
            set { _tradingAllocation = value; }
        }    
        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        private ClientPorfolio _portfolio;
        public ClientPorfolio Portfolio
        {
            get { return _portfolio; }
            set { _portfolio = value; }
        }
 
        public Account()
        {
            _investmentNumber = 0;
         _client=string.Empty;
         _accountType=string.Empty;
         _description=string.Empty;
         _group=string.Empty;
         _cashAccount=string.Empty;
         _riskRating=string.Empty;
         _account=0;         
         _inceptionDate=DateTime.Now;
         _performancestartDate = DateTime.Now;
         _assetAllocation=string.Empty;
         _benchmark=string.Empty;
         _advisor=string.Empty;
         _accountGoal=string.Empty;
         _tradingAllocation=string.Empty;         
         _notes=string.Empty;
         _LstBenchMark = new List<ClientPorfolio>();
        }
    }
}
