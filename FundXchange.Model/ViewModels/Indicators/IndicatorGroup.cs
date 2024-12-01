using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FundXchange.Model.ViewModels.Indicators
{
    public class IndicatorGroup
    {
        public IndicatorGroup(string groupName, IEnumerable<Indicator> siblingIndicators)
        {
            _siblingIndicators = siblingIndicators;
        }

        public IndicatorGroup(string groupName, params Indicator[] siblingIndicators)
            : this(groupName, siblingIndicators.AsEnumerable()) { }

        private readonly string _groupName;
        private readonly IEnumerable<Indicator> _siblingIndicators;

        public bool HasChildren()
        {
            return _siblingIndicators.Count() > 0;
        }

        public bool HasChild(Indicator indicator)
        {
            return _siblingIndicators.Contains(indicator);
        }

        public IEnumerable<Indicator> GetSiblingsOf(Indicator indicator)
        {
            return _siblingIndicators.Except(new[] { indicator });
        }

        public static IndicatorGroup Create(string groupName, IEnumerable<Indicator> availableIndicators, Func<Indicator, bool> predicate)
        {
            return new IndicatorGroup(groupName, availableIndicators.Where(predicate));
        }
    }
}
