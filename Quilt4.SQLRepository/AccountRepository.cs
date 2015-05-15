using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.SQLRepository.Membership;

namespace Quilt4.SQLRepository
{
    public class AccountRepository : IAccountRepository
    {
        private ApplicationSignInManager _applicationSignInManager;
        private ApplicationUserManager _applicationUserManager;

        public AccountRepository()
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

        public IEnumerable<IDeveloper> GetUsers()
        {
            var hasLocalAccount = true; //TODO: Fix
            var creationDate = new DateTime(); //TODO: Fix
            var lastActivityDate = new DateTime(); //TODO: Fix
            return _applicationUserManager.Users.Select(x => new Developer(x.Id, x.UserName, hasLocalAccount, x.Logins.Select(y => y.LoginProvider).ToArray(), creationDate, lastActivityDate, null, x.Email, x.EmailConfirmed, x.Roles.Select(y => y.RoleId).ToArray()));
        }
    }
}