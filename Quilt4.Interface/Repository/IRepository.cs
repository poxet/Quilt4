using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IRepository
    {
        void AddInitiative(IInitiative initiative);
        void UpdateInitiative(IInitiative initiative);
        void UpdateInitiative(Guid id, string name, string sessionToken, string owner);

        //IEnumerable<IInitiative> GetInitiativesByDeveloper(string developerName);
        IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId);
        IEnumerable<IInitiativeHead> GetInitiativeHeadsByDeveloper(string developerName, string[] roleNames);
        IEnumerable<IInitiative> GetInitiatives();
        IInitiative GetInitiative(Guid initiativeId);
        IInitiative GetInitiativeByClientToken(string clientToken);
        IInitiative GetInitiativeByApplication(Guid applicationId);
        void DeleteInitiative(Guid initiativeId);

        void UpdateApplicationVersion(IApplicationVersion applicationVersion);
        IEnumerable<IApplicationVersion> GetApplicationVersions();
        IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId);
        IEnumerable<IApplicationVersion> GetArchivedApplicationVersions(Guid applicationId);
        IEnumerable<IApplicationVersion> GetApplicationVersionsForDeveloper(string developerName);
        IEnumerable<IApplicationVersion> GetApplicationVersionsForApplications(IEnumerable<Guid> applicationIds);
        IEnumerable<IApplicationVersion> GetApplicationVersionsForMachine(string machineId);
        void UpdateSessionUsage(Guid sessionId, DateTime serverLastKnown);
        IApplicationVersion GetApplicationVersion(string applicationVersionFingerprint);
        void AddApplicationVersion(IApplicationVersion applicationVersion);
        void DeleteApplicationVersion(string applicationVersionFingerprint);
        void DeleteApplicationVersionForApplication(Guid applicationVersionId);
        IApplicationVersion UpdateApplicationVersionId(string applicationVersionFingerprint, Guid applicationId);

        void AddSession(ISession session);
        ISession GetSession(Guid id);
        IEnumerable<ISession> GetSessionsForApplicationVersion(string applicationVersionId);
        IEnumerable<ISession> GetSessionsForApplication(Guid applicationId);
        IEnumerable<ISession> GetSessionsForDeveloper(string developerName);
        IEnumerable<ISession> GetSessionsForMachine(string machineFingerprint);
        //IEnumerable<ISession> GetActiveSessions(int timeoutSeconds);
        void EndSession(Guid sessionId, DateTime serverEndTime);

        IUser GetUser(string fingerprint);
        void AddUser(IUser user);
        IEnumerable<IUser> GetUsersByApplicationVersion(string applicationFingerprint);

        IMachine GetMachine(string id);
        void AddMachine(IMachine machine);
        IEnumerable<IMachine> GetMachinesByApplicationVersion(string applicationFingerprint);
        IEnumerable<IMachine> GetMachinesByApplicationVersions(IEnumerable<string> applicationFingerprints);

        void RegisterToolkitCompability(Version serverVersion, DateTime registerDate, string supportToolkitNameVersion, Compatibility compatibility);
        IEnumerable<IToolkitCompatibilities> GetToolkitCompability(Version version);
        IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> initiativeId);
        void UpdateMachine(IMachine machine);
        bool CanConnect();

        IEnumerable<ISession> GetSessionStatistics(DateTime from, DateTime to);
        IEnumerable<IIssue> GetIssueStatistics(DateTime from, DateTime to);

        IDataBaseInfo GetDatabaseStatus();
        void LogEmail(string fromEmail, string to, string subject, string body, DateTime dateSent, bool status, string errorMessage);
        IEnumerable<IEmail> GetLastHundredEmails();
        int GetInitiativeCount();
        int GetApplicationCount();
        int GetIssueTypeCount();
        int GetIssueCount();

        ISetting GetSetting(string name);
        IEnumerable<ISetting> GetSettings();
        void SetSetting(ISetting setting);
        void DeleteSessionForApplication(Guid applicationId);
        IEnumerable<ISession> GetSessionsForUser(string userId);
        //IApplication GetApplicationByApplicationId(Guid applicationId);
        void ArchiveApplicationVersion(string versionId);
        IEnumerable<IInvitation> GetInvitations(string email);
        IEnumerable<ISession> GetSessions();
    }
}