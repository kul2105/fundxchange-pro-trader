using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace InsightCapital.STTAPI.MessageLibrary
{   
    /// <summary>
    /// Author :- Rohit Kumar
    /// Proto Header Message with Each Message Packet
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public class ProtoHeader
    {
        [ProtoMember(1, IsRequired = true)]
        public MITCHMessageType MessageType;
        [ProtoMember(2, IsRequired = true)]
        public int SequenceNumber;
        [ProtoMember(3, IsRequired = true)]
        public uint InstrumentId;
        [ProtoMember(4, IsRequired = true)]
        public byte[] Message;
    }
}
