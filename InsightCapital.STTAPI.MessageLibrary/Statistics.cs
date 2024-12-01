using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Statistics Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public struct Statistics :IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public MessageHeader messageHeader;
        [ProtoMember(2, IsRequired = true)]
        public uint Nanosecond;
        [ProtoMember(3, IsRequired = true)]
        public uint InstrumentID;
        [ProtoMember(4, IsRequired = true)]
        public char Reserved;
        [ProtoMember(5, IsRequired = true)]
        public char Reserved1;
        [ProtoMember(6, IsRequired = true)]
        public char StatisticType;
        [ProtoMember(7, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Price;
        [ProtoMember(8, IsRequired = true)]
        public char OpenCloseIndicator;
        [ProtoMember(9, IsRequired = true)]
        public byte SubBook;
        public static int Length => Marshal.SizeOf(typeof(Statistics));

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- Statistics " + " Nanosecond :-" + Nanosecond.ToString()
                    + " InstrumentID :-" + InstrumentID.ToString() + " Reserved :-" + Reserved
                     + " Reserved1 :-" + Reserved1 + " StatisticType :-" + StatisticType.ToString()
                     + " Price :-" + JSEConverter.ConvertPriceToLong(Price)
                     + " OpenCloseIndicator :-" + OpenCloseIndicator.ToString()
                     + " SubBook :-" + SubBook.ToString();
        }
    }
}
