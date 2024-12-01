using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Trade Break Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public struct TradeBreak : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public MessageHeader messageHeader;
        [ProtoMember(2, IsRequired = true)]
        public uint Nanosecond;
        [ProtoMember(3, IsRequired = true)]
        public ulong TradeID;
        [ProtoMember(4, IsRequired = true)]
        public char TradeType;
        public static int Length => Marshal.SizeOf(typeof(TradeBreak));

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- TradeBreak" + " Nanosecond :-" + Nanosecond.ToString()
                    + " TradeID :-" + TradeID.ToString() + " TradeType :-" + TradeType;
        }
    }
}
