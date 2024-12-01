using System;
namespace Fin24.Util.General.Ipc
{
    public interface IDataReceiver
    {
        event EventHandler DataAvailable;

        object Read(int timeout);
        object BlockingRead();
    }
}