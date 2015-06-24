using System;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    [Authorize]
    public class IssueTypeController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IUserBusiness _userBusiness;

        public IssueTypeController(IInitiativeBusiness initiativeBusiness, IApplicationVersionBusiness applicationVersionBusiness, ISessionBusiness sessionBusiness, IUserBusiness userBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
            _sessionBusiness = sessionBusiness;
            _userBusiness = userBusiness;
        }
        //// GET: IssueType
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: IssueType/Details/5
        public ActionResult Details(string id, string application, string version, string issueType)
        {
            var initiative = _initiativeBusiness.GetInitiatives().SingleOrDefault(x => x.Name == id);
            var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).SingleOrDefault(x => x.Name == application).Id;
            //TODO: version is an uniqe something ... how should this be solved?
            var ver = _applicationVersionBusiness.GetApplicationVersions(applicationId).SingleOrDefault(x => x.Version == version);
            
            var model = new IssueTypeModel
            {
                IssueType = ver.IssueTypes.Single(x => x.Ticket.ToString() == issueType), 
                Sessions = _sessionBusiness.GetSessionsForApplicationVersion(ver.Id)
            };
            model.Users = model.Sessions.Select(user => _userBusiness.GetUser(user.UserFingerprint)).ToList();
            


            //id -> Initiative
            //var initiative = _initiativeBusiness.GetInitiatives().Single(x => x.Name == id);
            //var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application).Id;
            //var ver = _applicationVersionBusiness.GetApplicationVersions(applicationId);
            //var issueTypes = ver.Single(x => x.Version == version).IssueTypes;
            //var model = new IssueTypeModel
            //{
            //    IssueType = issueTypes.Single(x => x.Ticket.ToString() == issueType), 
            //    Sessions = _sessionBusiness.GetSessionsForApplicationVersion(application)
            //};
            return View(model);
        }

        //// GET: IssueType/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: IssueType/Create
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

        //// GET: IssueType/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: IssueType/Edit/5
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

        //// GET: IssueType/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: IssueType/Delete/5
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
