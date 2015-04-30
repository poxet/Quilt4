using System;

namespace Quilt4.MongoDBRepository.Membership
{
    //TODO: This is duplicate from Quilt4.SQLRepository
    internal class ApplicationSignInManagerCreatedEventArgs : EventArgs
    {
        private readonly ApplicationSignInManager _applicationSignInManager;

        public ApplicationSignInManagerCreatedEventArgs(ApplicationSignInManager applicationSignInManager)
        {
            _applicationSignInManager = applicationSignInManager;
        }

        public ApplicationSignInManager ApplicationSignInManager { get { return _applicationSignInManager; } }
    }
}