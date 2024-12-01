using System;
using System.Net.Sockets;
using Fin24.Util.General.Threading;

namespace FundXchange.Data.McGregorBFA
{
    public class AsyncUserToken
    {
        public Future<SocketError> SyncLock { get; set; }
    }
}
