﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.WinForms;
using DemoWF.CateForm;
using DemoWF.LoginForm;
using DemoWF.OrderForm;
using DemoWF.ProductForm;

namespace DemoWF
{
    public partial class MainForm : DevComponents.DotNetBar.Office2007Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            IsMdiContainer = true;
        }

        private void addCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddCate addcate = new frmAddCate();
            addcate.Show();
            addcate.MdiParent = this;
        }

        private void searchCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSearch search = new frmSearch();
            search.ShowDialog();
            search.MdiParent = this;
        }

        private void addOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string cusid;
            cusid = LoginFrm.CusID;
            frmMainOrder or = new frmMainOrder(cusid);
            or.MdiParent = this;
            or.Show();
        }

        private void addProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMainPro pro = new frmMainPro();
            pro.MdiParent = this;
            pro.Show();
        }
    }
}
