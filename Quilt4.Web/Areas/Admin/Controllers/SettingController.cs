using System;
using System.Web.Mvc;
using Quilt4.Interface;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    public class SettingController : Controller
    {
        private readonly ISettingsBusiness _settingsBusiness;

        public SettingController(ISettingsBusiness settingsBusiness)
        {
            _settingsBusiness = settingsBusiness;
        }

        // GET: Admin/Setting
        public ActionResult Index()
        {
            var settings = _settingsBusiness.GetAllDatabaseSettings();
            return View(settings);
        }

        //// GET: Admin/Setting/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Admin/Setting/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Admin/Setting/Create
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

        // GET: Admin/Setting/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id", "The parameter id was not provided.");

            var item = _settingsBusiness.GetDatabaseSetting(id);
            return View(item);
        }

        // POST: Admin/Setting/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, FormCollection collection)
        {
            try
            {
                var typeName = collection["Type"];
                var type = Type.GetType(typeName);
                var data = Convert.ChangeType(collection["Value"], type);
                if (data == null) throw new InvalidOperationException();
                _settingsBusiness.SetDatabaseSetting(id, collection["Value"], type);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //// GET: Admin/Setting/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Admin/Setting/Delete/5
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
