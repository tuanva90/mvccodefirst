using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using UDI.CORE.UnitOfWork;
using UDI.EF.DAL;

namespace UDI.EF.UnitOfWork
{
    public class EFUnitOfWorkManager : IUnitOfWorkManager
    {
        private bool _isDisposed;
        private static EFContext _context;
        private static IUnitOfWorkManager _instance;

        public EFUnitOfWorkManager(IEFContext context)
        {
            Database.SetInitializer<EFContext>(null);
            _context = context as EFContext;
        }

        public IUnitOfWorkManager GetInstance()
        {
            if (_instance == null)
                _instance = new EFUnitOfWorkManager(_context);
            return _instance;
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return new EFUnitOfWork(_context);
        }

        public void Dispose()
        {
            if (this._isDisposed == false)
            {
                _context.Dispose();
                _instance = null;
                this._isDisposed = true;
            }
        }
    }
}
