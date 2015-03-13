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
        [OperationContract]
        IQueryable<OrderDetail> GetAll();
        OrderDetail Get(int orid);
        [OperationContract]
        int Add(OrderDetail or);
        [OperationContract]
        int Update(OrderDetail or);
        [OperationContract]
        int Delete(OrderDetail or);
    }

    public class OrderDetailService : IOrderDetail
    {
        public IQueryable<OrderDetail> GetAll()
        {
            throw new NotImplementedException();
        }

        public OrderDetail Get(int orid)
        {
            throw new NotImplementedException();
        }

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