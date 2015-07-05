using System;

namespace Quilt4.Interface
{
    public interface IInitiativeHead
    {
        //TODO: REFACTOR: Make immutable
        Guid Id { get; }
        string Name { get; set; }
        string ClientToken { get; }
        string OwnerDeveloperName { get; }
    }
}