using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Quilt4.Interface;

namespace Quilt4.MultiRepository
{
    public class RepositoryHandler : IRepositoryHandler
    {
        public void Register(IAppBuilder app)
        {
            throw new NotImplementedException();
        }

        public Func<CookieValidateIdentityContext, Task> OnValidateIdentity()
        {
            throw new NotImplementedException();
        }

        public object GetApplicationSignInManager()
        {
            throw new NotImplementedException();
        }

        public object GetApplicationUserManager()
        {
            throw new NotImplementedException();
        }
    }
}