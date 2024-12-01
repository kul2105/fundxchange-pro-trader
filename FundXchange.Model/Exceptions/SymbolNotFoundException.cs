using System;
using FundXchange.Infrastructure;

namespace FundXchange.Model.Exceptions
{
    public class SymbolNotFoundException : DomainException
    {
        public SymbolNotFoundException(string symbol, string exchange)
            : base("Symbol (" + symbol + ") could not be found for Exchange (" + exchange + ")")
        {
        }
    }
}
