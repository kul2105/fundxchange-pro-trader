using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace FundXchange.Controls.GridView
{
    public class MarketDataGrid : Nevron.UI.WinForm.Controls.NDataGridView
    {
        public MarketDataGrid(): base()
        {
            DoubleBuffered = true;
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            try
            {
                base.OnValidating(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        protected override void OnValidated(EventArgs e)
        {
            try
            {
                base.OnValidated(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                lock (this)
                {
                    //if (base != null)
                    base.OnPaint(e);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }
}
