using System;

namespace Quilt4.MongoDBRepository.Membership
{
    //TODO: This is duplicate from Quilt4.SQLRepository
    internal class ApplicationUserManagerCreatedEventArgs : EventArgs
    {
        private readonly ApplicationUserManager _manager;

        public ApplicationUserManagerCreatedEventArgs(ApplicationUserManager manager)
        {
            _manager = manager;
        }

        public ApplicationUserManager ApplicationUserManager { get { return _manager; } }
    }
}