using FundXchange.Model.ViewModels.Indicators;
using System.Collections.Generic;
using FundXchange.Domain.ValueObjects;
using FundXchange.Domain.Enumerations;

namespace FundXchange.Model.ViewModels.RadarView
{
    public delegate void RadarIndicatorChangedDelegate(RadarItemIndicator sender);

    public interface IRadarViewIndicator
    {
        event RadarIndicatorChangedDelegate OnChanged;
        Indicator AssociatedIndicator { get; }
        double Value { get; }
        bool IsOverBought { get; }
        bool IsOverSold { get; }
        AlertScriptTypes AlertTriggered { get; }
        bool IsActive { get; }
        void PrepareForRemoval();
        void AddCandle(Candlestick candlestick);
        void UpdateCandle(Candlestick candlestick);
        void InitializeWith(IEnumerable<Candlestick> candles);
        void SetStatus(bool areActive);
    }
}