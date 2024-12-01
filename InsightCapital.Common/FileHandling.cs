//#define UI
using System;
using System.IO;

namespace InsightCapital.Common
{
    public class FileHandling
    {
        static bool LoggingEnable = true;
        //static string delemeter = ",";

        static string strInsert;
        static string strUpdate;
        static string strLog;
        static string strLogEx;
        static string strLog4OrderIn;
        static string strLog4OrderOut;
        static string str4Debug;

        static object obj = new object();
        //static FileMode fm = FileMode.OpenOrCreate;//.Append;
        static FileMode fm = FileMode.Append;
        static object lckInsert = new object();
        static object lckUpdate = new object();
        static object lckLog = new object();
        static object lckLogEx = new object();
        static object lckOrderIn = new object();
        static object lckOrderOut = new object();
        static object lck4Debug = new object();

        static FileStream fileInsert;
        static FileStream fileUpdate;
        static FileStream fileLog;
        static FileStream fileLogEx;
        static FileStream fileLog4OrderIn;
        static FileStream fileLog4OrderOut;
        static FileStream file4Debug;

        static StreamWriter swInsert;
        static StreamWriter swUpdate;
        static StreamWriter swLog;
        static StreamWriter swLogEx;
        static StreamWriter swLog4OrderIn;
        static StreamWriter swLog4OrderOut;
        static StreamWriter sw4Debug;

        public static string FullpathofLog;
        public static string LogDirName;
        static string strLogFileStart = "Log_";

        static FileHandling()
        {

            string path = Directory.GetCurrentDirectory();

            Random rnd = new Random();
            DeleteOldLogs(path);

            string path2Move = strLogFileStart + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond;// +"_" + Guid.NewGuid();
            //if (Directory.Exists(path + "_" + path2Move))
            //{
            //    //Directory.Move(path + "\\Logs", path2Move);
            //}
            //else
            {
                Directory.CreateDirectory(path + "\\" + path2Move);
            }

            LogDirName = path2Move;
            FullpathofLog = path + "\\" + path2Move;

            strInsert = FullpathofLog + @"/LogInsert.txt";
            strUpdate = FullpathofLog + @"/LogUpdate.txt";
            strLog = FullpathofLog + @"/Log.txt";
            strLogEx = FullpathofLog + @"/LogEx.txt";
            strLog4OrderIn = FullpathofLog + @"/LogOrderIn.txt";
            strLog4OrderOut = FullpathofLog + @"/LogOrderOut.txt";
            str4Debug = FullpathofLog + @"/4Debug.txt";

            //strInsert =FullpathofLog+ path2Move + @"/LogInsert.txt";
            //strUpdate = FullpathofLog  +path2Move + @"/LogUpdate.txt";
            //strLog = FullpathofLog + path2Move + @"/Log.txt";
            //strLogEx = FullpathofLog + path2Move + @"/LogEx.txt";
            //strLog4OrderIn = FullpathofLog + path2Move + @"/LogOrderIn.txt";
            //strLog4OrderOut = FullpathofLog + path2Move + @"/LogOrderOut.txt";
            bool flag = false;
            while (!flag)
            {
                if (File.Exists(strInsert) || File.Exists(strUpdate) || File.Exists(strLog) || File.Exists(strLogEx) || File.Exists(strLog4OrderIn) || File.Exists(strLog4OrderOut))
                {
                    string k = rnd.Next().ToString();
                    strInsert += k.ToString();
                    strUpdate += k.ToString();
                    strLog += k.ToString();
                    strLogEx += k.ToString();
                    strLog4OrderIn += k.ToString();
                    strLog4OrderOut += k.ToString();
                    str4Debug += k.ToString();
                    clsUtility.WriteLineColor("Found Duplicate File For Logging", ConsoleColor.Red);
                }
                else
                {
                    flag = true;
                }
            }
            //if (fileInsert.CanWrite && fileUpdate.CanWrite && fileLog.CanWrite && fileLogEx.CanWrite && fileLog4OrderIn.CanWrite && fileLog4OrderOut.CanWrite)
            {
                fileInsert = new FileStream(strInsert, fm, FileAccess.Write);
                fileUpdate = new FileStream(strUpdate, fm, FileAccess.Write);
                fileLog = new FileStream(strLog, fm, FileAccess.Write);
                fileLogEx = new FileStream(strLogEx, fm, FileAccess.Write);
                fileLog4OrderIn = new FileStream(strLog4OrderIn, fm, FileAccess.Write);
                fileLog4OrderOut = new FileStream(strLog4OrderOut, fm, FileAccess.Write);
                file4Debug = new FileStream(str4Debug, fm, FileAccess.Write);

                swInsert = new StreamWriter(fileInsert);
                swUpdate = new StreamWriter(fileUpdate);
                swLog = new StreamWriter(fileLog);
                swLogEx = new StreamWriter(fileLogEx);
                swLog4OrderIn = new StreamWriter(fileLog4OrderIn);
                swLog4OrderOut = new StreamWriter(fileLog4OrderOut);
                sw4Debug = new StreamWriter(file4Debug);

                swInsert.AutoFlush = true;
                swUpdate.AutoFlush = true;
                swLog.AutoFlush = true;
                swLogEx.AutoFlush = true;
                swLog4OrderIn.AutoFlush = true;
                swLog4OrderOut.AutoFlush = true;
                sw4Debug.AutoFlush = true;
            }
            //else
            //{
            //    string strMsg = "fileInsert-->" + fileInsert.CanWrite + "fileUpdate-->" + fileUpdate.CanWrite + "fileLog-->" + fileLog.CanWrite + "fileLogEx-->" + fileLogEx.CanWrite + "fileLog4OrderIn-->" + fileLog4OrderIn.CanWrite + "fileLog4OrderOut-->" + fileLog4OrderOut.CanWrite;
            //    throw new Exception("Dont Have Write Permission On File" + strMsg);
            //}            
        }

