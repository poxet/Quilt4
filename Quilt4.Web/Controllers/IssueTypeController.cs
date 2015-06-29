using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    [Authorize]
    public class IssueTypeController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IUserBusiness _userBusiness;

        public IssueTypeController(IInitiativeBusiness initiativeBusiness, IApplicationVersionBusiness applicationVersionBusiness, ISessionBusiness sessionBusiness, IUserBusiness userBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
            _sessionBusiness = sessionBusiness;
            _userBusiness = userBusiness;
        }

        // GET: IssueType/Details/5
        public ActionResult Details(string id, string application, string version, string issueType)
        {
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id).ToModel(null);
            var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).SingleOrDefault(x => x.Name == application).Id;
            //TODO: version is an uniqe something ... how should this be solved?
            var ver = _applicationVersionBusiness.GetApplicationVersions(applicationId).SingleOrDefault(x => x.Version == version);
            
            var model = new IssueTypeModel
            {
                IssueType = ver.IssueTypes.Single(x => x.Ticket.ToString() == issueType), 
                Sessions = _sessionBusiness.GetSessionsForApplicationVersion(ver.Id),
                Initiative = id,
                Application = application,
                Version = version,
            };
            model.Users = model.Sessions.Select(user => _userBusiness.GetUser(user.UserFingerprint)).ToList();
            


            //id -> Initiative
            //var initiative = _initiativeBusiness.GetInitiatives().Single(x => x.Name == id);
            //var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application).Id;
            //var ver = _applicationVersionBusiness.GetApplicationVersions(applicationId);
            //var issueTypes = ver.Single(x => x.Version == version).IssueTypes;
            //var model = new IssueTypeModel
            //{
            //    IssueType = issueTypes.Single(x => x.Ticket.ToString() == issueType), 
            //    Sessions = _sessionBusiness.GetSessionsForApplicationVersion(application)
            //};
            return View(model);
        }
    }
}