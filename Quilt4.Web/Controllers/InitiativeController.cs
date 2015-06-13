using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;
using Quilt4.Web.Agents;
using Quilt4.Web.Models;
using System.Collections;

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
            try
            {
                //var currentDeveloper = _compositeRoot.MembershipAgent.GetDeveloper();

                //var service = new Service.WebService(_compositeRoot.Repository);
                //var initiatives = service.GetInitiativesByDeveloperHead(currentDeveloper.DeveloperName);                
                var developerName = User.Identity.Name;
                var ins = _initiativeBusiness.GetInitiativesByDeveloperHead(developerName);

                //var ib = new InitiativeBusiness(_compositeRoot.Repository);

                //var pending = ib.GetPendingApprovals(currentDeveloper.EMail);

                var initiatives = new Initiatives
                {
                    InitiativeInfos = ins.Select(x => new Initiative{ Name = x.Name, ClientToken = x.ClientToken, Id = x.Id, OwnerDeveloperName = x.OwnerDeveloperName}),
                //    IsEMailConfirmed = _compositeRoot.MembershipAgent.IsEMailConfirmed(currentDeveloper.DeveloperName),
                //    InviteEMail = currentDeveloper.EMail,
                //    Invitations = pending,
                //    SingleInitiative = false,
                };
                //return View("Index", m);
                return View(initiatives);
            }
            catch (Exception exception)
            {
                ViewBag.Message = exception.Message;
                //return View("Index", new Initiatives { InitiativeInfos = new List<Service.Model.Initiative>(), Invitations = new List<IInviteApproval>() });
                return View();
            }
        }

        public ActionResult Member(string initiativeId)
        {
            if (string.IsNullOrEmpty(initiativeId))
            {
                return Redirect("Index");
            }
                
            Guid id;
            Guid.TryParse(initiativeId, out id);

            var initiative = _initiativeBusiness.GetInitiative(id);

            var members = initiative.DeveloperRoles;

            return View(members);
        }

        // GET: Initiative/Details/5
        public ActionResult Details(string id)
        {
            var initiative = _initiativeBusiness.GetInitiativesByDeveloperHead(User.Identity.GetUserName()).Single(x => x.Name == id);


            return View(initiative); 
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

                var developerName = User.Identity.GetUserName();
                _initiativeBusiness.Create(developerName, createInitiative.Name);

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
