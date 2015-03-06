using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using MVC_DI_IOC.Core.NorthWND.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Core.NorthWND.Data
{
    public interface IUnitOfWork
    {
        IRepository<TEntity, TPrimaryKey> Repository<TEntity, TPrimaryKey>() where TEntity : Entity<TPrimaryKey>;
        //IRepository<Category, int> CategoryRepository { get; }
        
        // Opens database connection and begins transaction.
        void BeginTransaction();

        // Commits transaction and closes database connection.
        void Commit();

        // Rollbacks transaction and closes database connection.
        void Rollback();
        void Dispose();
    }
}