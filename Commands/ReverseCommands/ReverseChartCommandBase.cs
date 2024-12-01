namespace M4.Commands.ReverseCommands
{
    public abstract class ReverseChartCommandBase : ICommand
    {
        protected ctlChartAdvanced ChartControlReceiver;
        private string _key;
        public string Key
        {
            get { return _key; }
        }

        public void Execute()
        {
            if(ChartControlReceiver != null)
            {
                ExecuteAction();
            }
        }

        public ReverseChartCommandBase CreateReverseCommand<T>(ctlChartAdvanced chartControlReceiver, string key) where T : ReverseChartCommandBase, new()
        {
            T TEntity = new T();

            TEntity.ChartControlReceiver = chartControlReceiver;
            TEntity._key = key;

            return TEntity;
        }

        protected abstract void ExecuteAction();
    }
}