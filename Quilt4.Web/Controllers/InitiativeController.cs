﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Castle.Core.Internal;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;
using Quilt4.Web.Areas.Admin.Models;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    [Authorize]
    public class InitiativeController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IEmailBusiness _emailBusiness;
        private readonly ISettingsBusiness _settingsBusiness;

        public InitiativeController(IInitiativeBusiness initiativeBusiness, IEmailBusiness emailBusiness, ISettingsBusiness settingsBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
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

                var initiatives = new InitiativesViewModel
                {
                    InitiativeInfos = ins.Select(x => new InitiativeViewModel { Name = x.Name, ClientToken = x.ClientToken, Id = x.Id, OwnerDeveloperName = x.OwnerDeveloperName, UniqueIdentifier = x.GetUniqueIdentifier(ins.Select(xx => xx.Name)) }),
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
            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloperOwner(developerName).ToArray();

            var model = new InviteMemberModel()
            {
                InitiativeId = initiativeid,
                Developer = developerRole,
                UniqueInitiativeIdentifier = initiative.GetUniqueIdentifier(ins.Select(x => x.Name))
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

                //TODO: Använd aldrig kod för att hantera olika miljöer. Detta måste göras utan en if-sats.
                if (root.Equals("http://localhost:54942/"))
                {
                    acceptlink = root + "Initiative/Accept?id=" + initiativeid + "&inviteCode=" + code;
                    declineLink = root + "Initiative/Decline?id=" + initiativeid + "&inviteCode=" + code;
                }
                else if (root.Equals("http://ci.quilt4.com/"))
                {
                    acceptlink = root + "Master/Web/Initiative/Accept?id=" + initiativeid + "&inviteCode=" + code;
                    declineLink = root + "Master/WebInitiative/Decline?id=" + initiativeid + "&inviteCode=" + code;
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
        public ActionResult RemoveMember(string id, string application)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);
            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloperOwner(developerName).ToArray();

            var model = new MemberModel
            {
                DeveloperName = initiative.DeveloperRoles.Single(x => x.DeveloperName == application).DeveloperName,
                InitiativeName = initiative.Name,
                UniqueInitiativeIdentifier = initiative.GetUniqueIdentifier(ins.Select(x => x.Name)),
            };

            return View(model);
        }

        //POST
        [HttpPost]
        public ActionResult RemoveMember(string id, string application, FormCollection collection)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);
            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloperOwner(developerName).ToArray();

            initiative.RemoveDeveloperRole(application);
            _initiativeBusiness.UpdateInitiative(initiative);

            return RedirectToAction("Member", "Initiative", new { id = initiative.GetUniqueIdentifier(ins.Select(x => x.Name)) });
        }

        //Get
        public ActionResult Member(string id)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);

            ViewBag.InviteError = TempData["InviteError"];
                
            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloperOwner(developerName).ToArray();

            var currentUser = User.Identity.GetUserName();

            var invite = new InviteModel
            {
                Initiative = initiative,
                IsAllowedToAdministrate = initiative.OwnerDeveloperName == currentUser || initiative.DeveloperRoles.Single(x => x.DeveloperName == User.Identity.Name).RoleName == "Administrator",
                UniqueInitiativeIdentifier = initiative.GetUniqueIdentifier(ins.Select(x => x.Name)),
            };

            return View(invite);
        }

        //Post
        [HttpPost]
        public ActionResult Invite(FormCollection collection)
        {
            var initiativeId = collection["InitiativeId"];
            var inviteEmail = collection["InviteEmail"];
            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(initiativeId));

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
            if (initiative.DeveloperRoles.Any(x => x.DeveloperName == inviteEmail))
            {
                TempData["InviteError"] = "This developer is already added to the initiative";
                return RedirectToAction("Member", "Initiative", new { id = initiativeId });
            }

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
                    acceptlink = root + "Initiative/Accept?id=" + initiativeId + "&inviteCode=" + invitationCode;
                    declineLink = root + "Initiative/Decline?id=" + initiativeId + "&inviteCode=" + invitationCode;
                }
                else if (root.Equals("http://ci.quilt4.com/"))
                {
                    acceptlink = root + "Master/Web/Initiative/Accept?id=" + initiativeId + "&inviteCode=" + invitationCode;
                    declineLink = root + "Master/WebInitiative/Decline?id=" + initiativeId + "&inviteCode=" + invitationCode;
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

        public ActionResult Accept(string id)
        {
            Guid initiativeId;
            IInitiative initiative;
            if (Guid.TryParse(id, out initiativeId))
            {
                initiative = _initiativeBusiness.GetInitiative(initiativeId);
            }
            else
            {
                initiative = _initiativeBusiness.GetInitiativeByInviteCode(id);
                if (initiative == null)
                {
                    ViewBag.AcceptError = "The invitation has been removed, or perhaps the invite code is wrong.";
                    return View();
                }
            }

            _initiativeBusiness.ConfirmInvitation(initiative.Id, User.Identity.Name);

            return View((object)initiative.Name);
        }

        [AllowAnonymous]
        public ActionResult Decline(string id)
        {
            _initiativeBusiness.DeclineInvitation(id);
            return View();
        }

        // GET: Initiative/Details/5
        public ActionResult Details(string id)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiativeNames = _initiativeBusiness.GetInitiativesByDeveloperOwner(User.Identity.GetUserName()).Select(x => x.Name).ToList();
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
        public ActionResult Create(CreateInitiativeViewModel createInitiative)
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

        // GET: Initiative/Properties/5
        public ActionResult Properties(string id)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);

            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloperOwner(developerName).ToArray();

            var model = new InitiativeViewModel()
            {
                Id = initiative.Id,
                Name = initiative.Name,
                UniqueIdentifier = initiative.GetUniqueIdentifier(ins.Select(x => x.Name))
            };
            
            return View(model);
        }

        // POST: Initiative/Properties/5
        [HttpPost]
        public ActionResult Properties(string id, FormCollection collection)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);

            try
            {
                initiative.Name = collection["Name"];
                _initiativeBusiness.UpdateInitiative(initiative);

                return RedirectToAction("Details", new { id = initiative.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Initiative/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloper(developerName).ToArray();
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id).ToModel(ins.Select(x => x.Name));

            return View(initiative);
        }

        // POST: Initiative/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            if (id == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), id);

            try
            {
                _initiativeBusiness.DeleteInitiative(initiative.Id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}