using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FundXchange.Morningstar.Enumerations
{
    public enum MessageTypes : int
    {
        TF_MSG_TYPE_UNDEF = 0,      // undefined
        TF_MSG_TYPE_TRADE = 1,      // trades
        TF_MSG_TYPE_QUOTE = 2,      // bid/ask
        TF_MSG_TYPE_RECAP = 3,      // refresh
        TF_MSG_TYPE_ADMIN = 4,      // administration (in text format)
        TF_MSG_TYPE_CONTROL = 5,    // control messages (reserved)
        TF_MSG_TYPE_STATIC = 6,     // fundamental data
        TF_MSG_TYPE_DYNAMIC = 7,    // dynamic size messages
        TF_MSG_TYPE_OTHERS = 8,     // other messages
        TF_MSG_TYPE_CLOSE = 9,      // closing message
        TF_MSG_TYPE_NEWS = 10,      // news message
        TF_MSG_TYPE_CHART = 13		// chart update (msg 11 and 12 are used internal)
    }
}
