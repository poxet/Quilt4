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
        public ActionResult Details(string initiativeUniqueIdentifier, string application, string version, string issueType)
        {
            if (initiativeUniqueIdentifier == null) throw new ArgumentNullException("initiativeUniqueIdentifier", "InitiativeId was not provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == initiativeUniqueIdentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == initiativeUniqueIdentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(initiativeUniqueIdentifier)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString());
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).SingleOrDefault(x => x.Name == application);
            if (app == null) throw new NullReferenceException("Cannot find application").AddData("Application", application);
            var applicationVersions = _applicationVersionBusiness.GetApplicationVersions(app.Id).ToArray();
            var ver = applicationVersions.SingleOrDefault(x => x.Version == version);

            var model = new IssueTypeModel
            {
                IssueType = ver.IssueTypes.Single(x => x.Ticket.ToString() == issueType), 
                Sessions = _sessionBusiness.GetSessionsForApplicationVersion(ver.Id),
                Application = application,
                Version = version,
                InitiativeName = initiative.Name,
                InitiativeUniqueIdentifier = initiativeUniqueIdentifier,
                ApplicationName = app.Name,
                VersionUniqueIdentifier = ver.GetUniqueIdentifier(applicationVersions.Select(x => x.Version))
            };
            model.Users = model.Sessions.Select(user => _userBusiness.GetUser(user.UserFingerprint)).ToList();
            
            return View(model);
        }
    }
}