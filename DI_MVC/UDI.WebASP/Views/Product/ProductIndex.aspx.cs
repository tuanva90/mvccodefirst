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
               DDL2.DataSource = _cate.GetAll();
               DDL2.DataTextField = "CategoryName";
               DDL2.DataValueField = "CategoryID";
               DDL2.DataBind();
 
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtgProduct.DataSource = _pro.GetAll(int.Parse(DropDownList1.SelectedItem.Value));
            dtgProduct.DataBind();
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {

        }

        protected void btnCrPro_Click(object sender, EventArgs e)
        {
            var product = new UDI.CORE.Entities.Product();
            product.ProductName = productName.Text;
            product.CategoryID = int.Parse(DDL2.SelectedItem.Value);
            product.QuantityPerUnit = QuantityPerUnit.Text;
            product.UnitPrice = int.Parse(UnitPrice.Text);
            product.UnitsInStock = int.Parse(UnitsInStock.Text);
            product.UnitsOnOrder = int.Parse(UnitsOnOrder.Text);
            product.ReorderLevel = int.Parse(ReorderLevel.Text);
            product.Quantity = int.Parse(Quantity.Text);
            product.Discontinued = chkDiscontinue.Checked;
            if(ModelState.IsValid)
            {
                _pro.Add(product);
                ClearControl();
                dtgProduct.DataSource = _pro.GetAll();
                dtgProduct.DataBind();
            }           
        }

        protected void ClearControl()
        {
            productName.Text = "";
            Quantity.Text = "";
            QuantityPerUnit.Text = "";
            UnitPrice.Text = "";
            UnitsInStock.Text = "";
            ReorderLevel.Text = "";
            chkDiscontinue.Checked = false;

        }
    }
}