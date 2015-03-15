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

namespace DemoWF.OrderForm
{
    public partial class frmMainOrder : DevComponents.DotNetBar.Office2007Form
    {
        public static string _Cusid;
        NorthwindService.Service1Client test = new NorthwindService.Service1Client();
        public frmMainOrder()
        {
            InitializeComponent();
        }
        public frmMainOrder(string id)
        {
            InitializeComponent();
            _Cusid = id;
        }

        private void frmMainOrder_Load(object sender, EventArgs e)
        {
            List<NorthwindService.Order> lsor = new List<NorthwindService.Order>();
            lsor = test.GetOrder(_Cusid).ToList();
            orderBindingSource.DataSource = lsor;
            dtgOrder.DataSource = orderBindingSource;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            NorthwindService.Order or = new NorthwindService.Order();
            or.CustomerID = _Cusid;
            or.OrderDate = DateTime.Today;
            or.RequireDate = DateTime.Parse(txtDateRequired.Text);
            or.ShippedDate = DateTime.Parse(txtDateShipped.Text);
            or.Freight = decimal.Parse(txtFreight.Text);
            int a = test.AddOrder(or);
            if (a == 1)
            {
                MessageBox.Show("Add successful !", "Add Category", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Add failed !", "Add Category", MessageBoxButtons.OK);
            }
        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
            frmSearch search = new frmSearch(_Cusid);
            search.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int rowcount = dtgOrder.Rows.Count;
            try
            {
                for (int i = 0; i < rowcount; i++)
                {
                    if (Convert.ToBoolean(dtgOrder.Rows[i].Cells[7].Value) == true)
                    {
                        int a = test.DeleteCategory(int.Parse(dtgOrder.Rows[i].Cells[0].Value.ToString()));
                        if (a == 1)
                        {
                            MessageBox.Show("Delete Susscess");
                        }
                        else
                            MessageBox.Show("Error!");
                    }
                }
            }
            catch(Exception ex)
            {
                ex.ToString();
            }
            
        }

        private void dtgOrder_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            NorthwindService.Order or = new NorthwindService.Order();
            int CurrentIndex = dtgOrder.CurrentCell.RowIndex;
            try
            {
                or.OrderID = Convert.ToInt32(dtgOrder.Rows[CurrentIndex].Cells[0].Value.ToString());
                or.CustomerID = _Cusid;
                or.OrderDate = Convert.ToDateTime(dtgOrder.Rows[CurrentIndex].Cells[2].Value.ToString());
                or.RequireDate = Convert.ToDateTime(dtgOrder.Rows[CurrentIndex].Cells[3].Value.ToString());
                or.ShippedDate = Convert.ToDateTime(dtgOrder.Rows[CurrentIndex].Cells[4].Value.ToString());
                or.ShipVia = Convert.ToInt32(dtgOrder.Rows[CurrentIndex].Cells[5].Value.ToString());
                or.Freight = Convert.ToDecimal(dtgOrder.Rows[CurrentIndex].Cells[6].Value.ToString());
                test.UpdateOrder(or);
            }
           catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
                      
        }
    }
}
