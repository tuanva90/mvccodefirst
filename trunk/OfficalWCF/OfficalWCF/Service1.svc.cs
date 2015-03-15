using Microsoft.Practices.EnterpriseLibrary.Data;
using OfficalWCF.Entities;
using OfficalWCF.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace OfficalWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceContract]
    public class Service1
    {
        private CategoryService _Cateservice;
        private OrderService _Orderservice;
        private ProductService _Proservice;
        private CustomerService _Cuservice;
        private OrderDetailService _Detailsevice;

        //private readonly ICategoryService _cate;
        
        #region Category
        [OperationContract]
        public Category GetCategogy(int id)
        {
            _Cateservice = new CategoryService();
            var ret = _Cateservice.Get(id);
            return ret;
        }

        [OperationContract]
        public IQueryable<Category> GetCategogyByName(string name)
        {
            _Cateservice = new CategoryService();
            IQueryable<Category> _a = _Cateservice.GetByName(name);
            return _a;
        }

        [OperationContract]
        public IQueryable<Category> GetAllCategory()
        {
            _Cateservice = new CategoryService();
            IQueryable<Category> _a = _Cateservice.GetAll();
            return _a;
        }

        [OperationContract]
        public  int AddCategory(Category cate)
        {
            _Cateservice = new CategoryService();
            if (_Cateservice.Add(cate) == 1)
                return 1;
            else
                return 0;
        }

        [OperationContract]
        public int UpdateCategory(Category cate)
        {
            _Cateservice = new CategoryService();
            if (_Cateservice.Update(cate) == 1)
                return 1;
            else
                return 0;
        }
        [OperationContract]
        public int DeleteCategory(int ID)
        {
            _Cateservice = new CategoryService();
            if (_Cateservice.Delete(ID) == 1)
                return 1;
            else
                return 0;
        }


        #endregion

        #region Order

        [OperationContract]
        public IQueryable<Order> GetOrder(string id)
        {
            _Orderservice = new OrderService();
            IQueryable<Order> ret = _Orderservice.Get(id);
            return ret;
        }

        [OperationContract]
        public IQueryable<Order> GetListOrderByDate(string cusid,DateTime fromdate, DateTime todate)
        {
            _Orderservice = new OrderService();
            IQueryable<Order> _a = _Orderservice.GetOrderByDate(cusid, fromdate, todate);
            return _a;
        }

        [OperationContract]
        public IQueryable<Order> GetAllOrder()
        {
            _Orderservice = new OrderService();
            IQueryable<Order> _a = _Orderservice.GetAll();
            return _a;
        }

        [OperationContract]
        public int AddOrder(Order or)
        {
            _Orderservice = new OrderService();
            if (_Orderservice.Add(or) == 1)
                return 1;
            else
                return 0;
        }

        public int UpdateOrder(Order or)
        {
            _Orderservice = new OrderService();
            if (_Orderservice.Update(or) == 1)
                return 1;
            else
                return 0;
        }

        public int DeleteOrder(Order or)
        {
            _Orderservice = new OrderService();
            if (_Orderservice.Delete(or) == 1)
                return 1;
            else
                return 0;
        }


        #endregion

        #region Product

        [OperationContract]
        public Product GetProduct(int id)
        {
            _Proservice = new ProductService();
            var ret = _Proservice.Get(id);
            return ret;
        }

        [OperationContract]
        public IQueryable<Product> GetAllProduct()
        {
            _Proservice = new ProductService();
            IQueryable<Product> _a = _Proservice.GetAll();
            return _a;
        }

        [OperationContract]
        public IQueryable<Product> GetProductByName(string name)
        {
            _Proservice = new ProductService();
            IQueryable<Product> _a = _Proservice.GetByName(name);
            return _a;
        }

        [OperationContract]
        public int AddProduct(Product or)
        {
            _Proservice = new ProductService();
            if (_Proservice.Add(or) == 1)
                return 1;
            else
                return 0;
        }

        public int UpdateProduct(Product or)
        {
            _Proservice = new ProductService();
            if (_Proservice.Update(or) == 1)
                return 1;
            else
                return 0;
        }

        public int DeleteProduct(Product or)
        {
            _Proservice = new ProductService();
            if (_Proservice.Delete(or) == 1)
                return 1;
            else
                return 0;
        }


        #endregion

        #region Customer

        [OperationContract]
        public string GetCustomer(string id,string pass)
        {
            _Cuservice = new CustomerService();
            var ret = _Cuservice.Get(id,pass);
            return ret;
        }

        [OperationContract]
        public IQueryable<Customer> GetAllCustomer()
        {
            _Cuservice = new CustomerService();
            IQueryable<Customer> _a = _Cuservice.GetAll();
            return _a;
        }

        [OperationContract]
        public int AddCustomer(Customer or)
        {
            _Cuservice = new CustomerService();
            if (_Cuservice.Add(or) == 1)
                return 1;
            else
                return 0;
        }

        public int UpdateCustomer(Customer or)
        {
            _Cuservice = new CustomerService();
            if (_Cuservice.Update(or) == 1)
                return 1;
            else
                return 0;
        }

        public int DeleteCustomer(Customer or)
        {
            _Cuservice = new CustomerService();
            if (_Cuservice.Delete(or) == 1)
                return 1;
            else
                return 0;
        }


        #endregion

        #region OrderDetail

        

        [OperationContract]
        public List<DetailProducts> getCusID(int CusID)
        {
            var a = new DetailProductsService();
            var re = a.Get(CusID);
            return re;
        }

        #endregion


        #region User

        [OperationContract]
        public string TestGetUser(string user, string password)
        {
            var a = new IUserService();
            var ret = a.Get(user, password);
            if (ret != null)
            {
                return "success";
            }
            else
            {
                return "failed";
            }
        }


        [OperationContract]
        public string TestAddUser(User user)
        {
            var a = new IUserService();
            var ret = a.Add(user);
            if (ret == 1)
            {
                return "success";
            }
            else
            {
                return "failed";
            }
        #endregion
        }
    }
}