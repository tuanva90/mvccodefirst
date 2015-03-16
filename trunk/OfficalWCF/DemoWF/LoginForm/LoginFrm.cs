using DemoWF.OrderForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoWF.LoginForm
{
    public partial class LoginFrm : DevComponents.DotNetBar.Office2007Form
    {
        public static string CusID;
        public LoginFrm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            NorthwindService.Service1Client northwind = new NorthwindService.Service1Client();
            CusID = northwind.GetCustomer(txtCusID.Text,txtCusPassword.Text);
            if (CusID != null)
            {
                //frmMainOrder order = new frmMainOrder(CusID);
                //order.MdiParent = this.
                //order.Show();
                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            else
                lblStatus.Text = "Invalid ID or Password ";
        }
    }
}
