using System;

namespace CommonLibrary.Cls
{
    [Serializable]
    public class ClsContracts
    {
        public ClsContracts(string instrumentId, string productType, string productName, string contract,
                            string seriesExpiry, string strikePrice, string optionType, string tradingCurrency)
        {
            // TODO: Complete member initialization
            InstrumentId = instrumentId;
            ProductType = productType;
            ProductName = productName;
            Contract = contract;
            SeriesExpiry = seriesExpiry;
            StrikePrice = strikePrice;
            OptionType = optionType;
            TradingCurrency = tradingCurrency;
        }

        public string InstrumentId { get; set; }
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public string Contract { get; set; }
        public string SeriesExpiry { get; set; }
        public string StrikePrice { get; set; }
        public string OptionType { get; set; }
        public string TradingCurrency { get; set; }
    }
}