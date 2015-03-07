using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Core.NorthWND.Data.Entities
{
    public class OrderDetail : Entity<int>
    {
        //public virtual int OrderID { get; set; }
        public virtual int ProductID { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual int Quantity { get; set; }
        public virtual float Discount { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}