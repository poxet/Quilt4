using System;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;

namespace Quilt4.Web.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;

        public SessionController(ISessionBusiness sessionBusiness, IApplicationVersionBusiness applicationVersionBusiness)
        {
            _sessionBusiness = sessionBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
        }

        // GET: Session
        public ActionResult Index()
        {
            throw new NotImplementedException("View is missing.");
            //return View();
        }

        public ActionResult Details(string applicationVersionId, string sessionId)
        {
            var session = _sessionBusiness.GetSessionsForApplicationVersion(applicationVersionId).Single(x => x.Id == Guid.Parse(sessionId));
            var version = _applicationVersionBusiness.GetApplicationVersions(session.ApplicationId).Single(x => x.Id == applicationVersionId);
            
            
            return View(version.IssueTypes);
        }
    }
}