using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using Tharga.Quilt4Net;

namespace Quilt4.Web
{
    public class MvcApplication : HttpApplication
    {
        private readonly static IWindsorContainer _container;

        static MvcApplication()
        {
            _container = new WindsorContainer();
        }

        public static IWindsorContainer Container { get { return _container; } }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterWindsor();

            //GlobalConfiguration.Configuration.MessageHandlers.Add(new WebApiCallLogHandler(new SettingsBusiness())); //TODO: Resolve the SettingsBusiness instead

            Tharga.Quilt4Net.Session.RegisterCompleteEvent += Session_RegisterCompleteEvent;
            Tharga.Quilt4Net.Session.BeginRegister(Assembly.GetAssembly(typeof(MvcApplication)));
        }

        void Session_RegisterCompleteEvent(object sender, Session.RegisterCompleteEventArgs e)
        {
            Debug.Write(e.Success);
            //TODO: Log problems to the event log
        }

        private static void RegisterWindsor()
        {
            //_container = new WindsorContainer();
            _container.Install(new ApplicationCastleInstaller());
            //_container.Install(new WindsorCompositionRoot());

            // Create the Controller Factory
            var castleControllerFactory = new CastleControllerFactory(_container);

            // Add the Controller Factory into the MVC web request pipeline
            ControllerBuilder.Current.SetControllerFactory(castleControllerFactory);

            //TODO: Is this line needed for web api calls?
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(_container));
        }
    }
}