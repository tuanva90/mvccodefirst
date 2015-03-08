using MVC_DI_IOC.Core.NorthWND.Data;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using MVC_DI_IOC.Core.NorthWND.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NorthWNDContext _context;
        private IDbTransaction _transaction;
        private readonly ObjectContext _objectContext;
        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();
        
        public UnitOfWork(NorthWNDContext context)
        {
            this._context = context as NorthWNDContext ?? new NorthWNDContext();
            _objectContext = ((IObjectContextAdapter)_context).ObjectContext;
            _objectContext.Connection.Open();
        }

        public IRepository<TEntity, TPrimaryKey> Repository<TEntity, TPrimaryKey>() where TEntity : Entity<TPrimaryKey>
        {
            if(repositories.Keys.Contains(typeof(TEntity)) == true)
            {
                return repositories[typeof(TEntity)] as IRepository<TEntity, TPrimaryKey>;
            }
            IRepository<TEntity, TPrimaryKey> repo = new RepositoryBase<TEntity, TPrimaryKey>(_context);
            repositories.Add(typeof(TEntity), repo);
            return repo;
        }

        public void BeginTransaction()
        {
            try
            {
                this._transaction = _objectContext.Connection.BeginTransaction();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void Commit()
        {
            try
            {
                this._context.SaveChanges();
                this._transaction.Commit();
            }
            catch(Exception ex)
            {
                Rollback();
                throw ex;
            }
        }

        public void Rollback()
        {
            this._transaction.Rollback();
            foreach(var entry in this._context.ChangeTracker.Entries())
            {
                switch(entry.State)
                {
                    case EntityState.Modified: 
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }
        public void Dispose()
        {
            if(this._context.Database.Connection.State == ConnectionState.Open) 
            {
                this._context.Database.Connection.Close();
            }
        }

    }
}