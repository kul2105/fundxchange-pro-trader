using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Fin24.Util.General.Container
{
   [Serializable]
   public class DefaultSession : ISession
   {
      public event SessionDestroyingHandler SessionDestroying;

      private readonly IDictionary<string, object> _sessionData= new Dictionary<string, object>();

      //---------------------------------------------------------------------------------*---------/
      public bool Contains (string key)
      {
         return _sessionData.ContainsKey(key);
      }

      //---------------------------------------------------------------------------------*---------/
      public object this[string key]
      {
         get
         {
            object val;
            _sessionData.TryGetValue(key, out val);

            return val;
         }

         set
         {
            _sessionData[key] = value;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void Remove (string key)
      {
         if (_sessionData.ContainsKey( key))
         {
            _sessionData.Remove(key);
         }
      }

      //---------------------------------------------------------------------------------*---------/
      ~DefaultSession ()
      {
         Dispose( false);
      }

      //---------------------------------------------------------------------------------*---------/
      public void Dispose ()
      {
         Dispose( true);
      }

      //---------------------------------------------------------------------------------*---------/
      public void Dispose (bool disposeManaged)
      {
         NotifySessionDestroing();
         GC.SuppressFinalize( this);   
      }

      //---------------------------------------------------------------------------------*---------/
      private void NotifySessionDestroing()
      {
         if (SessionDestroying != null)
         {
            foreach (Delegate evtListener in SessionDestroying.GetInvocationList())
            {
               try
               {
                  evtListener.DynamicInvoke(this, new SessionDestroyingEventArgs{Session=this});
               }
               catch (Exception)
               {

               }
            }
         }
      }
   }
}