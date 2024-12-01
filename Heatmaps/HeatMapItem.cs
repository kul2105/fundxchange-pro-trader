using System;
using System.Drawing;

namespace M4.Heatmaps
{
    public class HeatMapItem
    {
        private float m_AreaWeight;
        private float m_Area;
        private SizeF m_ViewSize = SizeF.Empty;
        private Brush m_Background;
        private float m_ColorWeight;
        private object m_Data;

        #region Properties

        public float AreaWeight
        {
            get { return m_AreaWeight; }
            set { m_AreaWeight = value; }
        }

        public float Area
        {
            get { return m_Area; }
            set { m_Area = value; }
        }

        public SizeF ViewSize
        {
            get { return m_ViewSize; }
            set { m_ViewSize = value; }
        }

        public float Ratio
        {
            get { return Math.Max(ViewSize.Width, ViewSize.Height) / Math.Min(ViewSize.Width, ViewSize.Height); }
        }

        public Brush Background
        {
            get { return m_Background; }
            set { m_Background = value; }
        }

        public float ColorWeight
        {
            get { return m_ColorWeight; }
            set { m_ColorWeight = value; }
        }

        public object Data
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

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
