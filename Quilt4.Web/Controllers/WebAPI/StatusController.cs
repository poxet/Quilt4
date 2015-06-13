using System.Web.Http;
using Quilt4.Web.DataTransfer;

namespace Quilt4.Web.Controllers.WebAPI
{
    public class StatusController : ApiController
    {
        // POST api/status
        [HttpGet]
        [Route("api/status")]
        [AllowAnonymous]
        public StatusResponse Get()
        {
            //TODO: Return something meaningful
            return new StatusResponse { X = "A" };
        }
    }
}