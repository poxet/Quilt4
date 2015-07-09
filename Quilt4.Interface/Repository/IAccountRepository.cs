using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Quilt4.Interface
{
    public interface IAccountRepository
    {
        Task<SignInStatus> PasswordSignInAsync(string email, string password, bool rememberMe, bool shouldLockout);
        Task<bool> HasBeenVerifiedAsync();
        Task<IApplicationUser> FindByIdAsync(string userId);
        Task<string> GetVerifiedUserIdAsync();
        Task<string> GenerateTwoFactorTokenAsync(string userId, string twoFactorProvider);
        Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);
        IDeveloper FindById(string userId);
        Task<string> GetPhoneNumberAsync(string userId);
        Task<bool> GetTwoFactorEnabledAsync(string userId);
        Task<IList<UserLoginInfo>> GetLoginsAsync(string userId);
        Task<Tuple<IdentityResult, IApplicationUser>> CreateAsync(string userName, string email, string password);
        Task<Tuple<IdentityResult, IApplicationUser>> CreateAsync(string userName, string email);
        Task SignInAsync(IApplicationUser user, bool isPersistent, bool rememberBrowser);
        Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<ClaimsIdentity> CreateIdentityAsync(IApplicationUser user, string applicationCookie);
        IList<UserLoginInfo> GetLogins(string userId);
        Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo userLoginInfo);
        Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string number);
        Task<string> GenerateEmailConfirmationTokenAsync(string userId);
        Task<string> GeneratePasswordResetTokenAsync(string userId);
        IIdentityMessageService SmsService { get; }
        Task SetTwoFactorEnabledAsync(string userId, bool enabled);
        Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string code);
        Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber);
        Task<IdentityResult> AddPasswordAsync(string userId, string newPassword);
        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<IApplicationUser> FindByNameAsync(string email);
        Task<bool> IsEmailConfirmedAsync(string userId);
        Task<IdentityResult> ResetPasswordAsync(string id, string code, string password);
        Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId);
        Task<bool> SendTwoFactorCodeAsync(string provider);
        Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent);
        IEnumerable<IDeveloper> GetUsers();
        IDeveloper GetUser(string userEmail);
        void DeleteUser(string userId);
        void AssignRole(string userId, string roleName);
    }
}