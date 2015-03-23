using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDI.CORE.Entities;
using UDI.CORE.Services;
using UDI.CORE.Services.Impl;
using UDI.EF.DAL;

namespace UDI.WebASP.Views.Product
{
    public partial class ProductIndex : System.Web.UI.Page
    {
        [Microsoft.Practices.Unity.Dependency]
        public ICategoryService Cate { get; set; }

        [Microsoft.Practices.Unity.Dependency]
        public IProductService Prod { get; set; }

        //public ProductIndex()
        //{
        //    Cate = new CategoryService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
        //    Prod = new ProductService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                LoadCateogries();
                LoadProduct();
            }
        }

        private void LoadCateogries()
        {
            List<Category> lsCate = new List<Category>();
            lsCate.Add(new Category { CategoryID = 0, CategoryName = "All", Description = "All" });
            lsCate.AddRange(Cate.GetAll());
            DropDownList1.DataSource = lsCate;
            DropDownList1.DataTextField = "CategoryName";
            DropDownList1.DataValueField = "CategoryID";
            DropDownList1.DataBind();
            DropDownList1.SelectedIndex = 0;
            CategoriesDDL2.DataSource = Cate.GetAll();
            CategoriesDDL2.DataTextField = "CategoryName";
            CategoriesDDL2.DataValueField = "CategoryID";
            CategoriesDDL2.DataBind(); 
        }

        private void LoadProduct()
        {
            if (int.Parse(DropDownList1.SelectedItem.Value) == 0)
            {
                ProductGridView.DataSource = Prod.GetAll();
                ProductGridView.DataBind();
            }
            else
            {
                ProductGridView.DataSource = Prod.GetAll(int.Parse(DropDownList1.SelectedItem.Value));
                ProductGridView.DataBind();
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {

        }

        protected void btnCrPro_Click(object sender, EventArgs e)
        {
            var product = new UDI.CORE.Entities.Product();
            product.ProductName = ProductNameTextBox.Text;
            product.CategoryID = int.Parse(CategoriesDDL2.SelectedItem.Value);
            product.QuantityPerUnit = QuantityPerUnitTextBox.Text;
            product.UnitPrice = (int)float.Parse(UnitPriceTextBox.Text);
            product.UnitsInStock = int.Parse(UnitsInStockTextBox.Text);
            product.UnitsOnOrder = int.Parse(UnitsOnOrderTextBox.Text);
            product.ReorderLevel = int.Parse(ReorderLevelTextBox.Text);
            product.Quantity = int.Parse(QuantityTextBox.Text);
            product.Discontinued = DiscontinueCheckBox.Checked;
            if(ModelState.IsValid)
            {
                var prodID = ProductIDHidenField.Value;
                if (prodID.Trim().Length == 0)
                {
                    Prod.Add(product);                    
                }
                else
                {
                    product.ProductID = int.Parse(prodID);
                    Prod.Edit(product);
                }
            }

            ClearControl();
            LoadProduct();
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseAlert", "CloseAlert();", true);
        }

        protected void ClearControl()
        {
            ProductIDHidenField.Value = "";
            ProductNameTextBox.Text = "";
            QuantityTextBox.Text = "";
            QuantityPerUnitTextBox.Text = "";
            UnitPriceTextBox.Text = "";
            UnitsInStockTextBox.Text = "";
            ReorderLevelTextBox.Text = "";
            DiscontinueCheckBox.Checked = false;
            OrderQuantityTextBox.Visible = false;
            AddOrderBtn.Visible = false;

        }

        protected void AddOrderBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                Response.Redirect("~/Account/Login.aspx");

            if (ProductIDHidenField.Value.Trim().Length == 0 || OrderQuantityTextBox.Text.Trim().Length == 0)
                return;

            var proID = int.Parse(ProductIDHidenField.Value);
            var num = int.Parse(OrderQuantityTextBox.Text.Trim());
            if (proID > 0 && num > 0)
            {
                var pro = Prod.Get((int)proID); // _uow.Repository<Product>().Get(p => p.ProductID == proID);
                pro.Quantity = (int)num;
                var ss = Session["Order_" + User.Identity.Name];
                if (ss == null)
                {
                    List<UDI.CORE.Entities.Product> prolist = new List<UDI.CORE.Entities.Product>();
                    prolist.Add(pro);
                    Session["Order_" + User.Identity.Name] = prolist;
                }
                else
                {
                    List<UDI.CORE.Entities.Product> prolist = (List<UDI.CORE.Entities.Product>)ss;
                    prolist.Add(pro);
                    Session["Order_" + User.Identity.Name] = prolist;
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", "ShowMessage('Added successed!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", "ShowMessage('Please input quantity!');", true);
            }
        }

        protected void ProductGridView_SelectedIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ProductGridView.PageIndex = e.NewPageIndex;
            LoadProduct();
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            var prodID = SelectedItem.Value;
            if (prodID.Trim().Length == 0)
                return;

            var prodItem = Prod.Get(int.Parse(prodID));
            if (prodItem != null)
            {
                ProductIDHidenField.Value = prodItem.ProductID.ToString();
                ProductNameTextBox.Text = prodItem.ProductName;
                CategoriesDDL2.SelectedIndex = CategoriesDDL2.Items.IndexOf(new ListItem(prodItem.Category.CategoryName, prodItem.Category.CategoryID.ToString()));
                QuantityPerUnitTextBox.Text = prodItem.QuantityPerUnit;
                UnitPriceTextBox.Text = prodItem.UnitPrice.ToString();
                UnitsInStockTextBox.Text = prodItem.UnitsInStock.ToString();
                UnitsOnOrderTextBox.Text = prodItem.UnitsOnOrder.ToString();
                ReorderLevelTextBox.Text = prodItem.ReorderLevel.ToString();
                QuantityTextBox.Text = prodItem.Quantity.ToString();
                DiscontinueCheckBox.Checked = prodItem.Discontinued;
                OrderQuantityTextBox.Text = "1";
                OrderQuantityTextBox.Visible = true;
                AddOrderBtn.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenAlert", "OpenAlert();", true);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (ListDelteID.Value.Trim().Length == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", "ShowMessage('Select at least 1 product!');", true);
                return;
            }

            var lsStrID = ListDelteID.Value.Substring(1).Split(',');

            foreach (String item in lsStrID)
            {
                if (item.Trim().Length > 0)
                {
                    var product = Prod.Get(int.Parse(item.Trim())) as UDI.CORE.Entities.Product;
                    Prod.Delete(product);
                }
            }
            ListDelteID.Value = "";
            LoadProduct();
        }
    }
}