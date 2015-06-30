namespace Quilt4.Interface
{
    public interface ICounterBusiness
    {
        //void Register(string counterName, int count, Dictionary<string, object> data);
        void RegisterInitiative(IInitiativeHead initiative);
        void RegisterApplication(IInitiativeHead initiative, IApplication application);
        void RegisterApplicationVersion(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion);
        void RegisterIssueType(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, IIssueType issueType);
        void RegisterIssue(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, IIssueType issueType, IIssue issue);
        void RegisterSession(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, ISession session);
        void RegisterMachine(IMachine machine);
        void RegisterUser(IUser user);
        void RegisterDeveloper(IDeveloper developer);
    }
}