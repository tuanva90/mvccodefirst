using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
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
    public partial class Login : Page
    {
        private IUserService _usr;
        private ICustomerService _cus;
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register";
            OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            }
        }

        public Login()
        {
            _usr = new UserService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
            _cus = new CustomerService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                if (_usr.IsValid(UserName.Text, Password.Text))
                {
                    var manager = new UserManager();
                    ApplicationUser CheckUser = manager.Find(UserName.Text, Password.Text);
                    if (CheckUser == null)
                    {
                        var user = new ApplicationUser() { UserName = UserName.Text };
                        IdentityResult result = manager.Create(user, Password.Text);
                        if (result.Succeeded)
                        {
                            IdentityHelper.SignIn(manager, user, isPersistent: false);
                        }

                    }
                    else
                    { IdentityHelper.SignIn(manager, CheckUser, RememberMe.Checked); }
                    
                    var loginUser = _usr.GetLoginUser(UserName.Text, Password.Text);
                    var cus = _cus.Find(loginUser.CustomerID);
                    if (cus == null)
                    {
                        Response.Redirect("~/Views/Customer/CustomerIndex.aspx");
                    }
                    else Response.Redirect("/Views/Product/ProductIndex.aspx");
                    
                }
                // Validate the user password
                else
                {
                    FailureText.Text = "Invalid username or password.";
                    ErrorMessage.Visible = true;
                }
               
                
            }
        }
    }
}