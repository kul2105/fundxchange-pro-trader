using System;
using FundXchange.Brokerage.Model.Gateways;
using FundXchange.Brokerage.Model.Enumerations;

namespace FundXchange.Brokerage.Model.Factories
{
    public class BrokerageGatewayFactory
    {
        public static IBrokerageGateway CreateBrokerageGateway(BrokerageTypes brokerageType)
        {
            switch (brokerageType)
            {
                case BrokerageTypes.Sanlam_iTrade:
                    return new Sanlam_iTrade_Brokerage_Gateway();
                default:
                    return new Sanlam_iTrade_Brokerage_Gateway();
            }
        }
    }
}
