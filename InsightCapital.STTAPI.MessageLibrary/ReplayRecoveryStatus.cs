using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace InsightCapital.STTAPI.MessageLibrary
{
    [ProtoContract]
    public enum ReplayRecoveryStatus
    {
        [ProtoEnum(Name = "NotUsedYet", Value = 0)]
        NotUsedYet = 0,
        [ProtoEnum(Name = "LoginAccepted", Value = 1)]
        LoginAccepted = 1,
        [ProtoEnum(Name = "CompIdInactiveOrLocked", Value = 2)]
        CompIdInactiveOrLocked = 2,
        [ProtoEnum(Name = "LoginLimitReached", Value = 3)]
        LoginLimitReached = 3,
        [ProtoEnum(Name = "ServiceUnavailable", Value = 4)]
        ServiceUnavailable = 4,
        [ProtoEnum(Name = "ConcurrentLimitReached", Value = 5)]
        ConcurrentLimitReached = 5,
        [ProtoEnum(Name = "Failed_Other", Value = 6)]
        Failed_Other = 6,
        [ProtoEnum(Name = "ReplayAccepted", Value = 7)]
        ReplayAccepted = 7,
        [ProtoEnum(Name = "ReplayLimitReached", Value = 8)]
        ReplayLimitReached = 8,
        [ProtoEnum(Name = "InvalidMarketGroup", Value = 9)]
        InvalidMarketGroup = 9,
        [ProtoEnum(Name = "OutOfRange", Value = 10)]
        OutOfRange = 10,
        [ProtoEnum(Name = "ReplayUnavailable", Value = 11)]
        ReplayUnavailable = 11,
        [ProtoEnum(Name = "UnsupportedMessageType", Value = 12)]
        UnsupportedMessageType = 12,
        [ProtoEnum(Name = "RecoveryAccepted", Value = 13)]
        RecoveryAccepted = 13,
        [ProtoEnum(Name = "RecoveryLimitReached", Value = 14)]
        RecoveryLimitReached = 14,
        [ProtoEnum(Name = "SnapshotUnavailable", Value = 15)]
        SnapshotUnavailable = 15,
        [ProtoEnum(Name = "SegmentInstrumentOrSubBookMissing", Value = 0x10)]
        SegmentInstrumentOrSubBookMissing = 0x10,
        [ProtoEnum(Name = "Unknown", Value = 0x11)]
        Unknown = 0x11,
        [ProtoEnum(Name = "UnableToConnect", Value = 0x12)]
        UnableToConnect = 0x12
    }
}