using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Castle.Core.Internal;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;
using Quilt4.Web.Agents;
using Quilt4.Web.Areas.Admin.Models;
using Quilt4.Web.Extensions;
using Quilt4.Web.Models;
using Tharga.Quilt4Net;

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

        public ActionResult ResendInvite(string initiativeid, string code)
        {
            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(initiativeid));
            var developerRole = initiative.DeveloperRoles.Single(x => x.InviteCode == code);

            var model = new InviteMemberModel()
            {
                InitiativeId = initiativeid,
                Developer = developerRole,
            };
            
            return View(model);
        }

        [HttpPost]
        public ActionResult ResendInvite(string initiativeid, string code, FormCollection collection)
        {

            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(initiativeid));
            var developerRole = initiative.DeveloperRoles.Single(x => x.InviteCode == code);

            var enabled = _settingsBusiness.GetEmailSetting().EMailConfirmationEnabled;
            if (enabled)
            {
                var root = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "/");
                var acceptlink = string.Empty;
                var declineLink = string.Empty;

                if (root.Equals("http://localhost:54942/"))
                {
                    acceptlink = root + "Initiative/ConfirmInvite?id=" + initiativeid + "&inviteCode=" + code;
                    declineLink = root + "Initiative/DeclineInvite?id=" + initiativeid + "&inviteCode=" + code;
                }
                else if (root.Equals("http://ci.quilt4.com/"))
                {
                    acceptlink = root + "Master/Web/Initiative/ConfirmInvite?id=" + initiativeid + "&inviteCode=" + code;
                    declineLink = root + "Master/WebInitiative/DeclineInvite?id=" + initiativeid + "&inviteCode=" + code;
                }
                else
                {
                    //Prod
                }

                var subject = "A reminder for the invitation to " + initiative.Name + " at www.quilt4.com";
                var message = initiative.OwnerDeveloperName + " want to remind you to answer the invitation to initiative " + initiative.Name + " at Quilt4. <br/><br/><a href='" + acceptlink + "'>Accept</a><br/><a href='" + declineLink + "'>Decline</a>";

                try
                {
                    _emailBusiness.SendEmail(new List<string> { developerRole.InviteEMail }, subject, message);
                }
                catch (SmtpException e)
                {
                    TempData["InviteError"] = "Could not connect to the email server";
                    return RedirectToAction("Member", "Initiative", new { id = initiativeid });
                }
                catch (Exception e)
                {
                    TempData["InviteError"] = "Something went wrong";
                    return RedirectToAction("Member", "Initiative", new { id = initiativeid });
                }
            }


            return RedirectToAction("Member", "Initiative", new { id = initiativeid});
        }

        //GET
        public ActionResult RemoveMember(string initiativeId, string developer)
        {
            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(initiativeId));
            var model = new MemberModel()
            {
                DeveloperName = initiative.DeveloperRoles.Single(x => x.DeveloperName == developer).DeveloperName,
                InitiativeName = initiative.Name,
                InitiativeId = initiativeId,
            };

            return View(model);
        }

        //POST
        [HttpPost]
        public ActionResult RemoveMember(string initiativeId, string developer, FormCollection collection)
        {
            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(initiativeId));
            initiative.RemoveDeveloperRole(developer);
            _initiativeBusiness.UpdateInitiative(initiative);

            return RedirectToAction("Details", "Initiative", new { id = initiativeId});
        }

        //Get
        public ActionResult Member(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("Index");
            }

            ViewBag.InviteError = TempData["InviteError"];
                
            Guid initiativeId;
            if (!Guid.TryParse(id, out initiativeId))
                throw new InvalidOperationException("Cannot parse as Guid!").AddData("id", id);

            var initiative = _initiativeBusiness.GetInitiative(initiativeId);

            var currentUser = User.Identity.GetUserName();

            var invite = new InviteModel
            {
                Initiative = initiative,
                IsAllowedToAdministrate = initiative.OwnerDeveloperName == currentUser || initiative.DeveloperRoles.Single(x => x.DeveloperName == User.Identity.Name).RoleName == "Administrator"
            };

            return View(invite);
        }

        //Post
        [HttpPost]
        public ActionResult Invite(FormCollection collection)
        {
            var initiativeId = collection["InitiativeId"];
            var inviteEmail = collection["InviteEmail"];

            if (inviteEmail.IsNullOrEmpty())
            {
                TempData["InviteError"] = "Please enter an email adress";
                return RedirectToAction("Member", "Initiative", new {id = initiativeId});
            }
            if (!new EmailAddressAttribute().IsValid(inviteEmail))
            {
                TempData["InviteError"] = "Please enter a valid email adress";
                return RedirectToAction("Member", "Initiative", new { id = initiativeId });
            }

            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(initiativeId));
            var invitationCode = initiative.AddDeveloperRolesInvitation(inviteEmail);
            initiative.DeveloperRoles.Single(x => x.InviteEMail == inviteEmail).DeveloperName = inviteEmail;

            var enabled = _settingsBusiness.GetEmailSetting().EMailConfirmationEnabled;
            if (enabled)
            {
                var root = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "/");
                var acceptlink = string.Empty;
                var declineLink = string.Empty;

                if (root.Equals("http://localhost:54942/"))
                {
                    acceptlink = root + "Initiative/ConfirmInvite?id=" + initiativeId + "&inviteCode=" + invitationCode;
                    declineLink = root + "Initiative/DeclineInvite?id=" + initiativeId + "&inviteCode=" + invitationCode;
                }
                else if (root.Equals("http://ci.quilt4.com/"))
                {
                    acceptlink = root + "Master/Web/Initiative/ConfirmInvite?id=" + initiativeId + "&inviteCode=" + invitationCode;
                    declineLink = root + "Master/WebInitiative/DeclineInvite?id=" + initiativeId + "&inviteCode=" + invitationCode;
                }
                else
                {
                    //Prod
                }
                

                var userMessage = "";
                if (!collection["Message"].IsNullOrEmpty())
                {
                    userMessage = "Message: " + collection["Message"] + "<br/><br/>";
                }

                var subject = "Invitation to " + initiative.Name + " at www.quilt4.com";
                var message = initiative.OwnerDeveloperName + " want to invite you to initiative " + initiative.Name + " at Quilt4. <br/><br/>" + userMessage + "<a href='" + acceptlink + "'>Accept</a><br/><a href='" + declineLink + "'>Decline</a>";

                try
                {
                    _emailBusiness.SendEmail(new List<string> { inviteEmail }, subject, message);
                }
                catch (SmtpException e)
                {
                    TempData["InviteError"] = "Could not connect to the email server";
                    return RedirectToAction("Member", "Initiative", new { id = initiativeId });
                }
                catch (Exception e)
                {
                    TempData["InviteError"] = "Something went wrong";
                    return RedirectToAction("Member", "Initiative", new { id = initiativeId });
                }
            }

            //if everything went well, save the initiative
            _initiativeBusiness.UpdateInitiative(initiative);

            return RedirectToAction("Member", "Initiative", new { id = collection["InitiativeId"] });
        }

        public ActionResult ConfirmInvite(string id, string inviteCode)
        {
            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(id));
            initiative.ConfirmInvitation(inviteCode, User.Identity.Name);
            _initiativeBusiness.UpdateInitiative(initiative);

            return View((object)initiative.Name);
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