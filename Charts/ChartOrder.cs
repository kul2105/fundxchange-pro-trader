using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.Charts
{
    internal class ChartOrder
    {
        public ctlOrderTAB.Orders.Side OrderSide;
        public double EntryPrice;
        public int ChartRecord;
        public string ChartObjectLineName;
        public string ChartObjectTextName;
        public string ChartObjectSymbolName;
        public bool Executed = false;
        public int Quantity = 0;
    }
}
