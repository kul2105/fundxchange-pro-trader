using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FundXchangeMessenger
{
    public partial class GroupWindow : Form
    {
        private string Groupname = "";
        private Dictionary<string, List<string>> dict_groupinfo = new Dictionary<string, List<string>>();
        public GroupWindow(string groupname)
        {
            InitializeComponent();
            Groupname = groupname;
        }

        string userinfofile = System.IO.Directory.GetParent(System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()) + "\\userinfo.csv";
        DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
        private void GroupWindow_Load(object sender, EventArgs e)
        {
            List<string> RegisteredUserName = new List<string>();
            using (var reader = new StreamReader(userinfofile))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    RegisteredUserName.Add(values[0]);
                }
            }
            

            checkColumn.AutoSizeMode =
                DataGridViewAutoSizeColumnMode.DisplayedCells;
            checkColumn.CellTemplate = new DataGridViewCheckBoxCell();
            checkColumn.FalseValue = true;
            checkColumn.Name = "CheckBoxes";
            checkColumn.TrueValue = true;
            checkColumn.FalseValue = false;            
            dgv_user.DataSource = RegisteredUserName.Select(x => new { Users = x }).ToList();
            dgv_user.Columns.Add("", "");
            dgv_user.Columns.Add("", "");
            dgv_user.Columns.Add("", "");
            dgv_user.Columns.Insert(4, checkColumn);
            dgv_user.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            Groupchat groupchat = new Groupchat();
            groupchat.Show();
            this.Hide();
        }

        private void btn_Done_Click(object sender, EventArgs e)
        {
            List<string> Users = new List<string>();
            foreach (DataGridViewRow row in dgv_user.Rows)
            {
                if (Convert.ToBoolean(row.Cells[checkColumn.Name].Value) == true)
                {
                    string name = row.Cells["Users"].Value.ToString();
                    Users.Add(name);
                }
            }
            dict_groupinfo.Add(Groupname, Users);

            if (!File.Exists(userinfofile))
            {

                File.WriteAllText(userinfofile, Groupname);
            }
            List<string> RegisteredUserName = new List<string>();
            using (var reader = new StreamReader(userinfofile))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    RegisteredUserName.Add(values[0]);
                }
            }
            int i = 0;
            if (!RegisteredUserName.Contains(Groupname))
            {
                string name = "";
                foreach (var item in Users)
                {
                    name = name+Users[i]+" ";
                    
                    i++;
                }
                File.AppendAllText(userinfofile, Environment.NewLine + Groupname + " " + name);
                //dgv_Loadusers.AutoGenerateColumns = true;

            }
            this.Hide();
        }
    }
}
