using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDI.CORE.Services;
using UDI.CORE.Services.Impl;
using UDI.EF.DAL;

namespace UDI.WebASP.Views.Product
{
    public partial class ProductIndex : System.Web.UI.Page
    {
        private ICategoryService _cate;
        private IProductService _pro;

        public ProductIndex()
        {
            _cate = new CategoryService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
            _pro = new ProductService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
               dtgProduct.DataSource = _pro.GetAll();
               dtgProduct.DataBind();
               DropDownList1.DataSource = _cate.GetAll();            
               DropDownList1.DataTextField = "CategoryName";
               DropDownList1.DataValueField = "CategoryID";
               DropDownList1.DataBind();
               DropDownList1.Text = "All";
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtgProduct.DataSource = _pro.GetAll(int.Parse(DropDownList1.SelectedItem.Value));
            dtgProduct.DataBind();
        }
    }
}