using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Quilt4.Interface;

namespace Quilt4.MongoDBRepository
{

    //TODO: This is duplicate from Quilt4.SQLRepository
    public class ApplicationUser : IdentityUser, IApplicationUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create()
        {
            //return new UserManager<ApplicationUser>(new UserStore<ApplicationUser>("Mongo"));
            return new ApplicationUserManager(new UserStore<ApplicationUser>("Mongo"));
        }
    }

    public class MongoDbRepositoryFactory : IRepositoryFactory
    {
        public void RegisterApplicationUserManager(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationUserManager.Create);

            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            //throw new NotImplementedException();
        }

        public void RegisterApplicationDbContext(IAppBuilder app)
        {
            //new IdentityDbContext<ApplicationUser>

            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //throw new NotImplementedException();
        }

        public void RegisterApplicationSignInManager(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            //throw new NotImplementedException();
        }

        public Func<CookieValidateIdentityContext, Task> OnValidateIdentity()
        {
            return SecurityStampValidator.OnValidateIdentity<UserManager<ApplicationUser>, ApplicationUser>(validateInterval: TimeSpan.FromMinutes(30), regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager));
            //throw new NotImplementedException();
        }
    }

    //TODO: This is duplicate from Quilt4.SQLRepository
    public class ApplicationSignInManagerCreatedEventArgs : EventArgs
    {
        private readonly ApplicationSignInManager _applicationSignInManager;

        public ApplicationSignInManagerCreatedEventArgs(ApplicationSignInManager applicationSignInManager)
        {
            _applicationSignInManager = applicationSignInManager;
        }

        public ApplicationSignInManager ApplicationSignInManager { get { return _applicationSignInManager; } }
    }

    //TODO: This is duplicate from Quilt4.SQLRepository
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string> //, IUserLockoutStore<ApplicationUser, string>
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

    public class AccountBusiness : IAccountBusiness
    {
        private ApplicationSignInManager _applicationSignInManager;
        //private ApplicationUserManager _applicationUserManager;

        public AccountBusiness()
        {
            ApplicationSignInManager.ApplicationSignInManagerCreatedEvent += ApplicationSignInManagerCreatedEvent;
            //ApplicationUserManager.ApplicationUserManagerCreatedEvent += ApplicationUserManagerCreatedEvent;
        }

        //private void ApplicationUserManagerCreatedEvent(object sender, ApplicationUserManagerCreatedEventArgs e)
        //{
        //    _applicationUserManager = e.ApplicationUserManager;
        //}

        private void ApplicationSignInManagerCreatedEvent(object sender, ApplicationSignInManagerCreatedEventArgs e)
        {
            _applicationSignInManager = e.ApplicationSignInManager;
        }

        public async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            return await _applicationSignInManager.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }

        public async Task<bool> HasBeenVerifiedAsync()
        {
            //return await _applicationSignInManager.HasBeenVerifiedAsync();
            throw new NotImplementedException();
        }

        public async Task<IApplicationUser> FindByIdAsync(string userId)
        {
            //return await _applicationUserManager.FindByIdAsync(userId);
            throw new NotImplementedException();
        }

        public async Task<string> GetVerifiedUserIdAsync()
        {
            //return await _applicationSignInManager.GetVerifiedUserIdAsync();
            throw new NotImplementedException();
        }

        public async Task<string> GenerateTwoFactorTokenAsync(string userId, string twoFactorProvider)
        {
            //return await _applicationUserManager.GenerateTwoFactorTokenAsync(userId, twoFactorProvider);
            throw new NotImplementedException();
        }

        public async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            //return await _applicationSignInManager.TwoFactorSignInAsync(provider, code, isPersistent, rememberBrowser);
            throw new NotImplementedException();
        }
    }
}
