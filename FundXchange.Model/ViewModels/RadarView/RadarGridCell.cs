using FundXchange.Model.ViewModels.Generic;
using System.Drawing;
using System;
using FundXchange.Domain.Enumerations;

namespace FundXchange.Model.ViewModels.RadarView
{
    public class RadarGridCell : GridCell
    {
        public RadarGridCell(Guid ownerId, string text, string tag)
            : this(ownerId, text, tag, RadarColumnType.InstrumentValue)
        {
        }

        public RadarGridCell(Guid ownerId, string text, string tag, RadarColumnType associatedColumnType)
            : base(text, Color.Black, Color.White)
        {
            OwnerId = ownerId;
            Tag = string.Format("{0}_{1}", tag, associatedColumnType);
            AssociatedColumnType = associatedColumnType;
        }

        public Guid OwnerId { get; private set; }
        public string Tag { get; private set; }
        public RadarColumnType AssociatedColumnType { get; private set; }
        public AlertScriptTypes AlertTriggered { get; set; }
    }
}
