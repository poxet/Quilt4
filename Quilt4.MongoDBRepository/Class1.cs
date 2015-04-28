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

        public IApplicationUser FindById(string userId)
        {
            return _applicationUserManager.FindById(userId);
        }

        public async Task<string> GetPhoneNumberAsync(string userId)
        {
            return await _applicationUserManager.GetPhoneNumberAsync(userId);
        }

        public async Task<bool> GetTwoFactorEnabledAsync(string userId)
        {
            return await _applicationUserManager.GetTwoFactorEnabledAsync(userId);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            return await _applicationUserManager.GetLoginsAsync(userId);
        }

        public async Task<Tuple<IdentityResult, IApplicationUser>> CreateAsync(string userName, string email, string password)
        {
            var applicationUser = new ApplicationUser { UserName = userName, Email = email };
            var item = await _applicationUserManager.CreateAsync(applicationUser, password);
            return new Tuple<IdentityResult, IApplicationUser>(item, applicationUser);
        }

        public async Task<Tuple<IdentityResult, IApplicationUser>> CreateAsync(string userName, string email)
        {
            var applicationUser = new ApplicationUser { UserName = userName, Email = email };
            var item = await _applicationUserManager.CreateAsync(applicationUser);
            return new Tuple<IdentityResult, IApplicationUser>(item, applicationUser);
        }

        public async Task SignInAsync(IApplicationUser user, bool isPersistent, bool rememberBrowser)
        {
            await _applicationSignInManager.SignInAsync(user as ApplicationUser, isPersistent, rememberBrowser);
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            return await _applicationUserManager.ChangePasswordAsync(userId, oldPassword, newPassword);
        }

        public async Task<ClaimsIdentity> CreateIdentityAsync(IApplicationUser user, string applicationCookie)
        {
            return await _applicationUserManager.CreateIdentityAsync(user as ApplicationUser, applicationCookie);
        }

        public IList<UserLoginInfo> GetLogins(string userId)
        {
            return _applicationUserManager.GetLogins(userId);
        }

        public async Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo userLoginInfo)
        {
            return await _applicationUserManager.RemoveLoginAsync(userId, userLoginInfo);
        }

        public async Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string number)
        {
            return await _applicationUserManager.GenerateChangePhoneNumberTokenAsync(userId, number);
        }

        public IIdentityMessageService SmsService { get { return _applicationUserManager.SmsService; } }

        public async Task SetTwoFactorEnabledAsync(string userId, bool enabled)
        {
            await _applicationUserManager.SetTwoFactorEnabledAsync(userId, enabled);
        }

        public async Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string code)
        {
            return await _applicationUserManager.ChangePhoneNumberAsync(userId, phoneNumber, code);
        }

        public async Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber)
        {
            return await _applicationUserManager.SetPhoneNumberAsync(userId, phoneNumber);
        }

        public async Task<IdentityResult> AddPasswordAsync(string userId, string phoneNumber)
        {
            return await _applicationUserManager.AddPasswordAsync(userId, phoneNumber);
        }
        
        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            return await _applicationUserManager.AddLoginAsync(userId, login);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            return await _applicationUserManager.ConfirmEmailAsync(userId, token);
        }

        public async Task<IApplicationUser> FindByNameAsync(string email)
        {
            return await _applicationUserManager.FindByNameAsync(email);
        }

        public async Task<bool> IsEmailConfirmedAsync(string userId)
        {
            return await _applicationUserManager.IsEmailConfirmedAsync(userId);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            return await _applicationUserManager.ResetPasswordAsync(userId, token, newPassword);
        }

        public async Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId)
        {
            return await _applicationUserManager.GetValidTwoFactorProvidersAsync(userId);
        }

        public async Task<bool> SendTwoFactorCodeAsync(string provider)
        {
            return await _applicationSignInManager.SendTwoFactorCodeAsync(provider);
        }

        public async Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent)
        {
            return await _applicationSignInManager.ExternalSignInAsync(loginInfo, isPersistent);
        }
    }
}
