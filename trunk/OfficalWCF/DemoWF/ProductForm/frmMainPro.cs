using DemoWF.CateForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoWF.ProductForm
{
    public partial class frmMainPro : DevComponents.DotNetBar.Office2007Form
    {
        NorthwindService.Service1Client test = new NorthwindService.Service1Client();
        public frmMainPro()
        {
            InitializeComponent();
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            NorthwindService.Product pro = new NorthwindService.Product();
            pro.CategoryID = int.Parse(cbCategory.SelectedValue.ToString());
            pro.ProductName = txtProductName.Text;
            pro.QuantityPerUnit = txtProductQuantity.Text;
            pro.UnitPrice = decimal.Parse(txtPrice.Text);
            pro.UnitsInStock = int.Parse(txtInStock.Text);
            pro.UnitsOnOrder = int.Parse(txtOnOrder.Text);
            pro.ReorderLevel = int.Parse(txtReorder.Text);
            pro.Discontinued = ckbDiscontinue.Checked;
            int a = test.AddProduct(pro);
            if (a == 1)
            {
                MessageBox.Show("Add successful !", "Add Category", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Add failed !", "Add Category", MessageBoxButtons.OK);
            }
        }

        private void frmMainPro_Load(object sender, EventArgs e)
        {
            List<NorthwindService.Category> lscate = new List<NorthwindService.Category>();
            lscate = test.GetAllCategory().ToList();
            categoryBindingSource.DataSource = lscate;
            cbCategory.DataSource = categoryBindingSource;
            cbCategory.ValueMember = "CategoryID";
            cbCategory.DisplayMember = "CategoryName";

            List<NorthwindService.Product> lspro = new List<NorthwindService.Product>();
            lspro = test.GetAllProduct().ToList();
            productBindingSource.DataSource = lspro;
            dtgProduct.DataSource = productBindingSource;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string pro = "Product";
            frmSearch search = new frmSearch(pro);
            search.ShowDialog();
        }
    }
}
