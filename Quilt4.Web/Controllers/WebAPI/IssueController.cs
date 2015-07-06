using System;
using System.Web.Http;
using System.Web.Script.Serialization;
using Quilt4.Interface;
using Tharga.Quilt4Net;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Web.Controllers.WebAPI
{
    public class IssueController : ApiController
    {
        private readonly IIssueBusiness _issueBusiness;

        public IssueController(IIssueBusiness issueBusiness)
        {
            _issueBusiness = issueBusiness;
        }

        // POST api/issue/register
        [HttpPost]
        [Route("api/issue/register")]
        [AllowAnonymous]
        public RegisterIssueResponse RegisterIssue([FromBody] object request)
        {
            if (request == null)
                throw new ArgumentNullException("request", "No request object provided.");
            
            try
            {
                var data = GetData(request);
                return _issueBusiness.RegisterIssue(data);
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

        private RegisterIssueRequest GetData(object request)
        {
            var reqiestString = request.ToString();

            var serializer = new JavaScriptSerializer();
            RegisterIssueRequest data;
            try
            {
                data = serializer.Deserialize<RegisterIssueRequest>(reqiestString);
            }
            catch (Exception exception)
            {
                exception.AddData("Request", request.ToString());
                _issueBusiness.RegisterIssue(exception, IssueLevel.Warning);

                var dataD = serializer.DeserializeObject(reqiestString) as dynamic;
                data = DynamicExtensions.ToRegisterIssueRequest(dataD);
            }

            return data;
        }
    }
}