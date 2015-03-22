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

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                ReloadCateGrid();
            }
            
        }

        private void ReloadCateGrid()
        {
            CateGrid.DataSource = Cate.GetAll();
            CateGrid.DataBind();
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
                    cate.Picture = txtPicture.FileBytes;
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
                    cate.Picture = txtPicture.FileBytes;
                }
                Cate.Edit(cate);
            }

            ReloadCateGrid();
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseAlert", "CloseAlert();", true); 
            
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtCateName.Text = "";
            txtDescription.Text = "";
            //txtPicture.
        }
               
        protected void CateGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var DtRow = e.Row.DataItem as Category;
                Image Img = e.Row.FindControl("CategoryImg") as Image;

                if (DtRow.Picture != null)
                {
                    Img.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(DtRow.Picture);
                }
                else
                {
                    Img.DescriptionUrl = "~/Images/noImages.jpg";
                }
            }
        }

        protected void CateGrid_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void CateGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CateGrid.PageIndex = e.NewPageIndex;
            ReloadCateGrid();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (ListDelteID.Value.Trim().Length == 0)
                return;

            var lsStrID = ListDelteID.Value.Substring(1).Split(',');

            foreach (String item in lsStrID)
            {
                if (item.Trim().Length > 0)
                {
                    var product = Cate.Get(int.Parse(item.Trim())) as Category;
                    Cate.Delete(product);
                }
            }

            ReloadCateGrid();
        }

    }
}