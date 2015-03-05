using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_DI_IOC.Core.NorthWND.Services
{
    public interface ICategoryService
    {
        List<Category> GetCateList();
        void CreateCate(Category cate);
        void UpdateCate(Category cate);
        void DeleteCate(int cateID);
    }
}
