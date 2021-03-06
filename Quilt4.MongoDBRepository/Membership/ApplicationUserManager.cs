using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Quilt4.MongoDBRepository.Membership
{
    internal class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private readonly UserStore<ApplicationUser> _store;
        public static event EventHandler<ApplicationUserManagerCreatedEventArgs> ApplicationUserManagerCreatedEvent;

        public ApplicationUserManager(UserStore<ApplicationUser> store)
            : base(store)
        {
            _store = store;

            var provider = new DpapiDataProtectionProvider("Sample");
            this.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
        }

        public static ApplicationUserManager Create()
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>("Mongo"));

            InvokeApplicationUserManagerCreatedEvent(manager);
            return manager;
        }

        private static void InvokeApplicationUserManagerCreatedEvent(ApplicationUserManager manager)
        {
            var handler = ApplicationUserManagerCreatedEvent;
            if (handler != null) handler(null, new ApplicationUserManagerCreatedEventArgs(manager));
        }

        public override IQueryable<ApplicationUser> Users
        {
            get
            {
                var task = Task.Run(async () => await _store.GetAllUsersAsync());
                return task.Result.AsQueryable();
            }            
        }        

        public override async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            if (token != null)
                return await base.ConfirmEmailAsync(userId, token);

            var user = await _store.FindByIdAsync(userId);
            await _store.SetEmailConfirmedAsync(user, true);

            return new IdentityResult();
        }
    }
}