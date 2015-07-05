using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;
using Quilt4.Web.Models;
using Tharga.Quilt4Net;

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
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).SingleOrDefault(x => x.Name == application);
            if (app == null) throw new NullReferenceException("Cannot find application").AddData("Application", application);
            var applicationVersions = _applicationVersionBusiness.GetApplicationVersions(app.Id).ToArray();
            var ver = applicationVersions.SingleOrDefault(x => x.Version == version);
            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloper(developerName).ToArray();

            var model = new IssueTypeModel
            {
                IssueType = ver.IssueTypes.Single(x => x.Ticket.ToString() == issueType), 
                Sessions = _sessionBusiness.GetSessionsForApplicationVersion(ver.Id),
                Initiative = id,
                Application = application,
                Version = version,
                InitiativeName = initiative.Name,
                InitiativeUniqueIdentifier = initiative.GetUniqueIdentifier(ins.Select(xx => xx.Name)),
                ApplicationName = app.Name,
                VersionUniqueIdentifier = ver.GetUniqueIdentifier(applicationVersions.Select(x => x.Version))
            };
            model.Users = model.Sessions.Select(user => _userBusiness.GetUser(user.UserFingerprint)).ToList();
            
            return View(model);
        }
    }
}