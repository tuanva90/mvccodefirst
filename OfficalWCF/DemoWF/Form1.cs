using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoWF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            ServiceReference2.Service1Client age = new ServiceReference2.Service1Client();
            int id = int.Parse(txtInputID.Text);
            var s = age.Test1(id);
            txtID.Text = s.CategoryID.ToString();
            txtName.Text = s.CategoryName;
            txtDescription.Text = s.Description;
            }
        }
}

