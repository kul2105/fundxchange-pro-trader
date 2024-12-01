using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InstantMessenger
{
    public partial class FundXchangeChatRegistrationForm : Form
    {
        public FundXchangeChatRegistrationForm()
        {
            InitializeComponent();
        }

        public string UserName;
        public string Password;
        public string EmailAddress;
        public string MobileNo;

        private void btn_submit_Click(object sender, EventArgs e)
        {
            UserName = txtbox_username.Text;
            Password = txtbox_confirmpassword.Text;
            EmailAddress = txtbox_emailaddress.Text;
            MobileNo = txtbox_mobileno.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
