using System;
using FundXchange.Model.AlertService;
using FundXchange.Model.Infrastructure;

namespace FundXchange.Model.Gateways
{
    public class AlertServiceGateway
    {
        private ErrorService _ErrorService;

        public AlertServiceGateway(ErrorService errorService)
        {
            _ErrorService = errorService;
        }

        public bool SaveAlertScript(int userId, AlertScriptDTO alertScriptDTO)
        {
            try
            {
                using (AlertService.AlertServiceClient proxy = new AlertServiceClient())
                {   
                    GeneralResult result = proxy.AddAlertScript(userId, alertScriptDTO);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return false;
            }
        }

        public bool RemoveAlertScript(int userId, string alertName)
        {
            try
            {
                using (AlertService.AlertServiceClient proxy = new AlertServiceClient())
                {
                    GeneralResult result = proxy.RemoveAlertScript(userId, alertName);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return false;
            }
        }

        public bool RemoveAlertScript(int userId, Guid alertId)
        {
            try
            {
                using (AlertService.AlertServiceClient proxy = new AlertServiceClient())
                {
                    GeneralResult result = proxy.RemoveAlertScriptById(userId, alertId);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return false;
            }
        }

        public bool SaveScanner(int userId, ScannerDTO scannerDTO)
        {
            try
            {
                return true;// For BarChart
                using (AlertService.AlertServiceClient proxy = new AlertServiceClient())
                {
                    GeneralResult result = proxy.SaveScanner(userId, scannerDTO);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return false;
            }
        }

        public bool RemoveScanner(int userId, string scannerName)
        {
            try
            {
                return true;// FOR BARCHART
                using (AlertService.AlertServiceClient proxy = new AlertServiceClient())
                {
                    GeneralResult result = proxy.RemoveScanner(userId, scannerName);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return false;
            }
        }

        public bool AddAlert(int userId, AlertDTO alertDTO)
        {
            try
            {
                return true;// For Barchart

                using (AlertService.AlertServiceClient proxy = new AlertServiceClient())
                {
                    GeneralResult result = proxy.AddAlert(userId, alertDTO);
                    return result.Object;
                }
            }
            catch (Exception ex)
            {
                _ErrorService.LogError(ex.Message, ex);
                return false;
            }
        }
    }
}
