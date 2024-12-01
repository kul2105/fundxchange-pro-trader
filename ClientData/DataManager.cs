using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
   public class DataManager
    {
        private List<Porfolio> _lstPortfolio;
        public List<Porfolio> PortfolioList
        {
            get { return _lstPortfolio; }
            set { _lstPortfolio = value; }
        }
        private List<Index> _lstIndex;
        public List<Index> IndexList
        {
            get { return _lstIndex; }
            set { _lstIndex = value; }
        }
        public DataManager()
        {
            _lstPortfolio = new List<Porfolio>();
            _lstIndex = new List<Index>();
        }
    }
}
