using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDI.CORE.UnitOfWork;

namespace UDI.CORE.UnitOfWork
{
    public interface IUnitOfWorkManager : IDisposable
    {
        IUnitOfWorkManager GetInstance();
        IUnitOfWork GetUnitOfWork();
    }
}
