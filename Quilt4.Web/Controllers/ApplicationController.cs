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

            var model = new ApplicationViewModel
            {
                InitiativeId = initiative.Id,
                InitiativeName = initiative.Name,
                InitiativeUniqueIdentifier = initiativeUniqueIdentifier,
                Application = application,
                
                Versions = versions.Select(x => new VersionViewModel
                {
                    Checked = false,
                    Build = x.BuildTime == null ? string.Empty : x.BuildTime.Value.ToLocalTime().ToDateTimeString(),
                    VersionId = x.Id,
                    Version = x.Version,
                    VersionIdentifier = x.GetUniqueIdentifier(versions.Select(y => y.Version)),
                    ApplicationIdentifier = application,
                    InitiativeIdentifier = initiativeUniqueIdentifier,
                    MachineCount = -1, //TODO: Load data (If slow, populate data when the list has already loaded)
                    SessionCount = -1, //TODO: Load data (If slow, populate data when the list has already loaded)
                    IssueTypeCount = -1, //TODO: Load data (If slow, populate data when the list has already loaded)
                    IssueCount = -1, //TODO: Load data (If slow, populate data when the list has already loaded)
                    FirstSessionTime = new DateTime(), //TODO: Load data (If slow, populate data when the list has already loaded)
                    LastSessionTime = new DateTime(), //TODO: Load data (If slow, populate data when the list has already loaded)
                    Environments = new List<EnvironmentViewModel>
                    {
                        new EnvironmentViewModel { Name = "A", Colour = "faf567" }, 
                        new EnvironmentViewModel { Name = "B", Colour = "1a65f7" }
                    }, //TODO: Load data (If slow, populate data when the list has already loaded)
                }).OrderByDescending(y => y.Version).ToList(),
            };

            //if (showArchivedVersions)
            //{
            //    model.ShowArchivedVersions = true;
            //    model.ArchivedVersions = archivedVersions.Select(x => new VersionViewModel
            //    {
            //        Version = x.Version,
            //        VersionId = x.Id,
            //        Build = x.BuildTime.ToString(),
            //        IssueTypes = x.IssueTypes,

            //        //TODO: This is sloooooow ... fix this
            //        //Machines = _machineBusiness.GetMachinesByApplicationVersion(x.Id),
            //        //Machines = machines.Where(z => sessions.Any(y => y.ApplicationVersionId == x.Id && y.MachineFingerprint == z.Id)),

            //        Sessions = sessions.Where(y => y.ApplicationVersionId == x.Id).ToArray(),
            //    }).OrderByDescending(y => y.Version).ToList();
            //}

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
            return View(model);
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
