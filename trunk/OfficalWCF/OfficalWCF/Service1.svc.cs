using Microsoft.Practices.EnterpriseLibrary.Data;
using OfficalWCF.Entities;
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
        
        //private readonly ICategoryService _cate;
        [OperationContract]
        public List<Category> Test()
        {
            var a = new ICategoryService();
            var ret = a.GetAll();
            return ret.ToList();
        }

        [OperationContract]
        public string TestAdd(Category cate)
        {
            var a = new ICategoryService();
            var ret = a.Add(cate);
            if(ret == 1)
            {
                return "success";
            }
            else
            {
                return "failed";
            }
        }
        [OperationContract]
        public string TestUpdate(Category cate)
        {
            var a = new ICategoryService();
            var ret = a.Update(cate);
            if (ret == 1)
            {
                return "success";
            }
            else
            {
                return "failed";
            }
        }
        [OperationContract]
        public string TestDelete(Category cate)
        {
            var a = new ICategoryService();
            var ret = a.Update(cate);
            if (ret == 1)
            {
                return "success";
            }
            else
            {
                return "failed";
            }
        }

        #region User

        [OperationContract]
        public string TestGetUser(string  user,string password)
        {
            var a = new IUserService();
            var ret = a.Get(user,password);
            if (ret !=null)
            {
                return "success";
            }
            else
            {
                return "failed";
            }
        } 
        #endregion
    }
}
