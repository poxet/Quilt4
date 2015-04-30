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
        private void RegisterApplicationUserManager(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
        }

        private void RegisterApplicationDbContext(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
        }

        private void RegisterApplicationSignInManager(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
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