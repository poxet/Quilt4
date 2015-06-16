using System;

namespace Quilt4.Interface
{
    public interface IInitiativeHead
    {
        Guid Id { get; }
        string Name { get; set; }
        string ClientToken { get; }
        string OwnerDeveloperName { get; set; }
    }
}