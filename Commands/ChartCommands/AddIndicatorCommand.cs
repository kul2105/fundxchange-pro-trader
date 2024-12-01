using M4.Commands.ReverseCommands;
using STOCKCHARTXLib;

namespace M4.Commands.ChartCommands
{
    public class AddIndicatorCommand : ChartCommandBase<ReverseAddIndicatorCommand>
    {
        public AddIndicatorCommand(ctlChartAdvanced chartControlReceiver, string indicator) : base(chartControlReceiver)
        {
            Key = indicator;
        }

        protected override bool ExecuteAction()
        {
            try
            {
                int indicatorIndex = -1;

                ChartControlReceiver.SortedIndicatorsList.TryGetValue(Key, out indicatorIndex);

                if (indicatorIndex > -1)
                {
                    int indicatorCount = ChartControlReceiver.StockChart.GetIndicatorCountByType((Indicator)indicatorIndex);

                    if(indicatorCount > 0)
                        Key += string.Format(" {0}", indicatorCount); 

                    int indicatorPanel = ChartControlReceiver.StockChart.AddChartPanel();

                    ChartControlReceiver.StockChart.AddIndicatorSeries((Indicator)indicatorIndex, Key, indicatorPanel, true);
                    ChartControlReceiver.StockChart.Update();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
