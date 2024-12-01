using System;
using FundXchange.Orders.Enumerations;

namespace FundXchange.Orders.Entities
{
    public class BrokerageCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Brokerages Brokerage { get; set; }

        public BrokerageCredentials(string username, string password, Brokerages brokerage)
        {
            Username = username;
            Password = password;
            Brokerage = brokerage;
        }
    }
}
