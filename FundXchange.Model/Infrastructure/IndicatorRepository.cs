using System.Linq;
using System.Collections.Generic;
using FundXchange.Model.Repositories;
using FundXchange.Model.ViewModels.Indicators;
using FundXchange.Model.AuthenticationService;
using FundXchange.Infrastructure;
using FundXchange.Model.Gateways;
using System;

namespace FundXchange.Model.Infrastructure
{
    public class IndicatorRepository : IIndicatorRepository
    {
        static IndicatorRepository()
        {

            User = IoC.Resolve<UserDTO>();
            InitializeIndicators();
            InitializeIndicatorGroups();
        }

        private static readonly UserDTO User;
        private static readonly AlertServiceGateway ServiceGateway = new AlertServiceGateway(new ErrorService());
        private static readonly List<Indicator> Indicators = new List<Indicator>();
        private static IEnumerable<IndicatorGroup> IndicatorGroups = new List<IndicatorGroup>();
        
        public IEnumerable<Indicator> AvailableIndicators
        {
            get { return Indicators; }
        }

        private static void InitializeIndicators()
        {
            //return;//By John            
            foreach (var alertScript in User.AlertScripts)
            {
                //System.Diagnostics.Debug.WriteLine("------AlertScript starts------");
                //System.Diagnostics.Debug.WriteLine(item.AlertName);
                //System.Diagnostics.Debug.WriteLine(item.Abbreviation);
                //System.Diagnostics.Debug.WriteLine(item.Bars);
                //System.Diagnostics.Debug.WriteLine(item.BuyScript);
                //System.Diagnostics.Debug.WriteLine(item.DayHours);
                //System.Diagnostics.Debug.WriteLine(item.DefaultScript);
                //System.Diagnostics.Debug.WriteLine(item.Enabled);
                //System.Diagnostics.Debug.WriteLine(item.EndOfDay);
                //System.Diagnostics.Debug.WriteLine(item.Exchange);
                //System.Diagnostics.Debug.WriteLine(item.ExitLongScript);
                //System.Diagnostics.Debug.WriteLine(item.ExitShortScript);
                //System.Diagnostics.Debug.WriteLine(item.GTC);
                //System.Diagnostics.Debug.WriteLine(item.GTCHours);
                //System.Diagnostics.Debug.WriteLine(item.Interval);
                //System.Diagnostics.Debug.WriteLine(item.IsUserDefined);
                //System.Diagnostics.Debug.WriteLine(item.Limit);
                //System.Diagnostics.Debug.WriteLine(item.Market);
                //System.Diagnostics.Debug.WriteLine(item.NumberOfLines);
                //System.Diagnostics.Debug.WriteLine(item.Period);
                //System.Diagnostics.Debug.WriteLine(item.Portfolio);
                //System.Diagnostics.Debug.WriteLine(item.Quantity);
                //System.Diagnostics.Debug.WriteLine(item.SellScript);
                //System.Diagnostics.Debug.WriteLine(item.StopLimit);
                //System.Diagnostics.Debug.WriteLine(item.StopLimitValue);
                //System.Diagnostics.Debug.WriteLine(item.StopMarket);
                //System.Diagnostics.Debug.WriteLine(item.Symbol);
                //System.Diagnostics.Debug.WriteLine(item.TradeSignalScript);
                //System.Diagnostics.Debug.WriteLine(item.UniqueId);
                //System.Diagnostics.Debug.WriteLine("------AlertScript Ends------");

                //By John for authentication Issue
                if (!AlertScriptAlreadyRepresented(alertScript))
                {
                    Indicators.Add(CreateIndicatorFrom(alertScript));
                }
            }
        }

        private static bool AlertScriptAlreadyRepresented(AlertScriptDTO alertScript)
        {
            Indicator foundIndicator = Indicators.FirstOrDefault(i => i.Name == alertScript.AlertName);
            return foundIndicator != null;
        }

        private static void InitializeIndicatorGroups()
        {
            IndicatorGroupingStrategy groupingStrategy = new IndicatorGroupingStrategy(Indicators);
            IndicatorGroups = groupingStrategy.GetGroups();
        }

        private static Indicator CreateIndicatorFrom(AlertScriptDTO alertScript)
        {
            return new Indicator(alertScript.UniqueId, alertScript.AlertName, alertScript.Abbreviation, alertScript.DefaultScript, alertScript.IsUserDefined) 
            { 
                BuyScript = alertScript.BuyScript, 
                SellScript = alertScript.SellScript, 
                TradeSignalScript = alertScript.TradeSignalScript 
            };
        }

        public void AddIndicator(Indicator indicator)
        {
            Indicators.Add(indicator);
            var newAlertScript = CreateAlertScriptDtoFrom(indicator);
            User.AlertScripts = User.AlertScripts.Union(new List<AlertScriptDTO>() { newAlertScript }).ToArray();
            ServiceGateway.SaveAlertScript(User.Account.UserId, CreateAlertScriptDtoFrom(newAlertScript));
        }

        public void UpdateIndicator(Guid uniqueId, Indicator newIndicatorDefinition)
        {
            if (newIndicatorDefinition.IsUserDefined)
            {
                RemoveIndicator(uniqueId);
                AddIndicator(newIndicatorDefinition);
            }
        }

