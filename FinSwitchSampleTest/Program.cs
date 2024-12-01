using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FinSwitchSampleTest.FinSwitchUAT;
//using FinSwitchService;

namespace FinSwitchSampleTest
{
    class Program
    {
        private const string _url = "https://Finswitch2.FinSwitch.com/webservices/FinSwitchwebservice.asmx";
        private const string login = "FundX@123";
        private const string passwd = "1Trader1";
        private const string CompanyCode = "FundX";
        static FinSwitchWebService service;

        static void Main(string[] args)
        {
            service = new FinSwitchWebService();
            //UploadFile(@"C:\Users\Kuldeep\Desktop\LatestDoc 22Jul18\FinSwitch_LINX_Path\Test.txt");
            //C:\Users\Kuldeep\Desktop\LatestDoc 22Jul18\FinSwitch Docs
            //importdatafromexcel(@"C:\Users\Kuldeep\Desktop\LatestDoc 22Jul18\cisfNew.xlsx");
            ImportTextFile(@"C:\Users\Kuldeep\Desktop\LatestDoc 22Jul18\cisfNewTAB.txt");
            //FundStaticDownload();
            //PDCustomeDownload();
            //FPDCustomeDownload();
        }

        private static void FPDCustomeDownload()
        {
            FPDDownloadOptions fpd = new FPDDownloadOptions();
            fpd.Login = login;
            fpd.Password = passwd;
            fpd.CompanyCode = CompanyCode;
            fpd.DateKind = DateSwitch.CycleDate;
            fpd.FromDate = new DateTime(2019, 03, 01);
            fpd.ToDate = new DateTime(2019, 03, 31);
            fpd.MancoCodes = GetAllMancos();
            fpd.StatusOption = PriceDistributionDownloadStatusOption.AllStatus;

            CustomDownloadResult res= service.FPDCustomDownload(fpd);
            string result= GetProcesResult(res.ProcessLogID);
        }

        private static string[] GetAllMancos()
        {
            return null;
            //List<string> lst = new List<string>();
            //try
            //{
            //    lock (SP_GetAllMancoCodes)
            //    {
            //        ISingleResult<SP_GetAllMancoCodesResult> ret = SP_GetAllMancoCodes.SP_GetAllMancoCodes();
            //        int sp_retval = GetInt(ret.ReturnValue);

            //        if (sp_retval == -555)//Success
            //        {
            //            foreach (var item in ret.ToList())
            //            {
            //                lst.Add(item.FinSwitchMancoCode);
            //            }

            //            return lst.ToArray();
            //        }
            //        else if (sp_retval == -999)//DB Failed
            //        {
            //            //throw new SPExchangeException(sp_MUK_AurumInsertOHLC);
            //        }
            //        return new string[0];
            //    }

            //}
            //catch (Exception ex)
            //{
            //    LogManager._instance.syncLog("Exception in GetAllMancos() :" + ex.Message + "   StackTrace : " + ex.StackTrace);
            //    return null;
            //}
        }

        private static string GetProcesResult(int processLogID)
        {
            return service.GetFileDownload(login, passwd, processLogID);
        }

        private static void PDCustomeDownload()
        {
            PDDownloadOptions pd = new PDDownloadOptions();
            pd.Login = login;
            pd.Password = passwd;
            pd.CompanyCode = CompanyCode;
            pd.DateKind = DateSwitch.CycleDate;
            pd.FromDate = new DateTime(2019, 03, 01);
            pd.ToDate = new DateTime(2019, 03, 31);
            //pd.Funds = empty;
            pd.FundTypes =  new string[] { "AN", "AS", "AY", "BP", "CP", "CS", "FS", "IC", "LP", "NP", "PP", "PR", "TN", "TS", "TY"};
            pd.IncludeISINNumber = true;
            pd.MancoCodes = GetAllMancos();
            pd.PriceTypes = new string[] { "NP", "CP", "IC" };
            pd.StatusOption = PriceDistributionDownloadStatusOption.AllStatus;
            CustomDownloadResult res = service.PDCustomDownload(pd);
            string result = GetProcesResult(res.ProcessLogID);
        }

