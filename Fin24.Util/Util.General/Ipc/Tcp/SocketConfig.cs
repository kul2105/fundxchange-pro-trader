using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fin24.Util.General.Ipc.Tcp
{
   public class SocketConfig
   {
      private int _socketBufferSize = 4 * 1024;

      //---------------------------------------------------------------------------------*---------/
      public SocketConfig(int? socketBufferSize)
      {
         if (socketBufferSize != null)
         {
            _socketBufferSize = socketBufferSize.Value;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public SocketConfig()
         : this(null)
      {

      }

      //---------------------------------------------------------------------------------*---------/
      public int SocketBufferSize
      {
         get
         {
            return _socketBufferSize;
         }

         set
         {
            _socketBufferSize = value;
         }
      }
   }
}