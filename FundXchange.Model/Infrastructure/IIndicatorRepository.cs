using System.Collections.Generic;
using FundXchange.Model.ViewModels.Indicators;
using System;

namespace FundXchange.Model.Repositories
{
    public interface IIndicatorRepository
    {
        IEnumerable<Indicator> AvailableIndicators { get; }
        void AddIndicator(Indicator indicator);
        void UpdateIndicator(Guid uniqueId, Indicator newIndicatorDefinition);
        void RemoveIndicator(System.Guid guid);
        bool IsGroupedIndicator(Indicator indicator);
        void RefreshIndicators();
        IEnumerable<Indicator> GetSiblingsOf(Indicator indicator);
        bool TryGetIndicatorBy(string name, out Indicator indicator);
        bool ScriptNameIsValid(string name);
    }
}