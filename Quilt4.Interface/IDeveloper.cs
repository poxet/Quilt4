using System;

namespace Quilt4.Interface
{
    public interface IDeveloper
    {
        string UserId { get; }
        string UserName { get; }
        bool HasLocalAccount { get; }
        string[] ProviderNames { get; }
        DateTime CreationDate { get; }
        DateTime LastActivityDate { get; }
        string LastIpAddress { get; }
        string Email { get; }
        bool EMailConfirmed { get; }
        string[] Roles { get; }
    }
}