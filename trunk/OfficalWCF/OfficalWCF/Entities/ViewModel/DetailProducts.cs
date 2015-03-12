using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Data;

namespace OfficalWCF.Entities.ViewModel
{
    [DataContract]
    public class DetailProducts
    {
        [DataMember]
        public int OrderID;
        [DataMember]
        public string ProductName;
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
    public interface IDetailProducts
    {
        [OperationContract]      
        DetailProducts Get(int orid);
 
    }

    public class DetailProductsService: IDetailProducts
    {
        public List<DetailProducts> Get(int orid)
        {
            List<DetailProducts> list = new List<DetailProducts>();
            string sqlCommand = "select Products.ProductName,OrderDetails.UnitPrice,OrderDetails.Quantity,OrderDetails.Discount FROM OrderDetails INNER JOIN Products ON OrderDetails.ProductID = Products.ProductID WHERE OrderID=" + orid;
            using (IDataReader dataReader = ConnectionClass.GetInstance().ExecuteReader(sqlCommand))
            {
                if(dataReader.Read())
                {
                    DetailProducts dp = new DetailProducts();
                    dp.ProductName = dataReader.GetString(0);
                    dp.UnitPrice = int.Parse(dataReader.GetString(1));
                    dp.Quantity = Int16.Parse(dataReader.GetString(2));
                    dp.Discount = float.Parse(dataReader.GetString(3));
                    list.Add(dp);
                }
                dataReader.Close();
            }
            return list;
        }
    }
    
}