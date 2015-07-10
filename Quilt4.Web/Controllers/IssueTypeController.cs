using System;
using System.Collections.Generic;
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

        public ActionResult Thread(string id, string issueThread)
        {
            if (id == null) throw new ArgumentNullException("id", "InitiativeId was not provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == id).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == id).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(id)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(initiativeId);
            var applicationIds = initiative.ApplicationGroups.SelectMany(x => x.Applications).Select(x => x.Id).ToArray();

            var versions = new List<IApplicationVersion>();
            
            foreach (var applicationId in applicationIds)
            {
                versions.AddRange(_applicationVersionBusiness.GetApplicationVersions(applicationId));
            }

            var issues = versions.SelectMany(x => x.IssueTypes).SelectMany(x => x.Issues).Where(x => x.IssueThreadGuid == Guid.Parse(issueThread)).ToArray();
            var sessions = _sessionBusiness.GetSessionsForApplications(applicationIds).ToArray();
            var users = sessions.Select(x => _userBusiness.GetUser(x.UserFingerprint));

            var model = new IssueThreadModel()
            {
                InitiativeUniqueIdentifier = id,
                InitiativeName = initiative.Name,
                Issues = issues,
                Sessions = sessions,
                Users = users,
            };

            return View(model);
        }

        // GET: IssueType/Details/5
        public ActionResult Details(string id, string application, string version, string issueType)
        {
            if (id == null) throw new ArgumentNullException("id", "InitiativeId was not provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == id).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == id).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(id)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString());
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).SingleOrDefault(x => x.Name == application);
            if (app == null) throw new NullReferenceException("Cannot find application").AddData("Application", application);
            var applicationVersions = _applicationVersionBusiness.GetApplicationVersions(app.Id).ToArray();

            var v = applicationVersions.Where(x => x.Version == version);
            var ver = v.Count() == 1 ? applicationVersions.Single(x => x.Version == version) : applicationVersions.Single(x => x.Id.Replace(":", "") == version);

            var model = new IssueTypeModel
            {
                IssueType = ver.IssueTypes.Single(x => x.Ticket.ToString() == issueType), 
                Sessions = _sessionBusiness.GetSessionsForApplicationVersion(ver.Id),
                Application = application,
                Version = ver.Version,
                InitiativeName = initiative.Name,
                InitiativeUniqueIdentifier = id,
                ApplicationName = app.Name,
                VersionUniqueIdentifier = ver.GetUniqueIdentifier(applicationVersions.Select(x => x.Version))
            };
            model.Users = model.Sessions.Select(user => _userBusiness.GetUser(user.UserFingerprint)).ToList();
            
            return View(model);
        }
    }
}