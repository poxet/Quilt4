using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

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
                var r = Task<IEnumerable<ApplicationUser>>.Factory.StartNew(() => _store.GetAllUsersAsync().Result);
                var response = r.Result.AsQueryable();
                return response;
            }            
        }
    }
}