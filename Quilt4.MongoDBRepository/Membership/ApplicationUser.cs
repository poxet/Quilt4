using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;

namespace Quilt4.MongoDBRepository.Membership
{
    //TODO: This is duplicate from Quilt4.SQLRepository
    internal class ApplicationUser : IdentityUser, IApplicationUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}