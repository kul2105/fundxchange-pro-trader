using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Depth Element Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public struct DepthElements : IComparable<DepthElements>
    {
        [ProtoMember(1, IsRequired = true)]
        public uint InstrumentID;
        [ProtoMember(2, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public char[] OrderID;
        [ProtoMember(3, IsRequired = true)]
        public uint Quantity;
        [ProtoMember(4, IsRequired = true)]
        public double Price;
        [ProtoMember(5, IsRequired = true)]
        public int SequenceNumber;
        [ProtoMember(6, IsRequired = true)]
        public char BuySell;
        [ProtoMember(7, IsRequired = true)]
        public long Date;
        [ProtoMember(8, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public char[] MemberCode;
        [ProtoMember(9, IsRequired = true)]
        public AddOrderFlags Flags;
        public override bool Equals(object oth)
        {
            if (oth == null)
            {
                return false;
            }
            DepthElements elements = (DepthElements)oth;
            return ((this.InstrumentID == elements.InstrumentID) && ((this.BuySell == elements.BuySell) && ((this.Price == elements.Price) && ((this.Quantity == elements.Quantity) && (this.OrderID.Equals(elements.OrderID) && this.MemberCode.Equals(elements.MemberCode))))));
        }

        public int CompareTo(DepthElements other)
        {
            if (this.Equals(other))
            {
                return 0;
            }
            if ((this.BuySell == 'B') || (this.BuySell == 'b'))
            {
                return ((this.Price >= other.Price) ? ((this.Price != other.Price) ? -1 : ((this.SequenceNumber >= other.SequenceNumber) ? 1 : -1)) : 1);
            }
            return ((this.Price <= other.Price) ? ((this.Price != other.Price) ? -1 : ((this.SequenceNumber >= other.SequenceNumber) ? 1 : -1)) : 1);
        }

        public override int GetHashCode() => this.SequenceNumber;

        public override string ToString()
        {
            return "Message Type :- Depth Elemment" + " InstrumentID :-" + InstrumentID.ToString()
                    + " OrderID :-" + new string(OrderID) + " Quantity :-" + Quantity.ToString()
                     + " Price :-" + Price.ToString() + " SequenceNumber :-" + SequenceNumber.ToString()
                     + " BuySell :-" + BuySell + " Date :-" + new DateTime(Date)
                     + " MemberCode :-" + new string(MemberCode)
                     + " Flags :-" + Flags.ToString();
        }
    }
}
