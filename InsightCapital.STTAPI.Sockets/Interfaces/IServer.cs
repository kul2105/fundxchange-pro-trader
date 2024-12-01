using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace InsightCapital.STTAPI.Sockets.Interfaces
{
    public interface IServer 
    {
       bool StartServer(int ServerPort);

       bool StopServer();
    }
}
