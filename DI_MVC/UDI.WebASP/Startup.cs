using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UDI.WebASP.Startup))]
namespace UDI.WebASP
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
