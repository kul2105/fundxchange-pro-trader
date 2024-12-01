using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;
using InsightCapital.STTAPI.MessageLibrary.Utility;

namespace InsightCapital.STTAPI.Sockets.Classes
{
    public delegate void ProtocolReaderHandler(IParser sender, IProtocolStruct structure);

    public class ProtocolParser : IParser
    {
        public class ProtocolParserException : Exception
        {
            public ProtocolParserException(string message)
                : base(message)
            {
            }
        }

        public event ProtocolReaderHandler PSRead = delegate { };

        private readonly MemoryStream stream_ = new MemoryStream();
        Dictionary<int, Type> Structs;
        private readonly byte[] buffer_ = new byte[65535];

        public void Read(byte[] data, int length)
        {
            lock (stream_)
            {
                stream_.Write(data, 0, length);
            }
        }

        public void Parse()
        {
            try
            {
                lock (stream_)
                {
                    int step = 0;
                    int structLength = -1;
                    IProtocolStruct currentStruct = null;
                    stream_.Position = 0;

                    while (stream_.Position + 8 <= stream_.Length)
                    {
                        if (step == 0)
                        {
                            stream_.Read(buffer_, 0, 8);
                            int structId = BitConverter.ToInt32(buffer_, 0);

                            Type currentStructType;
                            if (!Structs.TryGetValue(structId, out currentStructType))
                            {
                                stream_.SetLength(0);
                                stream_.Seek(0, SeekOrigin.Begin);
                                return;
                            }

                            currentStruct = (IProtocolStruct)Activator.CreateInstance(currentStructType);

                            //read struct Length
                            structLength = BitConverter.ToInt32(buffer_, 4);

                            step = 1;
                            continue;
                        }
                        if (step == 1)
                        {
                            if (currentStruct == null || structLength == -1)
                                throw new ProtocolParserException("Got to read the structure, but not created.");
                            if (structLength + stream_.Position <= stream_.Length)
                            {
                                stream_.Read(buffer_, 0, structLength);
                                currentStruct.StartRead(buffer_);
                                PSRead(this, currentStruct);
                                step = 0;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (step == 1)
                        stream_.Position -= 8;

                    int shiftBufferLength = (int)(stream_.Length - stream_.Position);
                    if (shiftBufferLength > 0)
                    {
                        stream_.Read(buffer_, 0, shiftBufferLength);
                        stream_.SetLength(0);
                        stream_.Seek(0, SeekOrigin.Begin);
                        stream_.Write(buffer_, 0, shiftBufferLength);
                    }
                    else
                    {
                        stream_.SetLength(0);
                        stream_.Seek(0, SeekOrigin.Begin);
                    }
                }
            }
            catch (Exception ex)
            {
                stream_.SetLength(0);
                stream_.Seek(0, SeekOrigin.Begin);

                FileHandling.WriteToLogEx(ex.Message + "@@@@Stack Trace :-   " + ex.StackTrace);
                throw ex;
            }
        }
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
