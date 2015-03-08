using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;
using UDI.CORE.Entities;
using UDI.CORE.Repositories;
using UDI.CORE.UnitOfWork;
using UDI.EF.DAL;
using UDI.EF.Repositories;

namespace UDI.EF.UnitOfWork
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly EFContext _context;
        private IDbTransaction _transaction;
        private readonly ObjectContext _objectContext;
        //private readonly DbContext _dbContext;

        public EFUnitOfWork(IEFContext context)
        {
            this._context = context as EFContext ?? new EFContext();

            _objectContext = ((IObjectContextAdapter)_context).ObjectContext;
            _objectContext.Connection.Open();

            //this._dbContext = ((Idbco IObjectContextAdapter)this._context).ObjectContext;

            //if (this._context.Database.Connection.State != ConnectionState.Open)
            //{
            //    this._context.Database.Connection.Open();
            //}
        }

        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public IRepository<T> Repository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)] as IRepository<T>;
            }

            IRepository<T> repo;
            switch (typeof(T).Name)
            {
                case "User":
                    repo = new EFUserRepository(_context) as IRepository<T>;
                    break;
                case "Category":
                    repo = new EFCategoryRepository(_context) as IRepository<T>;
                    break;
                case "Customer":
                    repo = new EFCustomerRepository(_context) as IRepository<T>;
                    break;
                case "Order":
                    repo = new EFOrderRepository(_context) as IRepository<T>;
                    break;
                case "OrderDetail":
                    repo = new EFOrderDetailRepository(_context) as IRepository<T>;
                    break;
                case "Product":
                    repo = new EFProductRepository(_context) as IRepository<T>;
                    break;
                default:
                    repo = null;
                    break;
            }
            
            repositories.Add(typeof(T), repo);
            return repo;
        }

        public void BeginTransaction()
        {
            try
            {
                this._transaction = _objectContext.Connection.BeginTransaction();
            }
            catch (Exception)
            {
                //Transaction is openning...
            }
            
        }
        public void Commit()
        {
            try
            {
                this._transaction.Commit();
            }
            catch (Exception ex)
            {
                Rollback();
                throw ex;
            }
        }

        public void Rollback()
        {
            this._transaction.Rollback();

            foreach (var entry in this._context.ChangeTracker.Entries())
            {
                switch (entry.State)
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
            if (this._context.Database.Connection.State == ConnectionState.Open)
            {
                this._context.Database.Connection.Close();
            }
        }
    }
}