        private static void DeleteOldLogs(string path)
        {
            try
            {
                string[] LogFiles = Directory.GetDirectories(path);
                int length = LogFiles.Length;
                LogFiles = SortArray(LogFiles);
                //Array.Sort(LogFiles);

                int cnt = LogFiles.Length - 2;
                for (int i = 0; i < cnt; i++)
                {
                    if (LogFiles[i].StartsWith(path + "\\" + strLogFileStart))
                    {
                        Directory.Delete(LogFiles[i], true);
                    }
                }
            }
            catch (Exception)
            {
                //throw ex
            }
        }

        public static string[] SortArray(string[] Array4Sort)
        {
            for (int pass = 0; pass < Array4Sort.Length; pass++)
            {
                for (int j = 0; j < Array4Sort.Length - 1; j++)
                {
                    if (Directory.GetCreationTime(Array4Sort[j]) > Directory.GetCreationTime(Array4Sort[j + 1]))
                    {
                        string Swap;
                        Swap = Array4Sort[j];
                        Array4Sort[j] = Array4Sort[j + 1];
                        Array4Sort[j + 1] = Swap;
                    }
                }
            }
            return Array4Sort;
        }


        public static void CloseFile()
        {
            try
            {
                lock (lckLog)
                {
                    swLog.Close();
                }
                lock (lckInsert)
                {
                    swInsert.Close();
                }
                lock (lckOrderIn)
                {
                    swLog4OrderIn.Close();
                }
                lock (lckOrderOut)
                {
                    swLog4OrderOut.Close();
                }
                lock (lck4Debug)
                {
                    sw4Debug.Close();
                }
            }
            catch (Exception)
            {

            }

        }
        public static void WriteToLog4OrderIn(string str)
        {
            lock (lckOrderIn)
            {
                //try
                {
                    if (LoggingEnable)
                    {
                        swLog4OrderIn.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss:fff") + "___________" + str);
                    }
                }
                //catch (Exception)
                {

                }

            }
        }
        public static void WriteToLog4OrderOut(string str)
        {
            lock (lckOrderOut)
            {
                //try
                {
                    if (LoggingEnable)
                    {
                        swLog4OrderOut.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss:fff") + "___________" + str);
                    }
                }
                //catch (Exception)
                {

                }

            }
        }
        public static void WriteToLog(string str)
        {
            lock (lckLog)
            {
                //try
                {
                    if (LoggingEnable)
                    {
                        swLog.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss:fff") + "___________" + str);
                    }
                }
                //catch (Exception)
                {

                }

            }
        }
        public static bool WriteToInsert(string str)
        {
            lock (lckInsert)
            {
                //try
                {
                    if (LoggingEnable)
                    {
                        swInsert.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss:fff") + "___________" + str);
                    }
                }
                //catch (Exception)
                {

                }
            }
            return true;
        }
        public static bool WriteToUpdate(string str)
        {
            lock (lckUpdate)
            {
                //try
                {
                    if (LoggingEnable)
                    {
                        swUpdate.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss:fff") + "___________" + str);
                    }
                }
                //catch (Exception)
                {

                }

            }
            return true;
        }
        public static bool WriteTo4Debug(string str)
        {
            lock (lck4Debug)
            {
                //try
                {
                    if (LoggingEnable)
                    {
                        sw4Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss:fff") + "___________" + str);
                    }
                }
                //catch (Exception)
                {

                }
            }
            return true;
        }
        public static bool WriteToLogEx(string str)
        {
            lock (lckLogEx)
            {
                //try
                {
                    if (LoggingEnable)
                    {
                        swLogEx.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss:fff") + "___________" + str);
                    }
                }
                //catch (Exception)
                {

                }
            }
            return true;
        }
    }
    //public class DBAccess
    //{
    //    static string DataBaseName = "OMEdatabase";
    //    static ConnectionAgent ca;// = new ConnectionAgent("(local)", "", "", "Aurum");
    //    static DataManager dm = null;
    //    static object INSERTNEWORDER = new object();
    //    static object UPDATEONTRADE = new object();
    //    static object UPDATEONORDERSTATUSREPORT = new object();
    //    static object REMOVEEXPIREDORDER = new object();
    //    static object GETORDER4MDB = new object();

