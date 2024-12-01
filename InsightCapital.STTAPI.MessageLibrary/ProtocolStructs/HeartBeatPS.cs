using System;


namespace InsightCapital.STTAPI.MessageLibrary.ProtocolStructs
{

    public class HeartBeatPS : IProtocolStruct
    {
        public DateTime PingingTime_ = DateTime.UtcNow;

        public override int ID
        {
            get { return ProtocolStructIDS.HeartBeat_ID; }
        }

        public override void StartWrite(byte[] buffer)
        {
            InitWrite(buffer);
            bw_.Write(PingingTime_.ToBinary());
            CloseWrite();
        }

        public override void StartRead(byte[] buffer)
        {
            try
            {
                InitRead(buffer);
                PingingTime_ = DateTime.FromBinary(br_.ReadInt64());
                CloseRead();
            }
            catch
            {
                PingingTime_ = DateTime.UtcNow;
            }
        }

        public override void ReadString(string Msgbfr)
        {
            StartStrR(Msgbfr);
            //PingingTime_ = clsUtility.GetDate(SpltString[0]);
            //SpltString = Msgbfr.Split(Utility.SpltStruct);
            //PingingTime_ = clsUtility.GetDate(SpltString[0]);
        }

        public override void WriteString()
        {
            StartStrW();
            AppendStr(PingingTime_);
            EndStrW();
            //sbStringTosend.Append(ID);
            //sbStringTosend.Append(Utility.SpltID);
            //sbStringTosend.Append(PingingTime_.ToString());
            //sbStringTosend.Append(Utility.SpltStruct);
            //
            //
        }
    }
}
