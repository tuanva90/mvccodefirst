using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using MVC_DI_IOC.Core.NorthWND.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MVC_DI_IOC.Data
{
    public class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {
        private readonly NorthWNDContext _context;
        private readonly IDbSet<TEntity> dbSet;

        public RepositoryBase(NorthWNDContext context)
        {
            this._context = context;
            dbSet = this._context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return dbSet.Where(predicate);
            }

            return dbSet.AsQueryable();
        }
        public TEntity Get(TPrimaryKey key)
        {
            return dbSet.Find(key);
        }
        public void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TPrimaryKey key)
        {
            var ob = dbSet.Find(key);
            dbSet.Remove(ob);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}