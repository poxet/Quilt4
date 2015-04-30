using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Script.Serialization;
using Microsoft.Owin.Security.Provider;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.Agents;
using Quilt4.Web.BusinessEntities;
using Tharga.Quilt4Net;
using Tharga.Quilt4Net.DataTransfer;
using Tharga.Quilt4Net.Web;
using IssueType = Tharga.Quilt4Net.DataTransfer.IssueType;

namespace Quilt4.Web.Controllers.WebAPI
{
    public static class Converters
    {
        public static Quilt4.BusinessEntities.Session ToSession(this Tharga.Quilt4Net.DataTransfer.Session item, Fingerprint applicationVersionId, Guid applicationId, DateTime serverStartTime, DateTime? serverEndTime, DateTime? serverLastKnown, string callerIp)
        {
            var machineId = (Fingerprint)item.Machine.Fingerprint;
            var userId = (Fingerprint)item.User.Fingerprint;
            return new Quilt4.BusinessEntities.Session(item.SessionGuid, applicationVersionId, item.Environment, applicationId, machineId, userId, item.ClientStartTime, serverStartTime, serverEndTime, serverLastKnown, callerIp);
        }
    }

    public class StatusResponse
    {
        public string X { get; set; }
    }

    public class StatusController : ApiController
    {
        // POST api/status
        [HttpGet]
        [AllowAnonymous]
        public StatusResponse Get()
        {
            //TODO: Return something meaningful
            return new StatusResponse { X = "A" };
        }
    }

    public class IssueController : ApiController
    {
        private readonly IIssueBusiness _issueBusiness;
        private readonly IMembershipAgent _membershipAgent;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IMachineBusiness _machineBusiness;
        private readonly ISettingsBusiness _settingsBusiness;

        public IssueController(IIssueBusiness issueBusiness, IMembershipAgent membershipAgent, IApplicationVersionBusiness applicationVersionBusiness, IInitiativeBusiness initiativeBusiness, ISessionBusiness sessionBusiness, IUserBusiness userBusiness, IMachineBusiness machineBusiness, ISettingsBusiness settingsBusiness)
        {
            _issueBusiness = issueBusiness;
            _membershipAgent = membershipAgent;
            _applicationVersionBusiness = applicationVersionBusiness;
            _initiativeBusiness = initiativeBusiness;
            _sessionBusiness = sessionBusiness;
            _userBusiness = userBusiness;
            _machineBusiness = machineBusiness;
            _settingsBusiness = settingsBusiness;
        }

        // POST api/issue/register
        [HttpPost]
        [ActionName("register")]
        [AllowAnonymous]
        public RegisterIssueResponse RegisterIssue([FromBody] object request)
        {
            //TODO: Move this logics to the business class
            if (request == null)
                throw new ArgumentNullException("request", "No request object provided.");

            try
            {
                var reqiestString = request.ToString();

                var serializer = new JavaScriptSerializer();
                RegisterIssueRequest data = null;
                try
                {
                    data = serializer.Deserialize<RegisterIssueRequest>(reqiestString);
                }
                catch (Exception exception)
                {
                    exception.AddData("Request", request.ToString());
                    _issueBusiness.RegisterIssue(exception, IssueLevel.Warning); //TODO: Refactor: _compositeRoot.LogAgent.RegisterIssue(exception, IssueLevel.Warning);

                    var dataD = serializer.DeserializeObject(reqiestString) as dynamic;
                    data = DynamicExtensions.ToRegisterIssueRequest(dataD);
                }

                return RegisterIssueEx(data);
            }
            catch (Exception exception)
            {
                exception.AddData("Request", request.ToString());
                var response = _issueBusiness.RegisterIssue(exception, IssueLevel.Warning); //TODO: Refactor: _compositeRoot.LogAgent.RegisterIssue(exception, IssueLevel.Warning);
                exception.AddData("IssueTypeTicket", response.IssueTypeTicket);
                exception.AddData("IssueInstanceTicket", response.IssueInstanceTicket);
                exception.AddData("ResponseMessage", response.ResponseMessage);
                throw;
            }
        }

