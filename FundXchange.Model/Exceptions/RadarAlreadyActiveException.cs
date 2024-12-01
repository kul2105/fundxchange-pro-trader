using System;

namespace FundXchange.Model.Exceptions
{
    public class RadarAlreadyActiveException : Exception
    {
        public RadarAlreadyActiveException(string symbol, string exchange)
            : base(string.Format("Radar for instrument {0} on the {1} exchange is already active.", symbol, exchange))
        {
        }
    }
}
