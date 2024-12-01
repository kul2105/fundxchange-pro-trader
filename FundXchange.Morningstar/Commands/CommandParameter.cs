using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FundXchange.Morningstar.Commands
{
    public class CommandParameter
    {
        public CommandParameter(string parameterName, string parameterValue)
            : this(parameterName, parameterValue, false) { }

        public CommandParameter(string parameterName, string parameterValue, bool requiresQuotes)
        {
            if(requiresQuotes)
                _parameter = new KeyValuePair<string, string>(parameterName, string.Format("\"{0}\"", parameterValue));
            else
                _parameter = new KeyValuePair<string,string>(parameterName, parameterValue);
        }

        public CommandParameter(string parameterName, string[] values)
        {
            _parameter = new KeyValuePair<string, string>(parameterName, string.Join(",", values));
        }

        private readonly KeyValuePair<string, string> _parameter;

        public static implicit operator string(CommandParameter commandParameter)
        {
            return commandParameter.ToString();
        }

        public override string ToString()
        {
            return string.Format("/{0}={1}", _parameter.Key, _parameter.Value);
        }
    }
}
