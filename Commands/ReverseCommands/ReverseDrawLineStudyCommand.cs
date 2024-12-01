using STOCKCHARTXLib;

namespace M4.Commands.ReverseCommands
{
    public class ReverseDrawLineStudyCommand : ReverseChartCommandBase
    {
        protected override void ExecuteAction()
        {
            ChartControlReceiver.StockChart.RemoveObject(ObjectType.otLineStudyObject, Key);
        }
    }
}