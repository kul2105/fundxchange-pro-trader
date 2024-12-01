using System;

namespace FundXchange.Infrastructure
{
    public class EntityNotFoundException : DomainException
    {
        public Type EntityType { get; private set; }
        public EntityNotFoundException(Type entityType, params string[] criteria)
            : base(string.Format("{0} could not be found with {1}", entityType.Name, string.Join(",", criteria)))
        {
            EntityType = entityType;
        }
    }
}
