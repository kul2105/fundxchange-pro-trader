using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsightCapital.STTAPI.Sockets.Classes
{
    public interface IParser
    {
        void Parse();
        void Read(byte[] data, int length);
        Dictionary<int, Type> Structs_ { get; set; }
        event ProtocolReaderHandler PSRead;
    }
}
