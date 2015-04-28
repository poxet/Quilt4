using System;

namespace Quilt4.SQLRepository
{
    public class ApplicationSignInManagerCreatedEventArgs : EventArgs
    {
        private readonly ApplicationSignInManager _applicationSignInManager;

        public ApplicationSignInManagerCreatedEventArgs(ApplicationSignInManager applicationSignInManager)
        {
            _applicationSignInManager = applicationSignInManager;
        }

        public ApplicationSignInManager ApplicationSignInManager { get { return _applicationSignInManager; } }
    }
}