namespace InstantMessenger
{
    partial class TalkForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TalkForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtbox_user = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.dgv_Loadusers = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkbox_All = new System.Windows.Forms.CheckBox();
            this.chkbox_chat = new System.Windows.Forms.CheckBox();
            this.chkbox_sms = new System.Windows.Forms.CheckBox();
            this.chkbox_email = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.talkText = new System.Windows.Forms.TextBox();
            this.btn_send = new System.Windows.Forms.Button();
            this.sendText = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.btn_groupchat = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Loadusers)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Gray;
            this.splitContainer1.Panel1.Controls.Add(this.btn_groupchat);
            this.splitContainer1.Panel1.Controls.Add(this.txtbox_user);
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox2);
            this.splitContainer1.Panel1.Controls.Add(this.dgv_Loadusers);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.talkText);
            this.splitContainer1.Panel1.ForeColor = System.Drawing.Color.Gray;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.Gray;
            this.splitContainer1.Panel2.Controls.Add(this.btn_send);
            this.splitContainer1.Panel2.Controls.Add(this.sendText);
            this.splitContainer1.Size = new System.Drawing.Size(1275, 692);
            this.splitContainer1.SplitterDistance = 533;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // txtbox_user
            // 
            this.txtbox_user.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtbox_user.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.txtbox_user.Location = new System.Drawing.Point(79, 55);
            this.txtbox_user.Multiline = true;
            this.txtbox_user.Name = "txtbox_user";
            this.txtbox_user.Size = new System.Drawing.Size(177, 28);
            this.txtbox_user.TabIndex = 12;
            this.txtbox_user.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.BackgroundImage")));
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(17, 55);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(60, 28);
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // dgv_Loadusers
            // 
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            this.dgv_Loadusers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_Loadusers.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgv_Loadusers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_Loadusers.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgv_Loadusers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Loadusers.ColumnHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Loadusers.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgv_Loadusers.GridColor = System.Drawing.SystemColors.Window;
            this.dgv_Loadusers.Location = new System.Drawing.Point(11, 131);
            this.dgv_Loadusers.Name = "dgv_Loadusers";
            this.dgv_Loadusers.ReadOnly = true;
            this.dgv_Loadusers.RowHeadersVisible = false;
            this.dgv_Loadusers.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgv_Loadusers.RowTemplate.Height = 24;
            this.dgv_Loadusers.Size = new System.Drawing.Size(388, 395);
            this.dgv_Loadusers.TabIndex = 10;
            this.dgv_Loadusers.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Loadusers_CellClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkbox_All);
            this.groupBox1.Controls.Add(this.chkbox_chat);
            this.groupBox1.Controls.Add(this.chkbox_sms);
            this.groupBox1.Controls.Add(this.chkbox_email);
            this.groupBox1.Location = new System.Drawing.Point(11, 81);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 45);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // chkbox_All
            // 
            this.chkbox_All.AutoSize = true;
            this.chkbox_All.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkbox_All.ForeColor = System.Drawing.Color.Black;
            this.chkbox_All.Location = new System.Drawing.Point(246, 15);
            this.chkbox_All.Name = "chkbox_All";
            this.chkbox_All.Size = new System.Drawing.Size(51, 21);
            this.chkbox_All.TabIndex = 8;
            this.chkbox_All.Text = "ALL";
            this.chkbox_All.UseVisualStyleBackColor = true;
            // 
            // chkbox_chat
            // 
            this.chkbox_chat.AutoSize = true;
            this.chkbox_chat.Checked = true;
            this.chkbox_chat.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbox_chat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkbox_chat.ForeColor = System.Drawing.Color.Black;
            this.chkbox_chat.Image = ((System.Drawing.Image)(resources.GetObject("chkbox_chat.Image")));
            this.chkbox_chat.Location = new System.Drawing.Point(17, 13);
            this.chkbox_chat.Name = "chkbox_chat";
            this.chkbox_chat.Size = new System.Drawing.Size(38, 24);
            this.chkbox_chat.TabIndex = 5;
            this.chkbox_chat.UseVisualStyleBackColor = true;
            this.chkbox_chat.CheckedChanged += new System.EventHandler(this.chkbox_chat_CheckedChanged);
            // 
            // chkbox_sms
            // 
            this.chkbox_sms.AutoSize = true;
            this.chkbox_sms.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkbox_sms.ForeColor = System.Drawing.Color.Black;
            this.chkbox_sms.Image = ((System.Drawing.Image)(resources.GetObject("chkbox_sms.Image")));
            this.chkbox_sms.Location = new System.Drawing.Point(164, 13);
            this.chkbox_sms.Name = "chkbox_sms";
            this.chkbox_sms.Size = new System.Drawing.Size(38, 24);
            this.chkbox_sms.TabIndex = 7;
            this.chkbox_sms.UseVisualStyleBackColor = true;
            // 
            // chkbox_email
            // 
            this.chkbox_email.AutoSize = true;
            this.chkbox_email.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkbox_email.ForeColor = System.Drawing.Color.Black;
            this.chkbox_email.Image = ((System.Drawing.Image)(resources.GetObject("chkbox_email.Image")));
            this.chkbox_email.Location = new System.Drawing.Point(89, 13);
            this.chkbox_email.Name = "chkbox_email";
            this.chkbox_email.Size = new System.Drawing.Size(38, 24);
            this.chkbox_email.TabIndex = 6;
            this.chkbox_email.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(17, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(33, 34);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(50, 11);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(349, 34);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "People groups && messages";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // talkText
            // 
            this.talkText.BackColor = System.Drawing.SystemColors.Window;
            this.talkText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.talkText.ForeColor = System.Drawing.Color.Blue;
            this.talkText.Location = new System.Drawing.Point(415, -2);
            this.talkText.Margin = new System.Windows.Forms.Padding(4);
            this.talkText.Multiline = true;
            this.talkText.Name = "talkText";
            this.talkText.ReadOnly = true;
            this.talkText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.talkText.Size = new System.Drawing.Size(857, 541);
            this.talkText.TabIndex = 2;
            this.talkText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn_send
            // 
            this.btn_send.BackColor = System.Drawing.SystemColors.Window;
            this.btn_send.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_send.BackgroundImage")));
            this.btn_send.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_send.FlatAppearance.BorderColor = System.Drawing.SystemColors.Window;
            this.btn_send.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_send.Location = new System.Drawing.Point(1213, -1);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(36, 153);
            this.btn_send.TabIndex = 2;
            this.btn_send.UseVisualStyleBackColor = false;
            this.btn_send.Click += new System.EventHandler(this.btn_send_Click);
            // 
            // sendText
            // 
            this.sendText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.sendText.Location = new System.Drawing.Point(415, -1);
            this.sendText.Margin = new System.Windows.Forms.Padding(4);
            this.sendText.Multiline = true;
            this.sendText.Name = "sendText";
            this.sendText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sendText.Size = new System.Drawing.Size(857, 153);
            this.sendText.TabIndex = 1;
            this.sendText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.sendText_KeyDown);
            this.sendText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.sendText_KeyPress);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 10000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // btn_groupchat
            // 
            this.btn_groupchat.BackColor = System.Drawing.Color.White;
            this.btn_groupchat.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_groupchat.FlatAppearance.BorderSize = 0;
            this.btn_groupchat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_groupchat.ForeColor = System.Drawing.Color.Black;
            this.btn_groupchat.Location = new System.Drawing.Point(315, 51);
            this.btn_groupchat.Name = "btn_groupchat";
            this.btn_groupchat.Size = new System.Drawing.Size(84, 32);
            this.btn_groupchat.TabIndex = 13;
            this.btn_groupchat.Text = "+ Chat";
            this.btn_groupchat.UseVisualStyleBackColor = false;
            this.btn_groupchat.Click += new System.EventHandler(this.btn_groupchat_Click);
            // 
            // TalkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(1275, 692);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TalkForm";
            this.Text = "FundExchange Chat Group";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TalkForm_FormClosing);
            this.Load += new System.EventHandler(this.TalkForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Loadusers)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox sendText;
        private System.Windows.Forms.TextBox talkText;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkbox_All;
        private System.Windows.Forms.CheckBox chkbox_chat;
        private System.Windows.Forms.CheckBox chkbox_sms;
        private System.Windows.Forms.CheckBox chkbox_email;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.DataGridView dgv_Loadusers;
        private System.Windows.Forms.TextBox txtbox_user;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btn_groupchat;
    }
}