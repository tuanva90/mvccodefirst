using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDI.CORE.Entities;
using UDI.CORE.Services;
using UDI.CORE.Services.Impl;
using UDI.EF.DAL;

namespace UDI.WebASP.Views.Category
{
    public partial class CategoryIndex : System.Web.UI.Page
    {
        private ICategoryService _cate;

        public CategoryIndex()
        {
            _cate = new CategoryService(new UDI.EF.UnitOfWork.EFUnitOfWork(new EFContext()));
        }

        //public CategoryIndex(ICategoryService cate)
        //{
        //    _cate = cate;
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
            _cate.GetAll();
            CateGrid.DataSource = _cate.GetAll();
            CateGrid.DataBind();
        }
    }
}