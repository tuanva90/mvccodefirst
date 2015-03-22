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
        public IOrderService _ord { get; set; }

        [Microsoft.Practices.Unity.Dependency]
        public IUserService _usr { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = _usr.GetLoginUser(Context.User.Identity.Name); // db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                var orders = _ord.GetListOrder(user.CustomerID); // db.Orders.Where(o => o.CustomerID == user.CustomerID).ToList();
                dtgOrder.DataSource = orders;
                dtgOrder.DataBind();
            }
            else
            {
                Response.Redirect("Account/Login.aspx");
            }
            
        }

        protected void dtgOrder_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "DetailCmd")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "callModal", "callModal();", true);
            }
        }
    }
}