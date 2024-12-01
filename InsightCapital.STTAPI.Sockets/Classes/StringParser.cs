using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;

namespace InsightCapital.STTAPI.Sockets.Classes
{
    public class StringParser : IParser
    {


        public string strBuffer, DataReceived;
        public Dictionary<int, Type> Structs;
        public event ProtocolReaderHandler PSRead = delegate { };

        Queue InQue = Queue.Synchronized(new Queue());
        Thread QProcessrIn;
        bool flagQProcessrIn = false;
        ManualResetEvent mreIn = new ManualResetEvent(false);
        //string Message;
        public StringParser()
        {
            InitQProcessrIn();
        }

        public class StringParserException : Exception
        {
            public StringParserException(string message)
                : base(message)
            {

            }
        }

        void mreInSet()
        {
            if (flagQProcessrIn)
            {
                InitQProcessrIn();
            }
            mreIn.Set();
        }

        private void InitQProcessrIn()
        {
            QProcessrIn = new Thread(new ThreadStart(ProcessDataReceived));
            QProcessrIn.IsBackground = false;
            //QProcessrIn.Priority = ThreadPriority.Highest;
            QProcessrIn.Name = "FromClient";
            QProcessrIn.Start();
        }


        public void Parse()
        {
            //if (string.IsNullOrEmpty(Message))
            //{
            //    return;
            //}
            //strBuffer += Message;
            ////strBuffer = strBuffer.Replace("\0", "");
            //System.Diagnostics.Debug.WriteLine(Message);
            //System.Diagnostics.Debug.WriteLine(strBuffer);
            //while (strBuffer.IndexOf(IProtocolStruct.SpltNewStrct) > 0)
            //{
            //    DataReceived = strBuffer.Substring(0, strBuffer.IndexOf(IProtocolStruct.SpltNewStrct));
            //    strBuffer = strBuffer.Substring(DataReceived.Length + IProtocolStruct.SpltNewStrct.Length);

            //    lock (InQue.SyncRoot)
            //    {
            //        InQue.Enqueue(DataReceived);
            //    }
            //    mreInSet();
            //}
        }

        void ProcessDataReceived()
        {
            try
            {
                while (true)
                {
                    mreIn.WaitOne(Timeout.Infinite, false);
                    while (InQue.Count > 0)
                    {
                        string stData = null;
                        lock (InQue.SyncRoot)
                        {
                            stData = InQue.Dequeue() as string;
                        }

                        string[] stId = stData.Split(IProtocolStruct.SpltID);
                        int structId = 0;
                        if (stId.Length > 1)
                        {
                            IProtocolStruct currentStruct = null;
                            structId = Convert.ToInt32(stId[0]);
                            Type currentStructType;
                            if (Structs.TryGetValue(structId, out currentStructType))
                            {
                                currentStruct = (IProtocolStruct)Activator.CreateInstance(currentStructType);
                                if (currentStruct != null)
                                {
                                    currentStruct.ReadString(stId[1]);
                                    PSRead(this, currentStruct);
                                }
                            }
                        }
                    }
                    mreIn.Reset();
                    Thread.Sleep(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            flagQProcessrIn = true;
        }



        #region IParser Members



        public void Read(byte[] data, int len)
        {
            string Message = Encoding.ASCII.GetString(data, 0, len);
            if (string.IsNullOrEmpty(Message))
            {
                return;
            }
            strBuffer += Message;
            //strBuffer = strBuffer.Replace("\0", "");
            System.Diagnostics.Debug.WriteLine(Message);
            System.Diagnostics.Debug.WriteLine(strBuffer);
            while (strBuffer.IndexOf(IProtocolStruct.SpltNewStrct) > 0)
            {
                DataReceived = strBuffer.Substring(0, strBuffer.IndexOf(IProtocolStruct.SpltNewStrct));
                strBuffer = strBuffer.Substring(DataReceived.Length + IProtocolStruct.SpltNewStrct.Length);

                lock (InQue.SyncRoot)
                {
                    InQue.Enqueue(DataReceived);
                }
                mreInSet();
            }
        }
        #endregion

        #region IParser Members


        public Dictionary<int, Type> Structs_
        {
            get
            {
                return Structs;
            }
            set
            {
                Structs = value;
            }
        }

        #endregion
    }
}
