using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IInitiativeBusiness
    {
        void Create(string developerName, string initiativename);
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
    }
}