using System;
using System.Collections.Generic;

namespace M4.Heatmaps
{
    public class HeatMapItemCollection : List<HeatMapItem>
    {
        #region Constructors

        public HeatMapItemCollection()
            : base()
        {
        }

        public HeatMapItemCollection(IEnumerable<HeatMapItem> sourceCollection)
            : base(sourceCollection)
        {
        }

        #endregion
    }
}
