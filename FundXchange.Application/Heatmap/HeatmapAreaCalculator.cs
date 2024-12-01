using System;
using System.Drawing;

namespace FundXchange.Application.Heatmap
{
    internal class HeatmapAreaCalculator
    {
        public float TotalAreaWeight { get; private set; }
        public float MinColorWeight { get; private set; }
        public float MaxColorWeight { get; private set; }
        public HeatMapItemCollection HeatmapItems { get; private set; }

        private static Color MIN_COLOR = Color.Red;
        private static Color MAX_COLOR = Color.Green;

        public HeatmapAreaCalculator(HeatMapItemCollection heatmapCollection, SizeF viewSize)
        {
            HeatmapItems = heatmapCollection;
            CalculateAreaWeight();
            CalculateHeatmapItemArea(viewSize);
        }

        private void CalculateAreaWeight()
        {
            // Calculate total area weight
            float totalAreaWeight = HeatmapItems[0].AreaWeight;
            float minColorWeight = HeatmapItems[0].ColorWeight;
            float maxColorWeight = HeatmapItems[0].ColorWeight;

            for (int i = 1; i < HeatmapItems.Count; i++)
            {
                totalAreaWeight += HeatmapItems[i].AreaWeight;
                if (minColorWeight > HeatmapItems[i].ColorWeight)
                    minColorWeight = HeatmapItems[i].ColorWeight;
                if (maxColorWeight < HeatmapItems[i].ColorWeight)
                    maxColorWeight = HeatmapItems[i].ColorWeight;
            }

            if (totalAreaWeight == 0)
                totalAreaWeight = 1;

            TotalAreaWeight = totalAreaWeight;
            MinColorWeight = minColorWeight;
            MaxColorWeight = maxColorWeight;
        }

        private void CalculateHeatmapItemArea(SizeF viewSize)
        {
            // Calculate area of each heat map item
            float heatMapArea = viewSize.Width * viewSize.Height;
            int minColorValue = ColorToRGB(MIN_COLOR);
            int maxColorValue = ColorToRGB(MAX_COLOR);
            if (minColorValue > maxColorValue)
            {
                int tmp = Math.Min(minColorValue, maxColorValue);
                maxColorValue = Math.Max(minColorValue, maxColorValue);
                minColorValue = tmp;
            }

            foreach (HeatMapItem item in HeatmapItems)
            {
                item.Area = heatMapArea * item.AreaWeight / TotalAreaWeight;
                float colorCoef = GetColorCoeficient(item);
                int argb = CalculateArgb(minColorValue, maxColorValue, colorCoef);
                item.Background = new SolidBrush(Color.FromArgb(argb));
            }
        }

        private float GetColorCoeficient(HeatMapItem item)
        {
            float colorCoef = MaxColorWeight != MinColorWeight
                                ? (item.ColorWeight - MinColorWeight) / (MaxColorWeight - MinColorWeight)
                                : 0.0f;
            return colorCoef;
        }

        private int CalculateArgb(int minColorValue, int maxColorValue, float colorCoef)
        {
            int argb = (255 << 24) + (int)(minColorValue + (maxColorValue - minColorValue) * colorCoef);
            return argb;
        }

        private int ColorToRGB(Color color)
        {
            return (color.R << 16) + (color.G << 8) + color.B;
        }
    }
}
