using FundXchangeMessenger;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using InstantMessengerServer;
using System.Xml;
using System.Security.Cryptography;

namespace InstantMessenger
{
    public partial class TalkForm : Form
    {
        private string Username = "";
        private string Emailaddress = "";
        private string Mobileno = "";
        private BackgroundWorker backgroundworker_smsreply = new BackgroundWorker();
        private Dictionary<string, List<string>> dict_groupinfo = new Dictionary<string, List<string>>();
        string RegUser = "";
        int usercounter = 0;
        int smscount = 0;
        string previoussmsreply = "";
        int smsreferencecount = 0;
        public TalkForm(IMClient im, string user,string mobileno,string emailaddress)
        {
            InitializeComponent();
            this.im = im;
            this.sendTo = user;
            RegUser = user;
            txtbox_user.Text = user;
            Username = user;
            Emailaddress = emailaddress;
            Mobileno = mobileno;
        }


        public IMClient im;
        public string sendTo;
        int Previouscount = 0;
        string userinfofile = System.IO.Directory.GetParent(System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()) + "\\userinfo.csv";
        string usersFileName = System.IO.Directory.GetParent(System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()) + "\\users.dat";
        public Dictionary<string, InstantMessengerServer.UserInfo> users = new Dictionary<string, InstantMessengerServer.UserInfo>();  // Information about users + connections info.

