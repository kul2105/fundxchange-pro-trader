namespace FundXchange.Morningstar.Enumerations
{
    public enum Orders
    {
        TF_FIELD_AMD_RESET         =    0, // clear complete order book side (used only for LSE order book messages)
        TF_FIELD_AMD_DELETE        =    1, /* delete static data for the symbol */
        TF_FIELD_AMD_MODIFY        =    2, /* modify static data for the symbol, add it if it does not exist */
        TF_FIELD_AMD_ADD           =    3, /* add static data for the symbol, modify if symbol already exists */
        TF_FIELD_AMD_UNCHG         =    4, /* static data for the symbol unchanged */
    }
}
