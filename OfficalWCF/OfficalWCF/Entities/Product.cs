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
    public class Product
    {
        [DataMember]
        public int ProductID;
        [DataMember]
        public string ProductName;
        [DataMember]
        public int CategoryID;
        [DataMember]
        public string QuantityPerUnit;
        [DataMember]
        public decimal UnitPrice;
        [DataMember]
        public int UnitsInStock;
        [DataMember]
        public int UnitsOnOrder;
        [DataMember]
        public int ReorderLevel;
        [DataMember]
        public bool Discontinued;
        [DataMember]
        public int Quantity;
    }
    [ServiceContract]
    public interface IProduct
    {
        [OperationContract]
        IQueryable<Product> GetAll();
        [OperationContract]
        Product Get(int proid);
        [OperationContract]
        int Add(Product pro);
        [OperationContract]
        int Update(Product pro);
        [OperationContract]
        int Delete(Product pro);
    }

    public class IProductService : IProduct
    {
        public Product Get(int id)
        {
            Product pro = new Product();
            string sqlCommand = "Select * from Products where ProductID=" + id;
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlCommand))
            {
                if (dr.Read())
                {
                    pro.ProductID = dr.GetInt32(0);
                    pro.ProductName = dr.GetString(1);
                    pro.CategoryID = dr.GetInt32(2);
                    pro.QuantityPerUnit = dr.GetString(3);
                    pro.UnitPrice = dr.GetDecimal(4);
                    pro.UnitsInStock = dr.GetInt16(5);
                    pro.UnitsOnOrder = dr.GetInt16(6);
                    pro.ReorderLevel = dr.GetInt16(7);
                    pro.Discontinued = dr.GetBoolean(8);
                }
            }
            return pro;
        }

        public IQueryable<Product> GetAll()
        {
            List<Product> lspro = new List<Product>();
            Product pro = new Product();
            string sqlcm = "Select * from Products";
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlcm))
            {
                while (dr.Read())
                {
                    pro.ProductID = dr.GetInt32(0);
                    pro.ProductName = dr.GetString(1);
                    pro.CategoryID = dr.GetInt32(2);
                    pro.QuantityPerUnit = dr.GetString(3);
                    pro.UnitPrice = dr.GetDecimal(4);
                    pro.UnitsInStock = dr.GetInt16(5);
                    pro.UnitsOnOrder = dr.GetInt16(6);
                    pro.ReorderLevel = dr.GetInt16(7);
                    pro.Discontinued = dr.GetBoolean(8);
                    lspro.Add(pro);
                }
                dr.Close();
            }
            return lspro.AsQueryable();
        }

        public int Add(Product pro)
        {
            string sqlcm = "Insert into Products "+
                           "value (ProductID=" + pro.ProductID + ",ProductName=" + pro.ProductName + ",CategoryID=" + pro.CategoryID +
                           ",QuantityPerUnit=" + pro.QuantityPerUnit + ",UnitPrice= "+ pro.UnitPrice +
                           ",UnitsInStock="+ pro.UnitsInStock +",UnitsOnOrder="+ pro.UnitsOnOrder +",ReorderLevel="+ pro.ReorderLevel +",Discontinued="+ pro.Discontinued +")";
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);

        }

        public int Update(Product pro)
        {
            string sqlcm = "Update Products set" +
                           ",ProductName=" + pro.ProductName + ",CategoryID=" + pro.CategoryID +
                           ",QuantityPerUnit=" + pro.QuantityPerUnit + ",UnitPrice= " + pro.UnitPrice +
                           ",UnitsInStock=" + pro.UnitsInStock + ",UnitsOnOrder=" + pro.UnitsOnOrder + ",ReorderLevel=" + pro.ReorderLevel + ",Discontinued=" + pro.Discontinued + "where ProductID="+pro.ProductID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Delete(Product pro)
        {
            string sqlcm = "Delete from Products where ProductID=" + pro.ProductID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }
    }
}