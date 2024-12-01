using System;
using System.Collections.Generic;

namespace FundXchange.Model.Observer
{
    public class ConnectionNotifier : IConnectionNotifier
    {
        private List<IConnectionObserver> _Observers;

        public ConnectionNotifier()
        {
            _Observers = new List<IConnectionObserver>();
        }

        public void AddObserver(IConnectionObserver observer)
        {
            _Observers.Add(observer);
        }

        public void RemoveObserver(IConnectionObserver observer)
        {
            _Observers.Remove(observer);
        }

        public void NotifyConnectionChanged(bool connected)
        {
            foreach (IConnectionObserver observer in _Observers)
            {
                observer.ConnectionChanged(connected);
            }
        }
    }
}
