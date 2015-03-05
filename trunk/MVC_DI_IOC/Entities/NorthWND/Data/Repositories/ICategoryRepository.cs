using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;

namespace MVC_DI_IOC.Core.NorthWND.Data.Repositories
{
    public interface ICategoryRepository : IRepository<Category, int>
    {

    }
}
