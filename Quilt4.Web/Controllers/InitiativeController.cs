using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.UI;
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
                var subject = "A reminder for the invitation to " + initiative.Name + " at www.quilt4.com";
                var message = _initiativeBusiness.GenerateInviteMessage(initiativeid, code, string.Empty, Request.Url);

                try
                {
                    _emailBusiness.SendEmail(new List<string> { developerRole.InviteEMail }, subject, message);
                }
                catch (SmtpException e)
                {
                    TempData["InviteError"] = "Could not connect to the email server";
                    return RedirectToAction("Member", "Initiative", new { initiativeUniqueIdentifier = initiativeid });
                }
                catch (Exception e)
                {
                    TempData["InviteError"] = "Something went wrong";
                    return RedirectToAction("Member", "Initiative", new { initiativeUniqueIdentifier = initiativeid });
                }
            }

            return RedirectToAction("Member", "Initiative", new { initiativeUniqueIdentifier = initiativeid });
        }

        //GET
        public ActionResult RemoveMember(string initiativeUniqueIdentifier, string application)
        {
            if (initiativeUniqueIdentifier == null) throw new ArgumentNullException("initiativeUniqueIdentifier", "InitiativeId was not provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == initiativeUniqueIdentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == initiativeUniqueIdentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(initiativeUniqueIdentifier)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString());
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
        public ActionResult RemoveMember(string initiativeUniqueIdentifier, string application, FormCollection collection)
        {
            if (initiativeUniqueIdentifier == null) throw new ArgumentNullException("initiativeUniqueIdentifier", "InitiativeId was not provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == initiativeUniqueIdentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == initiativeUniqueIdentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(initiativeUniqueIdentifier)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString());
            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloperOwner(developerName).ToArray();

            initiative.RemoveDeveloperRole(application);
            _initiativeBusiness.UpdateInitiative(initiative);

            return RedirectToAction("Member", "Initiative", new { initiativeUniqueIdentifier = initiative.GetUniqueIdentifier(ins.Select(x => x.Name)) });
        }

        //Get
        public ActionResult Member(string initiativeUniqueIdentifier)
        {
            if (initiativeUniqueIdentifier == null) throw new ArgumentNullException("initiativeUniqueIdentifier", "InitiativeId was not provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == initiativeUniqueIdentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == initiativeUniqueIdentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(initiativeUniqueIdentifier)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString());

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
                return RedirectToAction("Member", "Initiative", new { initiativeUniqueIdentifier = initiativeId });
            }
            if (!new EmailAddressAttribute().IsValid(inviteEmail))
            {
                TempData["InviteError"] = "Please enter a valid email adress";
                return RedirectToAction("Member", "Initiative", new { initiativeUniqueIdentifier = initiativeId });
            }
            if (initiative.DeveloperRoles.Any(x => x.DeveloperName == inviteEmail))
            {
                TempData["InviteError"] = "This developer is already added to the initiative";
                return RedirectToAction("Member", "Initiative", new { initiativeUniqueIdentifier = initiativeId });
            }

            var invitationCode = initiative.AddDeveloperRolesInvitation(inviteEmail);
            initiative.DeveloperRoles.Single(x => x.InviteEMail == inviteEmail).DeveloperName = inviteEmail;

            var enabled = _settingsBusiness.GetEmailSetting().EMailConfirmationEnabled;
            if (enabled)
            {
                var userMessage = "";
                if (!collection["Message"].IsNullOrEmpty())
                {
                    userMessage = "Message: " + collection["Message"] + "<br/><br/>";
                }

                var subject = "Invitation to " + initiative.Name + " at www.quilt4.com";
                var message = _initiativeBusiness.GenerateInviteMessage(initiativeId, invitationCode, userMessage, Request.Url);

                try
                {
                    _emailBusiness.SendEmail(new List<string> { inviteEmail }, subject, message);
                }
                catch (SmtpException e)
                {
                    TempData["InviteError"] = "Could not connect to the email server";
                    return RedirectToAction("Member", "Initiative", new { initiativeUniqueIdentifier = initiativeId });
                }
                catch (Exception e)
                {
                    TempData["InviteError"] = "Something went wrong";
                    return RedirectToAction("Member", "Initiative", new { id = initiativeId });
                }
            }

            //if everything went well, save the initiative
            _initiativeBusiness.UpdateInitiative(initiative);

            return RedirectToAction("Member", "Initiative", new { initiativeUniqueIdentifier = collection["InitiativeId"] });
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
        public ActionResult Details(string initiativeUniqueIdentifier)
        {
            if (initiativeUniqueIdentifier == null) throw new ArgumentNullException("id", "No initiative id provided.");

            var i = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.Name).Where(x => x.Name == initiativeUniqueIdentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.Name).Single(x => x.Name == initiativeUniqueIdentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.Name).Single(x => x.Id == Guid.Parse(initiativeUniqueIdentifier)).Id;
            }

            var initiativeNames = _initiativeBusiness.GetInitiativesByDeveloperOwner(User.Identity.GetUserName()).Select(x => x.Name).ToList();
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString()).ToModel(initiativeNames);
            initiative.UniqueIdentifier = initiativeUniqueIdentifier;

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

        // GET: Initiative/Edit/5
        public ActionResult Edit(string initiativeUniqueIdentifier)
        {
            if (initiativeUniqueIdentifier == null) throw new ArgumentNullException("initiativeUniqueIdentifier", "InitiativeId was not provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == initiativeUniqueIdentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == initiativeUniqueIdentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(initiativeUniqueIdentifier)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString());

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

        // POST: Initiative/Edit/5
        [HttpPost]
        public ActionResult Edit(string initiativeUniqueIdentifier, FormCollection collection)
        {
            if (initiativeUniqueIdentifier == null) throw new ArgumentNullException("initiativeUniqueIdentifier", "InitiativeId was not provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == initiativeUniqueIdentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == initiativeUniqueIdentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(initiativeUniqueIdentifier)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString());

            try
            {
                initiative.Name = collection["Name"];
                _initiativeBusiness.UpdateInitiative(initiative);

                return RedirectToAction("Details", new { initiativeUniqueIdentifier = initiative.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Initiative/Delete/5
        public ActionResult Delete(string initiativeUniqueIdentifier)
        {
            if (initiativeUniqueIdentifier == null) throw new ArgumentNullException("initiativeUniqueIdentifier", "No initiative id provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == initiativeUniqueIdentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == initiativeUniqueIdentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(initiativeUniqueIdentifier)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var developerName = User.Identity.Name;
            var ins = _initiativeBusiness.GetInitiativesByDeveloper(developerName).ToArray();
            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString()).ToModel(ins.Select(x => x.Name));

            return View(initiative);
        }

        // POST: Initiative/Delete/5
        [HttpPost]
        public ActionResult Delete(string initiativeUniqueIdentifier, FormCollection collection)
        {
            if (initiativeUniqueIdentifier == null) throw new ArgumentNullException("initiativeUniqueIdentifier", "InitiativeId was not provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == initiativeUniqueIdentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == initiativeUniqueIdentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(initiativeUniqueIdentifier)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(User.Identity.GetUserName(), initiativeId.ToString());

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