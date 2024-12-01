using System;
using System.Drawing;
using FundXchange.Domain.Entities;

namespace FundXchange.Application.Heatmap
{
    public static class HeatmapService
    {
        public static void RecalculateHeatMapItemsPosition(HeatMap heatMap, float x, float y)
        {
            if (heatMap.HeatMapItems.Count > 0 && heatMap.ChildHeatMaps.Count == 0)
            {
                float xOffset = x,
                        yOffset = y;
                foreach (HeatMapItem item in heatMap.HeatMapItems)
                {
                    HeatMapItemData data = (HeatMapItemData)item.Data;
                    data.ItemRectangle = new RectangleF(xOffset, yOffset, item.ViewSize.Width - 1, item.ViewSize.Height - 1);
                    if (heatMap.IsHorizontalOriented)
                        xOffset += item.ViewSize.Width;
                    else
                        yOffset += item.ViewSize.Height;
                }
            }
            else
            {
                // Draw child heat maps
                foreach (HeatMap hmap in heatMap.ChildHeatMaps)
                {
                    RecalculateHeatMapItemsPosition(hmap, x, y);
                    if (hmap.IsHorizontalOriented)
                        y += hmap.ViewSize.Height;
                    else
                        x += hmap.ViewSize.Width;
                }
            }
        }

        public static float GetColorWeightForInstrument(Instrument instrument, float colorWeightParam)
        {
            float colorWeight = colorWeightParam;
            if (colorWeight < 50)
            {
                if (Math.Ceiling(instrument.PercentageMoved / 10) % 2 == 0)
                {
                    colorWeight = (float)Math.Ceiling(instrument.PercentageMoved / 10) + 1;
                }
                else
                {
                    colorWeight = (float)Math.Ceiling(instrument.PercentageMoved / 10);
                }
            }
            if (colorWeight >= 50)
            {
                if (colorWeight > 90 && colorWeight <= 100)
                    colorWeight = 100.0F;
                if (colorWeight > 60 && colorWeight <= 70)
                    colorWeight = 99.0F;
                if (colorWeight > 70 && colorWeight <= 80)
                    colorWeight = 98.0F;
                if (colorWeight >= 50 && colorWeight <= 60)
                    colorWeight = 97.0F;
                if (colorWeight > 80 && colorWeight <= 90)
                    colorWeight = 96.0F;
            }
            return colorWeight;
        }
    }
}
