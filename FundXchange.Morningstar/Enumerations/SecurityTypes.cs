namespace FundXchange.Morningstar.Enumerations
{
    public enum SecurityTypes
    {
        TF_SEC_TYPE_UNDEF = 0,          // undefined security type
        TF_SEC_TYPE_STOCK = 1,          // stocks
        TF_SEC_TYPE_OPTION = 2,         // options
        TF_SEC_TYPE_FUTURE = 3,         // commodities/futures
        TF_SEC_TYPE_FUT_OPT = 4,        // commodity options
        TF_SEC_TYPE_SPOT = 5,           // spots
        TF_SEC_TYPE_SPECS = 6,          // contract specs (was reserved)
        TF_SEC_TYPE_CORP_BOND = 7,      // corporate bonds
        TF_SEC_TYPE_MUT_FUND = 8,       // mutual funds
        TF_SEC_TYPE_GOV_BOND = 9,       // government bonds
        TF_SEC_TYPE_INDEX = 10,         // indices
        TF_SEC_TYPE_MUN_BOND = 11,      // municipial bonds
        TF_SEC_TYPE_NEWS = 12,          // news
        TF_SEC_TYPE_SPREAD = 13,        // spreads (since 19.12.05)
        TF_SEC_TYPE_RES2 = 13,          // reserved
        TF_SEC_TYPE_STAT = 14,          // statistic symbols
        TF_SEC_TYPE_MON_FUND = 15,      // monetary funds
        TF_SEC_TYPE_UNSP_BOND = 16,     // unspecified bonds
        TF_SEC_TYPE_UNSP_FUND = 17,     // unspecified funds
        TF_SEC_TYPE_MISC = 18,          // miscelaneous securities
        TF_SEC_TYPE_MON_MKT = 19,       // money market
        TF_SEC_TYPE_RES3 = 19,          // for compatibility
        TF_SEC_TYPE_FOREX = 20,         // forex symbols
    }
}
