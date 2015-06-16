using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IInitiative : IInitiativeHead
    {
        IEnumerable<IDeveloperRole> DeveloperRoles { get; }
        IEnumerable<IApplicationGroup> ApplicationGroups { get; }

        void AddApplicationGroup(IApplicationGroup applicationGroup);
        void RemoveApplicationGroup(IApplicationGroup applicationGroup);
        string AddDeveloperRolesInvitation(string email);
        void RemoveDeveloperRole(string developer);
        void DeclineInvitation(string inviteCode);
        void ConfirmInvitation(string inviteCode, string developerName);
    }
}