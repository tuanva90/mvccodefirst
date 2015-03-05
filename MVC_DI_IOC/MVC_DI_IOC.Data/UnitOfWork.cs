using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Data
{
    public class UnitOfWork : IDisposable
    {
        private readonly NorthWNDContext _context;
        private bool _disposed;
        private Dictionary<string, object> _repositories;

        //public UnitOfWork(NorthWNDContext context)
        //{
        //    this._context = context;
        //}
        public UnitOfWork()
        {
            _context = new NorthWNDContext();
        }
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public Repository<TEntity, TPrimaryKey> Repository<TEntity, TPrimaryKey>() where TEntity : Entity<TPrimaryKey>
        {
            if(_repositories == null)
            {
                _repositories = new Dictionary<string, object>();
            }

            var type = typeof(TEntity).Name;

            if(!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<TEntity,TPrimaryKey>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
            }
            return (Repository<TEntity, TPrimaryKey>)_repositories[type];
        }
    }
}