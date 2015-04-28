using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Quilt4.Interface;

namespace Quilt4.SQLRepository.Business
{
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
    }
}