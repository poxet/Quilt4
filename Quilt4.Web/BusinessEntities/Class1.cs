using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Web;
using Quilt4.Interface;
using Quilt4.Web.Business;
using Tharga.Quilt4Net;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Web.BusinessEntities
{
    public class Issue : IIssue
    {
        private readonly Guid _id;
        private readonly DateTime _clientTime;
        private readonly DateTime _serverTime;
        private readonly bool? _visibleToUser;
        private readonly IDictionary<string, string> _data;
        private readonly Guid? _issueThreadGuid;
        private readonly string _userHandle;
        private readonly string _userInput;
        private readonly int _ticket;
        private readonly Guid _sessionId;

        public Issue(Guid id, DateTime clientTime, DateTime serverTime, bool? visibleToUser, IDictionary<string, string> data, Guid? issueThreadGuid, string userHandle, string userInput, int ticket, Guid sessionId)
        {
            _id = id;
            _clientTime = clientTime;
            _serverTime = serverTime;
            _visibleToUser = visibleToUser;
            _data = data;
            _issueThreadGuid = issueThreadGuid;
            _userHandle = userHandle;
            _userInput = userInput;
            _ticket = ticket;
            _sessionId = sessionId;
        }

        public Guid Id { get { return _id; } }
        public DateTime ClientTime { get { return _clientTime; } }
        public DateTime ServerTime { get { return _serverTime; } }
        public bool? VisibleToUser { get { return _visibleToUser; } }
        public IDictionary<string, string> Data { get { return _data; } }
        public Guid? IssueThreadGuid { get { return _issueThreadGuid; } }
        public string UserHandle { get { return _userHandle; } }
        public string UserInput { get { return _userInput; } }
        public int Ticket { get { return _ticket; } }
        public Guid SessionId { get { return _sessionId; } }
    }

    public class InnerIssueType : IInnerIssueType
    {
        private readonly string _exceptionTypeName;
        private readonly string _message;
        private readonly string _stackTrace;
        private readonly string _issueLevel;
        private readonly IInnerIssueType _innerIssueType;

        public InnerIssueType(string exceptionTypeName, string message, string stackTrace, string issueLevel, IInnerIssueType innerIssueType)
        {
            _exceptionTypeName = exceptionTypeName;
            _message = message;
            _stackTrace = stackTrace;
            _issueLevel = issueLevel;
            _innerIssueType = innerIssueType;
        }

        public string ExceptionTypeName { get { return _exceptionTypeName; } }
        public string Message { get { return _message; } }
        public string StackTrace { get { return _stackTrace; } }
        public string IssueLevel { get { return _issueLevel; } }
        public IInnerIssueType Inner { get { return _innerIssueType; } }
    }

    public class IssueType : IIssueType
    {
        private readonly string _exceptionTypeName;
        private readonly string _message;
        private readonly string _stackTrace;
        private readonly IssueLevel _issueLevel;
        private readonly IInnerIssueType _inner;
        private readonly List<IIssue> _issues;
        private readonly int _ticket;

        public IssueType(string exceptionTypeName, string message, string stackTrace, IssueLevel issueLevel, IInnerIssueType inner, IEnumerable<IIssue> issues, int ticket, string responseMessage)
        {
            _exceptionTypeName = exceptionTypeName;
            _message = message;
            _stackTrace = stackTrace;
            _issueLevel = issueLevel;
            _inner = inner;
            _issues = issues.ToList();
            _ticket = ticket;
            ResponseMessage = responseMessage;
        }

        public string ExceptionTypeName { get { return _exceptionTypeName; } }
        public string Message { get { return _message; } }
        public string StackTrace { get { return _stackTrace; } }
        public IssueLevel IssueLevel { get { return _issueLevel; } }
        public IInnerIssueType Inner { get { return _inner; } }
        public IEnumerable<IIssue> Issues { get { return _issues; } }
        public int Ticket { get { return _ticket; } }
        public string ResponseMessage { get; set; }

        public void Add(IIssue issue)
        {
            _issues.Add(issue);
        }
    }

    public class User : IUser
    {
        private readonly string _fingerprint;
        private readonly string _userName;

        public User(string fingerprint, string userName)
        {
            _fingerprint = fingerprint;
            _userName = userName;
        }

        public string Id { get { return _fingerprint; } }
        public string UserName { get { return _userName; } }
    }

    public class UserBusiness : IUserBusiness
    {
        private readonly IRepository2 _repository;

        public UserBusiness(IRepository2 repository)
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

    public class Machine : IMachine
    {
        private readonly string _fingerprint;
        private readonly string _name;
        private IDictionary<string, string> _data;

        public Machine(string fingerprint, string name, IDictionary<string, string> data)
        {
            _fingerprint = fingerprint;
            _name = name;
            _data = data;
        }

        public string Id { get { return _fingerprint; } }
        public string Name { get { return _name; } }

        public IDictionary<string, string> Data
        {
            get { return _data; }
            set { _data = value; }
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
            if (!item.Inner.AreEqual((IssueType)issueType.Inner)) return false;
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
        private readonly IRepository2 _repository;

        public MachineBusiness(IRepository2 repository)
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

    public class Session : ISession
    {
        private readonly Guid _id;
        private readonly Fingerprint _applicationVersionId;
        private readonly string _environment;
        private readonly Guid _applicationId;
        private readonly Fingerprint _machineId;
        private readonly Fingerprint _userId;
        private readonly DateTime _clientStartTime;
        private readonly DateTime _serverStartTime;
        private readonly DateTime? _serverEndTime;
        private readonly DateTime? _serverLastKnown;
        private readonly string _callerIp;

        public Session(Guid id, Fingerprint applicationVersionId, string environment, Guid applicationId, Fingerprint machineId, Fingerprint userId, DateTime clientStartTime, DateTime serverStartTime, DateTime? serverEndTime, DateTime? serverLastKnown, string callerIp)
        {
            _id = id;
            _applicationVersionId = applicationVersionId;
            _environment = environment;
            _applicationId = applicationId;
            _machineId = machineId;
            _userId = userId;
            _clientStartTime = clientStartTime;
            _serverStartTime = serverStartTime;
            _serverEndTime = serverEndTime;
            _serverLastKnown = serverLastKnown;
            _callerIp = callerIp;
        }

        public Guid Id { get { return _id; } }
        public string ApplicationVersionId { get { return _applicationVersionId; } }
        public string Environment { get { return _environment; } }
        public Guid ApplicationId { get { return _applicationId; } }
        public string MachineFingerprint { get { return _machineId; } }
        public string UserFingerprint { get { return _userId; } }
        public DateTime ClientStartTime { get { return _clientStartTime; } }
        public DateTime ServerStartTime { get { return _serverStartTime; } }
        public DateTime? ServerEndTime { get { return _serverEndTime; } }
        public DateTime? ServerLastKnown { get { return _serverLastKnown; } }
        public string CallerIp { get { return _callerIp; } }
    }

    public class SessionBusiness : ISessionBusiness
    {
        internal int ThreadTestDelay;
        private readonly IRepository2 _repository;

        public SessionBusiness(IRepository2 repository)
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

    public class Application : IApplication
    {
        private readonly Guid _id;
        private readonly string _name;
        private readonly DateTime _firstRegistered;

        public Application(Guid id, string name, DateTime firstRegistered, string ticketPrefix)
        {
            _id = id;
            _name = name;
            _firstRegistered = firstRegistered;
            TicketPrefix = ticketPrefix;
        }

        public Guid Id { get { return _id; } }
        public string Name { get { return _name; } }
        public DateTime FirstRegistered { get { return _firstRegistered; } }
        public string TicketPrefix { get; set; }
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

    public class ApplicationGroup : IApplicationGroup
    {
        private readonly string _name;
        private readonly List<IApplication> _applications;

        public ApplicationGroup(string name, IEnumerable<IApplication> applications)
        {
            _name = name;
            _applications = applications.ToList();
        }

        public string Name { get { return _name; } }
        public IEnumerable<IApplication> Applications { get { return _applications; } }

        public void Add(IApplication application)
        {
            if (_applications.Any(x => string.Compare(x.Name, application.Name, StringComparison.InvariantCultureIgnoreCase) == 0))
                throw new InvalidOperationException("The application name already exists in this group.");

            _applications.Add(application);
        }

        public void Remove(IApplication application)
        {
            if (!_applications.Remove(application))
                throw new InvalidOperationException("Cannot remove application from application group.");
        }
    }

    public class DeveloperRole : IDeveloperRole
    {
        private readonly string _inviteCode;
        private readonly string _inviteEMail;
        private readonly DateTime _inviteTime;
        private string _developerName;
        private string _roleName;

        public DeveloperRole(string developerName, string roleName, string inviteCode, string inviteEMail, DateTime inviteTime)
        {
            _developerName = developerName;
            _roleName = roleName;
            _inviteCode = inviteCode;
            _inviteEMail = inviteEMail;
            _inviteTime = inviteTime;
        }

        public string DeveloperName { get { return !string.IsNullOrEmpty(_developerName) ? _developerName : null; } set { _developerName = value; } }
        public string RoleName { get { return _roleName; } set { _roleName = value; } }
        public string InviteCode { get { return _inviteCode; } }
        public string InviteEMail { get { return _inviteEMail; } }
        public DateTime InviteTime { get { return _inviteTime; } }
    }

    public class RandomUtility
    {
        private static readonly Random Rng = new Random((int)DateTime.UtcNow.Ticks);

        public class CharInterval
        {
            public int Interval;
            public char Chr;
        }

        public static string GetRandomString(int size, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", CharInterval charInterval = null)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < size; i++)
            {
                char ch;
                if (charInterval != null && (i + 1) % (charInterval.Interval + 1) == 0)
                    ch = charInterval.Chr;
                else
                {
                    //var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Rng.NextDouble() + 65)));
                    var index = Convert.ToInt32(Math.Floor(chars.Length * Rng.NextDouble()));
                    ch = chars[index];
                }
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }

    public class Initiative : IInitiative
    {
        private readonly Guid _id;
        private readonly string _clientToken;
        private readonly List<IDeveloperRole> _developerRoles;
        private readonly List<IApplicationGroup> _applicationGroups;
        private string _name;
        private string _ownerDeveloperName;

        public Initiative(Guid id, string name, string clientToken, string ownerDeveloperName, IEnumerable<IDeveloperRole> developerRoles, IEnumerable<IApplicationGroup> applicationGroups)
        {
            _id = id;
            Name = name;
            _clientToken = clientToken;
            OwnerDeveloperName = ownerDeveloperName;
            _applicationGroups = applicationGroups.ToList();
            _developerRoles = developerRoles.ToList();
        }

        public Guid Id { get { return _id; } }
        public string Name { get { return !string.IsNullOrEmpty(_name) ? _name : null; } set { _name = value; } }
        public string ClientToken { get { return _clientToken; } }
        public string OwnerDeveloperName
        {
            get { return _ownerDeveloperName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new InvalidOperationException(string.Format("Initiative needs to have a owner developer to be saved."));

                _ownerDeveloperName = value;
            }
        }
        public IEnumerable<IDeveloperRole> DeveloperRoles { get { return _developerRoles; } }
        public IEnumerable<IApplicationGroup> ApplicationGroups { get { return _applicationGroups; } }

        public void AddApplicationGroup(IApplicationGroup applicationGroup)
        {
            if (_applicationGroups.Any(x => string.Compare(x.Name, applicationGroup.Name, StringComparison.InvariantCultureIgnoreCase) == 0))
                throw new InvalidOperationException("There is already an application group with this name in this initiative.");

            _applicationGroups.Add(applicationGroup);
        }

        public void RemoveApplicationGroup(IApplicationGroup applicationGroup)
        {
            if (!_applicationGroups.Remove(applicationGroup))
                throw new InvalidOperationException("Cannot remove application group from initiative.");
        }

        public string AddDeveloperRolesInvitation(string email)
        {
            var inviteCode = RandomUtility.GetRandomString(10);
            _developerRoles.Add(new DeveloperRole(null, "Invited", inviteCode, email, DateTime.UtcNow));
            return inviteCode;
        }

        public void RemoveDeveloperRole(string developer)
        {
            var item = _developerRoles.FirstOrDefault(x => string.Compare(x.DeveloperName, developer, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (item == null)
                item = _developerRoles.FirstOrDefault(x => string.Compare(x.InviteEMail, developer, StringComparison.InvariantCultureIgnoreCase) == 0);

            if (item != null)
                _developerRoles.Remove(item);
        }

        public void DeclineInvitation(string inviteCode)
        {
            var item = _developerRoles.FirstOrDefault(x => string.Compare(x.InviteCode, inviteCode, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (item == null) throw new NullReferenceException(string.Format("Cannot find invitation with provided code."));
            item.RoleName = "Declined";
        }

        public void ConfirmInvitation(string inviteCode, string developerName)
        {
            var item = _developerRoles.FirstOrDefault(x => string.Compare(x.InviteCode, inviteCode, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (item == null) throw new NullReferenceException(string.Format("Cannot find invitation with provided code."));
            item.DeveloperName = developerName;
            item.RoleName = "Administrator";
        }
    }

    public class InitiativeBusiness : IInitiativeBusiness
    {
        private readonly IRepository2 _repository;

        public InitiativeBusiness(IRepository2 repository)
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

    public class ApplicationVersion : IApplicationVersion
    {
        private readonly Fingerprint _id;
        private readonly Guid _applicationId;
        private readonly string _version;
        private readonly List<IIssueType> _issueTypes;
        private readonly DateTime? _buildTime;
        private readonly string _supportToolkitNameVersion;
        private string _responseMesssage;
        private bool _isOfficial;
        private bool _ignore;

        public ApplicationVersion(Fingerprint id, Guid applicationId, string version, IEnumerable<IIssueType> issueTypes, string responseMesssage, bool isOfficial, bool ignore, string supportToolkitNameVersion, DateTime? buildTime)
        {
            _id = id;
            _applicationId = applicationId;
            _version = version;
            _issueTypes = issueTypes.ToList();
            _responseMesssage = responseMesssage;
            _isOfficial = isOfficial;
            _ignore = ignore;
            _supportToolkitNameVersion = supportToolkitNameVersion;
            _buildTime = buildTime;
        }

        public string Id { get { return _id; } }
        public Guid ApplicationId { get { return _applicationId; } }
        public string Version { get { return _version; } }
        public IEnumerable<IIssueType> IssueTypes { get { return _issueTypes; } }
        public string ResponseMessage { get { return _responseMesssage; } set { _responseMesssage = value == string.Empty ? null : value; } }
        public bool IsOfficial { get { return _isOfficial; } set { _isOfficial = value; } }

        public bool Ignore
        {
            get { return _ignore; }
            set { _ignore = value; }
        }

        public string SupportToolkitNameVersion { get { return _supportToolkitNameVersion; } }
        public DateTime? BuildTime { get { return _buildTime; } }

        public void Add(IIssueType issueType)
        {
            _issueTypes.Add(issueType);
        }
    }

    public class FingerprintException : Exception
    {
        public FingerprintException(string message)
            : base(message)
        {
        }
    }

    public class Fingerprint : IFingerprint
    {
        private readonly string _value;

        private Fingerprint(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new FingerprintException("No value for fingerprint provided. A globally unique identifier should be provided, perhaps a hash from unique data.");
            _value = value;
        }

        public static implicit operator string(Fingerprint item)
        {
            return item._value;
        }

        public static implicit operator Fingerprint(string item)
        {
            return new Fingerprint(item);
        }
    }

    public class ApplicationVersionBusiness : IApplicationVersionBusiness
    {
        private readonly IRepository2 _repository;

        public ApplicationVersionBusiness(IRepository2 repository)
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