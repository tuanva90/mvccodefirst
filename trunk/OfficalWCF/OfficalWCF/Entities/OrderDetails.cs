using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace OfficalWCF.Entities
{
    [DataContract]
    public class OrderDetail
    {
        [DataMember]
        public int OrderID;
        [DataMember]
        public string ProductID;
        [DataMember]
        public decimal UnitPrice;
        [DataMember]
        public int Quantity;
        [DataMember]
        public float Discount;
    }

    [ServiceContract]
    public interface IOrderDetail
    {
        //[OperationContract]
        //IQueryable<OrderDetail> GetAll();
        
        //OrderDetail Get(int orid);
        //[OperationContract]
        int Add(OrderDetail or);
        [OperationContract]
        int Update(OrderDetail or);
        [OperationContract]
        int Delete(OrderDetail or);
    }

    public class OrderDetailService : IOrderDetail
    {


        //public OrderDetail Get(int orid)
        //{
        //    //Product pro = new Product();
        //    string sqlCommand = "Select  from Products where ProductID=" + orid;
        //    using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlCommand))
        //    {
        //        if (dr.Read())
        //        {
        //            pro.ProductID = dr.GetInt32(0);
        //            pro.ProductName = dr.GetString(1);
        //            pro.CategoryID = dr.GetInt32(2);
        //            pro.QuantityPerUnit = dr.GetString(4);
        //            pro.UnitPrice = dr.GetDecimal(5);
        //            pro.UnitsInStock = dr.GetInt16(6);
        //            pro.UnitsOnOrder = dr.GetInt16(7);
        //            pro.ReorderLevel = dr.GetInt16(8);
        //            pro.Discontinued = dr.GetBoolean(9);
        //        }
        //    }
        //    return pro;
        //}

        public int Add(OrderDetail or)
        {
            throw new NotImplementedException();
        }

        public int Update(OrderDetail or)
        {
            throw new NotImplementedException();
        }

        public int Delete(OrderDetail or)
        {
            throw new NotImplementedException();
        }
    }
}