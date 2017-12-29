using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TitaniumForum.Web.Startup))]

namespace TitaniumForum.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}