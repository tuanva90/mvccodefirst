using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using UDI.CORE.Services;
using UDI.CORE.Services.Impl;
using UDI.EF.DAL;
using UDI.WebASP.Models;

namespace UDI.WebASP.Account
{
    public partial class Register : Page
    {
        private IUserService _usr;
        private ICustomerService _cus;

        public Register()
        {
            _usr = new UserService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
            _cus = new CustomerService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
        }
        protected void CreateUser_Click(object sender, EventArgs e)
        {
            var model = new UDI.CORE.Entities.User();
            model.UserName = UserName.Text;
            model.Password = Password.Text;
            model.Email = Email.Text;
            model.Roles = false;
            
            
            var manager = new UserManager();
            var user = new ApplicationUser() { UserName = UserName.Text };
            IdentityResult result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                IdentityHelper.SignIn(manager, user, isPersistent: false);
                _usr.Add(model);
                Response.Redirect("~/Views/Customer/CustomerIndex.aspx");
            }
            else 
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
    }
}