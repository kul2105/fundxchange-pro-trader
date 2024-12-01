using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fin24.Util.General.Container;

namespace Fin24.Util.General.Ipc
{
   public abstract class AbstractChannelHandler
   {
      private IChannel _channel;

      //---------------------------------------------------------------------------------*---------/
      public IChannel Channel
      {
         protected get
         {
            return _channel;
         }

         set
         {
            if (_channel == value)
            {
               return;
            }

            _channel = value;
            AttachEventHandlers( _channel);
         }
      }

      //---------------------------------------------------------------------------------*---------/
      protected abstract void OnChannelConnected();

      //---------------------------------------------------------------------------------*---------/
      protected abstract void OnChannelDisconnected();

      //---------------------------------------------------------------------------------*---------/
      protected abstract void OnChannelDataSent(byte[] bytes);

      //---------------------------------------------------------------------------------*---------/
      protected abstract void OnChannelDataReceived(byte[] bytes);

      //---------------------------------------------------------------------------------*---------/
      private void ChannelDisconnectHandler(object sender, ChannelActionEventArgs evtArgs)
      {
         DetachEventHandlers(evtArgs.Channel);
         OnChannelDisconnected();
      }

      //---------------------------------------------------------------------------------*---------/
      private void ChannelDataSentHandler(object sender, ChannelActionEventArgs evtArgs)
      {
         OnChannelDataSent(evtArgs.ChannelData);
      }

      //---------------------------------------------------------------------------------*---------/
      private void ChannelDataReceivedHandler(object sender, ChannelActionEventArgs evtArgs)
      {
         OnChannelDataReceived(evtArgs.ChannelData);
      }

      //---------------------------------------------------------------------------------*---------/
      private void ChannelConnectedHandler(object sender, ChannelActionEventArgs evtArgs)
      {
         OnChannelConnected();
      }

      //---------------------------------------------------------------------------------*---------/
      private void AttachEventHandlers(IChannel channel)
      {
         channel.ChannelConnected += new ChannelActionHandler(ChannelConnectedHandler);
         channel.ChannelDataReceived += new ChannelActionHandler(ChannelDataReceivedHandler);
         channel.ChannelDataSent += new ChannelActionHandler(ChannelDataSentHandler);
         channel.ChannelDisconnected += new ChannelActionHandler(ChannelDisconnectHandler);
      }

      //---------------------------------------------------------------------------------*---------/
      private void DetachEventHandlers(IChannel channel)
      {
         channel.ChannelConnected -= new ChannelActionHandler(ChannelConnectedHandler);
         channel.ChannelDataReceived -= new ChannelActionHandler(ChannelDataReceivedHandler);
         channel.ChannelDataSent -= new ChannelActionHandler(ChannelDataSentHandler);
         channel.ChannelDisconnected -= new ChannelActionHandler(ChannelDisconnectHandler);
      }

   }
}
