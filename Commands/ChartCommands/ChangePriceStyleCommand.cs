using System.Drawing;
using System.Windows.Forms;
using M4.Commands.ReverseCommands;
using STOCKCHARTXLib;

namespace M4.Commands.ChartCommands
{
    public class ChangePriceStyleCommand : ChartCommandBase<ReverseChangePriceStyleCommand>
    {
        private readonly string _priceStyle;

        public ChangePriceStyleCommand(ctlChartAdvanced chartControlReceiver, string priceStyle, string previousPriceStyle) : base(chartControlReceiver)
        {
            _priceStyle = priceStyle;
            Key = previousPriceStyle;
        }

        protected override bool ExecuteAction()
        {
            try
            {
                switch (_priceStyle)
                {
                    case "Bar Chart":
                        SetSeriesType(SeriesType.stStockBarChart);
                        break;
                    case "Candle Chart":
                        SetSeriesType(SeriesType.stCandleChart);
                        break;
                    case "Point && Figure":
                        SetPriceStyle(PriceStyle.psPointAndFigure, true);
                        break;
                    case "Renko":
                        SetPriceStyle(PriceStyle.psRenko, true);
                        break;
                    case "Kagi":
                        SetPriceStyle(PriceStyle.psKagi, true);
                        break;
                    case "Three Line Break":
                        SetPriceStyle(PriceStyle.psThreeLineBreak, true);
                        break;
                    case "EquiVolume":
                        SetPriceStyle(PriceStyle.psEquiVolume, false);
                        break;
                    case "EquiVolume Shadow":
                        SetPriceStyle(PriceStyle.psEquiVolumeShadow, false);
                        break;
                    case "Candle Volume":
                        SetPriceStyle(PriceStyle.psCandleVolume, false);
                        break;
                    case "Heikin Ashi":
                        SetPriceStyle(PriceStyle.psHeikinAshi, false);
                        break;
                    case "Line Chart":
                        DisplayLineChart();
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Private Methods
        private void SetSeriesType(SeriesType seriesType)
        {
            RestoreSeriesToDefault();

            ChartControlReceiver.StockChart.PriceStyle = PriceStyle.psStandard;
            ChartControlReceiver.StockChart.set_SeriesType(string.Format("{0}.open", ChartControlReceiver.ChartSelection.Symbol), seriesType);
            ChartControlReceiver.StockChart.set_SeriesType(string.Format("{0}.high", ChartControlReceiver.ChartSelection.Symbol), seriesType);
            ChartControlReceiver.StockChart.set_SeriesType(string.Format("{0}.low", ChartControlReceiver.ChartSelection.Symbol), seriesType);
            ChartControlReceiver.StockChart.set_SeriesType(string.Format("{0}.close", ChartControlReceiver.ChartSelection.Symbol), seriesType);
            ChartControlReceiver.StockChart.set_SeriesType(string.Format("{0}.% move", ChartControlReceiver.ChartSelection.Symbol), seriesType);

            ChartControlReceiver.StockChart.Update();
        }

        private void SetPriceStyle(PriceStyle priceStyle, bool displayPriceStyle)
        {
            RestoreSeriesToDefault();

            ChartControlReceiver.StockChart.PriceStyle = priceStyle;
            if (displayPriceStyle)
                (new frmPriceStyle()).GetInput(ChartControlReceiver.StockChart, _priceStyle);

            ChartControlReceiver.StockChart.Update();
        }

        private void DisplayLineChart()
        {
            if (ChartControlReceiver.StockChart.SeriesCount == 4)
            {
                MessageBox.Show("Line Chart option is not supported for this chart", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.open", ChartControlReceiver.ChartSelection.Symbol), false);
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.high", ChartControlReceiver.ChartSelection.Symbol), false);
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.low", ChartControlReceiver.ChartSelection.Symbol), false);
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.close", ChartControlReceiver.ChartSelection.Symbol), false);
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.% move", ChartControlReceiver.ChartSelection.Symbol), false);
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.line", ChartControlReceiver.ChartSelection.Symbol), true);

            ChartControlReceiver.StockChart.set_SeriesColor(string.Format("{0}.line", ChartControlReceiver.ChartSelection.Symbol), ColorTranslator.ToOle(Color.Black));

            ChartControlReceiver.StockChart.Update();
        }

        private void RestoreSeriesToDefault()
        {
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.open", ChartControlReceiver.ChartSelection.Symbol), true);
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.high", ChartControlReceiver.ChartSelection.Symbol), true);
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.low", ChartControlReceiver.ChartSelection.Symbol), true);
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.close", ChartControlReceiver.ChartSelection.Symbol), true);
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.% move", ChartControlReceiver.ChartSelection.Symbol), false);
            ChartControlReceiver.StockChart.set_SeriesVisible(string.Format("{0}.line", ChartControlReceiver.ChartSelection.Symbol), false);

            ChartControlReceiver.StockChart.set_SeriesColor(string.Format("{0}.line", ChartControlReceiver.ChartSelection.Symbol), ColorTranslator.ToOle(Color.Black));
        }
        #endregion
    }
}
