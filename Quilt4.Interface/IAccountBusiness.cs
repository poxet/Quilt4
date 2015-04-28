using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Quilt4.Interface
{
    public interface IAccountBusiness
    {
        Task<SignInStatus> PasswordSignInAsync(string email, string password, bool rememberMe, bool shouldLockout);
        Task<bool> HasBeenVerifiedAsync();
        Task<IApplicationUser> FindByIdAsync(string userId);
        Task<string> GetVerifiedUserIdAsync();
        Task<string> GenerateTwoFactorTokenAsync(string userId, string twoFactorProvider);
        Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);
        IApplicationUser FindById(string getUserId);
        Task<string> GetPhoneNumberAsync(string getUserId);
        Task<bool> GetTwoFactorEnabledAsync(string getUserId);
        Task<IList<UserLoginInfo>> GetLoginsAsync(string getUserId);
        Task<Tuple<IdentityResult,IApplicationUser>> CreateAsync(string userName, string email, string password);
        Task SignInAsync(IApplicationUser user, bool isPersistent, bool rememberBrowser);
    }
}