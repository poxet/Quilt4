﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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

        // GET: Application/Details/5
        public ActionResult Details(string id, string application)
        {
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id).ToModel(null);

            var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application).Id;
            var versions = _applicationVersionBusiness.GetApplicationVersions(applicationId).ToArray();
            var versionNames = versions.Select(x => x.Version);
            var versionIds = versions.Select(x => x.Id);

            var sessions = _sessionBusiness.GetSessionsForApplications(new List<Guid> { applicationId }).ToArray();

            var machines = _machineBusiness.GetMachinesByApplicationVersions(versionIds);

            var model = new ApplicationModel
            {
                Initiative = id, 
                Application = application,
                Versions = versions.Select(x => new VersionModel
                {
                    Version = x.Version,
                    IssueTypes = x.IssueTypes,
                    UniqueIdentifier = x.GetUniqueIdentifier(versionNames),

                    //TODO: This is fucking sloooooow ... fix this
                    //Machines = _machineBusiness.GetMachinesByApplicationVersion(x.Id),
                    Machines = machines.Where(z => sessions.Any(y => y.ApplicationVersionId == x.Id && y.MachineFingerprint == z.Id)),


                    Sessions = sessions.Where(y => y.ApplicationVersionId == x.Id),
                }).ToList()
            };

            return View(model);
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

        //// GET: Application/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Application/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

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
