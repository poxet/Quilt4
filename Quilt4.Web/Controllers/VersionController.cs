using System;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    [Authorize]
    public class VersionController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IMachineBusiness _machineBusiness;

        public VersionController(IInitiativeBusiness initiativeBusiness, IApplicationVersionBusiness applicationVersionBusiness, ISessionBusiness sessionBusiness, IUserBusiness userBusiness, IMachineBusiness machineBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
            _sessionBusiness = sessionBusiness;
            _userBusiness = userBusiness;
            _machineBusiness = machineBusiness;
        }

        // GET: Version/Details/5
        public ActionResult Details(string id, string application, string version)
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
            var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application).Id;
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).SingleOrDefault(x => x.Id == applicationId);
            var versions = _applicationVersionBusiness.GetApplicationVersions(applicationId);
            var versionName = _applicationVersionBusiness.GetApplicationVersion(initiativeId.ToString(), applicationId.ToString(), version).Version;

            var ver = versions.Single(x => x.Id.Replace(":", "") == version || x.Version == version);

            var issue = new IssueViewModel
            {
                InitiativeId = initiativeId.ToString(),
                InitiativeName = initiative.Name,
                ApplicationName = application,
                Version = version,
                VersionName = versionName,
                IssueTypes = ver.IssueTypes,
                Sessions = _sessionBusiness.GetSessionsForApplicationVersion(ver.Id),
                ApplicationVersionId = applicationId.ToString(),
                //TODO: Add applicationversion id
                InitiativeUniqueIdentifier = id,
            };

            //TODO: fetch version anmes
            issue.UniqueIdentifier = issue.GetUniqueIdentifier(versionName);

            //issue.ExceptionTypeName = ver.IssueTypes.Select(x => x.ExceptionTypeName);
            //issue.Message = ver.IssueTypes.Select(x => x.Message);
            //issue.Level = ver.IssueTypes.Select(x => x.IssueLevel.ToString());
            //issue.Count = ver.IssueTypes.Select(x => x.Count.ToString());
            //issue.Ticket = ver.IssueTypes.Select(x => x.Ticket.ToString());


            var users = issue.Sessions.Select(user => _userBusiness.GetUser(user.UserFingerprint)).ToList().GroupBy(x => x.Id).Select(x => x.First());

            issue.Users = users;

            issue.Machines = _machineBusiness.GetMachinesByApplicationVersion(ver.Id);
    
            return View(issue);
        }
    }
}