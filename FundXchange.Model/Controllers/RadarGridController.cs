using System;
using System.Collections.Generic;
using System.Linq;
using FundXchange.Model.Infrastructure;
using FundXchange.Model.ViewModels.RadarView;
using FundXchange.Domain.Entities;
using FundXchange.Model.ViewModels.Indicators;
using FundXchange.Model.ViewModels.Charts;
using FundXchange.Model.ViewModels.Generic;
using FundXchange.Model.Repositories;
using FundXchange.Infrastructure;

namespace FundXchange.Model.Controllers
{
    public class RadarGridController : IRadarGridController
    {
        #region Private members

        private readonly IMarketRepository _marketRepository;
        private readonly IRadarGridView _view;
        private readonly IDictionary<Guid, IRadarItem> _radars;
        private IList<Indicator> _activeIndicators;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly object _lockObject = new object();

        private delegate IRadarItem CreateRadarItemDelegate(Guid radarKey, SubscriptionDescriptor descriptor);
        private delegate void RemoveIndicatorsFromRadarItemsDelegate(IEnumerable<Indicator> selectedIndicators);

        #endregion

        #region Constructor

        public RadarGridController(IMarketRepository marketRepository, IRadarGridView view)
        {
            _marketRepository = marketRepository;
            _indicatorRepository = IoC.Resolve<IIndicatorRepository>();
            _view = view;
            _radars = new Dictionary<Guid, IRadarItem>();
            _activeIndicators = new List<Indicator>();
            _marketRepository.InstrumentUpdatedEvent += MarketRepositoryInstrumentUpdatedEvent;
        }

        #endregion

        #region Public properties

        public IEnumerable<SubscriptionDescriptor> SubscribedSymbolDescriptors
        {
            get
            {
                return _radars.Where(r => r.Value != null).Select(r => r.Value.Descriptor).Distinct();
            }
        }

        private RadarTemplate _activeTemplate;
        public RadarTemplate Template
        {
            get
            {
                if (null == _activeTemplate)
                    _activeTemplate = RadarTemplate.Default;
                return _activeTemplate;
            }
            set { _activeTemplate = value; }
        }

        #endregion

        #region Public methods

        public void AddDefaultIndicators()
        {
            AddTrendIndicator();
        }

        public IEnumerable<IRadarItem> GetRadars()
        {
            IRadarItem[] radars = _radars.Values.ToArray();
            return radars;
        }

        public IEnumerable<Indicator> GetActiveIndicators()
        {
            Indicator[] indicators = _activeIndicators.ToArray();
            return indicators;
        }

        public void AddRadar(string symbol, string exchange)
        {
            var descriptor = new SubscriptionDescriptor(symbol, exchange, Template.IndicatorInterval, Template.IndicatorPeriodicity);
            AddRadar(descriptor);
        }

        public void AddRadar(SubscriptionDescriptor descriptor)
        {
            ValidateSubscriptionRequest(descriptor);

            Guid radarKey = Guid.NewGuid();
            _radars.Add(radarKey, null);

            var del = new CreateRadarItemDelegate(CreateRadarItem);
            del.BeginInvoke(radarKey, descriptor, delegate(IAsyncResult result)
            {
                if (result.IsCompleted)
                {
                    lock (_lockObject)
                    {
                        IRadarItem radarViewItem = del.EndInvoke(result);
                        if (null != radarViewItem)
                        {
                            _radars[radarKey] = radarViewItem;
                            _view.AddRadar(radarKey, radarViewItem.GetCells(false));

                            foreach (Indicator indicator in _activeIndicators)
                                radarViewItem.AddIndicator(indicator);
                        }
                        else
                        {
                            _radars.Remove(radarKey);
                        }
                    }
                }
            }, null);

        }

        public void RemoveRadars(IEnumerable<Guid> radarIdentifiers)
        {
            foreach (Guid uniqueId in radarIdentifiers)
            {
                RemoveRadar(uniqueId);
            }
        }

        public SubscriptionDescriptor GetRadarDescriptor(Guid uniqueIdentifer)
        {
            if (_radars.ContainsKey(uniqueIdentifer))
            {
                if(null != _radars[uniqueIdentifer])
                    return _radars[uniqueIdentifer].Descriptor;
            }
            return null;
        }

        public void SetRadarTimeFrames(IEnumerable<Guid> radarKeys, Periodicity periodicity, int interval)
        {
            foreach (Guid radarKey in radarKeys)
            {
                if (null != _radars[radarKey])
                {
                    _radars[radarKey].SetIndicatorIntervals(periodicity, interval, Template.StartingBars);
                }
            }
        }

        public void UpdateIndicators(IEnumerable<Indicator> selectedIndicators)
        {
            lock (_lockObject)
            {
                RemoveDeprecatedIndicators(selectedIndicators);
                AddNewIndicators(selectedIndicators.ToArray());
            }
        }

        public void RefreshIndicator(Indicator indicatorToRefresh)
        {
            var foundIndicator = _activeIndicators.FirstOrDefault(i => i.UniqueId == indicatorToRefresh.UniqueId);
            if (null != foundIndicator)
            {
                var del = new RemoveIndicatorsFromRadarItemsDelegate(RemoveIndicatorsFromRadarItems);
                del.BeginInvoke(new[] { foundIndicator }, delegate
                {
                    _activeIndicators.Remove(foundIndicator);

                    AddNewIndicator(indicatorToRefresh);
                }, null);
            }
        }

        public void SetIndicatorStatus(bool areEnabled)
        {
            foreach (var radar in _radars)
            {
                if(null != radar.Value)
                    radar.Value.SetIndicatorStatus(areEnabled);
            }
        }

