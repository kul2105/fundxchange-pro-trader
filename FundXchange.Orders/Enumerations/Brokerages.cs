using System;
using System.Runtime.Serialization;

namespace FundXchange.Orders.Enumerations
{
    [DataContract]
    public enum Brokerages
    {
        [EnumMember]
        Sanlam_iTrade,
        [EnumMember]
        FundXchange,
        [EnumMember]
        FinSwitch,
        [EnumMember]
        NotSupported
    }
}
