using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;

namespace UDI.CORE.Services
{
    public interface ICategoryService : IServiceBase<Category>
    {
        Category Get(int categoryID);
    }
}
