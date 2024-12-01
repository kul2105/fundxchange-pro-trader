using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FundXchange.Morningstar.Commands
{
    public class Command
    {
        public Command(InteractiveApiCommands command)
        {
            _command = command;
            _parameters = new List<CommandParameter>();
            _switches = new List<CommandSwitch>();
        }

        private readonly InteractiveApiCommands _command;
        private readonly List<CommandParameter> _parameters;
        private readonly List<CommandSwitch> _switches;

        public Command With(CommandParameter commandParameter)
        {
            AddParameter(commandParameter);
            return this;
        }

        public Command With(CommandSwitch commandSwitch)
        {
            AddSwitch(commandSwitch);
            return this;
        }

        private void AddParameter(CommandParameter commandParameter)
        {
            if (!_parameters.Contains(commandParameter))
            {
                _parameters.Add(commandParameter);
            }
        }

        private void AddSwitch(CommandSwitch commandSwitch)
        {
            if (!_switches.Contains(commandSwitch))
            {
                _switches.Add(commandSwitch);
            }
        }

        public override string ToString()
        {
            return BuildCommandString();
        }

        public static implicit operator string(Command command)
        {
            return command.ToString();
        }

        private string BuildCommandString()
        {
            var stringBuilder = new StringBuilder(string.Format("{0} ", _command));

            foreach (var parameter in _parameters)
            {
                stringBuilder.AppendFormat("{0} ", parameter);
            }
            foreach (var commandSwitch in _switches)
            {
                stringBuilder.AppendFormat("{0} ", commandSwitch);
            }
            return stringBuilder.ToString();
        }
    }
}
