using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Core.NorthWND.Data.Entities
{
    public class Order :Entity<int>
    {
        public virtual int OrderID { get; set; }
        public virtual int CustomerID { get; set; }
        public virtual DateTime OrderDate { get; set; }
        public virtual DateTime RequireDate { get; set; }
        public virtual DateTime ShippedDate { get; set; }
        public virtual int ShipVia { get; set; }
        public virtual decimal Freight { get; set; }
        public virtual string ShipName { get; set; }
        public virtual string ShipAddress { get; set; }
        public virtual string ShipCity { get; set; }
        public virtual string ShipRegion { get; set; }
        public virtual string ShipPostalCode { get; set; }
        public virtual string ShipCountry { get; set; }
    }
}