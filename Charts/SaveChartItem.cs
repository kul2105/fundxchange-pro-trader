using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FundXchange.Model.ViewModels.Charts;

namespace M4.Charts
{
    [Serializable]
    public class SaveChartItem
    {
        public string Symbol = string.Empty;
        public Periodicity Periodicity;
        public int Interval = 0;
        public int Bars = 0;
        public string FileName = string.Empty;

        public static SaveChartItem Initial(ChartSelection selection, string dir)
        {
            SaveChartItem retVal = new SaveChartItem();

            string sym = selection.Symbol;
            if (sym.Contains('/'))
                sym = sym.Replace("/", "").Trim();


            retVal.Symbol = sym;
            retVal.Interval = selection.Interval;
            retVal.Bars = selection.Bars;
            retVal.Periodicity = selection.Periodicity;
            retVal.FileName = dir + sym + "_" + selection.Periodicity.ToString() + "_" +
                    selection.Interval + "_" + selection.Bars + ".chart";
            return retVal;
        }
    }
}
