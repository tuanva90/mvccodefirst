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
    public class Customer
    {
        [DataMember]
        public string CustomerID;
        [DataMember]
        public string CompanyName;
        [DataMember]
        public string ContactName;
        [DataMember]
        public string ContactTitle;
        [DataMember]
        public string Address;
        [DataMember]
        public string City;
        [DataMember]
        public string Region;
        [DataMember]
        public string PostalCode;
        [DataMember]
        public string Country;
        [DataMember]
        public string Phone;
        [DataMember]
        public string Fax;
        [DataMember]
        public string Password;
    }

    [ServiceContract]
    public interface ICustomer
    {
        [OperationContract]
        IQueryable<Customer> GetAll();
        [OperationContract]
        Customer Get(int _cusid,string pass);
        [OperationContract]
        int Add(Customer _cus);
        [OperationContract]
        int Update(Customer _cus);
        [OperationContract]
        int Delete(Customer _cus);
    }
    public class CustomerService : ICustomer
    {
        public Customer Get(int id,string password)
        {
            Customer _cus = new Customer();
            string sqlCommand = "Select * from Customers where CustomerID='" + id+"' AND Password='"+password+"'";
            using (IDataReader dataReader = ConnectionClass.GetInstance().ExecuteReader(sqlCommand))
            {
                if (dataReader.Read())
                {
                    _cus.CustomerID = dataReader.GetString(0);
                    _cus.CompanyName = dataReader.GetString(1);
                    _cus.ContactName = dataReader.GetString(2);
                    //_cus.contacttitle = datareader.getstring(3);
                    //_cus.address = datareader.getstring(4);
                    //_cus.city = datareader.getstring(5);
                    //_cus.region = datareader.getstring(6);
                    //_cus.postalcode = datareader.getstring(7);
                    //_cus.country= datareader.getstring(8);
                    //_cus.phone = datareader.getstring(9);
                    //_cus.fax = datareader.getstring(10);
                }
                dataReader.Close();

            }
            return _cus;
        }

        public IQueryable<Customer> GetAll()
        {
            List<Customer> ls_cus = new List<Customer>();
            string sqlcm = "Select * from Customers";
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlcm))
            {
                while (dr.Read())
                {
                    Customer _cus = new Customer();
                    _cus.CustomerID = dr.GetString(0);
                    _cus.CompanyName = dr.GetString(1);
                    _cus.ContactName = dr.GetString(2);
                    _cus.ContactTitle = dr.GetString(3);
                    _cus.Address = dr.GetString(4);
                    _cus.City = dr.GetString(5);
                    _cus.Region = dr.GetString(6);
                    _cus.PostalCode = dr.GetString(7);
                    _cus.Country = dr.GetString(8);
                    _cus.Phone = dr.GetString(9);
                    _cus.Fax = dr.GetString(10);
                    ls_cus.Add(_cus);
                }
                dr.Close();
            }
            return ls_cus.AsQueryable();
        }

        public int Add(Customer _cus)
        {
            string sqlcm = "insert into Customers(CustomerID,CompanyName,) values('" + _cus.CustomerID + "','" + _cus.CompanyName + "')"; 
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Update(Customer _cus)
        {
            string sqlcm = "update Customers set CompanyName='" + _cus.CompanyName + "',ContactName='" + _cus.ContactName + "',ContactTitle='" + _cus.ContactTitle + "',Address='" + _cus.Address + "',City='" + _cus.City + "',Region='" + _cus.Region + "',PosttalCode='" + _cus.PostalCode + "',Country='" + _cus.Country + "' where CustomerID=" + _cus.CustomerID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Delete(Customer _cus)
        {
            string sqlcm = "Delete from Customer where CustomerID=" + _cus.CustomerID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }
    }
}