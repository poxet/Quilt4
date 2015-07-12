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

        public ApplicationViewModel GetApplicationModel(IInitiative initiative, string application, bool showArchivedVersions)
        {
            ////var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId);
            ////var initiative = _initiativeBusiness.GetInitiative(initiativeId);
            var developerName = User.Identity.Name;
            var initiativeHeads = _initiativeBusiness.GetInitiativesByDeveloperOwner(developerName).ToArray();

            //var initiatives = initiativeHeads.Select(head => _initiativeBusiness.GetInitiative(head.Id)).ToArray();

            //var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            //var applicationId = app.Id;
            //var versions = _applicationVersionBusiness.GetApplicationVersions(applicationId).ToArray();
            //var archivedVersions = _applicationVersionBusiness.GetArchivedApplicationVersions(applicationId).ToArray();
            //var versionNames = versions.Select(x => x.Version);

            //var sessions = _sessionBusiness.GetSessionsForApplications(new List<Guid> { applicationId }).ToArray();

            ////var machines = _machineBusiness.GetMachinesByApplicationVersions(versionIds);

            var model = new ApplicationViewModel
            {
                InitiativeId = initiative.Id,
                InitiativeName = initiative.Name,
                InitiativeUniqueIdentifier = initiative.GetUniqueIdentifier(initiativeHeads.Select(xx => xx.Name)),
                Application = application,
                
            //    Versions = versions.Select(x => new VersionViewModel
            //    {
            //        Version = x.Version,
            //        VersionId = x.Id,
            //        Build = x.BuildTime.ToString(),
            //        IssueTypes = x.IssueTypes,
            //        UniqueIdentifier = x.GetUniqueIdentifier(versionNames),
            //        InitiativeIdentifier = initiativeId,
            //        ApplicationIdentifier = application,

            //        //TODO: This is sloooooow ... fix this
            //        //Machines = _machineBusiness.GetMachinesByApplicationVersion(x.Id),
            //        //Machines = machines.Where(z => sessions.Any(y => y.ApplicationVersionId == x.Id && y.MachineFingerprint == z.Id)),

            //        Sessions = sessions.Where(y => y.ApplicationVersionId == x.Id),
            //    }).OrderByDescending(y => y.Version).ToList(),
            };

            //var apps = new List<IApplication>();
            //if (app.DevColor.IsNullOrEmpty() || app.CiColor.IsNullOrEmpty() || app.ProdColor.IsNullOrEmpty())
            //{
            //    foreach (var i in initiatives)
            //    {
            //        foreach (var a in i.ApplicationGroups.SelectMany(x => x.Applications))
            //        {
            //            apps.Add(a);
            //        }
            //    }

            //    if (!app.DevColor.IsNullOrEmpty())
            //    {
            //        model.DevColor = app.DevColor;
            //    }
            //    else
            //    {
            //        var devColors = apps.Select(x => x.DevColor);
            //        foreach (var devColor in devColors)
            //        {
            //            if (!devColor.IsNullOrEmpty())
            //            {
            //                app.DevColor = devColor;
            //                model.DevColor = devColor;
            //                break;
            //            }
            //        }
            //        if (model.DevColor.IsNullOrEmpty())
            //        {
            //            app.DevColor = "#00297A";
            //            model.DevColor = "#00297A";
            //        }
            //    }

            //    if (!app.CiColor.IsNullOrEmpty())
            //    {
            //        model.CiColor = app.CiColor;
            //    }
            //    else
            //    {
            //        var ciColors = apps.Select(x => x.CiColor);
            //        foreach (var ciColor in ciColors)
            //        {
            //            if (!ciColor.IsNullOrEmpty())
            //            {
            //                app.CiColor = ciColor;
            //                model.CiColor = ciColor;
            //                break;
            //            }
            //        }
            //        if (model.CiColor.IsNullOrEmpty())
            //        {
            //            app.CiColor = "#1947A3";
            //            model.CiColor = "#1947A3";
            //        }
            //    }

            //    if (!app.ProdColor.IsNullOrEmpty())
            //    {
            //        model.ProdColor = app.ProdColor;
            //    }
            //    else
            //    {
            //        var prodColors = apps.Select(x => x.ProdColor);
            //        foreach (var prodColor in prodColors)
            //        {
            //            if (!prodColor.IsNullOrEmpty())
            //            {
            //                app.ProdColor = prodColor;
            //                model.ProdColor = prodColor;
            //                break;
            //            }
            //        }
            //        if (model.ProdColor.IsNullOrEmpty())
            //        {
            //            app.ProdColor = "#8099CC";
            //            model.ProdColor = "#8099CC";
            //        }
            //    }
            //    _initiativeBusiness.UpdateInitiative(initiative);
            //}
            //else
            //{
            //    model.DevColor = app.DevColor;
            //    model.CiColor = app.CiColor;
            //    model.ProdColor = app.ProdColor;
            //}
            
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

            var model = GetApplicationModel(initiative, application, false);

            return View(model);
        }

        [HttpPost]
        public ActionResult Details(ApplicationViewModel model, FormCollection collection)
        {
            switch (collection["submit"])
            {
                //case "Delete Versions" :
                //    var checkedVersions = model.Versions.Where(x => x.Checked).ToList();
                //    return View("ConfirmDeleteVersions", checkedVersions);
                    
                //case "Archive Versions" :
                //    checkedVersions = model.Versions.Where(x => x.Checked).ToList();
                //    return View("ConfirmArchiveVersions", checkedVersions);

                case "Show Archived Versions":
                    var initiative = _initiativeBusiness.GetInitiative(model.InitiativeId);
                    var newModel = GetApplicationModel(initiative, model.Application, true);
                    return View(newModel);
                    
                default :
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
