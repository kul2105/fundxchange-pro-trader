using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FundXchange.Morningstar.Commands
{
    public static class CommandSwitches
    {
        public static CommandSwitch ActivateRealtimeUpdates = new CommandSwitch("/ActReal");
        public static CommandSwitch DeActivateRealtimeUpdates = new CommandSwitch("/DeActReal");
        public static CommandSwitch RequestRecap = new CommandSwitch("/ReqReal");
        public static CommandSwitch RequestAllFields = new CommandSwitch("/All");
    }
}
