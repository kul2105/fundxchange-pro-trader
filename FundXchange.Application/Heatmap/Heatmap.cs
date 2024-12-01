using System;
using System.Drawing;

namespace FundXchange.Application.Heatmap
{
    public class HeatMap
    {
        #region Properties

        public HeatMap Parent { get; set; }
        public SizeF ViewSize { get; set; }
        public bool IsHorizontalOriented { get; set; }

        public HeatMapItemCollection HeatMapItems { get; set; }
        public HeatMapCollection ChildHeatMaps { get; private set; }

        #endregion

        #region Constructors

        public HeatMap()
        {
            ChildHeatMaps = new HeatMapCollection();
            HeatMapItems = new HeatMapItemCollection();
        }

        public HeatMap(SizeF size, HeatMap parent)
            : this()
        {
            Parent = parent;
            ViewSize = size;
            UpdateOrientationStatus();
        }

        #endregion

        public void Refresh()
        {
            ChildHeatMaps.Clear();

            if (HeatMapItems.Count == 0)
                return;

            HeatmapAreaCalculator areaWeightCalculator = new HeatmapAreaCalculator(HeatMapItems, ViewSize);
            HeatMapItems = areaWeightCalculator.HeatmapItems;
            HeatMapItems.Sort(HeatMapItem.CompareByAreaDesc);
            HeatMapItemCollection itemsToArrange = new HeatMapItemCollection(HeatMapItems);
            ArrangeItems(itemsToArrange);
        }
        
        #region Private methods

        private void UpdateOrientationStatus()
        {
            IsHorizontalOriented = ViewSize.Height > ViewSize.Width;
        }

        private void ArrangeItems(HeatMapItemCollection items)
        {
            UpdateOrientationStatus();
            HeatMap child = new HeatMap(ViewSize, this);
            ChildHeatMaps.Add(child);
            child.PlaceItems(items);
            if (items.Count > 0)
            {
                SizeF freeSpaceSize = ViewSize;
                if (child.IsHorizontalOriented)
                    freeSpaceSize.Height -= child.ViewSize.Height;
                else
                    freeSpaceSize.Width -= child.ViewSize.Width;
                HeatMap freeSpace = new HeatMap(freeSpaceSize, this);

                ChildHeatMaps.Add(freeSpace);
                freeSpace.ArrangeItems(items);
            }
        }

        private void PlaceItems(HeatMapItemCollection itemsToPlace)
        {
            double ratio = double.MaxValue;
            while (itemsToPlace.Count > 0)
            {
                HeatMapItems.Add(itemsToPlace[0]);
                UpdateLayout();
                if (ratio < HeatMapItems[0].Ratio)
                {
                    HeatMapItems.RemoveAt(HeatMapItems.Count - 1);
                    UpdateLayout();
                    if (IsHorizontalOriented)
                        ViewSize = new SizeF(ViewSize.Width, HeatMapItems[0].ViewSize.Height);
                    else
                        ViewSize = new SizeF(HeatMapItems[0].ViewSize.Width, ViewSize.Height);

                    break;
                }
                else
                {
                    itemsToPlace.RemoveAt(0);
                }
                ratio = HeatMapItems[0].Ratio;
            }
        }

        private void UpdateLayout()
        {
            if (HeatMapItems.Count == 0)
                return;

            // Calculate area needed for all items
            double necessaryArea = 0.0;
            foreach (HeatMapItem item in HeatMapItems)
            {
                necessaryArea += item.Area;
            }
            // Calculate missing side length - Height if view rectangle is wider than high, Width if view rectangle is higher than wide
            float sideLength = (float)(necessaryArea / Math.Min(ViewSize.Height, ViewSize.Width));
            foreach (HeatMapItem item in HeatMapItems)
            {
                if (IsHorizontalOriented)
                {
                    item.ViewSize = new SizeF((float)(item.Area / sideLength), sideLength);
                }
                else
                {
                    item.ViewSize = new SizeF(sideLength, (float)(item.Area / sideLength));
                }
            }
        }

        #endregion
    }
}
