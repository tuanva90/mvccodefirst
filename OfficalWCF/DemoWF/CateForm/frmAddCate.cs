﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoWF.CateForm
{
    public partial class frmAddCate : DevComponents.DotNetBar.Office2007Form
    {
        NorthwindService.Service1Client test = new NorthwindService.Service1Client();
        public frmAddCate()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //NorthwindService.Category ca = new NorthwindService.Category();
            //ca.CategoryName = txtCateName.Text;
            //ca.Description = txtCateDescription.Text;
            //ca.Picture = null;
            //int ans = test.AddCategory(ca);
            MessageBox.Show("Add successful !","Add Category",MessageBoxButtons.OK);
        }

        private void frmAddCate_Load(object sender, EventArgs e)
        {
            List<NorthwindService.Category> lscate = new List<NorthwindService.Category>();
            lscate = test.GetAllCategory().ToList();
            categoryBindingSource.DataSource = lscate;
            dtgCategory.DataSource = categoryBindingSource;
        }

        //private void btnSearch_Click(object sender, EventArgs e)
        //{
        //    string cate="Category";
        //    frmSearch search = new frmSearch(cate);
        //    search.ShowDialog();
        //}

        private void btnClean_Click(object sender, EventArgs e)
        {
            List<NorthwindService.Category> lscate = new List<NorthwindService.Category>();
            lscate = test.GetAllCategory().ToList();
            categoryBindingSource.DataSource = lscate;
            dtgCategory.DataSource = categoryBindingSource;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int rowcount = dtgCategory.Rows.Count;
            for (int i = 0; i < rowcount; i++)
            {
                if (Convert.ToBoolean(dtgCategory.Rows[i].Cells[4].Value)==true)
                {
                    MessageBox.Show(dtgCategory.Rows[i].Cells[1].ToString());
                }
            }
                
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string cate = "Category";
            frmSearch search = new frmSearch(cate);
            search.ShowDialog();
        }
    }
}