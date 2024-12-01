using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightCapital.STTAPI.MessageLibrary.ProtocolStructs
{
    [Serializable]
    public abstract class IProtocolStruct
    {
        //static string SpltNewStrct = "£|Ç";
        //static string[] SpltID = { "£#Ç" };
        //static string[] SpltStruct = { "£$Ç" };
        //public const string SpltNewStrct = "œ";
        //public const char SpltID = 'þ';
        //public const char SpltStruct = '®';
        public const string SpltNewStrct = "|";
        public const char SpltID = '#';
        public const char SpltStruct = '~';


        protected BinaryWriter bw_;
        protected BinaryReader br_;

        protected string[] SpltString;
        StringBuilder sbStringTosend = new StringBuilder();

        public StringBuilder SbStringTosend
        {
            get { return sbStringTosend; }
        }

        public MemoryStream ms_;
        private int initialStreamPosition_ = 0;

        public int Length { get; set; }
        public abstract int ID { get; }
        public abstract void StartWrite(byte[] buffer);
        public abstract void StartRead(byte[] buffer);

        public abstract void WriteString();
        public abstract void ReadString(string Msgbfr);

        protected void InitWrite(byte[] buffer)
        {
            // ms_ = new MemoryStream();
            ms_ = new MemoryStream(buffer);
            ms_.Seek(0, SeekOrigin.Begin);
            bw_ = new BinaryWriter(ms_);

            bw_.Write(ID);
            bw_.Write(Length); //zero initially
        }

        protected void CloseWrite()
        {
            Length = (int)(ms_.Position - initialStreamPosition_);

            bw_.Seek(initialStreamPosition_ + sizeof(int), SeekOrigin.Begin);
            bw_.Write(Length);
            bw_.Close();
            ms_.Dispose();
        }

        protected void InitRead(byte[] buffer)
        {
            //ms_ = new MemoryStream();
            ms_ = new MemoryStream(buffer);
            ms_.Seek(0, SeekOrigin.Begin);
            br_ = new BinaryReader(ms_);
        }

        protected void CloseRead()
        {
            br_.Close();
            ms_.Dispose();
        }


        protected void AppendStr(double str)
        {
            AppendStr(str.ToString());
        }
        protected void AppendStr(bool str)
        {
            AppendStr(str.ToString());
        }
        protected void AppendStr(DateTime str)
        {
            AppendStr(str.ToLongDateString());
        }

        protected void AppendStr(int str)
        {
            AppendStr(str.ToString());
        }
        protected void AppendStr(decimal str)
        {
            AppendStr(str.ToString());
        }

        protected void AppendStr(long str)
        {
            AppendStr(str.ToString());
        }
        protected void AppendStr(string str)
        {
            sbStringTosend.Append(str);
            //sbStringTosend.Append(SpltStruct);
        }
        protected void StartStrR(string Msgbfr)
        {
            SpltString = Msgbfr.Split(SpltStruct);
        }
        protected void StartStrW()
        {
            sbStringTosend.Append(ID);
            //sbStringTosend.Append(SpltID);
        }
        protected void EndStrW()
        {
            sbStringTosend.Append(SpltNewStrct);
        }


    }
}