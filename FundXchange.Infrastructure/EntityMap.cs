using System;
using FluentNHibernate.Mapping;

namespace FundXchange.Infrastructure
{
    public class EntityMap<T> : ClassMap<T>
        where T : Entity
    {
        public EntityMap()
        {
            Id(x => x.Id);
            Map(x => x.LastUpdated);
        }
    }
}
