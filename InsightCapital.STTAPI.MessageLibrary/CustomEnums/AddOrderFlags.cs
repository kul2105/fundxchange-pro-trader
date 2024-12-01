using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace InsightCapital.STTAPI.MessageLibrary
{
    [ProtoContract]
    public enum AddOrderFlags
    {
        [ProtoEnum(Name = "Normal", Value = 0)]
        Normal = 0,
        [ProtoEnum(Name = "Regular", Value = 1)]
        Regular = 1,
        [ProtoEnum(Name = "MarketOrder", Value = 0x10)]
        MarketOrder = 0x10,
        [ProtoEnum(Name = "BulletinBoard", Value = 0x20)]
        BulletinBoard = 0x20
    }
}
