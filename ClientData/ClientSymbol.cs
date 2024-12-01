using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    public class Asset
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }        
        public Dictionary<string, double> lstAssetWeight = new Dictionary<string, double>();
        public List<string> listAsset = new List<string>();
        public double Equity = 0;       
        public double Bonds = 0;        
        public double Cash = 0;        
        public double Property = 0;
        
    }
    [Serializable]
    public class IntAsset
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public double Equity = 0;
        public double Bonds = 0;
        public double Cash = 0;
        public double Property = 0;

        public Dictionary<string, double> lstIntAssetWeight = new Dictionary<string, double>();
        public List<string> listIntAsset = new List<string>();
    }
    [Serializable]
    public class Geographic
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public Dictionary<string, double> lstGeoWeight = new Dictionary<string, double>();
        public List<string> listGeo = new List<string>();
    }

    [Serializable]
    public class Currency
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public Dictionary<string, double> lstCurrWeight = new Dictionary<string, double>();
        public List<string> listCurrency = new List<string>();
    }
    [Serializable]
    public class Risk
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public Dictionary<string, double> lstRiskWeight = new Dictionary<string, double>();
        public List<string> listRisk = new List<string>();
    }
}
