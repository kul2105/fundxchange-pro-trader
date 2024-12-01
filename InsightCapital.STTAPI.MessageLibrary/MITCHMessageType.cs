using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Mitch Message Types
    /// </summary>
    [ProtoContract]
    public enum MITCHMessageType
    {       
        [ProtoEnum(Name = "Time", Value = 0x54)]
        Time = 0x54,
        [ProtoEnum(Name = "SystemEvent", Value = 0x53)]
        SystemEvent = 0x53,
        [ProtoEnum(Name = "SymbolDirectory", Value = 0x52)]
        SymbolDirectory = 0x52,
        [ProtoEnum(Name = "SymbolStatus", Value = 0x48)]
        SymbolStatus = 0x48,       
        [ProtoEnum(Name = "TradeBreak", Value = 0x42)]
        TradeBreak = 0x42,      
        [ProtoEnum(Name = "AuctionInfo", Value = 0x49)]
        AuctionInfo = 0x49,
        [ProtoEnum(Name = "Statistics", Value = 0x77)]
        Statistics = 0x77,
        [ProtoEnum(Name = "ExtendedStatistics", Value = 0x80)]
        ExtendedStatistics = 0x80,
        [ProtoEnum(Name = "News", Value = 0x75)]
        News = 0x75,      
        [ProtoEnum(Name = "Logon", Value = 500)]
        Logon = 500,
        [ProtoEnum(Name = "FabricatedTrade", Value = 0x1f5)]
        FabricatedTrade = 0x1f5,
        [ProtoEnum(Name = "DepthUpdate", Value = 0x1f6)]
        DepthUpdate = 0x1f6,
        [ProtoEnum(Name = "HeartBeat", Value = 0x1f7)]
        HeartBeat = 0x1f7
    }
}
