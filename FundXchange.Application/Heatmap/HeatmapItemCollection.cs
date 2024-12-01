using System;
using System.Collections.Generic;

namespace FundXchange.Application.Heatmap
{
    public class HeatMapItemCollection : List<HeatMapItem>
    {
        #region Constructors

        public HeatMapItemCollection()
        {
        }

        public HeatMapItemCollection(IEnumerable<HeatMapItem> sourceCollection)
            : base(sourceCollection)
        {
        }

        #endregion
    }
}
