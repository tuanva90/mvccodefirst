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
    public class Category
    {
        [DataMember]
        public int CategoryID;
        [DataMember]
        public string CategoryName;
        [DataMember]
        public string Description;
        [DataMember]
        public byte[] Picture;
    }
    [ServiceContract]
    public interface ICategory
    {
        [OperationContract]
        IQueryable<Category> GetAll();
        [OperationContract]
        Category Get(int cateid);
        [OperationContract]
        int Add(Category cate);
        [OperationContract]
        int Update(Category cate);
        [OperationContract]
        int Delete(Category cate);
    }
    public class CategoryService : ICategory
    {
        public Category Get(int id)
        {
            Category cate = new Category();
            string sqlCommand = "Select * from Categories where CategoryID=" + id;
            using (IDataReader dataReader = ConnectionClass.GetInstance().ExecuteReader(sqlCommand))
            {
                if (dataReader.Read())
                {
                    cate.CategoryID = dataReader.GetInt32(0);
                    cate.CategoryName = dataReader.GetString(1);
                    cate.Description = dataReader.GetString(2);
                }
                dataReader.Close();

            }
            return cate;
        }

        public IQueryable<Category> GetAll()
        {
            List<Category> lscate = new List<Category>();
            string sqlcm = "Select * from Categories";
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlcm))
            {
                while (dr.Read())
                {
                    Category cate = new Category();
                    cate.CategoryID = dr.GetInt32(0);
                    cate.CategoryName = dr.GetString(1);
                    cate.Description = dr.GetString(2);
                    lscate.Add(cate);
                }
                dr.Close();
            }
            return lscate.AsQueryable();
        }

        public int Add(Category cate)
        {
            string sqlcm = "insert into Categories(CategoryName,Description) values('" + cate.CategoryName +"','" + cate.Description +"')"; //+ cate.Picture == null ? " " : ",Picture=" + cate.Picture + ")";
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Update(Category cate)
        {
            string sqlcm = "update Categories set CategoryName='"+ cate.CategoryName +"',Description='"+ cate.Description +"' where CategoryID=" + cate.CategoryID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Delete(Category cate)
        {
            string sqlcm = "Delete from Categories where CategoryID=" + cate.CategoryID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }
    }
}