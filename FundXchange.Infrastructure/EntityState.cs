using System;

namespace FundXchange.Infrastructure
{
    public enum EntityState
    {
        Unchanged = 0,
        New,
        Deleted,
        Dirty
    }
}
