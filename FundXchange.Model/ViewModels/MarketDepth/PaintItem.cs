using System;
using System.Collections.Generic;
using System.Drawing;

namespace FundXchange.Model.ViewModels.MarketDepth
{
    public class PaintItem
    {
        public List<long> Size = new List<long>();
        public List<Color> Color = new List<Color>();
        public long Max = 0;
    }
}
