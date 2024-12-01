using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Symbol Status Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public struct SymbolStatus : IMessage
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
        public char TradingStatus;
        [ProtoMember(7, IsRequired = true), MarshalAs(UnmanagedType.I1)]
        public byte Flags;
        [ProtoMember(8, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] HaltReason;
        [ProtoMember(9, IsRequired = true)]
        public byte SessionChangeReason;
        [ProtoMember(10, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] NewEndTime;
        [ProtoMember(11, IsRequired = true)]
        public byte BookType;
        public static int Length => Marshal.SizeOf(typeof(SymbolStatus));

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- SymbolStatus" + " Nanosecond :-" + Nanosecond.ToString()
                    + " InstrumentID :-" + InstrumentID.ToString() + " Reserved :-" + Reserved
                     + " Reserved1 :-" + Reserved1 + " TradingStatus :-" + TradingStatus
                     + " Flags :-" + Flags + " HaltReason :-" + new string(HaltReason).Trim()
                     + " SessionChangeReason :-" + SessionChangeReason
                     + " NewEndTime :-" + new string(NewEndTime).Trim()
                     + " BookType :-" + BookType;
        }
    }
}

