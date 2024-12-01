using System;

namespace FundXchange.Model.Extensions
{
    public static class NumberExtensions
    {
        public static double ToPercentage(this double value)
        {
            return Math.Round(value, 2);
        }

        public static decimal ToPercentage(this decimal value)
        {
            return Math.Round(value, 2);
        }
    }
}
