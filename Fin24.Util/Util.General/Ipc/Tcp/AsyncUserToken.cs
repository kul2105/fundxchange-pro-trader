using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Fin24.Util.General.Threading;

namespace Fin24.Util.General.Ipc.Tcp
{
   public class AsyncUserToken
   {
      public Future<SocketError> SyncLock { get; set; }
      private bool _closed;
      private readonly object _syncLock = new object();

      public bool Closed
      {
         get
         {
            lock (_syncLock)
            {
               return _closed;
            }
         }

         set
         {
            lock (_syncLock)
            {
               _closed = value;
            }
         }
      }
   }
}