using System;
using M4.Commands.ReverseCommands;
using STOCKCHARTXLib;

namespace M4.Commands.ChartCommands
{
    public class DrawLineStudyCommand : ChartCommandBase<ReverseDrawLineStudyCommand>
    {
        private readonly StudyType _studyType;

        public DrawLineStudyCommand(ctlChartAdvanced chartControlReceiver, StudyType studyType)
            : base(chartControlReceiver)
        {
            Key = string.Format("{0}:{1}", studyType, DateTime.Now.Ticks);
            _studyType = studyType;
        }

        protected override bool ExecuteAction()
        {
            try
            {
                ChartControlReceiver.StockChart.DrawLineStudy(_studyType, Key);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}