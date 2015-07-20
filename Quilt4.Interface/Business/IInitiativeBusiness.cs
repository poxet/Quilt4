using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IInitiativeBusiness
    {
        void Create(string developerEmail, string initiativename);
        IEnumerable<IInitiativeHead> GetInitiativesByDeveloperOwner(string developerEmail);
        IEnumerable<IInitiativeHead> GetInitiativesByDeveloper(string developerEmail);
        IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId);
        IApplication RegisterApplication(IClientToken clientToken, string applicationName, string applicationVersionFingerprint);
        IEnumerable<IInitiative> GetInitiatives();
        IEnumerable<IIssue> GetIssueStatistics(DateTime fromDate, DateTime toDate);
        IInitiative GetInitiative(Guid id);
        IInitiative GetInitiative(string developerEmail, string initiativeIdentifier);
        int GetInitiativeCount();
        int GetApplicationCount();
        int GetIssueTypeCount();
        int GetIssueCount();
        void UpdateInitiative(Guid id, string name, string sessionToken, string owner);
        void UpdateInitiative(IInitiative initiative);
        void DeleteInitiative(Guid id);
        void DeleteApplicationVersion(string applicationVersionFingerprint);
        void ArchiveApplicationVersion(string versionId);
        IDictionary<string, string> GetEnvironmentColors(string userId, string userName);
        void UpdateEnvironmentColors(string userId, IDictionary<string, string> environmentColors);
        //void AddEnvironmentColors(string userId, IDictionary<string, string> environmentColors);
        IInitiative GetInitiativeByInviteCode(string inviteCode);
        void ConfirmInvitation(Guid initiativeId, string developerEmail);
        void DeclineInvitation(string inviteCode);
        IEnumerable<IInvitation> GetInvitations(string userId);
        string GenerateInviteMessage(string initiativeid, string code, string message, Uri url);
        IApplication GetApplicationByVersion(string versionId);
        IInitiative GetInitiativeByApplication(Guid applicationId);
    }
}