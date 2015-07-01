using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;
using Quilt4.Web.Extensions;
using Quilt4.Web.Models;
using IUser = Quilt4.Interface.IUser;

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
            var initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == id).Id;
            var initiative = _initiativeBusiness.GetInitiative(initiativeId).ToModel(null);
            var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application).Id;
            var versions = _applicationVersionBusiness.GetApplicationVersions(applicationId);
            var versionName = _applicationVersionBusiness.GetApplicationVersion(initiativeId.ToString(), version).Version;

            var ver = versions.Single(x => x.Id.Replace(":", "") == version || x.Version == version);

            var issue = new IssueModel
            {
                InitiativeId = initiativeId.ToString(),
                ApplicationName = application,
                Version = version,
                VersionName = versionName,
                IssueTypes = ver.IssueTypes,
                Sessions = _sessionBusiness.GetSessionsForApplicationVersion(ver.Id),
                ApplicationVersionId = applicationId.ToString(),
                //TODO: Add applicationversion id
            };

            //TODO: fetch version anmes
            issue.UniqueIdentifier = issue.GetUniqueIdentifier(versionName);

            //issue.ExceptionTypeName = ver.IssueTypes.Select(x => x.ExceptionTypeName);
            //issue.Message = ver.IssueTypes.Select(x => x.Message);
            //issue.Level = ver.IssueTypes.Select(x => x.IssueLevel.ToString());
            //issue.Count = ver.IssueTypes.Select(x => x.Count.ToString());
            //issue.Ticket = ver.IssueTypes.Select(x => x.Ticket.ToString());


            var users = issue.Sessions.Select(user => _userBusiness.GetUser(user.UserFingerprint)).ToList();

            issue.Users = users;

            issue.Machines = _machineBusiness.GetMachinesByApplicationVersion(ver.Id);
    
            return View(issue);
        }


        //// GET: Version/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Version/Create
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

        //// GET: Version/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Version/Edit/5
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

        //// GET: Version/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Version/Delete/5
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
