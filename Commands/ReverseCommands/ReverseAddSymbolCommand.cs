using STOCKCHARTXLib;

namespace M4.Commands.ReverseCommands
{
    public class ReverseAddSymbolCommand : ReverseChartCommandBase
    {
        protected override void ExecuteAction()
        {
            ChartControlReceiver.StockChart.RemoveObject(ObjectType.otSymbolObject, Key);
        }
    }
}
