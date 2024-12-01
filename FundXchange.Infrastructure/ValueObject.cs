using System;

namespace FundXchange.Infrastructure
{
    public class ValueObject
    {
        public virtual DateTime TimeCreated { get; set; }
        public virtual ValueObjectState State { get; set; }

        protected ValueObject()
        {
            State = ValueObjectState.Unchanged;
            UnitOfWork.TrackValueObject(this);
        }

        public ValueObject(DateTime timeCreated)
        {
            TimeCreated = timeCreated;
            State = ValueObjectState.New;
            UnitOfWork.TrackValueObject(this);
        }

        public virtual void Delete()
        {
            State = ValueObjectState.Deleted;
        }
    }
}
