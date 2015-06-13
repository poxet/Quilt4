using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quilt4.Web.Controllers
{
    public class IssueTypeController : Controller
    {
        //// GET: IssueType
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: IssueType/Details/5
        public ActionResult Details(string id, string application, string version, string issueType)
        {
            //id -> Initiative

            return View();
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
