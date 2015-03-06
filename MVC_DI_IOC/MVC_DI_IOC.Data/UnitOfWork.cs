using MVC_DI_IOC.Core.NorthWND.Data;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using MVC_DI_IOC.Core.NorthWND.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NorthWNDContext _context;
        private IDbTransaction _transaction;
        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();
        
        public UnitOfWork(NorthWNDContext context)
        {
            this._context = context as NorthWNDContext ?? new NorthWNDContext();

            
        }

        public IRepository<TEntity, TPrimaryKey> Repository<TEntity, TPrimaryKey>() where TEntity : Entity<TPrimaryKey>
        {
            if(repositories.Keys.Contains(typeof(TEntity)) == true)
            {
                return repositories[typeof(TEntity)] as IRepository<TEntity, TPrimaryKey>;
            }
            IRepository<TEntity, TPrimaryKey> repo = new RepositoryBase<TEntity, TPrimaryKey>(_context);
        }

        public void BeginTransaction()
        {

        }

        public void Commit()
        {

        }

        public void Rollback()
        {

        }

    }
}