using System;
using System.Drawing;

namespace FundXchange.Model.Infrastructure
{
    public class GlobalDeclarations
    {
        public const string PORTFOLIO_WATCHLIST = "PortfolioWatchList";
        public const string ALL_WATCHLIST = "AllWatchList";

        public const string LEVEL_TWO_INSTRUMENT = "LevelTwoInstrument";
        public const string EXCHANGE = "JSE";

        public static Color COLOR_POSITIVE = Color.LimeGreen;
        public static Color COLOR_NEGATIVE = Color.Red;
        public static Color COLOR_NEUTRAL = Color.Yellow;

        public const int FULL_MARKET_DEPTH = -1;
        public const int MAX_BID_OFFER_CROSS = 20;
    }
}
