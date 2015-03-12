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
    public class User
    {
        [DataMember]
        public int CustomerID;
        [DataMember]
        public string UserName;
        [DataMember]
        public string PassWord;
        [DataMember]
        public bool Role;
    }
    [ServiceContract]
    public interface IUser
    {
        [OperationContract]
        IQueryable<User> GetAll();
        [OperationContract]
        User Get(string userName, string Password);
        [OperationContract]
        int Add(User user);
        [OperationContract]
        int Update(User user);
        [OperationContract]
        int Delete(User user);
    }

    public class IUserService : IUser
    {
        public User Get(string Username, string Password)
        {
            User user = new User();
            string sqlCommand = "SELECT * FROM [User] where CustomerID='" + Username+"' AND PassWord='"+Password+"'";
            using (IDataReader dataReader = ConnectionClass.GetInstance().ExecuteReader(sqlCommand))
            {
                if (dataReader.Read())
                {
                    user.UserName = dataReader.GetString(0);
                    user.PassWord = dataReader.GetString(1);
                }
                dataReader.Close();

            }
            return user;
        }

        public IQueryable<User> GetAll()
        {
            List<User> lsuser = new List<User>();
            string sqlcm = "Select * from [user]";
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlcm))
            {
                while (dr.Read())
                {
                    User user = new User();
                    user.CustomerID = dr.GetInt32(0);
                    user.PassWord = dr.GetString(2);
                    lsuser.Add(user);
                }
                dr.Close();
            }
            return lsuser.AsQueryable();
        }

        public int Add(User user)
        {
            string sqlcm = "insert into [User](UserName,Password) values('" + user.UserName + "','" + user.PassWord + "')"; //+ user.Picture == null ? " " : ",Picture=" + user.Picture + ")";
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Update(User user)
        {
            string sqlcm = "update [user] set UserName='" + user.UserName + "',Password='" + user.PassWord + "' where UserID=" + user.CustomerID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Delete(User user)
        {
            string sqlcm = "Delete from [user] where UserID=" + user.CustomerID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }
    }
}