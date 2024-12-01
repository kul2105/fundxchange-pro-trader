using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fin24.Util.General.Container;

namespace Fin24.Util.General.Ipc
{
   public delegate void ChannelActionHandler(object sender, ChannelActionEventArgs evtArgs);

   public class ChannelActionEventArgs : EventArgs
   {
      public IChannel Channel { get; private set; }
      public ChannelActionEnum Action { get; set; }
      public byte[] ChannelData { get; set; }

      //---------------------------------------------------------------------------------*---------/
      public ChannelActionEventArgs(IChannel channel, ChannelActionEnum action) : this(channel, action, null)
      {
      }

      //---------------------------------------------------------------------------------*---------/
      public ChannelActionEventArgs(IChannel channel, ChannelActionEnum action, byte []channelData)
      {
         Channel = channel;
         Action = action;
         ChannelData= channelData;
      }

   }

   public enum ChannelActionEnum
   {
      Connected,
      Receive,
      Send,
      Closed,
      Created
   } ;

   public interface IChannel
   {
      event ChannelActionHandler ChannelConnected;
      event ChannelActionHandler ChannelDisconnected;
      event ChannelActionHandler ChannelDataReceived;
      event ChannelActionHandler ChannelDataSent;

      void Send(byte[] data);
      void Close();
      string Uri { get; set; }
      ISession Session { get; set; }
      bool Connected { get; }
   }
}