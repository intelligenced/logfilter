using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(experiment2.Startup))]
namespace experiment2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
