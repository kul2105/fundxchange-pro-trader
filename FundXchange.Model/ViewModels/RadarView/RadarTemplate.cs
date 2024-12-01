using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using FundXchange.Model.ViewModels.Charts;
using FundXchange.Model.Serialization;

namespace FundXchange.Model.ViewModels.RadarView
{
    [Serializable]
    public class RadarTemplate : IPersistable
    {
        public RadarTemplate()
        {
            _columnTemplates = new List<IndicatorColumnTemplate>();
        }

        public string PageName { get; set; }
        public bool IndicatorsEnabled { get; set; }
        public Periodicity IndicatorPeriodicity { get; set; }
        public int IndicatorInterval { get; set; }
        public int StartingBars { get; set; }
        public Font Font { get; set; }
        public bool ShowVerticalGridLines { get; set; }
        public bool ShowHorizontalGridLines { get; set; }
        public Color GridLineColor { get; set; }
        public bool IsSavedToDisk { get; set; }        
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public static RadarTemplate Default
        {
            get { return GetDefault(); }
        }

        private readonly List<IndicatorColumnTemplate> _columnTemplates;
        public IEnumerable<IndicatorColumnTemplate> ColumnTemplates
        {
            get { return _columnTemplates; }
        }

        #region To be removed

        public RadarAlertType AlertType;
        public bool HasMultipleIndicators;
        public bool IndicatorLong;
        public bool BlankRow;
        public bool UpperCase;
        public bool ReferenceRows;

        #endregion

        public void AddColumnTemplate(IndicatorColumnTemplate columnTemplate)
        {
            if (!_columnTemplates.Contains(columnTemplate))
            {
                _columnTemplates.Add(columnTemplate);
            }
        }

        public void UpdateColumnTemplate(IndicatorColumnTemplate columnTemplate)
        {
            if (_columnTemplates.Contains(columnTemplate))
            {
                RemoveColumnTemplate(columnTemplate.ColumnIdentifier);
            }
            AddColumnTemplate(columnTemplate);
        }

        public void RemoveColumnTemplate(string columnIdentifier)
        {
            IndicatorColumnTemplate associatedTemplate = _columnTemplates.FirstOrDefault(ct => ct.ColumnIdentifier == columnIdentifier);
            if (null != associatedTemplate)
            {
                _columnTemplates.Remove(associatedTemplate);
            }
        }

        public IndicatorColumnTemplate FindTemplate(string columnIdentifier)
        {
            return _columnTemplates.FirstOrDefault(ct => ct.ColumnIdentifier == columnIdentifier);
        }

        public static RadarTemplate GetDefault()
        {
            return new RadarTemplate()
            {
                PageName = "Page 1",
                IndicatorsEnabled = true,
                IndicatorPeriodicity = Periodicity.Daily,//Periodicity.Minutely,
                IndicatorInterval = 1,//5,
                StartingBars = 100,
                Font = SystemFonts.DefaultFont,
                ShowHorizontalGridLines = true,
                ShowVerticalGridLines = true,
                GridLineColor = SystemColors.Window
            };
        }

        public static RadarTemplate Clone(RadarTemplate templateToClone)
        {
            var newTemplate = new RadarTemplate()
            {
                PageName = templateToClone.PageName,
                Font = templateToClone.Font,
                IndicatorsEnabled = templateToClone.IndicatorsEnabled,
                IndicatorPeriodicity = templateToClone.IndicatorPeriodicity,
                StartingBars = templateToClone.StartingBars,
                IndicatorInterval = templateToClone.IndicatorInterval,
                ShowHorizontalGridLines = templateToClone.ShowHorizontalGridLines,
                ShowVerticalGridLines = templateToClone.ShowVerticalGridLines,
                GridLineColor = templateToClone.GridLineColor,
                IsSavedToDisk = templateToClone.IsSavedToDisk,
                FileName = templateToClone.FileName,
                FilePath = templateToClone.FilePath
            };
            foreach (var columnTemplate in templateToClone.ColumnTemplates)
            {
                newTemplate.AddColumnTemplate(columnTemplate);
            }
            return newTemplate;
        }
    }
}
