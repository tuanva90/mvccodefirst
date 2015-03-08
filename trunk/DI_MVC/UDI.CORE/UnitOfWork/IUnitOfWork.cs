using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDI.CORE.Repositories;

namespace UDI.CORE.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Get repository for corresponding class type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRepository<T> Repository<T>() where T : class;

        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        void Rollback();
    }
}
