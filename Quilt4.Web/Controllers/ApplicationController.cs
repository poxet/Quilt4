using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Internal;
using Microsoft.AspNet.Identity;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.Models;
using Constants = Quilt4.Web.Models.Constants;

namespace Quilt4.Web.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly IMachineBusiness _machineBusiness;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IAccountRepository _accountRepository;

        public ApplicationController(IInitiativeBusiness initiativeBusiness, IApplicationVersionBusiness applicationVersionBusiness, IMachineBusiness machineBusiness, ISessionBusiness sessionBusiness, IAccountRepository accountRepository) 
        {
            _initiativeBusiness = initiativeBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
            _machineBusiness = machineBusiness;
            _sessionBusiness = sessionBusiness;
            _accountRepository = accountRepository;
        }

        public ApplicationViewModel GetApplicationModel(IInitiative initiative, string application, IEnumerable<IApplicationVersion> versions)
        {
            var developerName = _accountRepository.FindById(User.Identity.GetUserId()).Email;
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
                    Environments = x.Environments.Select(y => string.IsNullOrEmpty(y) ? Constants.DefaultEnvironmentName : y)
                }).OrderByDescending(y => y.Version).ToList(),
            };

            var envs = applicationVersions.SelectMany(x => x.Environments).Distinct().ToArray();
            var environmentColors = _initiativeBusiness.GetEnvironmentColors(User.Identity.GetUserId(), _accountRepository.FindById(User.Identity.GetUserId()).UserName).ToArray();

            model.EnvironmentColors = (from environmentColor in environmentColors where envs.Any(x => x == environmentColor.Key) select new EnvironmentViewModel()
            {
                Name = string.IsNullOrEmpty(environmentColor.Key) ? Constants.DefaultEnvironmentName : environmentColor.Key,
                Color = environmentColor.Value
            }).ToList();

            return model;
        }

        // GET: Application/Details/5
        public ActionResult Details(string id, string application)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(_accountRepository.FindById(User.Identity.GetUserId()).Email, id);
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var versions = _applicationVersionBusiness.GetApplicationVersions(app.Id).ToArray();
            var model = GetApplicationModel(initiative, application, versions);

            @ViewBag.IsArchive = false;
            @ViewBag.Title = "Application Details";
            @ViewBag.SiteRoot = GetSiteRoot();
            return View(model);
        }

        // GET: Application/Sessions/A/B
        public JsonResult Sessions(string id, string application, bool archived)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(_accountRepository.FindById(User.Identity.GetUserId()).Email, id);
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var sessions = archived ? _sessionBusiness.GetArchivedSessionsForApplications(new List<Guid> { app.Id }).ToArray() : _sessionBusiness.GetSessionsForApplications(new List<Guid> { app.Id }).ToArray();

            var versions = archived ? _applicationVersionBusiness.GetArchivedApplicationVersions(app.Id).ToArray() : _applicationVersionBusiness.GetApplicationVersions(app.Id).ToArray();

            //TODO: Här skall data som first, last med.
            var vers = versions.Select(x =>
            {
                var ss = sessions.Where(y => y.ApplicationVersionId == x.Id).ToArray();
                return new
                {
                    Id = x.Id,
                    SessionCount = ss.Count(y => y.ApplicationVersionId == x.Id),
                    First = ss.Any() ? ss.Min(y => y.ServerStartTime).ToLocalTime().ToTimeAgo() : "N/A",
                    Last = ss.Any() ? ss.Max(y => y.ServerStartTime).ToLocalTime().ToTimeAgo() : "N/A",
                };
            }).ToArray();

            var response = Json(vers, JsonRequestBehavior.AllowGet);
            return response;
        }

        public JsonResult Machines(string id, string application, bool archived)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(_accountRepository.FindById(User.Identity.GetUserId()).Email, id);
            var app = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application);
            var versions = archived ? _applicationVersionBusiness.GetArchivedApplicationVersions(app.Id).ToArray() : _applicationVersionBusiness.GetApplicationVersions(app.Id).ToArray();

            var ms = new List<object>();
            foreach (var version in versions)
            {
                ms.Add(new
                {
                    Id = version.Id,
                    MachineCount = _machineBusiness.GetMachinesByApplicationVersion(version.Id).Count(), //TODO: this method is super slow, takes ~0,5sek per version
                });
            }

            var response = Json(ms, JsonRequestBehavior.AllowGet);
            return response;
        }

        // GET: Application/Archive/5
        public ActionResult Archive(string id, string application)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(_accountRepository.FindById(User.Identity.GetUserId()).Email, id);
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

            var initiative = _initiativeBusiness.GetInitiative(_accountRepository.FindById(User.Identity.GetUserId()).Email, id);

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

            if (app.KeepLatestVersions == null)
            {
                model.AutoArchive = false;
                model.KeepLatestVersions = 5;
            }
            else
            {
                model.AutoArchive = true;
                model.KeepLatestVersions = app.KeepLatestVersions;
            }

            return View(model);
        }

        // POST: Application/Edit/5
        [HttpPost]
        public ActionResult Edit(ApplicationPropetiesModel model, FormCollection collection)
        {
            var initiative = _initiativeBusiness.GetInitiative(_accountRepository.FindById(User.Identity.GetUserId()).Email, model.InitiativeId);
            var applicationGroup = initiative.ApplicationGroups.Single(x => x.Applications.Any(y => y.Name == model.ApplicationName));
            var application = applicationGroup.Applications.Single(x => x.Name == model.ApplicationName);
            application.TicketPrefix = model.TicketPrefix;

            List<IApplicationVersion> versionsToArchive = null;

            //application.KeepLatestVersions = model.AutoArchive ? model.KeepLatestVersions : null;

            if (model.AutoArchive && model.KeepLatestVersions > 0)
            {
                var versions = _applicationVersionBusiness.GetApplicationVersions(application.Id).ToArray();
                versionsToArchive = versions.OrderBy(x => x.Version).Take(versions.Count() - model.KeepLatestVersions.Value).ToList();

                if (versionsToArchive.IsNullOrEmpty()) //No versions to archive, set autoarchive to true
                {
                    application.KeepLatestVersions = model.KeepLatestVersions;
                }
            }

            if(initiative.ApplicationGroups.Any(x => x.Name == model.ApplicationGroupName))
            {
                if (initiative.ApplicationGroups.Single(x => x.Name == model.ApplicationGroupName).Applications.All(x => x.Name != model.ApplicationName))
                {
                    initiative.ApplicationGroups.Single(x => x.Name == model.ApplicationGroupName).Add(application);
                }
                else
                {
                    _initiativeBusiness.UpdateInitiative(initiative);

                    //Check if versions to archive, if yes inform user
                    if (versionsToArchive.IsNullOrEmpty())
                    {
                        return RedirectToAction("Details", "Application", new { id = model.InitiativeId, application = model.ApplicationName });
                    }
                    
                    //return EnableArchive(versionsToArchive);
                    return RedirectToAction("EnableArchive", "Application", new { applicationId = application.Id, keepLatestVersions = model.KeepLatestVersions});
                }
            }
            else
            {
                initiative.AddApplicationGroup(new ApplicationGroup(model.ApplicationGroupName, new List<IApplication>{application}));
            }

            applicationGroup.Remove(application);
            _initiativeBusiness.UpdateInitiative(initiative);

            //Check if versions to archive, if yes inform user
            if (versionsToArchive.IsNullOrEmpty())
            {
                return RedirectToAction("Details", "Application", new { id = model.InitiativeId, application = model.ApplicationName });
            }

            //return EnableArchive(versionsToArchive);
            return RedirectToAction("EnableArchive", "Application", new { applicationId = application.Id, keepLatestVersions = model.KeepLatestVersions });
        }

        public ActionResult EnableArchive(string applicationId, int keepLatestVersions)
        {
            var versions = _applicationVersionBusiness.GetApplicationVersions(Guid.Parse(applicationId)).ToArray();
            var versionsToArchive = versions.OrderBy(x => x.Version).Take(versions.Count() - keepLatestVersions).ToArray();
            var model = new EnableArchiveModel()
            {
                VersionsToArchive = versionsToArchive,
                Application = applicationId,
                InitiativeId = _initiativeBusiness.GetInitiativeByApplication(Guid.Parse(applicationId)).Id.ToString()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EnableArchive(FormCollection collection)
        {
            var initiativeId = collection.GetValue("InitiativeId").AttemptedValue;
            var applicationId = collection.GetValue("ApplicationId").AttemptedValue;
            var applicationName = _initiativeBusiness.GetInitiatives().SelectMany(x => x.ApplicationGroups).SelectMany(x => x.Applications).Single(x => x.Id == Guid.Parse(applicationId)).Name;

            collection.Remove("InitiativeId");
            collection.Remove("ApplicationId");

            var versionIds = (from object key in collection.Keys select collection.GetValue(key.ToString()).AttemptedValue).ToList();

            foreach (var versionId in versionIds)
            {
                _initiativeBusiness.ArchiveApplicationVersion(versionId);
            }

            return RedirectToAction("Details", "Application", new { id = initiativeId, application = applicationName });
        }

        public static string GetSiteRoot()//Move this method to a better place
        {
            string port = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            if (port == null || port == "80" || port == "443")
                port = "";
            else
                port = ":" + port;

            string protocol = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (protocol == null || protocol == "0")
                protocol = "http://";
            else
                protocol = "https://";

            string sOut = protocol + System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + port + System.Web.HttpContext.Current.Request.ApplicationPath;

            if (sOut.EndsWith("/"))
            {
                sOut = sOut.Substring(0, sOut.Length - 1);
            }

            return sOut;
        }
    }
}