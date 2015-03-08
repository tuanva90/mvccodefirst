using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;

namespace UDI.CORE.Services
{
    public interface IOrderService : IServiceBase<Order>
    {
        Order Get(int orderID);
        List<Order> GetListOrder(int customerID);
        void AddOrder(Customer cus, List<Product> lsProd);
    }
}
