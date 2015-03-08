using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;
using UDI.CORE.UnitOfWork;

namespace UDI.CORE.Services.Impl
{
    public class ProductService : ServiceBase<Product>, IProductService
    {
        public ProductService(IUnitOfWork uow)
            : base(uow)
        {
        }

        #region IProductService Members

        public List<Product> GetAll(int categoryID)
        {
            return _uow.Repository<Product>().GetAll(p => p.CategoryID == categoryID).ToList();
        }

        public Product Get(int productID)
        {
            return _uow.Repository<Product>().Get(p => p.ProductID == productID);
        }

        #endregion
    }
}

