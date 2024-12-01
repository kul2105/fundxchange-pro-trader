using System;
using fin24.Patterns.InfrastructurePatterns.Logging;
using System.Diagnostics;

namespace FundXchange.DataProvider
{
    public class Logger : ILog
    {
        #region ILog Members

        public bool LogError(string message, Exception ex)
        {
            Debug.WriteLine("Error: " + ex.ToString());
            return true;
        }

        public bool LogInfo(string message, LogType logType, LogPriority priority)
        {
            Debug.WriteLine("Error: " + message);
            return true;
        }

        #endregion
    }
}
