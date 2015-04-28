using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Quilt4.Interface;

namespace Quilt4.SQLRepository
{
    public class SqlRepositoryFactory : IRepositoryFactory
    {
        public void RegisterApplicationUserManager(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
        }

        public void RegisterApplicationDbContext(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
        }

        public void RegisterApplicationSignInManager(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
        }

        public Func<CookieValidateIdentityContext, Task> OnValidateIdentity()
        {
            return SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(validateInterval: TimeSpan.FromMinutes(30), regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager));
        }
    }
}