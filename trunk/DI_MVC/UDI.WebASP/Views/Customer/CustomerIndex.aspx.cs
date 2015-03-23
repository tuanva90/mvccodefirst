﻿using System;
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
        [Microsoft.Practices.Unity.Dependency]
        public ICustomerService Cus { get; set; }

        [Microsoft.Practices.Unity.Dependency]
        public IUserService Usr { get; set; }

        //public CustomerIndex()
        //{
        //    Cus = new CustomerService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
        //    Usr = new UserService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                Response.Redirect("~/Account/Login.aspx");
        }

        protected void btnCreat_Click(object sender, EventArgs e)
        {
            var customer = new UDI.CORE.Entities.Customer();
            customer.ContactName = ContactName.Text;
            customer.ContactTitle = ContactTitle.Text;
            customer.Address = Address.Text;
            customer.City = City.Text;
            customer.Phone = Phone.Text;
            customer.Region = Region.Text;
            customer.Fax = Fax.Text;
            customer.PostalCode = PostalCode.Text;
            customer.Country = Country.Text;

            
            var loginUser = Usr.GetLoginUser(Context.User.Identity.GetUserName()); // db.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault();
            if (loginUser != null)
            {
                customer.CustomerID = loginUser.CustomerID;
            }

            if (ModelState.IsValid)
            {
                Cus.Add(customer);
                Response.Redirect("~/Views/Product/ProductIndex.aspx");
            }   
        }
    }
}