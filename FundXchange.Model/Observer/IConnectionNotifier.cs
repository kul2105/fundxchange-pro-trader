using System;

namespace FundXchange.Model.Observer
{
    public interface IConnectionNotifier
    {
        void AddObserver(IConnectionObserver observer);
        void RemoveObserver(IConnectionObserver observer);
        void NotifyConnectionChanged(bool connected);
    }
}
