using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IInitiative : IInitiativeHead
    {
        IEnumerable<IDeveloperRole> DeveloperRoles { get; }
        IEnumerable<IApplicationGroup> ApplicationGroups { get; }

        //TODO: REFACTOR: Move thees functions to the business layer
        void AddApplicationGroup(IApplicationGroup applicationGroup);
        void RemoveApplicationGroup(IApplicationGroup applicationGroup);
        string AddDeveloperRolesInvitation(string email);
        void RemoveDeveloperRole(string developer);
    }
}