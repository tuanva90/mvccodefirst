using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_DI_IOC.Core.NorthWND.Data.Repositories
{
    public interface IUserRepository : IRepository<User, int>
    {
    }
}
