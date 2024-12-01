using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Auction Info Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public struct AuctionInfo : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public MessageHeader messageHeader;
        [ProtoMember(2, IsRequired = true)]
        public uint Nanosecond;
        [ProtoMember(3, IsRequired = true)]
        public uint PairedQuantity;
        [ProtoMember(4, IsRequired = true)]
        public uint Reserved;
        [ProtoMember(5, IsRequired = true)]
        public char ImbalanceDirection;
        [ProtoMember(6, IsRequired = true)]
        public uint InstrumentID;
        [ProtoMember(7, IsRequired = true)]
        public char Reserved1;
        [ProtoMember(8, IsRequired = true)]
        public char Reserved2;
        [ProtoMember(9, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Price;
        [ProtoMember(10, IsRequired = true)]
        public char AuctionType;
        public static int Length => Marshal.SizeOf(typeof(AuctionInfo));

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- AuctionInfo" + "Nanosecond :-" + Nanosecond.ToString()
                    + "PairedQuantity :-" + PairedQuantity.ToString() + "Reserved :-" + Reserved.ToString()
                     + "ImbalanceDirection :-" + ImbalanceDirection.ToString() + "InstrumentID :-" + InstrumentID.ToString()
                     + "Reserved1 :-" + Reserved1.ToString() + "Reserved2 :-" + Reserved2.ToString()
                     + "Price :-" + Price.ToString() + "AuctionType :-" + AuctionType.ToString();
        }
    }
}
