using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace InsightCapital.STTAPI.MessageLibrary
{
    [ProtoContract]
    public enum TradeFlags
    {
        [ProtoEnum(Name = "Normal", Value = 0)]
        Normal = 0,
        [ProtoEnum(Name = "TradeConditionFlag", Value = 1)]
        TradeConditionFlag = 1,
        [ProtoEnum(Name = "CrossOrderTrade", Value = 2)]
        CrossOrderTrade = 2
    }

    [ProtoContract]
    public enum OrderModifiedFlag
    {
        [ProtoEnum(Name = "PriorityLost", Value = 0)]
        PriorityLost = 0,
        [ProtoEnum(Name = "PriorityRetained", Value = 1)]
        PriorityRetained = 1
    }

    [ProtoContract]
    public enum SnapshotCompleteSubBook
    {
        [ProtoEnum(Name = "Normal", Value = 0)]
        Normal = 0,
        [ProtoEnum(Name = "Regular", Value = 1)]
        Regular = 1,
        [ProtoEnum(Name = "OffBook", Value = 2)]
        OffBook = 2,
        [ProtoEnum(Name = "BulletinBoard", Value = 0x20)]
        BulletinBoard = 0x20,
        [ProtoEnum(Name = "NegotiatedTrades", Value = 0x40)]
        NegotiatedTrades = 0x40
    }

    [ProtoContract]
    public enum SymbolDirectoryFlags
    {
        [ProtoEnum(Name = "NormalOrderBook", Value = 0)]
        NormalOrderBook = 0,
        [ProtoEnum(Name = "InverseOrderBook", Value = 1)]
        InverseOrderBook = 1
    }

    [ProtoContract]
    public enum SymbolDirectorySubBook
    {
        [ProtoEnum(Name = "None", Value = 0)]
        None = 0,
        [ProtoEnum(Name = "Regular", Value = 1)]
        Regular = 1,
        [ProtoEnum(Name = "OffBook", Value = 2)]
        OffBook = 2,
        [ProtoEnum(Name = "BulletinBoard", Value = 0x20)]
        BulletinBoard = 0x20,
        [ProtoEnum(Name = "NegotiatedTrades", Value = 0x40)]
        NegotiatedTrades = 0x40
    }

    [ProtoContract]
    public enum TopOfBookFlags
    {
        [ProtoEnum(Name = "Normal", Value = 0)]
        Normal = 0,
        [ProtoEnum(Name = "Regular", Value = 1)]
        Regular = 1,
        [ProtoEnum(Name = "BulletinBoard", Value = 0x20)]
        BulletinBoard = 0x20
    }
}
