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
        int Delete(int id);
    }

    public class ProductService : IProduct
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
                    pro.QuantityPerUnit = dr.GetString(4);
                    pro.UnitPrice = dr.GetDecimal(5);
                    pro.UnitsInStock = dr.GetInt16(6);
                    pro.UnitsOnOrder = dr.GetInt16(7);
                    pro.ReorderLevel = dr.GetInt16(8);
                    pro.Discontinued = dr.GetBoolean(9);
                }
            }
            return pro;
        }

        public IQueryable<Product> GetAll()
        {
            List<Product> lspro = new List<Product>();
            string sqlcm = "Select * from Products";
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlcm))
            {
                while (dr.Read())
                {
                    Product pro = new Product();
                    pro.ProductID = dr.GetInt32(0);
                    pro.ProductName = dr.GetString(1);
                    pro.CategoryID = dr.GetInt32(3);
                    pro.QuantityPerUnit = dr.GetString(4);
                    pro.UnitPrice = dr.GetDecimal(5);
                    pro.UnitsInStock = dr.GetInt16(6);
                    pro.UnitsOnOrder = dr.GetInt16(7);
                    pro.ReorderLevel = dr.GetInt16(8);
                    pro.Discontinued = dr.GetBoolean(9);
                    lspro.Add(pro);
                }
                dr.Close();
            }
            return lspro.AsQueryable();
        }

        public int Add(Product pro)
        {
            if(pro.Discontinued==true)
            {
                string sqlcm = "Insert into Products(ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued) " +
                           "values ('" + pro.ProductName + "',1,'" + pro.CategoryID + "','" + pro.QuantityPerUnit + "'," + pro.UnitPrice +
                           "," + pro.UnitsInStock + "," + pro.UnitsOnOrder + "," + pro.ReorderLevel + ",1)";
                return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
            }
            else
            {
                string sqlcm = "Insert into Products(ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued) " +
                           "values ('" + pro.ProductName + "',1,'" + pro.CategoryID + "','" + pro.QuantityPerUnit + "'," + pro.UnitPrice +
                           "," + pro.UnitsInStock + "," + pro.UnitsOnOrder + "," + pro.ReorderLevel + ",0)";
                return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
            }
        }

        public int Update(Product pro)
        {
            if(pro.Discontinued == true)
            {
                string sqlcm = "Update Products set" +
                           "ProductName=" + pro.ProductName + ",SupplierID=1,CategoryID=" + pro.CategoryID +
                           ",QuantityPerUnit=" + pro.QuantityPerUnit + ",UnitPrice= " + pro.UnitPrice +
                           ",UnitsInStock=" + pro.UnitsInStock + ",UnitsOnOrder=" + pro.UnitsOnOrder + ",ReorderLevel=" + pro.ReorderLevel + ",Discontinued=1 where ProductID=" + pro.ProductID;
                return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
            }
            else
            {
                string sqlcm = "Update Products set" +
                           " ProductName='" + pro.ProductName + "',SupplierID=1,CategoryID=" + pro.CategoryID +
                           ",QuantityPerUnit='" + pro.QuantityPerUnit + "',UnitPrice= " + pro.UnitPrice +
                           ",UnitsInStock=" + pro.UnitsInStock + ",UnitsOnOrder=" + pro.UnitsOnOrder + ",ReorderLevel=" + pro.ReorderLevel + ",Discontinued=0 where ProductID=" + pro.ProductID;
                return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
            }
            
        }

        public int Delete(int id)
        {

            string sql = "select * from OrderDetails where ProductID=" + id;
            IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sql);
            if (dr != null)
            {
                string sqlcm = "Delete OrderDetails  from OrderDetails INNER JOIN Products ON OrderDetails.ProductID = Products.ProductID where OrderDetails.ProductID=" + id;
                ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
                string sqlcm2 = "delete from Products where ProductID=" + id;
                return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm2);
            }
            else
            {
                string sqlcm2 = "delete from Products where ProductID=" + id;
                return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm2);
            }
        }
        public IQueryable<Product> GetByName(string name)
        {
            List<Product> lspro = new List<Product>();
            string sqlcm = "Select * from Products where ProductName like '"+name+"'";
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlcm))
            {
                while (dr.Read())
                {
                    Product pro = new Product();
                    pro.ProductID = dr.GetInt32(0);
                    pro.ProductName = dr.GetString(1);
                    pro.CategoryID = dr.GetInt32(3);
                    pro.QuantityPerUnit = dr.GetString(4);
                    pro.UnitPrice = dr.GetDecimal(5);
                    pro.UnitsInStock = dr.GetInt16(6);
                    pro.UnitsOnOrder = dr.GetInt16(7);
                    pro.ReorderLevel = dr.GetInt16(8);
                    pro.Discontinued = dr.GetBoolean(9);
                    lspro.Add(pro);
                }
                dr.Close();
            }
            return lspro.AsQueryable();
        }
    }
}