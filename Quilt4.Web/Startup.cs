using Microsoft.Owin;
using Owin;
using Quilt4.Interface;
using Quilt4.Web;

[assembly: OwinStartup(typeof(Startup))]
namespace Quilt4.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var factory = MvcApplication.Container.Resolve<IRepositoryHandler>();
            ConfigureAuth(app, factory);
        }
    }
}