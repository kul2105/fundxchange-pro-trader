using System;
using System.Collections.Generic;
using FundXchange.Brokerage.Model.Enumerations;
using FundXchange.Brokerage.Model.ValueObjects;

namespace FundXchange.Brokerage.Model.Entities
{
    public class BrokerageAccount : EntityBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public IList<TradingAccount> TradingAccounts { get; set; }
        public BrokerageTypes BrokerageType { get; set; }
        public Portfolio Portfolio { get; set; }
    }
}
