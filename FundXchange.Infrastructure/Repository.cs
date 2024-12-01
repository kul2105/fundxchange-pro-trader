using System;
using System.Linq;
using NHibernate.Linq;
using System.Collections.Generic;

namespace FundXchange.Infrastructure
{
    public class Repository : NHibernateContext
    {
        public Repository()
            : base(UnitOfWork.CurrentWork.Session)
        {

        }

        public T GetById<T>(int id) where T : Entity
        {
            return GetQueryable<T>().FirstOrDefault(o => o.Id == id);
        }

        protected IOrderedQueryable<TEntity> GetQueryable<TEntity>()
        {
            return Session.Linq<TEntity>();
        }

        public T GetByCriteria<T>(Func<T, bool> criteria)
            where T : Entity
        {
            T entity = GetQueryable<T>().FirstOrDefault(criteria);
            AssertIfNotFound<T>(entity, new string[] { });
            return entity;
        }

        public List<T> GetListByCriteria<T>(Func<T, bool> criteria)
            where T : Entity
        {
            List<T> list = GetQueryable<T>().Where(criteria).ToList();
            return list;
        }

        protected void AssertIfNotFound<TEntity>(TEntity entity, params string[] criteria)
            where TEntity : Entity
        {
            if (entity == null)
                throw new EntityNotFoundException(typeof(TEntity), criteria);
        }
    }
}
