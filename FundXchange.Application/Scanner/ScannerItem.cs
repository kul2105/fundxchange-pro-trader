using System.Timers;
using TradeScriptLib;
using System;
using FundXchange.Model.ViewModels.Charts;
using System.IO;

namespace FundXchange.Application.Scanner
{
    public class ScannerItem : IEquatable<ScannerItem>,IDisposable
    {
        private const int TIMER_INTERVAL = 60000;

        public ScannerItem(string symbol, Alert alertObject, int scanInterval)
        {
            Symbol = symbol;
            AlertObject = alertObject;
            ScannerInterval = scanInterval;
            Timer = new Timer(TIMER_INTERVAL);
            Timer.Elapsed += Timer_Elapsed;

            _suspendedScript = AlertObject.AlertScript;
        }
        public ScannerItem(string symbol, Alert alertObject, int scanInterval,Periodicity periodicity,int interval)
        {
            ChartPeriodicity = periodicity;
            Interval = interval;
            Symbol = symbol;
            AlertObject = alertObject;
            ScannerInterval = scanInterval;
            Timer = new Timer(TIMER_INTERVAL);
            Timer.Elapsed += Timer_Elapsed;

            _suspendedScript = AlertObject.AlertScript;
        }
        public delegate void DataRequiredDelegate(ScannerItem scannerItem);
        public event DataRequiredDelegate OnDataRequired;

        private string _suspendedScript;

        public string Symbol { get; private set; }
        public Alert AlertObject { get; private set; }
        public int ScannerInterval { get; private set; }
        public Timer Timer { get; private set; }
        public ScannerState State { get; private set; }
        public bool IsLocked { get; private set; }
        public Periodicity ChartPeriodicity { get; private set;}
        public int Interval { get; private set; }
        
        public void Start()
        {
           
                State = ScannerState.Running;
                if (Timer == null)
                    return;
                if (!Timer.Enabled)
                    Timer.Enabled = true;
                Timer.Start();
                AlertObject.AlertScript = _suspendedScript;
            
        }

        public void Stop()
        {
            
            State = ScannerState.Paused;
            if (Timer == null)
                return;
            Timer.Stop();

            _suspendedScript = AlertObject.AlertScript;
            AlertObject.AlertScript = string.Empty;
             
        }

        public void Lock()
        {
            IsLocked = true;
        }

        public void Unlock()
        {
            IsLocked = false;
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if((e.SignalTime.Minute % (ScannerInterval / TIMER_INTERVAL)) == 0)
            {
                if (null != OnDataRequired)
                    OnDataRequired(this);
            }
            
        }

        #region Implementation of IEquatable<ScannerItem>

        public bool Equals(ScannerItem other)
        {
            return Symbol == other.Symbol;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Timer.Enabled = false;
            Timer.Close();
            Timer.Dispose();
            Timer = null;
            AlertObject.ClearRecords();
        }

        #endregion
    }
}
