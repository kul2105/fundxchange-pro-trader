using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FundXchange.Controls.GridView
{
    /// <summary>
    /// This class represents a TextBoxCell that spans multiple columns.
    /// This class was originally created by Robert Gowland: http://www.gowland.ca/datagridviewspanningtextboxcell
    /// </summary>
    public class SpanningTextBoxCell : DataGridViewTextBoxCell
    {
        #region Properties

        /// <summary>
        /// Gets and sets the number of columns this DataGridViewCell spans.
        /// </summary>
        public int ColumnSpan
        {
            get
            {
                if (null != DataGridView)
                    return DataGridView.ColumnCount;
                return 1;
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Override of OnDataGridViewChanged sets the CellPainting event
        /// </summary>
        protected override void OnDataGridViewChanged()
        {
            base.OnDataGridViewChanged();
            if (this.DataGridView != null)
            {
                this.DataGridView.CellPainting += this.DataGridView_CellPainting;
            }
        }

        /// <summary>
        /// Override PositionEditingControl sets the cell clipping region
        /// </summary>
        /// <param name="setLocation">true to have the control placed as specified by the other arguments; false to allow the control to place itself.</param>
        /// <param name="setSize">true to specify the size; false to allow the control to size itself.</param>
        /// <param name="cellBounds">A System.Drawing.Rectangle that defines the cell bounds.</param>
        /// <param name="cellClip">The area that will be used to paint the editing control.</param>
        /// <param name="cellStyle">A System.Windows.Forms.DataGridViewCellStyle that represents the style of the cell being edited.</param>
        /// <param name="singleVerticalBorderAdded">true to add a vertical border to the cell; otherwise, false.</param>
        /// <param name="singleHorizontalBorderAdded">true to add a horizontal border to the cell; otherwise, false.</param>
        /// <param name="isFirstDisplayedColumn">true if the hosting cell is in the first visible column; otherwise, false.</param>
        /// <param name="isFirstDisplayedRow">true if the hosting cell is in the first visible row; otherwise, false.</param>
        public override void PositionEditingControl(bool setLocation, bool setSize, Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
        {
            cellBounds.Width = 0;
            cellClip.Width = 0;

            for (int i = 0; i < this.ColumnSpan; i++)
            {
                if (this.DataGridView.Columns[this.ColumnIndex + i].Visible)
                {
                    cellBounds.Width += this.DataGridView.Columns[this.ColumnIndex + i].Width;
                    cellClip.Width += this.DataGridView.Columns[this.ColumnIndex + i].Width;
                }
            }

            base.PositionEditingControl(setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
        }

        /// <summary>
        /// Override DetachEditingControl invalidates cells
        /// </summary>
        public override void DetachEditingControl()
        {
            base.DetachEditingControl();
            this.DataGridView.InvalidateCell(this);
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Handles the cell painting event for the DataGridView
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The argument parameters</param>
        protected virtual void DataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.RowIndex == this.RowIndex) && (this.RowIndex != -1))
            {
                if ((e.ColumnIndex >= this.ColumnIndex) && (this.DataGridView[this.ColumnIndex, e.RowIndex] is SpanningTextBoxCell))
                {
                    /*
                     * Note the difference between the terms "displayed" and "visible".
                     * A visible column is one that has its Visible property set to true.
                     * A displayed column is one that is visible and is not hidden from view by horizontal scrolling.
                     */

                    //The total width of visible columns included in the span.
                    int spanWidth = 0;

                    //The total width of columns in the span that are to the left of the first displayed column.
                    //(This does not include the part of the width of a the first displayed column that may be partially displayed.)
                    int totalScrolledColumnWidth = 0;

                    //The offset from the first column (visible or not) in the span to the first displayed column in the span.
                    int offsetToFirstDisplayedColumnInSpan = this.DataGridView.FirstDisplayedCell.ColumnIndex - this.ColumnIndex;
                    if (offsetToFirstDisplayedColumnInSpan < 0)
                    {
                        offsetToFirstDisplayedColumnInSpan = 0;
                    }

                    //Calculate the total span and scrolled span.
                    for (int i = 0; i < this.ColumnSpan; i++)
                    {
                        if (this.DataGridView.Columns[this.ColumnIndex + i].Visible)
                        {
                            spanWidth += this.DataGridView.Columns[this.ColumnIndex + i].Width;

                            if (i < offsetToFirstDisplayedColumnInSpan)
                            {
                                totalScrolledColumnWidth += this.DataGridView.Columns[this.ColumnIndex + i].Width;
                            }
                        }
                    }

                    //Get a rectangle that defines the area on the screen that we want to draw to.
                    Rectangle spanningDisplayRectangle = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex + offsetToFirstDisplayedColumnInSpan, e.RowIndex, false);
                    spanningDisplayRectangle.Width = spanWidth;

                    //Adjust the rectangle to compensate for visible row headers.
                    if (this.DataGridView.RowHeadersVisible)
                    {
                        if (spanningDisplayRectangle.X < this.DataGridView.RowHeadersWidth)
                        {
                            spanningDisplayRectangle.Width -= (this.DataGridView.RowHeadersWidth - spanningDisplayRectangle.X);
                            spanningDisplayRectangle.X = this.DataGridView.RowHeadersWidth + 1;
                        }
                    }

                    //Set clipping to the calculated rectangle
                    RectangleF oldClip = e.Graphics.ClipBounds;
                    e.Graphics.SetClip(spanningDisplayRectangle);

                    //If the Horizontal Scrolling offset is non-zero, the display rectangle's X property is always set to 1 (at
                    //least for Visual Studio 2008 9.0.21022.8 RTM), which is the correct value for clipping, so we need to start
                    //drawing further left than the start of the rectagle. 
                    if (this.DataGridView.HorizontalScrollingOffset > 0)
                    {
                        Rectangle rootCellDisplayRectangle = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex + offsetToFirstDisplayedColumnInSpan, e.RowIndex, false);
                        spanningDisplayRectangle.X += rootCellDisplayRectangle.Width - this.DataGridView.Columns[this.ColumnIndex + offsetToFirstDisplayedColumnInSpan].Width;
                        spanningDisplayRectangle.X -= totalScrolledColumnWidth;
                    }

                    //Move the text a pixel away from the cell border
                    int stringX = spanningDisplayRectangle.X + 1;
                    int stringY = spanningDisplayRectangle.Y + 1;

                    //Draw the cell and border
                    Brush foreBrush = null;
                    Brush backBrush = null;
                    try
                    {
                        if (this.Selected)
                        {
                            foreBrush = new SolidBrush(this.InheritedStyle.SelectionForeColor);
                            backBrush = new SolidBrush(this.InheritedStyle.SelectionBackColor);
                        }
                        else
                        {
                            foreBrush = new SolidBrush(this.InheritedStyle.ForeColor);
                            backBrush = new SolidBrush(this.InheritedStyle.BackColor);
                        }

                        e.Graphics.FillRectangle(backBrush, spanningDisplayRectangle);
                        e.Graphics.DrawString(this.FormattedValue.ToString(), this.DataGridView.Columns[this.ColumnIndex].InheritedStyle.Font, foreBrush, stringX, stringY);
                        Pen pen = new Pen(this.DataGridView.GridColor);
                        e.Graphics.DrawLine(pen, spanningDisplayRectangle.X, spanningDisplayRectangle.Top, spanningDisplayRectangle.Right, spanningDisplayRectangle.Top);
                        e.Graphics.DrawLine(pen, spanningDisplayRectangle.X, spanningDisplayRectangle.Y - 1, spanningDisplayRectangle.X, spanningDisplayRectangle.Bottom + 1);
                    }
                    finally
                    {
                        if (foreBrush != null)
                        {
                            foreBrush.Dispose();
                        }
                        if (backBrush != null)
                        {
                            backBrush.Dispose();
                        }
                    }

                    //Reset the clipping to its original value
                    e.Graphics.SetClip(oldClip);

                    e.Handled = true;
                }
            }
        }

        #endregion
    }
}
