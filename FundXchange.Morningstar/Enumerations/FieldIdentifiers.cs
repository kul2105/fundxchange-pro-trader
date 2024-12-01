namespace FundXchange.Morningstar
{
    public enum FieldIdentifiers
    {
        #region Standard field-types
        TF_FIELD_SYMBOL = 0,            // n symbol name
        TF_FIELD_MIN_OF_DAY = 1,        // 2 time in minutes of the day (0..1440)
        TF_FIELD_LAST = 2,              // 4 last price
        TF_FIELD_VOLUME = 3,            // 4 last volume
        TF_FIELD_BID = 4,               // 4 bid price
        TF_FIELD_BID_SIZE = 5,          // 4 bid size
        TF_FIELD_ASK = 6,               // 4 ask price
        TF_FIELD_ASK_SIZE = 7,          // 4 ask size
        TF_FIELD_BID_MARKET = 8,        // 1 bid market
        TF_FIELD_BID_TIME = 9,          // 2 best bid time (in minutes of the day)
        TF_FIELD_BEST_BID = 10,         // 4 best bid price
        TF_FIELD_BEST_BID_SIZE = 11,    // 4 best bid size
        TF_FIELD_ASK_MARKET = 12,       // 1 ask market
        TF_FIELD_ASK_TIME = 13,         // 2 best ask time (in minutes of the day)
        TF_FIELD_BEST_ASK = 14,         // 4 best ask price
        TF_FIELD_BEST_ASK_SIZE = 15,    // 4 best ask size
        TF_FIELD_CUM_VOLUME = 16,       // 4 cumulative volume
        TF_FIELD_OPEN = 17,             // 4 open price
        TF_FIELD_HIGH = 18,             // 4 high price
        TF_FIELD_LOW = 19,      // n ISIN code
        TF_FIELD_CLOSE = 20,            // 4 close price
        TF_FIELD_OP_INT = 21,           // 4 open interest
        TF_FIELD_TIME_DAY = 22,         // 1 system time - day
        TF_FIELD_TIME_MONTH = 23,       // 1 system time - month
        TF_FIELD_TIME_YEAR = 24,        // 1 system time - year
        TF_FIELD_TIME_HOURS = 25,       // 1 system time - hour
        TF_FIELD_TIME_MINUTES = 26,     // 1 system time - minute
        TF_FIELD_TIME_SECONDS = 27,     // 1 system time - second

        TF_FIELD_IMPLIED_BID = 128,    /* 4 implied bid */
        TF_FIELD_IMPLIED_BID_SIZE = 129,    /* 4 implied bid size */
        TF_FIELD_IMPLIED_BID_IND = 130,    /* n implied bid indicator */
        TF_FIELD_IMPLIED_ASK = 131,   /* 4 implied ask */
        TF_FIELD_IMPLIED_ASK_SIZE = 132,   /* 4 implied ask size */
        TF_FIELD_IMPLIED_ASK_IND = 133,   /* n implied ask indicator */

        TF_FIELD_TRADE_COUNT = 30,     // 4 # of trades since market opening
        TF_FIELD_FFM_SPOT = 31,   // 4 German Spot price (Kassakurs)

        TF_FIELD_ISIN     =          245,     // 0x00F5    ISIN for non-static messages (independent of TF_FIELD_STATIC_ISIN)
        #endregion

        #region Field definitions for error messages from the TIC card
        TF_FIELD_ERROR_CODE = 28,       // 4 TIC error code
        TF_FIELD_ERROR_TEXT = 29,       // n TIC error text
        #endregion

        #region Field-types for options, futures and future options
        TF_FIELD_PREV_VOLUME = 32,    /* 4 previous days volume */
        TF_FIELD_EST_VOLUME = 33,   /* 4 estimated volume (= cumulative volume for futures) */
        TF_FIELD_SETTL = 34,   /* 4 settlement price */
        TF_FIELD_PREV_SETTL = 35,   /* 4 previous days settlement */
        TF_FIELD_OP_RANGE1 = 36,   /* 4 opening range 1 */
        TF_FIELD_OP_RANGE2 = 37,   /* 4 opening range 2 */
        TF_FIELD_CL_RANGE1 = 38,   /* 4 closing range 1 */
        TF_FIELD_CL_RANGE2 = 39,   /* 4 closing range 2 */
        TF_FIELD_SP_RANGE1 = 40,   /* 4 special range 1 */
        TF_FIELD_SP_RANGE2 = 41,  /* 4 special range 1 */
        TF_FIELD_LIMIT_HIGH = 42,  /* 4 limit high */
        TF_FIELD_LIMIT_LOW = 43,  /* 4 limit low */
        TF_FIELD_OVERNIGHT_FLAG  =   244,     // 0x00F4    Future overnight flag (indicate that msg belongs to next day)
        #endregion

        #region LME field-types
        TF_FIELD_LME_CF_MONTH = 44,  /* 1 LME cash/forward month */
        TF_FIELD_LME_SPRD_MONTH = 45,   /* 1 LME spread month */
        TF_FIELD_LME_PR_QUAL = 46,  /* 1 LME price qualifier */
        TF_FIELD_LME_DEALER = 47,  /* n LME dealer */
        TF_FIELD_LME_CF_DAY = 76,    /* LME cash/forward day */
        TF_FIELD_LME_CF_YEAR = 77,  /* LME cash/forward year */
        TF_FIELD_LME_SPRD_DAY = 78,  /* LME spread month day */
        TF_FIELD_LME_SPRD_YEAR = 79,   /* LME spread month year */
        TF_FIELD_LME_PRICE = 80,   /* LME price value */
        TF_FIELD_LME_BATE_CODE = 81,   /* LME bate code */
        TF_FIELD_LME_CONTANGO = 82,  /* LME contango (to indicate neg. prices) */
        TF_FIELD_LME_VOLUME = 83, /* LME volume */
TF_FIELD_LME_DMAS    =       246 ,    // 0x00F6    LME, Daily moving average settlement (TAPO)
TF_FIELD_LME_NAVG    =       247 ,    // 0x00F7    LME, Notional Average (TAPO)
TF_FIELD_LME_VOL     =       248 ,    // 0x00F8    LME, Volatility (TAPO)
        #endregion

        #region Ticker Plant Frankfurt field-types
        TF_FIELD_TPF_CQUAL = 48,  /* 1 close qualifier (for IBIS(213): broker, for FFM(200): qualifier) */
        TF_FIELD_TPF_SQUAL = 49,  /* 1 spot qualifier (for IBIS(213): broker, for FFM(200): qualifier) */
        TF_FIELD_TPF_LQUAL = 50,  /* 1 last qualifier (for IBIS(213): broker, for FFM(200): qualifier) */
        TF_FIELD_TPF_BQUAL = 51,  /* 1 bid qualifier (for IBIS(213): broker, for FFM(200): qualifier) */
        TF_FIELD_TPF_AQUAL = 52,  /* 1 ask qualifier (for IBIS(213): broker, for FFM(200): qualifier) */
        TF_FIELD_TPF_SPOT = 53,  /* 4 German Spot price (Kassakurs) */
        #endregion

        #region Swisse Market Feed field-types
        TF_FIELD_SMF_FORWARD = 54,  /* 4 not in use */
        TF_FIELD_SMF_FWD_MONTH = 55,  /* 1 not in use */
        TF_FIELD_SMF_PREMIUM = 56,  /* 4 not in use */
        #endregion

        #region LSE field-types
        TF_FIELD_MID_PRICE = 57,    /* LSE mid price */
        TF_FIELD_CMP_HIGH = 58,    /* 0x3a 4 composite high price */
        TF_FIELD_CMP_LOW = 59,    /* 0x3b 4 composite low price */
        #endregion

        #region Statistic messages
        TF_FIELD_STAT_FACTOR = 84,    /* 4 multiplicator for the following values */
        TF_FIELD_STAT_VALUE = 85,    /* 4 value */
        #endregion

        #region LIFFE/LTOM AutoQuote field-types
        TF_FIELD_AUTO_BID = 86,    /* 1 autoquote bid */
        TF_FIELD_AUTO_ASK = 87,    /* 1 autoquote ask */
        #endregion

        #region FOREX field-types
        TF_FIELD_FRX_BANK = 88,    /* n bank name */
        TF_FIELD_FRX_CITY = 89,   /* n city */
        TF_FIELD_FRX_BID = 90,   /* 4 forex bid */
        TF_FIELD_FRX_ASK = 91,  /* 4 forex ask */
        TF_FIELD_FRX_QUAL = 92,   /* 1 quote qualifier (London am, pm, ...) */
        #endregion

        #region NASDAQ field-types
        TF_FIELD_FOOTNOTE = 93,     /* n footnote */
        TF_FIELD_AVG_MAT = 94,   /* 4 average maturity */
        TF_FIELD_7DY_YIELD = 95,    /* 4 7 days yield */
        TF_FIELD_EFF_7DY_YIELD = 96,    /* 4 effective 7 days yield */
        TF_FIELD_ASSET = 97,   /* 4 asset */
        TF_FIELD_NAV = 98,   /* 4 net asset value */
        TF_FIELD_OFFER = 99,    /* 4 offer */
        TF_FIELD_CAP_GAIN = 100,   /* 4 capital gain */
        TF_FIELD_DIVIDEND = 101,    /* 4 dividend */
        TF_FIELD_SPL_DIV = 102,   /* 4 spilt dividend */
        TF_FIELD_NET_NAV = 103,   /* 4 NAV net change */
        TF_FIELD_PREV_NAV = 104,   /* 4 previous days NAV */
        TF_FIELD_OPEN_NAV = 105,   /* 4 beginning NAV */
        TF_FIELD_CLOSE_NAV = 106,   /* 4 closing NAV */
        #endregion

        #region Field-types to force change/change(%) recalculation
        TF_FIELD_RECALC_CHANGE = 107,    /* 0x6b 1 recalculate net change fields () */
        TF_FIELD_TRADE_COND = 108,    /* bit flag to updated last/high/low/open/vol/composite */
        TF_FIELD_BEST_LIMITS = 109,   // best bid/ask values packet in a binary data block
        TF_FIELD_CHART_UPDATE = 110,    // chart update packet in a binary data block
        TF_FIELD_OPEN_IND = 111,    /* n open indicator flag */
        TF_FIELD_HIGH_IND = 112,   /* n high indicator flag */
        TF_FIELD_LOW_IND = 113,   /* n low indicator flag */
        TF_FIELD_BID_IND = 114,   /* n bid indicator flag */
        TF_FIELD_ASK_IND = 115,   /* n ask indicator flag */
        TF_FIELD_SUB_MARKET = 116,   /* 4 sub market */
        TF_FIELD_CHART_INTERVAL = 117,   /* 4 chart tick resolution in minutes */
        #endregion

        #region AEX field-types
        TF_FIELD_ASSET_BID = 128,    /* 4 asset bid (n.u.) */
        TF_FIELD_ASSET_BID_SIZE = 129,   /* 4 asset bid size (n.u.) */
        TF_FIELD_ASSET_BID_MEM = 130,    /* n asset bid member (n.u.) */
        TF_FIELD_ASSET_ASK = 131,   /* 4 asset ask (n.u.) */
        TF_FIELD_ASSET_ASK_SIZE = 132,  /* 4 asset ask size (n.u.) */
        TF_FIELD_ASSET_ASK_MEM = 133, /* n asset ask member (n.u.) */
        TF_FIELD_LIMBO_BID = 134,/* 4 limbo bid */
        TF_FIELD_LIMBO_BID_SIZE = 135,  /* 4 limbo bid size */
        TF_FIELD_LIMBO_ASK = 136, /* 4 limbo ask */
        TF_FIELD_LIMBO_ASK_SIZE = 137,/* 4 limbo ask size */
        TF_FIELD_TRADE_IND = 138,/* n trade indicator flag */
        TF_FIELD_EXDIV = 139,/* 1 ex divident flag */
        TF_FIELD_THPRICE = 140,/* 4 theoretical price */

        TF_FIELD_WVOL = 141,     /* 0x008d  4 wholesale volume */
        TF_FIELD_WTRADE_COUNT = 142,    /* 0x008e  4 # of wholesale trades */
        TF_FIELD_RVOL = 143,   /* 0x008f  4 retail volume */
        TF_FIELD_RTRADE_COUNT = 144,  /* 0x0090  4 # of retail trades */

        TF_FIELD_BOOK_BID = 145,     /* 0x0091  4 AEO book bid */
        TF_FIELD_BOOK_BID_SIZE = 146,    /* 0x0092  4 AEO book bid size */
        TF_FIELD_BOOK_ASK = 147,   /* 0x0093  4 AEO book ask */
        TF_FIELD_BOOK_ASK_SIZE = 148,  /* 0x0094  4 AEO book ask size */

        TF_FIELD_BEST_BID_COMP = 149,     /* 0x0095  1 asset bid joint flag */
        TF_FIELD_BEST_ASK_COMP = 150,     /* 0x0096  1 asset ask joint flag */
        TF_FIELD_CLOSE_IND = 151,     /* 0x0097  n close indicator flag */
        TF_FIELD_HIST_UV_PRICE = 152,     /* 0x0098  4 historlical underlining value */
        TF_FIELD_DAYSTOEXP = 153,     /* 0x0099  4 days to expiration */
        TF_FIELD_TRANS_COUNT = 154,     /* 0x009a  4 total # of transactions */
        #endregion

        #region FIM field-types
        TF_FIELD_SUSPENDEDTIME = 155,     /* 0x009b  4 053 0x0035 C 004 Suspended time */
        TF_FIELD_ASKCOMP_FIXCOURS = 156,     /* 0x009c  4 060 0x003c F 004 Compensation price  (== FNO_ASKCOMP) */
        TF_FIELD_ASKREP_FIXASK = 157,     /* 0x009d  4 061 0x003d F 004 Contango price      (== FNO_ASKREP) */
        TF_FIELD_RATEREP = 158,     /* 0x009e  4 062 0x003e T 012 Contango rate */
        TF_FIELD_INTEREST_RATE = 159,     /* 0x009f  4 063 0x003f F 004 Interest rate    (== FNO_RATEINT) */
        TF_FIELD_DEALINCVOL = 160,     /* 0x00a0  4 083 0x0053 U 004 Trade inc.vol */
        TF_FIELD_DEALCUMVOL = 161,     /* 0x00a1  4 084 0x0054 U 004 Trade cum.vol */
        TF_FIELD_SUSPENDED = 162,     /* 0x00a2  1 088 0x0058 H 001 Suspended */
        TF_FIELD_INXCHANGE3 = 163,     /* 0x00a3  4 130 0x0082 F 004 Change 1 year */
        TF_FIELD_INXCHANGE = 164,     /* 0x00a4  4 131 0x0083 F 004 Change (opening level) */
        TF_FIELD_INXSETTLEMENT = 165,     /* 0x00a5  4 132 0x0084 F 004 Change - settlement */
        TF_FIELD_INXQUOTESAVAIL = 166,     /* 0x00a6  4 133 0x0085 I 002 Quoted shares */
        TF_FIELD_BIDNO1 = 167,     /* 0x00a7  4 134 0x0086 I 002 Bid no. 1 */
        TF_FIELD_BIDNO2 = 168,     /* 0x00a8  4 135 0x0087 I 002 Bid no. 2 */
        TF_FIELD_BIDNO3 = 169,     /* 0x00a9  4 136 0x0088 I 002 Bid no. 3 */
        TF_FIELD_BIDNO4 = 170,     /* 0x00aa  4 137 0x0089 I 002 Bid no. 4 */
        TF_FIELD_BIDNO5 = 171,     /* 0x00ab  4 138 0x008a I 002 Bid no. 5 */
        TF_FIELD_BIDNO6 = 172,     /* 0x00ac  4 139 0x008b I 002 Bid no. 6 */
        TF_FIELD_BIDVOL1 = 173,     /* 0x00ad  4 140 0x008c U 004 Bid vol. 1 */
        TF_FIELD_BIDVOL2 = 174,     /* 0x00ae  4 141 0x008d U 004 Bid vol. 2 */
        TF_FIELD_BIDVOL3 = 175,     /* 0x00af  4 142 0x008e U 004 Bid vol. 3 */
        TF_FIELD_BIDVOL4 = 176,     /* 0x00b0  4 143 0x008f U 004 Bid vol. 4 */
        TF_FIELD_BIDVOL5 = 177,     /* 0x00b1  4 144 0x0090 U 004 Bid vol. 5 */
        TF_FIELD_BIDVOL6 = 178,     /* 0x00b2  4 145 0x0091 U 004 Bid vol. 6 */
        TF_FIELD_BID1 = 179,     /* 0x00b3  4 146 0x0092 F 004 Bid 1 */
        TF_FIELD_BID2 = 180,     /* 0x00b4  4 147 0x0093 F 004 Bid 2 */
        TF_FIELD_BID3 = 181,     /* 0x00b5  4 148 0x0094 F 004 Bid 3 */
        TF_FIELD_BID4 = 182,     /* 0x00b6  4 149 0x0095 F 004 Bid 4 */
        TF_FIELD_BID5 = 183,     /* 0x00b7  4 150 0x0096 F 004 Bid 5 */
        TF_FIELD_BID6 = 184,     /* 0x00b8  4 151 0x0097 F 004 Bid 6 */
        TF_FIELD_ASKNO1 = 185,     /* 0x00b9  4 152 0x0098 I 002 Ask no. 1 */
        TF_FIELD_ASKNO2 = 186,     /* 0x00ba  4 153 0x0099 I 002 Ask no. 2 */
        TF_FIELD_ASKNO3 = 187,     /* 0x00bb  4 154 0x009a I 002 Ask no. 3 */
        TF_FIELD_ASKNO4 = 188,     /* 0x00bc  4 155 0x009b I 002 Ask no. 4 */
        TF_FIELD_ASKNO5 = 189,     /* 0x00bd  4 156 0x009c I 002 Ask no. 5 */
        TF_FIELD_ASKNO6 = 190,     /* 0x00be  4 157 0x009d I 002 Ask no. 6 */
        TF_FIELD_ASKVOL1 = 191,     /* 0x00bf  4 158 0x009e U 004 Ask vol. 1 */
        TF_FIELD_ASKVOL2 = 192,     /* 0x00c0  4 159 0x009f U 004 Ask vol. 2 */
        TF_FIELD_ASKVOL3 = 193,     /* 0x00c1  4 160 0x00a0 U 004 Ask vol. 3 */
        TF_FIELD_ASKVOL4 = 194,     /* 0x00c2  4 161 0x00a1 U 004 Ask vol. 4 */
        TF_FIELD_ASKVOL5 = 195,     /* 0x00c3  4 162 0x00a2 U 004 Ask vol. 5 */
        TF_FIELD_ASKVOL6 = 196,     /* 0x00c4  4 163 0x00a3 U 004 Ask vol. 6 */
        TF_FIELD_ASK1 = 197,     /* 0x00c5  4 164 0x00a4 F 004 Ask 1 */
        TF_FIELD_ASK2 = 198,     /* 0x00c6  4 165 0x00a5 F 004 Ask 2 */
        TF_FIELD_ASK3 = 199,     /* 0x00c7  4 166 0x00a6 F 004 Ask 3 */
        TF_FIELD_ASK4 = 200,     /* 0x00c8  4 167 0x00a7 F 004 Ask 4 */
        TF_FIELD_ASK5 = 201,     /* 0x00c9  4 168 0x00a8 F 004 Ask 5 */
        TF_FIELD_ASK6 = 202,     /* 0x00ca  4 169 0x00a9 F 004 Ask 6 */
        TF_FIELD_ORIGIN = 203,     /* 0x00cb  1 206 0x00ce H 004 origin market */
        TF_FIELD_CURRENCYCODE = 204,     /* 0x00cc  4 323 0x0143 T 004 Currency Code */
        TF_FIELD_BLOCK_TRADE_TIME = 205,     /* 0x00cd  4 935 0x03a7 C 004 Block trade time */
        TF_FIELD_BLOCK_PRICE = 206,     /* 0x00ce  4 936 0x03a8 F 004 Block price */
        TF_FIELD_BLOCK_VOLUME = 207,     /* 0x00cf  4 937 0x03a9 U 004 Block volume */
        TF_FIELD_FIX_PRICE = 208,     /* 0x00d0  4 938 0x03aa F 004 Fix price */
        TF_FIELD_FIX_BID = 209,     /* 0x00d1  4 939 0x03ab F 004 Fix bid */
        TF_FIELD_INXVALUE = 210,     /* 0x00d2  4 101 0x0065 F 004 index value */
        TF_FIELD_FIX_ASK = 211,     /* 0x00d3  4 940 0x03ac F 004 Fix ask */
        TF_FIELD_FOURCHBID = 212,     /* 0x00d4  4 065 0x0041 F 004 fourchette bid */
        TF_FIELD_FOURCHASK = 213,     /* 0x00d5  4 064 0x0040 F 004 fourchette ask */
        TF_FIELD_LAST_MARKET = 214,     // 4 origination market of last trade for composite updates
        #endregion

        #region SIA field-types
        TF_FIELD_OFF_PRICE = 216,// 0xD8 = official price
        TF_FIELD_REF_PRICE = 217, // 0xD9 = reference price (== close for net change calc)
        #endregion

        #region Additional market depth field-types
        TF_FIELD_BIDNO7 = 218,     // 0x00DA    Bid no. 7
        TF_FIELD_BIDNO8 = 219,    // 0x00DB    Bid no. 8
        TF_FIELD_BIDNO9 = 220,     // 0x00DC    Bid no. 9
        TF_FIELD_BIDNO10 = 221,    // 0x00DD    Bid no. 10
        TF_FIELD_BIDVOL7 = 222,    // 0x00DE    Bid vol. 7
        TF_FIELD_BIDVOL8 = 223,    // 0x00DF    Bid vol. 8
        TF_FIELD_BIDVOL9 = 224,    // 0x00E0    Bid vol. 9
        TF_FIELD_BIDVOL10 = 225,    // 0x00E1    Bid vol. 10
        TF_FIELD_BID7 = 226,    // 0x00E2    Bid 7
        TF_FIELD_BID8 = 227,    // 0x00E3    Bid 8
        TF_FIELD_BID9 = 228,    // 0x00E4    Bid 9
        TF_FIELD_BID10 = 229,    // 0x00E5    Bid 10
        TF_FIELD_ASKNO7 = 230,    // 0x00E6    Ask no. 7
        TF_FIELD_ASKNO8 = 231,    // 0x00E7    Ask no. 8
        TF_FIELD_ASKNO9 = 232,    // 0x00E8    Ask no. 9
        TF_FIELD_ASKNO10 = 233,    // 0x00E9    Ask no. 10
        TF_FIELD_ASKVOL7 = 234,    // 0x00EA    Ask vol. 7
        TF_FIELD_ASKVOL8 = 235,    // 0x00EB    Ask vol. 8
        TF_FIELD_ASKVOL9 = 236,    // 0x00EC    Ask vol. 9
        TF_FIELD_ASKVOL10 = 237,    // 0x00ED    Ask vol. 10
        TF_FIELD_ASK7 = 238,    // 0x00EE    Ask 7
        TF_FIELD_ASK8 = 239,    // 0x00EF    Ask 8
        TF_FIELD_ASK9 = 240,    // 0x00F0    Ask 9
        TF_FIELD_ASK10 = 241,    // 0x00F1    Ask 10
        #endregion

        #region VWAP field-types
        TF_FIELD_VWAP_VOL = 242,     // 0x00F2    VWAP volume (ulong)
        TF_FIELD_VWAP_PRICE = 243, // 0x00F3 = VWAP price (frac)
        #endregion

        #region Unsorted field-types
        TF_FIELD_LSE_BID_NO = 384,  	// 384 u number of orders at best bid
        TF_FIELD_LSE_ASK_NO = 385,  	// 385 u number of orders at best ask
        TF_FIELD_LSE_MID_HIGH = 394,   	// 394 f Mid High
        TF_FIELD_LSE_MID_HIGH_TM = 395,   	// 395 u Mid High Time (min of day)
        TF_FIELD_LSE_MID_LOW = 396,   	// 396 f Mid Low
        TF_FIELD_LSE_MID_LOW_TM = 397,   	// 397 u Mid Low Time (min of day)
        TF_FIELD_LSE_UNC_SIZE = 398,   	// 398 u Uncrossing volume
        TF_FIELD_LSE_VWAP = 399,   	// 399 f VWAP from trades
        TF_FIELD_LSE_AUTO_VWAP = 400,   	// 400 f VWAP from automatic trades
        
        // Level 2 Fields
        TF_FIELD_LSE_DEPTH_AMD = 401,   	// 401 u add/mod/del flag (see TF_FIELD_AMD... definitions)
        TF_FIELD_LSE_DEPTH_SIDE = 402,		// 402 u 0=bid, 1=ask
        TF_FIELD_LSE_DEPTH_POS = 403,	   	// 403 u position
        TF_FIELD_LSE_DEPTH_PRICE = 404,	   	// 404 f price
        TF_FIELD_LSE_DEPTH_SIZE = 405,   	// 405 u size
        TF_FIELD_LSE_DEPTH_NO = 406,   	// 406 u # of quotes at given price
        TF_FIELD_LSE_DEPTH_MM = 407,   	// 407 s market maker code
        TF_FIELD_LSE_PERIOD = 408,		// 408 s trading period name
        TF_FIELD_LSE_DEPTH_ID = 409,   	// 409 u depth entry id (unique per symbol and side)
        TF_FIELD_LSE_AVOL = 410,   	// 410 u cum volume of automatic trades
        TF_FIELD_LSE_NVOL = 411,   	// 411 u cum volume of non-automatic trades
        TF_FIELD_LSE_TWAS = 412,   	// 412 f time weighted average spread
        TF_FIELD_LSE_PERIOD_LEN = 413,   	// 413 u period length of TWAS
        TF_FIELD_LSE_DEPTH_TIME = 414,		// 414 u creation timestamp of depth entry (min of day)
        TF_FIELD_LSE_DEPTH_DATE = 415,  	// 415 u creation date of depth entry (julian format)
        TF_FIELD_LSE_DEPTH_CODE = 416,   	// 416 n original order code
        TF_FIELD_LSE_CLOSE_BID = 417,  	// 417 f closing bid
        TF_FIELD_LSE_CLOSE_ASK = 418,   	// 418 f closing ask
        TF_FIELD_LSE_DEPTH_END = 419,   	// 419 u depth end marker (in refreshes)
        TF_FIELD_DOL_HIGH = 420,  	// 420 f LSE DOL high
        TF_FIELD_DOL_LOW = 421,  	// 421 f LSE DOL low
        TF_FIELD_TRADE_DATE = 438, // 438 u date of last trade activity (julian format)
        TF_FIELD_NEWS_FLAG = 439, // 439 u bit flag to copy news stories to many markets
        TF_FIELD_ORG_TRADE_TIME = 502, // 502 u original trade time from the source in ms since midnight

        CAPI_MSG_LAST_RECORD = 0x00010000,        /* identifies the last record */
        CAPI_MSG_NO_DATA = 0x00020000,       /* no records available */

        TF_FIELD_CAPI_MSG = 0x0f08, /* 3848 4 ClientApi dbase request return value */

        TF_FIELD_FIELD_ID = 0x0f09, /* 3849 4 field id */
        TF_FIELD_REQUEST_ID = 0x0f0a, /* 3850 4 request id for ServerApi */
        TF_FIELD_NEWS_NUMBER = 0x0f0b, /* 3851 4 number of a news story */
        TF_FIELD_NEWS_LINES = 0x0f0c, /* 3852 4 number of lines in the news story */
        TF_FIELD_NUM_TICKS = 0x0f0d, /* 3853 2 number of single ticks in an aggregated tick */
        TF_FIELD_TICK_LENGTH = 0x0f0e, /* 3854 4 time length of an aggregated tick in seconds */
        TF_FIELD_NEWS_KEYWORD = 0x0f0f, /* 3855 n news keyword */
        #endregion

        #region Subscription message field-types
        TF_FIELD_SUBS_ID = 0x0f20, /* 3872 2 subscription id */
        TF_FIELD_SUBS_FLAG = 0x0f21, /* 3873 2 flag: 1-enabled  0-disabled */
        TF_FIELD_SUBS_TYPE = 0x0f22, /* 3874 2 type: 1-market  2-news  3-special service 4-service(will be used in future versions of QuoteSpeed */
        TF_FIELD_SUBS_CFGNUM = 0x0f23, /* 3875 2 the number of the user cfg */
        TF_FIELD_SUBS_MAXUSERS = 0x0f24, /* 3876 2 the number of users for this subsciption */
        TF_FIELD_SUBS_SUBSID = 0x0f25, /* 3877 n the subscription id (dongle number) */
        TF_FIELD_SUBS_DELAY = 0x0f28, /* 3880 2 delay for the subscription */
        TF_FIELD_GEN_STATUS = 3888,	// 3888 2 general flags definition see below
        #endregion

        #region Trade Fields
        TF_FIELD_TRADE_COND_LAST     = 0x01, /* update last price */
        TF_FIELD_TRADE_COND_HIGHLOW  = 0x02, /* update high/low values from last price */
        TF_FIELD_TRADE_COND_CMP      = 0x04, /* update composite symbol also */
        TF_FIELD_TRADE_COND_OPEN     = 0x08, /* update open value from last price */
        TF_FIELD_TRADE_COND_VOL      = 0x10, /* update only volume */
        TF_FIELD_TRADE_COND_DEFAULT  = (TF_FIELD_TRADE_COND_LAST | TF_FIELD_TRADE_COND_HIGHLOW | TF_FIELD_TRADE_COND_VOL),
        #endregion
    }
}
