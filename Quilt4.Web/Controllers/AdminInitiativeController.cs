using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;

namespace Quilt4.Web.Controllers
{
    //TODO: Roles
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class AdminInitiativeController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IIssueBusiness _issueBusiness;

        public AdminInitiativeController(IInitiativeBusiness initiativeBusiness, ISessionBusiness sessionBusiness, IIssueBusiness issueBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
            _sessionBusiness = sessionBusiness;
            _issueBusiness = issueBusiness;
        }

        // GET: AdminInitiative
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

        public ActionResult Initiative(Guid? id)
        {
            if (id == null)
                return Redirect("Index");

            var initiative = _initiativeBusiness.GetInitiative((Guid)id).ToModel();

            return View(initiative);
        }

        //// GET: AdminInitiative/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: AdminInitiative/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: AdminInitiative/Create
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

        //// GET: AdminInitiative/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: AdminInitiative/Edit/5
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

        //// GET: AdminInitiative/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: AdminInitiative/Delete/5
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
