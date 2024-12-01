using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FundXchange.Application.Heatmap
{
    public class HeatMapItem
    {
        #region Properties

        public float AreaWeight { get; set; }
        public float Area { get; set; }
        public SizeF ViewSize { get; set; }
        public float Ratio
        {
            get { return Math.Max(ViewSize.Width, ViewSize.Height) / Math.Min(ViewSize.Width, ViewSize.Height); }
        }

        public Brush Background { get; set; }
        public float ColorWeight { get; set; }
        public object Data { get; set; }

        #endregion

        #region Constructors

        public HeatMapItem(float areaWeight, float colorWeight)
        {
            AreaWeight = areaWeight;
            ColorWeight = colorWeight;
        }

        public HeatMapItem(float areaWeight, float colorWeight, object data)
            : this(areaWeight, colorWeight)
        {
            Data = data;
        }

        #endregion

        #region Comparison

        public static int CompareByAreaAsc(HeatMapItem left, HeatMapItem right)
        {
            if (left == null || right == null)
                return 1;
            return (int)(left.Area - right.Area);
        }

        public static int CompareByAreaDesc(HeatMapItem left, HeatMapItem right)
        {
            if (left == null || right == null)
                return 1;
            return (int)(right.Area - left.Area);
        }

        #endregion
    }
}
