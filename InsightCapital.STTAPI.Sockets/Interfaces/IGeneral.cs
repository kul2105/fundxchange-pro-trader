using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsightCapital.STTAPI.Sockets.Interfaces
{
    public interface iGenParam4clsClient
    {
        int HeartBeatTolerance { get; set; }
        int HeartBeatInterval { get; set; }
        int ReConnectionInterval { get; set; }
    }
}
