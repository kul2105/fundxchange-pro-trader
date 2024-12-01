using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ClientDLL_Model.Cls;
using Nevron.UI.WinForm.Controls;
using ClientDLL_Model.Cls.Contract;
//BOChanges using PALSA.AlertScriptService;
using ClientDLL_Model.Cls.Market;
using System.Globalization;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using CommonLibrary.Cls;

namespace PALSA.Cls
{
    public static class ClsGlobal
    {
        public static int BrokerAccountId = 0;
        public static int MarketMakerAccountId = 0;
        //public static string CurrentPortfolio;
        //public static object LatestPortfolio;
        //public static Dictionary<string, InstrumentSpec> DDContractInfo;
        public static Dictionary<string, sbyte> DDOrderTypes;
        public static Dictionary<string, sbyte> DDSide;
        public static Dictionary<string, sbyte> DDValidity;
        public static Dictionary<string, sbyte> DDProductTypes;
        public static Dictionary<string, sbyte> DDOrderStatus;
        public static Dictionary<sbyte, string> DDReverseOrderStatus;
        public static Dictionary<sbyte, string> DDReverseProductType;
        public static Dictionary<sbyte, string> DDReverseOrderType;
        public static Dictionary<sbyte, string> DDReverseSide;
        public static Dictionary<sbyte, string> DDReverseValidity;
        public static Dictionary<string, double> DDLeftZLevel;
        public static Dictionary<string, double> DDRightZLevel;
        public static Dictionary<string, double> DDLTP;
        public static Dictionary<string, int> DDRightZLevelQty;
        public static Dictionary<string, int> DDLeftZLevelQty;
        public static Dictionary<string, double> DDTotalValue;
        public static Dictionary<string, long> DDTotalVolume;
        public static Dictionary<string, double> DDConversion;
        private static int _clientOrderId = 700;
        //private static List<String> indicatorListValues;
        public static string[] Countries;
        public static string[] Leverage;
        //private static readonly AlertScriptServiceClient AlertServ = new AlertScriptService.AlertScriptServiceClient();
        //BOChanges public static List<AlertScripts> AlertScripts; //= AlertServ.GetAlertScripts();
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int LCMapString(int Locale, int dwMapFlags, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpSrcStr, int cchSrc, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpDestStr, int cchDest);
        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int LCMapStringA(int Locale, int dwMapFlags, [MarshalAs(UnmanagedType.LPArray)] byte[] lpSrcStr, int cchSrc, [MarshalAs(UnmanagedType.LPArray)] byte[] lpDestStr, int cchDest);
        static ClsGlobal()
        {
            //try
            //{
            //    Countries = PALSA.Cls.ClsTWSOrderManager.INSTANCE.GetCountryList().ToArray();
            //}
            //catch
            //{
            Countries = new string[] { "India", "Nepal", "USA" };
            //}
            //try
            //{
            //    Leverage = PALSA.Cls.ClsTWSOrderManager.INSTANCE.GetLeverageList().ToArray();
            //}
            //catch
            //{
            Leverage = new string[] { "1", "10", "100" };
            //}

            //DDContractInfo = new Dictionary<string, InstrumentSpec>();
            //LatestPortfolio = new object();
            //CurrentPortfolio = string.Empty;
            //BOChanges AlertScripts = new List<AlertScriptService.AlertScripts>();
            DDRightZLevelQty = new Dictionary<string, int>();
            DDLeftZLevelQty = new Dictionary<string, int>();
            DDLeftZLevel = new Dictionary<string, double>();
            DDRightZLevel = new Dictionary<string, double>();
            DDLTP = new Dictionary<string, double>();
            FieldInfo[] fieldInfos = typeof(clsHashDefine).GetFields();
            DDConversion = new Dictionary<string, double>();
            if(!DDConversion.Keys.Contains("AUDCAD"))
            DDConversion.Add("AUDCAD", 1.01366);
            if (!DDConversion.Keys.Contains("AUDCHF"))
            DDConversion.Add("AUDCHF", 1.07849);
            if (!DDConversion.Keys.Contains("AUDJPY"))
            DDConversion.Add("AUDJPY", 0.01137);
            if (!DDConversion.Keys.Contains("AUDNZD"))
            DDConversion.Add("AUDNZD", 0.82287);
            if (!DDConversion.Keys.Contains("AUDUSD"))
            DDConversion.Add("AUDUSD", 1);
            if (!DDConversion.Keys.Contains("CADCHF"))
            DDConversion.Add("CADCHF", 1.07849);
            if (!DDConversion.Keys.Contains("CADJPY"))
            DDConversion.Add("CADJPY", 0.0113726);
            if (!DDConversion.Keys.Contains("CHFJPY"))
            DDConversion.Add("CHFJPY", 0.0113726);
            if (!DDConversion.Keys.Contains("EURAUD"))
            DDConversion.Add("EURAUD", 1.04172);
            if (!DDConversion.Keys.Contains("EURCAD"))
            DDConversion.Add("EURCAD", 1.01366);
            if (!DDConversion.Keys.Contains("EURCHF"))
            DDConversion.Add("EURCHF", 1.07849);
            if (!DDConversion.Keys.Contains("EURGBP"))
            DDConversion.Add("EURGBP", 1.60189);
            if (!DDConversion.Keys.Contains("EURJPY"))
            DDConversion.Add("EURJPY", 0.01137);
            if (!DDConversion.Keys.Contains("EURNZD"))
            DDConversion.Add("EURNZD", 0.82338);
            if (!DDConversion.Keys.Contains("EURUSD"))
            DDConversion.Add("EURUSD", 1);
            if (!DDConversion.Keys.Contains("GBPAUD"))
            DDConversion.Add("GBPAUD", 1.04185);
            if (!DDConversion.Keys.Contains("GBPCAD"))
            DDConversion.Add("GBPCAD", 1.01366);
            if (!DDConversion.Keys.Contains("GBPCHF"))
            DDConversion.Add("GBPCHF", 1.07849);
            if (!DDConversion.Keys.Contains("GBPJPY"))
            DDConversion.Add("GBPJPY", 0.01137);
            if (!DDConversion.Keys.Contains("GBPUSD"))
            DDConversion.Add("GBPUSD", 1);
            if (!DDConversion.Keys.Contains("NZDJPY"))
            DDConversion.Add("NZDJPY", 0.01137);
            if (!DDConversion.Keys.Contains("NZDUSD"))
            DDConversion.Add("NZDUSD", 1);
            if (!DDConversion.Keys.Contains("USDCAD"))
            DDConversion.Add("USDCAD", 1.01366);
            if (!DDConversion.Keys.Contains("USDCHF"))
            DDConversion.Add("USDCHF", 1.07849);
            if (!DDConversion.Keys.Contains("USDJPY"))
            DDConversion.Add("USDJPY", 0.01137);
            DDOrderStatus = new Dictionary<string, sbyte>();
            DDProductTypes = new Dictionary<string, sbyte>();
            DDOrderTypes = new Dictionary<string, sbyte>();
            DDSide = new Dictionary<string, sbyte>();
            DDValidity = new Dictionary<string, sbyte>();
            DDReverseOrderStatus = new Dictionary<sbyte, string>();
            DDReverseProductType = new Dictionary<sbyte, string>();
            DDReverseOrderType = new Dictionary<sbyte, string>();
            DDReverseSide = new Dictionary<sbyte, string>();
            DDReverseValidity = new Dictionary<sbyte, string>();
            DDTotalValue = new Dictionary<string, double>();
            DDTotalVolume = new Dictionary<string, long>();
            for (int i = 0; i < fieldInfos.Count(); i++)
            {
                if (fieldInfos[i].Name.Contains("ORDER_TYPE_"))
                {
                    string x = fieldInfos[i].Name.Replace("ORDER_TYPE_", "").Replace("_ORDER", "");
                    DDOrderTypes.Add(x, (sbyte)Convert.ToByte(fieldInfos[i].GetValue(fieldInfos[i])));
                }
                else if (fieldInfos[i].Name.Contains("SIDE_BUY") || fieldInfos[i].Name.Contains("SIDE_SELL"))
                {
                    string x = fieldInfos[i].Name.Replace("SIDE_", "");
                    DDSide.Add(x, (sbyte)Convert.ToByte(fieldInfos[i].GetValue(fieldInfos[i])));
                }
                else if (fieldInfos[i].Name.Contains("TIF_"))
                {
                    string x = fieldInfos[i].Name.Replace("TIF_", "");
                    DDValidity.Add(x, (sbyte)Convert.ToByte(fieldInfos[i].GetValue(fieldInfos[i])));
                }
                else if (fieldInfos[i].Name.Contains("ORDER_STATUS_"))
                {
                    string x = fieldInfos[i].Name.Replace("ORDER_STATUS_", "");
                    if (x.ToLower() == "new")
                        x = "WORKING";
                    DDOrderStatus.Add(x, (sbyte)Convert.ToByte(fieldInfos[i].GetValue(fieldInfos[i])));
                }
                else if (fieldInfos[i].Name.Contains("SECURITY_TYPE_"))
                {
                    string x = fieldInfos[i].Name.Replace("SECURITY_TYPE_", "");
                    DDProductTypes.Add(x, (sbyte)Convert.ToByte(fieldInfos[i].GetValue(fieldInfos[i])));
                }
            }
            foreach (string s in DDOrderStatus.Keys.ToArray())
            {
                if (!DDReverseOrderStatus.ContainsKey(DDOrderStatus[s]))
                    DDReverseOrderStatus.Add(DDOrderStatus[s], s);
            }
            foreach (string s in DDOrderTypes.Keys.ToArray())
            {
                DDReverseOrderType.Add(DDOrderTypes[s], s);
            }
            foreach (string s in DDSide.Keys.ToArray())
            {
                DDReverseSide.Add(DDSide[s], s);
            }
            foreach (string s in DDProductTypes.Keys.ToArray())
            {
                DDReverseProductType.Add(DDProductTypes[s], s);
            }
            foreach (string s in DDValidity.Keys.ToArray())
            {
                DDReverseValidity.Add(DDValidity[s], s);
            }
            //indicatorListValues = new List<string>();
            //LoadListValues();
            //LoadAlertScripts();
        }

