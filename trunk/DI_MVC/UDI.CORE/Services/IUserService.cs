using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;

namespace UDI.CORE.Services
{
    public interface IUserService : IServiceBase<User>
    {
        bool IsValid(string _username, string _password);
        User GetLoginUser(string _username, string _password);
        User GetLoginUser(string _username);
        User Get(int userID);
        User Find(String userName);
    }
}
