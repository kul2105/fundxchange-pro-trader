using System;
using System.Diagnostics;

namespace FundXchange.Infrastructure
{
    public class Entity
    {
        #region Public Properties

        public virtual int Id { get; private set; }
        public virtual EntityState State { get; set; }
        public virtual DateTime LastUpdated { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// This constructor is called during reconstitution
        /// </summary>
        public Entity()
        {
            State = EntityState.Unchanged;
            UnitOfWork.TrackEntity(this);
        }
        /// <summary>
        /// This constructor is always called when a new entity is being created
        /// </summary>
        public Entity(DateTime lastUpdated)
        {
            LastUpdated = lastUpdated;
            State = EntityState.New;
            UnitOfWork.TrackEntity(this);
        }

        #endregion

        #region Public Methods

        public virtual void MarkDirty()
        {
            if (State == EntityState.Unchanged && !CalledFromFramework())
            {
                State = EntityState.Dirty;
                UnitOfWork.TrackEntity(this);
            }
        }
        public virtual void AcceptChanges()
        {
            if (State != EntityState.Deleted)
            {
                State = EntityState.Unchanged;
            }
        }
        public virtual void Delete()
        {
            State = EntityState.Deleted;
        }

        #endregion

        #region Private Methods

        private bool CalledFromFramework()
        {
            StackTrace trace = new StackTrace();
            if (trace.GetFrames().Length > 3)
            {
                if (trace.GetFrame(3).GetMethod().Name == string.Empty)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
