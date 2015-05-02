using System.Web.Http;

namespace Quilt4.Web.Controllers.WebAPI
{
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
}