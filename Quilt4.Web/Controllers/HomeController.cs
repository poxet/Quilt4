using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Castle.Core.Internal;
using Quilt4.Interface;
using Quilt4.Web.Models;
using Tharga.Quilt4Net;

namespace Quilt4.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;

        public HomeController(IInitiativeBusiness initiativeBusiness, IApplicationVersionBusiness applicationVersionBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult System()
        {
            ViewBag.Version = Assembly.GetAssembly(typeof(HomeController)).GetName().Version.ToString();
            ViewBag.Environment = Information.Environment;
            ViewBag.Quilt4SessionStarter = Tharga.Quilt4Net.Session.ClientStartTime.ToLocalTime();
            ViewBag.Quilt4RegisteredOnServer = Tharga.Quilt4Net.Session.RegisteredOnServer;
            ViewBag.Quilt4HasClientToken = !string.IsNullOrEmpty(Configuration.ClientToken);
            ViewBag.Quilt4IsEnabled = Configuration.Enabled;

            return View();
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchModel model)
        {
            if (model.SearchText.IsNullOrEmpty())
            {
                return RedirectToAction("Search","Home");
            }

            var initiativeHeads = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.Name);
            var initiatives = initiativeHeads.Select(initiativeHead => _initiativeBusiness.GetInitiative(initiativeHead.Id)).ToArray();
            //var applicationIds = initiatives.SelectMany(x => x.ApplicationGroups).SelectMany(x => x.Applications.Select(y => y.Id));

            var applicationIds = new List<Guid>();
            foreach (var initiative in initiatives)
            {
                applicationIds.AddRange(initiative.ApplicationGroups.SelectMany(x => x.Applications).Select(x => x.Id));
            }

            //var versions = applicationIds.SelectMany(applicationId => _applicationVersionBusiness.GetApplicationVersions(applicationId));
            var versions = new List<IApplicationVersion>();
            foreach (var applicationId in applicationIds)
            {
                versions.AddRange(_applicationVersionBusiness.GetApplicationVersions(applicationId));
            }

            var allIssueTypes = versions.SelectMany(x => x.IssueTypes).ToArray();
            
            var issueTypes = new List<IIssueType>();
            foreach (var issueType in allIssueTypes)
            {
                if (!issueType.ExceptionTypeName.IsNullOrEmpty())
                {
                    if (issueType.ExceptionTypeName.Contains(model.SearchText))
                    {
                        issueTypes.Add(issueType);
                    }
                }
                
            }

            model.IssueTypes = issueTypes.OrderBy(x => x.ExceptionTypeName);

            return View("SearchResults", model);
        }
    }
}