using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;

namespace Quilt4.Web
{
    public class MvcApplication : HttpApplication
    {
        private readonly static WindsorContainer _container;

        static MvcApplication()
        {
            _container = new WindsorContainer();
        }

        public static WindsorContainer Container { get { return _container; } }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterWindsor();
        }

        private static void RegisterWindsor()
        {
            //_container = new WindsorContainer();
            _container.Install(new ApplicationCastleInstaller());

            // Create the Controller Factory
            var castleControllerFactory = new CastleControllerFactory(_container);

            // Add the Controller Factory into the MVC web request pipeline
            ControllerBuilder.Current.SetControllerFactory(castleControllerFactory);
        }
    }
}
