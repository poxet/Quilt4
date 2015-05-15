using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Quilt4.Interface;
using Quilt4.SQLRepository.Membership;

namespace Quilt4.SQLRepository
{
    public class RepositoryHandler : IRepositoryHandler
    {
        private ApplicationUserManager _applicationUserManager;
        private ApplicationSignInManager _applicationSignInManager;

        public object GetApplicationUserManager() { return _applicationUserManager; }
        public object GetApplicationSignInManager() { return _applicationSignInManager; }

        private void RegisterApplicationUserManager(IAppBuilder app)
        {
            ApplicationUserManager.ApplicationUserManagerCreatedEvent += ApplicationUserManager_ApplicationUserManagerCreatedEvent;
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
        }

        void ApplicationUserManager_ApplicationUserManagerCreatedEvent(object sender, ApplicationUserManagerCreatedEventArgs e)
        {
            _applicationUserManager = e.ApplicationUserManager;
        }

        private void RegisterApplicationDbContext(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
        }

        private void RegisterApplicationSignInManager(IAppBuilder app)
        {
            ApplicationSignInManager.ApplicationSignInManagerCreatedEvent += ApplicationSignInManager_ApplicationSignInManagerCreatedEvent;
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
        }

        void ApplicationSignInManager_ApplicationSignInManagerCreatedEvent(object sender, ApplicationSignInManagerCreatedEventArgs e)
        {
            _applicationSignInManager = e.ApplicationSignInManager;
        }

        public void Register(IAppBuilder app)
        {
            RegisterApplicationDbContext(app);
            RegisterApplicationUserManager(app);
            RegisterApplicationSignInManager(app);
        }

        public Func<CookieValidateIdentityContext, Task> OnValidateIdentity()
        {
            return SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(validateInterval: TimeSpan.FromMinutes(30), regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager));
        }
    }
}