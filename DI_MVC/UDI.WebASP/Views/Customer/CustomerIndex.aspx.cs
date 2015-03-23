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
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    Response.Redirect("~/Account/Login.aspx");
                    return;
                }

                var userlogin = Usr.GetLoginUser(User.Identity.Name);
                var cus = Cus.Get(userlogin.CustomerID);
                if (cus != null)
                {
                    CustomerID.Value = cus.CustomerID.ToString();
                    ContactName.Text = cus.ContactName;
                    ContactTitle.Text = cus.ContactTitle;
                    Address.Text = cus.Address;
                    City.Text = cus.City;
                    Phone.Text = cus.Phone;
                    Region.Text = cus.Region;
                    Region.Text = cus.Fax;
                    PostalCode.Text = cus.PostalCode;
                    Country.Text = cus.Country;
                    Fax.Text = cus.Fax;
                    btnCreat.Text = "Update";
                }
                else
                {
                    CustomerID.Value = "";
                    ContactName.Text = "";
                    ContactTitle.Text = "";
                    Address.Text = "";
                    City.Text = "";
                    Phone.Text = "";
                    Region.Text = "";
                    Region.Text = "";
                    PostalCode.Text = "";
                    Country.Text = "";
                    Fax.Text = "";
                    btnCreat.Text = "Create";
                }
            }
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
            customer.Fax = Fax.Text;
            
            var loginUser = Usr.GetLoginUser(Context.User.Identity.GetUserName()); // db.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault();
            if (loginUser != null)
            {
                customer.CustomerID = loginUser.CustomerID;
            }

            if (ModelState.IsValid)
            {
                if (CustomerID.Value.Trim().Length > 0)
                {
                    Cus.Edit(customer);
                }
                else
                {
                    Cus.Add(customer);
                }
                
                Response.Redirect("~/Views/Product/ProductIndex.aspx");
            }   
        }
    }
}