using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Core.NorthWND.Data.Entities
{
    public class Product : Entity<int>
    {
        public virtual int ProductID { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual string ProductName { get; set; }
        public virtual string QuantityPerUnit { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual int UnitsInStock { get; set; }
        public virtual int UnitsOnOrder { get; set; }
        public virtual int ReorderLevel { get; set; }
        public virtual bool Discontinued { get; set; }
        public virtual int Quantity { get; set; }
    }
}