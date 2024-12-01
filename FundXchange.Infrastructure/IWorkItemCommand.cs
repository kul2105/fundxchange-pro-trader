using System;

namespace FundXchange.Infrastructure
{
    public interface IWorkItemCommand
    {
        void Prepare();
        void Execute();
        void Rollback();
    }
}
