using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeScriptLib;
using FundXchange.Domain.Delegates;
using FundXchange.Domain.Enumerations;

namespace FundXchange.Domain.ValueObjects
{
    public class AlertScript
    {        
        public event AlertTriggeredDelegate OnAlertTriggered;

        public AlertScript(AlertScriptTypes type, string script)
            : this(type, script, "NA") { }

        public AlertScript(AlertScriptTypes type, string script, string symbol)
        {
            _type = type;
            _symbol = symbol;
            _script = script;
            _scriptObject = new Alert()
            {
                Symbol = symbol,
                AlertName = string.Format("{0}_{1}", symbol, Guid.NewGuid()),
                AlertScript = script,
                License = "XRT93NQR79ABTW788XR48"
            };
            _scriptObject.Alert += new _IAlertEvents_AlertEventHandler(_scriptObject_Alert);
        }

        private readonly Alert _scriptObject;
        private object m_lock = new object();
        private readonly AlertScriptTypes _type;
        private readonly string _script, _symbol;
        
        public int RecordCount
        {
            get { return _scriptObject.RecordCount; }
        }

        public string ScriptHelp
        {
            get { return _scriptObject.ScriptHelp; }
        }

        public void Append(Candlestick candlestick)
        {
            try
            {
                lock (m_lock)
                {
                    double julianDate = _scriptObject.ToJulianDate(candlestick.TimeOfClose.Year,
                                                                  candlestick.TimeOfClose.Month,
                                                                  candlestick.TimeOfClose.Day,
                                                                  candlestick.TimeOfClose.Hour,
                                                                  candlestick.TimeOfClose.Minute,
                                                                  candlestick.TimeOfClose.Second,
                                                                  candlestick.TimeOfClose.Millisecond);
                    _scriptObject.AppendRecord(julianDate, candlestick.Open, candlestick.High, candlestick.Low, candlestick.Close, (int)candlestick.Volume);
                }
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ClearRecords()
        {
            _scriptObject.ClearRecords();
        }

        public void Update(Candlestick candlestick)
        {
            try
            {
                lock (m_lock)
                {
                    double julianDate = _scriptObject.ToJulianDate(candlestick.TimeOfClose.Year,
                                                                  candlestick.TimeOfClose.Month,
                                                                  candlestick.TimeOfClose.Day,
                                                                  candlestick.TimeOfClose.Hour,
                                                                  candlestick.TimeOfClose.Minute,
                                                                  candlestick.TimeOfClose.Second,
                                                                  candlestick.TimeOfClose.Millisecond);
                    _scriptObject.EditRecord(julianDate, candlestick.Open, candlestick.High, candlestick.Low, candlestick.Close, (int)candlestick.Volume);
                }
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void _scriptObject_Alert(string symbol, string alertName)
        {
            RaiseAlertTriggeredEvent();
        }

        private void RaiseAlertTriggeredEvent()
        {
            if (null != OnAlertTriggered)
                OnAlertTriggered(_type);
        }
    }
}
