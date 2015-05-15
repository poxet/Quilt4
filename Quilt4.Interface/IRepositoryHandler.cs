using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Quilt4.Interface
{
    public interface IRepositoryHandler
    {
        void Register(IAppBuilder app);
        Func<CookieValidateIdentityContext, Task> OnValidateIdentity();
        object GetApplicationSignInManager();
        object GetApplicationUserManager();
    }
}