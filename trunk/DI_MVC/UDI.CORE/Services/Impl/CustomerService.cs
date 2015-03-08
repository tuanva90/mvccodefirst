using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;
using UDI.CORE.UnitOfWork;

namespace UDI.CORE.Services.Impl
{
    public class CustomerService : ServiceBase<Customer>, ICustomerService
    {
        public CustomerService(IUnitOfWork uow)
            : base(uow)
        {
        }

        #region ICustomerService Members

        public Customer Get(int customerID)
        {
            return _uow.Repository<Customer>().Get(c => c.CustomerID == customerID);
        }

        public Customer Find(int customerID)
        {
            return _uow.Repository<Customer>().GetAll(c => c.CustomerID == customerID).FirstOrDefault();
        }
        
        #endregion
    }
}