        public static bool IsNumeric(object Expression)
        {
            IConvertible convertible = Expression as IConvertible;
            if (convertible != null)
            {
                switch (convertible.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return true;

                    case TypeCode.Char:
                    case TypeCode.String:
                        {
                            double num;
                            string str = convertible.ToString(null);
                            try
                            {
                                long num2 = 0;
                                if (IsHexOrOctValue(str, ref num2))
                                {
                                    return true;
                                }
                            }
                            catch (FormatException)
                            {
                                return false;
                            }
                            return double.TryParse(str, out num);
                        }
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
            }
            return false;
        }
        internal static bool IsHexOrOctValue(string Value, ref long i64Value)
        {
            int num = 0;
            int length = Value.Length;
            while (num < length)
            {
                char ch = Value[num];
                if ((ch == '&') && ((num + 2) < length))
                {
                    ch = char.ToLower(Value[num + 1], CultureInfo.InvariantCulture);
                    string str = ToHalfwidthNumbers(Value.Substring(num + 2), GetCultureInfo());
                    switch (ch)
                    {
                        case 'h':
                            i64Value = Convert.ToInt64(str, 0x10);
                            return true;

                        case 'o':
                            i64Value = Convert.ToInt64(str, 8);
                            return true;
                    }
                    throw new FormatException();
                }
                if ((ch != ' ') && (ch != '　'))
                {
                    return false;
                }
                num++;
            }
            return false;
        }
        internal static string ToHalfwidthNumbers(string s, CultureInfo culture)
        {
            int num = culture.LCID & 0x3ff;
            if (((num != 4) && (num != 0x11)) && (num != 0x12))
            {
                return s;
            }
            return vbLCMapString(culture, 0x400000, s);
        }

        internal static string vbLCMapString(CultureInfo loc, int dwMapFlags, string sSrc)
        {
            int length = sSrc == null ? 0 : sSrc.Length;
            if (length == 0)
            {
                return "";
            }
            int lCID = loc.LCID;
            Encoding encoding = Encoding.GetEncoding(loc.TextInfo.ANSICodePage);
            if (!encoding.IsSingleByte)
            {
                string s = sSrc;
                if (s != null)
                {
                    byte[] bytes = encoding.GetBytes(s);
                    int num2 = LCMapStringA(lCID, dwMapFlags, bytes, bytes.Length, null, 0);
                    byte[] buffer = new byte[(num2 - 1) + 1];
                    LCMapStringA(lCID, dwMapFlags, bytes, bytes.Length, buffer, num2);
                    return encoding.GetString(buffer);
                }
            }
            string lpDestStr = new string(' ', length);
            LCMapString(lCID, dwMapFlags, ref sSrc, length, ref lpDestStr, length);
            return lpDestStr;
        }

        internal static CultureInfo GetCultureInfo()
        {
            return Thread.CurrentThread.CurrentCulture;
        }


        //private static void LoadAlertScripts()
        //{
        //    AlertScripts = AlertServ.GetAlertScripts();
        //}

        //private static void LoadListValues()
        //{

        //    #region For WPF Chart
        //    //AddNewIndicator("Simple Moving Average");//// 0,
        //    //AddNewIndicator("Exponential Moving Average");//// 1,
        //    //AddNewIndicator("Time Series Moving Average");//// 2,
        //    //AddNewIndicator("Triangular Moving Average");//// 3,
        //    //AddNewIndicator("Variable Moving Average");//// 4,
        //    //AddNewIndicator("VIDYA");//// 5,
        //    //AddNewIndicator("Welles Wilder Smoothing");//// 6,
        //    //AddNewIndicator("Weighted Moving Average");//// 7,
        //    //AddNewIndicator("Williams PctR");//// 8,
        //    //AddNewIndicator("Williams Accumulation Distribution");//// 9,
        //    //AddNewIndicator("Volume Oscillator");//// 10,
        //    //AddNewIndicator("Vertical Horizontal Filter");//// 11,
        //    ////AddNewIndicator("Ultimate Oscillator");//// 12,
        //    //AddNewIndicator("True Range");//// 13,
        //    //AddNewIndicator("TRIX");//// 14,
        //    //AddNewIndicator("Rainbow Oscillator");//// 15,
        //    //AddNewIndicator("Price Oscillator");//// 16,
        //    //AddNewIndicator("Parabolic SAR");//// 17,
        //    //AddNewIndicator("Momentum Oscillator");//// 18,
        //    //AddNewIndicator("MACD");//// 19,
        //    //AddNewIndicator("Ease Of Movement");//// 20,
        //    ////AddNewIndicator("Directional Movement System");//// 21,
        //    //AddNewIndicator("Detrended Price Oscillator");//// 22,
        //    //AddNewIndicator("Chande Momentum Oscillator");//// 23,
        //    //AddNewIndicator("Chaikin Volatility");//// 24,
        //    //AddNewIndicator("Aroon");//// 25,
        //    //AddNewIndicator("Aroon Oscillator");//// 26,
        //    //AddNewIndicator("Linear Regression RSquared");//// 27,
        //    //AddNewIndicator("Linear Regression Forecast");//// 28,
        //    //AddNewIndicator("Linear Regression Slope");//// 29,
        //    //AddNewIndicator("Linear Regression Intercept");//// 30,
        //    ////AddNewIndicator("Price Volume Trend");//// 31,
        //    //AddNewIndicator("Performance Index");//// 32,
        //    //AddNewIndicator("Commodity Channel Index");//// 33,
        //    //AddNewIndicator("Chaikin Money Flow");//// 34,
        //    //AddNewIndicator("Weighted Close");//// 35,
        //    //AddNewIndicator("Volume ROC");//// 36,
        //    //AddNewIndicator("Typical Price");//// 37,
        //    //AddNewIndicator("Standard Deviation");//// 38,
        //    //AddNewIndicator("Price ROC");//// 39,
        //    //AddNewIndicator("Median");//// 40,
        //    //AddNewIndicator("High Minus Low");//// 41,
        //    //AddNewIndicator("Bollinger Bands");//// 42,
        //    //AddNewIndicator("Fractal Chaos Bands");//// 43,
        //    //AddNewIndicator("High Low Bands");//// 44,
        //    //AddNewIndicator("Moving Average Envelope");//// 45,
        //    //AddNewIndicator("Swing Index");//// 46,
        //    //AddNewIndicator("Accumulative Swing Index");//// 47,
        //    //AddNewIndicator("Comparative Relative Strength");//// 48,
        //    //AddNewIndicator("Mass Index");//// 49,
        //    //AddNewIndicator("Money Flow Index");//// 50,
        //    ////AddNewIndicator("Negative Volume Index");//// 51,
        //    ////AddNewIndicator("On Balance Volume");//// 52,
        //    ////AddNewIndicator("Positive Volume Index");//// 53,
        //    //AddNewIndicator("Relative Strength Index");//// 54,
        //    //AddNewIndicator("Trade Volume Index");//// 55,
        //    //AddNewIndicator("Stochastic Oscillator");//// 56,
        //    //AddNewIndicator("Stochastic Momentum Index");//// 57,
        //    //AddNewIndicator("Fractal Chaos Oscillator");//// 58,
        //    //AddNewIndicator("Prime Number Oscillator");//// 59,
        //    //AddNewIndicator("Prime Number Bands");//// 60,
        //    //AddNewIndicator("Historical Volatility");//// 61,
        //    //AddNewIndicator("MACD Histogram");//// 62,
        //    ////AddNewIndicator("Ichimoku");//// 63,
        //    ////AddNewIndicator("Elder Ray Bull Power");//// 64,
        //    ////AddNewIndicator("Elder Ray Bear Power");//// 65,
        //    ////AddNewIndicator("Ehler's Fisher Transform");//// 66,
        //    ////AddNewIndicator("Elder Force Index");//// 67,
        //    ////AddNewIndicator("Elder Thermometer");//// 68,
        //    ////AddNewIndicator("Keltner Channel");//// 69,
        //    ////AddNewIndicator("Stoller Average Range Channels");//// 70,
        //    ////AddNewIndicator("Market Facilitation Index");//// 71,
        //    ////AddNewIndicator("Schaff Trend Cycle");//// 72,
        //    ////AddNewIndicator("QStick");//// 73,
        //    //AddNewIndicator("Center Of Gravity");//// 74,
        //    //AddNewIndicator("Coppock Curve");//// 75,
        //    //AddNewIndicator("Chande Forecast Oscillator");//// 76,
        //    ////AddNewIndicator("Gopalakrishnan Range Index");//// 77,
        //    //// AddNewIndicator("Intraday Momentum Index");//// 78,
        //    //// AddNewIndicator("Klinger Volume Oscillator");//// 79,
        //    ////AddNewIndicator("Pretty Good Oscillator");//// 80,
        //    //AddNewIndicator("RAVI");//// 81,
        //    ////AddNewIndicator("Random Walk Index");//// 82,
        //    //// AddNewIndicator("Twiggs Money Flow");//// 83,
        //    ////AddNewIndicator("Custom Indicator");//// 84,
        //    #endregion

        //    //=====For ctlChart

        //    AddNewIndicator("Simple Moving Average");
        //    AddNewIndicator("Exponential Moving Average");
        //    AddNewIndicator("Time Series Moving Average");
        //    AddNewIndicator("Triangular Moving Average");
        //    AddNewIndicator("Variable Moving Average");
        //    AddNewIndicator("VIDYA");
        //    AddNewIndicator("Welles Wilder Smoothing");
        //    AddNewIndicator("Weighted Moving Average");
        //    AddNewIndicator("Williams Pct R");
        //    AddNewIndicator("Williams Accumulation Distribution");
        //    AddNewIndicator("Volume Oscillator");
        //    AddNewIndicator("Vertical Horizontal Filter");
        //    AddNewIndicator("Ultimate Oscillator");
        //    AddNewIndicator("True Range");
        //    AddNewIndicator("TRIX");
        //    AddNewIndicator("Rainbow Oscillator");
        //    AddNewIndicator("Price Oscillator");
        //    AddNewIndicator("Parabolic SAR");
        //    AddNewIndicator("Momentum Oscillator");
        //    AddNewIndicator("MACD");
        //    AddNewIndicator("Ease Of Movement");
        //    AddNewIndicator("Directional Movement System");
        //    AddNewIndicator("Detrended Price Oscillator");
        //    AddNewIndicator("Chande Momentum Oscillator");
        //    AddNewIndicator("Chaikin Volatility");
        //    AddNewIndicator("Aroon");
        //    AddNewIndicator("Aroon Oscillator");
        //    AddNewIndicator("Linear Regression Required");
        //    AddNewIndicator("Linear Regressin Forecast");
        //    AddNewIndicator("Linear Regression Slope");
        //    AddNewIndicator("Linear Regression Intercept");
        //    AddNewIndicator("Price Volume Trend");
        //    AddNewIndicator("Performance Index");
        //    AddNewIndicator("Commodity Channel Index");
        //    AddNewIndicator("Chaikin Money Flow");
        //    AddNewIndicator("Weighted Close");
        //    AddNewIndicator("Volume ROC");
        //    AddNewIndicator("Typical Price");
        //    AddNewIndicator("Standard Deviation");
        //    AddNewIndicator("Price ROC");
        //    AddNewIndicator("Median");
        //    AddNewIndicator("High Minus Low");
        //    AddNewIndicator("Bollinger Bands");
        //    AddNewIndicator("Fractal Chaos Bands");
        //    AddNewIndicator("High Low Bands");
        //    AddNewIndicator("Moving Average Envelope");
        //    AddNewIndicator("Swing Index");
        //    AddNewIndicator("Accumulative Swing Index");
        //    AddNewIndicator("Comperative RSI");
        //    AddNewIndicator("Mass Index");
        //    AddNewIndicator("Money Flow Index");
        //    AddNewIndicator("Negative Volume Index");
        //    AddNewIndicator("On Balance Volume");
        //    AddNewIndicator("Positive Volume Index");
        //    AddNewIndicator("Relative Strength Index");
        //    AddNewIndicator("Trade Volume Index");
        //    AddNewIndicator("Stochastic Oscillator");
        //    AddNewIndicator("Stochastic Momentum Index");
        //    AddNewIndicator("Fractal Chaos Oscillator");
        //    AddNewIndicator("Prime Number Oscillator");
        //    AddNewIndicator("Prime Number Bands");
        //    AddNewIndicator("Historical Volatility");
        //    AddNewIndicator("MACD Histogram");
        //    AddNewIndicator("Custom Indicator");
        //    AddNewIndicator("Last Indicator");

        //    indicatorListValues.Sort();
        //}

        //private static void AddNewIndicator(string p)
        //{
        //    indicatorListValues.Add(p);
        //    //indicatorIndexForStockChart.Add(p, _index++);
        //}

        public static double GetZeroLevelBidPrice(string symbol)
        {
            if (DDLeftZLevel.Keys.Contains(symbol))
                return DDLeftZLevel[symbol];
            else
                return 0;
        }

        public static double GetZeroLevelAskPrice(string symbol)
        {
            if (DDRightZLevel.Keys.Contains(symbol))
                return DDRightZLevel[symbol];
            else
                return 0;
        }

        public static int GetClientOrderID()
        {
            return ++_clientOrderId;
        }

        public static void SetColumnSortMode(this NDataGridView dataGridView, DataGridViewColumnSortMode sortMode)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = sortMode;
            }
        }
        public static DateTime GetDateTimeDT(string datetime)
        {
            //yyyy-mm-DD-hh-mm-ss
            DateTime dtx = DateTime.MinValue;
            if (datetime != string.Empty)
            {
                string[] x = datetime.Split('-');
                //if (x.Length == 6)
                {
                    string dat = x[1].ToString() + "/" + x[2].ToString() + "/" + x[0].ToString() + " " + x[3].ToString() + ":" + x[4].ToString() + ":" + x[5].ToString();
                    //DateTime dtx = DateTime.MinValue;
                    DateTime.TryParse(dat, out dtx);
                    //string date = string.Empty;
                    //if (dtx != DateTime.MinValue)
                    //{
                    //    date = string.Format(
                    //                Properties.Settings.Default.TimeFormat.Contains("24")
                    //                    ? "{0:dd/MM/yyyy HH:mm:ss}"
                    //                    : "{0:dd/MM/yyyy hh:mm:ss tt}", dtx);
                    //}
                    return dtx;
                }
                //else
                //{
                //    DateTime.TryParse(datetime, out dtx);

                //    return dtx;
                //}
            }
            else
                return dtx;
        }
        //public static List<string> GetIndicators()
        //{
        //    return indicatorListValues;
        //}

