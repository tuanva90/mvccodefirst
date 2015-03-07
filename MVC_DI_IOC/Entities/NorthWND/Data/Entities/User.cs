using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Core.NorthWND.Data.Entities
{
    public class User : Entity<int>
    {
        //public virtual int CustomerID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string PassWord { get; set; }
        public virtual bool Role { get; set; }
        public virtual bool Remember { get; set; }
        public virtual Customer Customer { get; set; }
    }
}