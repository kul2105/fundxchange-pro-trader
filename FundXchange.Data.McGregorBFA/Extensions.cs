using System;
namespace FundXchange.Data.McGregorBFA
{
    public static class Extensions
    {
        public static DateTime AsProtoDateTime(this string dateString)
        {
            if (!String.IsNullOrEmpty(dateString) && dateString.Length > 12)
            {
                int year = Convert.ToInt32(dateString.Substring(0, 4));
                int month = Convert.ToInt32(dateString.Substring(4, 2));
                int day = Convert.ToInt32(dateString.Substring(6, 2));
                int hour = Convert.ToInt32(dateString.Substring(8, 2));
                int minute = Convert.ToInt32(dateString.Substring(10, 2));
                int second = Convert.ToInt32(dateString.Substring(12, 2));

                DateTime date = new DateTime(year, month, day, hour, minute, second);
                return date;
            }
            return DateTime.MinValue;
        }
    }
}
