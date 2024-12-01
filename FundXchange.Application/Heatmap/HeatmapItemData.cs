using System;
using System.Drawing;

namespace FundXchange.Application.Heatmap
{
    public class HeatMapItemData
    {
        #region Properties

        public string Symbol { get; set; }
        public RectangleF ItemRectangle { get; set; }
        public bool IsSelected { get; set; }
        public string CompanyName { get; set; }
        public string Exchange { get; set; }
        public double Rm { get; set; }
        public double PerMove { get; set; }
        public double Move { get; set; }
        public double Price { get; set; }

        #endregion

        #region Constructors

        public HeatMapItemData()
        {
        }

        public HeatMapItemData(string symbol)
            : this()
        {
            Symbol = symbol;
        }

        #endregion
    }
}
