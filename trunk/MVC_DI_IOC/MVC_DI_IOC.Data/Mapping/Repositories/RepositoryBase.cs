using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using MVC_DI_IOC.Core.NorthWND.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Data.Mapping.Repositories
{
    public abstract class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {
        protected
    }
}