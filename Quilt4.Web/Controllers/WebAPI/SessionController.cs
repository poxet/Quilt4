using System;
using System.Web.Http;
using System.Web.Script.Serialization;
//using Tharga.Quilt4Net.BusinessEntities;
//using Tharga.Quilt4Net.BusinessLogics;
using Tharga.Quilt4Net.DataTransfer;
using Quilt4.Interface;
using Quilt4.Web.Agents;
using Quilt4.Web.Business;
using Quilt4.Web.BusinessEntities;

namespace Tharga.Quilt4Net.Web.Controllers.WebAPI
{
    public class SessionController : ApiController
    {
        private readonly IIssueBusiness _issueBusiness;
        private readonly IMembershipAgent _membershipAgent;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IMachineBusiness _machineBusiness;
        private readonly ISessionBusiness _sessionBusiness;

        public SessionController(IIssueBusiness issueBusiness, IMembershipAgent membershipAgent, IApplicationVersionBusiness applicationVersionBusiness, IInitiativeBusiness initiativeBusiness, IUserBusiness userBusiness, IMachineBusiness machineBusiness, ISessionBusiness sessionBusiness)
        {
            _issueBusiness = issueBusiness;
            _membershipAgent = membershipAgent;
            _applicationVersionBusiness = applicationVersionBusiness;
            _initiativeBusiness = initiativeBusiness;
            _userBusiness = userBusiness;
            _machineBusiness = machineBusiness;
            _sessionBusiness = sessionBusiness;
        }

        //private readonly ICompositeRoot _compositeRoot;

        //public SessionController(ICompositeRoot compositeRoot)
        //{
        //    _compositeRoot = compositeRoot;
        //}

        // GET api/session/register
        [HttpPost]
        [ActionName("register")]
        [AllowAnonymous]
        public void RegisterSession([FromBody] object request)
        {
            if (request == null)
                throw new ArgumentNullException("request", "No request object provided.");

            try
            {
                var reqiestString = request.ToString();

                var serializer = new JavaScriptSerializer();
                RegisterSessionRequest data = null;
                try
                {
                    data = serializer.Deserialize<RegisterSessionRequest>(reqiestString);
                }
                catch (Exception exception)
                {
                    exception.AddData("Request", request.ToString());
                    //_compositeRoot.LogAgent.RegisterIssue(exception, IssueLevel.Warning);
                    _issueBusiness.RegisterIssue(exception, IssueLevel.Warning);

                    var dataD = serializer.DeserializeObject(reqiestString) as dynamic;
                    var sessionD = dataD["Session"];
                    data = new RegisterSessionRequest { Session = DynamicExtensions.ToSession(sessionD) };
                }

                RegisterSessionEx(data);
            }
            catch (Exception exception)
            {
                exception.AddData("Request", request.ToString());
                var response = _issueBusiness.RegisterIssue(exception, IssueLevel.Warning);
                exception.AddData("IssueTypeTicket", response.IssueTypeTicket);
                exception.AddData("IssueInstanceTicket", response.IssueInstanceTicket);
                exception.AddData("ResponseMessage", response.ResponseMessage);
                throw;
            }
        }

        private void RegisterSessionEx(RegisterSessionRequest request)
        {
            if (request == null) throw new ArgumentNullException("request", "No request object provided.");
            if (request.Session == null) throw new ArgumentException("No session object in request was provided. Need object '{ \"Session\":{...} }' in root.");
            if (request.Session.SessionGuid == Guid.Empty) throw new ArgumentException("No valid session guid provided.");
            if (string.IsNullOrEmpty(request.Session.ClientToken)) throw new ArgumentException("No ClientToken provided.");
            if (request.Session.Application == null) throw new ArgumentException("No application object in request was provided. Need object '{ \"Application\":{...} }' in session.");
            if (string.IsNullOrEmpty(request.Session.Application.Name)) throw new ArgumentException("No name provided for application.");
            if (request.Session.User == null) throw new ArgumentException("No user object in request was provided. Need object '{ \"User\":{...} }' in session.");
            if (request.Session.Machine == null) throw new ArgumentException("No machine object in request was provided. Need object '{ \"Machine\":{...} }' in session.");

            var callerIp = _membershipAgent.GetUserHostAddress();

            //var avb = new ApplicationVersionBusiness(_compositeRoot.Repository);
            var avb = _applicationVersionBusiness;
            var fingerprint = avb.AssureApplicationFingerprint(request.Session.Application.Fingerprint, request.Session.Application.Version, request.Session.Application.SupportToolkitNameVersion, request.Session.Application.BuildTime, request.Session.Application.Name, request.Session.ClientToken);

            IApplication application;
            try
            {
                //var ib = new InitiativeBusiness(_compositeRoot.Repository);
                var ib = _initiativeBusiness;
                application = ib.RegisterApplication((ClientToken)request.Session.ClientToken, request.Session.Application.Name, request.Session.Application.Fingerprint);
            }
            catch (FingerprintException)
            {
                throw new ArgumentException("No value for application fingerprint provided. A globally unique identifier should be provided, perhaps a machine sid or a hash of unique data that does not change.");
            }

            var applicationVersion = avb.RegisterApplicationVersion(fingerprint, application.Id, request.Session.Application.Version, request.Session.Application.SupportToolkitNameVersion, request.Session.Application.BuildTime);

            if (applicationVersion.Ignore)
            {
                return;
            }

            try
            {
                //var ub = new UserBusiness(_compositeRoot.Repository);
                var ub = _userBusiness;
                ub.RegisterUser((Fingerprint)request.Session.User.Fingerprint, request.Session.User.UserName);
            }
            catch (FingerprintException)
            {
                throw new ArgumentException("No value for user fingerprint provided. A globally unique identifier should be provided, perhaps a username and domain or a hash of unique data that does not change.");
            }

            try
            {
                //var mb = new MachineBusiness(_compositeRoot.Repository);
                var mb = _machineBusiness;
                mb.RegisterMachine((Fingerprint)request.Session.Machine.Fingerprint, request.Session.Machine.Name, request.Session.Machine.Data);
            }
            catch (FingerprintException)
            {
                throw new ArgumentException("No value for machine fingerprint provided. A globally unique identifier should be provided, perhaps a DeviceId, SID or a hash of unique data that does not change.");
            }

            //var sb = new SessionBusiness(_compositeRoot.Repository);
            var sb = _sessionBusiness;
            sb.RegisterSession(request.Session.ToSession(applicationVersion.Id, application.Id, DateTime.UtcNow, null, null, callerIp));
        }

        // GET api/session/end
        [HttpPost]
        [ActionName("end")]
        [AllowAnonymous]
        public void EndSession([FromBody]Guid sessionId)
        {
            var sb = _sessionBusiness; //new SessionBusiness(_compositeRoot.Repository);            
            sb.EndSession(sessionId);
        }
    }
}