using System;

namespace PALSA.Cls
{
    public class ClsNewOrder
    {
        public uint Account { get; set; }
        public uint OrderID { get; set; }
        public string ClOrderID { get; set; }
        public string ContractName { get; set; }
        public string StrProductType { get; set; }
        public sbyte SbProductType { get; set; }
        public string ProductName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string GatewayName { get; set; }
        public double Qty { get; set; }
        public sbyte OrderType { get; set; }
        public double Price { get; set; }
        public double StopPX { get; set; }
        public sbyte Side { get; set; }
        public sbyte Tif { get; set; }
        public uint MinorDisclosedQty { get; set; }
        public sbyte PositionEffect { get; set; }
        public double Slipage { get; set; }
        public double StopLoss { get; set; }
        public double TakeProfit { get; set; }
        public string CloseClOrdID { get; set; }
        public string CloseOrderID { get; set; }
    }
}