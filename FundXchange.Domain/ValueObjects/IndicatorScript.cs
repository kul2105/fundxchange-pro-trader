using System;
using TradeScriptLib;

namespace FundXchange.Domain.ValueObjects
{
    public class IndicatorScript
    {
        public IndicatorScript(string script)
        {
            _script = script;
            _scriptObject = new ScriptOutput()
            {
                License = "XRT93NQR79ABTW788XR48",
            };
        }

        private readonly ScriptOutput _scriptObject;
        private object m_lock = new object();
        private readonly string _script;

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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public string GetResult()
        {
            if(!string.IsNullOrEmpty(_script))
            {
                try
                {
                    return _scriptObject.GetScriptOutput(_script);
                }
                catch (AccessViolationException) { }
            }
            return string.Empty;
        }
    }
}
