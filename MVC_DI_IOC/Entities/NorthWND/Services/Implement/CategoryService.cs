using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using MVC_DI_IOC.Core.NorthWND.Data.Repositories;
using MVC_DI_IOC.Core.NorthWND.Data;

namespace MVC_DI_IOC.Core.NorthWND.Services.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [UnitOfWork]
        public List<Category> GetCateList()
        {
            return _categoryRepository.GetAll().OrderBy(cate => cate.CategoryID).ToList();
        }

        public void CreateCate(Category cate)
        {
            _categoryRepository.Insert(cate);
        }

        public void UpdateCate(Category cate)
        {
            _categoryRepository.Update(cate);
        }

        [UnitOfWork]
        public void DeleteCate(int cateID)
        {
            _categoryRepository.Delete(cateID);
        }
    }
}