using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Core.NorthWND.Data.Entities
{
    public class Entity<TPrimaryKey> : IEnity<TPrimaryKey>
    {
        public virtual TPrimaryKey ID { get; set; }
    }
}