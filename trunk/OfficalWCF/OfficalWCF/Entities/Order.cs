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
    public class Order
    {
        [DataMember]
        public int OrderID;
        [DataMember]
        public string CustomerID;
        [DataMember]
        public DateTime OrderDate;
        [DataMember]
        public DateTime RequireDate;
        [DataMember]
        public DateTime? ShippedDate;
        [DataMember]
        public int ShipVia;
        [DataMember]
        public decimal Freight;
        //[DataMember]
        //public string ShipName;
        //[DataMember]
        //public string ShipAddress;
        //[DataMember]
        //public string ShipCity;
        //[DataMember]
        //public string ShipRegion;
        //[DataMember]
        //public string ShipPostalCode;
        //[DataMember]
        //public string ShipCountry;
    }
    [ServiceContract]
    public interface IOrder
    {
        [OperationContract]
        IQueryable<Order> GetAll();
        [OperationContract]
        Order Get(int orid);
        [OperationContract]
        int Add(Order or);
        [OperationContract]
        int Update(Order or);
        [OperationContract]
        int Delete(Order or);
    }
    public class OrderService : IOrder
    {

        public IQueryable<Order> GetAll()
        {
            List<Order> lsor = new List<Order>();
            DateTime date = DateTime.Today;
            string sqlcm = "Select top 10 OrderID, CustomerID, OrderDate, RequiredDate, ShippedDate, Shipvia, Freight from Orders";
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlcm))
            {
                while (dr.Read())
                {
                    Order or = new Order();
                    or.OrderID = dr.GetInt32(0);
                    or.CustomerID = dr.GetString(1);
                    or.OrderDate = dr.GetDateTime(2);
                    or.RequireDate = dr.GetDateTime(3);
                    if (dr.IsDBNull(4))
                    {
                        or.ShippedDate = null;
                    }
                    else
                    {
                        or.ShippedDate = dr.GetDateTime(4);
                    }

                    or.ShipVia = dr.GetInt32(5);
                    or.Freight = dr.GetDecimal(6);
                    lsor.Add(or);
                }
                dr.Close();
            }
            return lsor.AsQueryable();
        }

        public Order Get(int orid)
        {
            Order or = new Order();
            string sqlCommand = "Select OrderID,CustomerID,OrderDate,RequiredDate,ShippedDate,Shipvia,Freight from Orders where OrderID=" + orid;
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlCommand))
            {
                if (dr.Read())
                {
                    or.OrderID = dr.GetInt32(0);
                    or.CustomerID = dr.GetString(1);
                    or.OrderDate = dr.GetDateTime(2);
                    or.RequireDate = dr.GetDateTime(3);
                    if (dr.IsDBNull(4))
                    {
                        or.ShippedDate = null;
                    }
                    else
                    {
                        or.ShippedDate = dr.GetDateTime(4);
                    }
                    or.ShipVia = dr.GetInt32(5);
                    or.Freight = dr.GetDecimal(6);
                }
                dr.Close();
            }
            return or;
        }

        public int Add(Order or)
        {
            string sqlcm = "insert into Orders(CustomerID,OrderDate,RequiredDate,ShippedDate,Shipvia,Freight) values('" + or.CustomerID + "','" + or.OrderDate + "','" + or.RequireDate + "','" + or.ShippedDate + "','" + or.ShipVia + "','" + or.Freight + "')";
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Update(Order or)
        {
            string sqlcm = "Update Orders set " +
                           "CustomerID='"+ or.CustomerID +"',OrderDate='"+ or.OrderDate.ToShortDateString() +
                           "',RequiredDate='"+ or.RequireDate.ToShortDateString() +"',ShippedDate='" + or.ShippedDate +
                           "',Shipvia=" + or.ShipVia + ",Freight=" + or.Freight + " where OrderID=" + or.OrderID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Delete(Order or)
        {
            string sqlcm = "Delete from Orders where OrderID=" + or.OrderID; ;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }
    }
}