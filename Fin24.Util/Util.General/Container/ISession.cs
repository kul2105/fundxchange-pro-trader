using System;

namespace Fin24.Util.General.Container
{
   public delegate void SessionDestroyingHandler(object sender, SessionDestroyingEventArgs evtArgs);

   public class SessionDestroyingEventArgs : EventArgs
   {
      public ISession Session { get; set; }
      public SessionDestroyingEventArgs()
      {

      }
   }

   public interface ISession : IDisposable 
   {
      event SessionDestroyingHandler SessionDestroying;
      bool Contains(string key);
      object this[string key] { get; set; }
   }
}