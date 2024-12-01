using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FundXchange.Morningstar.Enumerations
{
    public enum FieldValueTypes
    {
        TF_VAL_TYPE_NULL = 0,     // undefined type
        TF_VAL_TYPE_CHAR = 1,     // signed character
        TF_VAL_TYPE_BYTE = 2,     // unsigned character
        TF_VAL_TYPE_SHORT = 3,     // signed short (2 bytes)
        TF_VAL_TYPE_WORD = 4,    // unsigned short (2 bytes)
        TF_VAL_TYPE_INT = 5,     // signed integer (4 bytes)
        TF_VAL_TYPE_UINT = 6,    // unsigned integer (4 bytes)
        TF_VAL_TYPE_LONG = 7,   // signed long
        TF_VAL_TYPE_ULONG = 8,   // unsigned long
        TF_VAL_TYPE_FLOAT = 9,   // float
        TF_VAL_TYPE_DOUBLE = 10,   // double
        TF_VAL_TYPE_LPSTR = 11,   // pointer to zero terminated string
        TF_VAL_TYPE_FRAC = 12,   // EAG fractional representation (see below)
        TF_VAL_TYPE_BINARY = 13,   // binary data block
        TF_VAL_TYPE_JULIAN = 14,   // julian date (use TApiConvJulianDate())
        TF_VAL_TYPE_SNAPPER_TIME = 20     // Time - MS nibble is time subtype, rest is system time since midnight in subtype units
    }
}