    //    static DBAccess()
    //    {
    //         //ca = new ConnectionAgent("(local)", "", "", DataBaseName);
    //        ca = new ConnectionAgent("192.168.1.150", "sa", "admin123@", DataBaseName);
    //        dm = new DataManager("SQL", ca);
    //    }
    //    public static string InsertNewOrder(Order order)
    //    {
    //       // return "True";
    //        lock (INSERTNEWORDER)
    //        {
    //            try
    //            {
    //                List<OleDbParameter> _struct = new List<OleDbParameter>();

    //                _struct.Add(dm.CreateParam("Instrument", OleDbType.VarChar, order.Instrument));
    //                _struct.Add(dm.CreateParam("Side", OleDbType.Integer, order.BuySell));
    //                _struct.Add(dm.CreateParam("OrderType", OleDbType.Integer, order.OrderType));
    //                _struct.Add(dm.CreateParam("Price", OleDbType.Decimal, order.Price));
    //                _struct.Add(dm.CreateParam("SL", OleDbType.Decimal, order.SL));
    //                _struct.Add(dm.CreateParam("TP", OleDbType.Decimal, order.TP));
    //                _struct.Add(dm.CreateParam("Quantity", OleDbType.BigInt, order.Quantity));
    //                _struct.Add(dm.CreateParam("ClOrderID", OleDbType.VarChar, order.ClientOrderID.ToString()));
    //                _struct.Add(dm.CreateParam("RequestTime", OleDbType.Date, order.TimeStamp));
    //                _struct.Add(dm.CreateParam("OrderStatus", OleDbType.Integer, order.orderStatus));
    //                _struct.Add(dm.CreateParam("GlobalOrderID", OleDbType.BigInt, order.ServerOrderID));
    //                _struct.Add(dm.CreateParam("TIF", OleDbType.Integer, order.TimeInForce));
    //                _struct.Add(dm.CreateParam("GTD", OleDbType.Date, order.GTD));

    //                return dm.InsertDataThroughProc("OMEdatabase", _struct, "PROC_Insertion");

    //            }
    //            catch (Exception ex)
    //            {
    //                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
    //                Debug.WriteLine(ex.Message);
    //                return ex.Message;
    //            }
    //        }
    //    }
    //    public static string UpdateOnTrade(Trade trade)
    //    {
    //      //  return "True";
    //        lock (UPDATEONTRADE)
    //        {
    //            try
    //            {
    //                List<OleDbParameter> _struct = new List<OleDbParameter>();
    //                _struct.Add(dm.CreateParam("TradeID", OleDbType.BigInt, trade.TradeID));
    //                _struct.Add(dm.CreateParam("Instrument", OleDbType.VarChar, trade.Instrument));
    //                _struct.Add(dm.CreateParam("Price", OleDbType.Decimal, trade.Price));
    //                _struct.Add(dm.CreateParam("Quantity", OleDbType.BigInt, trade.Quantity));
    //                _struct.Add(dm.CreateParam("BuyOrderID", OleDbType.BigInt, trade.BuyServerOrderID));
    //                _struct.Add(dm.CreateParam("SellOrderID", OleDbType.BigInt, trade.SellServerOrderID));