        private static void ImportTextFile(string path)
        {
            string line = File.ReadAllText(path);
            List<string> listStrLineElements = line.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            List<string> rowList = listStrLineElements.SelectMany(s => s.Split('\t')).ToList();

            //Data Source=197.242.148.230;User ID=sa;Password=admin123@
            string ssqlconnectionstring = "Data Source=CHAUHAN;Initial Catalog=FinSwitch;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";
            //string ssqlconnectionstring = "Data Source=197.242.148.230;User ID=sa;Password=admin123@;Initial Catalog=MaitlandReports;Integrated Security=false;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";
            SqlConnection myConnection = new SqlConnection(ssqlconnectionstring);
            try
            {
                myConnection.Open();
                for (int i = 9; i <= rowList.Count; i+=10)//Implement by 3...
                {
                    //Replace table_name with your table name, and Column1 with your column names (replace for all).
                    SqlCommand myCommand = new SqlCommand("INSERT INTO CISFunds (FundCode, FundName, SectorCode,SectorName,FundType,FoF,ThirdPartyFund,ThirdPartyManager) " +
                                         String.Format("Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", rowList[i + 1], rowList[i + 2], rowList[i + 3], rowList[i + 4], rowList[i + 5], rowList[i + 6], rowList[i + 7], rowList[i + 8]), myConnection);
                    myCommand.ExecuteNonQuery();
                }

            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            try { myConnection.Close(); }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

        }

        private static void FundStaticDownload()
        {            
            DateTime dateTime = new DateTime(2008, 01, 01);
            string Date = dateTime.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime dt = DateTime.Parse(Date);
            string[] str = new string[0];//{ "ADIFA", "ADIBF", "ADIB1" };

            string manco = string.Empty;//"AAMCI";
                                        //string[] str = new string[0];

            //string res = ccc.FundAccountFileDownload(login, passwd, CompanyCode, "", "", "", CompanyCode, "", true);
            //int x = ccc.ProductStaticDataDownload(login, passwd, CompanyCode, true, CompanyCode);
            //string prodData = ccc.GetFileDownload(login, passwd, x);

            int abc = service.FundStaticDataDownload(login, passwd, CompanyCode, dt, true, manco, true, str);
            string fundData = service.GetFileDownload(login, passwd, abc);

            string path = "E:\\Projects Only\\Pro_Trader\\fundxchange-pro-trader\\bin\\x86\\Debug\\Res\\AllFundsDetails.txt";
            //if (!File.Exists(path))
            //    File.Create(path);
            File.WriteAllText(path, fundData);
            
        }

        public static void UploadFile(string path)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            string fname = Path.GetFileName(path);
            var ccc = new FinSwitchWebService();
           
            string Date = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
           
            int processId = ccc.UploadFile(login, passwd, Path.GetFileName(path), "text/plain", bytes, CompanyCode);
            string response = ccc.GetProcessStatus(login, passwd, processId);

            string res = ccc.DownloadFileAsString(login, passwd, "TR", DateTime.Parse("18-03-2019"), false, CompanyCode, "0");

            //string res = ccc.TRPreviewDownloadAsString(login, passwd, CompanyCode, processId);
            //string res = ccc.TRCustomDownload(login, passwd, CompanyCode, processId);

            //ccc.()
        }

        public static void importdatafromexcel(string excelfilepath)
        {
            //declare variables - edit these based on your particular situation
            string ssqltable = "CISFunds";
            // make sure your sheet name is correct, here sheet name is sheet1, so you can change your sheet name if have
            //different
            string myexceldataquery = "select * from [Sheet1$]";
            try
            {

                //create our connection strings
                //string sexcelconnectionstring = @"provider=microsoft.jet.oledb.4.0;data source=" + excelfilepath +
                //";extended properties=" + "\"excel 8.0;hdr=yes;\"";
                string sexcelconnectionstring = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=yes;IMEX=1'", excelfilepath);

               // string sexcelconnectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;" + excelfilepath + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\";";

                // string sexcelconnectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + excelfilepath + ";Extended Properties=Excel 12.0;";
                string ssqlconnectionstring = "Data Source=CHAUHAN;Initial Catalog=FinSwitch;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";
                
                //execute a query to erase any previous data from our destination table
                string sclearsql = "delete from " + ssqltable;
                SqlConnection sqlconn = new SqlConnection(ssqlconnectionstring);
                SqlCommand sqlcmd = new SqlCommand(sclearsql, sqlconn);
                sqlconn.Open();
                sqlcmd.ExecuteNonQuery();
                sqlconn.Close();
                //series of commands to bulk copy data from the excel file into our sql table
                OleDbConnection oledbconn = new OleDbConnection(sexcelconnectionstring);
                OleDbCommand oledbcmd = new OleDbCommand(myexceldataquery, oledbconn);
                oledbconn.Open();
                OleDbDataReader dr = oledbcmd.ExecuteReader();
                SqlBulkCopy bulkcopy = new SqlBulkCopy(ssqlconnectionstring);
                bulkcopy.DestinationTableName = ssqltable;
                while (dr.Read())
                {
                    bulkcopy.WriteToServer(dr);
                }

                oledbconn.Close();
            }
            catch (Exception ex)
            {
                //handle exception
            }
        }
    }
}
