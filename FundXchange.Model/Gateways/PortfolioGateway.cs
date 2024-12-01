using System;
using FundXchange.Model.Infrastructure;
using FundXchange.Model.PortfolioService;

namespace FundXchange.Model.Gateways
{
    public class PortfolioGateway
    {
        private ErrorService _ErrorService;

        public PortfolioGateway(ErrorService errorService)
        {
            _ErrorService = errorService;
        }

        public bool SaveDataPortfolio(int userId, string portfolioName)
        {
            try
            {
                using (PortfolioServiceClient proxy = new PortfolioServiceClient())
                {   
                    GeneralResult result = proxy.AddDataPortfolio(userId, portfolioName);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return false;
            }
        }

        public bool DeleteDataPortfolio(int userId, string portfolioName)
        {
            try
            {
                using (PortfolioServiceClient proxy = new PortfolioServiceClient())
                {
                    GeneralResult result = proxy.RemoveDataPortfolio(userId, portfolioName);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return false;
            }
        }

        public bool AddDataPortfolioInstrument(int userId, string portfolioName, string exchange, string symbol)
        {
            try
            {
                using (PortfolioServiceClient proxy = new PortfolioServiceClient())
                {
                    GeneralResult result = proxy.AddDataPortfolioInstrument(userId, portfolioName, exchange, symbol);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return false;
            }
        }

        public bool RemoveDataPortfolioInstrument(int userId, string portfolioName, string exchange, string symbol)
        {

            return true;
            //Commented By John
            //try
            //{
            //    using (PortfolioServiceClient proxy = new PortfolioServiceClient())
            //    {
            //        GeneralResult result = proxy.RemoveDataPortfolioInstrument(userId, portfolioName, exchange, symbol);
            //        return result.Object;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _ErrorService.LogError(ex.Message, ex);
            //    return false;
            //}
        }
    }
}
