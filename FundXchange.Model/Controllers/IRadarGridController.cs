using System.Collections.Generic;
using FundXchange.Model.ViewModels.Charts;
using FundXchange.Model.ViewModels.Indicators;
using FundXchange.Model.ViewModels.RadarView;
using System;
using FundXchange.Model.ViewModels.Generic;

namespace FundXchange.Model.Controllers
{
    public interface IRadarGridController
    {
        IEnumerable<SubscriptionDescriptor> SubscribedSymbolDescriptors { get; }
        RadarTemplate Template { get; set; }

        void AddRadar(SubscriptionDescriptor descriptor);
        void AddRadar(string symbol, string exchange);
        void RemoveRadars(IEnumerable<Guid> radarIdentifiers);
        SubscriptionDescriptor GetRadarDescriptor(Guid uniqueIdentifer);
        void SetRadarTimeFrames(IEnumerable<Guid> radarIdentifiers, Periodicity periodicity, int interval);
        void UpdateIndicators(IEnumerable<Indicator> indicators);
        IEnumerable<Indicator> GetActiveIndicators();
        void SetIndicatorStatus(bool areActive);
        void AddDefaultIndicators();
        void RefreshIndicator(Indicator indicatorToRefresh);

        //void ProcessExtSymbol(string symb);
    }
}
