using System;

namespace FundXchange.Brokerage.Model.Enumerations
{
    public enum OrderStatusTypes
    {
        Open,
        Cancelled,
        Matched,
        Partially_Filled,
        Awaiting_Authorization,
        Pending_Cancel
    }
}
