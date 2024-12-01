using System;
using M4.Commands.ReverseCommands;
using STOCKCHARTXLib;

namespace M4.Commands.ChartCommands
{
    public class AddSymbolCommand : ChartCommandBase<ReverseAddSymbolCommand>
    {
        private readonly SymbolType _symbolType;

        public AddSymbolCommand(ctlChartAdvanced chartControlReceiver, SymbolType symbolType) : base(chartControlReceiver)
        {
            Key = string.Format("{0}:{1}", symbolType, DateTime.Now.Ticks);
            _symbolType = symbolType;
        }

        protected override bool ExecuteAction()
        {
            try
            {
                ChartControlReceiver.StockChart.AddUserSymbolObject(_symbolType, Key, "");
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
