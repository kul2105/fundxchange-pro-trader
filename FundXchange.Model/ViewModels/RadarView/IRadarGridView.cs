using System.Collections.Generic;
using FundXchange.Model.ViewModels.Charts;
using FundXchange.Model.ViewModels.Indicators;
using System;
using FundXchange.Model.ViewModels.Generic;

namespace FundXchange.Model.ViewModels.RadarView
{
    public interface IRadarGridView
    {
        RadarTemplate ActiveTemplate { get; }
        IEnumerable<string> SubscribedSymbolDescriptors { get; }
        bool IsEmpty { get; }
        IEnumerable<Guid> SelectedRadarItemIdentifiers { get; }

        void AddRadar(string symbol, string exchange);
        void AddRadar(SubscriptionDescriptor descriptor);
        void AddRadar(Guid uniqueId, IEnumerable<RadarGridCell> cells);
        void AddIndicator(Indicator indicator);
        void AddIndicatorColumns(Indicator indicator);
        void RemoveIndicatorColumns(Indicator indicator);
        void RefreshIndicator(Indicator indicatorToRefresh);
        void InitializeCell(RadarGridCell cell);
        void UpdateCell(RadarGridCell cell);
        void UpdateCellsFor(Guid uniqueId, IEnumerable<RadarGridCell> cells);
        void ApplyTemplate(RadarTemplate definedTemplate);
        void UpdateSelectedRadarTimeFrames(Periodicity periodicity, int interval);
        void SelectRadar(string symbol);
        void UpdateIndicators(IEnumerable<Indicator> selectedIndicators);
        IEnumerable<Indicator> GetActiveIndicators();
        void InsertLabelRow();
        void InsertBlankRow();
        void DeleteSelectedRows();
        void Clear();
        void SaveColumnOrder(string str);
        
        //void ProcessExtSymbol(string symb);
    }
}
