using System.Collections.Generic;
using STOCKCHARTXLib;

namespace M4.Commands.ReverseCommands
{
    public class ReverseAddIndicatorCommand : ReverseChartCommandBase
    {
        protected override void ExecuteAction()
        {
            int seriesCount = ChartControlReceiver.StockChart.SeriesCount;
            List<string> seriesToRemove = new List<string>();

            // iterate through the specified indicator series and add them to Removal List.
            for (int i = 0; i < seriesCount; i++)
            {
                string seriesName = ChartControlReceiver.StockChart.get_SeriesName(i);

                if (seriesName.ToLower().Contains(Key.ToLower()))
                    seriesToRemove.Add(seriesName);
            }
            foreach (string series in seriesToRemove)
            {
                ChartControlReceiver.StockChart.RemoveSeries(series);
            }
            ChartControlReceiver.StockChart.Update();
        }
    }
}
