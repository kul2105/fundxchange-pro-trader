using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    public sealed class ColumnOrderItem
    {
        public int DisplayIndex { get; set; }
        public int Width { get; set; }
        public bool Visible { get; set; }
        public int ColumnIndex { get; set; }
        public bool BestFitAll { get; set; }
    }
}
