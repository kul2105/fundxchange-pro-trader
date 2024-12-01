using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Logon Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public class Logon : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public bool Connected;
        [ProtoMember(2, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public char[] IPAddress;
        [ProtoMember(3, IsRequired = true)]
        public int Port;

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- Logon" + " Connected :-" + Connected.ToString()
                    + " IPAddress :-" + new string(IPAddress) + " Port :-" + Port.ToString();
        }
    }
}


