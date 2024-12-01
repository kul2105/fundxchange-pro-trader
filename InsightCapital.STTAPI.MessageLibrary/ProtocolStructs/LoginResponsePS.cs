using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;
using InsightCapital.STTAPI.MessageLibrary;
using System;

namespace InsightCapital.STTAPI.MessageLibrary.ProtocolStructs
{
    public class LoginResponsePS : IProtocolStruct
    {
        public LoginResponse LoginResponse_ = new LoginResponse();

        public override int ID
        {
            get { return ProtocolStructIDS.Login_ResponsePS_ID; }
        }

        public override void StartWrite(byte[] buffer)
        {
            InitWrite(buffer);
            bw_.Write(LoginResponse_.PingingTimeInterval);
            bw_.Write(LoginResponse_.HeartBeatToleranceLevel);
            bw_.Write(LoginResponse_.AuthenticationID);
            bw_.Write(LoginResponse_.AuthenticationStatus.ToString());
            CloseWrite();
        }

        public override void StartRead(byte[] buffer)
        {
            InitRead(buffer);
            LoginResponse_.PingingTimeInterval = br_.ReadInt32();
            LoginResponse_.HeartBeatToleranceLevel = br_.ReadInt32();
            LoginResponse_.AuthenticationID = br_.ReadString();
            LoginResponse_.AuthenticationStatus = (Authentication)Enum.Parse(typeof(Authentication), br_.ReadString(), true);
            CloseRead();
        }
        public override void ReadString(string Msgbfr)
        {
            StartStrR(Msgbfr);

        }
        public override void WriteString()
        {
            StartStrW();

        }
    }

    public class LoginResponse
    {
        public int PingingTimeInterval;
        public int HeartBeatToleranceLevel;
        public string AuthenticationID;
        public Authentication AuthenticationStatus;

        public LoginResponse()
        {
            AuthenticationID = string.Empty;
            PingingTimeInterval = 11000;
            HeartBeatToleranceLevel = 13000;
            AuthenticationStatus = Authentication.Declined;
        }

        public override string ToString()
        {
            return "AuthenticationID = " + AuthenticationID + "PingingTimeInterval = " + PingingTimeInterval + "HeartBeatToleranceLevel = " + HeartBeatToleranceLevel
                 + "AuthenticationStatus = " + AuthenticationStatus.ToString();
        }
    }
}
