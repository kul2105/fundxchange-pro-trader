using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Fin24.Util.General.Management
{
   public class PerformanceCounterHelper
   {
      #region Private Members

      private readonly CounterCreationDataCollection _counterCollection;
      private PerformanceCounter _operationsExecuted;
      private PerformanceCounter _operationsPerSecond;
      private PerformanceCounter _averageTimeToExecute;
      private PerformanceCounter _averageTimeToExecutePerBase;
      private readonly string _performanceCounterCategoryName;
      private PerformanceCounterHelper _performanceCounterInstance;

      #endregion

      #region Constructors

      protected PerformanceCounterHelper()
      {
         _counterCollection = new CounterCreationDataCollection();

         AssemblyName executingAssemblyName = Assembly.GetEntryAssembly().GetName();

         string appName = executingAssemblyName.Name;
         string appVersion = executingAssemblyName.Version.ToString();
         _performanceCounterCategoryName = string.Format("{0} v {1}", appName, appVersion);

         if (!PerformanceCounterCategory.Exists(_performanceCounterCategoryName))
         {
            DeletePreviousPerformanceCounters(appName);

            CreateCounterData("operations executed", "Total number of operations executed", PerformanceCounterType.NumberOfItems32);
            CreateCounterData("operations per second", "Total number of operations per second", PerformanceCounterType.RateOfCountsPerSecond32);
            CreateCounterData("average time to execute", "Average time taken to execute", PerformanceCounterType.AverageTimer32);
            CreateCounterData("average time to execute per base", "Average duration per operation execution base", PerformanceCounterType.AverageBase);

            PerformanceCounterCategory.Create(_performanceCounterCategoryName, "Fin24.Utils Performance Management",
                                              PerformanceCounterCategoryType.SingleInstance, _counterCollection);
         }

         _operationsExecuted = new PerformanceCounter(_performanceCounterCategoryName, "operations executed", "", false);
         _operationsPerSecond = new PerformanceCounter(_performanceCounterCategoryName, "operations per second", "", false);
         _averageTimeToExecute = new PerformanceCounter(_performanceCounterCategoryName, "average time to execute", "", false);
         _averageTimeToExecutePerBase = new PerformanceCounter(_performanceCounterCategoryName, "average time to execute per base", "", false);
      }

      #endregion

      #region Private Methods

      private void CreateCounterData(string counterName, string counterHelp, PerformanceCounterType counterType)
      {
         CounterCreationData totalOperationsExecuted = new CounterCreationData(counterName, counterHelp, counterType);
         _counterCollection.Add(totalOperationsExecuted);
      }

      private static void DeletePreviousPerformanceCounters(string appName)
      {
         List<PerformanceCounterCategory> performanceCounterCategories = new List<PerformanceCounterCategory>(PerformanceCounterCategory.GetCategories());

         foreach (var cateogry in performanceCounterCategories.FindAll(pc => pc.CategoryName.Contains(appName)))
         {
            PerformanceCounterCategory.Delete(cateogry.CategoryName);
         }
      }

      #endregion

      #region Public Methods

      public PerformanceCounterHelper GetPerformanceCounterManager()
      {
         if(_performanceCounterInstance == null)
         {
            _performanceCounterInstance = new PerformanceCounterHelper();
         }

         return _performanceCounterInstance;
      }

      public void Increment(long value)
      {
         _operationsExecuted.Increment();
         _operationsPerSecond.Increment();
         _averageTimeToExecute.IncrementBy(value);
         _averageTimeToExecutePerBase.Increment();
      }

      public void DecrementCounters()
      {
         _operationsExecuted.Decrement();
         _operationsPerSecond.Decrement();
         _averageTimeToExecute.Decrement();
         _averageTimeToExecutePerBase.Decrement();
      }

      public void ClearCounters()
      {
         _operationsExecuted.RawValue = 0;
         _operationsPerSecond.RawValue = 0;
         _averageTimeToExecute.RawValue = 0;
         _averageTimeToExecutePerBase.RawValue = 0;
      }

      [DllImport("Kernel32.dll")]
      public static extern void QueryPerformanceCounter(ref long ticks);

      #endregion
   }
}