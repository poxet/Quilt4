using System.Threading.Tasks;
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
    }
}