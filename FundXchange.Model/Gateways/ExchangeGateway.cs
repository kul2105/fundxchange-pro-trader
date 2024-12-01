using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FundXchange.Model.Infrastructure;
using FundXchange.Model.ExchangeService;

namespace FundXchange.Model.Gateways
{
    public class ExchangeGateway
    {
        private ErrorService _ErrorService;

        public ExchangeGateway(ErrorService errorService)
        {
            _ErrorService = errorService;
        }

        public ExchangeDTO GetExchangeWithSymbols(string exchangeName)
        {
            try
            {
                using (ExchangeServiceClient proxy = new ExchangeServiceClient())
                {
                    var result = proxy.GetExchange(exchangeName);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return null;
            }
        }

        public int GetLatestExchangeSymbolListVersion(string exchangeName)
        {
            try
            {
                using (ExchangeServiceClient proxy = new ExchangeServiceClient())
                {
                    ActionResultOfint result = proxy.GetLatestExchangeSymbolListVersion(exchangeName);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return 0;
            }
        }
    }
}
