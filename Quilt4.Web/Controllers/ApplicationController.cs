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

        public ApplicationModel GenerateApplicationModel(string id, string application, bool showArchivedVersions)
        {
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);
            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloperOwner(developerName).ToArray();

            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var applicationId = app.Id;
            var versions = _applicationVersionBusiness.GetApplicationVersions(applicationId).ToArray();
            var archivedVersions = _applicationVersionBusiness.GetArchivedApplicationVersions(applicationId).ToArray();
            var versionNames = versions.Select(x => x.Version);

            var sessions = _sessionBusiness.GetSessionsForApplications(new List<Guid> { applicationId }).ToArray();

            //var machines = _machineBusiness.GetMachinesByApplicationVersions(versionIds);

            var model = new ApplicationModel
            {
                Initiative = id,
                InitiativeName = initiative.Name,
                InitiativeUniqueIdentifier = initiative.GetUniqueIdentifier(ins.Select(xx => xx.Name)), //initiative.UniqueIdentifier,
                Application = application,
                DevColor = app.DevColor,
                CiColor = app.CiColor,
                ProdColor = app.ProdColor,

                Versions = versions.Select(x => new VersionViewModel
                {
                    Version = x.Version,
                    VersionId = x.Id,
                    Build = x.BuildTime.ToString(),
                    IssueTypes = x.IssueTypes,
                    UniqueIdentifier = x.GetUniqueIdentifier(versionNames),
                    InitiativeIdentifier = id,
                    ApplicationIdentifier = application,

                    //TODO: This is sloooooow ... fix this
                    //Machines = _machineBusiness.GetMachinesByApplicationVersion(x.Id),
                    //Machines = machines.Where(z => sessions.Any(y => y.ApplicationVersionId == x.Id && y.MachineFingerprint == z.Id)),

                    Sessions = sessions.Where(y => y.ApplicationVersionId == x.Id),
                }).OrderByDescending(y => y.Version).ToList(),
            };

            if (showArchivedVersions)
            {
                model.ShowArchivedVersions = true;
                model.ArchivedVersions = archivedVersions.Select(x => new VersionViewModel
                {
                    Version = x.Version,
                    VersionId = x.Id,
                    Build = x.BuildTime.ToString(),
                    IssueTypes = x.IssueTypes,

                    //TODO: This is sloooooow ... fix this
                    //Machines = _machineBusiness.GetMachinesByApplicationVersion(x.Id),
                    //Machines = machines.Where(z => sessions.Any(y => y.ApplicationVersionId == x.Id && y.MachineFingerprint == z.Id)),

                    Sessions = sessions.Where(y => y.ApplicationVersionId == x.Id).ToArray(),
                }).OrderByDescending(y => y.Version).ToList();
            }

            return model;
        }

        // GET: Application/Details/5
        public ActionResult Details(string initiativeUniqueIdentifier, string application)
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

            var model = GenerateApplicationModel(initiativeId.ToString(), application, false);
            model.InitiativeUniqueIdentifier = initiativeUniqueIdentifier;

            return View(model);
        }

        [HttpPost]
        public ActionResult Details(ApplicationModel model, FormCollection collection)
        {
            switch (collection["submit"])
            {
                case "Delete Versions" :
                    var checkedVersions = model.Versions.Where(x => x.Checked).ToList();
                    return View("ConfirmDeleteVersions", checkedVersions);
                    
                case "Archive Versions" :
                    checkedVersions = model.Versions.Where(x => x.Checked).ToList();
                    return View("ConfirmArchiveVersions", checkedVersions);

                case "Show Archived Versions":
                    var newModel = GenerateApplicationModel(model.Initiative, model.Application, true);
                    return View(newModel);
                    
                default : 
                    throw new ArgumentException("Submit button has an invalid value");
            }
        }

        [HttpPost]
        public ActionResult ArchiveVersions(List<VersionViewModel> model)
        {
            foreach (var version in model)
            {
                _initiativeBusiness.ArchiveApplicationVersion(version.VersionId);
            }

            return RedirectToAction("Details", new { initiativeUniqueIdentifier = model.First().InitiativeIdentifier, application = model.First().ApplicationIdentifier });
        }

        [HttpPost]
        public ActionResult DeleteVersions(List<VersionViewModel> model)
        {
            foreach (var version in model)
            {
                _initiativeBusiness.DeleteApplicationVersion(version.VersionId);
            }

            return RedirectToAction("Details", new { initiativeUniqueIdentifier = model.First().InitiativeIdentifier, application = model.First().ApplicationIdentifier });
        }

        // GET: Application/Edit/5
        public ActionResult Edit(string initiativeUniqueIdentifier, string application)
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

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString()).ToModel(null);
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var applicationGroup = initiative.ApplicationGroups.Single(x => x.Applications.Any(y => y.Name == application)).Name;
            
            var environments = _sessionBusiness.GetSessionsForApplications(new List<Guid>() { app.Id }).GroupBy(x => x.Environment).Select(x => x.First()).Select(x => x.Environment).ToArray();

            var model = new ApplicationPropetiesModel()
            {
                ApplicationGroupName = applicationGroup,
                TicketPrefix = app.TicketPrefix,
                InitiativeId = initiativeId.ToString(),
                ApplicationName = application,
                DevColor = app.DevColor,
                CiColor = app.CiColor,
                ProdColor = app.ProdColor,
                Environments = environments,
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
            application.DevColor = model.DevColor;
            application.CiColor = model.CiColor;
            application.ProdColor = model.ProdColor;

            if(initiative.ApplicationGroups.Any(x => x.Name == model.ApplicationGroupName))
            {
                if (!initiative.ApplicationGroups.Single(x => x.Name == model.ApplicationGroupName).Applications.Any(x => x.Name == model.ApplicationName))
                {
                    initiative.ApplicationGroups.Single(x => x.Name == model.ApplicationGroupName).Add(application);
                }
                else
                {
                    _initiativeBusiness.UpdateInitiative(initiative);
                    return RedirectToAction("Details", "Application", new { initiativeUniqueIdentifier = model.InitiativeId, application = model.ApplicationName });
                }
            }
            else
            {
                initiative.AddApplicationGroup(new ApplicationGroup(model.ApplicationGroupName, new List<IApplication>{application}));
            }

            applicationGroup.Remove(application);
            _initiativeBusiness.UpdateInitiative(initiative);

            return RedirectToAction("Details", "Application", new { initiativeUniqueIdentifier = model.InitiativeId, application = model.ApplicationName });
        }
    }
}