    //                return dm.InsertDataThroughProc("OMEdatabase", _struct, "Proc_Trade");
    //            }
    //            catch (Exception ex)
    //            {
    //                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
    //                Debug.WriteLine(ex.Message);
    //                return ex.Message;
    //            }
    //        }
    //    }
    //    public static string UpdateOnOrderStatusReport(OrderStatusReport report)
    //    {
    //      //  return "True";
    //        lock (UPDATEONORDERSTATUSREPORT)
    //        {
    //            try
    //            {
    //                List<OleDbParameter> _struct = new List<OleDbParameter>();
    //                _struct.Add(dm.CreateParam("OrderID", OleDbType.BigInt, report.ServerOrderID));
    //                _struct.Add(dm.CreateParam("Reason", OleDbType.VarChar, report.Reason.ToString()));
    //                _struct.Add(dm.CreateParam("Status", OleDbType.Integer, report.orderStatus));
    //                _struct.Add(dm.CreateParam("Price", OleDbType.Decimal, report.Price));
    //                _struct.Add(dm.CreateParam("SL", OleDbType.Decimal, report.SL));
    //                _struct.Add(dm.CreateParam("TP", OleDbType.Decimal, report.TP));
    //                _struct.Add(dm.CreateParam("OrderType", OleDbType.Integer, report.OrderType));
    //                _struct.Add(dm.CreateParam("ReportID", OleDbType.BigInt, report.ExecID));

    //                return dm.InsertDataThroughProc("OMEdatabase", _struct, "Proc_OrderStatus");
    //            }
    //            catch (Exception ex)
    //            {
    //                FileHandling.WriteToLogEx("Exception:"+ex.Message+":"+ex.StackTrace);
    //                Debug.WriteLine(ex.Message);
    //                return ex.Message;
    //            }
    //        }
    //    }
    //    public static string RemoveExpiredOrder(TIF tif, DateTime dt)
    //    {
    //       // return "True";
    //        lock (REMOVEEXPIREDORDER)
    //        {
    //            try
    //            {
    //                List<OleDbParameter> _struct = new List<OleDbParameter>();
    //                _struct.Add(dm.CreateParam("TIF", OleDbType.Integer, tif));
    //                _struct.Add(dm.CreateParam("StartDate", OleDbType.Integer, dt));

    //                return dm.InsertDataThroughProc("OMEdatabase", _struct, "Proc_FireTIF");

    //            }
    //            catch (Exception ex)
    //            {
    //                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
    //                Debug.WriteLine(ex.Message);
    //                return ex.Message;
    //            }
    //        }
    //    }
    //    public static DataSet getOrder4mDB(string instrument, OrderType type, Side side)
    //    {
    //        //return null;
    //        lock (GETORDER4MDB)
    //        {
    //            try
    //            {
    //                List<OleDbParameter> _struct = new List<OleDbParameter>();
    //                _struct.Add(dm.CreateParam("Instrument", OleDbType.VarChar, instrument));
    //                _struct.Add(dm.CreateParam("orderType", OleDbType.Integer, type));
    //                _struct.Add(dm.CreateParam("side", OleDbType.Integer, side));
    //               // _struct[0] = new OleDbParameter("?Instrument", OleDbType.VarChar);
    //               // _struct[0].Value = instrument;
    //              //  _struct[1] = new OleDbParameter("?orderType", OleDbType.Integer);
    //               // _struct[1].Value = type;
    //               // _struct[2] = new OleDbParameter("?side", OleDbType.Integer);
    //              //  _struct[2].Value = side;

    //                //  return dm.InsertDataThroughProc("OMEdatabase",_struct, "Proc_FireTIF");
    //                return dm.GetDataThroughProc("OMEdatabase", _struct, "Proc_getOrder");
    //            }
    //            catch (Exception ex)
    //            {
    //                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
    //                Debug.WriteLine(ex.Message);
    //                return null;
    //            }
    //        }
    //    }
    //    public static DataSet getLastIDs()
    //    {
    //        OleDbParameter[] oldb = new OleDbParameter[0];
    //        return dm.GetDataThroughProc("OMEdatabase", oldb, "getLastInfo");
    //    }
    //}

}

