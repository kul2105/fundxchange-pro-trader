using System;
using System.Collections.Generic;
using FundXchange.Domain.ValueObjects;
using FundXchange.Model.ViewModels.Indicators;
using Candlestick = FundXchange.Domain.ValueObjects.Candlestick;
using FundXchange.Domain.Enumerations;

namespace FundXchange.Model.ViewModels.RadarView
{    
    public class RadarItemIndicator : IRadarViewIndicator
    {
        public RadarItemIndicator(Indicator indicator)
        {
            CreateAlertObjects(indicator);
            AssociatedIndicator = indicator;
            IsActive = true;
        }

        private IndicatorScript _standardIndicator;
        private readonly IList<AlertScript> _alertObjects = new List<AlertScript>();
        private bool _isInitializing;

        public event RadarIndicatorChangedDelegate OnChanged;

        public Indicator AssociatedIndicator { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsOverBought { get; private set; }
        public bool IsOverSold { get; private set; }
        public AlertScriptTypes AlertTriggered { get; private set; }

        private double _value = 0;
        public double Value 
        {
            get { return _value; }
        }

        public void PrepareForRemoval()
        {
            UnBindAlertObjectEvents();

            _standardIndicator.ClearRecords();
            foreach (AlertScript alertObject in _alertObjects)
            {
                alertObject.ClearRecords();
            }
            _standardIndicator = null;
            _alertObjects.Clear();
        }

        public void InitializeWith(IEnumerable<Candlestick> candles)
        {
            _isInitializing = true;

            BindAlertObjectEvents();

            foreach (Candlestick candlestick in candles)
            {
                _standardIndicator.Append(candlestick);
                foreach (AlertScript alertObject in _alertObjects)
                {
                    alertObject.Append(candlestick);
                }
            }
            
            _isInitializing = false;

            GetLatestIndicatorValue();
        }

        public void AddCandle(Candlestick candlestick)
        {
            _standardIndicator.Append(candlestick);
            foreach (AlertScript alertObject in _alertObjects)
            {
                alertObject.Append(candlestick);
            }
            GetLatestIndicatorValue();
        }

        public void UpdateCandle(Candlestick candlestick)
        {
            _standardIndicator.Update(candlestick);
            foreach (AlertScript alertObject in _alertObjects)
            {
                alertObject.Update(candlestick);
            }
            GetLatestIndicatorValue();
        }

        public void Clear()
        {
            _standardIndicator.ClearRecords();
            foreach (AlertScript alertObject in _alertObjects)
            {
                alertObject.ClearRecords();
            }
        }

        public void SetStatus(bool mustBeActive)
        {
            IsActive = mustBeActive;
        }

        private void CreateAlertObjects(Indicator associatedIndicator)
        {
            _standardIndicator = new IndicatorScript(associatedIndicator.Script);
            if (!string.IsNullOrEmpty(associatedIndicator.BuyScript))
            {
                var buyAlertIndicator = new AlertScript(AlertScriptTypes.Buy, associatedIndicator.BuyScript);
                _alertObjects.Add(buyAlertIndicator);
            }
            if (!string.IsNullOrEmpty(associatedIndicator.SellScript))
            {
                var sellAlertIndicator = new AlertScript(AlertScriptTypes.Sell, associatedIndicator.SellScript);
                _alertObjects.Add(sellAlertIndicator);
            }
            if (!string.IsNullOrEmpty(associatedIndicator.TradeSignalScript))
            {
                var tradeSignalIndicator = new AlertScript(AlertScriptTypes.TradeSignal, associatedIndicator.TradeSignalScript);
                _alertObjects.Add(tradeSignalIndicator);
            }
        }

        private void BindAlertObjectEvents()
        {
            foreach (AlertScript alertObject in _alertObjects)
            {
                alertObject.OnAlertTriggered += AlertIndicator_OnAlertTriggered;
            }
        }

        private void UnBindAlertObjectEvents()
        {   
            foreach (AlertScript alertObject in _alertObjects)
            {
                alertObject.OnAlertTriggered -= AlertIndicator_OnAlertTriggered;
            }
        }

        private void GetLatestIndicatorValue()
        {
            if (_standardIndicator.RecordCount >= 50 && IsActive)
            {
                string scriptOutput = _standardIndicator.GetResult();
                if (!string.IsNullOrEmpty(scriptOutput))
                {
                    string[] lines = scriptOutput.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

                    if (lines.Length > 1)
                    {
                        string lastLine = lines[lines.Length - 1];
                        string[] lineValues = lastLine.Split(',');

                        double computedValue;
                        if (double.TryParse(lineValues[lineValues.Length - 1], out computedValue))
                        {
                            if (computedValue != _value)
                            {
                                _value = computedValue;
                                RaiseValueChangedEvent();
                            }
                        }
                    }
                }
            }
        }

        private void RaiseValueChangedEvent()
        {
            if (null != OnChanged && !_isInitializing)
                OnChanged(this);
        }

        void AlertIndicator_OnAlertTriggered(AlertScriptTypes type)
        {
            AlertTriggered = type;
            RaiseValueChangedEvent();
        }
    }
}
