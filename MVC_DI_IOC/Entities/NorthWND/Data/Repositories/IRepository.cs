using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System.Linq.Expressions;

namespace MVC_DI_IOC.Core.NorthWND.Data.Repositories
{
    public interface IRepository
    {

    }

    public interface IRepository<TEnity,TPrimaryKey> : IRepository where TEnity : Entity<TPrimaryKey>
    {
        IQueryable<TEnity> GetAll();
        IQueryable<TEnity> GetAll(Expression<Func<TEnity, bool>> predicate = null);
        TEnity Get(TPrimaryKey key);
        void Insert(TEnity entity);
        void Update(TEnity entity);
        void Delete(TPrimaryKey id);
    }
}
