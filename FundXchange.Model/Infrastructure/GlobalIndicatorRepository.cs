using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using FundXchange.Domain.ValueObjects;
using System.Net;
using System.Xml;

namespace FundXchange.Model.Infrastructure
{
    public class GlobalIndicatorRepository
    {
        public string CurrencyUrl { get; set; }
        public string JseIndiciesUrl { get; set; }
        public string IntIndiciesUrl { get; set; }
        public string CommoditiesUrl { get; set; }
        public string BondsUrl { get; set; }

        private ErrorService _errorService;

        public GlobalIndicatorRepository(ErrorService errorService)
        {
            _errorService = errorService;

            CurrencyUrl = ConfigurationManager.AppSettings["CurrencyUrl"];
            JseIndiciesUrl = ConfigurationManager.AppSettings["JseIndiciesUrl"];
            IntIndiciesUrl = ConfigurationManager.AppSettings["IntIndiciesUrl"];
            CommoditiesUrl = ConfigurationManager.AppSettings["CommoditiesUrl"];
            BondsUrl = ConfigurationManager.AppSettings["BondsUrl"];
        }

        public List<InstrumentStatistics> GetCurrencies()
        {
            List<InstrumentStatistics> indicators = GetIndicatorsFromUrl(CurrencyUrl.Replace('|', '&'));
            return indicators;
        }

        public List<InstrumentStatistics> GetJseIndicies()
        {
            List<InstrumentStatistics> indicators = GetIndicatorsFromUrl(JseIndiciesUrl.Replace('|', '&'));
            return indicators;
        }

        public List<InstrumentStatistics> GetIntIndicies()
        {
            //List<InstrumentStatistics> indicators = GetIndicatorsFromUrl(IntIndiciesUrl.Replace('|', '&'));
            //return indicators;
            return new List<InstrumentStatistics>();
        }

        public List<InstrumentStatistics> GetCommodities()
        {
            List<InstrumentStatistics> indicators = GetIndicatorsFromUrl(CommoditiesUrl.Replace('|', '&'));
            return indicators;
        }

        public List<InstrumentStatistics> GetBonds()
        {
            List<InstrumentStatistics> indicators = GetIndicatorsFromUrl(BondsUrl.Replace('|', '&'));
            return indicators;
        }

        private List<InstrumentStatistics> GetIndicatorsFromUrl(string url)
        {
            List<InstrumentStatistics> indicators = new List<InstrumentStatistics>();

            try
            {
                // Create an interface to the web
                WebClient c = new WebClient();
                // Download the XML into a string
                string xml = ASCIIEncoding.Default.GetString(c.DownloadData(url));
                // Document to contain the feed
                XmlDocument doc = new XmlDocument();
                // Parse the xml
                doc.LoadXml(xml);

                foreach (XmlNode node in doc.SelectNodes("/data/record"))
                {
                    try
                    {
                        InstrumentStatistics indicator = new InstrumentStatistics();

                        indicator.ShortName = GetNodeText(node, "ShortName");
                        indicator.Time = GetNodeText(node, "Time");
                        indicator.RP = GetNodeDouble(node, "RP");
                        indicator.Movement = GetNodeDouble(node, "Movement");
                        indicator.PercentageMoved = GetNodeDouble(node, "MovementPercentage");
                        indicator.MovementIndicator = GetNodeText(node, "MovementIndicator");
                        indicator.Currency = GetNodeText(node, "CurrencyCD");

                        indicators.Add(indicator);
                    }
                    catch (Exception parseException)
                    {
                        _errorService.LogError("Error loading global indicators", parseException);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorService.LogError("Error loading global indicators", ex);
            }
            return indicators;
        }

        private double GetNodeDouble(XmlNode node, string xpath)
        {
            string text = GetNodeText(node, xpath);
            if (!String.IsNullOrEmpty(text))
                return Convert.ToDouble(text);
            return 0;
        }

        private string GetNodeText(XmlNode node, string xpath)
        {
            XmlNode selectNode = node.SelectSingleNode(xpath);
            if (selectNode != null)
            {
                return selectNode.InnerText;
            }
            return "";
        }
    }
}
