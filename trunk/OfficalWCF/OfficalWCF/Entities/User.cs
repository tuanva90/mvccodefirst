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
        public int UserID;
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
            string sqlCommand = "Select * from user where UserName='" + Username+"' AND Password='"+Password+"'";
            using (IDataReader dataReader = ConnectionClass.GetInstance().ExecuteReader(sqlCommand))
            {
                if (dataReader.Read())
                {
                    user.UserID = dataReader.GetInt32(0);
                    user.UserName = dataReader.GetString(1);
                    user.PassWord = dataReader.GetString(2);
                }
                dataReader.Close();

            }
            return user;
        }

        public IQueryable<User> GetAll()
        {
            List<User> lsuser = new List<User>();
            string sqlcm = "Select * from user";
            using (IDataReader dr = ConnectionClass.GetInstance().ExecuteReader(sqlcm))
            {
                while (dr.Read())
                {
                    User user = new User();
                    user.UserID = dr.GetInt32(0);
                    user.UserName = dr.GetString(1);
                    user.PassWord = dr.GetString(2);
                    lsuser.Add(user);
                }
                dr.Close();
            }
            return lsuser.AsQueryable();
        }

        public int Add(User user)
        {
            string sqlcm = "insert into user(UserName,Password) values('" + user.UserName + "','" + user.PassWord + "')"; //+ user.Picture == null ? " " : ",Picture=" + user.Picture + ")";
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Update(User user)
        {
            string sqlcm = "update user set UserName='" + user.UserName + "',Password='" + user.PassWord + "' where UserID=" + user.UserID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }

        public int Delete(User user)
        {
            string sqlcm = "Delete from user where UserID=" + user.UserID;
            return ConnectionClass.GetInstance().ExecuteNonQuery(sqlcm);
        }
    }
}