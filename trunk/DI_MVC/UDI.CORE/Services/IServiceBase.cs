using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDI.CORE.Services
{
    public interface IServiceBase<T> : IDisposable where T : class
    {
        List<T> GetAll();
        void Add(T entity);
        void Edit(T entity);
        void Delete(T entity);
    }
}
