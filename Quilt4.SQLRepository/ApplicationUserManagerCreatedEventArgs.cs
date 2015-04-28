﻿using System;

namespace Quilt4.SQLRepository
{
    public class ApplicationUserManagerCreatedEventArgs : EventArgs
    {
        private readonly ApplicationUserManager _manager;

        public ApplicationUserManagerCreatedEventArgs(ApplicationUserManager manager)
        {
            _manager = manager;
        }

        public ApplicationUserManager ApplicationUserManager { get { return _manager; } }
    }
}