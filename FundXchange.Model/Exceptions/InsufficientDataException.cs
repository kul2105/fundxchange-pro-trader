using System;

namespace FundXchange.Model.Exceptions
{
    public class InsufficientDataException : ApplicationException
    {
        public InsufficientDataException(string symbol, string exchange):
            base(string.Format("Insufficient Data found for Symbol({0}) on Exchange({1})", symbol, exchange))
        {
            
        }
    }
}
