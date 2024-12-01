using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fin24.Util.General.Ipc.Udp
{
   public class MulticastChannelConfig
   {
      [Flags]
      public enum ChannelType
      {
         Send= 1,
         Receive= 2
      }

      private int _multicastPort;
      private int? _localPort;

      public string MultiCastGroup { get; set; }
      public string LocalInterfaceAddress { get; set; }
      public ChannelType Usage{ get; set; }

      //---------------------------------------------------------------------------------*---------/
      public int MulticastPort
      {
         get
         {
            return _multicastPort;
         }
         set
         {
            _multicastPort = value;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public int LocalPort
      {
         get
         {
            if (_localPort == null)
            {
               return _multicastPort;
            }

            return (int)_localPort;
         }
         set
         {
            _localPort = value;
         }
      }
   }
}
