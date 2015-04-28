using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Quilt4.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new WindsorContainer();
            container.Install(new ApplicationCastleInstaller());

            // Create the Controller Factory
            var castleControllerFactory = new CastleControllerFactory(container);

            // Add the Controller Factory into the MVC web request pipeline
            ControllerBuilder.Current.SetControllerFactory(castleControllerFactory);
        }
    }

    public class ApplicationCastleInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Register working dependencies
            //container.Register(Component.For().ImplementedBy().LifestylePerWebRequest());

            // Register the MVC controllers one by one
            // container.Register(Component.For().LifestylePerWebRequest());

            container.Register(Classes.FromThisAssembly().InNamespace("Eplicta.MediaMapper.Web.Agents").WithService.DefaultInterfaces().LifestyleTransient());

            // Register all the MVC controllers in the current executing assembly
            var contollers = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(Controller) || x.BaseType == typeof(ApiController)).ToList();
            foreach (var controller in contollers)
            {
                container.Register(Component.For(controller).LifestylePerWebRequest());
            }
        }
    }

    public class CastleControllerFactory : DefaultControllerFactory
    {
        public IWindsorContainer Container { get; protected set; }

        public CastleControllerFactory(IWindsorContainer container)
        {
            if (container == null)
            {
                //System.Configuration.ConfigurationManager.AppSettings
                throw new ArgumentNullException("container");
            }

            Container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return null;
            }

            // Retrieve the requested controller from Castle
            return Container.Resolve(controllerType) as IController;
        }

        public override void ReleaseController(IController controller)
        {
            // If controller implements IDisposable, clean it up responsibly
            var disposableController = controller as IDisposable;
            if (disposableController != null)
            {
                disposableController.Dispose();
            }

            // Inform Castle that the controller is no longer required
            Container.Release(controller);
        }
    }
}
