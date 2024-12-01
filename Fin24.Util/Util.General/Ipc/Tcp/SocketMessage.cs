using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fin24.Util.General.Ipc;
using Fin24.Util.General.Ipc.Tcp;

namespace Fin24.Util.Ipc.Tcp
{
   //TODO Delete this class
   public class SocketMessage
   {
      public TcpChannel Channel { get; set; }
      public byte[] SocketData { get; set; }
      public ChannelActionEnum ChannelAction { get; set; }

      //---------------------------------------------------------------------------------*---------/
      public SocketMessage(ChannelActionEnum channelAction, TcpChannel channel)
         : this(channelAction, channel, null)
      {
      }

      //---------------------------------------------------------------------------------*---------/
      public SocketMessage(ChannelActionEnum channelAction, TcpChannel channel, byte[] data)
      {
         Channel = channel;
         SocketData = data;
         ChannelAction = channelAction;
      }
   }
}