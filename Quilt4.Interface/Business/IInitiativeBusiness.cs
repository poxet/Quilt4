using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IInitiativeBusiness
    {
        void Create(string developerName, string initiativename);
        IEnumerable<IInitiativeHead> GetInitiativesByDeveloperOwner(string developerName);
        IEnumerable<IInitiativeHead> GetInitiativesByDeveloper(string developerName);
        IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId);
        IApplication RegisterApplication(IClientToken clientToken, string applicationName, string applicationVersionFingerprint);
        IEnumerable<IInitiative> GetInitiatives();
        IEnumerable<IIssue> GetIssueStatistics(DateTime fromDate, DateTime toDate);
        IInitiative GetInitiative(Guid id);
        IInitiative GetInitiative(string developerName, string initiativeIdentifier);
        int GetInitiativeCount();
        int GetApplicationCount();
        int GetIssueTypeCount();
        int GetIssueCount();
        void UpdateInitiative(Guid id, string name, string sessionToken, string owner);
        void UpdateInitiative(IInitiative initiative);
        void DeleteInitiative(Guid id);
        void DeleteApplicationVersion(string applicationVersionFingerprint);
        void ArchiveApplicationVersion(string versionId);
        IInitiative GetInitiativeByInviteCode(string inviteCode);
        void ConfirmInvitation(Guid initiativeId, string developerName);
        void DeclineInvitation(string inviteCode);
        IEnumerable<IInvitation> GetInvitations(string email);
    }
}