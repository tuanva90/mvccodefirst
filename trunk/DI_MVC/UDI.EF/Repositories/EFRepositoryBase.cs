﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UDI.CORE.Repositories;
using UDI.EF.DAL;

namespace UDI.EF.Repositories
{
    public abstract class EFRepositoryBase<T> : IRepository<T> where T : class
    {
        protected EFContext _dbContext = null;
        protected DbSet<T> _dbSet;

        public EFRepositoryBase(EFContext inputDBContext)
        {
            _dbContext = inputDBContext;
            _dbSet = _dbContext.Set<T>();
        }

        //public IEnumerable<T> GetAll(Func<T, bool> predicate = null)
        //{
        //    if (predicate != null)
        //    {
        //        return _dbSet.Where(predicate);
        //    }

        //    return _dbSet.AsEnumerable();
        //}

        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return _dbSet.AsNoTracking().Where(predicate);
            }

            return _dbSet.AsNoTracking().AsQueryable();
        }

        public virtual T Get(Func<T, bool> predicate =null)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(predicate);
            //return _dbSet.AsNoTracking().First(predicate);
        }

        public virtual void Add(T entity)
        {
            if (entity != null)
                _dbSet.Add(entity);
        }

        public virtual void Attach(T entity)
        {
            if (entity != null)
                _dbSet.Attach(entity);
        }

        public virtual void Delete(T entity)
        {
            //_dbSet.Remove(entity);
            if (entity != null)
                _dbContext.Entry(entity).State = EntityState.Deleted;
        }

        public virtual void Edit(T entity)
        {
            if (entity != null)
                _dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}