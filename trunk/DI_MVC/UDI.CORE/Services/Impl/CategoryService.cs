using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;
using UDI.CORE.UnitOfWork;

namespace UDI.CORE.Services.Impl
{
    public class CategoryService : ServiceBase<Category>, ICategoryService
    {
        public CategoryService(IUnitOfWork uow) : base(uow)
        {
        }

        public Category Get(int categoryID)
        {
            return _uow.Repository<Category>().Get(c => c.CategoryID == categoryID);
        }
    }
}
