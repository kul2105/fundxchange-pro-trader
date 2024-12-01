using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Fabricated Trade Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public class FabricatedTrade : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public uint InstrumentId;
        [ProtoMember(2, IsRequired = true)]
        public char BuySell;
        [ProtoMember(3, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public char[] OrderId;
        [ProtoMember(4, IsRequired = true)]
        public double Price;
        [ProtoMember(5, IsRequired = true)]
        public int Quantity;
        [ProtoMember(6, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public char[] TradeId;
        [ProtoMember(7, IsRequired = true)]
        public double LastOptPx;
        [ProtoMember(8, IsRequired = true)]
        public double Volatility;
        [ProtoMember(9, IsRequired = true)]
        public double UnderlyingReferencePrice;
        [ProtoMember(10, IsRequired = true)]
        public int SequenceNumber;
        [ProtoMember(11, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x16)]
        public long TradeTime;
        [ProtoMember(12, IsRequired = true)]
        public bool IsOffBook;

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- FabricatedTrade"
                     + " InstrumentID :-" + InstrumentId.ToString() + " BuySell :-" + BuySell.ToString()
                     + " OrderId :-" + new string(OrderId) + " Price :-" + Price.ToString()
                     + " Quantity :-" + Quantity.ToString() + " TradeId :-" + new string(TradeId)
                     + " LastOptPx :-" + LastOptPx.ToString() + " Volatility :-" + Volatility.ToString()
                     + " UnderlyingReferencePrice :-" + UnderlyingReferencePrice.ToString()
                     + " SequenceNumber :-" + SequenceNumber.ToString()
                     + " TradeTime :-" + TradeTime + " IsOffBook :-" + IsOffBook.ToString();
        }
    }
}
