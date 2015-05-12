using System;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Agents;
using Quilt4.Web.Business;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    [Authorize]
    public class InitiativeController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IMembershipAgent _membershipAgent;

        public InitiativeController(IInitiativeBusiness initiativeBusiness, IMembershipAgent membershipAgent)
        {
            _initiativeBusiness = initiativeBusiness;
            _membershipAgent = membershipAgent;
        }

        // GET: Initiative
        public ActionResult Index()
        {
            return View();
        }

        // GET: Initiative/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Initiative/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Initiative/Create
        [HttpPost]
        public ActionResult Create(CreateInitiative createInitiative)
        {
            try
            {
                if (createInitiative == null) throw new ArgumentNullException("createInitiative");

                var developer = _membershipAgent.GetDeveloper();
                _initiativeBusiness.Create(developer.DeveloperName, createInitiative.Name);

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                ViewBag.Message = exception.Message;
                return View();
            }
        }

        // GET: Initiative/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Initiative/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Initiative/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Initiative/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
