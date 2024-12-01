using System;
using FundXchange.Model.ViewModels.Indicators;
using System.Drawing;

namespace FundXchange.Model.ViewModels.RadarView
{
    [Serializable]
    public class IndicatorColumnTemplate : IEquatable<IndicatorColumnTemplate>
    {
        public IndicatorColumnTemplate(Indicator associatedIndicator, string columnIdentifier)
        {
            AssociatedIndicator = associatedIndicator;
            ColumnIdentifier = columnIdentifier;
            SortOrder = -1;
            OverBoughtConditionColor = Color.LimeGreen;
            OverSoldConditionColor = Color.Red;
        }

        public Indicator AssociatedIndicator { get; private set; }
        public string ColumnIdentifier { get; private set; }
        public Color OverBoughtConditionColor { get; set; }
        public Color OverSoldConditionColor { get; set; }
        public int OverBoughtValue { get; set; }
        public int OverSoldValue { get; set; }
        public int SortOrder { get; set; }

        public static IndicatorColumnTemplate CreateDefault(Indicator associatedIndicator, string columnIdentifier)
        {
            return new IndicatorColumnTemplate(associatedIndicator, columnIdentifier);
        }

        #region IEquatable<IndicatorColumnTemplate> Members

        public bool Equals(IndicatorColumnTemplate other)
        {
            return this.ColumnIdentifier == other.ColumnIdentifier;
        }

        #endregion
    }
}
