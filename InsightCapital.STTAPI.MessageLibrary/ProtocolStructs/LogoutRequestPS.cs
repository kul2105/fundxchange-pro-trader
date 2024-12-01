using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;

namespace InsightCapital.STTAPI.MessageLibrary.ProtocolStructs
{
    public class LogoutRequestPS : IProtocolStruct
    {
        public LogoutRequest LoginRequest_ = new LogoutRequest();

        public override int ID
        {
            get { return ProtocolStructIDS.LogOut_RequestPS_ID; }
        }

        public override void StartWrite(byte[] buffer)
        {
            InitWrite(buffer);
            bw_.Write(LoginRequest_.UserName_);
            bw_.Write(LoginRequest_.PassWord_);
            bw_.Write(LoginRequest_.SenderCompID_);
            bw_.Write(LoginRequest_.TargetCompID_);
            CloseWrite();
        }

        public override void StartRead(byte[] buffer)
        {
            InitRead(buffer);
            LoginRequest_.UserName_ = br_.ReadString();
            LoginRequest_.PassWord_ = br_.ReadString();
            LoginRequest_.SenderCompID_ = br_.ReadString();
            LoginRequest_.TargetCompID_ = br_.ReadString();
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

    public class LogoutRequest
    {
        public string UserName_;
        public string PassWord_;
        public string SenderCompID_;
        public string TargetCompID_;

        public LogoutRequest()
        {
            UserName_ = string.Empty;
            PassWord_ = string.Empty;
            SenderCompID_ = string.Empty;
            TargetCompID_ = string.Empty;
        }
    }
}