        #endregion

        #region Helper methods

        private void ValidateSubscriptionRequest(SubscriptionDescriptor descriptor)
        {
            //John
            //descriptor.Exchange = "JSE";
            if (string.IsNullOrEmpty(descriptor.Symbol))
                throw new ArgumentException("Symbol cannot be empty");
            //if (string.IsNullOrEmpty(descriptor.Exchange))
            //    throw new ArgumentException("Exchange cannot be empty");

            int count = _marketRepository.AllInstrumentReferences.Where(i => i.Symbol == descriptor.Symbol).Count();//_marketRepository.AllInstrumentReferences.Where(i => i.Symbol == descriptor.Symbol).Count();
            if (count > 0)
                return;
            //if (count == 0)
            //    throw new ApplicationException(string.Format("Invalid symbol specified: {0}", descriptor.Symbol));
        }

        private IRadarItem CreateRadarItem(Guid radarKey, SubscriptionDescriptor descriptor)
        {
            Instrument radarInstrument = _marketRepository.SubscribeLevelOneWatch(descriptor.Symbol, descriptor.Exchange);
            if (null != radarInstrument)
            {
                IRadarItem radarViewItem = new RadarItem(radarKey, radarInstrument, _marketRepository, descriptor.Periodicity, descriptor.Interval, _activeTemplate.StartingBars);

                radarViewItem.OnCellAdditionRequired += RadarViewItemIndicatorAdded;
                radarViewItem.OnCellUpdateRequired += RadarViewItemIndicatorValueChanged;
                radarViewItem.OnRowUpdateRequired += RadarViewItemOnIndicatorIntervalChanged;

                return radarViewItem;
            }
            return null;
        }        

        private void RemoveRadar(Guid radarKey)
        {
            var radarEntry = _radars[radarKey];
            if (null != radarEntry)
            {
                radarEntry.PrepareForRemoval();
                _radars.Remove(radarKey);
            }
        }

        private void RemoveIndicator(Guid indicatorId)
        {
            var foundIndicator = _activeIndicators.FirstOrDefault(i => i.UniqueId == indicatorId);
            if (null != foundIndicator)
            {
                var del = new RemoveIndicatorsFromRadarItemsDelegate(RemoveIndicatorsFromRadarItems);
                del.BeginInvoke(new[] { foundIndicator }, delegate
                {
                    _activeIndicators.Remove(foundIndicator);
                }, null);
            }
        }

        private void AddTrendIndicator()
        {
            Indicator trendIndicator;
            if (_indicatorRepository.TryGetIndicatorBy("Trend indicator", out trendIndicator))
            {
                AddNewIndicator(trendIndicator);
            }
        }

        private void AddNewIndicator(Indicator indicator)
        {
            if (!_activeIndicators.Contains(indicator))
            {
                _activeIndicators.Add(indicator);
                _view.AddIndicatorColumns(indicator);                

                AddIndicatorToRadarItems(indicator);
            }
        }

        private void AddNewIndicators(IEnumerable<Indicator> selectedIndicators)
        {
            lock (_lockObject)
            {
                foreach (Indicator indicator in selectedIndicators)
                {
                    AddNewIndicator(indicator);
                }
            }
        }

        private void AddIndicatorToRadarItems(Indicator indicator)
        {
            lock (_lockObject)
            {
                foreach (var radar in _radars)
                {
                    if (null != radar.Value)
                        radar.Value.AddIndicator(indicator);
                }
            }
        }

        private void RemoveIndicatorsFromRadarItems(IEnumerable<Indicator> indicatorsToRemove)
        {
            lock (_lockObject)
            {
                foreach (Indicator indicator in indicatorsToRemove)
                {
                    foreach (var radar in _radars)
                    {
                        if (null != radar.Value)
                            radar.Value.RemoveIndicator(indicator);
                    }
                    _view.RemoveIndicatorColumns(indicator);
                }
            }
        }

        private void RemoveDeprecatedIndicators(IEnumerable<Indicator> selectedIndicators)
        {
            var removalList = new List<Indicator>();
            foreach (Indicator indicator in _activeIndicators)
            {
                if (!selectedIndicators.Contains(indicator))
                {
                    removalList.Add(indicator);
                }
            }

            var del = new RemoveIndicatorsFromRadarItemsDelegate(RemoveIndicatorsFromRadarItems);
            del.BeginInvoke(removalList, delegate
            {
                _activeIndicators = selectedIndicators.ToList();
            }, null);
        }

        #endregion

        #region Event handlers

        private void MarketRepositoryInstrumentUpdatedEvent(Instrument instrument)
        {
            lock (_lockObject)
            {
                var interestedRadars = _radars.Values.Where(ri => ri != null && ri.IsInterestedIn(instrument));
                foreach (var radarItem in interestedRadars)
                {
                    radarItem.UpdateWith(instrument);
                    _view.UpdateCellsFor(radarItem.UniqueId, radarItem.GetCells(true));
                } 
            }
        }

        void RadarViewItemOnIndicatorIntervalChanged(Guid itemIdentifier, IEnumerable<RadarGridCell> cells)
        {
            _view.UpdateCellsFor(itemIdentifier, cells);
        }

        void RadarViewItemIndicatorAdded(RadarGridCell newCell)
        {
            _view.InitializeCell(newCell);
        }

        void RadarViewItemIndicatorValueChanged(RadarGridCell updatedCell)
        {
            _view.UpdateCell(updatedCell);
        }

        #endregion

        #region IRadarGridController Members


        public void ProcessExtSymbol(string symb)
        {
            //_view.ProcessExtSymbol(symb);
        }

        #endregion
    }
}
