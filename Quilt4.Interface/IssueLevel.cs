using System;
using System.Collections.Generic;

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

    public interface ISettingsBusiness
    {
        T GetSetting<T>(string name);
    }

    public interface ICompatibilityBusiness
    {
        void RegisterToolkitCompability(Version version, DateTime utcNow, string supportToolkitNameVersion, object o);
    }

    public interface ISessionBusiness
    {
        void RegisterSession(ISession session);
        void EndSession(Guid sessionId);
    }

    public interface IInitiativeBusiness
    {
        IApplication RegisterApplication(IClientToken clientToken, string applicationName, string applicationVersionFingerprint);
    }

    public interface IClientToken
    {
        
    }

    public interface IMachineBusiness
    {
        void RegisterMachine(IFingerprint id, string name, IDictionary<string, string> data);
        IMachine GetMachine(string machineFingerprint);
    }

    public interface IUserBusiness
    {
        void RegisterUser(IFingerprint id, string userName);
        IUser GetUser(string userFingerprint);
    }

    public interface IInviteApproval
    {
        Guid InitiativeId { get; }
        string InitiativeName { get; }
        string InitiativeOwner { get; }
        //string Message { get; }
        DateTime InviteTime { get; }
        string InviteCode { get; }
    }

    public interface IUser
    {
        string Id { get; }
        string UserName { get; }
    }

    public interface IMachine
    {
        string Id { get; }
        string Name { get; }
        IDictionary<string, string> Data { get; }
    }

    public interface IToolkitCompatibilities
    {
        Version ServerVersion { get; }
        DateTime? RegisterDate { get; }
        string SupportToolkitNameVersion { get; }
        ECompatibility Compatibility { get; }
        DateTime? LastUsed { get; set; }
    }

    public interface IFingerprint
    {
        
    }

    public interface IApplicationVersionBusiness
    {
        IFingerprint AssureApplicationFingerprint(string applicationFingerprint, string version, string supportToolkitNameVersion, DateTime? buildTime, string applicationName, string clientToken);
        IApplicationVersion RegisterApplicationVersion(IFingerprint applicationVersionFingerprint, Guid applicationId, string version, string supportToolkitNameVersion, DateTime? buildTime);
        IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId);
    }

    //TODO: This is supposed to be the different repositories for SQL and MongoDB
    public interface IRepository
    {
        void AddInitiative(IInitiative initiative);
        void UpdateInitiative(IInitiative initiative);

        IEnumerable<IInitiative> GetInitiativesByDeveloper(string developerName);
        IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId);
        IEnumerable<IInitiative> GetInitiativeHeadsByDeveloper(string developerName);
        IEnumerable<IInitiative> GetInitiatives();
        IInitiative GetInitiative(Guid initiativeId);
        IInitiative GetInitiativeByClientToken(string clientToken);
        IInitiative GetInitiativeByApplication(Guid applicationId);
        void DeleteInitiative(Guid initiativeId);

        void UpdateApplicationVersion(IApplicationVersion applicationVersion);
        IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId);
        IEnumerable<IApplicationVersion> GetApplicationVersionsForDeveloper(string developerName);
        IEnumerable<IApplicationVersion> GetApplicationVersionsForApplications(IEnumerable<Guid> applicationIds);
        IEnumerable<IApplicationVersion> GetApplicationVersionsForMachine(string machineId);
        void UpdateSessionUsage(Guid sessionId, DateTime serverLastKnown);
        IApplicationVersion GetApplicationVersion(string applicationVersionFingerprint);
        void AddApplicationVersion(IApplicationVersion applicationVersion);
        void DeleteApplicationVersion(string applicationVersionFingerprint);
        IApplicationVersion UpdateApplicationVersionId(string applicationVersionFingerprint, Guid applicationId);

        void AddSession(ISession session);
        ISession GetSession(Guid id);
        IEnumerable<ISession> GetSessionsForApplicationVersion(string applicationVersionId);
        IEnumerable<ISession> GetSessionsForApplication(Guid applicationId);
        IEnumerable<ISession> GetSessionsForDeveloper(string developerName);
        IEnumerable<ISession> GetSessionsForMachine(string machineFingerprint);
        void EndSession(Guid sessionId, DateTime serverEndTime);

        IUser GetUser(string fingerprint);
        void AddUser(IUser user);
        IEnumerable<IUser> GetUsersByApplicationVersion(string applicationFingerprint);

        IMachine GetMachine(string id);
        void AddMachine(IMachine machine);
        IEnumerable<IMachine> GetMachinesByApplicationVersion(string applicationFingerprint);

        void RegisterToolkitCompability(Version serverVersion, DateTime registerDate, string supportToolkitNameVersion, ECompatibility compatibility);
        IEnumerable<IToolkitCompatibilities> GetToolkitCompability(Version version);
        IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> initiativeId);
        void UpdateMachine(IMachine machine);
        bool CanConnect();

        IEnumerable<ISession> GetSessionStatistics(DateTime from, DateTime to);
        IEnumerable<IIssue> GetIssueStatistics(DateTime from, DateTime to);
    }

    public interface IDeveloperRole
    {
        string DeveloperName { get; set; }
        string RoleName { get; set; }
        string InviteCode { get; }
        string InviteEMail { get; }
        DateTime InviteTime { get; }
        //string InviteMessage { get; } // TODO: Add this property
    }

    public interface IApplication
    {
        Guid Id { get; }
        string Name { get; }
        DateTime FirstRegistered { get; }
        string TicketPrefix { get; set; }
    }

    public interface IApplicationGroup
    {
        string Name { get; }
        IEnumerable<IApplication> Applications { get; }

        void Add(IApplication application);
        void Remove(IApplication application);
    }

    public interface IInitiative
    {
        Guid Id { get; }
        string Name { get; set; }
        string ClientToken { get; }
        string OwnerDeveloperName { get; set; }
        IEnumerable<IDeveloperRole> DeveloperRoles { get; }
        IEnumerable<IApplicationGroup> ApplicationGroups { get; }

        void AddApplicationGroup(IApplicationGroup applicationGroup);
        void RemoveApplicationGroup(IApplicationGroup applicationGroup);
        string AddDeveloperRolesInvitation(string email);
        void RemoveDeveloperRole(string developer);
        void DeclineInvitation(string inviteCode);
        void ConfirmInvitation(string inviteCode, string developerName);
    }


    public interface IIssue
    {
        Guid Id { get; }
        DateTime ClientTime { get; }
        DateTime ServerTime { get; }
        bool? VisibleToUser { get; }
        IDictionary<string, string> Data { get; }
        Guid? IssueThreadGuid { get; }
        string UserHandle { get; }
        string UserInput { get; }
        int Ticket { get; }
        Guid SessionId { get; }
    }

    public interface IInnerIssueType
    {
        string ExceptionTypeName { get; }
        string Message { get; }
        string StackTrace { get; }
        string IssueLevel { get; }
        IInnerIssueType Inner { get; }
    }

    public interface IIssueType
    {
        string ExceptionTypeName { get; }
        string Message { get; }
        string StackTrace { get; }
        IssueLevel IssueLevel { get; }
        IInnerIssueType Inner { get; }
        IEnumerable<IIssue> Issues { get; }
        int Ticket { get; }
        string ResponseMessage { get; set; }

        void Add(IIssue issue);
    }

    public interface IApplicationVersion
    {
        string Id { get; }
        Guid ApplicationId { get; }
        string Version { get; }
        IEnumerable<IIssueType> IssueTypes { get; }
        string ResponseMessage { get; set; }
        bool IsOfficial { get; set; }
        bool Ignore { get; set; }
        string SupportToolkitNameVersion { get; }
        DateTime? BuildTime { get; }

        void Add(IIssueType issueType);
    }

    public interface ILogResponse
    {
        string IssueInstanceTicket { get; }
        string IssueTypeTicket { get; }
        string ResponseMessage { get; }
    }

    public interface ISession
    {
        Guid Id { get; }
        string ApplicationVersionId { get; }
        string Environment { get; }
        Guid ApplicationId { get; }
        string MachineFingerprint { get; }
        string UserFingerprint { get; }
        DateTime ClientStartTime { get; }
        DateTime ServerStartTime { get; }
        DateTime? ServerEndTime { get; }
        DateTime? ServerLastKnown { get; }
        string CallerIp { get; }
    }

    public interface IMembershipUser
    {
        string DeveloperName { get; }
        string EMail { get; }
    }

    public enum ECompatibility { Compable, Incompatible, Inconclusive, Unknown }

    public enum IssueLevel
    {
        Information,
        Warning,
        Error
    }
}
