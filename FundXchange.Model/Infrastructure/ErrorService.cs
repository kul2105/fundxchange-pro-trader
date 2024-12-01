using System;
using System.Diagnostics;

namespace FundXchange.Model.Infrastructure
{
    public interface IErrorService
    {
        void LogError(string message, Exception ex);
    }

    public class ErrorService : IErrorService
    {
        public void LogError(string message, Exception ex)
        {
            Console.WriteLine(String.Format("Exception ({0}): {1}", message, ex.ToString()));
            Debug.WriteLine(String.Format("Exception ({0}): {1}", message, ex.ToString()));
        }
    }
}
