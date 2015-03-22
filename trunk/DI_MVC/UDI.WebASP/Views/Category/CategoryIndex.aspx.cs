using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDI.CORE.Entities;
using UDI.CORE.Services;
using UDI.CORE.Services.Impl;
using UDI.EF.DAL;

namespace UDI.WebASP.Views.Categories
{
    public partial class CategoryIndex : System.Web.UI.Page
    {
        [Microsoft.Practices.Unity.Dependency]
        public ICategoryService Cate { get; set; }

        //public CategoryIndex()
        //{
        //    Cate = new CategoryService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                //_cate.GetAll();
                CateGrid.DataSource = Cate.GetAll();
                CateGrid.DataBind();
            }
            
        }
        public Category Getpro(int id)
        {
            var ca = Cate.Get(id);
            return ca;
        }
        //protected void btnAdd_Click(object sender, EventArgs e)
        //{
        //    if(!IsPostBack)
        //    {
        //        lblModalTitle.Text = "Add new category";
        //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
        //        upModal.Update();
        //    }
        //}

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if(txtCateID.Text == "")
            {
                Category cate = new Category();
                cate.CategoryName = txtCateName.Text;
                cate.Description = txtDescription.Text;
                if (txtPicture.HasFile == true)
                {
                    var file = Request.Files["txtPicture"];
                    if (file != null && file.ContentLength > 0)
                    {
                        var red = new BinaryReader(file.InputStream);
                        cate.Picture = red.ReadBytes(file.ContentLength);
                    }
                }
                Cate.Add(cate);
            }
            else
            {
                Category cate = new Category();
                cate.CategoryID = int.Parse(txtCateID.Text);
                cate.CategoryName = txtCateName.Text;
                cate.Description = txtDescription.Text;
                if (txtPicture.HasFile == true)
                {
                    var file = Request.Files["txtPicture"];
                    if (file != null && file.ContentLength > 0)
                    {
                        var red = new BinaryReader(file.InputStream);
                        cate.Picture = red.ReadBytes(file.ContentLength);
                    }
                }
                Cate.Edit(cate);
            }
            
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtCateName.Text = "";
            txtDescription.Text = "";
            //txtPicture.
        }
    }
}