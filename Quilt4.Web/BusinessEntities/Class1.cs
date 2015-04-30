using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Web;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.Business;
using Tharga.Quilt4Net;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Web.BusinessEntities
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IRepository _repository;

        public UserBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<IUser> GetUsersByApplicationVersion(string applicationVersionId)
        {
            var users = _repository.GetUsersByApplicationVersion(applicationVersionId).OrderBy(x => x.UserName);
            return users;
        }

        public void RegisterUser(IFingerprint id, string userName)
        {
            var user = _repository.GetUser((Fingerprint)id);
            if (user == null)
                _repository.AddUser(new User((Fingerprint)id, userName));
        }

        public IUser GetUser(string userFingerprint)
        {
            return _repository.GetUser(userFingerprint);
        }
    }

    public static class IssueLevelExtensions
    {
        public static IssueLevel ToIssueLevel(this string issueLevel)
        {
            IssueLevel il;
            if (!Enum.TryParse(issueLevel.Replace("Message", "").Replace("Exception", ""), true, out il))
                throw new ArgumentException(string.Format("Invalid value for IssueLevel. Use one of the following; Information, Warning or Error.")).AddData("IssueLevel", issueLevel);
            return il;
        }
    }

    static class IssueTypeExtensions
    {
        public static bool AreEqual(this Tharga.Quilt4Net.DataTransfer.IssueType item, IIssueType issueType)
        {
            if (item.ExceptionTypeName != issueType.ExceptionTypeName) return false;
            if (String.Compare(Clean(item.Message), Clean(issueType.Message), StringComparison.InvariantCultureIgnoreCase) != 0) return false;
            if (String.Compare(Clean(item.StackTrace), Clean(issueType.StackTrace), StringComparison.InvariantCultureIgnoreCase) != 0) return false;
            if (item.IssueLevel.ToIssueLevel() != issueType.IssueLevel) return false;
            if (!item.Inner.AreEqual((Quilt4.BusinessEntities.IssueType)issueType.Inner)) return false;
            return true;
        }

        private static string Clean(string data)
        {
            if (data == null) return string.Empty;
            return data.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
        }
    }

    static class MachineExtensions
    {
        public static bool AreEqual(this IMachine item, IMachine other)
        {
            if (item.Data == null || other.Data == null)
            {
                if (item.Data != null)
                    return false;

                if (other.Data != null)
                    return false;

                return true;
            }

            if (item.Data.Count != other.Data.Count)
                return false;

            foreach (var a in item.Data)
            {
                if (other.Data[a.Key] != a.Value)
                    return false;
            }

            return true;
        }
    }

    public class MachineBusiness : IMachineBusiness
    {
        private readonly IRepository _repository;

        public MachineBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public void RegisterMachine(IFingerprint id, string name, IDictionary<string, string> data)
        {
            var machine = _repository.GetMachine((Fingerprint)id);
            if (machine == null)
                _repository.AddMachine(new Machine((Fingerprint)id, name, data));
            else
            {
                var newMachine = new Machine((Fingerprint)id, machine.Name, data);
                if (!machine.AreEqual(newMachine))
                {
                    _repository.UpdateMachine(newMachine);
                }
            }
        }

        public IMachine GetMachine(string machineFingerprint)
        {
            return _repository.GetMachine(machineFingerprint);
        }

        public IEnumerable<IMachine> GetMachinesByApplicationVersion(string applicationVersionId)
        {
            var machines = _repository.GetMachinesByApplicationVersion(applicationVersionId).OrderBy(x => x.Name);
            return machines;
        }

        public IMachine GetMachine(Fingerprint machineFinterprint)
        {
            return _repository.GetMachine(machineFinterprint);
        }
    }

    public class SessionBusiness : ISessionBusiness
    {
        internal int ThreadTestDelay;
        private readonly IRepository _repository;

        public SessionBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public void RegisterSession(ISession session)
        {
            var mutex = new Mutex(false, session.Id.ToString());

            try
            {
                mutex.WaitOne();

                var existingSession = _repository.GetSession(session.Id);
                Thread.Sleep(ThreadTestDelay);
                if (existingSession == null)
                    _repository.AddSession(session);
                else
                {
                    if (session.ApplicationVersionId != existingSession.ApplicationVersionId)
                    {
                        var ex = new ArgumentException("The session belongs to a different application version.");
                        ex.Data.Add("SessionId", session.Id);
                        ex.Data.Add("ApplicationVersionId", session.ApplicationVersionId);
                        ex.Data.Add("ExistingApplicationVersionId", existingSession.ApplicationVersionId);
                        throw ex;
                    }
                    _repository.UpdateSessionUsage(session.Id, DateTime.UtcNow);
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public void EndSession(Guid sessionId)
        {
            _repository.EndSession(sessionId, DateTime.UtcNow);
        }

        public IEnumerable<ISession> GetSessionsForApplicationVersion(string applicationVersionId)
        {
            return _repository.GetSessionsForApplicationVersion(applicationVersionId).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForApplication(Guid applicationId)
        {
            return _repository.GetSessionsForApplication(applicationId).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForDeveloper(string developerName)
        {
            return _repository.GetSessionsForDeveloper(developerName).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> initiativeId)
        {
            return _repository.GetSessionsForApplications(initiativeId).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForMachine(Fingerprint machineFingerprint)
        {
            return _repository.GetSessionsForMachine(machineFingerprint);
        }
    }

    public class InviteApproval : IInviteApproval
    {
        public Guid InitiativeId { get; set; }
        public string InitiativeName { get; set; }
        public string InitiativeOwner { get; set; }
        public string Message { get; set; }
        public DateTime InviteTime { get; set; }
        public string InviteCode { get; set; }
    }

    public class ClientToken : IClientToken
    {
        private readonly string _value;

        private ClientToken(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value", "No value for client token provided.");

            _value = value;
        }

        public static implicit operator string(ClientToken item)
        {
            return item._value;
        }

        public static implicit operator ClientToken(string item)
        {
            return new ClientToken(item);
        }
    }

    public class InitiativeBusiness : IInitiativeBusiness
    {
        private readonly IRepository _repository;

        public InitiativeBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<IInitiative> GetAllByDeveloper(string developerName)
        {
            var initiatives = _repository.GetInitiativesByDeveloper(developerName).ToList();
            if (!initiatives.Any())
            {
                var defaultInitiative = new Initiative(Guid.NewGuid(), null, GenerateClientToken(), developerName ?? "*", new List<IDeveloperRole>(), new List<ApplicationGroup>());
                initiatives.Insert(0, defaultInitiative);
                _repository.AddInitiative(defaultInitiative);
            }
            return initiatives;
        }

        public IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId)
        {
            var initiatives = _repository.GetApplicationGroups(initiativeId).ToList();
            return initiatives;
        }

        public IEnumerable<IInitiative> GetAllHeadsByDeveloper(string developerName)
        {
            var initiatives = _repository.GetInitiativeHeadsByDeveloper(developerName).ToList();
            if (!initiatives.Any())
            {
                var defaultInitiative = new Initiative(Guid.NewGuid(), null, GenerateClientToken(), developerName ?? "*", new List<IDeveloperRole>(), new List<ApplicationGroup>());
                initiatives.Insert(0, defaultInitiative);
                _repository.AddInitiative(defaultInitiative);
            }
            return initiatives;
        }


        private string GenerateClientToken()
        {
            var response = RandomUtility.GetRandomString(32);
            return response;
        }

        public void Create(string developerName, string initiativename)
        {
            developerName = developerName ?? "*";

            var initiative = new Initiative(Guid.NewGuid(), initiativename, GenerateClientToken(), developerName, new List<IDeveloperRole>(), new List<ApplicationGroup>());
            _repository.AddInitiative(initiative);
        }

        public IInitiative GetInitiative(Guid initiativeId)
        {
            return _repository.GetInitiative(initiativeId);
        }

        public IInitiative GetInitiativeByApplication(Guid applicationId)
        {
            return _repository.GetInitiativeByApplication(applicationId);
        }

        public IApplication GetApplication(Guid applicationId)
        {
            var initiative = _repository.GetInitiativeByApplication(applicationId);
            var applicationGroup = initiative.ApplicationGroups.Single(x => x.Applications.Any(y => y.Id == applicationId));
            var app = applicationGroup.Applications.Single(x => x.Id == applicationId);
            return app;
        }

        public IApplication RegisterApplication(IClientToken clientToken, string applicationName, string applicationVersionFingerprint)
        {
            if (clientToken == null) throw new ArgumentNullException("clientToken");
            if (applicationName == null) throw new ArgumentNullException("applicationName");

            var initiative = _repository.GetInitiativeByClientToken((ClientToken)clientToken);

            if (initiative == null) throw new InvalidOperationException("Invalid client token.");

            var application = initiative.ApplicationGroups.SelectMany(x => x.Applications).SingleOrDefault(x => x.Name == applicationName);
            if (application != null)
                return application;

            var applicationGroup = GetDefaultApplicationGroup(initiative);

            application = new Application(Guid.NewGuid(), applicationName, DateTime.UtcNow, null);
            applicationGroup.Add(application);

            _repository.UpdateInitiative(initiative);

            return application;
        }

        private static IApplicationGroup GetDefaultApplicationGroup(IInitiative initiative)
        {
            var applicationGroup = initiative.ApplicationGroups.SingleOrDefault(x => x.Name == null);
            if (applicationGroup == null)
            {
                applicationGroup = new ApplicationGroup(null, new List<IApplication>());
                initiative.AddApplicationGroup(applicationGroup);
            }
            return applicationGroup;
        }

        public void SaveInitiative(IInitiative initiative)
        {
            if (initiative.Name == string.Empty)
                initiative.Name = null;

            _repository.UpdateInitiative(initiative);
        }

        public void DeleteInitiative(Guid initiativeId)
        {
            _repository.DeleteInitiative(initiativeId);
        }

        public void DeleteApplication(Guid applicationId)
        {
            var initiative = GetInitiativeByApplication(applicationId);
            var applicationGroup = initiative.ApplicationGroups.Single(x => x.Applications.Any(y => y.Id == applicationId));
            var app = applicationGroup.Applications.Single(x => x.Id == applicationId);
            applicationGroup.Remove(app);

            DeleteApplicationVersions(applicationId);

            SaveInitiative(initiative);
        }

        private void DeleteApplicationVersions(Guid applicationId)
        {
            var versions = _repository.GetApplicationVersions(applicationId);
            foreach (var version in versions)
                _repository.DeleteApplicationVersion(version.Id);
        }

        public IEnumerable<IInitiative> GetInitiatives()
        {
            return _repository.GetInitiatives();
        }

        public IEnumerable<IInviteApproval> GetPendingApprovals(string developerEMail)
        {
            var initiatives = _repository.GetInitiatives().Where(x => x.DeveloperRoles.Any(y => y.InviteEMail == developerEMail && y.RoleName == "Invited")).ToArray();
            return initiatives.Select(x =>
            {
                var developerRole = x.DeveloperRoles.Single(y => y.InviteEMail == developerEMail && y.RoleName == "Invited");
                return new InviteApproval
                {
                    InitiativeId = x.Id,
                    InitiativeName = x.Name,
                    Message = developerRole.RoleName,
                    InviteTime = developerRole.InviteTime,
                    InitiativeOwner = x.OwnerDeveloperName,
                    InviteCode = developerRole.InviteCode
                };
            });
        }
    }

    public class ApplicationVersionBusiness : IApplicationVersionBusiness
    {
        private readonly IRepository _repository;

        public ApplicationVersionBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId)
        {
            var response = _repository.GetApplicationVersions(applicationId);
            return response;
        }

        public IFingerprint AssureApplicationFingerprint(string applicationFingerprint, string version, string supportToolkitNameVersion, DateTime? buildTime, string applicationName, string clientToken)
        {
            if (applicationFingerprint == null)
                applicationFingerprint = string.Format("AI2:{0}", string.Format("{0}{1}{2}{3}{4}", applicationName, version, supportToolkitNameVersion, clientToken, buildTime).ToMd5Hash());

            //Make sure that provided data is valid for the provided fingerprint
            var applicationVersion = _repository.GetApplicationVersion(applicationFingerprint);
            if (applicationVersion != null)
            {
                var application = _repository.GetInitiativeByApplication(applicationVersion.ApplicationId).ApplicationGroups.SelectMany(x => x.Applications).First(x => x.Id == applicationVersion.ApplicationId);

                if (application.Name != applicationName) throw new InvalidOperationException("Provided application name does not match the application name stored for the application fingerprint. If the application name has changed a new fingerprint needs to be provided, or provide null and the server will generate a fingerprint for you.");
                if (applicationVersion.Version != version) throw new InvalidOperationException("Provided version does not match the version stored for the application fingerprint. If the version has changed a new fingerprint needs to be provided, or provide null and the server will generate a fingerprint for you.");
                if (applicationVersion.SupportToolkitNameVersion != supportToolkitNameVersion) throw new InvalidOperationException("Provided supportToolkitNameVersion does not match the supportToolkitNameVersion stored for the application fingerprint. If the supportToolkitNameVersion has changed a new fingerprint needs to be provided, or provide null and the server will generate a fingerprint for you.");
                if (applicationVersion.BuildTime != null || buildTime != null)
                {
                    if (applicationVersion.BuildTime == null || buildTime == null || applicationVersion.BuildTime.Value.ToUniversalTime() != buildTime.Value.ToUniversalTime())
                    {
                        throw new InvalidOperationException("Provided buildTime does not match the buildTime stored for the application fingerprint. If the buildTime has changed a new fingerprint needs to be provided, or provide null and the server will generate a fingerprint for you.");
                    }
                }
            }

            return (Fingerprint)applicationFingerprint;
        }

        public IApplicationVersion RegisterApplicationVersion(IFingerprint applicationVersionFingerprint, Guid applicationId, string version, string supportToolkitNameVersion, DateTime? buildTime)
        {
            Version tmp;
            if (string.IsNullOrEmpty(version)) throw new ArgumentException(String.Format("No version provided for application."));
            if (!Version.TryParse(version, out tmp)) throw new ArgumentException(String.Format("Invalid version format."));

            var applicationVersion = RegisterApplicationVersionEx((Fingerprint)applicationVersionFingerprint, applicationId);
            if (applicationVersion != null)
                return applicationVersion;

            try
            {
                applicationVersion = new ApplicationVersion((Fingerprint)applicationVersionFingerprint, applicationId, version, new List<IIssueType>(), null, false, false, supportToolkitNameVersion, buildTime);
                _repository.AddApplicationVersion(applicationVersion);
            }
            catch (System.Data.SqlClient.SqlException)
            {
                applicationVersion = RegisterApplicationVersionEx((Fingerprint)applicationVersionFingerprint, applicationId);
                if (applicationVersion == null)
                    throw;
            }
            return applicationVersion;
        }

        private IApplicationVersion RegisterApplicationVersionEx(Fingerprint applicationVersionFingerprint, Guid applicationId)
        {
            var response = _repository.GetApplicationVersion(applicationVersionFingerprint);
            if (response != null)
            {
                if (response.ApplicationId != applicationId)
                    response = _repository.UpdateApplicationVersionId(applicationVersionFingerprint, applicationId);
            }
            return response;
        }

        public IApplicationVersion GetApplicationVersion(Fingerprint applicationVersionFingerprint)
        {
            return _repository.GetApplicationVersion(applicationVersionFingerprint);
        }

        public void SaveApplicationVersion(IApplicationVersion applicationVersion)
        {
            _repository.UpdateApplicationVersion(applicationVersion);
        }

        public void Remove(Fingerprint applicationVersionFingerprint)
        {
            _repository.DeleteApplicationVersion(applicationVersionFingerprint);
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForDeveloper(string developerName)
        {
            return _repository.GetApplicationVersionsForDeveloper(developerName);
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForApplications(IEnumerable<Guid> initiativeId)
        {
            return _repository.GetApplicationVersionsForApplications(initiativeId);
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForMachine(string machineId)
        {
            return _repository.GetApplicationVersionsForMachine(machineId);
        }

        //public IEnumerable<IApplicationVersion> GetApplicationVersionsForApplications(IEnumerable<Guid> initiative)
        //{
        //    return _repository.GetApplicationVersionsForApplications(initiative);
        //}
    }
}