using System;
using System.Web.Http;
using System.Web.Script.Serialization;
using Quilt4.Interface;
using Tharga.Quilt4Net;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Web.Controllers.WebAPI
{
    public class SessionController : ApiController
    {
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IIssueBusiness _issueBusiness;

        public SessionController(ISessionBusiness sessionBusiness, IIssueBusiness issueBusiness)
        {
            _sessionBusiness = sessionBusiness;
            _issueBusiness = issueBusiness;
        }

        // GET api/session/register
        [HttpPost]
        [Route("api/session/register")]
        [AllowAnonymous]
        public void RegisterSession([FromBody] object request)
        {
            if (request == null)
                throw new ArgumentNullException("request", "No request object provided.");

            try
            {
                var data = GetData(request);
                _sessionBusiness.RegisterSessionEx(data);
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

        private RegisterSessionRequest GetData(object request)
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
                _issueBusiness.RegisterIssue(exception, IssueLevel.Warning);

                var dataD = serializer.DeserializeObject(reqiestString) as dynamic;
                var sessionD = dataD["Session"];
                data = new RegisterSessionRequest { Session = DynamicExtensions.ToSession(sessionD) };
            }
            return data;
        }

        // GET api/session/end
        [HttpPost]
        [Route("api/session/end")]
        [AllowAnonymous]
        public void EndSession([FromBody]object sessionId)
        {
            _sessionBusiness.EndSession(Guid.Parse(sessionId.ToString()));
        }
    }
}