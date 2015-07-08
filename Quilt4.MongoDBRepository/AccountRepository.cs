using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.MongoDBRepository.Membership;

namespace Quilt4.MongoDBRepository
{
    //TODO: This class is very similar to the one in SQLRepo. If we are using interfaces it could be the same
    public class AccountRepository : IAccountRepository
    {
        private readonly IRepositoryHandler _repositoryHandler;

        public AccountRepository(IRepositoryHandler repositoryHandler)
        {
            _repositoryHandler = repositoryHandler;
        }

        private ApplicationSignInManager ApplicationSignInManager { get { return _repositoryHandler.GetApplicationSignInManager() as ApplicationSignInManager; } }
        private ApplicationUserManager ApplicationUserManager { get { return _repositoryHandler.GetApplicationUserManager() as ApplicationUserManager; } }

        public async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            return await ApplicationSignInManager.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }

        public async Task<bool> HasBeenVerifiedAsync()
        {
            return await ApplicationSignInManager.HasBeenVerifiedAsync();
        }

        public async Task<IApplicationUser> FindByIdAsync(string userId)
        {
            return await ApplicationUserManager.FindByIdAsync(userId);
        }

        public async Task<string> GetVerifiedUserIdAsync()
        {
            return await ApplicationSignInManager.GetVerifiedUserIdAsync();
        }

        public async Task<string> GenerateTwoFactorTokenAsync(string userId, string twoFactorProvider)
        {
            return await ApplicationUserManager.GenerateTwoFactorTokenAsync(userId, twoFactorProvider);
        }

        public async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            return await ApplicationSignInManager.TwoFactorSignInAsync(provider, code, isPersistent, rememberBrowser);
        }

        public IDeveloper FindById(string userId)
        {
            return ApplicationUserManager.FindById(userId).ToDeveloper();
        }

        public async Task<string> GetPhoneNumberAsync(string userId)
        {
            return await ApplicationUserManager.GetPhoneNumberAsync(userId);
        }

        public async Task<bool> GetTwoFactorEnabledAsync(string userId)
        {
            return await ApplicationUserManager.GetTwoFactorEnabledAsync(userId);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            return await ApplicationUserManager.GetLoginsAsync(userId);
        }

        public async Task<Tuple<IdentityResult, IApplicationUser>> CreateAsync(string userName, string email, string password)
        {
            var applicationUser = new ApplicationUser { UserName = userName, Email = email };
            var item = await ApplicationUserManager.CreateAsync(applicationUser, password);
            return new Tuple<IdentityResult, IApplicationUser>(item, applicationUser);
        }

        public async Task<Tuple<IdentityResult, IApplicationUser>> CreateAsync(string userName, string email)
        {
            var applicationUser = new ApplicationUser { UserName = userName, Email = email };
            var item = await ApplicationUserManager.CreateAsync(applicationUser);
            return new Tuple<IdentityResult, IApplicationUser>(item, applicationUser);
        }

        public async Task SignInAsync(IApplicationUser user, bool isPersistent, bool rememberBrowser)
        {
            await ApplicationSignInManager.SignInAsync(user as ApplicationUser, isPersistent, rememberBrowser);
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            return await ApplicationUserManager.ChangePasswordAsync(userId, oldPassword, newPassword);
        }

        public async Task<ClaimsIdentity> CreateIdentityAsync(IApplicationUser user, string applicationCookie)
        {
            return await ApplicationUserManager.CreateIdentityAsync(user as ApplicationUser, applicationCookie);
        }

        public IList<UserLoginInfo> GetLogins(string userId)
        {
            return ApplicationUserManager.GetLogins(userId);
        }

        public async Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo userLoginInfo)
        {
            return await ApplicationUserManager.RemoveLoginAsync(userId, userLoginInfo);
        }

        public async Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string number)
        {
            return await ApplicationUserManager.GenerateChangePhoneNumberTokenAsync(userId, number);
        }

        public IIdentityMessageService SmsService { get { return ApplicationUserManager.SmsService; } }

        public async Task SetTwoFactorEnabledAsync(string userId, bool enabled)
        {
            await ApplicationUserManager.SetTwoFactorEnabledAsync(userId, enabled);
        }

        public async Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string code)
        {
            return await ApplicationUserManager.ChangePhoneNumberAsync(userId, phoneNumber, code);
        }

        public async Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber)
        {
            return await ApplicationUserManager.SetPhoneNumberAsync(userId, phoneNumber);
        }

        public async Task<IdentityResult> AddPasswordAsync(string userId, string phoneNumber)
        {
            return await ApplicationUserManager.AddPasswordAsync(userId, phoneNumber);
        }
        
        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            return await ApplicationUserManager.AddLoginAsync(userId, login);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            return await ApplicationUserManager.ConfirmEmailAsync(userId, token);
        }

        public async Task<IApplicationUser> FindByNameAsync(string email)
        {
            return await ApplicationUserManager.FindByNameAsync(email);
        }

        public async Task<bool> IsEmailConfirmedAsync(string userId)
        {
            return await ApplicationUserManager.IsEmailConfirmedAsync(userId);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            return await ApplicationUserManager.ResetPasswordAsync(userId, token, newPassword);
        }

        public async Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId)
        {
            return await ApplicationUserManager.GetValidTwoFactorProvidersAsync(userId);
        }

        public async Task<bool> SendTwoFactorCodeAsync(string provider)
        {
            return await ApplicationSignInManager.SendTwoFactorCodeAsync(provider);
        }

        public async Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent)
        {
            return await ApplicationSignInManager.ExternalSignInAsync(loginInfo, isPersistent);
        }

        public IEnumerable<IDeveloper> GetUsers()
        {
            return ApplicationUserManager.Users.Select(x => x.ToDeveloper());
        }

        public IDeveloper GetUser(string userEmail)
        {
            return ApplicationUserManager.Users.Single(x => x.Email == userEmail).ToDeveloper();
        }

        public void DeleteUser(string userId)
        {
            var user = ApplicationUserManager.FindById(userId);
            ApplicationUserManager.Delete(user);
        }

        public void AssignRole(string userId, string roleName)
        {
            ApplicationUserManager.AddToRole(userId, roleName);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(string userId)
        {
            return await ApplicationUserManager.GenerateEmailConfirmationTokenAsync(userId);
        }
    }

    internal static class Converter2
    {
        public static IDeveloper ToDeveloper(this ApplicationUser x)
        {
            var hasLocalAccount = true; //TODO: Fix
            var creationDate = new DateTime(); //TODO: Fix
            var lastActivityDate = new DateTime(); //TODO: Fix
            var developer = new Developer(x.Id, x.UserName, hasLocalAccount, x.Logins != null ? x.Logins.Select(y => y.LoginProvider).ToArray() : new string[] { }, creationDate, lastActivityDate, null, x.Email, x.EmailConfirmed, x.Roles != null ? x.Roles.ToArray() : new string[] { });
            return developer;
        }
    }
}