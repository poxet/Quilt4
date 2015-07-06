using System;

namespace Quilt4.Interface
{
    public interface ICounterBusiness
    {
        //void RegisterInitiative(IInitiativeHead initiative);
        //void RegisterApplication(IInitiativeHead initiative, IApplication application);
        void UpdateApplicationVersionCounters();
        //void RegisterIssueType(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, IIssueType issueType);
        //void RegisterIssue(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, IIssueType issueType, IIssue issue);
        void UpdateSessionCounters();
        //void RegisterMachine(IMachine machine);
        //void RegisterUser(IUser user);
        //void RegisterDeveloper(IDeveloper developer);
        //void UnregisterSession(Guid sessionId);
    }
}