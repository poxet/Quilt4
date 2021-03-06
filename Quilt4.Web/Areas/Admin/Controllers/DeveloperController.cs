﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Quilt4.Interface;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Admin")]
    public class DeveloperController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IInitiativeBusiness _initiativeBusiness;

        public DeveloperController(IAccountRepository accountRepository, IInitiativeBusiness initiativeBusiness)
        {
            _accountRepository = accountRepository;
            _initiativeBusiness = initiativeBusiness;
        }

        // GET: Admin/Developer/Index
        public ActionResult Index()
        {
            ViewBag.ConfirmEmailError = TempData["ConfirmEmailError"];
            var users = _accountRepository.GetUsers();
            return View(users);
        }

        // GET: Admin/Developer/MakeDeveloperAdmin
        public ActionResult MakeDeveloperAdmin(string id)
        {
            _accountRepository.AssignRole(id, "Admin");
            return RedirectToAction("Index");
        }

        // GET: Admin/Developer/ConfirmDeveloperEMail
        public ActionResult ConfirmDeveloperEMail(string id)
        {
            var developer = _accountRepository.FindById(id);
            
            return View(developer);
        }

        // POST: Admin/Developer/ConfirmDeveloperEMail
        [HttpPost]
        public async Task<ActionResult> ConfirmDeveloperEMail(string id, FormCollection collection)
        {
            var token = await _accountRepository.GenerateEmailConfirmationTokenAsync(id);
            var result = await _accountRepository.ConfirmEmailAsync(id, token);

            if (!result.Succeeded)
            {
                TempData["ConfirmEmailError"] = "Could not confirm developer!";
            }
            return RedirectToAction("Index", "Developer");

        }

        // GET: Admin/Developer/Delete/5
        public ActionResult Delete(string id)
        {
            var developer = _accountRepository.GetUsers().Single(x => x.UserId == id);
            
            return View(developer);
        }

        // POST: Admin/Developer/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var user = _accountRepository.GetUsers().Single(x => x.UserId == id);
                var initiativeHeads = _initiativeBusiness.GetInitiativesByDeveloperOwner(user.UserName);
                var initiatives = initiativeHeads.Select(initiativeHead => _initiativeBusiness.GetInitiative(initiativeHead.Id)).ToList();

                foreach (var initiative in initiatives)
                {
                    if (initiative.DeveloperRoles.Any(x => (x.DeveloperName == user.UserName) && x.RoleName.Equals(RoleNameConstants.Administrator)))
                    {
                        initiative.DeveloperRoles.Single(x => (x.DeveloperName == user.UserName) && x.RoleName.Equals(RoleNameConstants.Administrator)).RoleName = RoleNameConstants.Deleted;
                        _initiativeBusiness.UpdateInitiative(initiative);
                    }
                }

                _accountRepository.DeleteUser(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}