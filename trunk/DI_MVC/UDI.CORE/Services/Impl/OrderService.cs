using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;
using UDI.CORE.UnitOfWork;

namespace UDI.CORE.Services.Impl
{
    public class OrderService : ServiceBase<Order>, IOrderService
    {
        public OrderService(IUnitOfWork uow)
            : base(uow)
        {
        }

        public Order Get(int orderID)
        {
            return _uow.Repository<Order>().Get(o => o.OrderID == orderID);
        }

        public List<Order> GetListOrder(int customerID)
        {
            return _uow.Repository<Order>().GetAll(o => o.CustomerID == customerID).ToList() ;
        }

        public void AddOrder(Customer cus, List<Product> lsProd)
        {
            _uow.BeginTransaction();
            var order = new Order { Customer = cus, ShipVia = 1, Freight = 1, OrderDate = DateTime.Today, RequiredDate = DateTime.Today, ShippedDate = DateTime.Today, CustomerID = cus.CustomerID };
            //order.OrderDetails.Add
            
            //var ord1 = _uow.Repository<Order>().GetAll().Where(c => c.CustomerID == cus.CustomerID).Max(c => c.OrderID);
            //var ord = Get(ord1); // db.Orders.Where(c => c.OrderID == ord1).First();
            order.OrderDetails = new List<OrderDetail>();
            foreach (Product item in lsProd)
            {
                order.OrderDetails.Add(new OrderDetail { Product = item, UnitPrice = item.UnitPrice, Quantity = item.Quantity, ProductID = item.ProductID });
                //_uow.Repository<OrderDetail>().Add(new OrderDetail { Order = ord, Product = item, UnitPrice = item.UnitPrice, Quantity = item.Quantity, OrderID = ord.OrderID, ProductID = item.ProductID });
            }
            _uow.Repository<Order>().Add(order);
            _uow.Commit();
        }
    }
}
