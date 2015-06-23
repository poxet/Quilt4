using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Internal;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;
using Quilt4.Web.Agents;
using Quilt4.Web.Areas.Admin.Models;
using Quilt4.Web.Extensions;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    [Authorize]
    public class InitiativeController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IMembershipAgent _membershipAgent;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly IEmailBusiness _emailBusiness;
        private readonly ISettingsBusiness _settingsBusiness;

        public InitiativeController(IInitiativeBusiness initiativeBusiness, IMembershipAgent membershipAgent, IApplicationVersionBusiness applicationVersionBusiness, IEmailBusiness emailBusiness, ISettingsBusiness settingsBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
            _membershipAgent = membershipAgent;
            _applicationVersionBusiness = applicationVersionBusiness;
            _emailBusiness = emailBusiness;
            _settingsBusiness = settingsBusiness;
        }

        // GET: Initiative
        public ActionResult Index()
        {
            try
            {
                var developerName = User.Identity.Name;
                var ins = _initiativeBusiness.GetInitiativesByDeveloper(developerName).ToArray();

                var initiatives = new Initiatives
                {
                    InitiativeInfos = ins.Select(x => new Initiative { Name = x.Name, ClientToken = x.ClientToken, Id = x.Id, OwnerDeveloperName = x.OwnerDeveloperName, UniqueIdentifier = x.GetUniqueIdentifier(ins.Select(xx => xx.Name)) }),
                    //InitiativeInfos = ins.Select(x =>x.ToModel())
                };
                return View(initiatives);
            }
            catch (Exception exception)
            {
                ViewBag.Message = exception.Message;
                return View();
            }
        }

        public ActionResult RemoveMember(string id, string developer)
        {
            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(id));
            initiative.RemoveDeveloperRole(developer);
            _initiativeBusiness.UpdateInitiative(initiative);

            return RedirectToAction("Member", "Initiative", new { initiativeId = id});
        }

        //Get
        public ActionResult Member(string initiativeId)
        {
            if (string.IsNullOrEmpty(initiativeId))
            {
                return Redirect("Index");
            }
                
            Guid id;
            Guid.TryParse(initiativeId, out id);

            var initiative = _initiativeBusiness.GetInitiative(id);
            
            var invite = new InviteModel();
            invite.Initiative = initiative;
            invite.RoleName = invite.Initiative.DeveloperRoles.Single(x => x.DeveloperName == User.Identity.Name).RoleName;

            return View(invite);
        }

        //Post
        [HttpPost]
        public ActionResult Invite(FormCollection collection)
        {
            var initiativeId = collection["InitiativeId"];
            var inviteEmail = collection["InviteEmail"];

            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(initiativeId));
            var invitationCode = initiative.AddDeveloperRolesInvitation(inviteEmail);
            initiative.DeveloperRoles.Single(x => x.InviteEMail == inviteEmail).DeveloperName = inviteEmail;

            _initiativeBusiness.UpdateInitiative(initiative);

            var enabled = _settingsBusiness.GetConfigSetting<bool>("EMailConfirmationEnabled");
            if (enabled)
            {
                var root = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "/");
                var acceptlink = root + "Initiative/ConfirmInvite?id=" + initiativeId + "&inviteCode=" + invitationCode;
                var declineLink = root + "Initiative/DeclineInvite?id=" + initiativeId + "&inviteCode=" + invitationCode;

                var userMessage = "";
                if (!collection["Message"].IsNullOrEmpty())
                {
                    userMessage = "Message: " + collection["Message"] + "<br/><br/>";
                }

                var subject = "Invitation to " + initiative.Name + " at www.quilt4.com";
                var message = initiative.OwnerDeveloperName + " want to invite you to initiative " + initiative.Name + " at Quilt4. <br/><br/>" + userMessage + "<a href='" + acceptlink + "'>Accept</a><br/><a href='" + declineLink + "'>Decline</a>";

                _emailBusiness.SendEmail(new List<string> { inviteEmail }, subject, message);
            }

            return RedirectToAction("Member", "Initiative", new { initiativeId = collection["InitiativeId"] });
        }

        public ActionResult ConfirmInvite(string id, string inviteCode)
        {
            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(id));
            initiative.ConfirmInvitation(inviteCode, User.Identity.Name);
            _initiativeBusiness.UpdateInitiative(initiative);

            return View();
        }

        [AllowAnonymous]
        public ActionResult DeclineInvite(string id, string inviteCode)
        {
            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(id));
            initiative.DeclineInvitation(inviteCode);
            _initiativeBusiness.UpdateInitiative(initiative);

            return View();
        }

        // GET: Initiative/Details/5
        public ActionResult Details(string id)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiativeNames = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.GetUserName()).Select(x => x.Name).ToList();
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id).ToModel(initiativeNames);

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
