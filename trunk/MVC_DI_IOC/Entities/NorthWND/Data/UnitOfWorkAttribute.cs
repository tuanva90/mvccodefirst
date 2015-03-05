using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Core.NorthWND.Data
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute : Attribute
    {

    }
}