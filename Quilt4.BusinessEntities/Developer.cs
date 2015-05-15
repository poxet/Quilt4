using System;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Developer : IDeveloper
    {
        public Developer(string userId, string userName, bool hasLocalAccount, string[] providerNames, DateTime creationDate, DateTime lastActivityDate, string lastIpAddress, string email, bool eMailConfirmed, string[] roles)
        {
            UserId = userId;
            UserName = userName;
            HasLocalAccount = hasLocalAccount;
            ProviderNames = providerNames;
            CreationDate = creationDate;
            LastActivityDate = lastActivityDate;
            LastIpAddress = lastIpAddress;
            Email = email;
            EMailConfirmed = eMailConfirmed;
            Roles = roles;
        }

        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public bool HasLocalAccount { get; private set; }
        public string[] ProviderNames { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime LastActivityDate { get; private set; }
        public string LastIpAddress { get; private set; }
        public string Email { get; private set; }
        public bool EMailConfirmed { get; private set; }
        public string[] Roles { get; private set; }
    }
}