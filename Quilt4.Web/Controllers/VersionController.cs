using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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
        private readonly IAccountRepository _accountRepository;

        public VersionController(IInitiativeBusiness initiativeBusiness, IApplicationVersionBusiness applicationVersionBusiness, ISessionBusiness sessionBusiness, IUserBusiness userBusiness, IMachineBusiness machineBusiness, IAccountRepository accountRepository)
        {
            _initiativeBusiness = initiativeBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
            _sessionBusiness = sessionBusiness;
            _userBusiness = userBusiness;
            _machineBusiness = machineBusiness;
            _accountRepository = accountRepository;
        }

        // GET: Version/Details/5
        public ActionResult Details(string id, string application, string version)
        {
            var initiative = _initiativeBusiness.GetInitiative(_accountRepository.FindById(User.Identity.GetUserId()).Email, id);
            var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application).Id;
            var versions = _applicationVersionBusiness.GetApplicationVersions(applicationId).ToArray();
            var versionName = _applicationVersionBusiness.GetApplicationVersion(initiative.Id.ToString(), applicationId.ToString(), version).Version;

            //var versionIds = versions.Select(v => v.Id).ToArray();

            //change to EnvironmentViewModel when fixed
            //var environments = new List<string>();
            //foreach (var versionId in versionIds)
            //{
            //    environments.Add(_sessionBusiness.GetSessionsForApplicationVersion(versionId).Select(x => x.Environment));
            //}

            var ver = versions.Single(x => x.Id.Replace(":", "") == version || x.Version == version);

            //var sessions = _sessionBusiness.GetSessionsForApplicationVersion();

            var issue = new IssueViewModel
            {
                InitiativeId = initiative.Id.ToString(),
                InitiativeName = initiative.Name,
                ApplicationName = application,
                Version = version,
                VersionName = versionName,
                IssueTypes = ver.IssueTypes,
                Sessions = _sessionBusiness.GetSessionsForApplicationVersion(ver.Id),
                ApplicationVersionId = applicationId.ToString(),
                InitiativeUniqueIdentifier = id,
            };
            var allEnvironments = _initiativeBusiness.GetEnvironmentColors(User.Identity.GetUserId(), _accountRepository.FindById(User.Identity.GetUserId()).UserName);
            var envs = issue.Sessions.Select(x => x.Environment).Distinct();

            var environments = new List<EnvironmentViewModel>();
            foreach (var environment in envs)
            {
                if (allEnvironments.ContainsKey(environment))
                {
                    string color;
                    allEnvironments.TryGetValue(environment, out color);
                    environments.Add(new EnvironmentViewModel()
                    {
                        Name = environment,
                        Color = color,
                    });
                }
            }

            //issue.Environments = environments.Select(x => new EnvironmentViewModel { Name = x.Key, Color = x.Value}).ToList();
            issue.Environments = environments;
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