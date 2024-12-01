///////REVISION HISTORY/////////////////////////////////////////////////////////////////////////////////////////////////////
//DATE			INITIALS	DESCRIPTION	
//20/01/2012	VP		    1.Added members in CommandIDs enum related to Theming
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
namespace PALSA.Cls
{
    public enum CommandIDS
    {
        LOGIN = 0,
        LOGOFF,
        LOAD_WORKSPACE,
        SAVE_WORKSPACE,
        CHANGE_PASSWORD,
        EXIT,
        TICKER,
        TRADE,
        NET_POSITION,
        MESSAGE_LOG,
        CONTRACT_INFORMATION,
        TOOL_BAR,
        FILTER_BAR,
        MESSAGE_BAR,
        STATUS_BAR,
        TOP_STATUS_BAR,
        MIDDLE_STATUS_BAR,
        BOTTOM_STATUS_BAR,
        ADMIN_MESSAGE_BAR,
        INDICES_VIEW,
        FULL_SCREEN,
        MARKET_WATCH,
        MARKET_PICTURE,
        SNAP_QUOTE,
        MARKET_STATUS,
        TOP_GAINERS_LOSERS,
        PLACE_BUY_ORDER,
        PLACE_SELL_ORDER,
        PLACE_ORDER,
        ORDER_BOOK,
        TRADES,
        CUSTOMIZE,
        LOCK_WORKSTATION,
        PORTFOLIO,
        PREFERENCES,
        NEW_WINDOW,
        CLOSE,
        CLOSE_ALL,
        CASCADE,
        TILE_HORIZONTALLY,
        TILE_VERTICALLY,
        WINDOW,
        HELP,
        LANGUAGES,
        ONLINE_BACKUP,
        PRINT,
        MODIFY_ORDER,
        CANCEL_SELECTED_ORDER,
        CANCEL_ALL_ORDERS,
        ENGLISH,
        HINDI,
        MAC_OS,
        OFFICE_2007_BLACk,
        OFFICE_2007_BLUE,
        ORANGE,
        VISTA,
        VISTA_ROYAL,
        INSPIRANT,
        VISTA_SLATE,
        SIMPLE,
        OPUS_ALPHA,
        OFFICE_2007_AQUA,
        VISTA_PLUS,
        PARTICIPANT_LIST,
        INDEX_BAR,
        FILTER,
        MODIFY_TRDES,
        MOST_ACTIVE_PRODCUTS,
        NEW_CHART,
        MARKET_QUOTE,
        ACCOUNTS_TO_TRADE,
        ROYAL,
        AQUA,
        MOONLIGHT,
        WOOD,
        GREEN,
        //===================Chart=============
        PERIODICITY_1_MINUTE,
        PERIODICITY_5_MINUTE,
        PERIODICITY_15_MINUTE,
        PERIODICITY_30_MINUTE,
        PERIODICITY_1_HOUR,
        PERIODICITY_4_HOUR,
        PERIODICITY_DAILY,
        PERIODICITY_WEEKLY,
        PERIODICITY_MONTHLY,
        CHARTTYPE_BAR_CHART,
        CHARTTYPE_CANDLE_CHART,
        CHARTTYPE_LINE_CHART,
        PRICETYPE_POINT_AND_FIGURE,
        PRICETYPE_RENKO,
        PRICETYPE_KAGI,
        PRICETYPE_THREE_LINE_BREAK,
        PRICETYPE_EQUI_VOLUME,
        PRICETYPE_EQUI_VOLUME_SHADOW,
        PRICETYPE_CANDLE_VOLUME,
        PRICETYPE_HEIKIN_ASHI,
        PRICETYPE_STANDARD_CHART,
        ZOOM_IN,
        ZOOM_OUT,
        TRACK_CURSOR,
        VOLUME,
        GRID,
        CHART_3D_STYLE,
        SNAPSHOT_PRINT,
        SNAPSHOT_SAVE,
        INDICATOR_LIST,
        HORIZONTAL_LINE,
        VERTICAL_LINE,
        TEXT,
        TREND_LINE,
        ELLIPSE,
        SPEED_LINES,
        GANN_FAN,
        FIBONACCI_ARC,
        FIBONACCI_RETRACEMENT,
        FIBONACCI_FAN,
        FIBONACCI_TIMEZONE,
        TIRONE_LEVEL,
        QUADRENT_LINES,
        RAFF_REGRESSION,
        ERROR_CHANNEL,
        RECTANGLE,
        FREE_HAND_DRAWING,
        //===================Chart=============
        SURVEILLANCE,
        ABOUTUS,
        SCANNER,
        CREATE_DEMO_ACCOUNT,
        PENDING_ORDERS,
        RADAR,
        MARKET_DEPTH,
        MATRIX,
        TIME_AND_SALES
    }

    public enum MessageLogType
    {
        ALL,
        DEBUG,
        INFO,
        CRITICAL
    }

    public enum MessageType
    {
        ORDER,
        TRADE,
        MARKET,
        SYSTEM,
        MEMBER,
        ALERT,
        ADMIN,
        SURVEILANCE,
        NEWS,
        OTHER
    }

    public enum Periodicity
    {
        Unspecified = 0,
        Secondly = 1,
        Minutely = 2,
        Hourly = 3,
        Daily = 4,
        Weekly = 5,
        Monthly = 6,
    }

    public enum PeriodEnum : int
    {
        Minute = 1,
        Hour = 60,
        Day = 480,
        Week = 2400,
        Month = 9600,
        Year = 115200
    }
    public enum ePeriodicity : int
    {
        Minutely_1 = 0,
        Minutely_5 = 1,
        Minutely_15 = 2,
        Minutely_30 = 3,
        Hourly_1 = 4,
        Hourly_4 = 5,
        Daily = 6,
        Weekly = 7,
        Monthly = 8, 
        Secondly = 9
    }

    public class OHLC
    {
        public double _Close;
        public double _High;
        public double _Low;
        public DateTime _OHLCTime;
        public double _Open;
        public long _Volume;
    }

    public enum NewHistoryType
    {
        YEAR = 0,
        QUARTER = 1,
        MONTH = 2,
        DAY = 3,
        WEEK = 4,
        HOUR = 5,
        MINUTE = 6,
        SECOND = 7,
        TICK = 8,
        VOLUME = 9
    }

    public enum ChartType
    {
        BAR,
        CANDLE,
        LINE
    }

    public enum PriceType
    {
        POINT_AND_FIGURE,
        RENKO,
        KAGI,
        THREE_LINE_BREAK,
        EQUI_VOLUME,
        EQUI_VOLUME_SHADOW,
        CANDLE_VOLUME,
        HEIKIN_ASHI,
        STANDARD_CHART,
    }
}