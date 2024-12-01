using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    public class Mandate
    {
        public string portfolioName = string.Empty;

        public string Equity = string.Empty;
        public string IntEquity = string.Empty;
        public string Bonds = string.Empty;
        public string IntBonds = string.Empty;
        public string Cash = string.Empty;
        public string IntCash = string.Empty;
        public string Property = string.Empty;
        public string IntProperty = string.Empty;
        public string userAsset1 = string.Empty;
        public string userAsset1Val = string.Empty;
        public string userAsset2 = string.Empty;
        public string userAsset2Val = string.Empty;
        public string userAsset3 = string.Empty;
        public string userAsset3Val = string.Empty;
        public string userAsset4 = string.Empty;
        public string userAsset4Val = string.Empty;
        public string userAsset5 = string.Empty;
        public string userAsset5Val = string.Empty;

        public string PortfolioRisk = string.Empty;
        public string InstrumentRisk = string.Empty;
        public string TimeHorizon = string.Empty;
        public string BenchMark = string.Empty;
        public string PortDescript = string.Empty;
        public string ClientProfile = string.Empty;
        public string PortfolioManager = string.Empty;

        public bool IsLongShort = false;
        public bool IsLong = false;
        public bool IsShort = false;
    }
}
