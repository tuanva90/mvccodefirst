using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDI.CORE.Services;
using UDI.CORE.Services.Impl;
using UDI.EF.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace UDI.WebASP.Views.Customer
{
    public partial class CustomerIndex : System.Web.UI.Page
    {
        private ICustomerService _cus;
        private IUserService _usr;

        public CustomerIndex()
        {
            _cus = new CustomerService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
            _usr = new UserService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreat_Click(object sender, EventArgs e)
        {
            var customer = new UDI.CORE.Entities.Customer();
            customer.ContactName = txtContactName.Text;
            customer.ContactTitle = txtContacTitle.Text;
            customer.Address = txtAdress.Text;
            customer.City = txtCity.Text;
            customer.Phone = txtPhone.Text;
            customer.Region = txtRegion.Text;
            customer.Fax = txtRegion.Text;
            customer.PostalCode = txtPostalcode.Text;
            customer.Country = txtCountry.Text;

            
            var loginUser = _usr.GetLoginUser(Context.User.Identity.GetUserName()); // db.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault();
            if (loginUser != null)
            {
                customer.CustomerID = loginUser.CustomerID;
            }

            if (ModelState.IsValid)
            {
                _cus.Add(customer);
                Response.Redirect("~/Views/Product/ProductIndex.aspx");
            }   
        }
    }
}