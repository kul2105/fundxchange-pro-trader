using System;

namespace FundXchange.Model.ViewModels.MarketDepth
{
    public class VolumeAndCount
    {
        public int Count = 0;
        public long Size = 0;
        public double Price = 0;

        public VolumeAndCount()
        {

        }

        public VolumeAndCount(double aPrice, long aSize)
        {
            Size = aSize;
            Count = 1;
            Price = aPrice;
        }
    }
}
