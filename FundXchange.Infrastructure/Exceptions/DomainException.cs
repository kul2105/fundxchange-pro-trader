using System;

namespace FundXchange.Infrastructure
{
    public class DomainException : ApplicationException
    {
        public DomainException()
        {
        }

        public DomainException(string message)
            : base(message)
        {
        }
    }
}
