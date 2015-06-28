using System;
using System.Web.Http;
using System.Web.Script.Serialization;
using Quilt4.Interface;
using Tharga.Quilt4Net;
using Tharga.Quilt4Net.DataTransfer;
using Tharga.Quilt4Net.Web;

namespace Quilt4.Web.Controllers.WebAPI
{
    public class CounterController : ApiController
    {
        private readonly ICounterBusiness _counterBusiness;
        private readonly IIssueBusiness _issueBusiness;

        public CounterController(ICounterBusiness counterBusiness, IIssueBusiness issueBusiness)
        {
            _counterBusiness = counterBusiness;
            _issueBusiness = issueBusiness;
        }

        // POST api/counter/register
        [HttpPost]
        [Route("api/counter/register")]
        [AllowAnonymous]
        public RegisterCounterResponse RegisterCounter([FromBody] object request)
        {
            //TODO: Move this logics to the business class
            if (request == null)
                throw new ArgumentNullException("request", "No request object provided.");

            try
            {
                var data = GetData(request);
                //return _counterBusiness.RegisterCounter(data);
                //return _counterBusiness.Register(data.)
                throw new NotImplementedException();
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

        private RegisterCounterRequest GetData(object request)
        {
            var reqiestString = request.ToString();

            var serializer = new JavaScriptSerializer();
            RegisterCounterRequest data;
            try
            {
                data = serializer.Deserialize<RegisterCounterRequest>(reqiestString);
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