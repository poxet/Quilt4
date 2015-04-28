using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Quilt4.Interface;

namespace Quilt4.SQLRepository.Business
{
    public class AccountBusiness : IAccountBusiness
    {
        private ApplicationSignInManager _applicationSignInManager;

        public AccountBusiness()
        {
            ApplicationSignInManager.ApplicationSignInManagerCreatedEvent += ApplicationSignInManagerCreatedEvent;
        }

        private void ApplicationSignInManagerCreatedEvent(object sender, ApplicationSignInManagerCreatedEventArgs e)
        {
            _applicationSignInManager = e.ApplicationSignInManager;
        }

        public async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            return await _applicationSignInManager.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }
    }
}