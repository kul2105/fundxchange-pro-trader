using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;

namespace Fin24.Util.General.Misc
{
   public class SimpleMultiTimer
   {
      private const int SLEEP_BETWEEN_SNAPSHOTS = 1000*5;
      private readonly IDictionary<Type, StopwatchInfo> _stopwatchList = new Dictionary<Type, StopwatchInfo>();
      private readonly object _syncLock = new object();
      private static readonly SimpleMultiTimer s_instance= new SimpleMultiTimer();
      private static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
      private Thread _snapshotThread;

      //---------------------------------------------------------------------------------*---------/
      private SimpleMultiTimer ()
      {}

      //---------------------------------------------------------------------------------*---------/
      public static SimpleMultiTimer Instance
      {
         get
         {
            return s_instance;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void Start (Type type)
      {
         lock (_syncLock)
         {
            Start();

            StopwatchInfo stopwatchInfo;
            if (!_stopwatchList.TryGetValue( type, out stopwatchInfo))
            {
               stopwatchInfo = new StopwatchInfo{Stopwatch= new Stopwatch()};
               _stopwatchList.Add( type, stopwatchInfo);
            }

            stopwatchInfo.Stopwatch.Start();
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void Stop (Type type)
      {
         lock (_syncLock)
         {
            StopwatchInfo stopwatchInfo;
            if (!_stopwatchList.TryGetValue(type, out stopwatchInfo))
            {
               return;
            }

            stopwatchInfo.Stopwatch.Stop();
            stopwatchInfo.Count++;

         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void Start ()
      {
         if (!LOG.IsDebugEnabled)
         {
            return;
         }

         if (_snapshotThread != null)
         {
            return;
         }

         lock (_syncLock)
         {
            if (_snapshotThread != null)
            {
               return;
            }

            _snapshotThread= new Thread( PrintSnapshot);
            _snapshotThread.Name = "Timer Snapshot thread";
            _snapshotThread.IsBackground = true;
            _snapshotThread.Start();
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void PrintSnapshot ()
      {
         while (true)
         {
            Thread.Sleep(SLEEP_BETWEEN_SNAPSHOTS);
            lock (_syncLock)
            {
               foreach (Type currType in _stopwatchList.Keys)
               {
                  StopwatchInfo stopwatchInfo = _stopwatchList[currType];
                  LOG.DebugFormat("{0} called {1} time averiging {2} milliseconds", currType, stopwatchInfo.Count, stopwatchInfo.Stopwatch.ElapsedMilliseconds / stopwatchInfo.Count);
               }
            }
         }
      }

      //---------------------------------------------------------------------------------*---------/
      //---------------------------------------------------------------------------------*---------/
      private class StopwatchInfo
      {
         public int Count { get; set; }
         public Stopwatch Stopwatch { get; set; }
      }
   }
}
