﻿using System;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var settings = _settingsBusiness.GetAllSettings();
            return View(settings.ToList().OrderBy(x => x.Name));
        }

        // GET: Admin/Setting/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id", "The parameter id was not provided.");

            var item = _settingsBusiness.GetSetting(id);
            return PartialView(item);
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
                var encrypt = collection["Encrypted"].Contains("true");
                _settingsBusiness.SetSetting(id, collection["Value"], type, encrypt);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}