using System;

namespace Quilt4.Interface
{
    public interface IIssueBusiness
    {
        ILogResponse RegisterIssue(Exception exception, IssueLevel warning);
        ISession GetSession(Guid id);
        void UpdateApplicationVersion(IApplicationVersion applicationVersion);
        IApplicationVersion GetApplicationVersion(string applicationVersionFingerprint);
        IInitiative GetInitiativeByApplication(Guid applicationId);
    }
}