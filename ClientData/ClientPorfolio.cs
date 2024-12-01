using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    public class ClientPorfolio
    {
        private string _porfolioName;
        public string PorfolioName
        {
            get { return _porfolioName; }
            set { _porfolioName = value; }
        }
        //private string _RetAmtWithdrawn;
        //public string RetAmtWithdrawn
        //{
        //    get { return _RetAmtWithdrawn; }
        //    set { _RetAmtWithdrawn = value; }
        //}
        //public long PorfolioNumber
        //{
        //    get { return _porfolioNumber; }
        //    set { _porfolioNumber = value; }
        //}
        private List<Symbol> symbolDetail;
        public List<Mandate> lstmandate;

        private double _portfolioValue;
        public double PortfolioValue
        {
            get { return _portfolioValue; }
            set { _portfolioValue = value; }
        }
        private DateTime _porfolioDate;
        public DateTime PorfolioDate
        {
            get { return _porfolioDate; }
            set { _porfolioDate = value; }
        }
        private double _portfolioLastHigh;
        public double PortfolioLastHigh
        {
            get { return _portfolioLastHigh; }
            set { _portfolioLastHigh = value; }
        }
        private double _portfolioUnits;
        public double PortfolioUnits
        {
            get { return _portfolioUnits; }
            set { _portfolioUnits = value; }
        }
        private double _portAvgReturn;
        public double PortAvgReturn
        {
            get { return _portAvgReturn; }
            set { _portAvgReturn = value; }
        }
        
        public Dictionary<string, double> dictAssetWeighting = new Dictionary<string, double>();
        public Dictionary<string, List<double>> dictAssetCalculatedWeighting = new Dictionary<string, List<double>>();       
        public RiskProfile _RiskProfile;

        private double _MaestroCapitalInvested;
        public double MaestroCapitalInvested
        {
            get { return _MaestroCapitalInvested; }
            set { _MaestroCapitalInvested = value; }
        }
        private double _MaestroCapitalPerTrade;
        public double MaestroCapitalPerTrade
        {
            get { return _MaestroCapitalPerTrade; }
            set { _MaestroCapitalPerTrade = value; }
        }

        private double _MaestroBrokerage;
        public double MaestroBrokerage
        {
            get { return _MaestroBrokerage; }
            set { _MaestroBrokerage = value; }
        }

        private double _MaestroBrokeragePer;
        public double MaestroBrokeragePer
        {
            get { return _MaestroBrokeragePer; }
            set { _MaestroBrokeragePer = value; }
        }
        public bool Is4Maestro = false;

        public Dictionary<string, double> lstAssetOptmizer;
            public Dictionary<string, ClientPorfolio> DictLinkedPortfolios;
            private double _PortfolioRisk;
            public double PortfolioRisk
            {
                get { return _PortfolioRisk; }
                set { _PortfolioRisk = value; }
            }
            private double _SymbolRisk;
            public double SymbolRisk
            {
                get { return _SymbolRisk; }
                set { _SymbolRisk = value; }
            }
        public ClientPorfolio()
        {
            //_porfolioNumber = 0;
            _porfolioName = string.Empty;
            symbolDetail = new List<Symbol>();
            lstmandate = new List<Mandate>();
            _RiskProfile = new RiskProfile();
            lstAssetOptmizer = new Dictionary<string, double>();
            DictLinkedPortfolios = new Dictionary<string, ClientPorfolio>();
        }

        public void ClearSymbolList()
        {
            symbolDetail.Clear();
        }
        public void RemoveSymbol(string symbolName)
        {
            if (symbolDetail.Count > 0)
            {
               Symbol symbol = symbolDetail.FirstOrDefault(i => i.SymbolName == symbolName);
                if (symbolDetail.Contains(symbol))
                    symbolDetail.Remove(symbol);
            }
        }
        public void AddSymbol(Symbol symbol)
        {
            if (!symbolDetail.Contains(symbol))
            symbolDetail.Add(symbol);
        }
        public void UpdateSymbol(Symbol symbol)
        {
            if (symbolDetail.Exists(i => i.SymbolName == symbol.SymbolName))
            {
                int index = symbolDetail.FindIndex(i => i.SymbolName == symbol.SymbolName);
                symbolDetail[index] = symbol;
            }
            else
            {
                symbolDetail.Add(symbol);
            }
        }
        public List<Symbol> GetAllSymbol()
        {
            return symbolDetail;
        }
        public Symbol GetSymbol(string symbolName)
        {
            Symbol symbol = null;
            if (symbolDetail.Count > 0)
            {
                symbol = symbolDetail.FirstOrDefault(i => i.SymbolName == symbolName);
            }
            return symbol;
        }


        internal void AddMandate(Mandate clsMandate)
        {
            lstmandate.Clear();
            lstmandate.Add(clsMandate);
        }        

        internal List<Mandate> GetMandate()
        {
            return lstmandate;
        }

        internal int GetBreachLevel(string symb)
        {
            return 0;
            //return GetSymbol(symb).GetBreachLevel();
        }
    }
}
