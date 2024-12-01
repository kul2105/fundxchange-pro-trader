using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using System.Configuration;

using VO;

namespace Logic
{
    public class MarketStatistics
    {
        public List<PriceDataVO> GetMarketsHomeData()
        {
            List<PriceDataVO> priceDataVo = new List<PriceDataVO>();
            //currencies
            priceDataVo.AddRange(new Parsing().ParsePriceData((ConfigurationManager.AppSettings["CurrencyFeedUrl"]).ToString().Replace("|", "&")));
            //JSE indicies
            priceDataVo.AddRange(new Parsing().ParsePriceData((ConfigurationManager.AppSettings["JSEIndicesFeedUrl"]).ToString().Replace("|", "&")));
            //International Inidies
            priceDataVo.AddRange(new Parsing().ParsePriceData((ConfigurationManager.AppSettings["IntIndicesFeedUrl"]).ToString().Replace("|", "&")));
            //Bonds - DateStamp
            priceDataVo.AddRange(new Parsing().ParsePriceData((ConfigurationManager.AppSettings["BondsFeedUrl"]).ToString().Replace("|", "&")));
            //commodities - Ticker
            priceDataVo.AddRange(new Parsing().ParsePriceData((ConfigurationManager.AppSettings["CommoditiesFeedUrl"]).ToString().Replace("|", "&")));

            return priceDataVo;

        }
    }
}
