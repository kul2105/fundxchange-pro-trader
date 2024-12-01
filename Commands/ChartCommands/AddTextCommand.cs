using System;
using M4.Commands.ReverseCommands;

namespace M4.Commands.ChartCommands
{
    public class AddTextCommand : ChartCommandBase<ReverseAddTextCommand>
    {
        public AddTextCommand(ctlChartAdvanced chartControlReceiver) : base(chartControlReceiver)
        {
            Key = string.Format("AddText:{0}", DateTime.Now.Ticks);
        }

        protected override bool ExecuteAction()
        {
            try
            {
                ChartControlReceiver.StockChart.AddUserDefinedText(Key);
                ChartControlReceiver.StockChart.Focus();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
