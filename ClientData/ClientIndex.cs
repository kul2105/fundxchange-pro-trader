using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    public class ClientIndex
    {
        private string _indexName;
        public string IndexName
        {
            get { return _indexName; }
            set { _indexName = value; }
        }
        private string _indexNumber;
        public string IndexNumber
        {
            get { return _indexNumber; }
            set { _indexNumber = value; }
        }
        
        private List<IndexSymbol> symbolDetail;

        private int _basePrice;
        public int BasePrice
        {
            get { return _basePrice; }
            set { _basePrice = value; }
        }

        private double _initialValuation;
        public double InitialValuation
        {
            get { return _initialValuation; }
            set { _initialValuation = value; }
        }

        public ClientIndex()
        {
            _indexNumber = string.Empty;
            _indexName = string.Empty;
            symbolDetail = new List<IndexSymbol>();
        }
        public void RemoveSymbol(string symbolName)
        {
            if (symbolDetail.Count > 0)
            {
                IndexSymbol symbol = symbolDetail.FirstOrDefault(i => i.SymbolName == symbolName);
                if (symbolDetail.Contains(symbol))
                    symbolDetail.Remove(symbol);
            }
        }
        public void AddSymbol(IndexSymbol symbol)
        {
            if (!symbolDetail.Contains(symbol))
                symbolDetail.Add(symbol);
        }
        public void UpdateSymbol(IndexSymbol symbol)
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
        public List<IndexSymbol> GetAllSymbol()
        {
            return symbolDetail;
        }
        public IndexSymbol GetSymbol(string symbolName)
        {
            IndexSymbol symbol = null;
            if (symbolDetail.Count > 0)
            {
                symbol = symbolDetail.FirstOrDefault(i => i.SymbolName == symbolName);
            }
            return symbol;
        }
        
    }
}
