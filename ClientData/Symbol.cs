using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    public class Symbol
    {
        private string _symbolName;
        public string SymbolName
        {
            get { return _symbolName; }
            set { _symbolName = value; }
        }
        private string _cusip;
        public string CUSIP
        {
            get { return _cusip; }
            set { _cusip = value; }

        }
        public string OrginalSymbolName;
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _securityType;
        public string SecurityType
        {
            get { return _securityType; }
            set { _securityType = value; }
        }
        private double _AmountInvested;
        public double AmountInvested
        {
            get { return _AmountInvested; }
            set { _AmountInvested = value; }
        }
        private string _PaymentFrequency;
        public string PaymentFrequency
        {
            get { return _PaymentFrequency; }
            set { _PaymentFrequency = value; }
        }

        private DateTime _MaturityDate;
        public DateTime MaturityDate
        {
            get { return _MaturityDate; }
            set { _MaturityDate = value; }
        }

        //-----New fields
        private string _MaturityValue;
        public string MaturityValue
        {
            get { return _MaturityValue; }
            set { _MaturityValue = value; }
        }
        private string _CurrentValue;
        public string CurrentValue
        {
            get { return _CurrentValue; }
            set { _CurrentValue = value; }
        }
        private string _ContriIncreasePer;
        public string ContriIncreasePer
        {
            get { return _ContriIncreasePer; }
            set { _ContriIncreasePer = value; }
        }
        private string _RecurringContri;
        public string RecurringContri
        {
            get { return _RecurringContri; }
            set { _RecurringContri = value; }
        }
        private double _AnnualPerReturn;
        public double AnnualPerReturn
        {
            get { return _AnnualPerReturn; }
            set { _AnnualPerReturn = value; }
        }       
        //----
        private string _InterestRate;
        public string InterestRate
        {
            get { return _InterestRate; }
            set { _InterestRate = value; }
        }
        private DateTime _IssuedDate;
        public DateTime IssuedDate
        {
            get { return _IssuedDate; }
            set { _IssuedDate = value; }
        }       
        private string _assetClass;
        public string AssetClass
        {
            get { return _assetClass; }
            set { _assetClass = value; }
        }
        private string _industry;
        public string Industry
        {
            get { return _industry; }
            set { _industry = value; }
        }
        private string _sector;
        public string Sector
        {
            get { return _sector; }
            set { _sector = value; }
        }
        private string _BenchMark;
        public string BenchMark
        {
            get { return _BenchMark; }
            set { _BenchMark = value; }
        }
        private string _Exchange;
        public string Exchange
        {
            get { return _Exchange; }
            set { _Exchange = value; }
        }
        private string _RiskCountry;
        public string RiskCountry
        {
            get { return _RiskCountry; }
            set { _RiskCountry = value; }
        }
        private string _IssueCountry;
        public string IssueCountry
        {
            get { return _IssueCountry; }
            set { _IssueCountry = value; }
        }
        private string _Notes;
        public string Notes
        {
            get { return _Notes; }
            set { _Notes = value; }
        }

        private double _units;
        public double Units
        {
            get { return _units; }
            set { _units = value; }
        }
        private double _perWeighting;
        public double PerWeighting
        {
            get { return _perWeighting; }
            set { _perWeighting = value; }
        }
        public Asset _Asset = new Asset();
        public IntAsset _IntAsset = new IntAsset();
        public Geographic geo = new Geographic();
        public Currency curr = new Currency();
        public Risk rsk = new Risk();

        public bool IsNonExchange = false;

        private double _lastHigh;
        public double LastHighPrc
        {
            get { return _lastHigh; }
            set { _lastHigh = value; }
        }
        private string _InitialPrice;
        public string InitialPrice
        {
            get { return _InitialPrice; }
            set { _InitialPrice = value; }
        }
        private string _AssetAllocation;
        public string AssetAllocation
        {
            get { return _AssetAllocation; }
            set { _AssetAllocation = value; }
        }
        public bool Is4Strategy = false;

        private bool _Is4Trade;
        public bool Is4Trade
        {
            get { return _Is4Trade; }
            set { _Is4Trade = value; }
        }

        private double _CurrentPrc;
        public double CurrentPrc
        {
            get { return _CurrentPrc; }
            set { _CurrentPrc = value; }
        }

        public Dictionary<string, string> dictStrategyResult = new Dictionary<string, string>();
        public Dictionary<string, Dictionary<DateTime, double>> dictStrategyDTPrice = new Dictionary<string, Dictionary<DateTime, double>>();
        public Symbol()
        {
            _symbolName = string.Empty;
            OrginalSymbolName = string.Empty;
            _cusip = string.Empty;
            _description = string.Empty;
            _securityType = string.Empty;
            _AmountInvested = 0;
            _PaymentFrequency = string.Empty;
            _MaturityDate = DateTime.Now;
            _MaturityValue = string.Empty;
            _CurrentValue = string.Empty;
            _ContriIncreasePer = string.Empty;
            _RecurringContri = string.Empty;
            _AnnualPerReturn = 0;
            _InterestRate = string.Empty;
            _IssuedDate = DateTime.Now;
            _assetClass = string.Empty;
            _industry = string.Empty;
            _sector = string.Empty;
            _BenchMark = string.Empty;
            _Exchange = string.Empty;
            _RiskCountry = string.Empty;
            _IssueCountry = string.Empty;
            _Notes = string.Empty;
            _AssetAllocation = string.Empty;
            _units = 0;
            _perWeighting = 0;
            _InitialPrice = string.Empty;
            //quotes = new List<ClientQuotes>();
        }
        //public void RemoveQuotes(DateTime date)
        //{
        //    if (quotes.Count > 0)
        //    {
        //        ClientQuotes quote = quotes.FirstOrDefault(i => i.Date == date);
        //        if (quotes.Contains(quote))
        //            quotes.Remove(quote);
        //    }
            
        //}
        //public void AddQuotes(ClientQuotes quote)
        //{
        //    if (!quotes.Contains(quote))
        //        quotes.Add(quote);
        //}
        //public void UpdateQuotes(ClientQuotes quote)
        //{
        //    if (quotes.Exists(i => i.Date == quote.Date))
        //    {
        //        int index = quotes.FindIndex(i => i.Date == quote.Date);
        //        quotes[index] = quote;
        //    }
        //    else
        //    {
        //        quotes.Add(quote);
        //    }
        //}
        //public List<ClientQuotes> GetAllQuotes()
        //{
        //    return quotes;
        //}
        //public ClientQuotes GetQuote(DateTime date)
        //{
        //    ClientQuotes quote = null;
        //    if (quotes.Count > 0)
        //    {
        //        quote = quotes.FirstOrDefault(i => i.Date == date);
        //    }
        //    return quote;
        //}
    }
}
