using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Core.NorthWND.Data.Entities
{
    public class Category : Entity<int>
    {
        public virtual int CategoryID { get; set; }
        public virtual string CategoryName { get; set; }
        public virtual string Description { get; set; }
        public virtual byte[] Picture { get; set; }
    }
}