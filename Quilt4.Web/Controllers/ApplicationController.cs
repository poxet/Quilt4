using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;

namespace Quilt4.Web.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;

        public ApplicationController(IInitiativeBusiness initiativeBusiness, IApplicationVersionBusiness applicationVersionBusiness) 
        {
            _initiativeBusiness = initiativeBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
        }
        //// GET: Application
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: Application/Details/5
        public ActionResult Details(string id, string application)
        {
            //Guid initiativeId;
            //if (!Guid.TryParse(id, out initiativeId) )
            //{

            //}

            //Guid inititiveId = Guid.Parse(id);
            //var applicationId = _initiativeBusiness.GetApplicationGroups(initiativeId);
            //var initiative = _initiativeBusiness.GetInitiative(initiativeId);

            //Hej Jonas. Jag ändrade metod för att hitta initiativ. Den använder namn om det är unikt, annars en guid.
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id).ToModel();
            var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Name == application).Id;
            //var versions = _applicationVersionBusiness.GetApplicationVersions();

            return View();
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
