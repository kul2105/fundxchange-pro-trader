using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Message Header
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public struct MessageHeader
    {
        [ProtoMember(1, IsRequired = true)]
        public ushort LengthOfMessage;
        [ProtoMember(2, IsRequired = true)]
        public byte MessageType;
        public static int Length => Marshal.SizeOf(typeof(MessageHeader));
    }
}
