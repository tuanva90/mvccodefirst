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
            LoadProducts();
            
        }

        private void LoadProducts()
        {
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int rowcount = dtgProduct.Rows.Count;
            try
            {
                DialogResult result = MessageBox.Show("Do you wanna delete rows were select?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    //code for Yes
                    for (int i = 0; i < rowcount; i++)
                    {
                        if (Convert.ToBoolean(dtgProduct.Rows[i].Cells[9].Value) == true)
                        {
                            test.DeleteProduct(int.Parse(dtgProduct.Rows[i].Cells[1].Value.ToString()));
                        }
                    }
                    LoadProducts();
                }
                if(result == DialogResult.No || result == DialogResult.Cancel)
                {
                    for(int i = 0;i < rowcount;i++)
                    {
                        dtgProduct.Rows[i].Cells[9].Value = false;
                    }
                }
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void dtgProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            
        }

        private void dtgProduct_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int CurrentIndex = dtgProduct.CurrentCell.RowIndex;
            NorthwindService.Product _product = new NorthwindService.Product();
            _product.CategoryID = Convert.ToInt32(dtgProduct.Rows[CurrentIndex].Cells[0].Value.ToString());
            _product.ProductID = Convert.ToInt32(dtgProduct.Rows[CurrentIndex].Cells[1].Value.ToString());
            _product.ProductName = Convert.ToString(dtgProduct.Rows[CurrentIndex].Cells[2].Value.ToString());
            _product.QuantityPerUnit = Convert.ToString(dtgProduct.Rows[CurrentIndex].Cells[3].Value.ToString());
            _product.ReorderLevel = Convert.ToInt16(dtgProduct.Rows[CurrentIndex].Cells[4].Value.ToString());
            _product.UnitPrice = Convert.ToDecimal(dtgProduct.Rows[CurrentIndex].Cells[5].Value.ToString());
            _product.UnitsInStock = Convert.ToInt16(dtgProduct.Rows[CurrentIndex].Cells[6].Value.ToString());
            _product.UnitsOnOrder = Convert.ToInt16(dtgProduct.Rows[CurrentIndex].Cells[7].Value.ToString());
            test.UpdateProduct(_product);
        }
    }
}
