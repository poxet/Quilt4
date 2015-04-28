using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Quilt4.Interface;
using Quilt4.MongoDBRepository;

//using Quilt4.SQLRepository;
//using Quilt4.SQLRepository.Business;

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

            //container.Register(Classes.FromThisAssembly().InNamespace("Eplicta.MediaMapper.Web.Agents").WithService.DefaultInterfaces().LifestyleTransient());
            //container.Register(Component.For<IOwinContextAgent>().ImplementedBy<OwinContextAgent>());            

            //container.Register(Classes.FromThisAssembly().InNamespace("Quilt4.Web.Business").WithService.DefaultInterfaces().LifestyleTransient());
            //container.Register(Classes.FromThisAssembly().InNamespace("Quilt4.SQLRepository.Business").WithService.DefaultInterfaces().LifestyleTransient());
            //TODO: Switch between SQLDatabase and MontoDB by only using configuration
            //container.Register(Component.For<IRepositoryFactory>().ImplementedBy<SqlRepositoryFactory>());
            container.Register(Component.For<IRepositoryFactory>().ImplementedBy<MongoDbRepositoryFactory>());
            container.Register(Component.For<IAccountBusiness>().ImplementedBy<AccountBusiness>());

            // Register all the MVC controllers in the current executing assembly
            var contollers = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(Controller) || x.BaseType == typeof(ApiController)).ToList();
            foreach (var controller in contollers)
            {
                container.Register(Component.For(controller).LifestylePerWebRequest());
            }
        }
    }
}