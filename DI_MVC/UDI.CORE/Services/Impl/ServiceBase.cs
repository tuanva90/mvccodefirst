using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;
using UDI.CORE.UnitOfWork;

namespace UDI.CORE.Services.Impl
{
    public abstract class ServiceBase<T> : IServiceBase<T> where T : class
    {
        protected IUnitOfWork _uow;

        public ServiceBase(IUnitOfWork uow)
        {
            _uow = uow;
        }

        #region IServiceBase<T> Members

        public List<T> GetAll()
        {
            return _uow.Repository<T>().GetAll().ToList();
        }

        public void Add(T entity)
        {
            //_uow.BeginTransaction();
            _uow.Repository<T>().Add(entity);
            //_uow.Commit();
        }

        public void Edit(T entity)
        {
            //_uow.BeginTransaction();
            _uow.Repository<T>().Edit(entity);
            //_uow.Commit();
        }

        public void Delete(T entity)
        {
            //_uow.BeginTransaction();
            _uow.Repository<T>().Delete(entity);
            //_uow.Commit();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //_uow.Dispose();
        }

        #endregion
    }
}
