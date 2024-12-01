using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using System.Reflection;
using System.Configuration;

namespace FundXchange.Infrastructure
{
    public class UnitOfWork
    {
        #region Members
        [ThreadStatic]
        private static UnitOfWork _CurrentWork;
        [ThreadStatic]
        private static List<WeakReference> _Entities;
        [ThreadStatic]
        private static List<WeakReference> _ValueObjects;
        [ThreadStatic]
        private static ISessionFactory _SessionFactory;

        public ISession Session { get; private set; }

        public static UnitOfWork CurrentWork
        {
            get
            {
                if (_CurrentWork == null)
                {
                    _CurrentWork = new UnitOfWork();
                }
                return _CurrentWork;
            }
        }
        private ValidationService _ValidationService;
        private Queue<IWorkItemCommand> _WorkItems;

        #endregion

        #region Constructor

        private UnitOfWork()
        {
            _ValidationService = new ValidationService();
            _WorkItems = new Queue<IWorkItemCommand>();
            _Entities = new List<WeakReference>();
            _ValueObjects = new List<WeakReference>();

            if (_SessionFactory == null)
            {
                Assembly domainMapping = Assembly.Load(new AssemblyName(ConfigurationManager.AppSettings["DomainAssembly"]));

                _SessionFactory = CreateSessionFactory(domainMapping);
            }
            Session = _SessionFactory.OpenSession();

        }
        #endregion

        #region Destructor
        ~UnitOfWork()
        {
            try
            {
                if (Session.IsConnected && Session.IsOpen)
                {
                    Session.Disconnect();
                }
            }
            finally
            {
                if (Session.IsOpen)
                {
                    Session.Close();
                }
            }
        }
        #endregion

        #region Public Methods

        public static void NewWork()
        {
            _CurrentWork = new UnitOfWork();
        }

        public static void TrackEntity(Entity entity)
        {
            if (!_Entities.Any(e => e.Target == entity) && EntityIsIdentifiable(entity))
            {
                _Entities.Add(new WeakReference(entity));
            }
        }

        public static void TrackValueObject(ValueObject valueObject)
        {
            if (!_ValueObjects.Any(e => e.Target == valueObject) && ValueObjectIsIdentifiable(valueObject))
            {
                _ValueObjects.Add(new WeakReference(valueObject));
            }
        }

        public void QueueWorkItem(IWorkItemCommand command)
        {
            _WorkItems.Enqueue(command);
        }

        public void Commit()
        {
            CleanDeadReferences();
            try
            {
                PrepareWorkItems();
                PrepareEntityChanges();
                CommitWorkItems();
                CommitEntityChanges();
            }
            catch (Exception ex)
            {
                RollbackWorkItems();
                RollbackEntityChanges();
                throw;
            }
            finally
            {
                _CurrentWork.Session.Close();
            }
        }

        public void Rollback()
        {
            CleanDeadReferences();
            try
            {
                foreach (Entity entity in _Entities.ConvertAll<Entity>(e => e.Target as Entity))
                {
                    entity.AcceptChanges();
                }
                Session.Clear();
            }
            catch (Exception)
            {
                RollbackWorkItems();
                RollbackEntityChanges();
                throw;
            }
            finally
            {
                _CurrentWork.Session.Close();
            }
        }

        #endregion

        #region Private Methods

        private static bool EntityIsIdentifiable(Entity entity)
        {
            return !(entity.Id == 0 && entity.State == EntityState.Unchanged);
        }

        private static bool ValueObjectIsIdentifiable(ValueObject valueObject)
        {
            return !(valueObject.State == ValueObjectState.Unchanged);
        }

        private void RollbackEntityChanges()
        {
            Session.Transaction.Rollback();
        }

        private void CommitEntityChanges()
        {
            Session.Transaction.Commit();
            foreach (Entity entity in _Entities.ConvertAll<Entity>(e => e.Target as Entity))
            {
                entity.AcceptChanges();
            }
            Session.Clear();
        }

        private void PrepareEntityChanges()
        {
            Session.Transaction.Begin();

            List<Entity> entities = _Entities.ConvertAll<Entity>(e => e.Target as Entity);
            List<object> persistEntities = entities.FindAll(e => e.State == EntityState.New || e.State == EntityState.Dirty).ConvertAll<object>(e => e as object);
            List<object> persistValueObjects = _ValueObjects.ConvertAll<object>(e => e.Target as object);
            List<object> allObjects = persistEntities.Union(persistValueObjects).ToList();

            _ValidationService.Validate(allObjects);

            foreach (Entity entity in entities)
            {
                switch (entity.State)
                {
                    case EntityState.New:
                        Session.Save(entity);
                        break;
                    case EntityState.Dirty:
                        Session.Update(entity);
                        break;
                    case EntityState.Deleted:
                        Session.Delete(entity);
                        break;
                    default:
                        break;
                }
            }
        }

        private void RollbackWorkItems()
        {
            foreach (IWorkItemCommand command in _WorkItems)
            {
                command.Rollback();
            }
        }

        private void CommitWorkItems()
        {
            foreach (IWorkItemCommand command in _WorkItems)
            {
                command.Execute();
            }
        }

        private void PrepareWorkItems()
        {
            foreach (IWorkItemCommand command in _WorkItems)
            {
                command.Prepare();
            }
        }

        private void CleanDeadReferences()
        {
            _Entities.RemoveAll(e => !e.IsAlive);
        }

        private static ISessionFactory CreateSessionFactory(Assembly domainMapping)
        {
            return NHibernateSessionFactory.CreateSessionFactory(domainMapping);

        }

        #endregion
    }
}
