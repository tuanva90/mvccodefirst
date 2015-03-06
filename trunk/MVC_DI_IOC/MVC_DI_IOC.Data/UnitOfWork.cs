using MVC_DI_IOC.Core.NorthWND.Data;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NorthWNDContext _context;
        private bool _disposed;
        private Dictionary<string, object> _repositories;

        //public UnitOfWork(NorthWNDContext context)
        //{
        //    this._context = context;
        //}
        public UnitOfWork(NorthWNDContext context)
        {
            this._context = context;
        }
        public void BeginTransaction()
        {

        }

        public void Commit()
        {

        }

        public void Rollback()
        {

        }

    }
}