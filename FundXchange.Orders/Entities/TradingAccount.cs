using System;
using System.Runtime.Serialization;
using FundXchange.Orders.Enumerations;

namespace FundXchange.Orders.Entities
{
    [DataContract]
    public class TradingAccount
    {
        [DataMember]
        public Brokerages Brokerage { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        
        public TradingAccount()
        {

        }

        public TradingAccount(string username, string password, Brokerages brokerage, string accountNumber, string accountName)
        {
            Brokerage = brokerage;
            Username = username;
            Password = password;
            AccountNumber = accountNumber;
            AccountName = accountName;
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(AccountName))
                return AccountName;
            return AccountNumber;
        }
    }
}
