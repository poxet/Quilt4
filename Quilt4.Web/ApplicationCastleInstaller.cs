using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Quilt4.Interface;
using Quilt4.SQLRepository;

namespace Quilt4.Web
{
    public class ApplicationCastleInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Register working dependencies
            //container.Register(Component.For().ImplementedBy().LifestylePerWebRequest());

            // Register the MVC controllers one by one
            // container.Register(Component.For().LifestylePerWebRequest());

            container.Register(Classes.FromThisAssembly().InNamespace("Eplicta.MediaMapper.Web.Agents").WithService.DefaultInterfaces().LifestyleTransient());

            //TODO: Switch between SQLDatabase and MontoDB by only using configuration
            container.Register(Component.For<IRepositoryFactory>().ImplementedBy<SqlRepositoryFactory>());

            // Register all the MVC controllers in the current executing assembly
            var contollers = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(Controller) || x.BaseType == typeof(ApiController)).ToList();
            foreach (var controller in contollers)
            {
                container.Register(Component.For(controller).LifestylePerWebRequest());
            }
        }
    }
}