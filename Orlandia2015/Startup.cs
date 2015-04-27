using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Orlandia2015.Startup))]
namespace Orlandia2015
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
