using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FundXchange.Morningstar.Commands
{
    public class CommandSwitch
    {
        public CommandSwitch(string outputString)
        {
            _outputString = outputString;
        }

        private readonly string _outputString;

        public override string ToString()
        {
            return _outputString;
        }

        public static implicit operator string(CommandSwitch parameter)
        {
            return parameter.ToString();
        }
    }
}