        IMAvailEventHandler availHandler;
        IMReceivedEventHandler receivedHandler;
        private void TalkForm_Load(object sender, EventArgs e)
        {
            //this.Text = sendTo;
            availHandler = new IMAvailEventHandler(im_UserAvailable);
            receivedHandler = new IMReceivedEventHandler(im_MessageReceived);
            im.UserAvailable += availHandler;
            im.MessageReceived += receivedHandler;
            im.IsAvailable(sendTo);

            LoadUsers();
        }
        private void TalkForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            im.UserAvailable -= availHandler;
            im.MessageReceived -= receivedHandler;
        }
        private void LoadUsers()
        {
            try
            {
                if (!File.Exists(userinfofile))
                {

                    File.WriteAllText(userinfofile, this.sendTo);
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
                if (!RegisteredUserName.Contains(this.sendTo))
                {
                    File.AppendAllText(userinfofile, Environment.NewLine + this.sendTo);
                    //dgv_Loadusers.AutoGenerateColumns = true;
                    dgv_Loadusers.DataSource = RegisteredUserName.Select(x => new { Users = x }).ToList();
                    dgv_Loadusers.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                
            }
            catch(Exception ex)
            {
                
            }
        }

        private void sendmessage()
        {
            if (dict_groupinfo.ContainsKey(sendTo))
            {
                List<string> users = new List<string>();
                dict_groupinfo.TryGetValue(sendTo, out users);
                int i = 0;
                foreach (var item in users)
                {
                    if(users[i]!="" && users[i] != RegUser)
                    im.SendMessage(users[i], sendText.Text);
                    i++;
                }
                talkText.Text += String.Format("[{0}] {1}\r\n", im.UserName, sendText.Text);
            }
            else
            {
                im.SendMessage(sendTo, sendText.Text);
                talkText.Text += String.Format("[{0}] {1}\r\n", im.UserName, sendText.Text);
                sendText.Text = "";
            }
        }

        bool lastAvail = false;
        void im_UserAvailable(object sender, IMAvailEventArgs e)
        {
            List<string> Sendusers = new List<string>();
            this.BeginInvoke(new MethodInvoker(delegate
            {
                try
                {
                    List<string> RegisteredUserName = new List<string>();
                    using (var reader = new StreamReader(userinfofile))
                    {

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(';');
                            if (values[0].Contains(" "))
                            {
                                int i = 1;
                                string[] name = values[0].Split(' ');
                                foreach (var item in name)
                                {
                                    if(name.Count()>(i) && name[i]!="")
                                    Sendusers.Add(name[i]);
                                    i++;
                                }
                                if(!dict_groupinfo.ContainsKey(name[0]))
                                dict_groupinfo.Add(name[0], Sendusers);

                                if (!RegisteredUserName.Contains(name[0]))
                                    RegisteredUserName.Add(name[0]);
                            }
                            else
                            {
                                RegisteredUserName.Add(values[0]);
                            }
                        }
                    }
                    if (RegisteredUserName.Count!=Previouscount)
                    {
                        dgv_Loadusers.DataSource = RegisteredUserName.Select(x => new { Users = x }).ToList();
                        dgv_Loadusers.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        Previouscount = RegisteredUserName.Count;
                    }
                }

                catch (Exception ex)
                {
                    string exception = ex.ToString();
                }
                //if (e.UserName == sendTo)
                //{
                //    if (lastAvail != e.IsAvailable)
                //    {
                //        lastAvail = e.IsAvailable;
                //        string avail = (e.IsAvailable ? "available" : "unavailable");
                //        this.Text = String.Format("{0} - {1}", sendTo, avail);
                //        talkText.Text += String.Format("[{0} is {1}]\r\n", sendTo, avail);
                //    }
                //}
                if (dict_groupinfo.ContainsKey(sendTo))
                {
                    List<string> users = new List<string>();
                    dict_groupinfo.TryGetValue(sendTo, out users);
                    foreach (var item in users)
                    {
                        if (e.UserName == sendTo)
                        {
                            //if (lastAvail != e.IsAvailable)
                            //{
                            //    lastAvail = e.IsAvailable;
                                string avail = (e.IsAvailable ? "available" : "unavailable");
                            if (usercounter < users.Count() && users[usercounter]!= RegUser)
                            {
                                this.Text = String.Format("{0} - {1}", users[usercounter], avail);
                                talkText.Text += String.Format("[{0} is {1}]\r\n", users[usercounter], avail);
                            }
                            //}
                        }
                        usercounter++;
                    }
                }
                else
                {
                    if (e.UserName == sendTo)
                    {
                        if (lastAvail != e.IsAvailable)
                        {
                            lastAvail = e.IsAvailable;
                            string avail = (e.IsAvailable ? "available" : "unavailable");
                            this.Text = String.Format("{0} - {1}", sendTo, avail);
                            talkText.Text += String.Format("[{0} is {1}]\r\n", sendTo, avail);
                        }
                    }
                }

            }));
        }
       
        void im_MessageReceived(object sender, IMReceivedEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                if (e.From == sendTo)
                {
                    talkText.Text += String.Format("[{0}] {1}\r\n", e.From, e.Message);
                }
            }));
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            im.IsAvailable(sendTo);
        }


        private void sendText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if(chkbox_chat.Checked)
                sendmessage();
                if (chkbox_email.Checked)
                    SendSimpleMessage();
                if (chkbox_sms.Checked)
                {
                    SendSMS();
                    if (!backgroundworker_smsreply.IsBusy)
                    {
                        backgroundworker_smsreply.DoWork += backgroundworker_smsreply_DoWork;
                        backgroundworker_smsreply.WorkerReportsProgress = true;
                        backgroundworker_smsreply.RunWorkerAsync();
                        backgroundworker_smsreply.WorkerSupportsCancellation = true;
                    }
                }
                if(chkbox_All.Checked)
                {
                    sendmessage();
                    SendSimpleMessage();
                    SendSMS();
                    if(!backgroundworker_smsreply.IsBusy)
                    {
                        backgroundworker_smsreply.DoWork += backgroundworker_smsreply_DoWork;
                        backgroundworker_smsreply.WorkerReportsProgress = true;
                        backgroundworker_smsreply.RunWorkerAsync();
                        backgroundworker_smsreply.WorkerSupportsCancellation = true;
                    }
                }
                sendText.Text = "";
                sendText.Select(sendText.Text.Length, 0);
            }
        }

        private void backgroundworker_smsreply_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                smscount++;
                string smsreply = SMSReply();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(smsreply); // suppose that myXmlString contains "<Names>...</Names>"

                XmlNodeList also_calledList = xml.GetElementsByTagName("message");
                XmlNode also_calledElement = also_calledList[0];
                XmlNodeList titleList = also_calledElement.ChildNodes;

                foreach (XmlNode titleNode in titleList)
                {

                    if (InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            if (smscount < 1000 && talkText.Text!=previoussmsreply)
                            {
                                talkText.Text += String.Format("[{0}] {1}\r\n", "SmsReply", titleNode.InnerText);
                                previoussmsreply = talkText.Text;
                                backgroundworker_smsreply.CancelAsync();
                            }
                        }));

                    }

                    else
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            talkText.Text += String.Format("[{0}] {1}\r\n", "SmsReply", titleNode.InnerText);
                        }));
                    }
                   
                }
            }
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            if (chkbox_chat.Checked)
                sendmessage();
            if (chkbox_email.Checked)
                SendSimpleMessage();
            if (chkbox_sms.Checked)
            {
                SendSMS();
                if (!backgroundworker_smsreply.IsBusy)
                {
                    backgroundworker_smsreply.DoWork += backgroundworker_smsreply_DoWork;
                    backgroundworker_smsreply.WorkerReportsProgress = true;
                    backgroundworker_smsreply.RunWorkerAsync();
                }
            }
            if (chkbox_All.Checked)
            {
                sendmessage();
                SendSimpleMessage();
                SendSMS();
                if (!backgroundworker_smsreply.IsBusy)
                {
                    backgroundworker_smsreply.DoWork += backgroundworker_smsreply_DoWork;
                    backgroundworker_smsreply.WorkerReportsProgress = true;
                    backgroundworker_smsreply.RunWorkerAsync();
                }
            }
        }

        private void chkbox_chat_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dgv_Loadusers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_Loadusers.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                this.sendTo = dgv_Loadusers.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
        }

        public  IRestResponse SendSimpleMessage()
        {
            BinaryFormatter bf = new BinaryFormatter();
            InstantMessengerServer.UserInfo information;
            FileStream file = new FileStream(usersFileName, FileMode.Open, FileAccess.Read);
            InstantMessengerServer.UserInfo[] infos = (InstantMessengerServer.UserInfo[])bf.Deserialize(file);      // Deserialize UserInfo array
            file.Close();
            users = infos.ToDictionary((u) => u.UserName, (u) => u);  // Convert UserInfo array to Dictionary
            users.TryGetValue(this.sendTo, out information);

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            "d22ac0328841fbcb50703993cb860cab-c9270c97-256170db");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandbox183895fbd0b242ed8b21abb6247754ce.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", Username+" <mailgun@sandbox183895fbd0b242ed8b21abb6247754ce.mailgun.org>");
            request.AddParameter("to", information.EmailAddress);
            request.AddParameter("subject", "ImportantMessage");
            request.AddParameter("text", sendText.Text);
            request.Method = Method.POST;
            return client.Execute(request);
        }
        private string SendSMS()
        {
            string result = "";
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                InstantMessengerServer.UserInfo information;
                FileStream file = new FileStream(usersFileName, FileMode.Open, FileAccess.Read);
                InstantMessengerServer.UserInfo[] infos = (InstantMessengerServer.UserInfo[])bf.Deserialize(file);      // Deserialize UserInfo array
                file.Close();
                users = infos.ToDictionary((u) => u.UserName, (u) => u);  // Convert UserInfo array to Dictionary
                users.TryGetValue(this.sendTo, out information);
                if (information.MobileNo.Contains("+"))
                    information.MobileNo = information.MobileNo.Replace("+", "");

                smsreferencecount++;

                string md5hash = CalculateMD5Hash(information.MobileNo+ sendText.Text);

                //var xmlvalue = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                // "<call>\n" +
                // "<batch id=\"1469_insight_20181201\" sequence=\"1\" messageCount=\"1\">\n" +
                // "<message recipient=\"27833950738\" content=\"Hi\" checksum=\"3285f3c4b0bf8e6ffcbad61e317ebc18\"/>\n" +
                // "</batch>\n" +
                // "<commitBatch value =\"true\"/>\n" +
                // "</call>";

                var xmlvalue = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                 "<call>\n" +
                 "<batch id="+"\"1469_insight_201812"+smsreferencecount+"\""+ " sequence="+"\"1"+"\""+ " messageCount=\"1\">\n" +
                 "<message recipient=" + "\""+information.MobileNo +"\""+ " content=" +"\""+ sendText.Text +"\""+ " checksum="+ "\""+md5hash+"\""+"/>\n" +
                 "</batch>\n" +
                 "<commitBatch value =\"true\"/>\n" +
                 "</call>";

                using (var wb = new WebClient())
                {
                    byte[] response = wb.UploadValues("http://webservices.smsweb.co.za/api/incoming.php", new NameValueCollection()
                {
                {"swusername" , "insight"},
                {"swclientid" , "1469"},
                {"swpassword" , "insight123!"},
                {"batchXML" , xmlvalue},
                {"mode","live" }
                });
                    result = System.Text.Encoding.UTF8.GetString(response);
                }
            }

            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
            return result;
        }
        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
        private string SMSBalanceEnquiry()
        {
            string result = "";
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("http://webservices.smsweb.co.za/api/balance.php", new NameValueCollection()
                {
                {"swusername" , "insight"},
                {"swclientid" , "1469"},
                {"swpassword" , "insight123!"}
                });
                result = System.Text.Encoding.UTF8.GetString(response);
            }
            return result;
        }
        private string SMSReply()
        {
            string result = "";
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("http://webservices.smsweb.co.za/xmlreplies.php?", new NameValueCollection()
                {
                {"user" , "insight"},
                {"pass" , "insight123!"}
                });
                result = System.Text.Encoding.UTF8.GetString(response);
            }
            return result;
        }

        private void sendText_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void btn_groupchat_Click(object sender, EventArgs e)
        {
            Groupchat frmgroupchat = new Groupchat();
            frmgroupchat.Show();
        }
    }
}
