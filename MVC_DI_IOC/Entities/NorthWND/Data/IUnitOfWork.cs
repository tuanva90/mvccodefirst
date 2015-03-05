using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Core.NorthWND.Data
{
    public interface IUnitOfWork
    {
        // Opens database connection and begins transaction.
        void BeginTransaction();

        // Commits transaction and closes database connection.
        void Commit();

        // Rollbacks transaction and closes database connection.
        void Rollback();
    }
}