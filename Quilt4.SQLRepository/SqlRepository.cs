using System;
using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.SQLRepository
{
    public class SqlRepository : IRepository
    {
        public void AddInitiative(IInitiative initiative)
        {
            throw new NotImplementedException();
        }

        public void UpdateInitiative(IInitiative initiative)
        {
            throw new NotImplementedException();
        }

        public void UpdateInitiative(Guid id, string name, string sessionToken, string owner)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInitiative> GetInitiativesByDeveloper(string developerName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInitiative> GetInitiativeHeadsByDeveloper(string developerName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInitiative> GetInitiatives()
        {
            throw new NotImplementedException();
        }

        public IInitiative GetInitiative(Guid initiativeId)
        {
            throw new NotImplementedException();
        }

        public IInitiative GetInitiativeByClientToken(string clientToken)
        {
            throw new NotImplementedException();
        }

        public IInitiative GetInitiativeByApplication(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public void DeleteInitiative(Guid initiativeId)
        {
            throw new NotImplementedException();
        }

        public void UpdateApplicationVersion(IApplicationVersion applicationVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForDeveloper(string developerName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForApplications(IEnumerable<Guid> applicationIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForMachine(string machineId)
        {
            throw new NotImplementedException();
        }

        public void UpdateSessionUsage(Guid sessionId, DateTime serverLastKnown)
        {
            throw new NotImplementedException();
        }

        public IApplicationVersion GetApplicationVersion(string applicationVersionFingerprint)
        {
            throw new NotImplementedException();
        }

        public void AddApplicationVersion(IApplicationVersion applicationVersion)
        {
            throw new NotImplementedException();
        }

        public void DeleteApplicationVersion(string applicationVersionFingerprint)
        {
            throw new NotImplementedException();
        }

        public IApplicationVersion UpdateApplicationVersionId(string applicationVersionFingerprint, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public void AddSession(ISession session)
        {
            throw new NotImplementedException();
        }

        public ISession GetSession(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetSessionsForApplicationVersion(string applicationVersionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetSessionsForApplication(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetSessionsForDeveloper(string developerName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetSessionsForMachine(string machineFingerprint)
        {
            throw new NotImplementedException();
        }

        public void EndSession(Guid sessionId, DateTime serverEndTime)
        {
            throw new NotImplementedException();
        }

        public IUser GetUser(string fingerprint)
        {
            throw new NotImplementedException();
        }

        public void AddUser(IUser user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IUser> GetUsersByApplicationVersion(string applicationFingerprint)
        {
            throw new NotImplementedException();
        }

        public IMachine GetMachine(string id)
        {
            throw new NotImplementedException();
        }

        public void AddMachine(IMachine machine)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMachine> GetMachinesByApplicationVersion(string applicationFingerprint)
        {
            throw new NotImplementedException();
        }

        public void RegisterToolkitCompability(Version serverVersion, DateTime registerDate, string supportToolkitNameVersion, ECompatibility compatibility)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolkitCompatibilities> GetToolkitCompability(Version version)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> initiativeId)
        {
            throw new NotImplementedException();
        }

        public void UpdateMachine(IMachine machine)
        {
            throw new NotImplementedException();
        }

        public bool CanConnect()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetSessionStatistics(DateTime @from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IIssue> GetIssueStatistics(DateTime @from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public IDataBaseInfo GetDatabaseStatus()
        {
            throw new NotImplementedException();
        }

        public void LogEmail(string fromEmail, string to, string subject, string body, DateTime dateSent, bool status, string errorMessage)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEmail> GetLastHundredEmails()
        {
            throw new NotImplementedException();
        }

        public int GetInitiativeCount()
        {
            throw new NotImplementedException();
        }

        public int GetApplicationCount()
        {
            throw new NotImplementedException();
        }

        public int GetIssueTypeCount()
        {
            throw new NotImplementedException();
        }

        public int GetIssueCount()
        {
            throw new NotImplementedException();
        }

        public ISetting GetSetting(string name)
        {
            throw new NotImplementedException();
        }

        public T GetSetting<T>(string name, T defaultValue)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISetting> GetSettings()
        {
            throw new NotImplementedException();
        }

        public void SetSetting(string name, string value, Type type)
        {
            throw new NotImplementedException();
        }
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMachine> GetMachinesByApplicationVersions(IEnumerable<string> applicationFingerprints)
    }
}