        private FundXchange.Model.AlertService.AlertScriptDTO CreateAlertScriptDtoFrom(AlertScriptDTO newAlertScript)
        {
            return new FundXchange.Model.AlertService.AlertScriptDTO()
            {
                UniqueId = newAlertScript.UniqueId,
                Abbreviation = newAlertScript.Abbreviation,
                AlertName = newAlertScript.AlertName,
                Bars = newAlertScript.Bars,
                BuyScript = newAlertScript.BuyScript,
                DayHours = newAlertScript.DayHours,
                DefaultScript = newAlertScript.DefaultScript,
                Enabled = newAlertScript.Enabled,
                EndOfDay = newAlertScript.EndOfDay,
                Exchange = newAlertScript.Exchange,
                ExitLongScript = newAlertScript.ExitLongScript,
                ExitShortScript = newAlertScript.ExitShortScript,
                GTC = newAlertScript.GTC,
                GTCHours = newAlertScript.GTCHours,
                Interval = newAlertScript.Interval,
                IsUserDefined = newAlertScript.IsUserDefined,
                Limit = newAlertScript.Limit,
                Market = newAlertScript.Market,
                Period = newAlertScript.Period,
                Portfolio = newAlertScript.Portfolio,
                Quantity = newAlertScript.Quantity,
                SellScript = newAlertScript.SellScript,
                StopLimit = newAlertScript.StopLimit,
                StopLimitValue = newAlertScript.StopLimitValue,
                StopMarket = newAlertScript.StopMarket,
                Symbol = newAlertScript.Symbol,
                TradeSignalScript = newAlertScript.TradeSignalScript
            };
        }

        private AlertScriptDTO CreateAlertScriptDtoFrom(Indicator indicator)
        {
            return new AlertScriptDTO()
            {
                AlertName = indicator.Name,
                Abbreviation = indicator.Abbreviation,
                DefaultScript = indicator.Script,
                BuyScript = indicator.BuyScript,
                SellScript = indicator.SellScript,
                TradeSignalScript = indicator.TradeSignalScript,
                IsUserDefined = true,
                UniqueId = indicator.UniqueId
            };
        }

        public bool TryGetIndicatorBy(string name, out Indicator indicator)
        {
            indicator = Indicators.FirstOrDefault(i => i.Name.ToLowerInvariant() == name.ToLowerInvariant());
            return indicator != null;
        }

        public bool ScriptNameIsValid(string name)
        {
            var foundSystemIndicator = Indicators.FirstOrDefault(i => i.Name.ToLowerInvariant() == name.ToLowerInvariant() && !i.IsUserDefined);
            return foundSystemIndicator == null;
        }

        #region IIndicatorRepository Members


        public bool IsGroupedIndicator(Indicator indicator)
        {
            foreach (var group in IndicatorGroups)
            {
                if (group.HasChild(indicator))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<Indicator> GetSiblingsOf(Indicator indicator)
        {
            if (IsGroupedIndicator(indicator))
            {
                IndicatorGroup group = FindGroup(indicator);
                if (null != group)
                {
                    return group.GetSiblingsOf(indicator);
                }
            }
            return new Indicator[] { };
        }

        private IndicatorGroup FindGroup(Indicator indicator)
        {
            foreach (var group in IndicatorGroups)
            {
                if (group.HasChild(indicator))
                {
                    return group;
                }
            }
            return null;
        }

        #endregion

        public void RefreshIndicators()
        {
            InitializeIndicators();
        }

        public void RemoveIndicator(System.Guid uniqueId)
        {
            var foundIndicator = Indicators.FirstOrDefault(i => i.UniqueId == uniqueId);
            if (null != foundIndicator)
            {
                if (foundIndicator.IsUserDefined)
                {
                    Indicators.Remove(foundIndicator);
                    ServiceGateway.RemoveAlertScript(User.Account.UserId, foundIndicator.UniqueId);
                }
            }
        }

        private class IndicatorGroupingStrategy
        {
            private readonly IEnumerable<Indicator> _availableIndicators;

            private readonly IDictionary<string, Func<Indicator, bool>> _requiredGroups;

            public IndicatorGroupingStrategy(IEnumerable<Indicator> availableIndicators)
            {
                _availableIndicators = availableIndicators;
                _requiredGroups = new Dictionary<string, Func<Indicator, bool>>()
                {
                    {"MACD", new Func<Indicator, bool>(i => i.Name.StartsWith("MACD") && !i.Name.ToLowerInvariant().Contains("system") && !i.IsUserDefined)}, 
                    {"Stochastic Momentum", new Func<Indicator, bool>(i => i.Name.StartsWith("Stochastic Momentum") && !i.IsUserDefined)}, 
                    {"Stochastic Oscillator", new Func<Indicator, bool>(i => i.Name.StartsWith("Stochastic Oscillator") && !i.IsUserDefined)}, 
                    {"Prime Number Bands", new Func<Indicator, bool>(i => i.Name.StartsWith("Prime Number Bands") && !i.IsUserDefined)}, 
                    {"Moving Average Envelope", new Func<Indicator, bool>(i => i.Name.StartsWith("Moving Average Envelope") && !i.IsUserDefined)}, 
                    {"Keltner Channel", new Func<Indicator, bool>(i => i.Name.StartsWith("Keltner Channel") && !i.IsUserDefined)}, 
                    {"Bollinger Bands", new Func<Indicator, bool>(i => i.Name.StartsWith("Bollinger Bands") && !i.IsUserDefined)}
                };
            }

            public IEnumerable<IndicatorGroup> GetGroups()
            {
                List<IndicatorGroup> groups = new List<IndicatorGroup>();
                foreach (var requiredGroup in _requiredGroups)
                {
                    IndicatorGroup group = new IndicatorGroup(requiredGroup.Key, _availableIndicators.Where(requiredGroup.Value));
                    if(group.HasChildren())
                    {
                        groups.Add(group);
                    }
                }
                return groups;
            }
        }
    }
}