using System;
using FundXchange.Brokerage.Model.ValueObjects;
using System.Collections.Generic;

namespace FundXchange.Brokerage.Model.Entities
{
    public class TradingAccount : EntityBase
    {
        public BrokerageAccount BrokerageAccount { get; set; }
        public string AccountNumber { get; set; }

        private IList<Transaction> _Transactions;
        public IEnumerable<Transaction> Transactions
        {
            get
            {
                foreach (Transaction transaction in _Transactions)
                {
                    yield return transaction;
                }
            }
        }

        public void AddTransaction(Transaction transaction)
        {
            _Transactions.Add(transaction);
        }
    }
}
