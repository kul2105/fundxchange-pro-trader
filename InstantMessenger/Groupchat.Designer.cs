namespace FundXchangeMessenger
{
    partial class Groupchat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Groupchat));
            this.txtbox_groupname = new System.Windows.Forms.TextBox();
            this.btn_closegrpchat = new System.Windows.Forms.Button();
            this.btn_next = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtbox_groupname
            // 
            this.txtbox_groupname.BackColor = System.Drawing.Color.White;
            this.txtbox_groupname.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtbox_groupname.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtbox_groupname.ForeColor = System.Drawing.Color.Gray;
            this.txtbox_groupname.Location = new System.Drawing.Point(42, 238);
            this.txtbox_groupname.Multiline = true;
            this.txtbox_groupname.Name = "txtbox_groupname";
            this.txtbox_groupname.Size = new System.Drawing.Size(505, 57);
            this.txtbox_groupname.TabIndex = 0;
            this.txtbox_groupname.Text = "Group Name";
            this.txtbox_groupname.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn_closegrpchat
            // 
            this.btn_closegrpchat.BackColor = System.Drawing.Color.Transparent;
            this.btn_closegrpchat.FlatAppearance.BorderSize = 0;
            this.btn_closegrpchat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_closegrpchat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_closegrpchat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_closegrpchat.ForeColor = System.Drawing.Color.Transparent;
            this.btn_closegrpchat.Location = new System.Drawing.Point(12, 25);
            this.btn_closegrpchat.Name = "btn_closegrpchat";
            this.btn_closegrpchat.Size = new System.Drawing.Size(46, 32);
            this.btn_closegrpchat.TabIndex = 1;
            this.btn_closegrpchat.UseVisualStyleBackColor = false;
            this.btn_closegrpchat.Click += new System.EventHandler(this.btn_closegrpchat_Click);
            // 
            // btn_next
            // 
            this.btn_next.BackColor = System.Drawing.Color.White;
            this.btn_next.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_next.BackgroundImage")));
            this.btn_next.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_next.FlatAppearance.BorderSize = 0;
            this.btn_next.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_next.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_next.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_next.ForeColor = System.Drawing.Color.Transparent;
            this.btn_next.Location = new System.Drawing.Point(451, 532);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new System.Drawing.Size(138, 121);
            this.btn_next.TabIndex = 2;
            this.btn_next.UseVisualStyleBackColor = false;
            this.btn_next.Click += new System.EventHandler(this.btn_next_Click);
            // 
            // Groupchat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(592, 653);
            this.Controls.Add(this.btn_next);
            this.Controls.Add(this.btn_closegrpchat);
            this.Controls.Add(this.txtbox_groupname);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Groupchat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Groupchat_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtbox_groupname;
        private System.Windows.Forms.Button btn_closegrpchat;
        private System.Windows.Forms.Button btn_next;
    }
}