using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Quilt4.Interface;

namespace Quilt4.MultiRepository
{
    public class AccountRepository : IAccountRepository
    {
        public async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasBeenVerifiedAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IApplicationUser> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetVerifiedUserIdAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateTwoFactorTokenAsync(string userId, string twoFactorProvider)
        {
            throw new NotImplementedException();
        }

        public async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            throw new NotImplementedException();
        }

        public IDeveloper FindById(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPhoneNumberAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<IdentityResult, IApplicationUser>> CreateAsync(string userName, string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<IdentityResult, IApplicationUser>> CreateAsync(string userName, string email)
        {
            throw new NotImplementedException();
        }

        public Task SignInAsync(IApplicationUser user, bool isPersistent, bool rememberBrowser)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<ClaimsIdentity> CreateIdentityAsync(IApplicationUser user, string applicationCookie)
        {
            throw new NotImplementedException();
        }

        public IList<UserLoginInfo> GetLogins(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo userLoginInfo)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string number)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GeneratePasswordResetTokenAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public IIdentityMessageService SmsService { get; }
        public Task SetTwoFactorEnabledAsync(string userId, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string code)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> AddPasswordAsync(string userId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<IApplicationUser> FindByNameAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEmailConfirmedAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ResetPasswordAsync(string id, string code, string password)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendTwoFactorCodeAsync(string provider)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDeveloper> GetUsers()
        {
            throw new NotImplementedException();
        }

        public IDeveloper GetUser(string userEmail)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(string userId)
        {
            throw new NotImplementedException();
        }

        public void AssignRole(string userId, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IApplicationUser> FindAsync(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateUsernameAsync(string userId, string newUsername)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateSecurityStampAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateEmailAsync(string id, string newEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent)
        {
            throw new NotImplementedException();
        }
    }
}