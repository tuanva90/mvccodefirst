using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using MVC_DI_IOC.Core.NorthWND.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Data
{
    public class CategoryRepository : RepositoryBase<Category, int>, ICategoryRepository
    {

    }
}