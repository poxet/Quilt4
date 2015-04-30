using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Quilt4.SQLRepository.Membership
{
    internal class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public static event EventHandler<ApplicationSignInManagerCreatedEventArgs> ApplicationSignInManagerCreatedEvent;

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            var applicationSignInManager = new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
            InvokeApplicationSignInManagerCreatedEvent(applicationSignInManager);
            return applicationSignInManager;
        }

        private static void InvokeApplicationSignInManagerCreatedEvent(ApplicationSignInManager applicationSignInManager)
        {
            var handler = ApplicationSignInManagerCreatedEvent;
            if (handler != null) handler(null, new ApplicationSignInManagerCreatedEventArgs(applicationSignInManager));
        }
    }
}