namespace FundXchangeMessenger
{
    partial class GroupWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupWindow));
            this.btn_Previous = new System.Windows.Forms.Button();
            this.btn_Done = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.txtbox_search = new System.Windows.Forms.TextBox();
            this.dgv_user = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_user)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Previous
            // 
            this.btn_Previous.BackColor = System.Drawing.Color.Transparent;
            this.btn_Previous.FlatAppearance.BorderSize = 0;
            this.btn_Previous.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_Previous.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_Previous.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Previous.Location = new System.Drawing.Point(10, 19);
            this.btn_Previous.Name = "btn_Previous";
            this.btn_Previous.Size = new System.Drawing.Size(75, 39);
            this.btn_Previous.TabIndex = 0;
            this.btn_Previous.UseVisualStyleBackColor = false;
            this.btn_Previous.Click += new System.EventHandler(this.btn_Previous_Click);
            // 
            // btn_Done
            // 
            this.btn_Done.BackColor = System.Drawing.Color.Transparent;
            this.btn_Done.FlatAppearance.BorderSize = 0;
            this.btn_Done.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_Done.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_Done.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Done.Location = new System.Drawing.Point(490, 12);
            this.btn_Done.Name = "btn_Done";
            this.btn_Done.Size = new System.Drawing.Size(92, 41);
            this.btn_Done.TabIndex = 1;
            this.btn_Done.UseVisualStyleBackColor = false;
            this.btn_Done.Click += new System.EventHandler(this.btn_Done_Click);
            // 
            // btn_close
            // 
            this.btn_close.BackColor = System.Drawing.Color.Transparent;
            this.btn_close.FlatAppearance.BorderSize = 0;
            this.btn_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_close.Location = new System.Drawing.Point(547, 80);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(45, 39);
            this.btn_close.TabIndex = 2;
            this.btn_close.UseVisualStyleBackColor = false;
            // 
            // txtbox_search
            // 
            this.txtbox_search.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtbox_search.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtbox_search.Location = new System.Drawing.Point(2, 80);
            this.txtbox_search.Multiline = true;
            this.txtbox_search.Name = "txtbox_search";
            this.txtbox_search.Size = new System.Drawing.Size(539, 39);
            this.txtbox_search.TabIndex = 3;
            this.txtbox_search.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // dgv_user
            // 
            this.dgv_user.BackgroundColor = System.Drawing.Color.White;
            this.dgv_user.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_user.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgv_user.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_user.ColumnHeadersVisible = false;
            this.dgv_user.Location = new System.Drawing.Point(2, 141);
            this.dgv_user.Name = "dgv_user";
            this.dgv_user.RowHeadersVisible = false;
            this.dgv_user.RowTemplate.Height = 24;
            this.dgv_user.Size = new System.Drawing.Size(590, 503);
            this.dgv_user.TabIndex = 4;
            // 
            // GroupWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(604, 656);
            this.Controls.Add(this.dgv_user);
            this.Controls.Add(this.txtbox_search);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.btn_Done);
            this.Controls.Add(this.btn_Previous);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "GroupWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GroupWindow";
            this.Load += new System.EventHandler(this.GroupWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_user)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Previous;
        private System.Windows.Forms.Button btn_Done;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.TextBox txtbox_search;
        private System.Windows.Forms.DataGridView dgv_user;
    }
}