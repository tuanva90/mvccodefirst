using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UDI.CORE.Services;

namespace UDI.WebASP.Views.Order
{
    public partial class OderIndex : System.Web.UI.Page
    {
        [Microsoft.Practices.Unity.Dependency]
        public IOrderService Ord { get; set; }

        [Microsoft.Practices.Unity.Dependency]
        public IUserService Usr { get; set; }

        [Microsoft.Practices.Unity.Dependency]
        public ICustomerService Cus { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    Response.Redirect("~/Account/Login.aspx");
                    return;
                }

                LoadData();
            }
            
        }

        private void LoadData()
        {
            var user = Usr.GetLoginUser(Context.User.Identity.Name); // db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                //Show current order
                LoadCurProduct();

                var orders = Ord.GetListOrder(user.CustomerID); // db.Orders.Where(o => o.CustomerID == user.CustomerID).ToList();
                dtgOrder.DataSource = orders;
                dtgOrder.DataBind();
            }
            else
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }

        protected void dtgOrder_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "DetailCmd")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "callModal", "callModal();", true);
                string ID = e.Item.Cells[1].Text;
                UDI.CORE.Entities.Order order = new CORE.Entities.Order();
                order = Ord.Get(int.Parse(ID));
                dtgOrderDetail.DataSource = order.OrderDetails;
                dtgOrderDetail.DataBind();
            }
        }

        protected void CurOrderGridView_SelectedIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CurOrderGridView.PageIndex = e.NewPageIndex;
            LoadCurProduct();
        }

        private void LoadCurProduct()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                Response.Redirect("~/Account/Login.aspx");

            var ss = Session["Order_" + User.Identity.Name];
            if (ss == null)
            {
                AddOrderBtn.Visible = false;
                CurOrderGridView.DataSource = null;
                CurOrderGridView.DataBind();
            }
            else
            {
                AddOrderBtn.Visible = true;
                List<UDI.CORE.Entities.Product> prolist = (List<UDI.CORE.Entities.Product>)ss;
                CurOrderGridView.DataSource = prolist;
                CurOrderGridView.DataBind();
            }
        }

        protected void AddOrderBtn_Click(object sender, EventArgs e)
        {
            var ss = Session["Order_" + User.Identity.Name];

            if (ss != null)
            {
                 var userlogin = Usr.GetLoginUser(User.Identity.Name);
                var cus = Cus.Get(userlogin.CustomerID); // db.Customers.Find(id);
                var curOrder = (List<UDI.CORE.Entities.Product>)Session["Order_" + User.Identity.Name];
                if (curOrder != null & cus != null)
                {
                    var rsl = Ord.AddOrder(cus, curOrder);
                    Session["Order_" + User.Identity.Name] = null;

                    if (rsl == 0)
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", "ShowMessage('Add failed!');", true);
                    else
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", "ShowMessage('Add successed!');", true);
                }
                else
                {
                    Response.Redirect("~/Account/Login.aspx");
                }
            }

            LoadData();
        }
    }
}