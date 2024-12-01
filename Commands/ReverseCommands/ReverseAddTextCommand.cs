using STOCKCHARTXLib;

namespace M4.Commands.ReverseCommands
{
    public class ReverseAddTextCommand : ReverseChartCommandBase
    {
        protected override void ExecuteAction()
        {
            ChartControlReceiver.StockChart.RemoveObject(ObjectType.otTextObject, Key);
        }
    }
}