        private RegisterIssueResponse RegisterIssueEx(RegisterIssueRequest request)
        {
            if (request == null) throw new ArgumentNullException("request", "No request object provided.");
            if (request.Session == null) throw new ArgumentException("No session object in request was provided. Need object '{ \"Session\":{...} }' in root.");
            if (request.Session.SessionGuid == Guid.Empty) throw new ArgumentException("No valid session guid provided.");
            if (string.IsNullOrEmpty(request.Session.ClientToken)) throw new ArgumentException("No ClientToken provided.");
            if (request.IssueType == null) throw new ArgumentException("No IssueType object in request was provided. Need object '{ \"IssueType\":{...} }' in root.");
            if (string.IsNullOrEmpty(request.IssueType.Message)) throw new ArgumentException("No message in issue type provided.");
            if (string.IsNullOrEmpty(request.IssueType.IssueLevel)) throw new ArgumentException("No issue level in issue type provided.");

            var callerIp = _membershipAgent.GetUserHostAddress();

            ISession session = null;
            if (request.Session.Application == null)
            {
                session = _issueBusiness.GetSession(request.Session.SessionGuid);
                if (session == null)
                {
                    throw new ArgumentException("Cannot find session with provided session id.").AddData("SessionGuid", request.Session.SessionGuid);
                }
            }

            var ad = GetApplicationData(request, session);

            //var avb = new ApplicationVersionBusiness(_compositeRoot.Repository);
            var avb = _applicationVersionBusiness;
            var fingerprint = avb.AssureApplicationFingerprint(ad.Fingerprint, ad.Version, ad.SupportToolkitNameVersion, ad.BuildTime, ad.Name, request.Session.ClientToken);

            IApplication application;
            try
            {
                //var ib = new InitiativeBusiness(_compositeRoot.Repository);
                var ib = _initiativeBusiness;
                application = ib.RegisterApplication((ClientToken)request.Session.ClientToken, ad.Name, ad.Fingerprint);
            }
            catch (FingerprintException)
            {
                throw new ArgumentException("No value for application fingerprint provided. A globally unique identifier should be provided, perhaps a machine sid or a hash of unique data that does not change.");
            }

            var applicationVersion = avb.RegisterApplicationVersion(fingerprint, application.Id, ad.Version, ad.SupportToolkitNameVersion, ad.BuildTime);

            if (applicationVersion.Ignore)
            {
                return new RegisterIssueResponse
                       {
                           IssueTypeTicket = null,
                           IssueInstanceTicket = null,
                           ResponseMessage = applicationVersion.ResponseMessage,
                           IsOfficial = applicationVersion.IsOfficial,
                       };
            }

            //var sb = new SessionBusiness(_compositeRoot.Repository);
            var sb = _sessionBusiness;
            if (session == null)
            {
                if (request.Session.User.Fingerprint == null) throw new ArgumentException("No user fingerprint provided.");
                session = request.Session.ToSession(applicationVersion.Id, application.Id, DateTime.UtcNow, null, null, callerIp);
            }
            sb.RegisterSession(session);

            var ud = GetUserData(request, session);

            //var ub = new UserBusiness(_compositeRoot.Repository);
            var ub = _userBusiness;
            ub.RegisterUser((Fingerprint)session.UserFingerprint, ud.UserName);

            var md = GetMachineData(request, session);

            //var mb = new MachineBusiness(_compositeRoot.Repository);
            var mb = _machineBusiness;
            mb.RegisterMachine((Fingerprint)md.Fingerprint, md.Name, md.Data);

            int issueTypeTicket;
            int issueTicket;
            string issueTypeResponseMessage;

            var mutex = new Mutex(false, application.Id.ToString());
            try
            {
                mutex.WaitOne();

                var applicationVersions = avb.GetApplicationVersions(application.Id).ToArray();

                var issueType = applicationVersion.IssueTypes.FirstOrDefault(x => request.IssueType.AreEqual(x));
                if (issueType == null)
                {
                    var issueTypes = applicationVersions.SelectMany(x => x.IssueTypes).ToArray();
                    var lastIssueTypeTicket = issueTypes.Any() ? issueTypes.Max(x => x.Ticket) : 0;
                    issueTypeTicket = lastIssueTypeTicket + 1;
                    var inner = ToInnerIssueType(request.IssueType.Inner);

                    issueType = new Quilt4.BusinessEntities.IssueType(request.IssueType.ExceptionTypeName, request.IssueType.Message, request.IssueType.StackTrace ?? string.Empty, request.IssueType.IssueLevel.ToIssueLevel(), inner, new List<IIssue>(), issueTypeTicket, null);
                    applicationVersion.Add(issueType);
                }
                else
                {
                    issueTypeTicket = issueType.Ticket;
                }

                issueTypeResponseMessage = issueType.ResponseMessage;

                var issues = applicationVersions.SelectMany(x => x.IssueTypes).SelectMany(x => x.Issues).ToArray();
                var lastIssueTicket = issues.Any() ? issues.Max(x => x.Ticket) : 0;
                issueTicket = lastIssueTicket + 1;

                var issue = new Quilt4.BusinessEntities.Issue(request.Id, request.ClientTime, DateTime.UtcNow, request.VisibleToUser, request.Data, request.IssueThreadGuid, request.UserHandle, request.UserInput, issueTicket, session.Id);
                issueType.Add(issue);

                //_compositeRoot.Repository.UpdateApplicationVersion(applicationVersion);
                _issueBusiness.UpdateApplicationVersion(applicationVersion);
            }
            finally
            {
                mutex.ReleaseMutex();
            }

            var response = new RegisterIssueResponse
                           {
                               //IssueTypeTicket = application.TicketPrefix + Settings.IssueTypeTicketPrefix + issueTypeTicket,
                               IssueTypeTicket = application.TicketPrefix + _settingsBusiness.GetSetting<string>("IssueTypeTicketPrefix") + issueTypeTicket,
                               //IssueInstanceTicket = application.TicketPrefix + Settings.IssueTicketPrefix + issueTicket,
                               IssueInstanceTicket = application.TicketPrefix + _settingsBusiness.GetSetting<string>("IssueTicketPrefix") + issueTicket,
                               ResponseMessage = applicationVersion.ResponseMessage ?? issueTypeResponseMessage,
                               IsOfficial = applicationVersion.IsOfficial,
                           };
            return response;
        }

