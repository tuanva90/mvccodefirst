using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using System.Web.Security;
namespace DemoMVCEntityFramework
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Check existence of Database
            //Database.SetInitializer<NorthWNDContext>(new TestInitializer());

            AreaRegistration.RegisterAllAreas();

            //Add dependency
            UDI.UnityDependency.Bootstrapper.Initialise();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {

                //old authentication, kill it
                FormsAuthentication.SignOut();
                //or use Response.Redirect to go to a different page

            }
        }
        
    }
}