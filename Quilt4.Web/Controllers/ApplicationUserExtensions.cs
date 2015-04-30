using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;

namespace Quilt4.Web.Controllers
{
    public static class ApplicationUserExtensions
    {
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this IApplicationUser user, IAccountRepository manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}