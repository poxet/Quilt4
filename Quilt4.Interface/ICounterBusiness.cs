namespace Quilt4.Interface
{
    public interface ICounterBusiness
    {
        void RegisterInitiative(IInitiativeHead initiative);
        void RegisterApplication(IInitiativeHead initiative, IApplication application);
        void RegisterApplicationVersion(IApplicationVersion applicationVersion);
        void RegisterIssueType(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, IIssueType issueType);
        void RegisterIssue(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, IIssueType issueType, IIssue issue);
        void RegisterSession(ISession session, int count);
        void RegisterMachine(IMachine machine);
        void RegisterUser(IUser user);
        void RegisterDeveloper(IDeveloper developer);
    }
}