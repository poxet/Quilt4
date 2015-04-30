using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Quilt4.Interface
{
    public interface IRepositoryFactory
    {
        void RegisterApplicationUserManager(IAppBuilder app);
        void RegisterApplicationDbContext(IAppBuilder app);
        void RegisterApplicationSignInManager(IAppBuilder app);
        Func<CookieValidateIdentityContext, Task> OnValidateIdentity();
    }
}