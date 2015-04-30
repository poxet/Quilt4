using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Quilt4.Interface;
using Quilt4.MongoDBRepository.Membership;

namespace Quilt4.MongoDBRepository
{
    public class RepositoryHandler : IRepositoryHandler
    {
        private void RegisterApplicationUserManager(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationUserManager.Create);
        }

        private void RegisterApplicationDbContext(IAppBuilder app)
        {
            //new IdentityDbContext<ApplicationUser>
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
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
            return SecurityStampValidator.OnValidateIdentity<UserManager<ApplicationUser>, ApplicationUser>(validateInterval: TimeSpan.FromMinutes(30), regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager));
        }
    }
}