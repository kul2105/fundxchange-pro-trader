using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonLibrary.Cls
{
    [Serializable]
    public class ClsPortfolio
    {
        private Dictionary<string, ClsContracts> _products = new Dictionary<string, ClsContracts>();
        private string _portfolioName;

        public string PortfolioName
        {
            get { return _portfolioName; }
            set { _portfolioName = value; }
        }

        public Dictionary<string, ClsContracts> Products
        {
            get { return _products; }
            set { _products = value; }
        }

        public bool AddProduct(string instrumentId, ClsContracts product)
        {
            if (!_products.Keys.Contains(instrumentId))
            {
                _products.Add(instrumentId, product);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveProduct(string instrumentId)
        {
            if (_products.Keys.Contains(instrumentId))
            {
                _products.Remove(instrumentId);
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}