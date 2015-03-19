using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;
using UDI.CORE.UnitOfWork;

namespace UDI.CORE.Services.Impl
{
    public class UserService : ServiceBase<User>, IUserService
    {
        public UserService(IUnitOfWork uow)
            : base(uow)
        {
        }

        public bool IsValid(string userName, string passWord)
        {
            var check = _uow.Repository<User>().GetAll(u => u.UserName == userName && u.Password == passWord); // from p in db.Users where p.UserName == _username && p.Password == _password select p.UserName;
            if (check.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public User GetLoginUser(string userName, string passWord)
        {
            var check = _uow.Repository<User>().Get(u => u.UserName == userName && u.Password == passWord); // from p in db.Users where p.UserName == _username && p.Password == _password select p.UserName;
            return check;
        }

        public User GetLoginUser(string userName)
        {
            var check = _uow.Repository<User>().Get(u => u.UserName == userName); // from p in db.Users where p.UserName == _username && p.Password == _password select p.UserName;
            return check;
        }

        public User Get(int customerID)
        {
            return _uow.Repository<User>().Get(u => u.CustomerID == customerID);
        }

        public User Find(string userName)
        {
            return _uow.Repository<User>().GetAll(u => u.UserName == userName).FirstOrDefault();
        }
    }
}
