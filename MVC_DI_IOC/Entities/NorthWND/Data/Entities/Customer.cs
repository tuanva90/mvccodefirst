using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Core.NorthWND.Data.Entities
{
    public class Customer : Entity<int>
    {
        //public virtual int CustomerID { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string ContactTitle { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string Region { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string Country { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Fax { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}