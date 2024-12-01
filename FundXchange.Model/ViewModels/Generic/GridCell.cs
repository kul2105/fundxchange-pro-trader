using System.Drawing;

namespace FundXchange.Model.ViewModels.Generic
{
    public class GridCell
    {
        public GridCell()
        {
        }

        public GridCell(string text, Color backColor, Color foreColor)
        {
            Text = text;
            BackColor = backColor;
            ForeColor = foreColor;
        }

        public string Text { get; set; }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
    }
}
