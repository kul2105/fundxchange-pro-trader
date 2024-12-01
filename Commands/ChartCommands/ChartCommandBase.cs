using M4.Commands.ReverseCommands;

namespace M4.Commands.ChartCommands
{
    public abstract class ChartCommandBase<T> where T : ReverseChartCommandBase, ICommand, new()
    {
        protected string Key;
        protected readonly ctlChartAdvanced ChartControlReceiver;

        protected ChartCommandBase(ctlChartAdvanced chartControlReceiver)
        {
            ChartControlReceiver = chartControlReceiver;   
        }

        public void Execute()
        {
            if (ChartControlReceiver != null)
            {
                if (ExecuteAction())
                {
                    CreateReverseCommand();
                }
            }
        }

        public void ExecuteWithoutReverseCommand()
        {
            if (ChartControlReceiver != null)
            {
                ExecuteAction();
            }
        }

        protected abstract bool ExecuteAction();

        private void CreateReverseCommand()
        {
            ReverseChartCommandBase command = new T().CreateReverseCommand<T>(ChartControlReceiver, Key);
            ChartControlReceiver.AddUndoCommand(command);
        }
    }
}