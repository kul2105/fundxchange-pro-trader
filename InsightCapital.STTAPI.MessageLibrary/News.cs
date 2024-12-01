using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// News Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public struct News : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public MessageHeader messageHeader;
        [ProtoMember(2, IsRequired = true)]
        public uint Nanosecond;
        [ProtoMember(3, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] Time;
        [ProtoMember(4, IsRequired = true)]
        public char Urgency;
        [ProtoMember(5, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public char[] Headline;
        [ProtoMember(6, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 750)]
        public char[] Text;
        [ProtoMember(7, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public char[] Instruments;
        [ProtoMember(8, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public char[] Underlyings;
        public static int Length => Marshal.SizeOf(typeof(News));

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- News" + " Nanosecond :-" + Nanosecond.ToString()
                    + " Time :-" + new string(Time) + " Urgency :-" + Urgency
                     + " Headline :-" + new string(Headline) + " Text :-" + new string(Text)
                     + " Instruments :-" + new string(Instruments) + " Underlyings :-" + new string(Underlyings);
        }
    }
}
