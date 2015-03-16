﻿using System;
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
        IQueryable<Order> Get(string orid);
        [OperationContract]
        int Add(Order or);
        [OperationContract]
        int Update(Order or);
        [OperationContract]
        int Delete(int or);
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

        public IQueryable<Order> GetOrderByDate(string cusid,DateTime fromdate, DateTime todate)
        {
            List<Order> lsor = new List<Order>();
            string sqlcm = "select distinct OrderID, a.CustomerID, OrderDate, RequiredDate, ShippedDate, Shipvia, Freight from Orders a, Customers where a.CustomerID='"+ cusid +"' and  a.OrderDate between '" + fromdate.ToString("yyyy-MM-dd") + "' and '" + todate.ToString("yyyy-MM-dd") + "'";
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

        public IQueryable<Order> Get(string cusID)
        {
            List<Order> lsor = new List<Order>();
            string sqlCommand = "Select OrderID,CustomerID,OrderDate,RequiredDate,ShippedDate,Shipvia,Freight from Orders where CustomerID='"+ cusID +"'";
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlCommand))
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


        public IQueryable<Order> LoadOrderBySP(string cusid, int numofrow,int numofnext)
        {
            List<Order> lsor = new List<Order>();
            using (IDataReader dr = ConnectionClass.GetInstance().ExcuteSP("loadorder", cusid, numofrow, numofnext))
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

        public int Add(Order or)
        {
            string shipped;
            DateTime ship;
            shipped = or.ShippedDate.ToString();
            ship = DateTime.Parse(shipped);
            string sqlcm = "insert into Orders(CustomerID,OrderDate,RequiredDate,ShippedDate,Shipvia,Freight) values('" + or.CustomerID + "','" + or.OrderDate.ToString("yyyy-MM-dd") + "','" + or.RequireDate.ToString("yyyy-MM-dd") + "','" + ship.ToString("yyyy-MM-dd") + "',3," + or.Freight + ")";
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

        public int Delete(int id)
        {
            string sqlcm = "Delete OrderDetails from OrderDetails INNER JOIN Orders ON OrderDetails.OrderID = Orders.OrderID where OrderDetails.OrderID=" + id;
            string sqlcm2 = "DELETE FROM Orders where OrderiD=" + id;
            ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm2);
        }
    }
}