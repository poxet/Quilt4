using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

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

            //container.Register(Classes.FromThisAssembly().InNamespace("Quilt4.Interface").WithService.DefaultInterfaces().LifestyleTransient());
            container.Register(Classes.FromThisAssembly().InNamespace("Quilt4.Web.Business").WithService.DefaultInterfaces().LifestyleTransient());
            container.Register(Classes.FromThisAssembly().InNamespace("Quilt4.Web.Agents").WithService.DefaultInterfaces().LifestyleTransient());
            container.Register(Classes.FromThisAssembly().InNamespace("Quilt4.Web.BusinessEntities").WithService.DefaultInterfaces().LifestyleTransient());            

            var repository = System.Configuration.ConfigurationManager.AppSettings["Repository"];
            RegisterRepository(container, repository);

            // Register all the MVC controllers in the current executing assembly
            var contollers = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(Controller) || x.BaseType == typeof(ApiController)).ToList();
            foreach (var controller in contollers)
            {
                if (controller.Name == "ActionController")
                {
                    container.Register(Component.For(controller).LifestyleTransient());
                }
                else
                {
                    container.Register(Component.For(controller).LifestylePerWebRequest());
                }
            }
        }

        private static void RegisterRepository(IWindsorContainer container, string repository)
        {
            //var assembly = Assembly.GetAssembly(typeof(MultiRepository.MultiRepository));
            var assembly = Assembly.GetAssembly(typeof(MongoDBRepository.MongoRepository));
            container.Register(Classes.FromAssembly(assembly).InNamespace(repository).WithService.DefaultInterfaces().LifestyleSingleton());

            //var file = string.Format("{0}bin\\{1}.dll", AppDomain.CurrentDomain.BaseDirectory, repository);
            //if (!System.IO.File.Exists(file))
            //{
            //    throw new InvalidOperationException(string.Format("The repository file {0} cannot be found.", file));
            //}

            //var assembly = Assembly.LoadFrom(file);
            //container.Register(Classes.FromAssembly(assembly).InNamespace(repository).WithService.DefaultInterfaces().LifestyleSingleton());
        }
    }
}