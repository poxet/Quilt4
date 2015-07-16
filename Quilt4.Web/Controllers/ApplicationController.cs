using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly IMachineBusiness _machineBusiness;
        private readonly IIssueBusiness _issueBusiness;
        private readonly ISessionBusiness _sessionBusiness;

        public ApplicationController(IInitiativeBusiness initiativeBusiness, IApplicationVersionBusiness applicationVersionBusiness, IMachineBusiness machineBusiness, IIssueBusiness issueBusiness, ISessionBusiness sessionBusiness) 
        {
            _initiativeBusiness = initiativeBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
            _machineBusiness = machineBusiness;
            _issueBusiness = issueBusiness;
            _sessionBusiness = sessionBusiness;
        }

        public ApplicationViewModel GetApplicationModel(IInitiative initiative, string application, IEnumerable<IApplicationVersion> versions)
        {
            var developerName = User.Identity.Name;
            var initiativeHeads = _initiativeBusiness.GetInitiativesByDeveloperOwner(developerName).ToArray();

            var initiativeUniqueIdentifier = initiative.GetUniqueIdentifier(initiativeHeads.Select(xx => xx.Name));

            var applicationVersions = versions as IApplicationVersion[] ?? versions.ToArray();

            var model = new ApplicationViewModel
            {
                InitiativeId = initiative.Id,
                InitiativeName = initiative.Name,
                InitiativeUniqueIdentifier = initiativeUniqueIdentifier,
                Application = application,
                
                
                Versions = applicationVersions.Select(x => new VersionViewModel
                {
                    Checked = false,
                    Build = x.BuildTime == null ? string.Empty : x.BuildTime.Value.ToLocalTime().ToDateTimeString(),
                    VersionId = x.Id,
                    Version = x.Version,
                    VersionIdentifier = x.GetUniqueIdentifier(applicationVersions.Select(y => y.Version)),
                    ApplicationIdentifier = application,
                    InitiativeIdentifier = initiativeUniqueIdentifier,
                    IssueTypeCount = x.IssueTypes.Count(),
                    IssueCount = x.IssueTypes.SelectMany(y => y.Issues).Count(),
                    MachineCount = -1,
                    SessionCount = -1, //TODO: Ta bort denna property och ladda med jquery.
                    FirstSessionTime = new DateTime(), //TODO: Ta bort denna property och ladda med jquery.
                    LastSessionTime = new DateTime(), //TODO: Ta bort denna property och ladda med jquery.
                    //Environment = _sessionBusiness.GetSessionsForApplicationVersion(x.Id).Select(y => y.Environment).Distinct(), TODO: Laddar superlångsamt
                    Environment = null,
                }).OrderByDescending(y => y.Version).ToList(),
            };
            var environments = _initiativeBusiness.GetEnvironmentColors(User.Identity.GetUserId()).First();

            model.Environments = environments.Select(x => new EnvironmentViewModel() { Name = x.Key, Colour = x.Value}).ToList();

            return model;
        }

        // GET: Application/Details/5
        public ActionResult Details(string id, string application)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var versions = _applicationVersionBusiness.GetApplicationVersions(app.Id).ToArray();
            var model = GetApplicationModel(initiative, application, versions);

            @ViewBag.IsArchive = false;
            @ViewBag.Title = "Application Details";
            return View(model);
        }

        public JsonResult Sessions(string id, string application)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var sessions = _sessionBusiness.GetSessionsForApplications(new List<Guid> { app.Id });

            var versions = _applicationVersionBusiness.GetApplicationVersions(app.Id).ToArray();

            //TODO: Här skall data som first, last och en lista med environments och dess färger med.
            var ss = versions.Select(x => new
            {
                Id = x.Id,
                SessionCount = sessions.Count(y => y.ApplicationVersionId == x.Id)
            }).ToArray();

            var response = Json(ss, JsonRequestBehavior.AllowGet);
            return response;
        }

        public JsonResult Machines(string id, string application)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var versions = _applicationVersionBusiness.GetApplicationVersions(app.Id).ToArray();
            var machines = new List<IMachine>();
            foreach (var version in versions)
            {
                machines.AddRange(_machineBusiness.GetMachinesByApplicationVersion(version.Id));
            }
            //var sessions = _machineBusiness.GetMachinesByApplicationVersions()

            //var versions = _applicationVersionBusiness.GetApplicationVersions(app.Id).ToArray();

            //TODO: Här skall data som first, last och en lista med environments och dess färger med.
            
            var ms = versions.Select(x => new

            {
                MachineCount = machines.Count()
            }).ToArray();

            var response = Json(ms, JsonRequestBehavior.AllowGet);
            return response;
        }

        // GET: Application/Archive/5
        public ActionResult Archive(string id, string application)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var archivedVersions = _applicationVersionBusiness.GetArchivedApplicationVersions(app.Id).ToArray();
            var model = GetApplicationModel(initiative, application, archivedVersions);

            @ViewBag.IsArchive = true;
            @ViewBag.Title = "Application Archive";
            return View("Details", model);
        }

        [HttpPost]
        public ActionResult Confirm(ApplicationViewModel model, FormCollection collection)
        {
            switch (collection["submit"])
            {
                case "Delete":
                    var checkedVersions = model.Versions.Where(x => x.Checked).ToList();
                    return View("ConfirmDeleteVersions", checkedVersions);

                case "Archive":
                    checkedVersions = model.Versions.Where(x => x.Checked).ToList();
                    return View("ConfirmArchiveVersions", checkedVersions);
                
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Submit button has an invalid value, {0}.", collection["submit"]));
            }
        }

        [HttpPost]
        public ActionResult ArchiveVersions(List<VersionViewModel> model)
        {
            foreach (var version in model)
            {
                _initiativeBusiness.ArchiveApplicationVersion(version.VersionId);
            }

            return RedirectToAction("Details", new { id = model.First().InitiativeIdentifier, application = model.First().ApplicationIdentifier });
        }

        [HttpPost]
        public ActionResult DeleteVersions(List<VersionViewModel> model)
        {
            foreach (var version in model)
            {
                _initiativeBusiness.DeleteApplicationVersion(version.VersionId);
            }

            return RedirectToAction("Details", new { id = model.First().InitiativeIdentifier, application = model.First().ApplicationIdentifier });
        }

        // GET: Application/Edit/5
        public ActionResult Edit(string id, string application)
        {
            if (id == null) throw new ArgumentNullException("id", "InitiativeId was not provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);

            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var applicationGroup = initiative.ApplicationGroups.Single(x => x.Applications.Any(y => y.Name == application)).Name;
            
            //var environments = _sessionBusiness.GetSessionsForApplications(new List<Guid>() { app.Id }).GroupBy(x => x.Environment).Select(x => x.First()).Select(x => x.Environment).ToArray();

            var model = new ApplicationPropetiesModel
            {
                ApplicationGroupName = applicationGroup,
                TicketPrefix = app.TicketPrefix,
                InitiativeId = initiative.Id.ToString(),
                ApplicationName = application,
            };

            return View(model);
        }

        // POST: Application/Edit/5
        [HttpPost]
        public ActionResult Edit(ApplicationPropetiesModel model, FormCollection collection)
        {
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), model.InitiativeId);
            var applicationGroup = initiative.ApplicationGroups.Single(x => x.Applications.Any(y => y.Name == model.ApplicationName));
            var application = applicationGroup.Applications.Single(x => x.Name == model.ApplicationName);
            application.TicketPrefix = model.TicketPrefix;

            if(initiative.ApplicationGroups.Any(x => x.Name == model.ApplicationGroupName))
            {
                if (initiative.ApplicationGroups.Single(x => x.Name == model.ApplicationGroupName).Applications.All(x => x.Name != model.ApplicationName))
                {
                    initiative.ApplicationGroups.Single(x => x.Name == model.ApplicationGroupName).Add(application);
                }
                else
                {
                    _initiativeBusiness.UpdateInitiative(initiative);
                    return RedirectToAction("Details", "Application", new { id = model.InitiativeId, application = model.ApplicationName });
                }
            }
            else
            {
                initiative.AddApplicationGroup(new ApplicationGroup(model.ApplicationGroupName, new List<IApplication>{application}));
            }

            applicationGroup.Remove(application);
            _initiativeBusiness.UpdateInitiative(initiative);

            return RedirectToAction("Details", "Application", new { id = model.InitiativeId, application = model.ApplicationName });
        }
    }
}
