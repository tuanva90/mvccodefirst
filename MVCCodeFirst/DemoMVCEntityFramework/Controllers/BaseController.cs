using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMVCEntityFramework.Controllers
{
    public class BaseController : Controller
    {
        private const string CookieName = "MyCookieMVC";

        public void SetLoginCookie(string UserName)
        {
            HttpCookie myCookie = System.Web.HttpContext.Current.Request.Cookies[CookieName] ?? new HttpCookie(CookieName);
            myCookie.Values["UserName"] = UserName;
            myCookie.Expires = DateTime.Now.AddDays(365);
            System.Web.HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        public String GetLoginUser()
        {
            HttpCookie myCookie = System.Web.HttpContext.Current.Request.Cookies[CookieName];
            if (myCookie != null)
            {
                var UserName = myCookie.Values["UserName"];
                return UserName;
            }
            return "";
        }


        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }
    }
}
