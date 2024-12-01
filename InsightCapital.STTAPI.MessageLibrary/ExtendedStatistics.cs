using ProtoBuf;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Extended Statistics Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public struct ExtendedStatistics : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public MessageHeader messageHeader;
        [ProtoMember(2, IsRequired = true)]
        public uint Nanosecond;
        [ProtoMember(3, IsRequired = true)]
        public uint InstrumentID;
        [ProtoMember(4, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] HighPrice;
        [ProtoMember(5, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] LowPrice;
        [ProtoMember(6, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] VWAP;
        [ProtoMember(7, IsRequired = true)]
        public uint Volume;
        [ProtoMember(8, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Turnover;
        [ProtoMember(9, IsRequired = true)]
        public uint NumberOfTrades;
        [ProtoMember(10, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] Reserverd;
        [ProtoMember(11, IsRequired = true)]
        public char SubBook;
        [ProtoMember(12, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] NotionalExposure;
        [ProtoMember(13, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] NotionalDeltaExposure;
        [ProtoMember(14, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] OpenInterest;
        public static int Length => Marshal.SizeOf(typeof(ExtendedStatistics));

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- ExtendedStatistics" + " Nanosecond :-" + Nanosecond.ToString()
                    + "InstrumentID :-" + InstrumentID.ToString() + "HighPrice :-" + JSEConverter.ConvertPriceToDouble(HighPrice)
                     + "LowPrice :-" + JSEConverter.ConvertPriceToDouble(LowPrice) + "VWAP :-" + JSEConverter.ConvertPriceToDouble(VWAP)
                     + "Volume :-" + Volume.ToString() + "Turnover :-" + JSEConverter.ConvertTurnoverToDouble(Turnover)
                     + "NumberOfTrades :-" + NumberOfTrades.ToString() + "Reserverd :-" + new string(Reserverd)
                     + "SubBook :-" + SubBook.ToString() + "NotionalExposure :-" + JSEConverter.ConvertPriceToDouble(NotionalExposure)
                     + "NotionalDeltaExposure :-" + JSEConverter.ConvertPriceToDouble(NotionalDeltaExposure)
                     + "OpenInterest :-" + JSEConverter.ConvertPriceToDouble(OpenInterest);
        }
    }
}


