using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Quilt4.Interface;
using Quilt4.MongoDBRepository;
using Quilt4.SQLRepository;
using IUser = Quilt4.Interface.IUser;

namespace Quilt4.MultiRepository
{
    public class MultiRepository : IRepository
    {
        private readonly List<IRepository> _repositories;

        public MultiRepository()
        {
            _repositories = new List<IRepository>();
            _repositories.Add(new MongoRepository());
            _repositories.Add(new SqlRepository());
        }

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

        public IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInitiativeHead> GetInitiativeHeadsByDeveloper(string developerEmail, string[] roleNames)
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

        public IEnumerable<IApplicationVersion> GetApplicationVersions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApplicationVersion> GetArchivedApplicationVersions(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForDeveloper(string developerEmail)
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

        public void DeleteApplicationVersionForApplication(Guid applicationVersionId)
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

        public IEnumerable<ISession> GetSessionsForDeveloper(string developerEmail)
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

        public IEnumerable<IMachine> GetMachinesByApplicationVersions(IEnumerable<string> applicationFingerprints)
        {
            throw new NotImplementedException();
        }

        public void RegisterToolkitCompability(Version serverVersion, DateTime registerDate, string supportToolkitNameVersion, Compatibility compatibility)
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
            var responses = _repositories.Select(x => x.GetSetting(name));
            var reference = responses.First();

            foreach (var response in responses)
            {
                //TODO: Compare with reference
            }

            return reference;
        }

        public IEnumerable<ISetting> GetSettings()
        {
            throw new NotImplementedException();
        }

        public void SetSetting(ISetting setting)
        {
            throw new NotImplementedException();
        }

        public void DeleteSessionForApplication(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetSessionsForUser(string userId)
        {
            throw new NotImplementedException();
        }

        public void ArchiveApplicationVersion(string versionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInvitation> GetInvitations(string email)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetSessions()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> GetEnvironmentColors(string userId)
        {
            throw new NotImplementedException();
        }

        public void UpdateEnvironmentColors(string userId, IDictionary<string, string> environmentColors)
        {
            throw new NotImplementedException();
        }

        public void AddEnvironmentColors(string userId, IDictionary<string, string> environmentColors)
        {
            throw new NotImplementedException();
        }

        public void ArchiveSessionsForApplicationVersion(string versionId)
        {
            throw new NotImplementedException();
        }

        public void DeleteSession(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public IApplicationVersion GetApplicationVersionByIssue(Guid issueId)
        {
            throw new NotImplementedException();
        }

        public IApplication GetApplicationByVersion(string versionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetArchivedSessionsForApplications(IEnumerable<Guid> applicationsIds)
        {
            throw new NotImplementedException();
        }

        public IApplication GetApplication(Guid applicationId)
        {
            throw new NotImplementedException();
        }
    }
}
