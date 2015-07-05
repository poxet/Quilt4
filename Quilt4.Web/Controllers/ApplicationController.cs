﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Internal;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.Extensions;
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
        // GET: Application/Details/5
        //public ActionResult Details(string id, string application)
        {
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id).ToModel(null);
            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloperOwner(developerName).ToArray();

            var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application).Id;
            var versions = _applicationVersionBusiness.GetApplicationVersions(applicationId).ToArray();
            var archivedVersions = _applicationVersionBusiness.GetArchivedApplicationVersions(applicationId).ToArray();
            var versionNames = versions.Select(x => x.Version);
            //var versionIds = versions.Select(x => x.Id);

            var sessions = _sessionBusiness.GetSessionsForApplications(new List<Guid> { applicationId }).ToArray();

            //var machines = _machineBusiness.GetMachinesByApplicationVersions(versionIds);

            var model = new ApplicationModel
            {
                Initiative = id,
                InitiativeName = initiative.Name,
                InitiativeUniqueIdentifier = initiative.UniqueIdentifier,
                Application = application,

                Versions = versions.Select(x => new VersionModel
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
                model.ArchivedVersions = archivedVersions.Select(x => new VersionModel
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
        public ActionResult Details(string id, string application)
        {
            var model = GenerateApplicationModel(id, application, false);

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
        public ActionResult ArchiveVersions(List<VersionModel> model)
        {
            foreach (var version in model)
            {
                _initiativeBusiness.ArchiveApplicationVersion(version.VersionId);
            }

            return RedirectToAction("Details", new { id = model.First().InitiativeIdentifier, application = model.First().ApplicationIdentifier });
        }

        [HttpPost]
        public ActionResult DeleteVersions(List<VersionModel> model)
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
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id).ToModel(null);
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var applicationGroup = initiative.ApplicationGroups.Single(x => x.Applications.Any(y => y.Name == application)).Name;

            var model = new ApplicationPropetiesModel()
            {
                ApplicationGroupName = applicationGroup,
                TicketPrefix = app.TicketPrefix,
                InitiativeId = id,
                ApplicationName = application,
                DevColor = app.DevColor,
                CiColor = app.CiColor,
                ProdColor = app.ProdColor
            };



            return View(model);
        }

        // POST: Application/Edit/5
        [HttpPost]
        public ActionResult Edit(ApplicationPropetiesModel model)
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
                    return RedirectToAction("Details", "Application", new { id = model.InitiativeId, application = model.ApplicationName});
                }
            }
            else
            {
                initiative.AddApplicationGroup(new ApplicationGroup(model.ApplicationGroupName, new List<IApplication>{application}));
            }

            applicationGroup.Remove(application);
            _initiativeBusiness.UpdateInitiative(initiative);

            return RedirectToAction("Details", "Application", new { id = model.InitiativeId, application = model.ApplicationName});
        }

        
        
        //// GET: Application/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}C:\Dev\Tharga\Quilt4\Quilt4.Web\Controllers\ApplicationController.cs

        //// POST: Application/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Application/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Application/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
