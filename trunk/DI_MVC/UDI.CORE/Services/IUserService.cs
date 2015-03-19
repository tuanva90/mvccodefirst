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
        bool IsValid(string userName, string passWord);
        User GetLoginUser(string userName, string passWord);
        User GetLoginUser(string userName);
        User Get(int userID);
        User Find(string userName);
    }
}
