using System;
using FundXchange.Infrastructure;

namespace FundXchange.Model.Exceptions
{
    public class SymbolNotValidException : DomainException
    {
        public SymbolNotValidException(string symbol, string exchange)
            : base("Symbol (" + symbol + ") is not valid for Exchange (" + exchange + ")")
        {
        }
    }
}
