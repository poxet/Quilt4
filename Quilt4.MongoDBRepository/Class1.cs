using System;
using System.Collections.Generic;
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

    public class ApplicationUserManager : UserManager<ApplicationUser> //, IUserLockoutStore<ApplicationUser, string> 
    {
        public static event EventHandler<ApplicationUserManagerCreatedEventArgs> ApplicationUserManagerCreatedEvent;

        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create()
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>("Mongo"));
            InvokeApplicationUserManagerCreatedEvent(manager);
            return manager;
        }

        private static void InvokeApplicationUserManagerCreatedEvent(ApplicationUserManager manager)
        {
            var handler = ApplicationUserManagerCreatedEvent;
            if (handler != null) handler(null, new ApplicationUserManagerCreatedEventArgs(manager));
        }

        //public Task CreateAsync(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UpdateAsync(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task DeleteAsync(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task ResetAccessFailedCountAsync(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class MongoDbRepositoryFactory : IRepositoryFactory
    {
        public void RegisterApplicationUserManager(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationUserManager.Create);
        }

        public void RegisterApplicationDbContext(IAppBuilder app)
        {
            //new IdentityDbContext<ApplicationUser>
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
        }

        public void RegisterApplicationSignInManager(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
        }

        public Func<CookieValidateIdentityContext, Task> OnValidateIdentity()
        {
            return SecurityStampValidator.OnValidateIdentity<UserManager<ApplicationUser>, ApplicationUser>(validateInterval: TimeSpan.FromMinutes(30), regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager));
        }
    }

    //TODO: This is duplicate from Quilt4.SQLRepository
    public class ApplicationUserManagerCreatedEventArgs : EventArgs
    {
        private readonly ApplicationUserManager _manager;

        public ApplicationUserManagerCreatedEventArgs(ApplicationUserManager manager)
        {
            _manager = manager;
        }

        public ApplicationUserManager ApplicationUserManager { get { return _manager; } }
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

    public class IdentityUserLogin<TKey>
    {
    }

    public class IdentityUserRole<TKey>
    {
    }

    //TODO: This is duplicate from Quilt4.SQLRepository
    //public class ApplicationSignInManager<TUser, TKey, TUserLogin, TUserRole, TUserClaim>
    //    //, TRole, TKey, TUserLogin, TUserRole, TUserClaim>
    //    :
    //        IUserLoginStore<TUser, TKey>,
    //    //IUserClaimStore<TUser, TKey>, 
    //    //IUserRoleStore<TUser, TKey>, 
    //        IUserPasswordStore<TUser, TKey>,
    //    //IUserSecurityStampStore<TUser, TKey>, 
    //    //IQueryableUserStore<TUser, TKey>, 
    //        IUserEmailStore<TUser, TKey>,
    //        IUserPhoneNumberStore<TUser, TKey>,
    //        IUserTwoFactorStore<TUser, TKey>,
    //        IUserLockoutStore<TUser, TKey>,
    //    //IUserStore<TUser>, 
    //        IDisposable
    //    where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>, IUser<string>, IUser<TKey>
    //    //where TRole : Microsoft.AspNet.Identity.EntityFramework.IdentityRole<TKey, TUserRole>
    //    where TKey : class, IEquatable<TKey>
    //    where TUserLogin : IdentityUserLogin<TKey>, new()
    //    where TUserRole : IdentityUserRole<TKey>, new()
    //    where TUserClaim : IdentityUserClaim<TKey>, new()
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

    //TODO: This class is very similar to the one in SQLRepo. If we are using interfaces it could be the same
    public class AccountBusiness : IAccountBusiness
    {
        private ApplicationSignInManager _applicationSignInManager;
        private ApplicationUserManager _applicationUserManager;

        public AccountBusiness()
        {
            ApplicationSignInManager.ApplicationSignInManagerCreatedEvent += ApplicationSignInManagerCreatedEvent;
            ApplicationUserManager.ApplicationUserManagerCreatedEvent += ApplicationUserManagerCreatedEvent;
        }

        private void ApplicationUserManagerCreatedEvent(object sender, ApplicationUserManagerCreatedEventArgs e)
        {
            _applicationUserManager = e.ApplicationUserManager;
        }

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
            return await _applicationSignInManager.HasBeenVerifiedAsync();
        }

        public async Task<IApplicationUser> FindByIdAsync(string userId)
        {
            return await _applicationUserManager.FindByIdAsync(userId);
        }

        public async Task<string> GetVerifiedUserIdAsync()
        {
            return await _applicationSignInManager.GetVerifiedUserIdAsync();
        }

        public async Task<string> GenerateTwoFactorTokenAsync(string userId, string twoFactorProvider)
        {
            return await _applicationUserManager.GenerateTwoFactorTokenAsync(userId, twoFactorProvider);
        }

        public async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            return await _applicationSignInManager.TwoFactorSignInAsync(provider, code, isPersistent, rememberBrowser);
        }

        public IApplicationUser FindById(string getUserId)
        {
            return _applicationUserManager.FindById(getUserId);
        }

        public async Task<string> GetPhoneNumberAsync(string getUserId)
        {
            return await _applicationUserManager.GetPhoneNumberAsync(getUserId);
        }

        public async Task<bool> GetTwoFactorEnabledAsync(string getUserId)
        {
            return await _applicationUserManager.GetTwoFactorEnabledAsync(getUserId);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(string getUserId)
        {
            return await _applicationUserManager.GetLoginsAsync(getUserId);
        }

        public async Task<Tuple<IdentityResult, IApplicationUser>> CreateAsync(string userName, string email, string password)
        {
            var applicationUser = new ApplicationUser { UserName = userName, Email = email };
            var item = await _applicationUserManager.CreateAsync(applicationUser, password);
            return new Tuple<IdentityResult, IApplicationUser>(item, applicationUser);
        }

        public async Task SignInAsync(IApplicationUser user, bool isPersistent, bool rememberBrowser)
        {
            await _applicationSignInManager.SignInAsync(user as ApplicationUser, isPersistent, rememberBrowser);
        }
    }
}
