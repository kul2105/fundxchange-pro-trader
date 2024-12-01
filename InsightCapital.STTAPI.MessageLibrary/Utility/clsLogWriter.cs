using InsightCapital.STTAPI.MessageLibrary;
using System;
using System.IO;
using System.Reflection;

namespace InsightCapital.STTAPI.MessageLibrary.Utility
{
    public class clsLogWriter
    {
        private clsLogWriter()
        {

        }
        // Singleton instance
        private static clsLogWriter _LogInstance;
        // Singleton exposed instance
        public static clsLogWriter LogInstance
        {
            get { return _LogInstance ?? (_LogInstance = new clsLogWriter()); }
        }

        // lock to restrict concurrent access
        private object LockObject
        {
            get { return _lockObject ?? (_lockObject = new object()); }
        }

        private object _lockObject;

        public void Log(string msg, MITCHMessageType messageType)
        {
            lock (LockObject)  // all other threads will wait for y
            {
                FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                string logPath = fileInfo.DirectoryName + "\\Logs\\UDPMItchFeed\\";
                string _path = logPath + "Log_" + DateTime.Now.ToString("dd_MM_yyyy");
                string FileName = _path + "\\" + messageType.ToString() + ".txt";

                if (File.Exists(FileName))
                {
                    try
                    {
                        TextWriter tsw = new StreamWriter(FileName, true);
                        tsw.WriteLine(DateTime.Now.ToString() + " : " + msg + System.Environment.NewLine);
                        tsw.Close();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    System.IO.Directory.CreateDirectory(_path);
                    FileStream fsDevelopmentLog = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write,
                                                      FileShare.ReadWrite);
                    fsDevelopmentLog.Close();
                    try
                    {
                        TextWriter tsw = new StreamWriter(FileName, true);
                        tsw.WriteLine(DateTime.Now.ToString() + " : " + msg + System.Environment.NewLine);
                        tsw.Close();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
