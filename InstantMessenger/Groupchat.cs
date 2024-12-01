using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FundXchangeMessenger
{
    public partial class Groupchat : Form
    {
        public Groupchat()
        {
            InitializeComponent();
        }

        private void Groupchat_Load(object sender, EventArgs e)
        {

        }

        private void btn_next_Click(object sender, EventArgs e)
        {

            GroupWindow groupwindow = new GroupWindow(txtbox_groupname.Text);
            groupwindow.Show();
            this.Hide();
        }

        private void btn_closegrpchat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