        //internal static AlertScripts[] GetAlertScripts()
        //{
        //    return AlertScripts;
        //}

        //BOChanges
        //private static List<PALSA.Cls.BarDataNew> _lstCandleStick = new List<Cls.BarDataNew>();
        //internal static BarDataNew[] GetHistory(string Symb, Periodicity periodicity, int intrvl, int Bars)
        //{
        //    _lstCandleStick.Clear();
        //    ClientDLL_Model.Cls.PeriodEnum prd = GetPeriodEnum(periodicity);
        //    List<ClientDLL_Model.Cls.Market.OHLC> lst = new List<ClientDLL_Model.Cls.Market.OHLC>();
        //    ClientDLL_Model.Cls.Market.LstOHLC _lstOHLC = new LstOHLC();
        //    lst = PALSA.Cls.ClsTWSDataManager.INSTANCE.GetOHLC(DateTime.UtcNow.ToString(), Symb, intrvl, Bars.ToString(), (int)prd);

        //    foreach (var item in lst)
        //    {
        //        PALSA.Cls.BarDataNew bar = new Cls.BarDataNew();
        //        bar.OpenPrice = item._Open;
        //        bar.HighPrice = item._High;
        //        bar.LowPrice = item._Low;
        //        bar.ClosePrice = item._Close;
        //        bar.Volume = item._Volume;
        //        bar.StartDateTime = Cls.ClsGlobal.GetDateTimeDT(item._OHLCTime).AddMinutes(-intrvl);
        //        bar.CloseDateTime = Cls.ClsGlobal.GetDateTimeDT(item._OHLCTime);
        //        bar.TradeDateTime = Cls.ClsGlobal.GetDateTimeDT(item._OHLCTime);

        //        _lstCandleStick.Add(bar);
        //    }

        //    return _lstCandleStick.ToArray();
        //}

        //private static ClientDLL_Model.Cls.PeriodEnum GetPeriodEnum(Periodicity periodicity)
        //{
        //    switch (periodicity)
        //    {
        //        case Periodicity.Hourly:
        //            return ClientDLL_Model.Cls.PeriodEnum.Hour;
        //        case Periodicity.Daily:
        //            return ClientDLL_Model.Cls.PeriodEnum.Day;
        //        case Periodicity.Weekly:
        //            return ClientDLL_Model.Cls.PeriodEnum.Week;
        //        case Periodicity.Monthly:
        //            return ClientDLL_Model.Cls.PeriodEnum.Month;
        //    }
        //    return ClientDLL_Model.Cls.PeriodEnum.Minute;
        //}

        public static Dictionary<string, ClsPortfolio> LatestPortfolio { get; set; }

        public static string CurrentPortfolio { get; set; }
    }
}