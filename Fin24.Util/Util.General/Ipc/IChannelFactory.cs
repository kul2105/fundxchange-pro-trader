using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fin24.Util.General.Ipc
{
   public delegate void ChannelFactoryEventHandler(object sender, ChannelFactoryEventArgs evtArgs);

   public class ChannelFactoryEventArgs : EventArgs
   {
      public IChannel Channel { get; private set; }
      public ChannelFactoryEventArgs( IChannel newChannel)
      {
         Channel = newChannel;
      }
   }

   public interface IChannelFactory
   {
      event ChannelFactoryEventHandler ChannelCreated;
   }
}