using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace FundXchange.Infrastructure
{
    public static class ApplicationConfiguration
    {
        public static string GetApplicationSetting(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (ConfigurationException)
            {
                return string.Empty;
            }
        }
    }
}
