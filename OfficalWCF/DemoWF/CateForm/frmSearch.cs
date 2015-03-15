using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar;

namespace DemoWF.CateForm
{
    public partial class frmSearch : DevComponents.DotNetBar.Office2007RibbonForm
    {
        NorthwindService.Service1Client test = new NorthwindService.Service1Client();
        public string cusid = "";
        public frmSearch()
        {
            InitializeComponent();
        }

        public frmSearch(string strType)
        {
            InitializeComponent();
            
            if(strType == "Category")
            {
                Catetab.Visible = true;
                Catetab.Focus();
                Protab.Visible = false;
                Ordertab.Visible = false;
            }
            else if (strType == "Product")
            {
                Protab.Visible = true;
                Protab.Focus();
                Catetab.Visible = false;
                Ordertab.Visible = false;
            }
            else
            {
                cusid = strType;
                Ordertab.Focus();
                Ordertab.Visible = true;
                Catetab.Visible = false;
                Protab.Visible = false;
            }
            
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {

        }

        private void btnSearchCate_Click(object sender, EventArgs e)
        {
            List<NorthwindService.Category> lscate = new List<NorthwindService.Category>();
            lscate = test.GetCategogyByName(txtCateSearch.Text).ToList();
            categoryBindingSource.DataSource = lscate;
            dtgCategory.DataSource = categoryBindingSource;
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            List<NorthwindService.Product> lspro = new List<NorthwindService.Product>();
            lspro = test.GetProductByName(txtSearchPro.Text).ToList();
            productBindingSource.DataSource = lspro;
            dtgProduct.DataSource = productBindingSource;
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            List<NorthwindService.Order> lsorder = new List<NorthwindService.Order>();
            lsorder = test.GetListOrderByDate(cusid, DateTime.Parse(txtDateFrom.Text), DateTime.Parse(txtDateTo.Text)).ToList();
            orderBindingSource.DataSource = lsorder;
            dtgOrder.DataSource = orderBindingSource;
        }
    }
}
