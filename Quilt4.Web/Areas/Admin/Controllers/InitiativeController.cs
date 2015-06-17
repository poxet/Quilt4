﻿using System;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Controllers;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Admin")]
    public class InitiativeController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;

        public InitiativeController(IInitiativeBusiness initiativeBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
        }

        // GET: Admin/Initiative/Index
        public ActionResult Index()
        {
            //var ub = new InitiativeBusiness(_compositeRoot.Repository);
            var initiatives = _initiativeBusiness.GetInitiatives().Select(x => x.ToModel()).ToList();

            //TODO: This code is too slow, we need to do something about that
            //var issues = _initiativeBusiness.GetIssueStatistics(new DateTime(1900, 01, 01), DateTime.Now);

            //var dateTime = new DateTime();


            //foreach (var initiative in initiatives)
            //{
            //    var sessions = _sessionBusiness.GetSessionsForApplications(initiative.ApplicationsIds).ToArray();
            //    var sessionIds = sessions.Select(x => x.Id);

            //    var list = new List<IIssue>();

            //    foreach (var sessionId in sessionIds)
            //    {
            //        var sessionIssues = issues.Where(x => x.SessionId == sessionId);
            //        list.AddRange(sessionIssues);
            //    }

            //    initiative.Sessions = sessions.Count().ToString();
            //    initiative.Issues = list.Count().ToString();
            //    var lastSessionDate = sessions.OrderBy(x => x.ServerStartTime).Select(y => y.ServerStartTime).FirstOrDefault();
            //    initiative.LastSession = lastSessionDate == dateTime ? "N/A" : lastSessionDate.ToString("yyyy-MM-dd hh:mm:ss");
            //}

            return View(initiatives);
        }

        // GET: Admin/Initiative/Details/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return Redirect("Index");

            var initiative = _initiativeBusiness.GetInitiative(id.Value).ToModel();

            return View(initiative);
        }

        [HttpPost]
        public ActionResult Edit(Web.Models.Initiative model)
        {
            _initiativeBusiness.UpdateInitiative(model.Id, model.Name, model.ClientToken, model.OwnerDeveloperName);
            return Redirect("Index");
        }
    }
}
