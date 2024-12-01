using System;
using FundXchange.Domain.Base;

namespace FundXchange.Domain.ValueObjects
{
    public class InstrumentReference : InstrumentBase, IComparable
    {
        public string ShortName { get; set; }
        public string ISIN { get; set; }
        public string Security { get; set; }
        public string MarketSegment { get; set; }
        public string AlternativeSymbol { get; set; }
        public string InstrumentType { get; set; }
        public uint InstrumentID { get; set; }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return this.ShortName.CompareTo((obj as InstrumentReference).ShortName);
        }

        #endregion
    }

    public class MutualFundsReference : InstrumentReference
    {

    }
}
