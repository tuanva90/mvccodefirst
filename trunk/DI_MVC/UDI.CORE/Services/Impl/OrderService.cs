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

        public int AddOrder(Customer cus, List<Product> lsProd)
        {
            try
            {
                _uow.BeginTransaction();
                var order = new Order { ShipVia = 1, Freight = 1, OrderDate = DateTime.Today, RequiredDate = DateTime.Today, ShippedDate = DateTime.Today, CustomerID = cus.CustomerID };

                order.OrderDetails = new List<OrderDetail>();
                foreach (Product item in lsProd)
                {
                    order.OrderDetails.Add(new OrderDetail { UnitPrice = item.UnitPrice, Quantity = item.Quantity, ProductID = item.ProductID });
                    //_uow.Repository<OrderDetail>().Add(new OrderDetail { Order = ord, Product = item, UnitPrice = item.UnitPrice, Quantity = item.Quantity, OrderID = ord.OrderID, ProductID = item.ProductID });
                }
                _uow.Repository<Order>().Add(order);
                _uow.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
            
        }
    }
}