        private ApplicationData GetApplicationData(RegisterIssueRequest request, ISession session)
        {
            ApplicationData ad;
            if (request.Session.Application == null)
            {
                //var av = _compositeRoot.Repository.GetApplicationVersion(session.ApplicationVersionId);
                var av = _issueBusiness.GetApplicationVersion(session.ApplicationVersionId);
                //var a = _compositeRoot.Repository.GetInitiativeByApplication(av.ApplicationId).ApplicationGroups.SelectMany(x => x.Applications).First(x => x.Id == av.ApplicationId);
                var a = _issueBusiness.GetInitiativeByApplication(av.ApplicationId).ApplicationGroups.SelectMany(x => x.Applications).First(x => x.Id == av.ApplicationId);

                ad = new ApplicationData
                     {
                         Version = av.Version,
                         Fingerprint = av.Id,
                         BuildTime = av.BuildTime,
                         SupportToolkitNameVersion = av.SupportToolkitNameVersion,
                         Name = a.Name
                     };
            }
            else
            {
                ad = request.Session.Application;
            }
            return ad;
        }

        private MachineData GetMachineData(RegisterIssueRequest request, ISession session)
        {
            MachineData md;
            if (request.Session.Machine == null)
            {
                //var machine = _compositeRoot.Repository.GetMachine(session.MachineFingerprint);
                var machine = _machineBusiness.GetMachine(session.MachineFingerprint);
                md = new MachineData
                     {
                         Fingerprint = machine.Id,
                         Name = machine.Name,
                         Data = machine.Data,
                     };
            }
            else
            {
                md = request.Session.Machine;
            }
            return md;
        }

        private UserData GetUserData(RegisterIssueRequest request, ISession session)
        {
            UserData ud;
            if (request.Session.User == null)
            {
                //var u = _compositeRoot.Repository.GetUser(session.UserFingerprint);
                var u = _userBusiness.GetUser(session.UserFingerprint);
                ud = new UserData
                     {
                         Fingerprint = u.Id,
                         UserName = u.UserName
                     };
            }
            else
            {
                ud = request.Session.User;
            }
            return ud;
        }

        private static IInnerIssueType ToInnerIssueType(IssueType issueType)
        {
            if (issueType == null)
                return null;

            var inner = ToInnerIssueType(issueType.Inner);

            var innerIssueType = new InnerIssueType(issueType.ExceptionTypeName, issueType.Message, issueType.StackTrace ?? string.Empty, issueType.IssueLevel, inner);
            return innerIssueType;
        }
    }
}