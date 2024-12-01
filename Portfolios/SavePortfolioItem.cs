using System;
using System.Collections.Generic;

namespace M4.Portfolios
{
    [Serializable]
    public class SavePortfolioItem
    {
        public List<M4.Portfolios.Portfolio> m_portfolios = new List<M4.Portfolios.Portfolio>();
        public string m_selectedindex;
        public static SavePortfolioItem SavePortfolio(DDFDataManager DM)
        {
            SavePortfolioItem retval = new SavePortfolioItem();
            for (int k = 0; k < DM.Portfolios.Count; k++)
            {
                retval.m_portfolios.Add(DM.Portfolios[k]);
            }
            retval.m_selectedindex = DM.cmbxPortfolio.SelectedText;
            return retval;
        }
    }
}
