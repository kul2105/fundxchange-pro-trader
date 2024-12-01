using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    public class IndexSymbol
    {

        private string _symbolName;
        public string SymbolName
        {
            get { return _symbolName; }
            set { _symbolName = value; }
        }
        private string _orginalSymbolName;
        public string OrginalSymbolName
        {
            get { return _orginalSymbolName; }
            set { _orginalSymbolName = value; }
        }
        private string _cusip;
        public string CUSIP
        {
            get { return _cusip; }
            set { _cusip = value; }

        }
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
        private long _currentValue;
        public long CurrentValue
        {
            get { return _currentValue; }
            set { _currentValue = value; }
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
        private bool _isIndex;

        public bool IsIndex
        {
            get { return _isIndex; }
            set { _isIndex = value; }
        }
        private double _SharePrice;
        public double SharePrice
        {
            get { return _SharePrice; }
            set { _SharePrice = value; }
        }
        public Dictionary<string, string> dictStrategyResult = new Dictionary<string, string>();
        public Dictionary<string, Dictionary<DateTime, double>> dictStrategyDTPrice = new Dictionary<string, Dictionary<DateTime, double>>();
        public IndexSymbol()
        {
            _symbolName = string.Empty;
            _cusip = string.Empty;
            _description = string.Empty;
            _securityType = string.Empty;
            _assetClass = string.Empty;
            _industry = string.Empty;
            _sector = string.Empty;
            _currentValue = 0;
            _units = 0;
            _perWeighting = 0;
            _isIndex = false;
            _orginalSymbolName = string.Empty;
            _SharePrice = 0;
        }
    }
}
