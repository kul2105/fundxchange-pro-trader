using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.Runtime.InteropServices;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Heart Beat Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract]
    public class HeartBeat : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public long LastPrimaryUDPMessage = DateTime.Now.Ticks;
        [ProtoMember(2, IsRequired = true)]
        public long LastBackupUDPMessage = DateTime.Now.Ticks;
        [ProtoMember(3, IsRequired = true)]
        public long CurrentTime = DateTime.Now.Ticks;
        [ProtoMember(4, IsRequired = true)]
        public ReplayRecoveryStatus PrimaryReplay = ReplayRecoveryStatus.NotUsedYet;
        [ProtoMember(5, IsRequired = true)]
        public ReplayRecoveryStatus SecondaryReplay = ReplayRecoveryStatus.NotUsedYet;
        [ProtoMember(6, IsRequired = true)]
        public ReplayRecoveryStatus PrimaryRecovery = ReplayRecoveryStatus.NotUsedYet;
        [ProtoMember(7, IsRequired = true)]
        public ReplayRecoveryStatus SecondaryRecovery = ReplayRecoveryStatus.NotUsedYet;

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- HeartBeat" + " LastPrimaryUDPMessage :-" + new DateTime(LastPrimaryUDPMessage).ToString()
                    + " LastBackupUDPMessage :-" + new DateTime(LastBackupUDPMessage).ToString() + "CurrentTime :-" + new DateTime(CurrentTime).ToString()
                     + " PrimaryReplay :-" + PrimaryReplay.ToString() + " SecondaryReplay :-" + SecondaryReplay.ToString()
                     + " PrimaryRecovery :-" + PrimaryRecovery.ToString() + " SecondaryRecovery :-" + SecondaryRecovery.ToString();
        }
    }
}