using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Quilt4.Interface
{
    public interface IAccountBusiness
    {
        Task<SignInStatus> PasswordSignInAsync(string email, string password, bool rememberMe, bool shouldLockout);
    }

    //public interface IOwinContextAgent
    //{
    //    object GetOwinContext();
    //}

    public interface IRepositoryFactory
    {
        void RegisterApplicationUserManager(IAppBuilder app);
        void RegisterApplicationDbContext(IAppBuilder app);
        void RegisterApplicationSignInManager(IAppBuilder app);
        Func<CookieValidateIdentityContext, Task> OnValidateIdentity();
    }
}
