using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
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

    public class FingerprintException : Exception
    {
        public FingerprintException(string message)
            : base(message)
        {
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
}
