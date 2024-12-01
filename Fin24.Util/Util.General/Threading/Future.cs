using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Fin24.Util.General.Threading
{
   public class Future<T>
   {
      public delegate void ResultAvailableHandler(object sender, ResultAvailableEventArgs evtArgs);

      public class ResultAvailableEventArgs : EventArgs
      {
         public ResultAvailableEventArgs()
         {

         }
      }

      public event ResultAvailableHandler ResultAvailable;

      private readonly EventWaitHandle _lock;
      private bool _resultAvailable;
      private T _result;
      private readonly object _syncLock = new object();

      //---------------------------------------------------------------------------------*---------/
      public Future()
      {
         _lock = new EventWaitHandle(false, EventResetMode.ManualReset);
      }

      //---------------------------------------------------------------------------------*---------/
      public Future(T result)
      {
         lock (_syncLock)
         {
            Result = result;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public T Result
      {
         get
         {
            lock (_syncLock)
            {
               return _result;
            }
         }

         set
         {
            lock (_syncLock)
            {
               _result = value;
               _resultAvailable = true;
               _lock.Set();
               NotifyResultAvailable();
            }
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void Set ()
      {
         _lock.Set();
      }

      //---------------------------------------------------------------------------------*---------/
      public void Reset ()
      {
         _lock.Reset();
      }

      //---------------------------------------------------------------------------------*---------/
      public bool IsResultAvailable
      {
         get
         {
            lock (_syncLock)
            {
               return _resultAvailable;
            }
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void WaitOne()
      {
         _lock.WaitOne();
      }

      //---------------------------------------------------------------------------------*---------/
      public bool WaitOne(int timeout)
      {
         return _lock.WaitOne(timeout);
      }

      //---------------------------------------------------------------------------------*---------/
      private void NotifyResultAvailable()
      {
         if (ResultAvailable != null)
         {
            foreach (Delegate evtListener in ResultAvailable.GetInvocationList())
            {
               try
               {
                  evtListener.DynamicInvoke(this, new ResultAvailableEventArgs());
               }
               catch (Exception)
               {

               }
            }
         }
      }

   }
}