using System;

namespace FundXchange.Model.Observer
{
    public interface IConnectionObserver
    {
        void ConnectionChanged(bool connected);
    }
}
